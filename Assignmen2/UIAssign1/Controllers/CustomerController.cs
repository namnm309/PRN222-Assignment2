using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.ViewModels;

public class CustomersController : Controller
{
    private readonly ICustomerService _service;
    private readonly IMappingService _mappingService;
    public CustomersController(ICustomerService service, IMappingService mappingService)
    {
        _service = service;
        _mappingService = mappingService;
    }

    [HttpGet]
    public async Task<IActionResult> Profile(Guid id)
    {
        var (ok, err, c) = await _service.GetAsync(id);
        if (!ok) return NotFound();
        
        // Map entity to view model using MappingService
        var vm = _mappingService.MapToCustomerViewModel(c);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(CustomerViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        
        // Map view model to entity using MappingService
        var entity = _mappingService.MapToCustomer(vm);
        entity.UpdatedAt = DateTime.UtcNow;
        
        var (ok, err, _) = await _service.UpdateProfileAsync(entity);
        if (!ok) { ModelState.AddModelError("", err); return View(vm); }
        TempData["Msg"] = "Cập nhật thành công.";
        return RedirectToAction(nameof(Profile), new { id = vm.Id });
    }
}
