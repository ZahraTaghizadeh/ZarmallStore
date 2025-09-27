using ZarmallStore.Application.Services.Interfaces;

namespace ZarmallStore.Application.Services.Implementations
{
    public class SmsService : ISmsService
    {
        #region Api Key
        private string apiKey = "6830636F2B31567A6A36547856574D50546449744E784A374C546874756F77565641377778435A6F6278303D";
        #endregion

        public async Task SendVerificationSms(string mobile, string code)
        {
            var senderApi = new Kavenegar.KavenegarApi(apiKey);
            await senderApi.VerifyLookup(mobile, code , "NouaShopAuth");
        }
    }
}
