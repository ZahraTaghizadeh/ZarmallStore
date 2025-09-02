using Microsoft.AspNetCore.Http;

namespace ZarmallStore.Data.DTOS.ProductDto
{
    public class CreateProductDto
    {
        public string Title { get; set; }
        public bool IsAvailable { get; set; }
        public IFormFile MainImageName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public long BrandId { get; set; }
        public List<long> Categories { get; set; }
        public List<IFormFile> ProductGalleries { get; set; }
    }
    public enum CreateProductResult
    {
        Success,
        Error,
        FileNotImage,
        BrandNotFound,
        CategoryNotFound
    }
}
