using System.ComponentModel.DataAnnotations;
using ZarmallStore.Data.Entities.ProductEntities;

namespace ZarmallStore.Data.DTOS.ProductDto
{
    public class ProductDetailDto
    {
        [Key]
        public long Id { get; set; }
        public DateTime CreatDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool IsDeleted { get; set; }
        public string Title { get; set; }
        public bool IsAvailable { get; set; }
        public string MainImageName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public ProductSelectedBrand ProductSelectedBrand { get; set; }

        public List<ProductSelectedCategory> SelectedCategories { get; set; }
        public List<ProductComment> ProductComments { get; set; }
        public List<ProductVariant> productVariants { get; set; }
        public List<ProductGallery> ProductGalleries { get; set; }
        public List<ProductFeature> ProductFeatures { get; set; }

    }
}
