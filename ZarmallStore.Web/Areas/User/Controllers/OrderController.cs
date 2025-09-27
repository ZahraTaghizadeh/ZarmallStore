using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Data.DTOs.OrderDto;
using ZarmallStore.Web.UserExtensions;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Areas.User.Controllers
{
    public class OrderController : UserBaseController
    {
        #region CTOR
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public OrderController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        #endregion

        #region Filter Orders
        [HttpGet("orders")]
        public async Task<IActionResult> FilterOrders(FilterOrderDto filter, FilterOrderState? state)
        {
            filter.FilterOrderState = state ?? FilterOrderState.All;
            filter.UserId = User.GetUserId();
            var model = await _orderService.FilterOrders(filter);
            return View(model);
        }
        #endregion

        #region Paid Order Detail
        [HttpGet("order-{orderId}")]
        public async Task<IActionResult> OrderDetail(long orderId)
        {
            var model = await _orderService.OrderDetail(orderId);
            return View(model);
        }
        #endregion

        #region Add Product To Cart

        [HttpPost("add-product-to-cart"), ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProductToCart(SubmitOrderDetailDto dto)
        {
            await _orderService.AddProductToOrder(dto);

            TempData[SuccessMessage] = "محصول به سبد خرید اضافه شد";
            return RedirectToAction("ProductDetail", "Product", new { area = "", productId = dto.ProductId });
        }
        #endregion

        #region Cart
        [HttpGet("cart")]
        public async Task<IActionResult> Cart()
        {
            var order = await _orderService.UserOpenOrderDetail(User.GetUserId());
            return View(order);
        }
        #endregion

        #region Checkout
        [HttpGet("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var model = await _orderService.UserOpenOrderDetail(User.GetUserId());
            ViewData["User"] = await _userService.GetUserById(User.GetUserId());
            return View(model);
        }
        #endregion

        #region Change Order Detail Count
        [HttpPost("change-order-detail-count")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeOrderDetailCount(long orderDetailId, int newCount)
        {
            await _orderService.ChangeOrderDetailCount(orderDetailId, newCount);
            var model = await _orderService.UserOpenOrderDetail(User.GetUserId());
            return PartialView("_CartContentPartial", model);
        }
        #endregion

        #region Delete Order Detail
        [HttpPost("delete-order-item")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrderDetail(long orderDetailId)
        {
            await _orderService.RemoveOrderDetail(orderDetailId);
            var model = await _orderService.UserOpenOrderDetail(User.GetUserId());
            return model == null ? PartialView("_CartContentPartial", null) : PartialView("_CartContentPartial", model);
        }
        #endregion
    }
}
