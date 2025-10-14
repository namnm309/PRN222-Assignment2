# AutoMapper Integration Guide

## Tổng quan
Dự án đã được tích hợp AutoMapper để tách biệt giữa PresentationLayer và DataAccessLayer. PresentationLayer không còn sử dụng trực tiếp entities từ DataAccessLayer.

## Cấu trúc

### 1. AutoMapper Profiles
- **File**: `PresentationLayer/Profiles/AutoMapperProfile.cs`
- **Chức năng**: Định nghĩa các mapping rules giữa entities và view models

### 2. View Models
- **Location**: `PresentationLayer/Models/`
- **Chức năng**: Các model dành riêng cho presentation layer

## Cách sử dụng trong Controllers

### 1. Inject IMapper
```csharp
public class YourController : Controller
{
    private readonly IMapper _mapper;
    
    public YourController(IMapper mapper)
    {
        _mapper = mapper;
    }
}
```

### 2. Mapping từ Entity sang ViewModel
```csharp
// Lấy entity từ service
var (ok, err, entity) = await _service.GetAsync(id);

// Map sang view model
var viewModel = _mapper.Map<YourViewModel>(entity);

return View(viewModel);
```

### 3. Mapping từ ViewModel sang Entity
```csharp
// Map từ view model sang entity
var entity = _mapper.Map<YourEntity>(viewModel);

// Gọi service để lưu
var (ok, err, _) = await _service.UpdateAsync(entity);
```

### 4. Mapping Collection
```csharp
// Map danh sách entities sang view models
var viewModels = _mapper.Map<List<YourViewModel>>(entities);

return View(viewModels);
```

## Mapping Rules đã được định nghĩa

### Product
- `Product` ↔ `ProductViewModel`
- `Product` ↔ `ProductCreateViewModel`
- `Product` ↔ `ProductEditViewModel`

### Customer
- `Customer` ↔ `CustomerViewModel`

### User
- `Users` ↔ `UserCreateViewModel`
- `Users` ↔ `UserEditViewModel`

### Brand
- `Brand` ↔ `BrandViewModel`

### Order
- `Order` ↔ `OrderCreateViewModel`

### Feedback
- `Feedback` ↔ `FeedbackViewModel`

### TestDrive
- `TestDrive` ↔ `TestDriveViewModel`

## Lợi ích

1. **Separation of Concerns**: PresentationLayer không phụ thuộc trực tiếp vào DataAccessLayer
2. **Maintainability**: Dễ dàng thay đổi cấu trúc entities mà không ảnh hưởng đến presentation
3. **Security**: Chỉ expose những properties cần thiết cho view
4. **Flexibility**: Có thể customize mapping logic phức tạp

## Lưu ý

- Khi thêm entity hoặc view model mới, cần cập nhật AutoMapperProfile
- Sử dụng `ForMember()` để customize mapping cho các trường đặc biệt
- Sử dụng `Ignore()` để bỏ qua các trường không cần mapping
