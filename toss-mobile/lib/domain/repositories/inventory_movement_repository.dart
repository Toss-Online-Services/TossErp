import '../entities/inventory_movement_entity.dart';

abstract class InventoryMovementRepository {
  Future<List<InventoryMovementEntity>> getAllInventoryMovements();
  Future<List<InventoryMovementEntity>> getInventoryMovementsByProduct(String productId);
  Future<List<InventoryMovementEntity>> getInventoryMovementsByType(MovementType type);
  Future<List<InventoryMovementEntity>> getInventoryMovementsByDateRange(
    DateTime startDate,
    DateTime endDate,
  );
  Future<List<InventoryMovementEntity>> getInventoryMovementsByLocation(String locationId);
  Future<InventoryMovementEntity?> getInventoryMovementById(int id);
  Future<int> createInventoryMovement(InventoryMovementEntity movement);
  Future<void> updateInventoryMovement(InventoryMovementEntity movement);
  Future<void> deleteInventoryMovement(int id);
  Future<List<InventoryMovementEntity>> getLowStockMovements();
  Future<List<InventoryMovementEntity>> getExpiredProductMovements();
  Future<Map<String, double>> getInventoryValuation();
  Future<List<Map<String, dynamic>>> getInventoryTurnoverReport();
  Future<int> createStockAdjustment(
    String productId,
    String reason,
    double quantity,
    String? notes,
    String? locationId,
  );
  Future<int> createInventoryTransfer(
    String productId,
    double quantity,
    String fromLocationId,
    String toLocationId,
    String? notes,
  );
  Future<void> seedSampleInventoryMovements();
}
