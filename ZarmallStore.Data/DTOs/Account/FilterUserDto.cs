using ZarmallStore.Data.DTOs.Paging;
using ZarmallStore.Data.Entities.Account;
using System.ComponentModel.DataAnnotations;

namespace ZarmallStore.Data.DTOs.Account
{
    public class FilterUserDto : BasePaging
    {
        public long? UserId { get; set; }
        public string Mobile { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public FilterUserState UserState { get; set; }
        public List<User> Data { get; set; }

        #region Counts
        public int UserCount { get; set; }
        #endregion

        #region methods
        public FilterUserDto SetData(List<User> data)
        {
            Data = data;
            return this;
        }

        public FilterUserDto SetPaging(BasePaging paging)
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

    public enum FilterUserState
    {
        [Display(Name = "همه")]
        All,
        [Display(Name = "بلاک شده")]
        BLocked,
    }
}
