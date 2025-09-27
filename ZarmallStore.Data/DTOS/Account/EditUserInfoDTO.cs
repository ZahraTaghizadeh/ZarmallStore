using System.ComponentModel.DataAnnotations;

namespace ZarmallStore.Data.DTOs.Account
{
    public class EditUserInfoDTO : CaptchaDto
    {
        public long UserId { get; set; }
        public bool ReturnToCheckout { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string FullName { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.EmailAddress, ErrorMessage = "لطفا یک ایمیل معتبر وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string? Email { get; set; }

        [Display(Name = "آدرس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Address { get; set; }

        [Display(Name = "کد پستی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [MinLength(10, ErrorMessage = "{0} نمی تواند کمتر از {1} کاراکتر باشد")]
        public string PostCode { get; set; }

        public string UserCity { get; set; }
    }
}
