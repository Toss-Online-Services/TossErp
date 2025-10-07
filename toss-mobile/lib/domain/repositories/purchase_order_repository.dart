import '../entities/purchase_order_entity.dart';

abstract class PurchaseOrderRepository {
  Future<List<PurchaseOrderEntity>> getAllPurchaseOrders();
  Future<PurchaseOrderEntity?> getPurchaseOrderById(int id);
  Future<List<PurchaseOrderEntity>> getPurchaseOrdersBySupplier(int supplierId);
  Future<List<PurchaseOrderEntity>> getPurchaseOrdersByStatus(PurchaseOrderStatus status);
  Future<List<PurchaseOrderEntity>> getOverduePurchaseOrders();
  Future<int> createPurchaseOrder(PurchaseOrderEntity purchaseOrder);
  Future<void> updatePurchaseOrder(PurchaseOrderEntity purchaseOrder);
  Future<void> deletePurchaseOrder(int id);
  Future<void> updatePurchaseOrderStatus(int id, PurchaseOrderStatus status);
  Future<void> seedSamplePurchaseOrders();
}
