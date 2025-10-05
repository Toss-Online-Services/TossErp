import 'package:sqflite/sqflite.dart';
import '../../../app/database/app_database.dart';
import '../../../domain/entities/purchase_order_entity.dart';
import '../interfaces/purchase_order_datasource.dart';

class PurchaseOrderLocalDatasourceImpl implements PurchaseOrderDatasource {
  final AppDatabase _appDatabase;

  PurchaseOrderLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createPurchaseOrder(PurchaseOrderEntity purchaseOrder) async {
    final id = await _appDatabase.database.insert(
      AppDatabaseConfig.purchaseOrderTableName,
      _purchaseOrderToMap(purchaseOrder),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
    return id;
  }

  @override
  Future<List<PurchaseOrderEntity>> getAllPurchaseOrders() async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.purchaseOrderTableName,
      orderBy: 'orderDate DESC',
    );
    return res.map((row) => _mapToPurchaseOrder(row)).toList();
  }

  @override
  Future<PurchaseOrderEntity?> getPurchaseOrderById(int id) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.purchaseOrderTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
    if (res.isNotEmpty) {
      return _mapToPurchaseOrder(res.first);
    }
    return null;
  }

  @override
  Future<List<PurchaseOrderEntity>> getPurchaseOrdersBySupplier(int supplierId) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.purchaseOrderTableName,
      where: 'supplierId = ?',
      whereArgs: [supplierId],
      orderBy: 'orderDate DESC',
    );
    return res.map((row) => _mapToPurchaseOrder(row)).toList();
  }

  @override
  Future<List<PurchaseOrderEntity>> getPurchaseOrdersByStatus(PurchaseOrderStatus status) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.purchaseOrderTableName,
      where: 'status = ?',
      whereArgs: [status.name],
      orderBy: 'orderDate DESC',
    );
    return res.map((row) => _mapToPurchaseOrder(row)).toList();
  }

  @override
  Future<List<PurchaseOrderEntity>> getOverduePurchaseOrders() async {
    final now = DateTime.now().toIso8601String();
    final res = await _appDatabase.database.rawQuery('''
      SELECT * FROM ${AppDatabaseConfig.purchaseOrderTableName}
      WHERE expectedDate < ? AND status != 'received'
      ORDER BY expectedDate ASC
    ''', [now]);
    
    return res.map((row) => _mapToPurchaseOrder(row)).toList();
  }

  @override
  Future<void> updatePurchaseOrder(PurchaseOrderEntity purchaseOrder) async {
    await _appDatabase.database.update(
      AppDatabaseConfig.purchaseOrderTableName,
      _purchaseOrderToMap(purchaseOrder),
      where: 'id = ?',
      whereArgs: [purchaseOrder.id],
    );
  }

  @override
  Future<void> deletePurchaseOrder(int id) async {
    await _appDatabase.database.delete(
      AppDatabaseConfig.purchaseOrderTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<void> updatePurchaseOrderStatus(int id, PurchaseOrderStatus status) async {
    await _appDatabase.database.update(
      AppDatabaseConfig.purchaseOrderTableName,
      {'status': status.name, 'updatedAt': DateTime.now().toIso8601String()},
      where: 'id = ?',
      whereArgs: [id],
    );
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
        items: [
          PurchaseOrderItemEntity(
            purchaseOrderId: DateTime.now().millisecondsSinceEpoch,
            productId: 1,
            quantity: 100,
            unitPrice: 2500, // $25.00 in cents
            totalPrice: 250000, // $2500.00 in cents
            notes: 'High priority item',
          ),
          PurchaseOrderItemEntity(
            purchaseOrderId: DateTime.now().millisecondsSinceEpoch,
            productId: 2,
            quantity: 50,
            unitPrice: 5000, // $50.00 in cents
            totalPrice: 250000, // $2500.00 in cents
            notes: 'Standard item',
          ),
        ],
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(days: 5)).toIso8601String(),
        updatedAt: DateTime.now().subtract(const Duration(days: 5)).toIso8601String(),
      ),
      PurchaseOrderEntity(
        id: DateTime.now().millisecondsSinceEpoch + 1,
        orderNumber: 'PO-2024-002',
        supplierId: 2,
        status: PurchaseOrderStatus.pending,
        orderDate: DateTime.now().subtract(const Duration(days: 2)),
        expectedDate: DateTime.now().add(const Duration(days: 10)),
        totalAmount: 300000, // $3000.00 in cents
        taxAmount: 30000, // $300.00 in cents
        notes: 'Regular monthly order',
        items: [
          PurchaseOrderItemEntity(
            purchaseOrderId: DateTime.now().millisecondsSinceEpoch + 1,
            productId: 3,
            quantity: 75,
            unitPrice: 4000, // $40.00 in cents
            totalPrice: 300000, // $3000.00 in cents
            notes: 'Monthly restock',
          ),
        ],
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(days: 2)).toIso8601String(),
        updatedAt: DateTime.now().subtract(const Duration(days: 2)).toIso8601String(),
      ),
    ];

    for (final order in sampleOrders) {
      await createPurchaseOrder(order);
    }
  }

  Map<String, dynamic> _purchaseOrderToMap(PurchaseOrderEntity purchaseOrder) {
    return {
      'id': purchaseOrder.id,
      'orderNumber': purchaseOrder.orderNumber,
      'supplierId': purchaseOrder.supplierId,
      'status': purchaseOrder.status.name,
      'orderDate': purchaseOrder.orderDate.toIso8601String(),
      'expectedDate': purchaseOrder.expectedDate?.toIso8601String(),
      'receivedDate': purchaseOrder.receivedDate?.toIso8601String(),
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

  PurchaseOrderEntity _mapToPurchaseOrder(Map<String, dynamic> map) {
    return PurchaseOrderEntity(
      id: map['id'] as int,
      orderNumber: map['orderNumber'] as String,
      supplierId: map['supplierId'] as int,
      status: PurchaseOrderStatus.values.firstWhere(
        (e) => e.name == map['status'],
        orElse: () => PurchaseOrderStatus.draft,
      ),
      orderDate: DateTime.parse(map['orderDate'] as String),
      expectedDate: map['expectedDate'] != null ? 
        DateTime.parse(map['expectedDate'] as String) : null,
      receivedDate: map['receivedDate'] != null ? 
        DateTime.parse(map['receivedDate'] as String) : null,
      totalAmount: map['totalAmount'] as int,
      taxAmount: map['taxAmount'] as int?,
      discountAmount: map['discountAmount'] as int?,
      notes: map['notes'] as String?,
      items: [], // TODO: Load items from separate table
      createdById: map['createdById'] as String?,
      createdAt: map['createdAt'] as String?,
      updatedAt: map['updatedAt'] as String?,
    );
  }
}
