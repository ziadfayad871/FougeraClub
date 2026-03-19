using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using FougeraClub.Application.DTOs;
using FougeraClub.Application.Interfaces.Services;
using FougeraClub.Web.Notifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using FougeraClub.Web.Otp;

namespace FougeraClub.Web.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService _service;
        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly INotificationStore _notificationStore;
        private readonly IManagerOtpService _managerOtpService;

        public PurchaseOrderController(
            IPurchaseOrderService service,
            IHubContext<NotificationsHub> hubContext,
            INotificationStore notificationStore,
            IManagerOtpService managerOtpService)
        {
            _service = service;
            _hubContext = hubContext;
            _notificationStore = notificationStore;
            _managerOtpService = managerOtpService;
        }

        public async Task<IActionResult> Index(string supplierName, DateTime? fromDate, DateTime? toDate)
        {
            var orderDtos = await _service.GetOrdersAsync(supplierName, fromDate, toDate);
            var suppliers = await _service.GetSuppliersAsync();

            ViewBag.SupplierName = supplierName;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.Suppliers = suppliers;

            return View(orderDtos);
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            ViewBag.Suppliers = await _service.GetSuppliersAsync();
            ViewBag.ManagerApprovedName = HttpContext.Session.GetString("ManagerApprovedName");

            if (!id.HasValue)
            {
                var newOrder = await _service.CreateNewOrderDtoAsync();
                return View(newOrder);
            }

            var orderDto = await _service.GetOrderDtoByIdAsync(id.Value);
            if (orderDto == null) return NotFound();
            return View(orderDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(PurchaseOrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers = await _service.GetSuppliersAsync();
                return View(dto);
            }

            var isNewOrder = dto.Id == 0;
            await _service.SaveAsync(dto);

            if (isNewOrder)
            {
                var title = "أمر شراء جديد";
                var message = $"تمت إضافة أمر الشراء رقم {dto.OrderNumber}";
                _notificationStore.Add(title, message);

                await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
                {
                    title,
                    message,
                    createdAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? supplierName, DateTime? fromDate, DateTime? toDate)
        {
            await _service.DeleteAsync(id);

            return RedirectToAction(nameof(Index), new
            {
                supplierName,
                fromDate,
                toDate
            });
        }

        [HttpGet]
        public async Task<IActionResult> Print(int id)
        {
            var order = await _service.GetOrderDtoByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            ViewBag.ManagerApprovedName = HttpContext.Session.GetString("ManagerApprovedName");
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> RequestManagerOtp()
        {
            var result = await _managerOtpService.IssueOtpAsync(HttpContext);
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                expiresAt = result.ExpiresAtUtcIso,
                deliveryEnabled = result.DeliveryEnabled
            });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyManagerOtp([FromBody] VerifyManagerOtpRequest request)
        {
            var result = await _managerOtpService.VerifyOtpAsync(HttpContext, request?.Otp);
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                managerName = result.ManagerName
            });
        }

        public class VerifyManagerOtpRequest
        {
            public string Otp { get; set; } = string.Empty;
        }
    }
}

