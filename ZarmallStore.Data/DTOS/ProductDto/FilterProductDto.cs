using System.ComponentModel.DataAnnotations;
using ZarmallStore.Data.DTOs.Paging;
using ZarmallStore.Data.Entities.ProductEntities;

namespace ZarmallStore.Data.DTOs.ProductDto
{
    public class FilterProductDto : BasePaging
    {
        public string Title { get; set; }
        public long? BrandId { get; set; }
        public long? CategoryId { get; set; }
        public string? categoryUrl { get; set; }
        public long? ColorId { get; set; }
        public int MostPrice { get; set; }
        public int LeastPrice { get; set; }
        public int? StartPrice { get; set; }
        public int? EndPrice { get; set; }
        public FilterProductOrder ProductOrder { get; set; } = FilterProductOrder.Newest;
        public FilterProductStatus ProductStatus { get; set; }
        public List<Product> Data { get; set; }

        #region Methods
        public FilterProductDto SetData(List<Product> data)
        {
            Data = data;
            return this;
        }

        public FilterProductDto SetPaging(BasePaging paging)
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
    public enum FilterProductStatus
    {
        [Display(Name = "همه")]
        All,
        [Display(Name = "فعال")]
        Available,
        [Display(Name = "غیرفعال")]
        NotAvailable,
        [Display(Name = "موجود در انبار")]
        HasStockCount,
        [Display(Name = "نا موجود")]
        HasZeroStockCount
    }

    public enum FilterProductOrder
    {
        [Display(Name = "جدیدترین")]
        Newest,
        [Display(Name = "قدیمی ترین")]
        Oldest,
        [Display(Name = "گرانترین")]
        MostExpensive,
        [Display(Name = "ارزان ترین")]
        Cheapest,
    }
}
