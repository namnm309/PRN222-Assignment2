using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DataAccessLayer.Entities;
using BusinessLayer.DTOs.Requests;
using BusinessLayer.DTOs.Responses;

namespace BusinessLayer.Services
{
    public class MappingService : IMappingService
    {
        private readonly IMapper _mapper;

        public MappingService(IMapper mapper)
        {
            _mapper = mapper;
        }

        // Product mappings
        public ProductResponse MapToProductViewModel(Product product)
        {
            return _mapper.Map<ProductResponse>(product);
        }

        public List<ProductResponse> MapToProductViewModels(List<Product> products)
        {
            return _mapper.Map<List<ProductResponse>>(products);
        }

        public Product MapToProduct(ProductCreateRequest viewModel)
        {
            return _mapper.Map<Product>(viewModel);
        }

        public Product MapToProduct(ProductEditRequest viewModel)
        {
            return _mapper.Map<Product>(viewModel);
        }

        // Customer mappings
        public CustomerResponse MapToCustomerViewModel(Customer customer)
        {
            return _mapper.Map<CustomerResponse>(customer);
        }

        public Customer MapToCustomer(CustomerResponse viewModel)
        {
            return _mapper.Map<Customer>(viewModel);
        }

        public List<CustomerResponse> MapToCustomerViewModels(List<Customer> customers)
        {
            return _mapper.Map<List<CustomerResponse>>(customers);
        }

        // User mappings
        public UserCreateRequest MapToUserCreateViewModel(Users user)
        {
            return _mapper.Map<UserCreateRequest>(user);
        }

        public Users MapToUser(UserCreateRequest viewModel)
        {
            return _mapper.Map<Users>(viewModel);
        }

        public UserEditRequest MapToUserEditViewModel(Users user)
        {
            return _mapper.Map<UserEditRequest>(user);
        }

        public Users MapToUser(UserEditRequest viewModel)
        {
            return _mapper.Map<Users>(viewModel);
        }

        // Brand mappings
        public BrandResponse MapToBrandViewModel(Brand brand)
        {
            return _mapper.Map<BrandResponse>(brand);
        }

        public List<BrandResponse> MapToBrandViewModels(List<Brand> brands)
        {
            return _mapper.Map<List<BrandResponse>>(brands);
        }

        // Order mappings
        public OrderResponse MapToOrderCreateViewModel(Order order)
        {
            return _mapper.Map<OrderResponse>(order);
        }

        public Order MapToOrder(OrderCreateRequest viewModel)
        {
            return _mapper.Map<Order>(viewModel);
        }

        public List<OrderResponse> MapToOrderCreateViewModels(List<Order> orders)
        {
            return _mapper.Map<List<OrderResponse>>(orders);
        }

        // Feedback mappings
        public FeedbackResponse MapToFeedbackViewModel(Feedback feedback)
        {
            return _mapper.Map<FeedbackResponse>(feedback);
        }

        public List<FeedbackResponse> MapToFeedbackViewModels(List<Feedback> feedbacks)
        {
            return _mapper.Map<List<FeedbackResponse>>(feedbacks);
        }

        public Feedback MapToFeedback(FeedbackCreateRequest viewModel)
        {
            return _mapper.Map<Feedback>(viewModel);
        }

        // TestDrive mappings
        public TestDriveResponse MapToTestDriveViewModel(TestDrive testDrive)
        {
            return _mapper.Map<TestDriveResponse>(testDrive);
        }

        public List<TestDriveResponse> MapToTestDriveViewModels(List<TestDrive> testDrives)
        {
            return _mapper.Map<List<TestDriveResponse>>(testDrives);
        }

        public TestDrive MapToTestDrive(TestDriveCreateRequest viewModel)
        {
            return _mapper.Map<TestDrive>(viewModel);
        }

        // PurchaseOrder mappings
        public PurchaseOrderResponse MapToPurchaseOrderViewModel(PurchaseOrder purchaseOrder)
        {
            return _mapper.Map<PurchaseOrderResponse>(purchaseOrder);
        }

        public List<PurchaseOrderResponse> MapToPurchaseOrderViewModels(List<PurchaseOrder> purchaseOrders)
        {
            return _mapper.Map<List<PurchaseOrderResponse>>(purchaseOrders);
        }

        // User mappings
        public UserResponse MapToUserViewModel(Users user)
        {
            return _mapper.Map<UserResponse>(user);
        }

        public List<UserResponse> MapToUserViewModels(List<Users> users)
        {
            return _mapper.Map<List<UserResponse>>(users);
        }

        // Dealer mappings
        public DealerResponse MapToDealerViewModel(Dealer dealer)
        {
            return _mapper.Map<DealerResponse>(dealer);
        }

        public List<DealerResponse> MapToDealerViewModels(List<Dealer> dealers)
        {
            return _mapper.Map<List<DealerResponse>>(dealers);
        }

        // PricingPolicy mappings
        public PricingPolicyResponse MapToPricingPolicyViewModel(PricingPolicy pricingPolicy)
        {
            return _mapper.Map<PricingPolicyResponse>(pricingPolicy);
        }

        public List<PricingPolicyResponse> MapToPricingPolicyViewModels(List<PricingPolicy> pricingPolicies)
        {
            return _mapper.Map<List<PricingPolicyResponse>>(pricingPolicies);
        }

        public PricingPolicy MapToPricingPolicy(PricingPolicyResponse viewModel)
        {
            return _mapper.Map<PricingPolicy>(viewModel);
        }

        // InventoryAllocation mappings
        public InventoryAllocationResponse MapToInventoryAllocationViewModel(InventoryAllocation inventoryAllocation)
        {
            return _mapper.Map<InventoryAllocationResponse>(inventoryAllocation);
        }

        public List<InventoryAllocationResponse> MapToInventoryAllocationViewModels(List<InventoryAllocation> inventoryAllocations)
        {
            return _mapper.Map<List<InventoryAllocationResponse>>(inventoryAllocations);
        }

        // InventoryTransaction mappings
        public InventoryTransactionResponse MapToInventoryTransactionViewModel(InventoryTransaction transaction)
        {
            return _mapper.Map<InventoryTransactionResponse>(transaction);
        }

        public List<InventoryTransactionResponse> MapToInventoryTransactionViewModels(List<InventoryTransaction> transactions)
        {
            return _mapper.Map<List<InventoryTransactionResponse>>(transactions);
        }

        // DealerContract mappings
        public DealerContractResponse MapToDealerContractViewModel(DealerContract dealerContract)
        {
            return _mapper.Map<DealerContractResponse>(dealerContract);
        }

        public List<DealerContractResponse> MapToDealerContractViewModels(List<DealerContract> dealerContracts)
        {
            return _mapper.Map<List<DealerContractResponse>>(dealerContracts);
        }

        // Region mappings
        public RegionResponse MapToRegionViewModel(Region region)
        {
            return _mapper.Map<RegionResponse>(region);
        }

        public List<RegionResponse> MapToRegionViewModels(List<Region> regions)
        {
            return _mapper.Map<List<RegionResponse>>(regions);
        }
    }
}
