import 'package:flutter/foundation.dart';

import '../../../domain/entities/inventory_movement_entity.dart';
import '../../../domain/usecases/inventory_usecases.dart';

class InventoryProvider extends ChangeNotifier {
  final GetAllInventoryMovements _getAllInventoryMovements;
  final GetInventoryMovementsByProduct _getInventoryMovementsByProduct;
  final GetInventoryMovementsByType _getInventoryMovementsByType;
  final GetInventoryMovementsByDateRange _getInventoryMovementsByDateRange;
  final GetInventoryMovementsByLocation _getInventoryMovementsByLocation;
  final GetInventoryMovementById _getInventoryMovementById;
  final CreateInventoryMovement _createInventoryMovement;
  final UpdateInventoryMovement _updateInventoryMovement;
  final DeleteInventoryMovement _deleteInventoryMovement;
  final GetLowStockMovements _getLowStockMovements;
  final GetExpiredProductMovements _getExpiredProductMovements;
  final GetInventoryValuation _getInventoryValuation;
  final GetInventoryTurnoverReport _getInventoryTurnoverReport;
  final CreateStockAdjustment _createStockAdjustment;
  final CreateInventoryTransfer _createInventoryTransfer;
  final SeedSampleInventoryMovements _seedSampleInventoryMovements;

  InventoryProvider(
    this._getAllInventoryMovements,
    this._getInventoryMovementsByProduct,
    this._getInventoryMovementsByType,
    this._getInventoryMovementsByDateRange,
    this._getInventoryMovementsByLocation,
    this._getInventoryMovementById,
    this._createInventoryMovement,
    this._updateInventoryMovement,
    this._deleteInventoryMovement,
    this._getLowStockMovements,
    this._getExpiredProductMovements,
    this._getInventoryValuation,
    this._getInventoryTurnoverReport,
    this._createStockAdjustment,
    this._createInventoryTransfer,
    this._seedSampleInventoryMovements,
  );

  // State
  List<InventoryMovementEntity> _movements = [];
  List<InventoryMovementEntity> _filteredMovements = [];
  InventoryMovementEntity? _selectedMovement;
  bool _isLoading = false;
  String? _error;
  String _searchQuery = '';
  MovementType? _selectedType;
  DateTime? _startDate;
  DateTime? _endDate;
  String? _selectedLocationId;
  String? _selectedProductId;

  // Getters
  List<InventoryMovementEntity> get movements => _filteredMovements;
  InventoryMovementEntity? get selectedMovement => _selectedMovement;
  bool get isLoading => _isLoading;
  String? get error => _error;
  String get searchQuery => _searchQuery;
  MovementType? get selectedType => _selectedType;
  DateTime? get startDate => _startDate;
  DateTime? get endDate => _endDate;
  String? get selectedLocationId => _selectedLocationId;
  String? get selectedProductId => _selectedProductId;

  // Statistics
  int get totalMovements => _movements.length;
  int get purchaseMovements => _movements.where((m) => m.type == MovementType.purchase).length;
  int get saleMovements => _movements.where((m) => m.type == MovementType.sale).length;
  int get adjustmentMovements => _movements.where((m) => m.type == MovementType.adjustment).length;
  int get transferMovements => _movements.where((m) => m.type == MovementType.transfer).length;
  double get totalValue => _movements.fold(0.0, (sum, m) => sum + (m.totalValue / 100.0));

  // Actions
  Future<void> loadAllMovements() async {
    _setLoading(true);
    _clearError();

    final result = await _getAllInventoryMovements(null);
    if (result.isSuccess) {
      _movements = result.data ?? [];
      _applyFilters();
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load inventory movements');
    }

    _setLoading(false);
  }

  Future<void> loadMovementsByProduct(String productId) async {
    _selectedProductId = productId;
    _setLoading(true);
    _clearError();

    final result = await _getInventoryMovementsByProduct(productId);
    if (result.isSuccess) {
      _filteredMovements = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load movements for product');
    }

    _setLoading(false);
  }

