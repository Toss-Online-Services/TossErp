import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mockito/mockito.dart';
import 'package:mockito/annotations.dart';
import '../../lib/presentation/providers/products/products_provider.dart';
import '../../lib/domain/repositories/product_repository.dart';
import '../../lib/domain/entities/product_entity.dart';
import '../../lib/domain/entities/product_category_entity.dart';
import '../../lib/core/usecase/usecase.dart';
import '../../lib/core/errors/errors.dart';
import '../../lib/app/services/auth/auth_service.dart';

import 'products_provider_test.mocks.dart';

@GenerateMocks([
  ProductRepository,
  AuthService,
])
void main() {
  late ProductsProvider provider;
  late MockProductRepository mockProductRepository;
  late MockAuthService mockAuthService;

  setUp(() {
    mockProductRepository = MockProductRepository();
    mockAuthService = MockAuthService();
    provider = ProductsProvider(productRepository: mockProductRepository);
  });

  // Sample test data
  final sampleProducts = [
    ProductEntity(
      id: 1,
      createdById: 'test_user_123',
      name: 'Albany Brown Bread 700g',
      sku: 'ALB-001',
      barcode: '6001234567890',
      imageUrl: 'https://images.unsplash.com/photo-1542834369-f10eb...',
      type: ProductType.physical,
      trackingMethod: TrackingMethod.simple,
      stock: 40,
      sold: 0,
      price: 1999, // R19.99
      costPrice: 1200,
      description: 'Daily fresh bread loaf',
      unit: 'each',
      isActive: true,
      isFavorite: false,
      categoryId: 1,
      category: const ProductCategoryEntity(
        id: 1,
        name: 'Bakery',
        description: 'Bakery products',
        isActive: true,
        createdById: 'test_user_123',
      ),
      variants: [],
      batches: [],
      lowStockThreshold: 10,
      reorderPoint: 20,
      reorderQuantity: 50,
      enableLowStockAlert: true,
      enableExpiryAlert: true,
      customAttributes: {'brand': 'Albany'},
      createdAt: DateTime.now().toIso8601String(),
      updatedAt: DateTime.now().toIso8601String(),
    ),
    ProductEntity(
      id: 2,
      createdById: 'test_user_123',
      name: 'Coca-Cola 2L',
      sku: 'CC-002',
      barcode: '6001234567891',
      imageUrl: 'https://images.unsplash.com/photo-1542834369-f10eb...',
      type: ProductType.physical,
      trackingMethod: TrackingMethod.simple,
      stock: 2, // Low stock
      sold: 0,
      price: 2499, // R24.99
      costPrice: 1800,
      description: 'Refreshing cola drink',
      unit: 'each',
      isActive: true,
      isFavorite: false,
      categoryId: 2,
      category: const ProductCategoryEntity(
        id: 2,
        name: 'Beverages',
        description: 'Drink products',
        isActive: true,
        createdById: 'test_user_123',
      ),
      variants: [],
      batches: [],
      lowStockThreshold: 5,
      reorderPoint: 15,
      reorderQuantity: 30,
      enableLowStockAlert: true,
      enableExpiryAlert: true,
      customAttributes: {'brand': 'Coca-Cola'},
      createdAt: DateTime.now().toIso8601String(),
      updatedAt: DateTime.now().toIso8601String(),
    ),
  ];

  group('ProductsProvider Tests', () {
    group('getAllProducts', () {
      test('should load products successfully', () async {
        // Arrange
        when(mockProductRepository.getUserProducts(
          any,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => Result.success(sampleProducts));

        // Act
        await provider.getAllProducts();

        // Assert
        expect(provider.allProducts, isNotNull);
        expect(provider.allProducts!.length, equals(2));
        expect(provider.allProducts!.first.name, equals('Albany Brown Bread 700g'));
        expect(provider.lowStockProducts.length, equals(1));
        expect(provider.lowStockProducts.first.name, equals('Coca-Cola 2L'));
        expect(provider.isLoadingMore, isFalse);
      });

      test('should handle load more functionality', () async {
        // Arrange
        provider.allProducts = sampleProducts.take(1).toList();
        final moreProducts = sampleProducts.skip(1).toList();
        
        when(mockProductRepository.getUserProducts(
          any,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => Result.success(moreProducts));

        // Act
        await provider.getAllProducts(offset: 1);

        // Assert
        expect(provider.allProducts!.length, equals(2));
        expect(provider.isLoadingMore, isFalse);
      });

      test('should handle search functionality', () async {
        // Arrange
        const searchTerm = 'bread';
        final filteredProducts = sampleProducts.where((p) => 
          p.name.toLowerCase().contains(searchTerm)).toList();
        
        when(mockProductRepository.getUserProducts(
          any,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => Result.success(filteredProducts));

        // Act
        await provider.getAllProducts(contains: searchTerm);

        // Assert
        expect(provider.allProducts!.length, equals(1));
        expect(provider.allProducts!.first.name.toLowerCase().contains(searchTerm), isTrue);
      });

      test('should handle errors gracefully', () async {
        // Arrange
        when(mockProductRepository.getUserProducts(
          any,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => Result.error(ServiceError(message: 'Network error')));

        // Act & Assert
        expect(() => provider.getAllProducts(), throwsA(isA<String>()));
      });

      test('should set loading state correctly during load more', () async {
        // Arrange
        provider.allProducts = sampleProducts.take(1).toList();
        bool loadingStateWasTrue = false;
        
        provider.addListener(() {
          if (provider.isLoadingMore) {
            loadingStateWasTrue = true;
          }
        });

        when(mockProductRepository.getUserProducts(
          any,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async {
          await Future.delayed(const Duration(milliseconds: 100));
          return Result.success(sampleProducts.skip(1).toList());
        });

        // Act
        await provider.getAllProducts(offset: 1);

        // Assert
        expect(loadingStateWasTrue, isTrue);
        expect(provider.isLoadingMore, isFalse);
      });
    });

    group('bulkUpdatePricesByIds', () {
      test('should update prices for selected products', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = {1, 2};
        const newPrice = 2999.0;

        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(()));

        // Act
        await provider.bulkUpdatePricesByIds(productIds, newPrice);

        // Assert
        expect(provider.allProducts![0].price, equals(newPrice.toInt()));
        expect(provider.allProducts![1].price, equals(newPrice.toInt()));
        verify(mockProductRepository.updateProduct(any)).called(2);
      });

      test('should update only selected products', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = {1}; // Only first product
        const newPrice = 2999.0;
        final originalSecondPrice = sampleProducts[1].price;

        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(()));

        // Act
        await provider.bulkUpdatePricesByIds(productIds, newPrice);

        // Assert
        expect(provider.allProducts![0].price, equals(newPrice.toInt()));
        expect(provider.allProducts![1].price, equals(originalSecondPrice));
        verify(mockProductRepository.updateProduct(any)).called(1);
      });

      test('should handle empty product selection', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = <int>{};
        const newPrice = 2999.0;

        // Act
        await provider.bulkUpdatePricesByIds(productIds, newPrice);

        // Assert
        verifyNever(mockProductRepository.updateProduct(any));
      });

      test('should handle update errors', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = {1};
        const newPrice = 2999.0;

        when(mockProductRepository.updateProduct(any))
            .thenThrow(Exception('Update failed'));

        // Act & Assert
        expect(() => provider.bulkUpdatePricesByIds(productIds, newPrice), 
               throwsA(isA<Exception>()));
      });

      test('should update updatedAt timestamp', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = {1};
        const newPrice = 2999.0;
        final originalTimestamp = provider.allProducts![0].updatedAt;

        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(()));

        // Wait to ensure timestamp difference
        await Future.delayed(const Duration(milliseconds: 10));

        // Act
        await provider.bulkUpdatePricesByIds(productIds, newPrice);

        // Assert
        expect(provider.allProducts![0].updatedAt, isNot(equals(originalTimestamp)));
      });
    });

    group('bulkUpdateStockByIds', () {
      test('should update stock for selected products', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = {1, 2};
        const newStock = 100;

        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(()));

        // Act
        await provider.bulkUpdateStockByIds(productIds, newStock);

        // Assert
        expect(provider.allProducts![0].stock, equals(newStock));
        expect(provider.allProducts![1].stock, equals(newStock));
        verify(mockProductRepository.updateProduct(any)).called(2);
      });

      test('should recompute low stock after update', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = {2}; // Update the low stock product
        const newStock = 100; // Above threshold

        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(()));

        // Initial state: should have 1 low stock product
        await provider.getAllProducts();
        expect(provider.lowStockProducts.length, equals(1));

        // Act
        await provider.bulkUpdateStockByIds(productIds, newStock);

        // Assert
        expect(provider.lowStockProducts.length, equals(0));
      });

      test('should handle stock reduction creating low stock', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = {1}; // Update the normal stock product
        const newStock = 2; // Below threshold

        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(()));

        // Act
        await provider.bulkUpdateStockByIds(productIds, newStock);

        // Assert
        expect(provider.lowStockProducts.length, equals(2)); // Both products now low stock
      });

      test('should update only selected products', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = {1}; // Only first product
        const newStock = 150;
        final originalSecondStock = sampleProducts[1].stock;

        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(()));

        // Act
        await provider.bulkUpdateStockByIds(productIds, newStock);

        // Assert
        expect(provider.allProducts![0].stock, equals(newStock));
        expect(provider.allProducts![1].stock, equals(originalSecondStock));
        verify(mockProductRepository.updateProduct(any)).called(1);
      });

      test('should handle empty product selection', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = <int>{};
        const newStock = 100;

        // Act
        await provider.bulkUpdateStockByIds(productIds, newStock);

        // Assert
        verifyNever(mockProductRepository.updateProduct(any));
      });

      test('should handle update errors', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = {1};
        const newStock = 100;

        when(mockProductRepository.updateProduct(any))
            .thenThrow(Exception('Update failed'));

        // Act & Assert
        expect(() => provider.bulkUpdateStockByIds(productIds, newStock), 
               throwsA(isA<Exception>()));
      });
    });

    group('bulkDeleteProductsByIds', () {
      test('should delete selected products', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = {1};

        when(mockProductRepository.deleteProduct(any))
            .thenAnswer((_) async => Result.success(()));

        // Act
        await provider.bulkDeleteProductsByIds(productIds);

        // Assert
        expect(provider.allProducts!.length, equals(1));
        expect(provider.allProducts!.first.id, equals(2));
        verify(mockProductRepository.deleteProduct(1)).called(1);
      });

      test('should delete multiple products', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = {1, 2};

        when(mockProductRepository.deleteProduct(any))
            .thenAnswer((_) async => Result.success(()));

        // Act
        await provider.bulkDeleteProductsByIds(productIds);

        // Assert
        expect(provider.allProducts!.length, equals(0));
        verify(mockProductRepository.deleteProduct(1)).called(1);
        verify(mockProductRepository.deleteProduct(2)).called(1);
      });

      test('should recompute low stock after deletion', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = {2}; // Delete the low stock product

        when(mockProductRepository.deleteProduct(any))
            .thenAnswer((_) async => Result.success(()));

        // Act
        await provider.bulkDeleteProductsByIds(productIds);

        // Assert
        expect(provider.lowStockProducts.length, equals(0));
      });

      test('should handle products with null IDs', () async {
        // Arrange
        final productsWithNullId = [
          sampleProducts[0].copyWith(id: null),
          sampleProducts[1],
        ];
        provider.allProducts = productsWithNullId;
        final productIds = {1, 2};

        when(mockProductRepository.deleteProduct(any))
            .thenAnswer((_) async => Result.success(()));

        // Act
        await provider.bulkDeleteProductsByIds(productIds);

        // Assert
        // Only the product with valid ID should be deleted
        verify(mockProductRepository.deleteProduct(2)).called(1);
        verifyNever(mockProductRepository.deleteProduct(1));
      });

      test('should handle empty product selection', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = <int>{};

        // Act
        await provider.bulkDeleteProductsByIds(productIds);

        // Assert
        expect(provider.allProducts!.length, equals(2));
        verifyNever(mockProductRepository.deleteProduct(any));
      });

      test('should handle deletion errors', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = {1};

        when(mockProductRepository.deleteProduct(any))
            .thenThrow(Exception('Deletion failed'));

        // Act & Assert
        expect(() => provider.bulkDeleteProductsByIds(productIds), 
               throwsA(isA<Exception>()));
      });
    });

    group('searchProductByBarcode', () {
      test('should find product by barcode successfully', () async {
        // Arrange
        const barcode = '6001234567890';
        final expectedProduct = sampleProducts.first;

        when(mockProductRepository.getProductByBarcode(barcode))
            .thenAnswer((_) async => Result.success(expectedProduct));

        // Act
        final result = await provider.searchProductByBarcode(barcode);

        // Assert
        expect(result, isNotNull);
        expect(result!.barcode, equals(barcode));
        expect(result.name, equals('Albany Brown Bread 700g'));
        verify(mockProductRepository.getProductByBarcode(barcode)).called(1);
      });

      test('should return null when product not found', () async {
        // Arrange
        const barcode = 'nonexistent';

        when(mockProductRepository.getProductByBarcode(barcode))
            .thenAnswer((_) async => Result.success(()));

        // Act
        final result = await provider.searchProductByBarcode(barcode);

        // Assert
        expect(result, isNull);
        verify(mockProductRepository.getProductByBarcode(barcode)).called(1);
      });

      test('should handle repository errors', () async {
        // Arrange
        const barcode = '6001234567890';

        when(mockProductRepository.getProductByBarcode(barcode))
            .thenAnswer((_) async => Result.error(ServiceError(message: 'Search failed')));

        // Act
        final result = await provider.searchProductByBarcode(barcode);

        // Assert
        expect(result, isNull);
        verify(mockProductRepository.getProductByBarcode(barcode)).called(1);
      });

      test('should handle search exceptions', () async {
        // Arrange
        const barcode = '6001234567890';

        when(mockProductRepository.getProductByBarcode(barcode))
            .thenThrow(Exception('Network error'));

        // Act
        final result = await provider.searchProductByBarcode(barcode);

        // Assert
        expect(result, isNull);
        verify(mockProductRepository.getProductByBarcode(barcode)).called(1);
      });

      test('should handle various barcode formats', () async {
        // Arrange
        const barcodes = [
          '123456789012',
          '1234567890123',
          '12345678901234',
          'CUSTOM-BARCODE-001',
        ];
        
        for (final barcode in barcodes) {
          when(mockProductRepository.getProductByBarcode(barcode))
              .thenAnswer((_) async => Result.success(sampleProducts.first.copyWith(barcode: barcode)));
        }

        // Act & Assert
        for (final barcode in barcodes) {
          final result = await provider.searchProductByBarcode(barcode);
          expect(result, isNotNull);
          expect(result!.barcode, equals(barcode));
        }
      });
    });

    group('Low Stock Detection', () {
      test('should detect low stock products correctly', () async {
        // Arrange
        when(mockProductRepository.getUserProducts(
          any,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => Result.success(sampleProducts));

        // Act
        await provider.getAllProducts();

        // Assert
        expect(provider.lowStockProducts.length, equals(1));
        expect(provider.lowStockProducts.first.name, equals('Coca-Cola 2L'));
        expect(provider.lowStockProducts.first.stock, equals(2));
      });

      test('should handle custom low stock threshold', () async {
        // Arrange
        final customThresholdProducts = [
          sampleProducts[0].copyWith(stock: 3, lowStockThreshold: 5), // Low stock
          sampleProducts[1].copyWith(stock: 10, lowStockThreshold: 5), // Normal stock
        ];

        provider.allProducts = customThresholdProducts;

        // Act
        // Call _computeLowStock indirectly through getAllProducts
        when(mockProductRepository.getUserProducts(
          any,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => Result.success(customThresholdProducts));

        await provider.getAllProducts();

        // Assert
        expect(provider.lowStockProducts.length, equals(1));
        expect(provider.lowStockProducts.first.stock, equals(3));
      });

      test('should handle products with no low stock threshold', () async {
        // Arrange
        final noThresholdProducts = [
          sampleProducts[0].copyWith(lowStockThreshold: null, stock: 1),
          sampleProducts[1].copyWith(lowStockThreshold: null, stock: 100),
        ];

        when(mockProductRepository.getUserProducts(
          any,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => Result.success(noThresholdProducts));

        // Act
        await provider.getAllProducts();

        // Assert
        // Should use default threshold of 5 from provider
        expect(provider.lowStockProducts.length, equals(1));
        expect(provider.lowStockProducts.first.stock, equals(1));
      });
    });

    group('Provider State Management', () {
      test('should notify listeners on data changes', () async {
        // Arrange
        bool wasNotified = false;
        provider.addListener(() {
          wasNotified = true;
        });

        when(mockProductRepository.getUserProducts(
          any,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => Result.success(sampleProducts));

        // Act
        await provider.getAllProducts();

        // Assert
        expect(wasNotified, isTrue);
      });

      test('should maintain state between operations', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);
        final productIds = {1};
        const newPrice = 2999.0;

        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(()));

        // Act
        await provider.bulkUpdatePricesByIds(productIds, newPrice);

        // Assert
        expect(provider.allProducts, isNotNull);
        expect(provider.allProducts!.length, equals(2));
        expect(provider.allProducts![0].price, equals(newPrice.toInt()));
      });

      test('should handle concurrent operations', () async {
        // Arrange
        provider.allProducts = List.from(sampleProducts);

        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async {
              await Future.delayed(const Duration(milliseconds: 50));
              return Result.success(());
            });

        // Act
        final futures = [
          provider.bulkUpdatePricesByIds({1}, 2999.0),
          provider.bulkUpdateStockByIds({2}, 100),
        ];

        await Future.wait(futures);

        // Assert
        expect(provider.allProducts![0].price, equals(2999));
        expect(provider.allProducts![1].stock, equals(100));
      });
    });
  });
}
