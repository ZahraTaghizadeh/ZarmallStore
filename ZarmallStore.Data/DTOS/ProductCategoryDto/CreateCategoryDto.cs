using ZarmallStore.Data.Entities.ProductEntities;

namespace ZarmallStore.Data.DTOS.ProductCategoryDto
{
    public class CreateCategoryDto
    {
        public long? ParentId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
    }
}
