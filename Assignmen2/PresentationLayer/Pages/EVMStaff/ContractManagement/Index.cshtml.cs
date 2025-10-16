using BusinessLayer.Services;
using BusinessLayer.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Pages.Base;

namespace PresentationLayer.Pages.EVMStaff.ContractManagement
{
    public class IndexModel : BaseEVMStaffPageModel
    {
        private readonly IDealerContractService _contractService;
        private readonly IMappingService _mappingService;

        public IndexModel(
            IDealerContractService contractService,
            IMappingService mappingService)
        {
            _contractService = contractService;
            _mappingService = mappingService;
        }

        public List<DealerContractResponse> Contracts { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public Guid? DealerId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var result = await _contractService.GetAllContractsAsync(DealerId, Status);

            if (result.Success && result.Data != null)
            {
                Contracts = _mappingService.MapToDealerContractViewModels(result.Data);
            }
            else
            {
                TempData["Error"] = result.Error ?? "Không thể tải danh sách hợp đồng";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(Guid id, string status)
        {
            var result = await _contractService.UpdateContractStatusAsync(id, status);

            if (result.Success)
            {
                TempData["Success"] = "Cập nhật trạng thái hợp đồng thành công!";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Không thể cập nhật trạng thái";
            }

            return RedirectToPage();
        }
    }
}

