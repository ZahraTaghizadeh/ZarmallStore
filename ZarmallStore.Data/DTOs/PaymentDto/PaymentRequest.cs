namespace ZarmallStore.Data.DTOs.PaymentDto
{
    public class PaymentRequest
    {
        /// <summary>
        /// مرچنت (کد درگاه پرداخت) كه بعد از ثبت درگاه در پنل نوینو قابل دریافت است
        /// </summary>
        public string merchant_id { get; set; }

        /// <summary>
        /// مبلغ قابل پرداخت(ریال) (حداقل مبلغ 10.000 ریال حداكثر مبلغ 1.000.000.000 ریال)
        /// </summary>
        public decimal amount { get; set; }

        /// <summary>
        /// پس از بازگشت خریدار از درگاه پرداخت برخی اطلاعات بصورت POST یا query string (وابسته به مقدار callback_method ) به این آدرس ارسال خواهد شد
        /// </summary>
        public string callback_url { get; set; }

        /// <summary>
        /// متد جهت فراخوانی آدرس بازگشتی صرفا مقدار POST یا GET پیش فرض GET( توصیه میشود)
        /// </summary>
        public string? callback_method { get; set; }

        /// <summary>
        /// 	شماره فاکتور داخلی پذیرنده
        /// </summary>
        public string? invoice_id { get; set; }

        /// <summary>
        /// توضیحات پذیرنده
        /// </summary>
        public string? description { get; set; }

        /// <summary>
        /// ایمیل خریدار
        /// </summary>
        public string? email { get; set; }

        /// <summary>
        /// تلفن همراه خریدار (در صورت عدم ارسال؛ آمار تحلیلی پنل کاربری و شماره کارت های ذخیره شده پرداخت کننده نمایش داده نخواهد شد)
        /// </summary>
        public string? mobile { get; set; }

        /// <summary>
        /// نام پرداخت کننده
        /// </summary>
        public string? name { get; set; }

        /// <summary>
        /// شماره کارت پرداخت کننده 16 رقم بدون – جهت الزام پرداخت كننده به پرداخت با كارت مشخص
        /// </summary>
        public string? card_pan { get; set; }
    }

    public class PaymentRequestResult
    {
        /// <summary>
        /// در صورت موفقیت آمیز بودن برابر 100 در غیر این صورت عددی منفی میباشد كه در صورت منفی بودن, تفسیر آن در كدهای برگشتی / خطاها و پارامتر message قابل برسی می باشد
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// تفسیر فارسی بر اساس status
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// مقدار object داخلی
        /// </summary>
        public PaymentRequestResultData data { get; set; }

        /// <summary>
        /// صرفا در حالتی كه status برابر 1 -باشد، در غیر اینصورت برابر null خواهد بود
        /// </summary>
        public List<string> errors { get; set; }
    }

    public class PaymentRequestResultData
    {
        /// <summary>
        /// کارمزد (ریال)
        /// </summary>
        public int wage { get; set; }

        /// <summary>
        /// پرداخت کننده کارمزد (merchant, customer)
        /// </summary>
        public string wage_payer { get; set; }

        /// <summary>
        /// شناسه دیجیتال تراکنش
        /// </summary>
        public string authority { get; set; }

        /// <summary>
        /// شناسه تراکنش نوینو
        /// </summary>
        public int trans_id { get; set; }

        /// <summary>
        /// لینک جهت ارجاع کاربر به درگاه
        /// </summary>
        public string payment_url { get; set; }
    }
}
