using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models;
using trainingCenter.Domain.Models.DTOs;
using trainingCenter.Services.Foundation.Interfaces;
using ArgumentException = trainingCenter.Common.Exceptions.ArgumentException;

namespace trainingCenter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService paymentService;
        private readonly IMapper mapper;

        public PaymentsController(IPaymentService paymentService, IMapper mapper)
        {
            this.paymentService = paymentService ?? throw new NullArgumentException(nameof(paymentService));
            this.mapper = mapper ?? throw new NullArgumentException(nameof(mapper));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto paymentDto)
        {
            try
            {
                var payment = mapper.Map<Payment>(paymentDto);
                var createdPayment = await paymentService.RegisterPaymentAsync(payment);
                var resultDto = mapper.Map<PaymentDto>(createdPayment);
                return CreatedAtAction(nameof(GetPaymentById), new { id = resultDto.Id }, resultDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if (page < 1 || size < 1)
                return BadRequest("Page and size must be positive.");

            var payments = await paymentService.RetrieveAllPaymentsAsync();
            var totalCount = payments.Count;
            var pagedPayments = payments.Skip((page - 1) * size).Take(size).ToList();
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
            var resultDto = mapper.Map<PaymentDto>(payment);
            return Ok(resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(Guid id, [FromBody] PaymentUpdateDto paymentDto)
        {
            if (id != paymentDto.Id)
                throw new ArgumentException("ID mismatch.");

            var payment = mapper.Map<Payment>(paymentDto);
            var updatedPayment = await paymentService.ModifyPaymentAsync(payment);
            var resultDto = mapper.Map<PaymentDto>(updatedPayment);
            return Ok(resultDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(Guid id)
        {
            await paymentService.RemovePaymentAsync(id);
            return NoContent();
        }
    }
}