using System.ComponentModel.DataAnnotations;

namespace ZarmallStore.Data.DTOS.Account
{
    public class EditUserInfoDTO
    {
        public long UserId { get; set; }
        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {.} را وارد کنید")]
        [MaxLength(12, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string? FullName { get; set; }
        [Display(Name = "ایمیل")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string? Email { get; set; }
        [Display(Name = "آدرس")]
        [Required(ErrorMessage = "لطفا {.} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Address { get; set; }
        [Display(Name = "کد پستی")]
        [Required(ErrorMessage = "لطفا {.} را وارد کنید")]
        [MaxLength(10, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [MinLength(10,ErrorMessage = "{0} نمی تواند بیشتر از {1}  باشد")]
        public string PostCode { get; set; }
    }
}
