import 'package:flutter_test/flutter_test.dart';

// Import all test files
import 'unit/product_repository_test.dart' as product_repository_tests;
import 'unit/products_provider_test.dart' as products_provider_tests;
import 'unit/auth_service_test.dart' as auth_service_tests;
import 'data/database_test.dart' as database_tests;
import 'widget/cart_panel_test.dart' as cart_panel_tests;
import 'integration/pos_workflow_test.dart' as pos_workflow_tests;

/// Comprehensive test runner for the POS system
/// 
/// This test runner executes all tests in the correct order:
/// 1. Unit tests (individual components)
/// 2. Data layer tests (database operations)
/// 3. Widget tests (UI components)
/// 4. Integration tests (end-to-end workflows)
void main() {
  group('🧪 POS System Test Suite', () {
    group('📦 Unit Tests', () {
      group('Repository Layer', () {
        product_repository_tests.main();
      });
      
      group('Provider Layer', () {
        products_provider_tests.main();
      });
      
      group('Authentication Service', () {
        auth_service_tests.main();
      });
    });

    group('💾 Data Layer Tests', () {
      group('Database Operations', () {
        database_tests.main();
      });
    });

    group('🎨 Widget Tests', () {
      group('Cart Panel UI', () {
        cart_panel_tests.main();
      });
    });

    group('🔄 Integration Tests', () {
      group('POS Workflow', () {
        pos_workflow_tests.main();
      });
    });
  });
}

/// Test configuration and utilities
class TestConfig {
  static const bool runIntegrationTests = true;
  static const bool runWidgetTests = true;
  static const bool runUnitTests = true;
  static const bool generateCoverageReport = true;
  
  /// Test timeouts for different test types
  static const Duration unitTestTimeout = Duration(seconds: 30);
  static const Duration widgetTestTimeout = Duration(seconds: 60);
  static const Duration integrationTestTimeout = Duration(minutes: 5);
  
  /// Mock data configuration
  static const String testUserId = 'test_user_123';
  static const String testTenantId = 'test_tenant_456';
  
  /// Database configuration for tests
  static const String testDatabaseName = 'test_pos_store.db';
  static const bool useInMemoryDatabase = true;
  
  /// Print test summary
  static void printTestSummary() {
    print('\n' + '=' * 60);
    print('🧪 POS SYSTEM TEST SUITE CONFIGURATION');
    print('=' * 60);
    print('📊 Unit Tests: ${runUnitTests ? "✅ Enabled" : "❌ Disabled"}');
    print('🎨 Widget Tests: ${runWidgetTests ? "✅ Enabled" : "❌ Disabled"}');
    print('🔄 Integration Tests: ${runIntegrationTests ? "✅ Enabled" : "❌ Disabled"}');
    print('📈 Coverage Report: ${generateCoverageReport ? "✅ Enabled" : "❌ Disabled"}');
    print('⏱️  Timeouts: Unit(${unitTestTimeout.inSeconds}s), Widget(${widgetTestTimeout.inSeconds}s), Integration(${integrationTestTimeout.inMinutes}m)');
    print('💾 Database: ${useInMemoryDatabase ? "In-Memory" : "File-based"}');
    print('=' * 60);
    print('');
  }
}

/// Test data factory for generating consistent test data
class TestDataFactory {
  /// Generate sample product for testing
  static Map<String, dynamic> createSampleProduct({
    int id = 1,
    String? name,
    String? sku,
    String? barcode,
    double? price,
    int? stock,
  }) {
    return {
      'id': id,
      'createdById': TestConfig.testUserId,
      'name': name ?? 'Test Product $id',
      'sku': sku ?? 'TEST-${id.toString().padLeft(3, '0')}',
      'barcode': barcode ?? '${1000000000000 + id}',
      'price': ((price ?? 19.99) * 100).toInt(), // Convert to cents
      'stock': stock ?? 10,
      'costPrice': ((price ?? 19.99) * 0.7 * 100).toInt(),
      'isActive': true,
      'isFavorite': false,
      'categoryId': 'test_category',
      'category': 'Test Category',
      'lowStockThreshold': 5,
      'reorderPoint': 10,
      'reorderQuantity': 20,
      'enableLowStockAlert': true,
      'enableExpiryAlert': false,
      'createdAt': DateTime.now().toIso8601String(),
      'updatedAt': DateTime.now().toIso8601String(),
    };
  }

  /// Generate sample products for bulk testing
  static List<Map<String, dynamic>> createSampleProducts(int count) {
    return List.generate(count, (index) => createSampleProduct(
      id: index + 1,
      name: 'Product ${index + 1}',
      price: (index + 1) * 5.99,
      stock: (index + 1) * 2,
    ));
  }

