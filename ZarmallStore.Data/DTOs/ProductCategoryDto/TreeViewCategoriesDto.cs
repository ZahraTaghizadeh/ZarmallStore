using ZarmallStore.Data.Entities.ProductEntities;

namespace ZarmallStore.Data.DTOs.ProductCategoryDto
{
    public class TreeViewCategoriesDto
    {
        // Create & Edit Categories 
        public List<ProductCategory> productCategories { get; set; }
        public int Level { get; set; }
        public long? ThisCategoryId { get; set; }

        // Select Product Categories
        public List<long>? SelectedCategoriesIds { get; set; }
    }
}
