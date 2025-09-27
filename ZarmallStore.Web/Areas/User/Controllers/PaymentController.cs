using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Data.DTOs.PaymentDto;
using ZarmallStore.Web.Areas.User.Services;
using ZarmallStore.Web.UserExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ZarmallStore.Web.Areas.User.Controllers
{
    public class PaymentController : UserBaseController
    {
        #region CTOR
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;

        public PaymentController(IUserService userService, IOrderService orderService, IConfiguration configuration, IPaymentService paymentService)
        {
            _userService = userService;
            _orderService = orderService;
            _configuration = configuration;
            _paymentService = paymentService;
        }

        #endregion

        #region Pay Order
        [HttpGet("pay-order")]
        public async Task<IActionResult> PayOrder()
        {
            #region Check User Detail
            var user = await _userService.GetUserById(User.GetUserId());
            if (user.FullName == null || user.UserCity == null || user.Address == null || user.PostCode == null)
            {
                TempData[ErrorMessage] = "لطفا پیش از پرداخت هزینه سفارش نسبت به تکمیل اطلاعات حساب کاربری خود اقدام کنید.";
                return RedirectToAction("EditUserDetail", "Account", new { returnToCheckout = true });
            }
            #endregion

            var order = await _orderService.UserOpenOrderDetail(User.GetUserId());
            if (order == null)
            {
                TempData[ErrorMessage] = "سبد خرید شما خالی است";
                return RedirectToAction("Cart", "Order");
            }

            if (order.TotalCartPrice() * 10 > 1000000000)
            {
                TempData[ErrorMessage] = "سفارشی که بیش از 100 میلیون تومان باشد قابل پرداخت نیست . لطغا از تعداد سفارش خود بکاهید.";
                return RedirectToAction("Cart", "Order");
            }

            var updatedPrice = await _orderService.UpdateOrderDetailPrices(order.Order.Id);

            //Note that you should Convert Toman Value to Rial
            var paymentRequest = new PaymentRequest
            {
                merchant_id = _configuration.GetValue<string>("NovinoPayment:MerchantId") ?? "",
                amount = updatedPrice * 10,
                invoice_id = order.Order.Id.ToString(),
                description = "پرداخت سفارش از وبسایت ZarmallStore",
                callback_url = _configuration.GetValue<string>("NovinoPayment:PaymentCallbackUrl") ?? "",
            };

            var requestPaymentResult = await _paymentService.CreatePayment(paymentRequest);

            if (requestPaymentResult == null) return NotFound();

            switch (requestPaymentResult.status)
            {
                case "100":
                    return Redirect(requestPaymentResult.data.payment_url);
            }

            return NotFound();
        }
        #endregion

        #region CallBack
        [AllowAnonymous]
        [HttpGet("payment-result")]
        public async Task<IActionResult> PaymentResult(string authority,string invoiceId , string paymentStatus)
        {
            var orderPrice = await _orderService.GetOrderTotalPrice(long.Parse(invoiceId));

            switch (paymentStatus)
            {
                case "OK":
                    var paymentVerification = new PaymentVerification
                    {
                        merchant_id = _configuration.GetValue<string>("NovinoPayment:MerchantId") ?? "",
                        authority = authority,
                        amount = orderPrice * 10
                    };

                    var result = await _paymentService.VerifyPayment(paymentVerification);
                    if (result != null)
                    {
                        switch (result.status)
                        {
                            case "100":
                                await _orderService.PayOrderPrice(result.data);
                                var model = await _orderService.OrderDetail(long.Parse(invoiceId));
                                ViewData["PaymentSuccessful"] = true;
                                return View(model);
                        }
                    }
                    break;
                case "NOK":
                    ViewData["OrderId"] = invoiceId;
                    ViewData["PaymentSuccessful"] = false;
                    break;
            }

            ViewData["OrderId"] = invoiceId;
            ViewData["PaymentSuccessful"] = false;
            return View();
        }
        #endregion
    }
}
