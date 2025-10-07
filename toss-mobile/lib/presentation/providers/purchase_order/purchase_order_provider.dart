import 'package:flutter/foundation.dart';

import '../../../domain/entities/purchase_order_entity.dart';
import '../../../domain/usecases/purchase_order_usecases.dart';

class PurchaseOrderProvider extends ChangeNotifier {
  final GetAllPurchaseOrders _getAllPurchaseOrders;
  final GetPurchaseOrderById _getPurchaseOrderById;
  final GetPurchaseOrdersBySupplier _getPurchaseOrdersBySupplier;
  final GetPurchaseOrdersByStatus _getPurchaseOrdersByStatus;
  final GetOverduePurchaseOrders _getOverduePurchaseOrders;
  final CreatePurchaseOrder _createPurchaseOrder;
  final UpdatePurchaseOrder _updatePurchaseOrder;
  final DeletePurchaseOrder _deletePurchaseOrder;
  final UpdatePurchaseOrderStatus _updatePurchaseOrderStatus;
  final SeedSamplePurchaseOrders _seedSamplePurchaseOrders;

  PurchaseOrderProvider(
    this._getAllPurchaseOrders,
    this._getPurchaseOrderById,
    this._getPurchaseOrdersBySupplier,
    this._getPurchaseOrdersByStatus,
    this._getOverduePurchaseOrders,
    this._createPurchaseOrder,
    this._updatePurchaseOrder,
    this._deletePurchaseOrder,
    this._updatePurchaseOrderStatus,
    this._seedSamplePurchaseOrders,
  );

  // State
  List<PurchaseOrderEntity> _purchaseOrders = [];
  List<PurchaseOrderEntity> _filteredPurchaseOrders = [];
  PurchaseOrderEntity? _selectedPurchaseOrder;
  bool _isLoading = false;
  String? _error;
  String _searchQuery = '';
  PurchaseOrderStatus? _selectedStatus;
  int? _selectedSupplierId;

  // Getters
  List<PurchaseOrderEntity> get purchaseOrders => _filteredPurchaseOrders;
  PurchaseOrderEntity? get selectedPurchaseOrder => _selectedPurchaseOrder;
  bool get isLoading => _isLoading;
  String? get error => _error;
  String get searchQuery => _searchQuery;
  PurchaseOrderStatus? get selectedStatus => _selectedStatus;
  int? get selectedSupplierId => _selectedSupplierId;

  // Statistics
  int get totalPurchaseOrders => _purchaseOrders.length;
  int get draftOrders => _purchaseOrders.where((o) => o.status == PurchaseOrderStatus.draft).length;
  int get pendingOrders => _purchaseOrders.where((o) => o.status == PurchaseOrderStatus.pending).length;
  int get orderedOrders => _purchaseOrders.where((o) => o.status == PurchaseOrderStatus.ordered).length;
  int get overdueOrders => _purchaseOrders.where((o) => o.isOverdue).length;
  double get totalValue => _purchaseOrders.fold(0.0, (sum, o) => sum + (o.totalAmount / 100.0));

  // Actions
  Future<void> loadAllPurchaseOrders() async {
    _setLoading(true);
    _clearError();

    final result = await _getAllPurchaseOrders(null);
    if (result.isSuccess) {
      _purchaseOrders = result.data ?? [];
      _applyFilters();
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load purchase orders');
    }

    _setLoading(false);
  }

  Future<void> loadPurchaseOrderById(int id) async {
    _setLoading(true);
    _clearError();

    final result = await _getPurchaseOrderById(id);
    if (result.isSuccess) {
      _selectedPurchaseOrder = result.data;
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load purchase order');
    }

    _setLoading(false);
  }

  Future<void> loadPurchaseOrdersBySupplier(int supplierId) async {
    _selectedSupplierId = supplierId;
    _setLoading(true);
    _clearError();

    final result = await _getPurchaseOrdersBySupplier(supplierId);
    if (result.isSuccess) {
      _purchaseOrders = result.data ?? [];
      _applyFilters();
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load purchase orders for supplier');
    }

    _setLoading(false);
  }

  Future<void> loadPurchaseOrdersByStatus(PurchaseOrderStatus status) async {
    _selectedStatus = status;
    _setLoading(true);
    _clearError();

    final result = await _getPurchaseOrdersByStatus(status);
    if (result.isSuccess) {
      _purchaseOrders = result.data ?? [];
      _applyFilters();
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load purchase orders by status');
    }

    _setLoading(false);
  }

  Future<void> loadOverduePurchaseOrders() async {
    _setLoading(true);
    _clearError();

    final result = await _getOverduePurchaseOrders(null);
    if (result.isSuccess) {
      _purchaseOrders = result.data ?? [];
      _applyFilters();
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load overdue purchase orders');
    }

    _setLoading(false);
  }

