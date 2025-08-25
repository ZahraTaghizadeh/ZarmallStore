using ZarmallStore.Application.Services.Interface;

namespace ZarmallStore.Application.Services.Implementarions
{
    public class SmsService : ISmsService
    {
        #region ApiKey
        private string apikey = "";
        #endregion
        public async Task SendVerificationSms(string mobile, string code)
        {
            var senderApi = new Kavenegar.KavenegarApi(apikey);
            await senderApi.VerifyLookup(mobile, code, "ZarmallStoreSmsVerification");
        }
    }
}