  Future<void> loadMovementsByType(MovementType type) async {
    _selectedType = type;
    _setLoading(true);
    _clearError();

    final result = await _getInventoryMovementsByType(type);
    if (result.isSuccess) {
      _filteredMovements = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load movements by type');
    }

    _setLoading(false);
  }

  Future<void> loadMovementsByDateRange(DateTime startDate, DateTime endDate) async {
    _startDate = startDate;
    _endDate = endDate;
    _setLoading(true);
    _clearError();

    final result = await _getInventoryMovementsByDateRange(DateRangeParams(
      startDate: startDate,
      endDate: endDate,
    ));
    if (result.isSuccess) {
      _filteredMovements = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load movements by date range');
    }

    _setLoading(false);
  }

  Future<void> loadMovementsByLocation(String locationId) async {
    _selectedLocationId = locationId;
    _setLoading(true);
    _clearError();

    final result = await _getInventoryMovementsByLocation(locationId);
    if (result.isSuccess) {
      _filteredMovements = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load movements by location');
    }

    _setLoading(false);
  }

  Future<void> loadMovementById(int id) async {
    _setLoading(true);
    _clearError();

    final result = await _getInventoryMovementById(id);
    if (result.isSuccess) {
      _selectedMovement = result.data;
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load movement');
    }

    _setLoading(false);
  }

