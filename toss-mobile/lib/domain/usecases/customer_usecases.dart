import '../entities/customer_entity.dart';
import '../repositories/customer_repository.dart';
import '../../core/usecase/usecase.dart';
import '../../core/errors/errors.dart';

class GetAllCustomers extends UseCase<Result, void> {
  final CustomerRepository repository;

  GetAllCustomers(this.repository);

  @override
  Future<Result<List<CustomerEntity>>> call(void params) async {
    try {
      final customers = await repository.getAllCustomers();
      return Result.success(customers);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetCustomerById extends UseCase<Result, String> {
  final CustomerRepository repository;

  GetCustomerById(this.repository);

  @override
  Future<Result<CustomerEntity?>> call(String id) async {
    try {
      final customer = await repository.getCustomerById(id);
      return Result.success(customer);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetCustomerByPhone extends UseCase<Result, String> {
  final CustomerRepository repository;

  GetCustomerByPhone(this.repository);

  @override
  Future<Result<CustomerEntity?>> call(String phone) async {
    try {
      final customer = await repository.getCustomerByPhone(phone);
      return Result.success(customer);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetCustomerByMembershipNumber extends UseCase<Result, String> {
  final CustomerRepository repository;

  GetCustomerByMembershipNumber(this.repository);

  @override
  Future<Result<CustomerEntity?>> call(String membershipNumber) async {
    try {
      final customer = await repository.getCustomerByMembershipNumber(membershipNumber);
      return Result.success(customer);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class SearchCustomers extends UseCase<Result, String> {
  final CustomerRepository repository;

  SearchCustomers(this.repository);

  @override
  Future<Result<List<CustomerEntity>>> call(String query) async {
    try {
      final result = await repository.search(query);
      if (result.isSuccess) {
        return Result.success(result.data);
      } else {
        return Result.error(result.error);
      }
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetCustomersByTier extends UseCase<Result, CustomerTier> {
  final CustomerRepository repository;

  GetCustomersByTier(this.repository);

  @override
  Future<Result<List<CustomerEntity>>> call(CustomerTier tier) async {
    try {
      final customers = await repository.getCustomersByTier(tier);
      return Result.success(customers);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetCustomersByLoyaltyTier extends UseCase<Result, LoyaltyTier> {
  final CustomerRepository repository;

  GetCustomersByLoyaltyTier(this.repository);

  @override
  Future<Result<List<CustomerEntity>>> call(LoyaltyTier loyaltyTier) async {
    try {
      final customers = await repository.getCustomersByLoyaltyTier(loyaltyTier);
      return Result.success(customers);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetCustomersByTag extends UseCase<Result, String> {
  final CustomerRepository repository;

  GetCustomersByTag(this.repository);

  @override
  Future<Result<List<CustomerEntity>>> call(String tag) async {
    try {
      final customers = await repository.getCustomersByTag(tag);
      return Result.success(customers);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetInactiveCustomers extends UseCase<Result, int> {
  final CustomerRepository repository;

  GetInactiveCustomers(this.repository);

  @override
  Future<Result<List<CustomerEntity>>> call(int daysThreshold) async {
    try {
      final customers = await repository.getInactiveCustomers(daysThreshold: daysThreshold);
      return Result.success(customers);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetBirthdayCustomers extends UseCase<Result, void> {
  final CustomerRepository repository;

  GetBirthdayCustomers(this.repository);

  @override
  Future<Result<List<CustomerEntity>>> call(void params) async {
    try {
      final customers = await repository.getBirthdayCustomers();
      return Result.success(customers);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetAnniversaryCustomers extends UseCase<Result, void> {
  final CustomerRepository repository;

  GetAnniversaryCustomers(this.repository);

  @override
  Future<Result<List<CustomerEntity>>> call(void params) async {
    try {
      final customers = await repository.getAnniversaryCustomers();
      return Result.success(customers);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class CreateCustomer extends UseCase<Result, CustomerEntity> {
  final CustomerRepository repository;

  CreateCustomer(this.repository);

  @override
  Future<Result<String>> call(CustomerEntity customer) async {
    try {
      final customerId = await repository.createCustomer(customer);
      return Result.success(customerId);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class UpdateCustomer extends UseCase<Result, CustomerEntity> {
  final CustomerRepository repository;

  UpdateCustomer(this.repository);

  @override
  Future<Result<void>> call(CustomerEntity customer) async {
    try {
      await repository.updateCustomer(customer);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class DeleteCustomer extends UseCase<Result, String> {
  final CustomerRepository repository;

  DeleteCustomer(this.repository);

  @override
  Future<Result<void>> call(String id) async {
    try {
      await repository.deleteCustomer(id);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class UpdateCustomerPoints extends UseCase<Result, UpdateCustomerPointsParams> {
  final CustomerRepository repository;

  UpdateCustomerPoints(this.repository);

  @override
  Future<Result<void>> call(UpdateCustomerPointsParams params) async {
    try {
      await repository.updateCustomerPoints(params.customerId, params.points);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class UpdateCustomerLoyaltyPoints extends UseCase<Result, UpdateCustomerLoyaltyPointsParams> {
  final CustomerRepository repository;

  UpdateCustomerLoyaltyPoints(this.repository);

  @override
  Future<Result<void>> call(UpdateCustomerLoyaltyPointsParams params) async {
    try {
      await repository.updateCustomerLoyaltyPoints(params.customerId, params.loyaltyPoints);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class UpdateCustomerVisit extends UseCase<Result, UpdateCustomerVisitParams> {
  final CustomerRepository repository;

  UpdateCustomerVisit(this.repository);

  @override
  Future<Result<void>> call(UpdateCustomerVisitParams params) async {
    try {
      await repository.updateCustomerVisit(params.customerId, params.amount);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class SeedSampleCustomers extends UseCase<Result, void> {
  final CustomerRepository repository;

  SeedSampleCustomers(this.repository);

  @override
  Future<Result<void>> call(void params) async {
    try {
      await repository.seedSampleCustomers();
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

// Parameter classes
class UpdateCustomerPointsParams {
  final String customerId;
  final int points;

  UpdateCustomerPointsParams({
    required this.customerId,
    required this.points,
  });
}

class UpdateCustomerLoyaltyPointsParams {
  final String customerId;
  final int loyaltyPoints;

  UpdateCustomerLoyaltyPointsParams({
    required this.customerId,
    required this.loyaltyPoints,
  });
}

class UpdateCustomerVisitParams {
  final String customerId;
  final double amount;

  UpdateCustomerVisitParams({
    required this.customerId,
    required this.amount,
  });
}
