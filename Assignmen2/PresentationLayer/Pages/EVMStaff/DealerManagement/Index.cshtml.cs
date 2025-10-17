using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.DealerManagement
{
    public class IndexModel : BaseEVMStaffPageModel
    {
        private readonly IDealerService _dealerService;
        private readonly IEVMReportService _evmService;
        private readonly IMappingService _mappingService;

        public IndexModel(IDealerService dealerService, IEVMReportService evmService, IMappingService mappingService)
        {
            _dealerService = dealerService;
            _evmService = evmService;
            _mappingService = mappingService;
        }

        public List<DealerResponse> Dealers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToPage("/Dashboard/Index");
            }

            var (ok, err, data) = await _dealerService.GetAllAsync();
            if (!ok || data == null)
            {
                TempData["Error"] = err ?? "Không thể tải danh sách đại lý";
                Dealers = new List<DealerResponse>();
                return Page();
            }

            Dealers = _mappingService.MapToDealerViewModels(data);

            // Bổ sung RegionName nếu thiếu
            var regions = await _evmService.GetAllRegionsAsync();
            var regionMap = regions.ToDictionary(r => r.Id, r => r.Name);
            foreach (var d in Dealers)
            {
                if (regionMap.TryGetValue(d.RegionId, out var name))
                {
                    d.RegionName = name;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (!IsAdmin())
            {
                TempData["Error"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToPage("/Dashboard/Index");
            }

            var (ok, err) = await _dealerService.DeleteAsync(id);
            if (ok)
            {
                TempData["Success"] = "Xóa đại lý thành công!";
            }
            else
            {
                TempData["Error"] = err ?? "Không thể xóa đại lý";
            }

            return RedirectToPage();
        }
    }
}


