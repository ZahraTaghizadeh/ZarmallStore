using Microsoft.AspNetCore.Http;

namespace ZarmallStore.Data.DTOs.ProductDto
{
    public class EditBrandDto
    {
        public long BranId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public IFormFile? ImageName { get; set; }
        public int Order { get; set; }
    }
}
