using ZarmallStore.Data.DTOs.Paging;
using ZarmallStore.Data.Entities.OrderEntities;
using System.ComponentModel.DataAnnotations;

namespace ZarmallStore.Data.DTOs.OrderDto
{
    public class FilterOrderDto : BasePaging
    {
        public long? UserId { get; set; }
        public int? MinimumPrice { get; set; }
        public string UserName { get; set; }
        public string DestinationCity { get; set; }
        public string Description { get; set; }
        public string TraceCode { get; set; }
        public long? PaymentRecordId { get; set; }
        public FilterOrderState FilterOrderState { get; set; }
        public List<Order> Data { get; set; }

        #region Methods
        public FilterOrderDto SetData(List<Order> data)
        {
            Data = data;
            return this;
        }

        public FilterOrderDto SetPaging(BasePaging paging)
        {
            PageId = paging.PageId;
            AllEntitiesCount = paging.AllEntitiesCount;
            StartPage = paging.StartPage;
            EndPage = paging.EndPage;
            HowManyShowPageAfterAndBefore = paging.HowManyShowPageAfterAndBefore;
            TakeEntity = paging.TakeEntity;
            SkipEntity = paging.SkipEntity;
            PageCount = paging.PageCount;
            return this;
        }
        #endregion
    }
    public enum FilterOrderState
    {
        [Display(Name = "همه")]
        All,
        [Display(Name = "ثبت شده")]
        Open,
        [Display(Name = "پرداخت شده")]
        Paid,
        [Display(Name = "ارسال شده")]
        Sent,
        [Display(Name = "لغو شده")]
        Canceled,
        [Display(Name = "موجوع شده")]
        Returned,
        [Display(Name = "در حال پردازش")]
        Pending
    }
}
