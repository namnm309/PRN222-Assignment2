using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DataAccessLayer.Entities;
using BusinessLayer.ViewModels;

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
        public ProductViewModel MapToProductViewModel(Product product)
        {
            return _mapper.Map<ProductViewModel>(product);
        }

        public List<ProductViewModel> MapToProductViewModels(List<Product> products)
        {
            return _mapper.Map<List<ProductViewModel>>(products);
        }

        public Product MapToProduct(ProductCreateViewModel viewModel)
        {
            return _mapper.Map<Product>(viewModel);
        }

        public Product MapToProduct(ProductEditViewModel viewModel)
        {
            return _mapper.Map<Product>(viewModel);
        }

        // Customer mappings
        public CustomerViewModel MapToCustomerViewModel(Customer customer)
        {
            return _mapper.Map<CustomerViewModel>(customer);
        }

        public Customer MapToCustomer(CustomerViewModel viewModel)
        {
            return _mapper.Map<Customer>(viewModel);
        }

        public List<CustomerViewModel> MapToCustomerViewModels(List<Customer> customers)
        {
            return _mapper.Map<List<CustomerViewModel>>(customers);
        }

        // User mappings
        public UserCreateViewModel MapToUserCreateViewModel(Users user)
        {
            return _mapper.Map<UserCreateViewModel>(user);
        }

        public Users MapToUser(UserCreateViewModel viewModel)
        {
            return _mapper.Map<Users>(viewModel);
        }

        public UserEditViewModel MapToUserEditViewModel(Users user)
        {
            return _mapper.Map<UserEditViewModel>(user);
        }

        public Users MapToUser(UserEditViewModel viewModel)
        {
            return _mapper.Map<Users>(viewModel);
        }

        // Brand mappings
        public BrandViewModel MapToBrandViewModel(Brand brand)
        {
            return _mapper.Map<BrandViewModel>(brand);
        }

        public List<BrandViewModel> MapToBrandViewModels(List<Brand> brands)
        {
            return _mapper.Map<List<BrandViewModel>>(brands);
        }

        // Order mappings
        public OrderCreateViewModel MapToOrderCreateViewModel(Order order)
        {
            return _mapper.Map<OrderCreateViewModel>(order);
        }

        public Order MapToOrder(OrderCreateViewModel viewModel)
        {
            return _mapper.Map<Order>(viewModel);
        }

        public List<OrderCreateViewModel> MapToOrderCreateViewModels(List<Order> orders)
        {
            return _mapper.Map<List<OrderCreateViewModel>>(orders);
        }

        // Feedback mappings
        public FeedbackViewModel MapToFeedbackViewModel(Feedback feedback)
        {
            return _mapper.Map<FeedbackViewModel>(feedback);
        }

        public List<FeedbackViewModel> MapToFeedbackViewModels(List<Feedback> feedbacks)
        {
            return _mapper.Map<List<FeedbackViewModel>>(feedbacks);
        }

        public Feedback MapToFeedback(FeedbackViewModel viewModel)
        {
            return _mapper.Map<Feedback>(viewModel);
        }

        // TestDrive mappings
        public TestDriveViewModel MapToTestDriveViewModel(TestDrive testDrive)
        {
            return _mapper.Map<TestDriveViewModel>(testDrive);
        }

        public List<TestDriveViewModel> MapToTestDriveViewModels(List<TestDrive> testDrives)
        {
            return _mapper.Map<List<TestDriveViewModel>>(testDrives);
        }

        public TestDrive MapToTestDrive(TestDriveViewModel viewModel)
        {
            return _mapper.Map<TestDrive>(viewModel);
        }

        // PurchaseOrder mappings
        public PurchaseOrderViewModel MapToPurchaseOrderViewModel(PurchaseOrder purchaseOrder)
        {
            return _mapper.Map<PurchaseOrderViewModel>(purchaseOrder);
        }

        public List<PurchaseOrderViewModel> MapToPurchaseOrderViewModels(List<PurchaseOrder> purchaseOrders)
        {
            return _mapper.Map<List<PurchaseOrderViewModel>>(purchaseOrders);
        }

        // User mappings
        public UserViewModel MapToUserViewModel(Users user)
        {
            return _mapper.Map<UserViewModel>(user);
        }

        public List<UserViewModel> MapToUserViewModels(List<Users> users)
        {
            return _mapper.Map<List<UserViewModel>>(users);
        }

        // Dealer mappings
        public DealerViewModel MapToDealerViewModel(Dealer dealer)
        {
            return _mapper.Map<DealerViewModel>(dealer);
        }

        public List<DealerViewModel> MapToDealerViewModels(List<Dealer> dealers)
        {
            return _mapper.Map<List<DealerViewModel>>(dealers);
        }

        // PricingPolicy mappings
        public PricingPolicyViewModel MapToPricingPolicyViewModel(PricingPolicy pricingPolicy)
        {
            return _mapper.Map<PricingPolicyViewModel>(pricingPolicy);
        }

        public List<PricingPolicyViewModel> MapToPricingPolicyViewModels(List<PricingPolicy> pricingPolicies)
        {
            return _mapper.Map<List<PricingPolicyViewModel>>(pricingPolicies);
        }

        public PricingPolicy MapToPricingPolicy(PricingPolicyViewModel viewModel)
        {
            return _mapper.Map<PricingPolicy>(viewModel);
        }

        // InventoryAllocation mappings
        public InventoryAllocationViewModel MapToInventoryAllocationViewModel(InventoryAllocation inventoryAllocation)
        {
            return _mapper.Map<InventoryAllocationViewModel>(inventoryAllocation);
        }

        public List<InventoryAllocationViewModel> MapToInventoryAllocationViewModels(List<InventoryAllocation> inventoryAllocations)
        {
            return _mapper.Map<List<InventoryAllocationViewModel>>(inventoryAllocations);
        }

        // InventoryTransaction mappings
        public InventoryTransactionViewModel MapToInventoryTransactionViewModel(InventoryTransaction transaction)
        {
            return _mapper.Map<InventoryTransactionViewModel>(transaction);
        }

        public List<InventoryTransactionViewModel> MapToInventoryTransactionViewModels(List<InventoryTransaction> transactions)
        {
            return _mapper.Map<List<InventoryTransactionViewModel>>(transactions);
        }

        // DealerContract mappings
        public DealerContractViewModel MapToDealerContractViewModel(DealerContract dealerContract)
        {
            return _mapper.Map<DealerContractViewModel>(dealerContract);
        }

        public List<DealerContractViewModel> MapToDealerContractViewModels(List<DealerContract> dealerContracts)
        {
            return _mapper.Map<List<DealerContractViewModel>>(dealerContracts);
        }

        // Region mappings
        public RegionViewModel MapToRegionViewModel(Region region)
        {
            return _mapper.Map<RegionViewModel>(region);
        }

        public List<RegionViewModel> MapToRegionViewModels(List<Region> regions)
        {
            return _mapper.Map<List<RegionViewModel>>(regions);
        }
    }
}
