using System.Collections.Generic;
using DataAccessLayer.Entities;
using BusinessLayer.ViewModels;

namespace BusinessLayer.Services
{
    public interface IMappingService
    {
        // Product mappings
        ProductViewModel MapToProductViewModel(Product product);
        List<ProductViewModel> MapToProductViewModels(List<Product> products);
        Product MapToProduct(ProductCreateViewModel viewModel);
        Product MapToProduct(ProductEditViewModel viewModel);

        // Customer mappings
        CustomerViewModel MapToCustomerViewModel(Customer customer);
        Customer MapToCustomer(CustomerViewModel viewModel);
        List<CustomerViewModel> MapToCustomerViewModels(List<Customer> customers);

        // User mappings
        UserCreateViewModel MapToUserCreateViewModel(Users user);
        Users MapToUser(UserCreateViewModel viewModel);
        UserEditViewModel MapToUserEditViewModel(Users user);
        Users MapToUser(UserEditViewModel viewModel);

        // Brand mappings
        BrandViewModel MapToBrandViewModel(Brand brand);
        List<BrandViewModel> MapToBrandViewModels(List<Brand> brands);

        // Order mappings
        OrderCreateViewModel MapToOrderCreateViewModel(Order order);
        Order MapToOrder(OrderCreateViewModel viewModel);
        List<OrderCreateViewModel> MapToOrderCreateViewModels(List<Order> orders);

        // Feedback mappings
        FeedbackViewModel MapToFeedbackViewModel(Feedback feedback);
        List<FeedbackViewModel> MapToFeedbackViewModels(List<Feedback> feedbacks);
        Feedback MapToFeedback(FeedbackViewModel viewModel);

        // TestDrive mappings
        TestDriveViewModel MapToTestDriveViewModel(TestDrive testDrive);
        List<TestDriveViewModel> MapToTestDriveViewModels(List<TestDrive> testDrives);
        TestDrive MapToTestDrive(TestDriveViewModel viewModel);

        // PurchaseOrder mappings
        PurchaseOrderViewModel MapToPurchaseOrderViewModel(PurchaseOrder purchaseOrder);
        List<PurchaseOrderViewModel> MapToPurchaseOrderViewModels(List<PurchaseOrder> purchaseOrders);

        // User mappings
        UserViewModel MapToUserViewModel(Users user);
        List<UserViewModel> MapToUserViewModels(List<Users> users);

        // Dealer mappings
        DealerViewModel MapToDealerViewModel(Dealer dealer);
        List<DealerViewModel> MapToDealerViewModels(List<Dealer> dealers);

        // PricingPolicy mappings
        PricingPolicyViewModel MapToPricingPolicyViewModel(PricingPolicy pricingPolicy);
        List<PricingPolicyViewModel> MapToPricingPolicyViewModels(List<PricingPolicy> pricingPolicies);
        PricingPolicy MapToPricingPolicy(PricingPolicyViewModel viewModel);

        // InventoryAllocation mappings
        InventoryAllocationViewModel MapToInventoryAllocationViewModel(InventoryAllocation inventoryAllocation);
        List<InventoryAllocationViewModel> MapToInventoryAllocationViewModels(List<InventoryAllocation> inventoryAllocations);

        // InventoryTransaction mappings
        InventoryTransactionViewModel MapToInventoryTransactionViewModel(InventoryTransaction transaction);
        List<InventoryTransactionViewModel> MapToInventoryTransactionViewModels(List<InventoryTransaction> transactions);

        // DealerContract mappings
        DealerContractViewModel MapToDealerContractViewModel(DealerContract dealerContract);
        List<DealerContractViewModel> MapToDealerContractViewModels(List<DealerContract> dealerContracts);

        // Region mappings
        RegionViewModel MapToRegionViewModel(Region region);
        List<RegionViewModel> MapToRegionViewModels(List<Region> regions);
    }
}
