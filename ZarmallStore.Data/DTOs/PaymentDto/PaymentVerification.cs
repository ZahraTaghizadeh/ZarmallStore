namespace ZarmallStore.Data.DTOs.PaymentDto
{
    public class PaymentVerification
    {
        /// <summary>
        /// مرچنت (کد درگاه پرداخت) كه بعد از ثبت درگاه در پنل نوینو قابل دریافت است
        /// </summary>
        public string merchant_id { get; set; }

        /// <summary>
        /// مبلغ قابل پرداخت(ریال) (حداقل مبلغ 10.000 ریال حداكثر مبلغ 1.000.000.000 ریال)
        /// </summary>
        public int amount { get; set; }

        /// <summary>
        /// شناسه دیجیتال تراکنش
        /// </summary>
        public string authority { get; set; }
    }

    public class PaymentVerificationResult
    {
        /// <summary>
        /// در صورت موفقیت آمیز بودن برابر 100 در غیر این صورت عددی منفی میباشد كه در صورت منفی بودن, تفسیر آن در كدهای برگشتی / خطاها و پارامتر message قابل برسی می باشد
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 	تفسیر فارسی بر اساس status
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// object
        /// </summary>
        public PaymentVerificationResultData data { get; set; }

        /// <summary>
        /// صرفا در حالتی كه status برابر 1 -باشد، در غیر اینصورت برابر null خواهد بود
        /// </summary>
        public List<string> errors { get; set; }
    }

    public class PaymentVerificationResultData
    {
        /// <summary>
        /// شناسه تراکنش نوینو
        /// </summary>
        public string trans_id { get; set; }

        /// <summary>
        /// شماره پیگیری بانک
        /// </summary>
        public string ref_id { get; set; }

        /// <summary>
        /// شناسه دیجیتال تراکنش
        /// </summary>
        public string authority { get; set; }

        /// <summary>
        /// شماره كارت پرداخت كننده بصورت Mask شده
        /// </summary>
        public string card_pan { get; set; }

        /// <summary>
        /// مبلغ (ریال)
        /// </summary>
        public int amount { get; set; }

        /// <summary>
        /// شماره صورتحساب پذیرنده
        /// </summary>
        public string invoice_id { get; set; }

        /// <summary>
        /// ای پی پرداخت کننده
        /// </summary>
        public string buyer_ip { get; set; }

        /// <summary>
        /// زمان پرداخت (timestamp)
        /// </summary>
        public int payment_time { get; set; }
    }
}
