using AutoMapper;
using DataAccessLayer.Entities;
using BusinessLayer.DTOs.Requests;
using BusinessLayer.DTOs.Responses;
using BusinessLayer.Enums;

namespace BusinessLayer.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Product mappings
            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<ProductCreateRequest, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Brand, opt => opt.Ignore());

            CreateMap<ProductEditRequest, Product>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Brand, opt => opt.Ignore());

            // Customer mappings
            CreateMap<Customer, CustomerResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<CustomerResponse, Customer>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // User mappings
            CreateMap<Users, UserCreateRequest>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore())
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.DealerId, opt => opt.MapFrom(src => src.DealerId));

            CreateMap<UserCreateRequest, Users>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Dealer, opt => opt.Ignore());

            // UserEditViewModel mappings
            CreateMap<Users, UserEditRequest>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            CreateMap<UserEditRequest, Users>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.DealerId, opt => opt.Ignore())
                .ForMember(dest => dest.Dealer, opt => opt.Ignore());

            // Brand mappings
            CreateMap<Brand, BrandResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            // Order mappings
            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.DealerId, opt => opt.MapFrom(src => src.DealerId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.SalesPersonId, opt => opt.MapFrom(src => src.SalesPersonId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.FinalAmount, opt => opt.MapFrom(src => src.FinalAmount))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.DeliveryDate, opt => opt.MapFrom(src => src.DeliveryDate))
                .ForMember(dest => dest.PaymentDueDate, opt => opt.MapFrom(src => src.PaymentDueDate))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : string.Empty))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Name : string.Empty))
                .ForMember(dest => dest.SalesPersonName, opt => opt.MapFrom(src => src.SalesPerson != null ? src.SalesPerson.FullName : string.Empty));

            CreateMap<OrderCreateRequest, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.RegionId, opt => opt.Ignore())
                .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
                .ForMember(dest => dest.FinalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentStatus, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentMethod, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDate, opt => opt.Ignore())
                .ForMember(dest => dest.DeliveryDate, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentDueDate, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Dealer, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Region, opt => opt.Ignore())
                .ForMember(dest => dest.SalesPerson, opt => opt.Ignore());

            // Feedback mappings
            CreateMap<Feedback, FeedbackResponse>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : string.Empty))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Email : string.Empty))
                .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.PhoneNumber : string.Empty))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.ReplyMessage, opt => opt.Ignore())
                .ForMember(dest => dest.RepliedAt, opt => opt.Ignore())
                .ForMember(dest => dest.RepliedByName, opt => opt.Ignore());

            CreateMap<FeedbackCreateRequest, Feedback>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            // TestDrive mappings
            CreateMap<TestDrive, TestDriveResponse>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.DealerId, opt => opt.MapFrom(src => src.DealerId))
                .ForMember(dest => dest.ScheduledDate, opt => opt.MapFrom(src => src.ScheduledDate))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (TestDriveStatus)src.Status))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
                .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.CustomerPhone))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.CustomerEmail))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Name : string.Empty))
                .ForMember(dest => dest.CustomerFullName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : src.CustomerName))
                .ForMember(dest => dest.CustomerPhoneNumber, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.PhoneNumber : src.CustomerPhone))
                .ForMember(dest => dest.CustomerEmailAddress, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Email : src.CustomerEmail));

            CreateMap<TestDriveCreateRequest, TestDrive>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Dealer, opt => opt.Ignore());

            // PurchaseOrder mappings
            CreateMap<PurchaseOrder, PurchaseOrderResponse>()
                .ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Name : string.Empty))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.RequestedByName, opt => opt.MapFrom(src => src.RequestedBy != null ? src.RequestedBy.FullName : string.Empty))
                .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => src.ApprovedBy != null ? src.ApprovedBy.FullName : string.Empty))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (PurchaseOrderStatus)src.Status));

            // User mappings
            CreateMap<Users, UserResponse>()
                .ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Name : string.Empty))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => (UserRole)src.Role));

            // Dealer mappings
            CreateMap<Dealer, DealerResponse>()
                .ForMember(dest => dest.RegionName, opt => opt.MapFrom(src => src.Region != null ? src.Region.Name : string.Empty));

            // PricingPolicy mappings
            CreateMap<PricingPolicy, PricingPolicyResponse>()
                .ForMember(dest => dest.ValidFrom, opt => opt.MapFrom(src => src.EffectiveDate))
                .ForMember(dest => dest.ValidTo, opt => opt.MapFrom(src => src.ExpiryDate));
            CreateMap<PricingPolicyResponse, PricingPolicy>()
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.ValidFrom))
                .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ValidTo))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // InventoryAllocation mappings
            CreateMap<InventoryAllocation, InventoryAllocationResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Name : string.Empty));

            // InventoryTransaction mappings
            CreateMap<InventoryTransaction, InventoryTransactionResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Name : string.Empty))
                .ForMember(dest => dest.ProcessedByUserName, opt => opt.MapFrom(src => src.ProcessedByUser != null ? src.ProcessedByUser.FullName : string.Empty));

            // DealerContract mappings
            CreateMap<DealerContract, DealerContractResponse>()
                .ForMember(dest => dest.DealerName, opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Name : string.Empty))
                .ForMember(dest => dest.RegionName, opt => opt.Ignore());
        }
    }
}
