import '../../../domain/entities/customer_entity.dart';

abstract class CustomerDatasource {
  Future<List<CustomerEntity>> getAllCustomers();
  Future<CustomerEntity?> getCustomerById(String id);
  Future<CustomerEntity?> getCustomerByPhone(String phone);
  Future<CustomerEntity?> getCustomerByMembershipNumber(String membershipNumber);
  Future<List<CustomerEntity>> searchCustomers(String query);
  Future<List<CustomerEntity>> getCustomersByTier(CustomerTier tier);
  Future<List<CustomerEntity>> getCustomersByLoyaltyTier(LoyaltyTier loyaltyTier);
  Future<List<CustomerEntity>> getCustomersByTag(String tag);
  Future<List<CustomerEntity>> getInactiveCustomers({int daysThreshold = 90});
  Future<List<CustomerEntity>> getBirthdayCustomers();
  Future<List<CustomerEntity>> getAnniversaryCustomers();
  Future<String> createCustomer(CustomerEntity customer);
  Future<void> updateCustomer(CustomerEntity customer);
  Future<void> deleteCustomer(String id);
  Future<void> updateCustomerPoints(String customerId, int points);
  Future<void> updateCustomerLoyaltyPoints(String customerId, int loyaltyPoints);
  Future<void> updateCustomerVisit(String customerId, double amount);
  Future<void> seedSampleCustomers();
}