  /// Generate low stock product
  static Map<String, dynamic> createLowStockProduct({int id = 999}) {
    return createSampleProduct(
      id: id,
      name: 'Low Stock Product',
      stock: 2, // Below threshold of 5
    );
  }

  /// Generate out of stock product
  static Map<String, dynamic> createOutOfStockProduct({int id = 998}) {
    return createSampleProduct(
      id: id,
      name: 'Out of Stock Product',
      stock: 0,
    );
  }
}

/// Test utilities and helper functions
class TestUtils {
  /// Wait for async operations to complete
  static Future<void> waitForAsync([Duration? duration]) async {
    await Future.delayed(duration ?? const Duration(milliseconds: 100));
  }

  /// Print test section header
  static void printSection(String title) {
    print('\n' + '-' * 40);
    print('🔍 $title');
    print('-' * 40);
  }

  /// Print test result summary
  static void printTestResult(String testName, bool passed, [String? details]) {
    final icon = passed ? '✅' : '❌';
    print('$icon $testName${details != null ? " - $details" : ""}');
  }

  /// Generate random barcode for testing
  static String generateTestBarcode() {
    final timestamp = DateTime.now().millisecondsSinceEpoch;
    return timestamp.toString().padLeft(13, '0').substring(0, 13);
  }

  /// Generate test user ID
  static String generateTestUserId() {
    return 'test_user_${DateTime.now().millisecondsSinceEpoch}';
  }
}

/// Performance monitoring for tests
class TestPerformanceMonitor {
  static final Map<String, DateTime> _startTimes = {};
  static final Map<String, Duration> _durations = {};

  /// Start timing a test operation
  static void startTiming(String operation) {
    _startTimes[operation] = DateTime.now();
  }

  /// End timing and record duration
  static Duration endTiming(String operation) {
    final startTime = _startTimes[operation];
    if (startTime == null) {
      throw StateError('No start time recorded for operation: $operation');
    }
    
    final duration = DateTime.now().difference(startTime);
    _durations[operation] = duration;
    _startTimes.remove(operation);
    
    return duration;
  }

  /// Get performance summary
  static String getPerformanceSummary() {
    if (_durations.isEmpty) {
      return 'No performance data recorded';
    }

    final buffer = StringBuffer();
    buffer.writeln('\n📊 Performance Summary:');
    buffer.writeln('=' * 50);
    
    _durations.entries.forEach((entry) {
      final operation = entry.key;
      final duration = entry.value;
      final ms = duration.inMilliseconds;
      
      String status;
      if (ms < 100) {
        status = '🟢 Fast';
      } else if (ms < 500) {
        status = '🟡 Moderate';
      } else {
        status = '🔴 Slow';
      }
      
      buffer.writeln('$status $operation: ${ms}ms');
    });
    
    buffer.writeln('=' * 50);
    return buffer.toString();
  }

  /// Clear performance data
  static void clear() {
    _startTimes.clear();
    _durations.clear();
  }
}

/// Test coverage tracking
class TestCoverageTracker {
  static final Set<String> _coveredFeatures = {};
  static final Set<String> _allFeatures = {
    'product_repository_crud',
    'product_repository_barcode_search',
    'products_provider_bulk_operations',
    'products_provider_low_stock_detection',
    'auth_service_sign_in',
    'auth_service_sign_out',
    'database_operations',
    'database_migrations',
    'cart_panel_ui',
    'cart_item_interactions',
    'pos_workflow_barcode_scanning',
    'pos_workflow_bulk_operations',
    'error_handling',
    'performance_testing',
  };

  /// Mark a feature as covered by tests
  static void markCovered(String feature) {
    _coveredFeatures.add(feature);
  }

  /// Get coverage percentage
  static double getCoveragePercentage() {
    if (_allFeatures.isEmpty) return 0.0;
    return (_coveredFeatures.length / _allFeatures.length) * 100;
  }

  /// Get coverage report
  static String getCoverageReport() {
    final buffer = StringBuffer();
    final percentage = getCoveragePercentage();
    
    buffer.writeln('\n📈 Test Coverage Report:');
    buffer.writeln('=' * 50);
    buffer.writeln('Overall Coverage: ${percentage.toStringAsFixed(1)}%');
    buffer.writeln('');
    
    buffer.writeln('Covered Features (${_coveredFeatures.length}):');
    for (final feature in _coveredFeatures) {
      buffer.writeln('  ✅ $feature');
    }
    
    final uncovered = _allFeatures.difference(_coveredFeatures);
    if (uncovered.isNotEmpty) {
      buffer.writeln('');
      buffer.writeln('Uncovered Features (${uncovered.length}):');
      for (final feature in uncovered) {
        buffer.writeln('  ❌ $feature');
      }
    }
    
    buffer.writeln('=' * 50);
    return buffer.toString();
  }

  /// Clear coverage data
  static void clear() {
    _coveredFeatures.clear();
  }
}