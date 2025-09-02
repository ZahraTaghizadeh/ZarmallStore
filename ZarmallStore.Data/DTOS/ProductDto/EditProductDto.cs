using Microsoft.AspNetCore.Http;

namespace ZarmallStore.Data.DTOS.ProductDto
{
    public class EditProductDto
    {
        public long ProductId { get; set; }
        public string Title { get; set; }
        public bool IsAvailable { get; set; }
        public IFormFile MainImageName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public long BrandId { get; set; }
        public List<long> Categories { get; set; }
    }
    public enum EditProductResult
    {
        Success,
        Error,
        FileNotImage,
        BrandNotFound,
        CategoryNotFound
    }
}
