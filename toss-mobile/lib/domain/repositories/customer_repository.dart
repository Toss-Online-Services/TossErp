import '../../core/usecase/usecase.dart';
import '../entities/customer_entity.dart';

abstract class CustomerRepository {
  Future<Result<CustomerEntity>> upsert(CustomerEntity customer);
  Future<Result<CustomerEntity>> get(String idOrPhone);
  Future<Result<void>> addPoints(String id, int delta);
  Future<Result<List<CustomerEntity>>> search(String query);
}



