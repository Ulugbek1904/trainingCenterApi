using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Infrastructure.providers.TelegramProvider;
using trainingCenter.Services.Foundation.Interfaces;
using trainingCenter.Services.Orchestration.Interfaces;

namespace trainingCenter.Services.Orchestration
{
    public class TelegramBotOrchestration : ITelegramBotOrchestration
    {
        private readonly ITelegramBotService telegramBotService;
        private readonly ILogger<TelegramBotOrchestration> logger;
        private readonly IStorageBroker storageBroker;
        private readonly ITelegramBotProvider provider;
        private readonly IMemoryCache cache;

        public TelegramBotOrchestration(
            ITelegramBotService telegramBotService,
            ILogger<TelegramBotOrchestration> logger,
            IStorageBroker storageBroker,
            ITelegramBotProvider provider,
            IMemoryCache cache)
        {
            this.telegramBotService = telegramBotService;
            this.logger = logger;
            this.storageBroker = storageBroker;
            this.provider = provider;
            this.cache = cache;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task ProcessUpdateAsync(Update update, Guid tenantId)
        {
            if (update == null || update.Message == null)
            {
                logger.LogWarning("[Bot] Received null update or message.");
                return;
            }

            var telegramId = update.Message.From?.Id.ToString();
            var chatId = update.Message.Chat.Id.ToString();
            var messageText = update.Message.Text;

            if (string.IsNullOrWhiteSpace(telegramId))
            {
                logger.LogWarning("[Bot] Telegram ID is empty.");
                return;
            }

            logger.LogInformation($"[Bot] Processing update. TenantId: {tenantId}, TelegramId: {telegramId}, ChatId: {chatId}, Message: {messageText}");

            // Check if the user is in the process of registering
            if (cache.TryGetValue($"Register_{telegramId}", out Guid cachedTenantId))
            {
                if (update.Message.Contact != null)
                {
                    // Handle contact sharing
                    var phoneNumber = update.Message.Contact.PhoneNumber;
                    cache.Remove($"Register_{telegramId}");
                    await telegramBotService.HandleRegisterPhoneNumberAsync(tenantId, telegramId, phoneNumber);
                    return;
                }
                else if (!string.IsNullOrWhiteSpace(messageText))
                {
                    // Handle manual phone number input
                    cache.Remove($"Register_{telegramId}");
                    await telegramBotService.HandleRegisterPhoneNumberAsync(tenantId, telegramId, messageText);
                    return;
                }
            }

            if (messageText == "/register")
            {
                // Store registration state
                cache.Set($"Register_{telegramId}", tenantId, TimeSpan.FromMinutes(5));
                var keyboard = new ReplyKeyboardMarkup(new KeyboardButton("📞 Kontaktni ulashish")
                {
                    RequestContact = true
                })
                {
                    ResizeKeyboard = true,
                    OneTimeKeyboard = true
                };
                await provider.SendMessageAsync(
                    tenantId: tenantId,
                    chatId: chatId,
                    message: "Iltimos, telefon raqamingizni kiriting (masalan, +998901234567) yoki 'Kontaktni ulashish' tugmasini bosing:",
                    replyMarkup: keyboard);
                return;
            }

            var student = await storageBroker.SelectAll<Student>()
                .Where(s => s.ParentTelegramId == telegramId && s.TenantId == tenantId)
                .FirstOrDefaultAsync();

            if (student == null)
            {
                logger.LogWarning($"[Bot] No student found for TelegramId: {telegramId}, TenantId: {tenantId}");
                await provider.SendMessageAsync(
                    tenantId: tenantId,
                    chatId: chatId,
                    message: "Siz ushbu tashkilotda ro‘yxatdan o‘tmagansiz. Iltimos, /register buyrug‘i orqali ro‘yxatdan o‘ting.");
                return;
            }

            switch (messageText)
            {
                case "/start":
                    await telegramBotService.SendMenuAsync(tenantId, telegramId);
                    break;

                case "/report":
                    await telegramBotService.SendReportMenuAsync(tenantId, telegramId);
                    break;

                default:
                    await telegramBotService.SendHelpAsync(tenantId, telegramId);
                    break;
            }
        }
    }
}