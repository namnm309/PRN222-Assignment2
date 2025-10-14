using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services;
using BusinessLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _service;
        public FeedbackController(IFeedbackService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> ByProduct(Guid productId)
        {
            var (ok, err, list) = await _service.GetByProductAsync(productId);
            if (!ok) { ModelState.AddModelError("", err); }
            ViewBag.ProductId = productId;
            return View(list); 
        }

        [HttpGet]
        public IActionResult Create(Guid productId, Guid customerId)
        {
            return View(new FeedbackViewModel { ProductId = productId, CustomerId = customerId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeedbackViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var (ok, err, _) = await _service.CreateAsync(vm.CustomerId, vm.ProductId, vm.Comment, vm.Rating);
            if (!ok)
            {
                ModelState.AddModelError("", err);
                return View(vm);
            }
            TempData["Msg"] = "Cảm ơn phản hồi của bạn.";
            return RedirectToAction(nameof(ByProduct), new { productId = vm.ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid productId)
        {
            var (ok, err) = await _service.DeleteAsync(id);
            if (!ok) return BadRequest(err);
            TempData["Msg"] = "Đã xóa phản hồi.";
            return RedirectToAction(nameof(ByProduct), new { productId });
        }
    }
}
