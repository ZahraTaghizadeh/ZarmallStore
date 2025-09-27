using System.Text;
using ZarmallStore.Data.DTOs.PaymentDto;
using Newtonsoft.Json;

namespace ZarmallStore.Web.Areas.User.Services
{
    public class PaymentService : IPaymentService
    {
        #region CTOR
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public PaymentService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }
        #endregion

        public async Task<PaymentRequestResult?> CreatePayment(PaymentRequest paymentRequest)
        {
            var json = JsonConvert.SerializeObject(paymentRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_configuration.GetValue<string>("NovinoPayment:RequestPaymentUrl") , content);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<PaymentRequestResult>(responseString);
        }

        public async Task<PaymentVerificationResult?> VerifyPayment(PaymentVerification verification)
        {
            var json = JsonConvert.SerializeObject(verification);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_configuration.GetValue<string>("NovinoPayment:PaymentVerificationUrl"), content);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<PaymentVerificationResult>(responseString);
        }
    }
}
