namespace ZarmallStore.Data.DTOs.ProductDto
{
    public class EditFeaturesDto
    {
        public long FeatureId { get; set; }
        public long ProductId { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
    }
}
