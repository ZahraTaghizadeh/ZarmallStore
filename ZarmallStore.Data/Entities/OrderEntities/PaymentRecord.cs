using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.OrderEntities
{
    public class PaymentRecord : BaseEntity
    {
        /// شناسه تراکنش نوینو
        public string TransactionId { get; set; }

        /// شماره پیگیری بانک
        public string RefId { get; set; }

        /// شناسه دیجیتال تراکنش
        public string Authority { get; set; }

        /// شماره كارت پرداخت كننده بصورت Mask شده
        public string CardPan { get; set; }

        /// مبلغ (ریال)
        public int Amount { get; set; }

        /// شماره صورتحساب پذیرنده
        public long InvoiceId { get; set; }

        /// ای پی پرداخت کننده
        public string BuyerId { get; set; }

        /// زمان پرداخت 
        public DateTime PaymentTime { get; set; }
    }
}
