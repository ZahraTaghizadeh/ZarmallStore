using ZarmallStore.Data.DTOS.Paging;
using ZarmallStore.Data.DTOS.ProductCategoryDto;
using ZarmallStore.Data.Entities.ProductEntities;

namespace ZarmallStore.Data.DTOS.ProductDto
{
    public class FilterColorDto: BasePaging
    {
        public string Title { get; set; }
        public List<ProductColor> Data { get; set; }
        #region Methods
        public FilterColorDto SetData(List<ProductColor> data)
        {
            Data = data;
            return this;
        }

        public FilterColorDto SetPaging(BasePaging paging)
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
