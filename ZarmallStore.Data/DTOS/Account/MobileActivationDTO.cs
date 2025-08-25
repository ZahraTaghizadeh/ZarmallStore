using System.ComponentModel.DataAnnotations;

namespace ZarmallStore.Data.DTOS.Account
{
    public class MobileActivationDTO
    {
        [Display(Name = "کد فعالسازی")]
        [Required(ErrorMessage = "لطفا {.} را وارد کنید")]
        [MaxLength(5, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [MinLength(5, ErrorMessage = "{0} نمی تواند بیشتر از {1}  باشد")]
        public string ActivationCode { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
