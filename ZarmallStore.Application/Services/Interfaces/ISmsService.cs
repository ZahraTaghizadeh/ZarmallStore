namespace ZarmallStore.Application.Services.Interfaces
{
    public interface ISmsService
    {
        Task SendVerificationSms(string mobile, string code);
    }
}
