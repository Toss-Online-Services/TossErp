import '../../core/errors/errors.dart';
import '../../core/usecase/usecase.dart';
import '../../domain/entities/customer_entity.dart';
import '../../domain/repositories/customer_repository.dart';
import '../datasources/local/customer_local_datasource_impl.dart';
import '../models/customer_model.dart';

class CustomerRepositoryImpl extends CustomerRepository {
  final CustomerLocalDatasourceImpl local;
  CustomerRepositoryImpl({required this.local});

  @override
  Future<Result<void>> addPoints(String id, int delta) async {
    try {
      await local.updatePoints(id, delta);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<CustomerEntity>> get(String idOrPhone) async {
    try {
      final c = await local.getCustomer(idOrPhone);
      if (c == null) return Result.error(APIError(message: 'Not found'));
      return Result.success(CustomerEntity(
        id: c.id,
        name: c.name,
        phone: c.phone,
        pointsBalance: c.pointsBalance,
        createdAt: c.createdAt,
        updatedAt: c.updatedAt,
      ));
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<CustomerEntity>>> search(String query) async {
    try {
      final list = await local.searchCustomers(query);
      return Result.success(list
          .map((c) => CustomerEntity(
                id: c.id,
                name: c.name,
                phone: c.phone,
                pointsBalance: c.pointsBalance,
                createdAt: c.createdAt,
                updatedAt: c.updatedAt,
              ))
          .toList());
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<CustomerEntity>> upsert(CustomerEntity customer) async {
    try {
      final model = CustomerModel(
        id: customer.id,
        name: customer.name,
        phone: customer.phone,
        pointsBalance: customer.pointsBalance,
        createdAt: customer.createdAt ?? DateTime.now().toIso8601String(),
        updatedAt: customer.updatedAt ?? DateTime.now().toIso8601String(),
      );
      await local.upsertCustomer(model);
      return Result.success(customer);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}



