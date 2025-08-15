import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class AuthService {
  static const FlutterSecureStorage _storage = FlutterSecureStorage();
  
  static Future<void> initialize() async {
    // Initialize secure storage
    await _storage.write(key: 'initialized', value: 'true');
  }
  
  // Store authentication token
  Future<void> storeToken(String token) async {
    await _storage.write(key: 'auth_token', value: token);
  }
  
  // Get authentication token
  Future<String?> getToken() async {
    return await _storage.read(key: 'auth_token');
  }
  
  // Remove authentication token
  Future<void> removeToken() async {
    await _storage.delete(key: 'auth_token');
  }
  
  // Check if user is authenticated
  Future<bool> isAuthenticated() async {
    final token = await getToken();
    return token != null && token.isNotEmpty;
  }
  
  // Store user data
  Future<void> storeUserData(Map<String, String> userData) async {
    for (final entry in userData.entries) {
      await _storage.write(key: 'user_${entry.key}', value: entry.value);
    }
  }
  
  // Get user data
  Future<Map<String, String?>> getUserData() async {
    final keys = ['id', 'username', 'email', 'role', 'name'];
    final Map<String, String?> userData = {};
    
    for (final key in keys) {
      userData[key] = await _storage.read(key: 'user_$key');
    }
    
    return userData;
  }
  
  // Clear all user data
  Future<void> clearUserData() async {
    final keys = ['auth_token', 'user_id', 'user_username', 'user_email', 'user_role', 'user_name'];
    for (final key in keys) {
      await _storage.delete(key: key);
    }
  }
  
  // Logout
  Future<void> logout() async {
    await clearUserData();
  }
}
