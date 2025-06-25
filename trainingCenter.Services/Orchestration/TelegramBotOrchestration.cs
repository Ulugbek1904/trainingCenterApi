using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using trainingCenter.Common.Exceptions;
using trainingCenter.Infrastructure.providers.TelegramProvider;
using trainingCenter.Services.Foundation.Interfaces;
using trainingCenter.Services.Orchestration.Interfaces;

public class TelegramBotOrchestrator : ITelegramBotOrchestration
{
    private readonly ITelegramBotProvider telegramBotProvider;
    private readonly ITelegramBotService telegramService;
    private readonly ILogger<TelegramBotOrchestrator> logger;
    private const string TrainingCenterName = "Jumaboyev Ulug'bek";

    public TelegramBotOrchestrator(
        ITelegramBotProvider telegramBotProvider,
        ITelegramBotService telegramService,
        ILogger<TelegramBotOrchestrator> logger)
    {
        this.telegramBotProvider = telegramBotProvider ??
            throw new ArgumentNullException(nameof(telegramBotProvider));
        this.telegramService = telegramService ??
            throw new ArgumentNullException(nameof(telegramService));
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await telegramBotProvider
            .StartReceivingAsync(ProcessUpdateAsync, cancellationToken);
    }

    public async Task ProcessUpdateAsync(Update update)
    {
        if (update == null || update.Message == null)
            return;

        var chatId = update.Message.Chat.Id.ToString();
        var telegramId = update.Message.From?.Id.ToString();
        var messageText = update.Message.Text;
        var name = update.Message.From?.FirstName;

        if (string.IsNullOrWhiteSpace(telegramId))
        {
            this.logger.LogError("User Telegram ID cannot be retrieved.");
            return;
        }

        this.logger.LogInformation($"Received message: {messageText} from {telegramId}");

        if (update.Message.Contact != null)
        {
            var phoneNumber = update.Message.Contact.PhoneNumber;
            try
            {
                await telegramService.RegisterParentAsync(telegramId, phoneNumber);
                await telegramBotProvider.SendMessageAsync(chatId,
                    "Telefon raqami muvaffaqiyatli ro'yxatdan o'tdi!",
                    replyMarkup: new ReplyKeyboardRemove());
            }
            catch (NotFoundException ex)
            {
                await telegramBotProvider.SendMessageAsync(chatId,
                    $"Xato: {ex.Message} Iltimos, to'g'ri telefon raqamini yuboring.\n " +
                    $"Yoki farzandingiz boshqa telefon nomerni ro'yhatdan o'tkazgan");
            }
            return;
        }

        if (messageText == "/start")
        {
            await telegramBotProvider.SendMessageAsync(chatId,
                $"Assalom alaykum {name}\r\n {TrainingCenterName} rasmiy botiga xush kelibsiz!\n" +
                $"Bu bot sizga o'quv markazidagi o'quv jarayoni, " +
                $"to'lovlar va bildirishnomalar haqida ma'lumot beradi.\n");
            return; 
        }
        else if (messageText == "/register")
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                KeyboardButton.WithRequestContact("📱 Kontaktni yuborish")
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
            await telegramBotProvider.SendMessageAsync(chatId,
                "Iltimos, telefon raqamingizni 'Kontaktni yuborish' orqali yuboring.",
                replyMarkup: keyboard);
            return; 
        }
        else
        {
            await telegramBotProvider.SendMessageAsync(chatId,
                "Noto'g'ri buyruq. Iltimos, /start yoki /register buyruq kiriting.");
        }
    }
}