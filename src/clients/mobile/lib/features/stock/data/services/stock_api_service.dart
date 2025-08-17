import 'package:dio/dio.dart';
import '../models/stock_item.dart';

class StockApiService {
  final Dio _dio;

  StockApiService(this._dio);

  // Stock Items API
  
  /// Get all stock items with optional filtering
  Future<List<StockItem>> getStockItems({
    String? search,
    String? category,
    String? status,
    int? page,
    int? limit,
  }) async {
    try {
      final queryParams = <String, dynamic>{};
      if (search != null) queryParams['search'] = search;
      if (category != null) queryParams['category'] = category;
      if (status != null) queryParams['status'] = status;
      if (page != null) queryParams['page'] = page;
      if (limit != null) queryParams['limit'] = limit;

      final response = await _dio.get('/stock/items', queryParameters: queryParams);
      
      if (response.data is List) {
        return (response.data as List)
            .map((item) => StockItem.fromJson(item as Map<String, dynamic>))
            .toList();
      }
      
      // Handle API response wrapper format
      final items = response.data['data'] ?? response.data['items'] ?? [];
      return (items as List)
          .map((item) => StockItem.fromJson(item as Map<String, dynamic>))
          .toList();
    } catch (e) {
      throw _handleApiError(e);
    }
  }

  /// Get stock overview/summary
  Future<StockOverview> getStockOverview() async {
    try {
      final response = await _dio.get('/stock/overview');
      return StockOverview.fromJson(response.data);
    } catch (e) {
      throw _handleApiError(e);
    }
  }

  /// Get stock item by ID
  Future<StockItem> getStockItem(String id) async {
    try {
      final response = await _dio.get('/stock/items/$id');
      return StockItem.fromJson(response.data);
    } catch (e) {
      throw _handleApiError(e);
    }
  }

  /// Create new stock item
  Future<StockItem> createStockItem(CreateStockItemRequest request) async {
    try {
      final response = await _dio.post('/stock/items', data: request.toJson());
      return StockItem.fromJson(response.data);
    } catch (e) {
      throw _handleApiError(e);
    }
  }

  /// Update stock item
  Future<StockItem> updateStockItem(String id, Map<String, dynamic> updates) async {
    try {
      final response = await _dio.put('/stock/items/$id', data: updates);
      return StockItem.fromJson(response.data);
    } catch (e) {
      throw _handleApiError(e);
    }
  }

  /// Delete stock item
  Future<void> deleteStockItem(String id) async {
    try {
      await _dio.delete('/stock/items/$id');
    } catch (e) {
      throw _handleApiError(e);
    }
  }

  /// Adjust stock quantity
  Future<StockMovement> adjustStock(StockAdjustmentRequest request) async {
    try {
      final response = await _dio.post('/stock/adjustments', data: request.toJson());
      return StockMovement.fromJson(response.data);
    } catch (e) {
      throw _handleApiError(e);
    }
  }

  // Stock Movements API

  /// Get stock movements with optional filtering
  Future<List<StockMovement>> getStockMovements({
    String? itemId,
    String? type,
    DateTime? startDate,
    DateTime? endDate,
    int? page,
    int? limit,
  }) async {
    try {
      final queryParams = <String, dynamic>{};
      if (itemId != null) queryParams['itemId'] = itemId;
      if (type != null) queryParams['type'] = type;
      if (startDate != null) queryParams['startDate'] = startDate.toIso8601String();
      if (endDate != null) queryParams['endDate'] = endDate.toIso8601String();
      if (page != null) queryParams['page'] = page;
      if (limit != null) queryParams['limit'] = limit;

      final response = await _dio.get('/stock/movements', queryParameters: queryParams);
      
      if (response.data is List) {
        return (response.data as List)
            .map((movement) => StockMovement.fromJson(movement as Map<String, dynamic>))
            .toList();
      }
      
      // Handle API response wrapper format
      final movements = response.data['data'] ?? response.data['movements'] ?? [];
      return (movements as List)
          .map((movement) => StockMovement.fromJson(movement as Map<String, dynamic>))
          .toList();
    } catch (e) {
      throw _handleApiError(e);
    }
  }

