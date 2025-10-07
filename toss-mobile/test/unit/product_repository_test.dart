import 'package:flutter_test/flutter_test.dart';
import 'package:mockito/mockito.dart';
import 'package:mockito/annotations.dart';
import 'package:toss_mobile/domain/entities/product_entity.dart';
import 'package:toss_mobile/domain/entities/product_category_entity.dart';
import 'package:toss_mobile/core/errors/errors.dart';
import 'package:toss_mobile/data/repositories/product_repository_impl.dart';
import 'package:toss_mobile/data/datasources/local/product_local_datasource_impl.dart';
import 'package:toss_mobile/data/datasources/remote/product_remote_datasource_impl.dart';
import 'package:toss_mobile/data/datasources/local/queued_action_local_datasource_impl.dart';
import 'package:toss_mobile/data/models/product_model.dart';

import 'product_repository_test.mocks.dart';

@GenerateMocks([
  ProductLocalDatasourceImpl,
  ProductRemoteDatasourceImpl,
  QueuedActionLocalDatasourceImpl,
])
void main() {
  late ProductRepositoryImpl repository;
  late MockProductLocalDatasourceImpl mockLocalDatasource;
  late MockProductRemoteDatasourceImpl mockRemoteDatasource;
  late MockQueuedActionLocalDatasourceImpl mockQueuedActionDatasource;

  setUp(() {
    mockLocalDatasource = MockProductLocalDatasourceImpl();
    mockRemoteDatasource = MockProductRemoteDatasourceImpl();
    mockQueuedActionDatasource = MockQueuedActionLocalDatasourceImpl();
    repository = ProductRepositoryImpl(
      productLocalDatasource: mockLocalDatasource,
      productRemoteDatasource: mockRemoteDatasource,
      queuedActionLocalDatasource: mockQueuedActionDatasource,
    );
  });

  group('ProductRepository Tests', () {
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
      price: 1999, // R19.99 in cents
      costPrice: 1200, // R12.00 in cents
      description: 'A test product for unit testing',
      unit: 'each',
      isActive: true,
      isFavorite: false,
      categoryId: 1,
      category: const ProductCategoryEntity(
        id: 1,
        name: 'Electronics',
        description: 'Electronic products',
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
      customAttributes: {'brand': 'TestBrand'},
      createdAt: DateTime.now().toIso8601String(),
      updatedAt: DateTime.now().toIso8601String(),
    );

    final sampleProductModel = ProductModel.fromEntity(sampleProduct);

    // Helper function to create modified ProductModel instances
    ProductModel createProductModel({
      int? id,
      String? name,
      String? barcode,
      ProductModel? base,
    }) {
      final baseModel = base ?? sampleProductModel;
      return ProductModel(
        id: id ?? baseModel.id,
        createdById: baseModel.createdById,
        name: name ?? baseModel.name,
        barcode: barcode ?? baseModel.barcode,
        imageUrl: baseModel.imageUrl,
        stock: baseModel.stock,
        sold: baseModel.sold,
        price: baseModel.price,
        description: baseModel.description,
        createdAt: baseModel.createdAt,
        updatedAt: baseModel.updatedAt,
      );
    }

    group('getUserProducts', () {
      test('should return products from local datasource when offline', () async {
        // Arrange
        const userId = 'test_user_123';
        final localProducts = [sampleProductModel];
        
        when(mockLocalDatasource.getUserProducts(
          userId,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => localProducts);

        // Act
        final result = await repository.getUserProducts(userId);

        // Assert
        expect(result.isSuccess, isTrue);
        expect(result.data?.length, equals(1));
        expect(result.data?.first.name, equals('Test Product'));
        
        verify(mockLocalDatasource.getUserProducts(
          userId,
          orderBy: 'createdAt',
          sortBy: 'DESC',
          limit: 10,
          offset: null,
          contains: null,
        )).called(1);
      });

      test('should sync products and return combined data when online', () async {
        // Arrange
        const userId = 'test_user_123';
        final localProducts = [sampleProductModel];
        final remoteProducts = [createProductModel(id: 2, name: 'Remote Product')];
        
        when(mockLocalDatasource.getUserProducts(
          userId,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => localProducts);
        
        when(mockRemoteDatasource.getUserProducts(
          userId,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => remoteProducts);

        // Act
        final result = await repository.getUserProducts(userId);

        // Assert
        expect(result.isSuccess, isTrue);
        verify(mockLocalDatasource.getUserProducts(userId, 
          orderBy: 'createdAt', sortBy: 'DESC', limit: 10, offset: null, contains: null)).called(1);
        verify(mockRemoteDatasource.getUserProducts(userId, 
          orderBy: 'createdAt', sortBy: 'DESC', limit: 10, offset: null, contains: null)).called(1);
      });

      test('should handle errors gracefully', () async {
        // Arrange
        const userId = 'test_user_123';
        when(mockLocalDatasource.getUserProducts(
          userId,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenThrow(Exception('Database error'));

        // Act
        final result = await repository.getUserProducts(userId);

        // Assert
        expect(result.isSuccess, isFalse);
        expect(result.error, isA<APIError>());
      });

      test('should filter products by contains parameter locally', () async {
        // Arrange
        const userId = 'test_user_123';
        const searchTerm = 'test';
        final products = [
          sampleProductModel,
          createProductModel(id: 2, name: 'Another Product'),
        ];
        
        when(mockLocalDatasource.getUserProducts(
          userId,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => products);
        
        when(mockRemoteDatasource.getUserProducts(
          userId,
          orderBy: anyNamed('orderBy'),
          sortBy: anyNamed('sortBy'),
          limit: anyNamed('limit'),
          offset: anyNamed('offset'),
          contains: anyNamed('contains'),
        )).thenAnswer((_) async => products);

        // Act
        final result = await repository.getUserProducts(userId, contains: searchTerm);

        // Assert
        expect(result.isSuccess, isTrue);
        expect(result.data?.length, equals(1));
        expect(result.data?.first.name.toLowerCase().contains(searchTerm), isTrue);
      });
    });

    group('getProduct', () {
      test('should return product from local datasource when offline', () async {
        // Arrange
        const productId = 1;
        
        when(mockLocalDatasource.getProduct(productId))
            .thenAnswer((_) async => sampleProductModel);

        // Act
        final result = await repository.getProduct(productId);

        // Assert
        expect(result.isSuccess, isTrue);
        expect(result.data?.id, equals(productId));
        expect(result.data?.name, equals('Test Product'));
        
        verify(mockLocalDatasource.getProduct(productId)).called(1);
      });

      test('should return null when product not found', () async {
        // Arrange
        const productId = 999;
        
        when(mockLocalDatasource.getProduct(productId))
            .thenAnswer((_) async => null);

        // Act
        final result = await repository.getProduct(productId);

        // Assert
        expect(result.isSuccess, isTrue);
        expect(result.data, isNull);
        
        verify(mockLocalDatasource.getProduct(productId)).called(1);
      });

      test('should handle errors gracefully', () async {
        // Arrange
        const productId = 1;
        
        when(mockLocalDatasource.getProduct(productId))
            .thenThrow(Exception('Database error'));

        // Act
        final result = await repository.getProduct(productId);

        // Assert
        expect(result.isSuccess, isFalse);
        expect(result.error, isA<APIError>());
      });
    });

    group('createProduct', () {
      test('should create product locally and remotely when online', () async {
        // Arrange
        const productId = 1;
        
        when(mockLocalDatasource.createProduct(any))
            .thenAnswer((_) async => productId);
        when(mockRemoteDatasource.createProduct(any))
            .thenAnswer((_) async => productId);

        // Act
        final result = await repository.createProduct(sampleProduct);

        // Assert
        expect(result.isSuccess, isTrue);
        expect(result.data, equals(productId));
        
        verify(mockLocalDatasource.createProduct(any)).called(1);
        verify(mockRemoteDatasource.createProduct(any)).called(1);
        verifyNever(mockQueuedActionDatasource.createQueuedAction(any));
      });

      test('should queue action when offline', () async {
        // Arrange
        const productId = 1;
        
        when(mockLocalDatasource.createProduct(any))
            .thenAnswer((_) async => productId);
        when(mockQueuedActionDatasource.createQueuedAction(any))
            .thenAnswer((_) async => 1);

        // Mock offline state by not setting up remote datasource

        // Act
        final result = await repository.createProduct(sampleProduct);

        // Assert
        expect(result.isSuccess, isTrue);
        expect(result.data, equals(productId));
        
        verify(mockLocalDatasource.createProduct(any)).called(1);
        verify(mockQueuedActionDatasource.createQueuedAction(any)).called(1);
      });

      test('should handle creation errors', () async {
        // Arrange
        when(mockLocalDatasource.createProduct(any))
            .thenThrow(Exception('Creation failed'));

        // Act
        final result = await repository.createProduct(sampleProduct);

        // Assert
        expect(result.isSuccess, isFalse);
        expect(result.error, isA<APIError>());
      });
    });

    group('updateProduct', () {
      test('should update product locally and remotely when online', () async {
        // Arrange
        when(mockLocalDatasource.updateProduct(any))
            .thenAnswer((_) async {});
        when(mockRemoteDatasource.updateProduct(any))
            .thenAnswer((_) async {});

        // Act
        final result = await repository.updateProduct(sampleProduct);

        // Assert
        expect(result.isSuccess, isTrue);
        
        verify(mockLocalDatasource.updateProduct(any)).called(1);
        verify(mockRemoteDatasource.updateProduct(any)).called(1);
        verifyNever(mockQueuedActionDatasource.createQueuedAction(any));
      });

      test('should queue action when offline', () async {
        // Arrange
        when(mockLocalDatasource.updateProduct(any))
            .thenAnswer((_) async {});
        when(mockQueuedActionDatasource.createQueuedAction(any))
            .thenAnswer((_) async => 1);

        // Act
        final result = await repository.updateProduct(sampleProduct);

        // Assert
        expect(result.isSuccess, isTrue);
        
        verify(mockLocalDatasource.updateProduct(any)).called(1);
        verify(mockQueuedActionDatasource.createQueuedAction(any)).called(1);
      });

      test('should handle update errors', () async {
        // Arrange
        when(mockLocalDatasource.updateProduct(any))
            .thenThrow(Exception('Update failed'));

        // Act
        final result = await repository.updateProduct(sampleProduct);

        // Assert
        expect(result.isSuccess, isFalse);
        expect(result.error, isA<APIError>());
      });
    });

    group('deleteProduct', () {
      test('should delete product locally and remotely when online', () async {
        // Arrange
        const productId = 1;
        
        when(mockLocalDatasource.deleteProduct(productId))
            .thenAnswer((_) async {});
        when(mockRemoteDatasource.deleteProduct(productId))
            .thenAnswer((_) async {});

        // Act
        final result = await repository.deleteProduct(productId);

        // Assert
        expect(result.isSuccess, isTrue);
        
        verify(mockLocalDatasource.deleteProduct(productId)).called(1);
        verify(mockRemoteDatasource.deleteProduct(productId)).called(1);
        verifyNever(mockQueuedActionDatasource.createQueuedAction(any));
      });

      test('should queue action when offline', () async {
        // Arrange
        const productId = 1;
        
        when(mockLocalDatasource.deleteProduct(productId))
            .thenAnswer((_) async {});
        when(mockQueuedActionDatasource.createQueuedAction(any))
            .thenAnswer((_) async => 1);

        // Act
        final result = await repository.deleteProduct(productId);

        // Assert
        expect(result.isSuccess, isTrue);
        
        verify(mockLocalDatasource.deleteProduct(productId)).called(1);
        verify(mockQueuedActionDatasource.createQueuedAction(any)).called(1);
      });

      test('should handle deletion errors', () async {
        // Arrange
        const productId = 1;
        
        when(mockLocalDatasource.deleteProduct(productId))
            .thenThrow(Exception('Deletion failed'));

        // Act
        final result = await repository.deleteProduct(productId);

        // Assert
        expect(result.isSuccess, isFalse);
        expect(result.error, isA<APIError>());
      });
    });

    group('syncAllUserProducts', () {
      test('should sync local and remote products when online', () async {
        // Arrange
        const userId = 'test_user_123';
        final localProducts = [sampleProductModel];
        final remoteProducts = [createProductModel(id: 2, name: 'Remote Product')];
        
        when(mockLocalDatasource.getAllUserProducts(userId))
            .thenAnswer((_) async => localProducts);
        when(mockRemoteDatasource.getAllUserProducts(userId))
            .thenAnswer((_) async => remoteProducts);

        // Act
        final result = await repository.syncAllUserProducts(userId);

        // Assert
        expect(result.isSuccess, isTrue);
        expect(result.data, isA<int>());
        
        verify(mockLocalDatasource.getAllUserProducts(userId)).called(1);
        verify(mockRemoteDatasource.getAllUserProducts(userId)).called(1);
      });

      test('should return 0 when offline', () async {
        // Arrange
        const userId = 'test_user_123';

        // Act (without setting up connectivity)
        final result = await repository.syncAllUserProducts(userId);

        // Assert
        expect(result.isSuccess, isTrue);
        expect(result.data, equals(0));
      });

      test('should handle sync errors', () async {
        // Arrange
        const userId = 'test_user_123';
        
        when(mockLocalDatasource.getAllUserProducts(userId))
            .thenThrow(Exception('Sync failed'));

        // Act
        final result = await repository.syncAllUserProducts(userId);

        // Assert
        expect(result.isSuccess, isFalse);
        expect(result.error, isA<APIError>());
      });
    });

    group('getProductByBarcode', () {
      test('should find product by barcode in local datasource', () async {
        // Arrange
        const barcode = '1234567890123';
        final products = [sampleProductModel];
        
        when(mockLocalDatasource.getAllUserProducts(''))
            .thenAnswer((_) async => products);

        // Act
        final result = await repository.getProductByBarcode(barcode);

        // Assert
        expect(result.isSuccess, isTrue);
        expect(result.data, isNotNull);
        expect(result.data?.barcode, equals(barcode));
        
        verify(mockLocalDatasource.getAllUserProducts('')).called(1);
      });

      test('should search remote when not found locally and online', () async {
        // Arrange
        const barcode = '9876543210987';
        final emptyProducts = <ProductModel>[];
        final remoteProducts = [createProductModel(barcode: barcode)];
        
        when(mockLocalDatasource.getAllUserProducts(''))
            .thenAnswer((_) async => emptyProducts);
        when(mockRemoteDatasource.getAllUserProducts(''))
            .thenAnswer((_) async => remoteProducts);
        when(mockLocalDatasource.createProduct(any))
            .thenAnswer((_) async => 1);

        // Act
        final result = await repository.getProductByBarcode(barcode);

        // Assert
        expect(result.isSuccess, isTrue);
        expect(result.data, isNotNull);
        expect(result.data?.barcode, equals(barcode));
        
        verify(mockLocalDatasource.getAllUserProducts('')).called(1);
        verify(mockRemoteDatasource.getAllUserProducts('')).called(1);
        verify(mockLocalDatasource.createProduct(any)).called(1);
      });

      test('should return null when product not found anywhere', () async {
        // Arrange
        const barcode = 'nonexistent';
        final emptyProducts = <ProductModel>[];
        
        when(mockLocalDatasource.getAllUserProducts(''))
            .thenAnswer((_) async => emptyProducts);
        when(mockRemoteDatasource.getAllUserProducts(''))
            .thenAnswer((_) async => emptyProducts);

        // Act
        final result = await repository.getProductByBarcode(barcode);

        // Assert
        expect(result.isSuccess, isTrue);
        expect(result.data, isNull);
        
        verify(mockLocalDatasource.getAllUserProducts('')).called(1);
        verify(mockRemoteDatasource.getAllUserProducts('')).called(1);
      });

      test('should handle barcode search errors', () async {
        // Arrange
        const barcode = '1234567890123';
        
        when(mockLocalDatasource.getAllUserProducts(''))
            .thenThrow(Exception('Search failed'));

        // Act
        final result = await repository.getProductByBarcode(barcode);

        // Assert
        expect(result.isSuccess, isFalse);
        expect(result.error, isA<ServiceError>());
      });

      test('should handle special barcode formats', () async {
        // Arrange
        const barcodes = ['123456789012', '1234567890123', '12345678901234'];
        final products = barcodes.map((barcode) => 
          createProductModel(barcode: barcode)).toList();
        
        when(mockLocalDatasource.getAllUserProducts(''))
            .thenAnswer((_) async => products);

        // Act & Assert
        for (final barcode in barcodes) {
          final result = await repository.getProductByBarcode(barcode);
          expect(result.isSuccess, isTrue);
          expect(result.data?.barcode, equals(barcode));
        }
      });

      test('should prioritize exact barcode matches', () async {
        // Arrange
        const targetBarcode = '1234567890123';
        final products = [
          createProductModel(id: 1, barcode: '1234567890124'), // Similar but different
          createProductModel(id: 2, barcode: targetBarcode),   // Exact match
          createProductModel(id: 3, barcode: '1234567890122'), // Similar but different
        ];
        
        when(mockLocalDatasource.getAllUserProducts(''))
            .thenAnswer((_) async => products);

        // Act
        final result = await repository.getProductByBarcode(targetBarcode);

        // Assert
        expect(result.isSuccess, isTrue);
        expect(result.data?.id, equals(2));
        expect(result.data?.barcode, equals(targetBarcode));
      });
    });
  });
}
