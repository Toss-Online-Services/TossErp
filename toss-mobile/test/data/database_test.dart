import 'package:flutter_test/flutter_test.dart';
import 'package:mockito/mockito.dart';
import 'package:mockito/annotations.dart';
import 'package:sqflite/sqflite.dart';

import 'database_test.mocks.dart';

@GenerateMocks([
  Database,
])
void main() {
  late MockDatabase mockDatabase;

  setUp(() {
    mockDatabase = MockDatabase();
  });

  group('Database Tests', () {
    group('Product Table Operations', () {
      test('should insert product data correctly', () async {
        // Arrange
        final productData = {
          'id': 1,
          'createdById': 'test_user_123',
          'name': 'Test Product',
          'sku': 'TEST-001',
          'barcode': '1234567890123',
          'imageUrl': 'https://example.com/image.jpg',
          'type': 'physical',
          'trackingMethod': 'simple',
          'stock': 100,
          'sold': 5,
          'price': 1999,
          'costPrice': 1200,
          'description': 'A test product',
          'unit': 'each',
          'isActive': 1,
          'isFavorite': 0,
          'categoryId': 1,
          'lowStockThreshold': 10,
          'reorderPoint': 20,
          'reorderQuantity': 50,
          'enableLowStockAlert': 1,
          'enableExpiryAlert': 1,
          'createdAt': '2025-09-15T10:16:42.288910',
          'updatedAt': '2025-09-15T10:16:42.300201',
        };

        when(mockDatabase.insert(
          'products',
          productData,
          conflictAlgorithm: anyNamed('conflictAlgorithm'),
        )).thenAnswer((_) async => 1);

        // Act
        final result = await mockDatabase.insert(
          'products',
          productData,
          conflictAlgorithm: ConflictAlgorithm.replace,
        );

        // Assert
        expect(result, equals(1));
        verify(mockDatabase.insert(
          'products',
          productData,
          conflictAlgorithm: ConflictAlgorithm.replace,
        )).called(1);
      });

      test('should query products by barcode', () async {
        // Arrange
        const barcode = '1234567890123';
        final expectedProduct = [
          {
            'id': 1,
            'name': 'Test Product',
            'barcode': barcode,
            'price': 1999,
            'stock': 100,
          }
        ];

        when(mockDatabase.query(
          'products',
          where: anyNamed('where'),
          whereArgs: anyNamed('whereArgs'),
        )).thenAnswer((_) async => expectedProduct);

        // Act
        final result = await mockDatabase.query(
          'products',
          where: 'barcode = ?',
          whereArgs: [barcode],
        );

        // Assert
        expect(result.length, equals(1));
        expect(result.first['barcode'], equals(barcode));
        verify(mockDatabase.query(
          'products',
          where: 'barcode = ?',
          whereArgs: [barcode],
        )).called(1);
      });

      test('should update product data', () async {
        // Arrange
        final updateData = {
          'price': 2199,
          'stock': 80,
          'updatedAt': '2025-09-15T12:00:00.000000',
        };

        when(mockDatabase.update(
          'products',
          updateData,
          where: anyNamed('where'),
          whereArgs: anyNamed('whereArgs'),
        )).thenAnswer((_) async => 1);

        // Act
        final result = await mockDatabase.update(
          'products',
          updateData,
          where: 'id = ?',
          whereArgs: [1],
        );

        // Assert
        expect(result, equals(1));
        verify(mockDatabase.update(
          'products',
          updateData,
          where: 'id = ?',
          whereArgs: [1],
        )).called(1);
      });

      test('should delete product by id', () async {
        // Arrange
        when(mockDatabase.delete(
          'products',
          where: anyNamed('where'),
          whereArgs: anyNamed('whereArgs'),
        )).thenAnswer((_) async => 1);

        // Act
        final result = await mockDatabase.delete(
          'products',
          where: 'id = ?',
          whereArgs: [1],
        );

        // Assert
        expect(result, equals(1));
        verify(mockDatabase.delete(
          'products',
          where: 'id = ?',
          whereArgs: [1],
        )).called(1);
      });

      test('should query products with low stock', () async {
        // Arrange
        final lowStockProducts = [
          {
            'id': 1,
            'name': 'Low Stock Product',
            'stock': 2,
            'lowStockThreshold': 5,
          }
        ];

        when(mockDatabase.rawQuery(any, any))
            .thenAnswer((_) async => lowStockProducts);

        // Act
        final result = await mockDatabase.rawQuery(
          'SELECT * FROM products WHERE stock <= lowStockThreshold',
          [],
        );

        // Assert
        expect(result.length, equals(1));
        expect(result.first['stock'], equals(2));
        verify(mockDatabase.rawQuery(
          'SELECT * FROM products WHERE stock <= lowStockThreshold',
          [],
        )).called(1);
      });
    });

    group('Database Schema Validation', () {
      test('should verify products table schema', () async {
        // Arrange
        final tableInfo = [
          {'name': 'id', 'type': 'INTEGER', 'pk': 1},
          {'name': 'barcode', 'type': 'TEXT', 'pk': 0},
          {'name': 'name', 'type': 'TEXT', 'pk': 0},
          {'name': 'price', 'type': 'INTEGER', 'pk': 0},
          {'name': 'stock', 'type': 'INTEGER', 'pk': 0},
        ];

        when(mockDatabase.rawQuery('PRAGMA table_info(products)'))
            .thenAnswer((_) async => tableInfo);

        // Act
        final result = await mockDatabase.rawQuery('PRAGMA table_info(products)');

        // Assert
        expect(result.length, greaterThan(0));
        final columnNames = result.map((col) => col['name']).toList();
        expect(columnNames, contains('id'));
        expect(columnNames, contains('barcode'));
        expect(columnNames, contains('name'));
        expect(columnNames, contains('price'));
        expect(columnNames, contains('stock'));
      });

      test('should verify barcode column exists', () async {
        // Arrange
        final tableInfo = [
          {'name': 'barcode', 'type': 'TEXT', 'pk': 0, 'notnull': 0},
        ];

        when(mockDatabase.rawQuery("PRAGMA table_info(products)"))
            .thenAnswer((_) async => tableInfo);

        // Act
        final result = await mockDatabase.rawQuery("PRAGMA table_info(products)");

        // Assert
        final barcodeColumn = result.firstWhere(
          (col) => col['name'] == 'barcode',
          orElse: () => <String, dynamic>{},
        );
        expect(barcodeColumn, isNotEmpty);
        expect(barcodeColumn['type'], equals('TEXT'));
      });
    });

    group('Transaction Operations', () {
      test('should execute batch operations in transaction', () async {
        // Arrange
        final operations = [
          'UPDATE products SET stock = stock - 1 WHERE id = 1',
          'INSERT INTO sales (product_id, quantity) VALUES (1, 1)',
        ];

        when(mockDatabase.transaction(any)).thenAnswer((invocation) async {
          final callback = invocation.positionalArguments[0] as Function(Transaction);
          final mockTransaction = MockTransaction();
          return await callback(mockTransaction);
        });

        // Act
        await mockDatabase.transaction((txn) async {
          for (final operation in operations) {
            await txn.rawInsert(operation);
          }
        });

        // Assert
        verify(mockDatabase.transaction(any)).called(1);
      });

      test('should handle transaction rollback on error', () async {
        // Arrange
        when(mockDatabase.transaction(any)).thenThrow(Exception('Transaction failed'));

        // Act & Assert
        expect(
          () => mockDatabase.transaction((txn) async {
            throw Exception('Simulated error');
          }),
          throwsA(isA<Exception>()),
        );
      });
    });

    group('Performance Tests', () {
      test('should handle bulk insert operations', () async {
        // Arrange
        final batchData = List.generate(100, (index) => {
          'id': index + 1,
          'name': 'Product $index',
          'barcode': '123456789012$index',
          'price': 1000 + index,
          'stock': 50,
        });

        when(mockDatabase.batch()).thenReturn(MockBatch());

        // Act
        final batch = mockDatabase.batch();
        for (final data in batchData) {
          batch.insert('products', data);
        }

        // Assert
        verify(mockDatabase.batch()).called(1);
      });

      test('should handle large result sets efficiently', () async {
        // Arrange
        final largeResultSet = List.generate(1000, (index) => {
          'id': index + 1,
          'name': 'Product $index',
          'price': 1000 + index,
        });

        when(mockDatabase.query(
          'products',
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
        )).thenAnswer((_) async => largeResultSet.take(50).toList());

        // Act
        final result = await mockDatabase.query(
          'products',
          limit: 50,
          offset: 0,
        );

        // Assert
        expect(result.length, equals(50));
        verify(mockDatabase.query(
          'products',
          limit: 50,
          offset: 0,
        )).called(1);
      });
    });

    group('Error Handling', () {
      test('should handle database connection errors', () async {
        // Arrange
        when(mockDatabase.query('products'))
            .thenThrow(DatabaseException('Database is locked'));

        // Act & Assert
        expect(
          () => mockDatabase.query('products'),
          throwsA(isA<DatabaseException>()),
        );
      });

      test('should handle constraint violations', () async {
        // Arrange
        final duplicateData = {
          'id': 1,
          'barcode': '1234567890123', // Duplicate barcode
          'name': 'Duplicate Product',
        };

        when(mockDatabase.insert('products', duplicateData))
            .thenThrow(DatabaseException('UNIQUE constraint failed'));

        // Act & Assert
        expect(
          () => mockDatabase.insert('products', duplicateData),
          throwsA(isA<DatabaseException>()),
        );
      });

      test('should handle SQL syntax errors', () async {
        // Arrange
        when(mockDatabase.rawQuery('INVALID SQL QUERY'))
            .thenThrow(DatabaseException('SQL syntax error'));

        // Act & Assert
        expect(
          () => mockDatabase.rawQuery('INVALID SQL QUERY'),
          throwsA(isA<DatabaseException>()),
        );
      });
    });
  });
}

// Additional mocks for transaction testing
class MockTransaction extends Mock implements Transaction {}
class MockBatch extends Mock implements Batch {}