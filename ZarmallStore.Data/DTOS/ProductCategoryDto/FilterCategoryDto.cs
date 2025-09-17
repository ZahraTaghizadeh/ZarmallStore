using ZarmallStore.Data.DTOS.Paging;
using ZarmallStore.Data.Entities.ProductEntities;

namespace ZarmallStore.Data.DTOS.ProductCategoryDto
{
    public class FilterCategoryDto : BasePaging
    {
        public long? CategoryId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public List<ProductCategory> Data { get; set; }
        #region Methods
        public FilterCategoryDto SetData(List<ProductCategory> data)
        {
            Data = data;
            return this;
        }

        public FilterCategoryDto SetPaging(BasePaging paging)
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
}
