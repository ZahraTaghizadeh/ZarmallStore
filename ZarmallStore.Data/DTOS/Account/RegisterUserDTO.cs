using System.ComponentModel.DataAnnotations;

namespace ZarmallStore.Data.DTOs.Account
{
    public class RegisterUserDTO : CaptchaDto
    {
        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(11, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [MinLength(11, ErrorMessage = "{0} نمی تواند کمتر از {1} کاراکتر باشد")]
        public string MobileNumber { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
