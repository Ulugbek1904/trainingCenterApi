using AutoMapper;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models.DTOs.Tenant;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace trainingCenter.Services.Foundation;

public class TenantService : ITenantService
{
    private readonly IStorageBroker storageBroker;
    private readonly IMapper mapper;

    public TenantService(IStorageBroker storageBroker, IMapper mapper)
    {
        this.storageBroker = storageBroker;
        this.mapper = mapper;
    }

    public async Task<TenantDto> CreateTenantAsync(TenantCreateDto dto)
    {
        var tenantId = Guid.NewGuid();

        var tenant = mapper.Map<Tenant>(dto);
        tenant.Id = tenantId;

        var botSetting = new TelegramBotSetting
        {
            TenantId = tenantId,
            BotToken = dto.TelegramBotToken
        };

        await storageBroker.InsertAsync(tenant);
        await storageBroker.InsertAsync(botSetting);

        return mapper.Map<TenantDto>(tenant);
    }


    public async Task<TenantDto> UpdateTenantAsync(Guid Id, TenantUpdateDto dto)
    {
        var tenant = await storageBroker.SelectByIdAsync<Tenant>(Id);
        if (tenant == null)
            throw new NotFoundException($"Tenant with ID {Id} not found.");

        mapper.Map(dto, tenant);
        await storageBroker.UpdateAsync(tenant);

        var botSetting = await storageBroker
            .SelectAll<TelegramBotSetting>()
            .FirstOrDefaultAsync(x => x.TenantId == Id);

        if (botSetting != null)
        {
            botSetting.BotToken = dto.TelegramBotToken;
            await storageBroker.UpdateAsync(botSetting);
        }
        else
        {
            var newSetting = new TelegramBotSetting
            {
                TelegramBotId = Guid.NewGuid(),
                TenantId = Id,
                BotToken = dto.TelegramBotToken
            };
            await storageBroker.InsertAsync(newSetting);
        }

        return mapper.Map<TenantDto>(tenant);
    }


    public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync()
    {
        var tenants = await storageBroker
            .SelectAll<Tenant>().ToListAsync();

        return mapper.Map<IEnumerable<TenantDto>>(tenants) ;
    }

    public async Task<TenantDto> GetTenantByIdAsync(Guid id)
    {
        var tenant = await storageBroker.SelectByIdAsync<Tenant>(id)
                     ?? throw new NotFoundException("Tenant not found");

        return mapper.Map<TenantDto>(tenant);
    }

    public async Task<bool> DeleteTenantAsync(Guid id)
    {
        var tenant = await storageBroker.SelectByIdAsync<Tenant>(id)
                     ?? throw new NotFoundException("Tenant not found");

        await storageBroker.DeleteAsync(tenant);
        return true;
    }

    public async Task<bool> ToggleTenantStatusAsync(Guid id)
    {
        var tenant = await storageBroker.SelectByIdAsync<Tenant>(id)
                     ?? throw new NotFoundException("Tenant not found");

        tenant.IsActive = !tenant.IsActive;
        await storageBroker.UpdateAsync(tenant);
        return tenant.IsActive;
    }
}


