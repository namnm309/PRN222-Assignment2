using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.ViewModels;
using BusinessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    public class UserManagementController : BaseDashboardController
    {
        private readonly IAuthenService _authenService;
        private readonly IEVMReportService _evmService;
        private readonly IMappingService _mappingService;

        public UserManagementController(IAuthenService authenService, IEVMReportService evmService, IMappingService mappingService)
        {
            _authenService = authenService;
            _evmService = evmService;
            _mappingService = mappingService;
        }

        // GET: UserManagement/Index - Danh sách user (Admin/Dealer Manager)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            List<UserViewModel> users;

            if (userRole == "Admin")
            {
                // Admin xem tất cả users
                var data = await _evmService.GetAllUsersAsync();
                users = _mappingService.MapToUserViewModels(data);
            }
            else if (userRole == "DealerManager" && !string.IsNullOrEmpty(dealerIdString) && Guid.TryParse(dealerIdString, out Guid dealerId))
            {
                // Dealer Manager chỉ xem staff của chính dealer mình
                var data2 = await _evmService.GetUsersByDealerAsync(dealerId);
                users = _mappingService.MapToUserViewModels(data2);
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Dashboard");
            }

            return View(users);
        }

        // GET: UserManagement/CreateDealerManager - Admin tạo Dealer Manager
        [HttpGet]
        public async Task<IActionResult> CreateDealerManager()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Chỉ Admin mới có quyền tạo Dealer Manager.";
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Dealers = await _evmService.GetAllDealersAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDealerManager(UserCreateViewModel model)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Chỉ Admin mới có quyền tạo Dealer Manager.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Dealers = await _evmService.GetAllDealersAsync();
                return View(model);
            }

            // Kiểm tra email đã tồn tại chưa
            var (existingUser, _) = await _authenService.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                ViewBag.Dealers = await _evmService.GetAllDealersAsync();
                return View(model);
            }

            var (success, error, user) = await _authenService.RegisterAsync(
                model.FullName, 
                model.Email, 
                model.Password, 
                model.PhoneNumber, 
                model.Address, 
                UserRole.DealerManager,
                model.DealerId
            );

            if (!success)
            {
                TempData["Error"] = error;
                ViewBag.Dealers = await _evmService.GetAllDealersAsync();
                return View(model);
            }

            TempData["Success"] = $"Tạo tài khoản Dealer Manager thành công! Email: {model.Email}";
            return RedirectToAction(nameof(Index));
        }

        // GET: UserManagement/CreateDealerStaff - Dealer Manager tạo Dealer Staff
        [HttpGet]
        public IActionResult CreateDealerStaff()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            
            if (userRole != "DealerManager")
            {
                TempData["Error"] = "Chỉ Dealer Manager mới có quyền tạo Dealer Staff.";
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDealerStaff(UserCreateViewModel model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            if (userRole != "DealerManager")
            {
                TempData["Error"] = "Chỉ Dealer Manager mới có quyền tạo Dealer Staff.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (string.IsNullOrEmpty(dealerIdString) || !Guid.TryParse(dealerIdString, out Guid dealerId))
            {
                TempData["Error"] = "Không xác định được dealer của bạn.";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra email đã tồn tại chưa
            var (existingUser, _) = await _authenService.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                return View(model);
            }

            // Tạo Dealer Staff với DealerId của Dealer Manager
            var (success, error, user) = await _authenService.RegisterAsync(
                model.FullName, 
                model.Email, 
                model.Password, 
                model.PhoneNumber, 
                model.Address, 
                UserRole.DealerStaff,
                dealerId // Gán cùng dealer với Dealer Manager
            );

            if (!success)
            {
                TempData["Error"] = error;
                return View(model);
            }

            TempData["Success"] = $"Tạo tài khoản Dealer Staff thành công! Email: {model.Email}";
            return RedirectToAction(nameof(Index));
        }

        // GET: UserManagement/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            var user = await _evmService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            // Admin có thể edit tất cả
            if (userRole != "Admin")
            {
                // Dealer Manager chỉ edit staff của chính dealer mình
                if (userRole == "DealerManager" && Guid.TryParse(dealerIdString, out Guid dealerId))
                {
                if (user.DealerId != dealerId || user.Role.ToString() != UserRole.DealerStaff.ToString())
                    {
                        TempData["Error"] = "Bạn chỉ có thể chỉnh sửa nhân viên của chính đại lý mình.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    TempData["Error"] = "Bạn không có quyền chỉnh sửa người dùng này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Map entity to view model using AutoMapper
            var viewModel = _mappingService.MapToUserEditViewModel(user);

            if (IsAdmin())
            {
                ViewBag.Dealers = await _evmService.GetAllDealersAsync();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            if (!ModelState.IsValid)
            {
                if (IsAdmin())
                {
                    ViewBag.Dealers = await _evmService.GetAllDealersAsync();
                }
                return View(model);
            }

            var user = await _evmService.GetUserByIdAsync(model.Id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            // Admin có thể edit tất cả
            if (userRole != "Admin")
            {
                // Dealer Manager chỉ edit staff của chính dealer mình
                if (userRole == "DealerManager" && Guid.TryParse(dealerIdString, out Guid dealerId))
                {
                if (user.DealerId != dealerId || user.Role.ToString() != UserRole.DealerStaff.ToString())
                    {
                        TempData["Error"] = "Bạn chỉ có thể chỉnh sửa nhân viên của chính đại lý mình.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    TempData["Error"] = "Bạn không có quyền chỉnh sửa người dùng này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Update user info
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.IsActive = model.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            var success = await _evmService.UpdateUserAsync(user);
            if (!success)
            {
                TempData["Error"] = "Không thể cập nhật thông tin người dùng.";
                return View(model);
            }

            TempData["Success"] = "Cập nhật thông tin người dùng thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: UserManagement/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            var dealerIdString = HttpContext.Session.GetString("DealerId");

            var user = await _evmService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            // Admin có thể xóa (deactivate) tất cả trừ chính mình
            if (userRole == "Admin")
            {
                var currentUserEmail = HttpContext.Session.GetString("UserEmail");
                if (user.Email == currentUserEmail)
                {
                    TempData["Error"] = "Bạn không thể xóa tài khoản của chính mình.";
                    return RedirectToAction(nameof(Index));
                }
            }
            // Dealer Manager chỉ xóa staff của chính dealer mình
            else if (userRole == "DealerManager" && Guid.TryParse(dealerIdString, out Guid dealerId))
            {
                if (user.DealerId != dealerId || user.Role.ToString() == UserRole.Admin.ToString())
                {
                    TempData["Error"] = "Bạn chỉ có thể xóa nhân viên của chính đại lý mình.";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền xóa người dùng này.";
                return RedirectToAction(nameof(Index));
            }

            // Hard delete - xóa thật sự khỏi database
            var success = await _evmService.DeleteUserAsync(id);
            if (!success)
            {
                TempData["Error"] = "Không thể xóa người dùng.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Xóa người dùng thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}

