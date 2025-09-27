using ZarmallStore.Data.DTOs.Paging;
using ZarmallStore.Data.Entities.ProductEntities;

namespace ZarmallStore.Data.DTOs.ProductDto
{
    public class FilterBrandDto : BasePaging
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public List<Brand> Data { get; set; }

        #region Methods
        public FilterBrandDto SetData(List<Brand> data)
        {
            Data = data;
            return this;
        }

        public FilterBrandDto SetPaging(BasePaging paging)
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
