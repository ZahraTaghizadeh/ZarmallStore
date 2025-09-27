using ZarmallStore.Data.DTOs.Paging;
using ZarmallStore.Data.Entities.Account;

namespace ZarmallStore.Data.DTOs.Account
{
    public class FilterFavoriteProductDto : BasePaging
    {
        public string ProductTitle { get; set; }
        public long? UserId { get; set; }
        public List<FavoriteProduct> Data { get; set; }

        #region Methods
        public FilterFavoriteProductDto SetData(List<FavoriteProduct> data)
        {
            Data = data;
            return this;
        }

        public FilterFavoriteProductDto SetPaging(BasePaging paging)
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
