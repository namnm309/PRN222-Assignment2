using System.Collections.Generic;
using DataAccessLayer.Entities;
using BusinessLayer.DTOs.Requests;
using BusinessLayer.DTOs.Responses;

namespace BusinessLayer.Services
{
    public interface IMappingService
    {
        // Product mappings
        ProductResponse MapToProductViewModel(Product product);
        List<ProductResponse> MapToProductViewModels(List<Product> products);
        Product MapToProduct(ProductCreateRequest viewModel);
        Product MapToProduct(ProductEditRequest viewModel);

        // Customer mappings
        CustomerResponse MapToCustomerViewModel(Customer customer);
        Customer MapToCustomer(CustomerResponse viewModel);
        List<CustomerResponse> MapToCustomerViewModels(List<Customer> customers);

        // User mappings
        UserCreateRequest MapToUserCreateViewModel(Users user);
        Users MapToUser(UserCreateRequest viewModel);
        UserEditRequest MapToUserEditViewModel(Users user);
        Users MapToUser(UserEditRequest viewModel);

        // Brand mappings
        BrandResponse MapToBrandViewModel(Brand brand);
        List<BrandResponse> MapToBrandViewModels(List<Brand> brands);

        // Order mappings
        OrderResponse MapToOrderCreateViewModel(Order order);
        Order MapToOrder(OrderCreateRequest viewModel);
        List<OrderResponse> MapToOrderCreateViewModels(List<Order> orders);

        // Feedback mappings
        FeedbackResponse MapToFeedbackViewModel(Feedback feedback);
        List<FeedbackResponse> MapToFeedbackViewModels(List<Feedback> feedbacks);
        Feedback MapToFeedback(FeedbackCreateRequest viewModel);

        // TestDrive mappings
        TestDriveResponse MapToTestDriveViewModel(TestDrive testDrive);
        List<TestDriveResponse> MapToTestDriveViewModels(List<TestDrive> testDrives);
        TestDrive MapToTestDrive(TestDriveCreateRequest viewModel);

        // PurchaseOrder mappings
        PurchaseOrderResponse MapToPurchaseOrderViewModel(PurchaseOrder purchaseOrder);
        List<PurchaseOrderResponse> MapToPurchaseOrderViewModels(List<PurchaseOrder> purchaseOrders);

        // User mappings
        UserResponse MapToUserViewModel(Users user);
        List<UserResponse> MapToUserViewModels(List<Users> users);

        // Dealer mappings
        DealerResponse MapToDealerViewModel(Dealer dealer);
        List<DealerResponse> MapToDealerViewModels(List<Dealer> dealers);

        // PricingPolicy mappings
        PricingPolicyResponse MapToPricingPolicyViewModel(PricingPolicy pricingPolicy);
        List<PricingPolicyResponse> MapToPricingPolicyViewModels(List<PricingPolicy> pricingPolicies);
        PricingPolicy MapToPricingPolicy(PricingPolicyResponse viewModel);

        // InventoryAllocation mappings
        InventoryAllocationResponse MapToInventoryAllocationViewModel(InventoryAllocation inventoryAllocation);
        List<InventoryAllocationResponse> MapToInventoryAllocationViewModels(List<InventoryAllocation> inventoryAllocations);

        // InventoryTransaction mappings
        InventoryTransactionResponse MapToInventoryTransactionViewModel(InventoryTransaction transaction);
        List<InventoryTransactionResponse> MapToInventoryTransactionViewModels(List<InventoryTransaction> transactions);

        // DealerContract mappings
        DealerContractResponse MapToDealerContractViewModel(DealerContract dealerContract);
        List<DealerContractResponse> MapToDealerContractViewModels(List<DealerContract> dealerContracts);

        // Region mappings
        RegionResponse MapToRegionViewModel(Region region);
        List<RegionResponse> MapToRegionViewModels(List<Region> regions);
    }
}
