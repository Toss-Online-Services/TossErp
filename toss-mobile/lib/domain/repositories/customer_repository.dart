import '../../core/usecase/usecase.dart';
import '../entities/customer_entity.dart';

abstract class CustomerRepository {
  Future<Result<CustomerEntity>> upsert(CustomerEntity customer);
  Future<Result<CustomerEntity>> get(String idOrPhone);
  Future<Result<void>> addPoints(String id, int delta);
  Future<Result<List<CustomerEntity>>> search(String query);
  
  // Additional methods for comprehensive CRM
  Future<List<CustomerEntity>> getAllCustomers();
  Future<CustomerEntity?> getCustomerById(String id);
  Future<CustomerEntity?> getCustomerByPhone(String phone);
  Future<CustomerEntity?> getCustomerByMembershipNumber(String membershipNumber);
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



