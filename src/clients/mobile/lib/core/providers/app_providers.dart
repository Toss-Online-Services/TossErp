import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:pos_store/core/database/database_service.dart';
import 'package:pos_store/core/network/api_service.dart';
import 'package:pos_store/shared/services/auth_service.dart';
import 'package:pos_store/shared/services/sync_service.dart';

class AppProviders {
  static Future<void> initialize() async {
    // Initialize core services
    await DatabaseService.initialize();
    await AuthService.initialize();
    await SyncService.initialize();
  }
}

// Database Service Provider
final databaseServiceProvider = Provider<DatabaseService>((ref) {
  return DatabaseService();
});

// API Service Provider
final apiServiceProvider = Provider<ApiService>((ref) {
  return ApiService();
});

// Auth Service Provider
final authServiceProvider = Provider<AuthService>((ref) {
  return AuthService();
});

// Sync Service Provider
final syncServiceProvider = Provider<SyncService>((ref) {
  return SyncService();
});
