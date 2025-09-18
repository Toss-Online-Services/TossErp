import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/foundation.dart';
import 'package:get_it/get_it.dart';
import 'package:provider/provider.dart';
import 'package:provider/single_child_widget.dart';

import 'presentation/providers/theme/theme_provider.dart';
import 'presentation/providers/home/home_provider.dart';
import 'presentation/providers/products/products_provider.dart';
import 'domain/repositories/transaction_repository.dart';
import 'domain/repositories/product_repository.dart';
import 'domain/entities/transaction_entity.dart';
import 'domain/entities/product_entity.dart';
import 'core/usecase/usecase.dart';
import 'core/errors/errors.dart';

final GetIt sl = GetIt.instance;

// Minimal web-only transaction repository implementation
class WebTransactionRepository implements TransactionRepository {
  @override
  Future<Result<int>> syncAllUserTransactions(String userId) async {
    return Result.success(0);
  }

  @override
  Future<Result<TransactionEntity>> getTransaction(int transactionId) async {
    return Result.error(null);
  }

  @override
  Future<Result<int>> createTransaction(TransactionEntity transaction) async {
    // For web demo, just return a mock transaction ID
    return Result.success(1);
  }

  @override
  Future<Result<void>> updateTransaction(TransactionEntity transaction) async {
    return Result.success(null);
  }

  @override
  Future<Result<void>> deleteTransaction(int transactionId) async {
    return Result.success(null);
  }

  @override
  Future<Result<List<TransactionEntity>>> getUserTransactions(
    String userId, {
    String orderBy = 'id',
    String sortBy = 'DESC',
    int limit = 20,
    int? offset,
    String? contains,
  }) async {
    return Result.success(<TransactionEntity>[]);
  }
}

