using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BusinessLayer.DTOs.Requests
{
	public class ProductCreateRequest
	{
		[Required(ErrorMessage = "SKU không được để trống")]
		[StringLength(50, ErrorMessage = "SKU không được vượt quá 50 ký tự")]
		public string Sku { get; set; } = string.Empty;

		[Required(ErrorMessage = "Tên sản phẩm không được để trống")]
		[StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
		public string Name { get; set; } = string.Empty;

		[Required(ErrorMessage = "Mô tả không được để trống")]
		[StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
		public string Description { get; set; } = string.Empty;

		[Required(ErrorMessage = "Giá không được để trống")]
		[Range(0.01, 999999999999.99, ErrorMessage = "Giá phải lớn hơn 0")]
		public decimal Price { get; set; }

		[Required(ErrorMessage = "Số lượng tồn kho không được để trống")]
		[Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải >= 0")]
		public int StockQuantity { get; set; }

		public bool IsActive { get; set; } = true;

		[Required(ErrorMessage = "Thương hiệu không được để trống")]
		public Guid BrandId { get; set; }

		public IFormFile? ImageFile { get; set; }

		[StringLength(500, ErrorMessage = "URL hình ảnh không được vượt quá 500 ký tự")]
		public string? ImageUrl { get; set; }
	}
}


