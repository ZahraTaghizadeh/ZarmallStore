using System.ComponentModel.DataAnnotations;

namespace ZarmallStore.Data.DTOS.Account
{
    public class MobileActivationDTO : CaptchaDTO
    {
        public string ActivationCodePart1 { get; set; }
        public string ActivationCodePart2 { get; set; }
        public string ActivationCodePart3 { get; set; }
        public string ActivationCodePart4 { get; set; }
        public string ActivationCodePart5 { get; set; }

        public string? ReturnUrl { get; set; }
        public string Mobile { get; set; }
    }
    public enum ActivationResult
    {
        Success,
        Error
    }
}