// Web-only product repository with sample data
class WebProductRepository implements ProductRepository {
  static final List<ProductEntity> _sampleProducts = [
    // Food Category (ID: 1)
    const ProductEntity(
      id: 1001,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Albany Brown Bread 700g',
      imageUrl: 'https://images.unsplash.com/photo-1542834369-f10ebf06d3cb?w=400',
      stock: 40,
      sold: 0,
      price: 1999,
      categoryId: 1,
      description: 'Daily fresh bread loaf',
      createdAt: '2025-09-15T15:20:52.845637',
      updatedAt: '2025-09-15T15:20:52.938990',
    ),
    const ProductEntity(
      id: 1002,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Maize Meal 10kg',
      imageUrl: 'https://images.unsplash.com/photo-1615486364033-8f2d7b8a9f2e?w=400',
      stock: 25,
      sold: 0,
      price: 12999,
      categoryId: 1,
      description: 'Staple maize meal bag',
      createdAt: '2025-09-15T15:20:52.940142',
      updatedAt: '2025-09-15T15:20:52.940262',
    ),
    const ProductEntity(
      id: 1003,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Rice 2kg',
      imageUrl: 'https://images.unsplash.com/photo-1536304993881-ff6e9eefa2a6?w=400',
      stock: 30,
      sold: 0,
      price: 4599,
      categoryId: 1,
      description: 'Premium white rice',
      createdAt: '2025-09-15T15:20:52.940292',
      updatedAt: '2025-09-15T15:20:52.940315',
    ),
    const ProductEntity(
      id: 1004,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Eggs (30 pack)',
      imageUrl: 'https://images.unsplash.com/photo-1518569656558-1f25e69d93d7?w=400',
      stock: 15,
      sold: 0,
      price: 6999,
      categoryId: 1,
      description: 'Fresh large eggs',
      createdAt: '2025-09-15T15:20:52.940374',
      updatedAt: '2025-09-15T15:20:52.940434',
    ),
    
    // Beverages Category (ID: 2)
    const ProductEntity(
      id: 2001,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Coca-Cola 2.25L',
      imageUrl: 'https://images.unsplash.com/photo-1589391886645-d51941baf7fb?w=400',
      stock: 36,
      sold: 0,
      price: 2499,
      categoryId: 2,
      description: 'Cold drink 2.25L',
      createdAt: '2025-09-15T15:20:52.941651',
      updatedAt: '2025-09-15T15:20:52.941678',
    ),
    const ProductEntity(
      id: 2002,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Fanta Orange 2L',
      imageUrl: 'https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?w=400',
      stock: 28,
      sold: 0,
      price: 2299,
      categoryId: 2,
      description: 'Orange flavored soda',
      createdAt: '2025-09-15T15:20:52.941693',
      updatedAt: '2025-09-15T15:20:52.941706',
    ),
    const ProductEntity(
      id: 2003,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Milk 1L',
      imageUrl: 'https://images.unsplash.com/photo-1563636619-e9143da7973b?w=400',
      stock: 22,
      sold: 0,
      price: 1899,
      categoryId: 2,
      description: 'Fresh whole milk',
      createdAt: '2025-09-15T15:20:52.941721',
      updatedAt: '2025-09-15T15:20:52.941734',
    ),
    const ProductEntity(
      id: 2004,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Mageu 1L',
      imageUrl: 'https://images.unsplash.com/photo-1566740933430-b5e70b06d2d5?w=400',
      stock: 20,
      sold: 0,
      price: 1599,
      categoryId: 2,
      description: 'Traditional fermented drink',
      createdAt: '2025-09-15T15:20:52.941756',
      updatedAt: '2025-09-15T15:20:52.941770',
    ),
    
    // Household Category (ID: 3)
    const ProductEntity(
      id: 3001,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Sunfoil Cooking Oil 2L',
      imageUrl: 'https://images.unsplash.com/photo-1505577058444-a3dab90d4253?w=400',
      stock: 20,
      sold: 0,
      price: 7499,
      categoryId: 3,
      description: 'Popular sunflower oil',
      createdAt: '2025-09-15T15:20:52.941784',
      updatedAt: '2025-09-15T15:20:52.941799',
    ),
    const ProductEntity(
      id: 3002,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Paraffin 1L',
      imageUrl: 'https://images.unsplash.com/photo-1600959907703-28ffe1108f7f?w=400',
      stock: 18,
      sold: 0,
      price: 2999,
      categoryId: 3,
      description: 'Lighting paraffin 1L',
      createdAt: '2025-09-15T15:20:52.941815',
      updatedAt: '2025-09-15T15:20:52.941831',
    ),
    const ProductEntity(
      id: 3003,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Candles (Pack of 6)',
      imageUrl: 'https://images.unsplash.com/photo-1543883658-3efdbf9fb6dd?w=400',
      stock: 50,
      sold: 0,
      price: 899,
      categoryId: 3,
      description: 'Emergency candles',
      createdAt: '2025-09-15T15:20:52.941847',
      updatedAt: '2025-09-15T15:20:52.941863',
    ),
    const ProductEntity(
      id: 3004,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Soap Bar',
      imageUrl: 'https://images.unsplash.com/photo-1585128903675-62ba048be788?w=400',
      stock: 35,
      sold: 0,
      price: 599,
      categoryId: 3,
      description: 'Personal hygiene soap',
      createdAt: '2025-09-15T15:20:52.941879',
      updatedAt: '2025-09-15T15:20:52.941892',
    ),
    
    // Airtime Category (ID: 4)
    const ProductEntity(
      id: 4001,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Airtime Voucher R10',
      imageUrl: 'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400',
      stock: 9999,
      sold: 0,
      price: 1000,
      categoryId: 4,
      description: 'Prepaid airtime R10',
      createdAt: '2025-09-15T15:20:52.941906',
      updatedAt: '2025-09-15T15:20:52.941919',
    ),
    const ProductEntity(
      id: 4002,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Airtime Voucher R20',
      imageUrl: 'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400',
      stock: 9999,
      sold: 0,
      price: 2000,
      categoryId: 4,
      description: 'Prepaid airtime R20',
      createdAt: '2025-09-15T15:20:52.941932',
      updatedAt: '2025-09-15T15:20:52.941945',
    ),
    const ProductEntity(
      id: 4003,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Airtime Voucher R50',
      imageUrl: 'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400',
      stock: 9999,
      sold: 0,
      price: 5000,
      categoryId: 4,
      description: 'Prepaid airtime R50',
      createdAt: '2025-09-15T15:20:52.941962',
      updatedAt: '2025-09-15T15:20:52.941976',
    ),
    const ProductEntity(
      id: 4004,
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      name: 'Data Bundle 1GB',
      imageUrl: 'https://images.unsplash.com/photo-1593642532973-d31b6557fa68?w=400',
      stock: 9999,
      sold: 0,
      price: 8999,
      categoryId: 4,
      description: 'Mobile data 1GB bundle',
      createdAt: '2025-09-15T15:20:52.941991',
      updatedAt: '2025-09-15T15:20:52.942004',
    ),
  ];

