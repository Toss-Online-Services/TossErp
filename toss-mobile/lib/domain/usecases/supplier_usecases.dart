import '../../core/usecase/usecase.dart';
import '../../core/errors/errors.dart';
import '../entities/supplier_entity.dart';
import '../repositories/supplier_repository.dart';

class GetAllSuppliers extends UseCase<Result, void> {
  final SupplierRepository repository;

  GetAllSuppliers(this.repository);

  @override
  Future<Result<List<SupplierEntity>>> call(void params) async {
    try {
      final suppliers = await repository.getAllSuppliers();
      return Result.success(suppliers);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetSupplierById extends UseCase<Result, int> {
  final SupplierRepository repository;

  GetSupplierById(this.repository);

  @override
  Future<Result<SupplierEntity?>> call(int id) async {
    try {
      final supplier = await repository.getSupplierById(id);
      return Result.success(supplier);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class SearchSuppliers extends UseCase<Result, String> {
  final SupplierRepository repository;

  SearchSuppliers(this.repository);

  @override
  Future<Result<List<SupplierEntity>>> call(String query) async {
    try {
      final suppliers = await repository.searchSuppliers(query);
      return Result.success(suppliers);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetActiveSuppliers extends UseCase<Result, void> {
  final SupplierRepository repository;

  GetActiveSuppliers(this.repository);

  @override
  Future<Result<List<SupplierEntity>>> call(void params) async {
    try {
      final suppliers = await repository.getActiveSuppliers();
      return Result.success(suppliers);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class CreateSupplier extends UseCase<Result, SupplierEntity> {
  final SupplierRepository repository;

  CreateSupplier(this.repository);

  @override
  Future<Result<int>> call(SupplierEntity supplier) async {
    try {
      final id = await repository.createSupplier(supplier);
      return Result.success(id);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class UpdateSupplier extends UseCase<Result, SupplierEntity> {
  final SupplierRepository repository;

  UpdateSupplier(this.repository);

  @override
  Future<Result<void>> call(SupplierEntity supplier) async {
    try {
      await repository.updateSupplier(supplier);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class DeleteSupplier extends UseCase<Result, int> {
  final SupplierRepository repository;

  DeleteSupplier(this.repository);

  @override
  Future<Result<void>> call(int id) async {
    try {
      await repository.deleteSupplier(id);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class SeedSampleSuppliers extends UseCase<Result, void> {
  final SupplierRepository repository;

  SeedSampleSuppliers(this.repository);

  @override
  Future<Result<void>> call(void params) async {
    try {
      await repository.seedSampleSuppliers();
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}
