using ZarmallStore.Application.Services.Interfaces;

namespace ZarmallStore.Application.Services.Implementations
{
    public class SmsService : ISmsService
    {
        #region Api Key
        private string apiKey = "6A75357657354656646453684F396541356635564E2B6770627A3541786159524C45585867643048304E6F3D";
        #endregion

        public async Task SendVerificationSms(string mobile, string code)
        {
            var senderApi = new Kavenegar.KavenegarApi(apiKey);
            await senderApi.VerifyLookup(mobile, code , "NouaShopAuth");
        }
    }
}
