import 'package:flutter/foundation.dart';

// Simple inventory item model
class InventoryItem {
  final int id;
  final int productId;
  final String productName;
  final String sku;
  final int quantity;
  final int minimumStock;
  final int maximumStock;
  final int costPrice;
  final int sellingPrice;
  final int locationId;
  final String locationName;
  final DateTime lastUpdated;

  InventoryItem({
    required this.id,
    required this.productId,
    required this.productName,
    required this.sku,
    required this.quantity,
    required this.minimumStock,
    required this.maximumStock,
    required this.costPrice,
    required this.sellingPrice,
    required this.locationId,
    required this.locationName,
    required this.lastUpdated,
  });
}

class SimpleInventoryProvider extends ChangeNotifier {
  // State
  List<InventoryItem> _inventory = [];
  bool _isLoading = false;
  String? _error;

  // Getters
  List<InventoryItem> get inventory => _inventory;
  bool get isLoading => _isLoading;
  String? get error => _error;

  // Statistics
  int get totalProducts => _inventory.length;
  int get lowStockProducts => _inventory.where((item) => item.quantity <= item.minimumStock).length;
  int get outOfStockProducts => _inventory.where((item) => item.quantity == 0).length;
  double get totalValue => _inventory.fold(0.0, (sum, item) => sum + (item.quantity * item.sellingPrice / 100.0));

  // Actions
  Future<void> loadAllInventory() async {
    _setLoading(true);
    _clearError();

    try {
      // Mock data for now - in real implementation, this would load from repository
      await Future.delayed(const Duration(milliseconds: 500));
      
      _inventory = _generateMockInventory();
      notifyListeners();
    } catch (e) {
      _setError('Failed to load inventory: $e');
    }

    _setLoading(false);
  }

  List<InventoryItem> _generateMockInventory() {
    return [
      InventoryItem(
        id: 1,
        productId: 1,
        productName: 'Coca Cola 330ml',
        sku: 'COCA-330',
        quantity: 50,
        minimumStock: 10,
        maximumStock: 100,
        costPrice: 800, // 8.00 in cents
        sellingPrice: 1200, // 12.00 in cents
        locationId: 1,
        locationName: 'Main Store',
        lastUpdated: DateTime.now(),
      ),
      InventoryItem(
        id: 2,
        productId: 2,
        productName: 'Bread Loaf',
        sku: 'BREAD-001',
        quantity: 5,
        minimumStock: 10,
        maximumStock: 50,
        costPrice: 1500, // 15.00 in cents
        sellingPrice: 2000, // 20.00 in cents
        locationId: 1,
        locationName: 'Main Store',
        lastUpdated: DateTime.now(),
      ),
      InventoryItem(
        id: 3,
        productId: 3,
        productName: 'Milk 1L',
        sku: 'MILK-1L',
        quantity: 0,
        minimumStock: 5,
        maximumStock: 30,
        costPrice: 1200, // 12.00 in cents
        sellingPrice: 1800, // 18.00 in cents
        locationId: 1,
        locationName: 'Main Store',
        lastUpdated: DateTime.now(),
      ),
      InventoryItem(
        id: 4,
        productId: 4,
        productName: 'Chocolate Bar',
        sku: 'CHOC-001',
        quantity: 25,
        minimumStock: 5,
        maximumStock: 50,
        costPrice: 500, // 5.00 in cents
        sellingPrice: 800, // 8.00 in cents
        locationId: 1,
        locationName: 'Main Store',
        lastUpdated: DateTime.now(),
      ),
      InventoryItem(
        id: 5,
        productId: 5,
        productName: 'Water Bottle 500ml',
        sku: 'WATER-500',
        quantity: 100,
        minimumStock: 20,
        maximumStock: 200,
        costPrice: 300, // 3.00 in cents
        sellingPrice: 500, // 5.00 in cents
        locationId: 1,
        locationName: 'Main Store',
        lastUpdated: DateTime.now(),
      ),
    ];
  }

  List<InventoryItem> getLowStockProducts() {
    return _inventory.where((item) => item.quantity <= item.minimumStock).toList();
  }

  List<InventoryItem> getOutOfStockProducts() {
    return _inventory.where((item) => item.quantity == 0).toList();
  }

  InventoryItem? getProductById(int productId) {
    try {
      return _inventory.firstWhere((item) => item.productId == productId);
    } catch (e) {
      return null;
    }
  }

  List<InventoryItem> searchProducts(String query) {
    if (query.isEmpty) return _inventory;
    
    final lowercaseQuery = query.toLowerCase();
    return _inventory.where((item) => 
      item.productName.toLowerCase().contains(lowercaseQuery) ||
      item.sku.toLowerCase().contains(lowercaseQuery)
    ).toList();
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
}

