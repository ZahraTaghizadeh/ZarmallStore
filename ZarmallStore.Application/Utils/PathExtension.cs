namespace ZarmallStore.Application.Utils
{
    public static class PathExtension
    {

        #region ProductImage
        public static string ProductImageImage = "/content/images/ProductImage/origin/";

        public static string ProductImageServer =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/ProductImage/origin/");

        public static string ProductImageThumb = "/content/images/ProductImage/thumb/";

        public static string ProductImageThumbServer =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/ProductImage/thumb/");
        #endregion

        #region Category
        public static string CategoryImage = "/content/images/Category/origin/";

        public static string CategoryServer =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/Category/origin/");

        public static string CategoryThumb = "/content/images/Category/thumb/";

        public static string CategoryThumbServer =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/Category/thumb/");
        #endregion

        #region ProductGallery
        public static string ProductGalleryImage = "/content/images/ProductGallery/origin/";

        public static string ProductGalleryServer =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/ProductGallery/origin/");

        public static string ProductGalleryThumb = "/content/images/ProductGallery/thumb/";

        public static string ProductGalleryThumbServer =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/ProductGallery/thumb/");
        #endregion

        #region Banner
        public static string BannerImage = "/content/images/Banner/origin/";

        public static string BannerServer =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/Banner/origin/");

        public static string BannerThumb = "/content/images/Banner/thumb/";

        public static string BannerThumbServer =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/Banner/thumb/");
        #endregion

        #region Brand
        public static string BrandImage = "/content/images/Brand/origin/";

        public static string BrandServer =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/Brand/origin/");

        public static string BrandThumb = "/content/images/Brand/thumb/";

        public static string BrandThumbServer =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/Brand/thumb/");
        #endregion
    }
}
