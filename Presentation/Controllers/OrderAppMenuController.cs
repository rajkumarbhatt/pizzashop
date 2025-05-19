using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Presentaion.Controllers;

namespace Presentation.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [CustomAuth]
    public class OrderAppMenu : Controller
    {
        private readonly IKotMenuService _kotMenuService;
        private readonly IJwtService _jwtService;
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderAppMenu> _logger;
        public OrderAppMenu(ILogger<OrderAppMenu> logger, IKotMenuService kotMenuService, IJwtService jwtService, IOrderService orderService)
        {
            _logger = logger;
            _kotMenuService = kotMenuService;
            _jwtService = jwtService;
            _orderService = orderService;
        }
        [Route("/OrderApp/Menu")]
        [Route("/OrderApp/Menu/{orderId}")]
        public async Task<ActionResult> Index(int? orderId)
        {
            string orderStatus = await _kotMenuService.GetOrderStatusAsync(orderId ?? 0);
            if (orderStatus == "Completed" || orderStatus == "Cancelled")
            {
                string encryptedOrderId = await _orderService.EncryptOrderIdAsync(orderId ?? 0);
                return RedirectToAction("OrderDetails", "Order", new { id = encryptedOrderId });
            }
            KotMenuViewModel kotMenuViewModel = await _kotMenuService.GetKotMenuAsync(orderId);
            return View(kotMenuViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetKotMenuItemsBasedOnCategory(int categoryId, string search)
        {
            KotMenuViewModel kotMenuViewModel = await _kotMenuService.SearchMenuItemsKotAsync(search, categoryId);
            return PartialView("_KotMenuItemsList", kotMenuViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> SearchMenuItemsKot(string search, int categoryId)
        {
            KotMenuViewModel kotMenuViewModel = await _kotMenuService.SearchMenuItemsKotAsync(search, categoryId);
            return PartialView("_KotMenuItemsList", kotMenuViewModel);
        }
        [HttpPut]
        public async Task<JsonResult> AddToFavourites(int itemId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _kotMenuService.AddToFavouritesAsync(itemId, userId);
        }
        [HttpDelete]
        public async Task<JsonResult> DeleteFromFavourites(int itemId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _kotMenuService.DeleteFromFavouritesAsync(itemId, userId);
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomerDetails(int orderId)
        {
            KotMenuViewModel kotMenuViewModel = await _kotMenuService.GetCustomerDetailsAsync(orderId);
            return PartialView("_CustomerDetailsModal", kotMenuViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCustomerDetails([FromForm] WaitingListModal waitingListModal)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Invalid Data" });
            }
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _kotMenuService.UpdateCustomerDetailsAsync(waitingListModal, userId);
        }
        [HttpGet]
        public async Task<IActionResult> GetSelectModifiersModalData(int itemId)
        {
            KotMenuViewModel kotMenuViewModel = await _kotMenuService.GetSelectModifiersModalDataAsync(itemId);
            return PartialView("_SelectModifiersModal", kotMenuViewModel);
        }
        [HttpGet]
        public async Task<JsonResult> GetOrderWiseComment(int orderId)
        {
            return await _kotMenuService.GetOrderWiseCommentAsync(orderId);
        }
        [HttpGet]
        public async Task<JsonResult> GetItemWiseComment(int orderItemId)
        {
            return await _kotMenuService.GetItemWiseCommentAsync(orderItemId);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrderWiseComment(int orderId, string comment)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _kotMenuService.AddOrderWiseCommentAsync(orderId, comment, userId);
        }
        [HttpPost]
        public async Task<IActionResult> AddItemWiseComment(int orderItemId, string comment)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _kotMenuService.AddItemWiseCommentAsync(orderItemId, comment, userId);
        }
        [HttpPost]
        public async Task<IActionResult> SaveOrder(SaveOrderViewModel saveOrderViewModel)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _kotMenuService.SaveOrderAsync(saveOrderViewModel, userId);
        }
        [HttpPost]
        public async Task<IActionResult> CompleteOrder(int orderId, string paymentMode)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _kotMenuService.CompleteOrderAsync(orderId, userId, paymentMode);
        }
        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _kotMenuService.CancelOrderAsync(orderId, userId);
        }
        [HttpPost]
        public async Task<IActionResult> SaveCustomerReview(SaveCustomerReviewViewModel saveCustomerReviewViewModel)
        {
            int userId = await _jwtService.GetUserIdFromJwtTokenAsync(Request.Cookies["token"] ?? "");
            return await _kotMenuService.SaveCustomerReviewAsync(saveCustomerReviewViewModel, userId);
        }
        [HttpGet]
        public async Task<JsonResult> CanDeleteFromOrder(int orderItemId)
        {
            return await _kotMenuService.CanDeleteFromOrderAsync(orderItemId);
        }
        [HttpGet]
        public async Task<JsonResult> CanReduceFromOrder(int orderItemId, int currentQuantity)
        {
            return await _kotMenuService.CanReduceFromOrderAsync(orderItemId, currentQuantity);
        }
        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            KotMenuViewModel kotMenuViewModel = await _kotMenuService.GetKotMenuAsync(orderId);
            return PartialView("_OrderItemDetailsPartial", kotMenuViewModel);
        }
        [HttpGet]
        public async Task<JsonResult> AreModifiersSelected(int itemId)
        {
            return await _kotMenuService.AreModifiersSelectedAsync(itemId);
        }
        [HttpGet]
        [Route("/OrderAppMenu/CreateOrder/{orderId}")]
        public async Task<ActionResult> CreateOrder(int orderId)
        {
            KotMenuViewModel kotMenuViewModel = await _kotMenuService.GetPaymentViewModalAsync(orderId);
            return View("PaymentPagePartial", kotMenuViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Complete(string rzp_paymentid, string rzp_orderid)
        {
            if (string.IsNullOrEmpty(rzp_paymentid) || string.IsNullOrEmpty(rzp_orderid))
            {
                return BadRequest("Invalid payment details.");
            }
            _logger.LogInformation($"Payment successful. Payment ID: {rzp_paymentid}, Order ID: {rzp_orderid}");
            return Ok("Payment processed successfully.");
        }
    }
}