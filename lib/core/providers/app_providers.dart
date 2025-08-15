import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:pos_store/core/database/database_service.dart';
import 'package:pos_store/core/network/api_service.dart';
import 'package:pos_store/shared/services/auth_service.dart';
import 'package:pos_store/shared/services/sync_service.dart';

class AppProviders {
  static Future<void> initialize() async {
    // Initialize database
    await DatabaseService.initialize();
    
    // Initialize other services
    await AuthService.initialize();
    await SyncService.initialize();
  }
}

// Database provider
final databaseProvider = Provider<DatabaseService>((ref) {
  return DatabaseService.instance;
});

// API service provider
final apiServiceProvider = Provider<ApiService>((ref) {
  return ApiService();
});

// Auth service provider
final authServiceProvider = Provider<AuthService>((ref) {
  return AuthService();
});

// Sync service provider
final syncServiceProvider = Provider<SyncService>((ref) {
  return SyncService();
});
