import 'package:dio/dio.dart';
import 'dart:io';

class ApiService {
  late final Dio _dio;
  
  // Environment-based configuration
  static const String _defaultBaseUrl = 'http://localhost:8080/api';
  static const int _defaultConnectTimeout = 30000;
  static const int _defaultReceiveTimeout = 30000;
  
  ApiService() {
    // Get base URL from environment or use default
    final baseUrl = _getBaseUrl();
    final connectTimeout = _getConnectTimeout();
    final receiveTimeout = _getReceiveTimeout();
    
    _dio = Dio(BaseOptions(
      baseUrl: baseUrl,
      connectTimeout: Duration(milliseconds: connectTimeout),
      receiveTimeout: Duration(milliseconds: receiveTimeout),
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
        'User-Agent': 'TossErp-Mobile/1.0.0',
      },
    ));
    
    // Add logging interceptor for development
    if (_isDebugMode()) {
      _dio.interceptors.add(LogInterceptor(
        requestBody: true,
        responseBody: true,
        logPrint: (obj) => print('API: $obj'),
      ));
    }
    
    // Add request/response interceptors
    _dio.interceptors.add(InterceptorsWrapper(
      onRequest: (options, handler) {
        // Add common headers or authentication
        _addCommonHeaders(options);
        handler.next(options);
      },
      onResponse: (response, handler) {
        // Handle common response patterns
        handler.next(response);
      },
      onError: (error, handler) {
        // Handle common error patterns
        _handleCommonErrors(error);
        handler.next(error);
      },
    ));
  }
  
  Dio get dio => _dio;
  
  // Environment configuration methods
  String _getBaseUrl() {
    // In a real app, you'd use a config package like flutter_dotenv
    // For now, we'll use a const or you can implement environment loading
    return _defaultBaseUrl;
  }
  
  int _getConnectTimeout() {
    // Get from environment or use default
    return _defaultConnectTimeout;
  }
  
  int _getReceiveTimeout() {
    // Get from environment or use default
    return _defaultReceiveTimeout;
  }
  
  bool _isDebugMode() {
    // Check if we're in debug mode
    return true; // You can implement proper debug detection
  }
  
  void _addCommonHeaders(RequestOptions options) {
    // Add any common headers like API version, client info, etc.
    options.headers['X-Client-Version'] = '1.0.0';
    options.headers['X-Client-Type'] = 'mobile';
  }
  
  void _handleCommonErrors(DioException error) {
    // Handle common error patterns like 401, 403, 500, etc.
    switch (error.response?.statusCode) {
      case 401:
        // Handle unauthorized - maybe clear auth token
        break;
      case 403:
        // Handle forbidden
        break;
      case 500:
        // Handle server error
        break;
    }
  }
  
  // Generic GET request
  Future<Response> get(String path, {Map<String, dynamic>? queryParameters}) async {
    try {
      return await _dio.get(path, queryParameters: queryParameters);
    } catch (e) {
      rethrow;
    }
  }
  
  // Generic POST request
  Future<Response> post(String path, {dynamic data}) async {
    try {
      return await _dio.post(path, data: data);
    } catch (e) {
      rethrow;
    }
  }
  
  // Generic PUT request
  Future<Response> put(String path, {dynamic data}) async {
    try {
      return await _dio.put(path, data: data);
    } catch (e) {
      rethrow;
    }
  }
  
  // Generic DELETE request
  Future<Response> delete(String path) async {
    try {
      return await _dio.delete(path);
    } catch (e) {
      rethrow;
    }
  }
  
  // Generic PATCH request
  Future<Response> patch(String path, {dynamic data}) async {
    try {
      return await _dio.patch(path, data: data);
    } catch (e) {
      rethrow;
    }
  }
  
  // Set authentication token
  void setAuthToken(String token) {
    _dio.options.headers['Authorization'] = 'Bearer $token';
  }
  
  // Clear authentication token
  void clearAuthToken() {
    _dio.options.headers.remove('Authorization');
  }
  
  // Health check method
  Future<bool> healthCheck() async {
    try {
      final response = await _dio.get('/health');
      return response.statusCode == 200;
    } catch (e) {
      return false;
    }
  }
  
  // Get current base URL
  String get currentBaseUrl => _dio.options.baseUrl;
}
