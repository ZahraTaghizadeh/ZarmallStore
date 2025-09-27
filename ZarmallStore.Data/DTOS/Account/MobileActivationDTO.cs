using System.ComponentModel.DataAnnotations;

namespace ZarmallStore.Data.DTOs.Account
{
    public class MobileActivationDTO : CaptchaDto
    {
        [Display(Name = "کد ورود")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(1, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string ActivationCodePart1 { get; set; }

        [Display(Name = "کد ورود")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(1, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string ActivationCodePart2 { get; set; }

        [Display(Name = "کد ورود")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(1, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string ActivationCodePart3 { get; set; }

        [Display(Name = "کد ورود")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(1, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string ActivationCodePart4 { get; set; }

        [Display(Name = "کد ورود")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(1, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string ActivationCodePart5 { get; set; }
        public string Mobile { get; set; }
        public string? ReturnUrl { get; set; }
    }

    public enum ActivationResult
    {
        Success,
        Error
    }
}
