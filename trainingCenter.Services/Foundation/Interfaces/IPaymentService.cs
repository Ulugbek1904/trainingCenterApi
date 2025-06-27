using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using trainingCenter.Domain.Models;

namespace trainingCenter.Services.Foundation.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> RegisterPaymentAsync(Payment payment);
        Task NotifyPendingPaymentsAsync(Guid courseId);
        Task<List<Payment>> RetrieveAllPaymentsAsync();
        Task<Payment> RetrievePaymentByIdAsync(Guid paymentId);
        Task<Payment> ModifyPaymentAsync(Payment payment);
        Task<Payment> RemovePaymentAsync(Guid paymentId);
    }
}