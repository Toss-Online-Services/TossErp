import 'package:flutter_test/flutter_test.dart';
import 'package:mockito/mockito.dart';
import 'package:mockito/annotations.dart';
import '../../lib/domain/repositories/product_repository.dart';
import '../../lib/presentation/providers/products/products_provider.dart';
import '../../lib/app/services/auth/auth_service.dart';
import '../../lib/domain/entities/product_entity.dart';
import '../../lib/core/usecase/usecase.dart';

import '../unit/pos_workflow_test.mocks.dart';

@GenerateMocks([
  ProductRepository,
  AuthService,
])
void main() {
  late ProductsProvider productsProvider;
  late MockProductRepository mockProductRepository;
  late MockAuthService mockAuthService;

  setUp(() {
    mockProductRepository = MockProductRepository();
    mockAuthService = MockAuthService();
    productsProvider = ProductsProvider(productRepository: mockProductRepository);
  });

  group('POS Workflow Integration Tests', () {
    final sampleProduct = ProductEntity(
      id: 1,
      createdById: 'test_user_123',
      name: 'Test Product',
      sku: 'TEST-001',
      barcode: '1234567890123',
      imageUrl: 'https://example.com/image.jpg',
      type: ProductType.physical,
      trackingMethod: TrackingMethod.simple,
      stock: 100,
      sold: 5,
      price: 1999,
      costPrice: 1200,
      description: 'A test product',
      unit: 'each',
      isActive: true,
      isFavorite: false,
    );

    group('Product Discovery Workflow', () {
      test('should find product by barcode scanning', () async {
        // Arrange
        const barcode = '1234567890123';
        when(mockProductRepository.getProductByBarcode(barcode))
            .thenAnswer((_) async => Result.success(sampleProduct));

        // Act
        final result = await productsProvider.searchProductByBarcode(barcode);

        // Assert
        expect(result, isNotNull);
        expect(result!.barcode, equals(barcode));
        expect(result.name, equals('Test Product'));
        verify(mockProductRepository.getProductByBarcode(barcode)).called(1);
      });

      test('should handle product not found in barcode search', () async {
        // Arrange
        const barcode = 'nonexistent';
        when(mockProductRepository.getProductByBarcode(barcode))
            .thenAnswer((_) async => Result.success(null));

        // Act
        final result = await productsProvider.searchProductByBarcode(barcode);

        // Assert
        expect(result, isNull);
        verify(mockProductRepository.getProductByBarcode(barcode)).called(1);
      });

      test('should load product catalog for browsing', () async {
        // Arrange
        final products = [sampleProduct];
        when(mockProductRepository.getUserProducts(
          any,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => Result.success(products));

        // Act
        await productsProvider.getAllProducts();

        // Assert
        expect(productsProvider.allProducts, isNotNull);
        expect(productsProvider.allProducts!.length, equals(1));
        expect(productsProvider.allProducts!.first.name, equals('Test Product'));
      });
    });

    group('Bulk Operations Workflow', () {
      test('should perform bulk price update operation', () async {
        // Arrange
        productsProvider.allProducts = [sampleProduct];
        final productIds = {1};
        const newPrice = 2499.0;

        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(null));

        // Act
        await productsProvider.bulkUpdatePricesByIds(productIds, newPrice);

        // Assert
        expect(productsProvider.allProducts![0].price, equals(newPrice.toInt()));
        verify(mockProductRepository.updateProduct(any)).called(1);
      });

      test('should perform bulk stock update operation', () async {
        // Arrange
        productsProvider.allProducts = [sampleProduct];
        final productIds = {1};
        const newStock = 150;

        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(null));

        // Act
        await productsProvider.bulkUpdateStockByIds(productIds, newStock);

        // Assert
        expect(productsProvider.allProducts![0].stock, equals(newStock));
        verify(mockProductRepository.updateProduct(any)).called(1);
      });

      test('should perform bulk delete operation', () async {
        // Arrange
        productsProvider.allProducts = [sampleProduct];
        final productIds = {1};

        when(mockProductRepository.deleteProduct(any))
            .thenAnswer((_) async => Result.success(null));

        // Act
        await productsProvider.bulkDeleteProductsByIds(productIds);

        // Assert
        expect(productsProvider.allProducts!.length, equals(0));
        verify(mockProductRepository.deleteProduct(1)).called(1);
      });
    });

    group('Sale Transaction Workflow', () {
      test('should complete basic sale transaction', () async {
        // Arrange
        const barcode = '1234567890123';
        when(mockProductRepository.getProductByBarcode(barcode))
            .thenAnswer((_) async => Result.success(sampleProduct));
        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(null));

        // Act - Simulate complete sale workflow
        // 1. Scan/find product
        final foundProduct = await productsProvider.searchProductByBarcode(barcode);
        expect(foundProduct, isNotNull);

        // 2. Add to cart (simulated)
        var cartItems = <ProductEntity>[foundProduct!];
        expect(cartItems.length, equals(1));

        // 3. Process sale (update stock)
        productsProvider.allProducts = [sampleProduct];
        await productsProvider.bulkUpdateStockByIds({1}, sampleProduct.stock - 1);

        // Assert
        expect(productsProvider.allProducts![0].stock, equals(99));
        verify(mockProductRepository.getProductByBarcode(barcode)).called(1);
        verify(mockProductRepository.updateProduct(any)).called(1);
      });

      test('should handle multiple items in transaction', () async {
        // Arrange
        final products = [
          sampleProduct,
          sampleProduct.copyWith(id: 2, barcode: '1234567890124', name: 'Product 2'),
        ];
        
        for (final product in products) {
          when(mockProductRepository.getProductByBarcode(product.barcode!))
              .thenAnswer((_) async => Result.success(product));
        }
        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(null));

        // Act
        final foundProducts = <ProductEntity>[];
        for (final product in products) {
          final found = await productsProvider.searchProductByBarcode(product.barcode!);
          if (found != null) foundProducts.add(found);
        }

        // Assert
        expect(foundProducts.length, equals(2));
        expect(foundProducts[0].name, equals('Test Product'));
        expect(foundProducts[1].name, equals('Product 2'));
      });
    });

    group('Error Recovery Workflow', () {
      test('should handle repository errors gracefully', () async {
        // Arrange
        const barcode = '1234567890123';
        when(mockProductRepository.getProductByBarcode(barcode))
            .thenAnswer((_) async => Result.error(ServiceError(message: 'Network error')));

        // Act
        final result = await productsProvider.searchProductByBarcode(barcode);

        // Assert
        expect(result, isNull);
        verify(mockProductRepository.getProductByBarcode(barcode)).called(1);
      });

      test('should handle update failures during bulk operations', () async {
        // Arrange
        productsProvider.allProducts = [sampleProduct];
        final productIds = {1};
        const newPrice = 2499.0;

        when(mockProductRepository.updateProduct(any))
            .thenThrow(Exception('Update failed'));

        // Act & Assert
        expect(
          () => productsProvider.bulkUpdatePricesByIds(productIds, newPrice),
          throwsA(isA<Exception>()),
        );
      });

      test('should maintain data consistency after partial failures', () async {
        // Arrange
        final multipleProducts = [
          sampleProduct,
          sampleProduct.copyWith(id: 2, name: 'Product 2'),
        ];
        productsProvider.allProducts = List.from(multipleProducts);
        final productIds = {1, 2};
        const newPrice = 2499.0;

        // First update succeeds, second fails
        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(null))
            .thenThrow(Exception('Second update failed'));

        // Act & Assert
        expect(
          () => productsProvider.bulkUpdatePricesByIds(productIds, newPrice),
          throwsA(isA<Exception>()),
        );

        // The provider should maintain consistency
        expect(productsProvider.allProducts!.length, equals(2));
      });
    });

    group('Performance and Concurrency', () {
      test('should handle concurrent operations', () async {
        // Arrange
        productsProvider.allProducts = [sampleProduct];

        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async {
              await Future.delayed(const Duration(milliseconds: 10));
              return Result.success(null);
            });

        // Act
        final futures = [
          productsProvider.bulkUpdatePricesByIds({1}, 2499.0),
          productsProvider.bulkUpdateStockByIds({1}, 80),
        ];

        await Future.wait(futures);

        // Assert
        expect(productsProvider.allProducts![0].price, equals(2499));
        expect(productsProvider.allProducts![0].stock, equals(80));
        verify(mockProductRepository.updateProduct(any)).called(2);
      });

      test('should handle large dataset operations', () async {
        // Arrange
        final largeProductSet = List.generate(100, (index) => 
          sampleProduct.copyWith(id: index + 1, name: 'Product ${index + 1}'));
        productsProvider.allProducts = largeProductSet;

        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(null));

        // Act
        final allIds = largeProductSet.map((p) => p.id!).toSet();
        await productsProvider.bulkUpdatePricesByIds(allIds, 1999.0);

        // Assert
        expect(productsProvider.allProducts!.length, equals(100));
        expect(productsProvider.allProducts!.every((p) => p.price == 1999), isTrue);
        verify(mockProductRepository.updateProduct(any)).called(100);
      });
    });

    group('Low Stock Management', () {
      test('should detect and track low stock items', () async {
        // Arrange
        final lowStockProduct = sampleProduct.copyWith(stock: 2);
        when(mockProductRepository.getUserProducts(
          any,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => Result.success([lowStockProduct]));

        // Act
        await productsProvider.getAllProducts();

        // Assert
        expect(productsProvider.lowStockProducts.length, equals(1));
        expect(productsProvider.lowStockProducts.first.stock, equals(2));
      });

      test('should update low stock status after stock changes', () async {
        // Arrange
        final lowStockProduct = sampleProduct.copyWith(stock: 2);
        productsProvider.allProducts = [lowStockProduct];

        when(mockProductRepository.updateProduct(any))
            .thenAnswer((_) async => Result.success(null));

        // Act
        await productsProvider.bulkUpdateStockByIds({1}, 100); // Increase stock

        // Assert
        expect(productsProvider.lowStockProducts.length, equals(0));
      });
    });
  });
}