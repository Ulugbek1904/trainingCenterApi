using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Secretary)},{nameof(Role.Teacher)}")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService paymentService;
    private readonly ICurrentUserService currentUser;
    private readonly IMapper mapper;

    public PaymentsController(
        IPaymentService paymentService,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        this.paymentService = paymentService ?? throw new NullArgumentException(nameof(paymentService));
        this.currentUser = currentUser ?? throw new NullArgumentException(nameof(currentUser));
        this.mapper = mapper ?? throw new NullArgumentException(nameof(mapper));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto paymentDto)
    {
        var payment = mapper.Map<Payment>(paymentDto);
        payment.TenantId = currentUser.TenantId;

        var createdPayment = await paymentService.RegisterPaymentAsync(payment);
        var resultDto = mapper.Map<PaymentDto>(createdPayment);
        return CreatedAtAction(nameof(GetPaymentById), new { id = resultDto.Id }, resultDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPayments([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        if (page < 1 || size < 1)
            return BadRequest("Page and size must be positive.");

        var payments = await paymentService.RetrieveAllPaymentsAsync();
        var filteredPayments = payments
            .Where(p => p.TenantId == currentUser.TenantId)
            .ToList();

        var totalCount = filteredPayments.Count;
        var pagedPayments = filteredPayments
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        var resultDtos = mapper.Map<List<PaymentDto>>(pagedPayments);

        var result = new PagedResult<PaymentDto>
        {
            Items = resultDtos,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = size
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(Guid id)
    {
        var payment = await paymentService.RetrievePaymentByIdAsync(id);

        if (payment.TenantId != currentUser.TenantId)
            return Forbid("Siz boshqa o‘quv markazining to‘loviga kira olmaysiz.");

        var resultDto = mapper.Map<PaymentDto>(payment);
        return Ok(resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePayment(Guid id, [FromBody] PaymentUpdateDto paymentDto)
    {
        if (id != paymentDto.Id)
            return BadRequest("ID mos emas.");

        var existing = await paymentService.RetrievePaymentByIdAsync(id);
        if (existing.TenantId != currentUser.TenantId)
            return Forbid("Siz boshqa o‘quv markazining to‘lovini yangilay olmaysiz.");

        var payment = mapper.Map<Payment>(paymentDto);
        payment.TenantId = currentUser.TenantId;

        var updatedPayment = await paymentService.ModifyPaymentAsync(payment);
        var resultDto = mapper.Map<PaymentDto>(updatedPayment);
        return Ok(resultDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePayment(Guid id)
    {
        var payment = await paymentService.RetrievePaymentByIdAsync(id);
        if (payment.TenantId != currentUser.TenantId)
            return Forbid("Siz boshqa o‘quv markazining to‘lovini o‘chira olmaysiz.");

        await paymentService.RemovePaymentAsync(id);
        return NoContent();
    }
}
