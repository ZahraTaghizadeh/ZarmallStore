using System.ComponentModel.DataAnnotations;
namespace ZarmallStore.Data.DTOS.Account
{
    public class RegisterUserDTO
    {
        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "لطفا {.} را وارد کنید")]
        [MaxLength(11,ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [MinLength(11, ErrorMessage = "{0} نمی تواند بیشتر از {1}  باشد")]
        public string MobileNumber { get; set; }
        public string? ReturnUrl { get; set; }
    }

}
