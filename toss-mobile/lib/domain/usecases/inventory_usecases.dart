import '../entities/inventory_movement_entity.dart';
import '../repositories/inventory_movement_repository.dart';
import '../../core/usecase/usecase.dart';
import '../../core/errors/errors.dart';

class GetAllInventoryMovements extends UseCase<Result, void> {
  final InventoryMovementRepository repository;

  GetAllInventoryMovements(this.repository);

  @override
  Future<Result<List<InventoryMovementEntity>>> call(void params) async {
    try {
      final movements = await repository.getAllInventoryMovements();
      return Result.success(movements);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetInventoryMovementsByProduct extends UseCase<Result, String> {
  final InventoryMovementRepository repository;

  GetInventoryMovementsByProduct(this.repository);

  @override
  Future<Result<List<InventoryMovementEntity>>> call(String productId) async {
    try {
      final movements = await repository.getInventoryMovementsByProduct(productId);
      return Result.success(movements);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetInventoryMovementsByType extends UseCase<Result, MovementType> {
  final InventoryMovementRepository repository;

  GetInventoryMovementsByType(this.repository);

  @override
  Future<Result<List<InventoryMovementEntity>>> call(MovementType type) async {
    try {
      final movements = await repository.getInventoryMovementsByType(type);
      return Result.success(movements);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetInventoryMovementsByDateRange extends UseCase<Result, DateRangeParams> {
  final InventoryMovementRepository repository;

  GetInventoryMovementsByDateRange(this.repository);

  @override
  Future<Result<List<InventoryMovementEntity>>> call(DateRangeParams params) async {
    try {
      final movements = await repository.getInventoryMovementsByDateRange(
        params.startDate,
        params.endDate,
      );
      return Result.success(movements);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetInventoryMovementsByLocation extends UseCase<Result, String> {
  final InventoryMovementRepository repository;

  GetInventoryMovementsByLocation(this.repository);

  @override
  Future<Result<List<InventoryMovementEntity>>> call(String locationId) async {
    try {
      final movements = await repository.getInventoryMovementsByLocation(locationId);
      return Result.success(movements);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetInventoryMovementById extends UseCase<Result, int> {
  final InventoryMovementRepository repository;

  GetInventoryMovementById(this.repository);

  @override
  Future<Result<InventoryMovementEntity?>> call(int id) async {
    try {
      final movement = await repository.getInventoryMovementById(id);
      return Result.success(movement);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class CreateInventoryMovement extends UseCase<Result, InventoryMovementEntity> {
  final InventoryMovementRepository repository;

  CreateInventoryMovement(this.repository);

  @override
  Future<Result<int>> call(InventoryMovementEntity movement) async {
    try {
      final movementId = await repository.createInventoryMovement(movement);
      return Result.success(movementId);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class UpdateInventoryMovement extends UseCase<Result, InventoryMovementEntity> {
  final InventoryMovementRepository repository;

  UpdateInventoryMovement(this.repository);

  @override
  Future<Result<void>> call(InventoryMovementEntity movement) async {
    try {
      await repository.updateInventoryMovement(movement);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class DeleteInventoryMovement extends UseCase<Result, int> {
  final InventoryMovementRepository repository;

  DeleteInventoryMovement(this.repository);

  @override
  Future<Result<void>> call(int id) async {
    try {
      await repository.deleteInventoryMovement(id);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetLowStockMovements extends UseCase<Result, void> {
  final InventoryMovementRepository repository;

  GetLowStockMovements(this.repository);

  @override
  Future<Result<List<InventoryMovementEntity>>> call(void params) async {
    try {
      final movements = await repository.getLowStockMovements();
      return Result.success(movements);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetExpiredProductMovements extends UseCase<Result, void> {
  final InventoryMovementRepository repository;

  GetExpiredProductMovements(this.repository);

  @override
  Future<Result<List<InventoryMovementEntity>>> call(void params) async {
    try {
      final movements = await repository.getExpiredProductMovements();
      return Result.success(movements);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetInventoryValuation extends UseCase<Result, void> {
  final InventoryMovementRepository repository;

  GetInventoryValuation(this.repository);

  @override
  Future<Result<Map<String, double>>> call(void params) async {
    try {
      final valuation = await repository.getInventoryValuation();
      return Result.success(valuation);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetInventoryTurnoverReport extends UseCase<Result, void> {
  final InventoryMovementRepository repository;

  GetInventoryTurnoverReport(this.repository);

  @override
  Future<Result<List<Map<String, dynamic>>>> call(void params) async {
    try {
      final report = await repository.getInventoryTurnoverReport();
      return Result.success(report);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class CreateStockAdjustment extends UseCase<Result, StockAdjustmentParams> {
  final InventoryMovementRepository repository;

  CreateStockAdjustment(this.repository);

  @override
  Future<Result<int>> call(StockAdjustmentParams params) async {
    try {
      final movementId = await repository.createStockAdjustment(
        params.productId,
        params.reason,
        params.quantity,
        params.notes,
        params.locationId,
      );
      return Result.success(movementId);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class CreateInventoryTransfer extends UseCase<Result, InventoryTransferParams> {
  final InventoryMovementRepository repository;

  CreateInventoryTransfer(this.repository);

  @override
  Future<Result<int>> call(InventoryTransferParams params) async {
    try {
      final movementId = await repository.createInventoryTransfer(
        params.productId,
        params.quantity,
        params.fromLocationId,
        params.toLocationId,
        params.notes,
      );
      return Result.success(movementId);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class SeedSampleInventoryMovements extends UseCase<Result, void> {
  final InventoryMovementRepository repository;

  SeedSampleInventoryMovements(this.repository);

  @override
  Future<Result<void>> call(void params) async {
    try {
      await repository.seedSampleInventoryMovements();
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

// Parameter classes
class DateRangeParams {
  final DateTime startDate;
  final DateTime endDate;

  DateRangeParams({
    required this.startDate,
    required this.endDate,
  });
}

class StockAdjustmentParams {
  final String productId;
  final String reason;
  final double quantity;
  final String? notes;
  final String? locationId;

  StockAdjustmentParams({
    required this.productId,
    required this.reason,
    required this.quantity,
    this.notes,
    this.locationId,
  });
}

class InventoryTransferParams {
  final String productId;
  final double quantity;
  final String fromLocationId;
  final String toLocationId;
  final String? notes;

  InventoryTransferParams({
    required this.productId,
    required this.quantity,
    required this.fromLocationId,
    required this.toLocationId,
    this.notes,
  });
}
