using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Data.DTOs.OrderDto;
using ZarmallStore.Web.Accessibility;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Areas.Admin.Controllers
{
    [AccessChecker(5)]
    public class OrderController : AdminBaseController
    {
        #region CTOR
        private readonly IOrderService _orderService;
        private readonly ICommonService _commonService;

        public OrderController(IOrderService orderService, ICommonService commonService)
        {
            _orderService = orderService;
            _commonService = commonService;
        }

        #endregion

        #region Filter
        [HttpGet("admin-filter-orders")]
        public async Task<IActionResult> FilterOrders(FilterOrderDto filter)
        {
            var model = await _orderService.FilterOrders(filter);
            return View(model);
        }
        #endregion

        #region Detail
        [HttpGet("admin-order-detail-{orderId}")]
        public async Task<IActionResult> OrderDetail(long orderId)
        {
            var model = await _orderService.OrderDetail(orderId);
            return View(model);
        }
        #endregion

        #region Factor
        [HttpGet("print-{orderId}")]
        public async Task<IActionResult> PrintOrder(long orderId)
        {
            var model = await _orderService.OrderDetail(orderId);
            ViewData["SiteInfo"] = await _commonService.GetSiteInfo();
            return View(model);
        }
        #endregion

        #region Process
        [HttpGet("process-order")]
        public async Task<IActionResult> ProcessOrder(long orderId)
        {
            var model = await _orderService.GetProcessOrder(orderId);
            return View(model);
        }

        [HttpPost("process-order") , ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessOrder(ProcessOrderDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _orderService.ProcessOrder(dto);
            TempData[SuccessMessage] = SuccessText;
            return RedirectToAction("OrderDetail" , new {area = "Admin" , orderId = dto.OrderId});
        }
        #endregion

        #region Delete
        [Route("delete-order")]
        public async Task<IActionResult> DeleteOrder(long orderId)
        {
            await _orderService.DeleteOrder(orderId);
            TempData[SuccessMessage] = DeleteText;
            return RedirectToAction("FilterOrders");
        }
        #endregion
    }
}
