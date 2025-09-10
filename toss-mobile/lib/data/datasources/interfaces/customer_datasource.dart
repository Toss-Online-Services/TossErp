import '../../models/customer_model.dart';

abstract class CustomerDatasource {
  Future<String> upsertCustomer(CustomerModel customer);
  Future<CustomerModel?> getCustomer(String idOrPhone);
  Future<void> updatePoints(String id, int delta);
  Future<List<CustomerModel>> searchCustomers(String query);
}