  Future<bool> createMovement(InventoryMovementEntity movement) async {
    _setLoading(true);
    _clearError();

    final result = await _createInventoryMovement(movement);
    if (result.isSuccess) {
      _setLoading(false);
      // Reload movements to get updated list
      loadAllMovements();
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to create movement');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> updateMovement(InventoryMovementEntity movement) async {
    _setLoading(true);
    _clearError();

    final result = await _updateInventoryMovement(movement);
    if (result.isSuccess) {
      _setLoading(false);
      // Update local list
      final index = _movements.indexWhere((m) => m.id == movement.id);
      if (index != -1) {
        _movements[index] = movement;
        _applyFilters();
        notifyListeners();
      }
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to update movement');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> deleteMovement(int id) async {
    _setLoading(true);
    _clearError();

    final result = await _deleteInventoryMovement(id);
    if (result.isSuccess) {
      _setLoading(false);
      // Remove from local list
      _movements.removeWhere((m) => m.id == id);
      _applyFilters();
      notifyListeners();
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to delete movement');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> createStockAdjustment(
    String productId,
    String reason,
    double quantity,
    String? notes,
    String? locationId,
  ) async {
    _setLoading(true);
    _clearError();

    final result = await _createStockAdjustment(StockAdjustmentParams(
      productId: productId,
      reason: reason,
      quantity: quantity,
      notes: notes,
      locationId: locationId,
    ));
    if (result.isSuccess) {
      _setLoading(false);
      // Reload movements to get updated list
      loadAllMovements();
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to create stock adjustment');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> createInventoryTransfer(
    String productId,
    double quantity,
    String fromLocationId,
    String toLocationId,
    String? notes,
  ) async {
    _setLoading(true);
    _clearError();

    final result = await _createInventoryTransfer(InventoryTransferParams(
      productId: productId,
      quantity: quantity,
      fromLocationId: fromLocationId,
      toLocationId: toLocationId,
      notes: notes,
    ));
    if (result.isSuccess) {
      _setLoading(false);
      // Reload movements to get updated list
      loadAllMovements();
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to create inventory transfer');
      _setLoading(false);
      return false;
    }
  }

  Future<void> loadLowStockMovements() async {
    _setLoading(true);
    _clearError();

    final result = await _getLowStockMovements(null);
    if (result.isSuccess) {
      _filteredMovements = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load low stock movements');
    }

    _setLoading(false);
  }

  Future<void> loadExpiredProductMovements() async {
    _setLoading(true);
    _clearError();

    final result = await _getExpiredProductMovements(null);
    if (result.isSuccess) {
      _filteredMovements = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load expired product movements');
    }

    _setLoading(false);
  }

  Future<Map<String, double>?> loadInventoryValuation() async {
    _setLoading(true);
    _clearError();

    final result = await _getInventoryValuation(null);
    if (result.isSuccess) {
      _setLoading(false);
      return result.data;
    } else {
      _setError(result.error?.message ?? 'Failed to load inventory valuation');
      _setLoading(false);
      return null;
    }
  }

  Future<List<Map<String, dynamic>>?> loadInventoryTurnoverReport() async {
    _setLoading(true);
    _clearError();

    final result = await _getInventoryTurnoverReport(null);
    if (result.isSuccess) {
      _setLoading(false);
      return result.data;
    } else {
      _setError(result.error?.message ?? 'Failed to load inventory turnover report');
      _setLoading(false);
      return null;
    }
  }

  Future<void> seedSampleMovements() async {
    _setLoading(true);
    _clearError();

    final result = await _seedSampleInventoryMovements(null);
    if (result.isSuccess) {
      _setLoading(false);
      // Reload movements to get seeded data
      loadAllMovements();
    } else {
      _setError(result.error?.message ?? 'Failed to seed sample movements');
      _setLoading(false);
    }
  }

  void selectMovement(InventoryMovementEntity? movement) {
    _selectedMovement = movement;
    notifyListeners();
  }

  void clearFilters() {
    _searchQuery = '';
    _selectedType = null;
    _startDate = null;
    _endDate = null;
    _selectedLocationId = null;
    _selectedProductId = null;
    _filteredMovements = _movements;
    notifyListeners();
  }

  void clearError() {
    _clearError();
    notifyListeners();
  }

  // Helper methods
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
    _filteredMovements = _movements.where((movement) {
      // Search filter
      if (_searchQuery.isNotEmpty) {
        final query = _searchQuery.toLowerCase();
        final matchesSearch = 
            movement.reason.name.toLowerCase().contains(query) ||
            (movement.notes?.toLowerCase().contains(query) ?? false) ||
            movement.productId.toString().contains(query);
        if (!matchesSearch) return false;
      }

      // Type filter
      if (_selectedType != null && movement.type != _selectedType) {
        return false;
      }

      // Product filter
      if (_selectedProductId != null && movement.productId.toString() != _selectedProductId) {
        return false;
      }

      // Location filter
      if (_selectedLocationId != null && 
          movement.fromLocationId?.toString() != _selectedLocationId && 
          movement.toLocationId?.toString() != _selectedLocationId) {
        return false;
      }

      // Date range filter
      if (_startDate != null && _endDate != null) {
        if (movement.createdAt.isBefore(_startDate!) || movement.createdAt.isAfter(_endDate!)) {
          return false;
        }
      }

      return true;
    }).toList();
  }

  // Utility methods
  InventoryMovementEntity createNewMovement({
    required int productId,
    required MovementType type,
    required MovementReason reason,
    required int quantity,
    int? batchId,
    int? fromLocationId,
    int? toLocationId,
    String? notes,
  }) {
    return InventoryMovementEntity(
      id: DateTime.now().millisecondsSinceEpoch,
      productId: productId,
      batchId: batchId,
      type: type,
      reason: reason,
      quantity: quantity,
      unitPrice: 0,
      totalValue: 0,
      notes: notes,
      fromLocationId: fromLocationId,
      toLocationId: toLocationId,
      createdById: 'current_user', // TODO: Get from auth
      createdAt: DateTime.now(),
      updatedAt: DateTime.now().toIso8601String(),
    );
  }

  Map<MovementType, int> getMovementsByTypeCount() {
    final counts = <MovementType, int>{};
    for (final type in MovementType.values) {
      counts[type] = _movements.where((m) => m.type == type).length;
    }
    return counts;
  }

  List<InventoryMovementEntity> getRecentMovements({int limit = 10}) {
    final sortedMovements = List<InventoryMovementEntity>.from(_movements);
    sortedMovements.sort((a, b) => b.createdAt.compareTo(a.createdAt));
    return sortedMovements.take(limit).toList();
  }
}
