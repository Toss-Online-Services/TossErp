import 'package:cloud_firestore/cloud_firestore.dart';
import '../../../domain/entities/purchase_order_entity.dart';
import '../interfaces/purchase_order_datasource.dart';

class PurchaseOrderRemoteDatasourceImpl implements PurchaseOrderDatasource {
  final FirebaseFirestore _firestore;
  final String _collection = 'purchase_orders';

  PurchaseOrderRemoteDatasourceImpl(this._firestore);

  @override
  Future<int> createPurchaseOrder(PurchaseOrderEntity purchaseOrder) async {
    final docRef = await _firestore.collection(_collection).add(_purchaseOrderToMap(purchaseOrder));
    return int.parse(docRef.id);
  }

  @override
  Future<List<PurchaseOrderEntity>> getAllPurchaseOrders() async {
    final snapshot = await _firestore
        .collection(_collection)
        .orderBy('orderDate', descending: true)
        .get();
    
    return snapshot.docs.map((doc) => _mapToPurchaseOrder(doc.data(), doc.id)).toList();
  }

  @override
  Future<PurchaseOrderEntity?> getPurchaseOrderById(int id) async {
    final doc = await _firestore.collection(_collection).doc(id.toString()).get();
    if (doc.exists) {
      return _mapToPurchaseOrder(doc.data()!, doc.id);
    }
    return null;
  }

  @override
  Future<List<PurchaseOrderEntity>> getPurchaseOrdersBySupplier(int supplierId) async {
    final snapshot = await _firestore
        .collection(_collection)
        .where('supplierId', isEqualTo: supplierId)
        .orderBy('orderDate', descending: true)
        .get();
    
    return snapshot.docs.map((doc) => _mapToPurchaseOrder(doc.data(), doc.id)).toList();
  }

  @override
  Future<List<PurchaseOrderEntity>> getPurchaseOrdersByStatus(PurchaseOrderStatus status) async {
    final snapshot = await _firestore
        .collection(_collection)
        .where('status', isEqualTo: status.name)
        .orderBy('orderDate', descending: true)
        .get();
    
    return snapshot.docs.map((doc) => _mapToPurchaseOrder(doc.data(), doc.id)).toList();
  }

  @override
  Future<List<PurchaseOrderEntity>> getOverduePurchaseOrders() async {
    final now = DateTime.now();
    final snapshot = await _firestore
        .collection(_collection)
        .where('expectedDate', isLessThan: now)
        .where('status', isNotEqualTo: 'received')
        .orderBy('expectedDate')
        .get();
    
    return snapshot.docs.map((doc) => _mapToPurchaseOrder(doc.data(), doc.id)).toList();
  }

  @override
  Future<void> updatePurchaseOrder(PurchaseOrderEntity purchaseOrder) async {
    await _firestore.collection(_collection).doc(purchaseOrder.id.toString()).update(_purchaseOrderToMap(purchaseOrder));
  }

  @override
  Future<void> deletePurchaseOrder(int id) async {
    await _firestore.collection(_collection).doc(id.toString()).delete();
  }

  @override
  Future<void> updatePurchaseOrderStatus(int id, PurchaseOrderStatus status) async {
    await _firestore.collection(_collection).doc(id.toString()).update({
      'status': status.name,
      'updatedAt': DateTime.now().toIso8601String(),
    });
  }

  @override
  Future<void> seedSamplePurchaseOrders() async {
    final sampleOrders = [
      PurchaseOrderEntity(
        id: DateTime.now().millisecondsSinceEpoch,
        orderNumber: 'PO-2024-001',
        supplierId: 1,
        status: PurchaseOrderStatus.ordered,
        orderDate: DateTime.now().subtract(const Duration(days: 5)),
        expectedDate: DateTime.now().add(const Duration(days: 7)),
        totalAmount: 500000, // $5000.00 in cents
        taxAmount: 50000, // $500.00 in cents
        discountAmount: 10000, // $100.00 in cents
        notes: 'Urgent order for Q1 inventory',
        items: [],
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(days: 5)).toIso8601String(),
        updatedAt: DateTime.now().subtract(const Duration(days: 5)).toIso8601String(),
      ),
    ];

    for (final order in sampleOrders) {
      await createPurchaseOrder(order);
    }
  }

  Map<String, dynamic> _purchaseOrderToMap(PurchaseOrderEntity purchaseOrder) {
    return {
      'orderNumber': purchaseOrder.orderNumber,
      'supplierId': purchaseOrder.supplierId,
      'status': purchaseOrder.status.name,
      'orderDate': purchaseOrder.orderDate,
      'expectedDate': purchaseOrder.expectedDate,
      'receivedDate': purchaseOrder.receivedDate,
      'totalAmount': purchaseOrder.totalAmount,
      'taxAmount': purchaseOrder.taxAmount,
      'discountAmount': purchaseOrder.discountAmount,
      'notes': purchaseOrder.notes,
      'items': purchaseOrder.items.map((item) => item.toMap()).toList(),
      'createdById': purchaseOrder.createdById,
      'createdAt': purchaseOrder.createdAt,
      'updatedAt': purchaseOrder.updatedAt,
    };
  }

  PurchaseOrderEntity _mapToPurchaseOrder(Map<String, dynamic> map, String id) {
    return PurchaseOrderEntity(
      id: int.parse(id),
      orderNumber: map['orderNumber'] as String,
      supplierId: map['supplierId'] as int,
      status: PurchaseOrderStatus.values.firstWhere(
        (e) => e.name == map['status'],
        orElse: () => PurchaseOrderStatus.draft,
      ),
      orderDate: (map['orderDate'] as Timestamp).toDate(),
      expectedDate: map['expectedDate'] != null ? 
        (map['expectedDate'] as Timestamp).toDate() : null,
      receivedDate: map['receivedDate'] != null ? 
        (map['receivedDate'] as Timestamp).toDate() : null,
      totalAmount: map['totalAmount'] as int,
      taxAmount: map['taxAmount'] as int?,
      discountAmount: map['discountAmount'] as int?,
      notes: map['notes'] as String?,
      items: [], // TODO: Load items from separate collection
      createdById: map['createdById'] as String?,
      createdAt: map['createdAt'] as String?,
      updatedAt: map['updatedAt'] as String?,
    );
  }
}
