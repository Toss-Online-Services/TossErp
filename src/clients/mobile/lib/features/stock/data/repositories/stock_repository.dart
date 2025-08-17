import '../models/stock_item.dart';
import '../services/stock_api_service.dart';

class StockRepository {
  final StockApiService _stockApiService;

  StockRepository(this._stockApiService);

  // Stock Items
  
  Future<List<StockItem>> getStockItems({
    String? search,
    String? category,
    String? status,
    int? page,
    int? limit,
  }) async {
    try {
      return await _stockApiService.getStockItems(
        search: search,
        category: category,
        status: status,
        page: page,
        limit: limit,
      );
    } catch (e) {
      // Add offline support here if needed
      rethrow;
    }
  }

  Future<StockOverview> getStockOverview() async {
    try {
      return await _stockApiService.getStockOverview();
    } catch (e) {
      // Return cached data or default values if offline
      rethrow;
    }
  }

  Future<StockItem> getStockItem(String id) async {
    return await _stockApiService.getStockItem(id);
  }

  Future<StockItem> createStockItem(CreateStockItemRequest request) async {
    final item = await _stockApiService.createStockItem(request);
    // Add to local cache if needed
    return item;
  }

  Future<StockItem> updateStockItem(String id, Map<String, dynamic> updates) async {
    final item = await _stockApiService.updateStockItem(id, updates);
    // Update local cache if needed
    return item;
  }

  Future<void> deleteStockItem(String id) async {
    await _stockApiService.deleteStockItem(id);
    // Remove from local cache if needed
  }

  Future<StockMovement> adjustStock(StockAdjustmentRequest request) async {
    final movement = await _stockApiService.adjustStock(request);
    // Update local cache if needed
    return movement;
  }

  // Stock Movements
  
  Future<List<StockMovement>> getStockMovements({
    String? itemId,
    String? type,
    DateTime? startDate,
    DateTime? endDate,
    int? page,
    int? limit,
  }) async {
    return await _stockApiService.getStockMovements(
      itemId: itemId,
      type: type,
      startDate: startDate,
      endDate: endDate,
      page: page,
      limit: limit,
    );
  }

  Future<List<StockMovement>> getRecentMovements({int limit = 10}) async {
    return await _stockApiService.getRecentMovements(limit: limit);
  }

  // Categories
  
  Future<List<String>> getCategories() async {
    try {
      return await _stockApiService.getCategories();
    } catch (e) {
      // Return cached categories or default list
      return ['Electronics', 'Clothing', 'Food & Beverages', 'Home & Garden'];
    }
  }

  // Search and Filters
  
  Future<List<StockItem>> searchItems(String query) async {
    return await _stockApiService.searchItems(query);
  }

  Future<List<StockItem>> getLowStockItems() async {
    return await _stockApiService.getLowStockItems();
  }

  Future<List<StockItem>> getOutOfStockItems() async {
    return await _stockApiService.getOutOfStockItems();
  }

  // Bulk Operations
  
  Future<List<StockItem>> bulkUpdateItems(List<Map<String, dynamic>> updates) async {
    return await _stockApiService.bulkUpdateItems(updates);
  }

  // Import/Export
  
  Future<Map<String, dynamic>> importFromCsv(String filePath) async {
    return await _stockApiService.importFromCsv(filePath);
  }

  Future<String> exportToCsv({String? category, String? status}) async {
    return await _stockApiService.exportToCsv(category: category, status: status);
  }

  // Offline Support Methods (for future implementation)
  
  Future<void> syncOfflineChanges() async {
    // Implement sync logic for offline changes
    // This would handle uploading cached changes when connection is restored
  }

  Future<List<StockItem>> getCachedStockItems() async {
    // Return cached items from local storage
    // Placeholder for offline support
    return [];
  }

  Future<void> cacheStockItems(List<StockItem> items) async {
    // Cache items to local storage for offline access
    // Placeholder for offline support
  }

  Future<bool> isOnline() async {
    // Check network connectivity
    // Placeholder - would use connectivity_plus package
    return true;
  }

  // Utility Methods
  
  Future<double> calculateTotalStockValue({String? category}) async {
    try {
      final items = await getStockItems(category: category);
      return items.fold(0.0, (total, item) => total + (item.price * item.quantity));
    } catch (e) {
      return 0.0;
    }
  }

  Future<int> getTotalItemCount({String? category}) async {
    try {
      final items = await getStockItems(category: category);
      return items.length;
    } catch (e) {
      return 0;
    }
  }

  Future<List<StockItem>> getItemsByStatus(String status) async {
    return await getStockItems(status: status);
  }

  Future<List<StockItem>> getItemsByCategory(String category) async {
    return await getStockItems(category: category);
  }
}

