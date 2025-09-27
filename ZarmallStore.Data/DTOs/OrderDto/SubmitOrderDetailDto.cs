namespace ZarmallStore.Data.DTOs.OrderDto
{
    public class SubmitOrderDetailDto
    {
        public long ProductId { get; set; }
        public long UserId { get; set; }
        public long ProductVariantId { get; set; }
        public int Count { get; set; }
    }
}
