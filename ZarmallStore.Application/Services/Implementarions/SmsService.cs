using ZarmallStore.Application.Services.Interface;

namespace ZarmallStore.Application.Services.Implementarions
{
    public class SmsService : ISmsService
    {
        #region ApiKey
        private string apikey = "6830636F2B31567A6A36547856574D50546449744E784A374C546874756F77565641377778435A6F6278303D";
        #endregion
        public async Task SendVerificationSms(string mobile, string code)
        {
            var senderApi = new Kavenegar.KavenegarApi(apikey);
            await senderApi.VerifyLookup(mobile, code, "ZarmallStoreSmsVerification");
        }
    }
}
