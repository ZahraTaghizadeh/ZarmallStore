namespace ZarmallStore.Data.DTOS.ProductDto
{
    public class EditFeaturesDto
    {
        public long FeatureId { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
    }
}
