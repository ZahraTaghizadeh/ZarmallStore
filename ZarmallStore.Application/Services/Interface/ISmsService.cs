namespace ZarmallStore.Application.Services.Interface
{
    public interface ISmsService
    {
        Task SendVerificationSms(string mobile, string code);
    }
}
