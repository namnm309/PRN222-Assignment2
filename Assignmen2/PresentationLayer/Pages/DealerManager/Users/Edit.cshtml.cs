using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationLayer.Pages.DealerManager.Users
{
	public class EditModel : BaseDealerManagerPageModel
	{
		public EditModel(
			IDealerService dealerService,
			IOrderService orderService,
			ITestDriveService testDriveService,
			ICustomerService customerService,
			IEVMReportService reportService,
			IDealerDebtService dealerDebtService,
			IAuthenService authenService,
			IPurchaseOrderService purchaseOrderService,
			IProductService productService,
			IBrandService brandService)
			: base(dealerService, orderService, testDriveService, customerService, reportService, dealerDebtService, authenService, purchaseOrderService, productService, brandService)
		{
		}

		public UserVm User { get; set; } = new();

		[BindProperty]
		public InputModel Input { get; set; } = new();

		public async Task<IActionResult> OnGetAsync(Guid id)
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			var users = await ReportService.GetUsersByDealerAsync(dealerId.Value);
			var user = users.FirstOrDefault(u => u.Id == id);
			
			if (user == null) return NotFound();

			User = new UserVm
			{
				Id = user.Id,
				FullName = user.FullName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				Role = user.Role,
				DealerName = user.Dealer?.Name,
				IsActive = user.IsActive
			};

			Input = new InputModel
			{
				Id = user.Id,
				FullName = user.FullName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				Role = user.Role,
				IsActive = user.IsActive
			};

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var dealerId = GetCurrentDealerId();
			if (dealerId == null) return RedirectToPage("/Dashboard/Index");

			// Check for double submit
			if (Request.Headers.ContainsKey("X-Requested-With") && 
				Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				return new JsonResult(new { success = false, message = "Request already processed" });
			}

			if (!ModelState.IsValid)
			{
				// Reload user data for display
				await ReloadUserDataAsync(dealerId.Value);
				return Page();
			}

			// Get current user first
			var (getSuccess, getError, currentUser) = await AuthenService.GetUserByIdAsync(Input.Id);
			if (!getSuccess || currentUser == null)
			{
				ModelState.AddModelError(string.Empty, "Không tìm thấy thông tin người dùng");
				await ReloadUserDataAsync(dealerId.Value);
				return Page();
			}

			// Check if email is being changed and if it already exists
			if (currentUser.Email != Input.Email)
			{
				var (existingUser, _) = await AuthenService.GetUserByEmailAsync(Input.Email);
				if (existingUser != null && existingUser.Id != Input.Id)
				{
					ModelState.AddModelError("Input.Email", "Email này đã được sử dụng bởi người dùng khác");
					await ReloadUserDataAsync(dealerId.Value);
					return Page();
				}
			}

			// Update user information
			currentUser.FullName = Input.FullName;
			currentUser.Email = Input.Email;
			currentUser.PhoneNumber = Input.PhoneNumber ?? string.Empty;
			currentUser.Role = Input.Role;
			currentUser.IsActive = Input.IsActive;

			var (success, error) = await AuthenService.UpdateUserAsync(currentUser);

			if (success)
			{
				TempData["SuccessMessage"] = "Cập nhật thông tin nhân viên thành công!";
				return RedirectToPage("/DealerManager/Users/Detail", new { id = Input.Id });
			}
			else
			{
				ModelState.AddModelError(string.Empty, error ?? "Không thể cập nhật thông tin nhân viên");
				await ReloadUserDataAsync(dealerId.Value);
				return Page();
			}
		}

		private async Task ReloadUserDataAsync(Guid dealerId)
		{
			var users = await ReportService.GetUsersByDealerAsync(dealerId);
			var user = users.FirstOrDefault(u => u.Id == Input.Id);
			if (user != null)
			{
				User = new UserVm
				{
					Id = user.Id,
					FullName = user.FullName,
					Email = user.Email,
					PhoneNumber = user.PhoneNumber,
					Role = user.Role,
					DealerName = user.Dealer?.Name,
					IsActive = user.IsActive
				};
			}
		}

		public class InputModel
		{
			public Guid Id { get; set; }

			[Required(ErrorMessage = "Họ và tên là bắt buộc")]
			[StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
			public string FullName { get; set; } = string.Empty;

			[Required(ErrorMessage = "Email là bắt buộc")]
			[EmailAddress(ErrorMessage = "Email không hợp lệ")]
			public string Email { get; set; } = string.Empty;

			[Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
			public string? PhoneNumber { get; set; }

			[Required(ErrorMessage = "Vai trò là bắt buộc")]
			public DataAccessLayer.Enum.UserRole Role { get; set; }

			public bool IsActive { get; set; } = true;
		}

		public class UserVm
		{
			public Guid Id { get; set; }
			public string FullName { get; set; } = string.Empty;
			public string Email { get; set; } = string.Empty;
			public string? PhoneNumber { get; set; }
			public DataAccessLayer.Enum.UserRole Role { get; set; }
			public string? DealerName { get; set; }
			public bool IsActive { get; set; }
		}
	}
}
