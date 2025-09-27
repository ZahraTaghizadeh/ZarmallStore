using ZarmallStore.Data.DTOs.PaymentDto;

namespace ZarmallStore.Web.Areas.User.Services
{
    public interface IPaymentService
    {
        Task<PaymentRequestResult?> CreatePayment(PaymentRequest paymentRequest);
        Task<PaymentVerificationResult?> VerifyPayment(PaymentVerification verification);
    }
}
