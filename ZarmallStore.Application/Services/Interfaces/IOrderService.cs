using ZarmallStore.Data.DTOs.OrderDto;
using ZarmallStore.Data.DTOs.PaymentDto;
using ZarmallStore.Data.Entities.OrderEntities;

namespace ZarmallStore.Application.Services.Interfaces
{
    public interface IOrderService : IAsyncDisposable
    {
        Task<FilterOrderDto> FilterOrders(FilterOrderDto filter);
        Task<OrderDetailDto> OrderDetail(long orderId);
        Task<Order> GetOrderById(long orderId);
        Task<int> GetOrderTotalPrice(long orderId);
        Task<int> UpdateOrderDetailPrices(long orderId);
        Task<OpenOrderDto?> UserOpenOrderDetail(long userId);
        Task<Order?> GetUserOpenOrder(long userId);
        Task<bool> CheckOrderDetailExist(long orderId , long variantId);
        Task<ProcessOrderDto> GetProcessOrder(long orderId);
        Task<long> AddOrderForUser(long userId);
        Task AddProductToOrder(SubmitOrderDetailDto dto);
        Task ChangeOrderDetailCount(long orderDetailId, int count);
        Task RemoveOrderDetail(long orderDetailId);
        Task ProcessOrder(ProcessOrderDto dto);
        Task PayOrderPrice(PaymentVerificationResultData dto);
        Task DeleteOrder(long orderId);
    }
}