  /// Get recent stock movements (last 10 by default)
  Future<List<StockMovement>> getRecentMovements({int limit = 10}) async {
    return await getStockMovements(limit: limit);
  }

  // Categories API

  /// Get all stock categories
  Future<List<String>> getCategories() async {
    try {
      final response = await _dio.get('/stock/categories');
      
      if (response.data is List) {
        return (response.data as List).cast<String>();
      }
      
      // Handle API response wrapper format
      final categories = response.data['data'] ?? response.data['categories'] ?? [];
      return (categories as List).cast<String>();
    } catch (e) {
      throw _handleApiError(e);
    }
  }

  /// Search stock items
  Future<List<StockItem>> searchItems(String query) async {
    return await getStockItems(search: query);
  }

  /// Get low stock items
  Future<List<StockItem>> getLowStockItems() async {
    return await getStockItems(status: 'low-stock');
  }

  /// Get out of stock items
  Future<List<StockItem>> getOutOfStockItems() async {
    return await getStockItems(status: 'out-of-stock');
  }

  /// Bulk update stock items
  Future<List<StockItem>> bulkUpdateItems(List<Map<String, dynamic>> updates) async {
    try {
      final response = await _dio.post('/stock/items/bulk-update', data: {'updates': updates});
      
      if (response.data is List) {
        return (response.data as List)
            .map((item) => StockItem.fromJson(item as Map<String, dynamic>))
            .toList();
      }
      
      final items = response.data['data'] ?? response.data['items'] ?? [];
      return (items as List)
          .map((item) => StockItem.fromJson(item as Map<String, dynamic>))
          .toList();
    } catch (e) {
      throw _handleApiError(e);
    }
  }

  /// Import stock items from CSV
  Future<Map<String, dynamic>> importFromCsv(String filePath) async {
    try {
      final formData = FormData.fromMap({
        'file': await MultipartFile.fromFile(filePath),
      });
      
      final response = await _dio.post('/stock/import', data: formData);
      return response.data;
    } catch (e) {
      throw _handleApiError(e);
    }
  }

  /// Export stock items to CSV
  Future<String> exportToCsv({
    String? category,
    String? status,
  }) async {
    try {
      final queryParams = <String, dynamic>{};
      if (category != null) queryParams['category'] = category;
      if (status != null) queryParams['status'] = status;

      final response = await _dio.get('/stock/export', queryParameters: queryParams);
      return response.data['downloadUrl'] ?? response.data['url'] ?? '';
    } catch (e) {
      throw _handleApiError(e);
    }
  }

  // Error handling
  Exception _handleApiError(dynamic error) {
    if (error is DioException) {
      switch (error.type) {
        case DioExceptionType.connectionTimeout:
        case DioExceptionType.sendTimeout:
        case DioExceptionType.receiveTimeout:
          return Exception('Connection timeout. Please check your internet connection.');
        
        case DioExceptionType.badResponse:
          final statusCode = error.response?.statusCode;
          final message = error.response?.data?['message'] ?? 
                         error.response?.data?['error'] ?? 
                         'Unknown server error';
          
          switch (statusCode) {
            case 400:
              return Exception('Bad request: $message');
            case 401:
              return Exception('Unauthorized. Please log in again.');
            case 403:
              return Exception('Access forbidden.');
            case 404:
              return Exception('Resource not found.');
            case 422:
              return Exception('Validation error: $message');
            case 500:
              return Exception('Server error. Please try again later.');
            default:
              return Exception('HTTP $statusCode: $message');
          }
        
        case DioExceptionType.cancel:
          return Exception('Request was cancelled.');
        
        case DioExceptionType.unknown:
          return Exception('Network error. Please check your connection.');
        
        default:
          return Exception('Unknown error occurred.');
      }
    }
    
    return Exception('Unexpected error: ${error.toString()}');
  }
}