  Future<bool> createPurchaseOrder(PurchaseOrderEntity purchaseOrder) async {
    _setLoading(true);
    _clearError();

    final result = await _createPurchaseOrder(purchaseOrder);
    if (result.isSuccess) {
      _setLoading(false);
      // Reload purchase orders to get updated list
      await loadAllPurchaseOrders();
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to create purchase order');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> updatePurchaseOrder(PurchaseOrderEntity purchaseOrder) async {
    _setLoading(true);
    _clearError();

    final result = await _updatePurchaseOrder(purchaseOrder);
    if (result.isSuccess) {
      _setLoading(false);
      // Update local list
      final index = _purchaseOrders.indexWhere((o) => o.id == purchaseOrder.id);
      if (index != -1) {
        _purchaseOrders[index] = purchaseOrder;
        _applyFilters();
        notifyListeners();
      }
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to update purchase order');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> deletePurchaseOrder(int id) async {
    _setLoading(true);
    _clearError();

    final result = await _deletePurchaseOrder(id);
    if (result.isSuccess) {
      _setLoading(false);
      // Remove from local list
      _purchaseOrders.removeWhere((o) => o.id == id);
      _applyFilters();
      notifyListeners();
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to delete purchase order');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> updatePurchaseOrderStatus(int id, PurchaseOrderStatus status) async {
    _setLoading(true);
    _clearError();

    final result = await _updatePurchaseOrderStatus(UpdatePurchaseOrderStatusParams(
      id: id,
      status: status,
    ));
    if (result.isSuccess) {
      _setLoading(false);
      // Update local list
      final index = _purchaseOrders.indexWhere((o) => o.id == id);
      if (index != -1) {
        _purchaseOrders[index] = _purchaseOrders[index].copyWith(status: status);
        _applyFilters();
        notifyListeners();
      }
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to update purchase order status');
      _setLoading(false);
      return false;
    }
  }

  Future<void> seedSamplePurchaseOrders() async {
    _setLoading(true);
    _clearError();

    final result = await _seedSamplePurchaseOrders(null);
    if (result.isSuccess) {
      _setLoading(false);
      // Reload purchase orders to get updated list
      await loadAllPurchaseOrders();
    } else {
      _setError(result.error?.message ?? 'Failed to seed sample purchase orders');
      _setLoading(false);
    }
  }

  void selectPurchaseOrder(PurchaseOrderEntity? purchaseOrder) {
    _selectedPurchaseOrder = purchaseOrder;
    notifyListeners();
  }

  void clearSelection() {
    _selectedPurchaseOrder = null;
    notifyListeners();
  }

  void searchPurchaseOrders(String query) {
    _searchQuery = query;
    _applyFilters();
    notifyListeners();
  }

  void filterByStatus(PurchaseOrderStatus? status) {
    _selectedStatus = status;
    _applyFilters();
    notifyListeners();
  }

  void filterBySupplier(int? supplierId) {
    _selectedSupplierId = supplierId;
    _applyFilters();
    notifyListeners();
  }

  void clearFilters() {
    _searchQuery = '';
    _selectedStatus = null;
    _selectedSupplierId = null;
    _applyFilters();
    notifyListeners();
  }

  // Private methods
  void _setLoading(bool loading) {
    _isLoading = loading;
    notifyListeners();
  }

  void _setError(String error) {
    _error = error;
    notifyListeners();
  }

  void _clearError() {
    _error = null;
  }

  void _applyFilters() {
    _filteredPurchaseOrders = _purchaseOrders.where((order) {
      // Search filter
      if (_searchQuery.isNotEmpty) {
        final query = _searchQuery.toLowerCase();
        final matchesSearch = 
            order.orderNumber.toLowerCase().contains(query) ||
            (order.notes?.toLowerCase().contains(query) ?? false);
        if (!matchesSearch) return false;
      }

      // Status filter
      if (_selectedStatus != null && order.status != _selectedStatus) {
        return false;
      }

      // Supplier filter
      if (_selectedSupplierId != null && order.supplierId != _selectedSupplierId) {
        return false;
      }

      return true;
    }).toList();
  }

  // Utility methods
  PurchaseOrderEntity createNewPurchaseOrder({
    required String orderNumber,
    required int supplierId,
    PurchaseOrderStatus status = PurchaseOrderStatus.draft,
    DateTime? orderDate,
    DateTime? expectedDate,
    required int totalAmount,
    int? taxAmount,
    int? discountAmount,
    String? notes,
    List<PurchaseOrderItemEntity> items = const [],
  }) {
    return PurchaseOrderEntity(
      id: DateTime.now().millisecondsSinceEpoch,
      orderNumber: orderNumber,
      supplierId: supplierId,
      status: status,
      orderDate: orderDate ?? DateTime.now(),
      expectedDate: expectedDate,
      totalAmount: totalAmount,
      taxAmount: taxAmount,
      discountAmount: discountAmount,
      notes: notes,
      items: items,
      createdById: 'current_user', // TODO: Get from auth
      createdAt: DateTime.now().toIso8601String(),
      updatedAt: DateTime.now().toIso8601String(),
    );
  }

  List<PurchaseOrderEntity> getRecentPurchaseOrders({int limit = 10}) {
    final sortedOrders = List<PurchaseOrderEntity>.from(_purchaseOrders);
    sortedOrders.sort((a, b) => b.orderDate.compareTo(a.orderDate));
    return sortedOrders.take(limit).toList();
  }

  List<PurchaseOrderEntity> getHighValueOrders({double minValue = 1000.0, int limit = 10}) {
    final minValueCents = (minValue * 100).toInt();
    final highValueOrders = _purchaseOrders
        .where((o) => o.totalAmount >= minValueCents)
        .toList();
    highValueOrders.sort((a, b) => b.totalAmount.compareTo(a.totalAmount));
    return highValueOrders.take(limit).toList();
  }
}
