using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.ViewModels;

public class ProductsController : Controller
{
    private readonly IProductService _service;
    private readonly IEVMReportService _evmService;
    private readonly IMappingService _mappingService;
    
    public ProductsController(IProductService service, IEVMReportService evmService, IMappingService mappingService)
    {
        _service = service;
        _evmService = evmService;
        _mappingService = mappingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] ProductViewModel vm)
    {
        var (ok, err, list) = await _service.SearchAsync(vm.Q, vm.BrandId, vm.MinPrice, vm.MaxPrice, vm.InStock, vm.IsActive);
        if (!ok) { ModelState.AddModelError("", err); }
        
        // Map entities to view models
        var productViewModels = _mappingService.MapToProductViewModels(list);
        return View(productViewModels); 
    }

    [HttpGet]
    public async Task<IActionResult> Detail(Guid id)
    {
        var (ok, err, product) = await _service.GetAsync(id);
        if (!ok) return NotFound();
        
        // Load dealers cho form đặt lịch lái thử
        ViewBag.Dealers = await _evmService.GetAllDealersAsync();
        
        // Map entity to view model
        var productViewModel = _mappingService.MapToProductViewModel(product);
        return View(productViewModel); 
    }
}
