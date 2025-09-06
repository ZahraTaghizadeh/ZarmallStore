using ZarmallStore.Data.Entities.Common;

namespace ZarmallStore.Data.Entities.ProductEntities
{
    public class Product : BaseEntity
    {
        #region Properties
        public string Title { get; set; }
        public bool IsAvailable { get; set; }
        public string MainImageName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public ProductSelectedBrand? ProductSelectedBrand { get; set; }
        #endregion

        #region Relations
        public ICollection<ProductSelectedCategory> SelectedCategories { get; set; }
        public ICollection<ProductComment> ProductComments { get; set; }
        public ICollection<ProductVariant> productVariants { get; set; }
        public ICollection<ProductGallery> ProductGalleries { get; set; }
        public ICollection<ProductFeature> ProductFeatures { get; set; }
        #endregion
    }
}