  @override
  Future<Result<List<ProductEntity>>> getUserProducts(
    String userId, {
    String orderBy = 'id',
    String sortBy = 'DESC',
    int limit = 20,
    int? offset,
    String? contains,
  }) async {
    if (kDebugMode) {
      debugPrint('🌐 WebProductRepository: getUserProducts called');
      debugPrint('🌐 UserId: $userId');
      debugPrint('🌐 OrderBy: $orderBy');
      debugPrint('🌐 SortBy: $sortBy');
      debugPrint('🌐 Limit: $limit');
      debugPrint('🌐 Total sample products: ${_sampleProducts.length}');
    }

    await Future.delayed(const Duration(milliseconds: 100));

    List<ProductEntity> products = List.from(_sampleProducts);

    if (contains?.isNotEmpty ?? false) {
      products = products.where((product) =>
        product.name.toLowerCase().contains(contains!.toLowerCase()) ||
        product.description?.toLowerCase().contains(contains.toLowerCase()) ?? false
      ).toList();
    }

    if (orderBy == 'name') {
      products.sort((a, b) => sortBy == 'ASC' ? a.name.compareTo(b.name) : b.name.compareTo(a.name));
    } else if (orderBy == 'createdAt') {
      products.sort((a, b) => sortBy == 'ASC' ? (a.createdAt ?? '').compareTo(b.createdAt ?? '') : (b.createdAt ?? '').compareTo(a.createdAt ?? ''));
    } else {
      products.sort((a, b) => sortBy == 'ASC' ? (a.id ?? 0).compareTo(b.id ?? 0) : (b.id ?? 0).compareTo(a.id ?? 0));
    }

    if (offset != null && offset > 0) {
      products = products.skip(offset).toList();
    }
    if (limit > 0 && products.length > limit) {
      products = products.take(limit).toList();
    }

    if (kDebugMode) {
      debugPrint('🌐 WebProductRepository: Returning ${products.length} products');
    }

    return Result.success(products);
  }

  @override
  Future<Result<ProductEntity>> getProduct(int productId) async {
    await Future.delayed(const Duration(milliseconds: 50));
    
    try {
      final product = _sampleProducts.firstWhere((p) => p.id == productId);
      return Result.success(product);
    } catch (e) {
      return Result.error(const ServiceError(message: 'Product not found'));
    }
  }

  @override
  Future<Result<ProductEntity?>> getProductByBarcode(String barcode) async {
    await Future.delayed(const Duration(milliseconds: 50));
    
    // For web demo, barcode is not implemented - return null
    return Result.success(null);
  }

  @override
  Future<Result<int>> createProduct(ProductEntity product) async {
    // For web demo, just return success
    return Result.success(product.id ?? DateTime.now().millisecondsSinceEpoch);
  }

  @override
  Future<Result<void>> updateProduct(ProductEntity product) async {
    return Result.success(null);
  }

  @override
  Future<Result<void>> deleteProduct(int productId) async {
    return Result.success(null);
  }

  @override
  Future<Result<int>> syncAllUserProducts(String userId) async {
    return Result.success(_sampleProducts.length);
  }
}

// Web-specific Service Locator (minimal dependencies)
void setupServiceLocatorWeb() {
  try {
    // Only register what we need for web
    sl.registerSingleton<FirebaseFirestore>(FirebaseFirestore.instance);
    sl.registerLazySingleton(() => ThemeProvider());
    sl.registerLazySingleton<TransactionRepository>(() => WebTransactionRepository());
    sl.registerLazySingleton<ProductRepository>(() => WebProductRepository());
    sl.registerLazySingleton(() => HomeProvider(transactionRepository: sl<TransactionRepository>()));
    sl.registerLazySingleton(() => ProductsProvider(productRepository: sl<ProductRepository>()));
    
    debugPrint('Web service locator setup complete');
  } catch (e) {
    debugPrint('Web service locator setup error: $e');
  }
}

// Web-compatible providers (minimal)
final List<SingleChildWidget> webProviders = [
  ChangeNotifierProvider(create: (_) => sl<ThemeProvider>()),
  ChangeNotifierProvider(create: (_) => sl<HomeProvider>()),
  ChangeNotifierProvider(create: (_) => sl<ProductsProvider>()),
];
