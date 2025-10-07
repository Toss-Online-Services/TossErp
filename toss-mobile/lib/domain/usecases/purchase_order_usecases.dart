import '../../core/usecase/usecase.dart';
import '../../core/errors/errors.dart';
import '../entities/purchase_order_entity.dart';
import '../repositories/purchase_order_repository.dart';

class GetAllPurchaseOrders extends UseCase<Result, void> {
  final PurchaseOrderRepository repository;

  GetAllPurchaseOrders(this.repository);

  @override
  Future<Result<List<PurchaseOrderEntity>>> call(void params) async {
    try {
      final orders = await repository.getAllPurchaseOrders();
      return Result.success(orders);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetPurchaseOrderById extends UseCase<Result, int> {
  final PurchaseOrderRepository repository;

  GetPurchaseOrderById(this.repository);

  @override
  Future<Result<PurchaseOrderEntity?>> call(int id) async {
    try {
      final order = await repository.getPurchaseOrderById(id);
      return Result.success(order);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetPurchaseOrdersBySupplier extends UseCase<Result, int> {
  final PurchaseOrderRepository repository;

  GetPurchaseOrdersBySupplier(this.repository);

  @override
  Future<Result<List<PurchaseOrderEntity>>> call(int supplierId) async {
    try {
      final orders = await repository.getPurchaseOrdersBySupplier(supplierId);
      return Result.success(orders);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetPurchaseOrdersByStatus extends UseCase<Result, PurchaseOrderStatus> {
  final PurchaseOrderRepository repository;

  GetPurchaseOrdersByStatus(this.repository);

  @override
  Future<Result<List<PurchaseOrderEntity>>> call(PurchaseOrderStatus status) async {
    try {
      final orders = await repository.getPurchaseOrdersByStatus(status);
      return Result.success(orders);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetOverduePurchaseOrders extends UseCase<Result, void> {
  final PurchaseOrderRepository repository;

  GetOverduePurchaseOrders(this.repository);

  @override
  Future<Result<List<PurchaseOrderEntity>>> call(void params) async {
    try {
      final orders = await repository.getOverduePurchaseOrders();
      return Result.success(orders);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class CreatePurchaseOrder extends UseCase<Result, PurchaseOrderEntity> {
  final PurchaseOrderRepository repository;

  CreatePurchaseOrder(this.repository);

  @override
  Future<Result<int>> call(PurchaseOrderEntity order) async {
    try {
      final id = await repository.createPurchaseOrder(order);
      return Result.success(id);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class UpdatePurchaseOrder extends UseCase<Result, PurchaseOrderEntity> {
  final PurchaseOrderRepository repository;

  UpdatePurchaseOrder(this.repository);

  @override
  Future<Result<void>> call(PurchaseOrderEntity order) async {
    try {
      await repository.updatePurchaseOrder(order);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class DeletePurchaseOrder extends UseCase<Result, int> {
  final PurchaseOrderRepository repository;

  DeletePurchaseOrder(this.repository);

  @override
  Future<Result<void>> call(int id) async {
    try {
      await repository.deletePurchaseOrder(id);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class UpdatePurchaseOrderStatus extends UseCase<Result, UpdatePurchaseOrderStatusParams> {
  final PurchaseOrderRepository repository;

  UpdatePurchaseOrderStatus(this.repository);

  @override
  Future<Result<void>> call(UpdatePurchaseOrderStatusParams params) async {
    try {
      await repository.updatePurchaseOrderStatus(params.id, params.status);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class SeedSamplePurchaseOrders extends UseCase<Result, void> {
  final PurchaseOrderRepository repository;

  SeedSamplePurchaseOrders(this.repository);

  @override
  Future<Result<void>> call(void params) async {
    try {
      await repository.seedSamplePurchaseOrders();
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class UpdatePurchaseOrderStatusParams {
  final int id;
  final PurchaseOrderStatus status;

  UpdatePurchaseOrderStatusParams({
    required this.id,
    required this.status,
  });
}
