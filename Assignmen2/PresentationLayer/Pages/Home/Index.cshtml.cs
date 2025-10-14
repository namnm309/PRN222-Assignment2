using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.DTOs.Responses;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IMappingService _mappingService;

        public IReadOnlyList<ProductResponse> Products { get; private set; } = Array.Empty<ProductResponse>();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public IndexModel(IProductService productService, IMappingService mappingService)
        {
            _productService = productService;
            _mappingService = mappingService;
        }

        public async Task OnGetAsync()
        {
            var (success, error, data) = await _productService.SearchAsync(Search, null, null, null, null, true);
            if (!success || data == null)
            {
                Products = Array.Empty<ProductResponse>();
                return;
            }

            // Map entities -> DTO responses using mapping service
            Products = _mappingService.MapToProductViewModels(data);
        }
    }
}


