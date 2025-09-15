import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/foundation.dart';
import 'package:get_it/get_it.dart';
import 'package:provider/provider.dart';
import 'package:provider/single_child_widget.dart';

import 'core/usecase/usecase.dart';
import 'domain/entities/transaction_entity.dart';
import 'domain/entities/product_entity.dart';
import 'domain/repositories/transaction_repository.dart';
import 'domain/repositories/product_repository.dart';

import 'app/database/app_database.dart';
import 'simple_dashboard_manager.dart';
import 'data/datasources/local/product_local_datasource_impl.dart';
import 'data/datasources/local/queued_action_local_datasource_impl.dart';
import 'data/datasources/local/transaction_local_datasource_impl.dart';
import 'data/datasources/local/user_local_datasource_impl.dart';
import 'data/datasources/remote/product_remote_datasource_impl.dart';
import 'data/datasources/remote/transaction_remote_datasource_impl.dart';
import 'data/datasources/remote/user_remote_datasource_impl.dart';
import 'data/datasources/local/payment_local_datasource_impl.dart';
import 'data/datasources/local/discount_local_datasource_impl.dart';
import 'data/repositories/product_repository_impl.dart';
import 'data/repositories/queued_action_repository_impl.dart';
import 'data/repositories/transaction_repository_impl.dart';
import 'data/repositories/user_repository_impl.dart';
import 'data/repositories/payment_repository_impl.dart';
import 'data/repositories/discount_repository_impl.dart';
import 'data/repositories/shift_repository_impl.dart';
import 'data/repositories/appointment_repository_impl.dart';
import 'presentation/providers/account/account_provider.dart';
import 'presentation/providers/auth/auth_provider.dart';
import 'presentation/providers/home/home_provider.dart';
import 'presentation/providers/main/main_provider.dart';
import 'presentation/providers/products/product_detail_provider.dart';
import 'presentation/providers/products/product_form_provider.dart';
import 'presentation/providers/products/products_provider.dart';
import 'presentation/providers/theme/theme_provider.dart';
import 'presentation/providers/transactions/transaction_detail_provider.dart';
import 'presentation/providers/transactions/transactions_provider.dart';
import 'data/datasources/local/shift_local_datasource_impl.dart';
import 'data/datasources/local/cash_movement_local_datasource_impl.dart';
import 'data/datasources/local/z_report_local_datasource_impl.dart';
import 'data/datasources/local/appointment_local_datasource_impl.dart';
import 'presentation/providers/shifts/shift_provider.dart';
import 'data/datasources/local/customer_local_datasource_impl.dart';
import 'data/repositories/customer_repository_impl.dart';
import 'domain/repositories/user_repository.dart';
import 'domain/repositories/product_repository.dart';
import 'domain/repositories/queued_action_repository.dart';

final GetIt sl = GetIt.instance;

// Service Locator for Web (simplified with dummy repositories)
void setupWebServiceLocator() async {
  try {
    // Register Firebase Firestore for web
    sl.registerSingleton<FirebaseFirestore>(FirebaseFirestore.instance);
    
    // Register basic providers that work without SQLite
    sl.registerLazySingleton(() => ThemeProvider());
    sl.registerLazySingleton(() => SimpleDashboardManager());
    
    // Create dummy repositories for web
    final dummyUserRepo = DummyUserRepository();
    final dummyProductRepo = DummyProductRepository();
    final dummyTransactionRepo = DummyTransactionRepository();
    final dummyQueuedActionRepo = DummyQueuedActionRepository();
    
    // Register web-compatible providers with dummy repositories
    sl.registerLazySingleton(() => MainProvider(
      userRepository: dummyUserRepo,
      productRepository: dummyProductRepo,
      transactionRepository: dummyTransactionRepo,
      queuedActionRepository: dummyQueuedActionRepo,
    ));
    sl.registerLazySingleton(() => HomeProvider(transactionRepository: dummyTransactionRepo));
    sl.registerLazySingleton(() => ProductsProvider(productRepository: dummyProductRepo));
    
    debugPrint('Web service locator setup completed');
  } catch (e) {
    debugPrint('Web service locator setup error: $e');
  }
}

// Dummy repository implementations for web
class DummyUserRepository implements UserRepository {
  @override
  noSuchMethod(Invocation invocation) => Future.value(null);
}

class DummyProductRepository implements ProductRepository {
  static final List<ProductEntity> _products = [
    ProductEntity(
      id: 1,
      createdById: 'demo-user',
      name: 'Coca Cola 330ml',
      sku: 'COKE-330',
      barcode: '1234567890123',
      imageUrl: 'https://via.placeholder.com/150',
      price: 250, // R2.50 in cents
      stock: 50,
      sold: 25,
      costPrice: 150,
      description: 'Refreshing cola drink',
      unit: 'can',
      categoryId: 1,
      lowStockThreshold: 10,
      reorderPoint: 20,
      reorderQuantity: 100,
    ),
    ProductEntity(
      id: 2,
      createdById: 'demo-user',
      name: 'White Bread 700g',
      sku: 'BREAD-WHITE',
      barcode: '2345678901234',
      imageUrl: 'https://via.placeholder.com/150',
      price: 1200, // R12.00 in cents
      stock: 30,
      sold: 45,
      costPrice: 800,
      description: 'Fresh white bread',
      unit: 'loaf',
      categoryId: 2,
      lowStockThreshold: 5,
      reorderPoint: 10,
      reorderQuantity: 50,
    ),
    ProductEntity(
      id: 3,
      createdById: 'demo-user',
      name: 'Milk 1L',
      sku: 'MILK-1L',
      barcode: '3456789012345',
      imageUrl: 'https://via.placeholder.com/150',
      price: 1800, // R18.00 in cents
      stock: 8, // Low stock
      sold: 120,
      costPrice: 1200,
      description: 'Full cream milk',
      unit: 'bottle',
      categoryId: 3,
      lowStockThreshold: 10,
      reorderPoint: 15,
      reorderQuantity: 50,
    ),
    ProductEntity(
      id: 4,
      createdById: 'demo-user',
      name: 'Banana per kg',
      sku: 'BANANA-KG',
      barcode: '4567890123456',
      imageUrl: 'https://via.placeholder.com/150',
      price: 2500, // R25.00 in cents
      stock: 20,
      sold: 80,
      costPrice: 1500,
      description: 'Fresh bananas',
      unit: 'kg',
      categoryId: 4,
      lowStockThreshold: 5,
      reorderPoint: 10,
      reorderQuantity: 30,
    ),
    ProductEntity(
      id: 5,
      createdById: 'demo-user',
      name: 'Chocolate Bar',
      sku: 'CHOC-BAR',
      barcode: '5678901234567',
      imageUrl: 'https://via.placeholder.com/150',
      price: 1500, // R15.00 in cents
      stock: 75,
      sold: 200,
      costPrice: 900,
      description: 'Milk chocolate bar',
      unit: 'piece',
      categoryId: 5,
      lowStockThreshold: 20,
      reorderPoint: 30,
      reorderQuantity: 100,
    ),
    ProductEntity(
      id: 6,
      createdById: 'demo-user',
      name: 'Instant Coffee 200g',
      sku: 'COFFEE-INST',
      barcode: '6789012345678',
      imageUrl: 'https://via.placeholder.com/150',
      price: 8500, // R85.00 in cents
      stock: 3, // Low stock
      sold: 35,
      costPrice: 6000,
      description: 'Premium instant coffee',
      unit: 'jar',
      categoryId: 6,
      lowStockThreshold: 5,
      reorderPoint: 10,
      reorderQuantity: 25,
    ),
  ];

  @override
  Future<Result<int>> syncAllUserProducts(String userId) async {
    return Result.success(_products.length);
  }

  @override
  Future<Result<ProductEntity>> getProduct(int productId) async {
    try {
      final product = _products.firstWhere((p) => p.id == productId);
      return Result.success(product);
    } catch (e) {
      return Result.error(null);
    }
  }

  @override
  Future<Result<int>> createProduct(ProductEntity product) async {
    final newId = _products.length + 1;
    final newProduct = product.copyWith(
      id: newId,
      createdAt: DateTime.now().toIso8601String(),
      updatedAt: DateTime.now().toIso8601String(),
    );
    _products.add(newProduct);
    return Result.success(newId);
  }

  @override
  Future<Result<void>> updateProduct(ProductEntity product) async {
    final index = _products.indexWhere((p) => p.id == product.id);
    if (index != -1) {
      _products[index] = product.copyWith(
        updatedAt: DateTime.now().toIso8601String(),
      );
    }
    return Result.success(null);
  }

  @override
  Future<Result<void>> deleteProduct(int productId) async {
    _products.removeWhere((p) => p.id == productId);
    return Result.success(null);
  }

  @override
  Future<Result<List<ProductEntity>>> getUserProducts(
    String userId, {
    String orderBy = 'name',
    String sortBy = 'asc',
    int limit = 50,
    int? offset,
    String? contains,
  }) async {
    var filteredProducts = _products.where((p) => p.createdById == userId).toList();
    
    // Apply search filter
    if (contains != null && contains.isNotEmpty) {
      filteredProducts = filteredProducts.where((p) {
        return p.name.toLowerCase().contains(contains.toLowerCase()) ||
               (p.sku?.toLowerCase().contains(contains.toLowerCase()) == true) ||
               (p.barcode?.contains(contains) == true) ||
               (p.description?.toLowerCase().contains(contains.toLowerCase()) == true);
      }).toList();
    }
    
    // Apply sorting
    filteredProducts.sort((a, b) {
      int comparison = 0;
      switch (orderBy) {
        case 'name':
          comparison = a.name.compareTo(b.name);
          break;
        case 'price':
          comparison = a.price.compareTo(b.price);
          break;
        case 'stock':
          comparison = a.stock.compareTo(b.stock);
          break;
        case 'sold':
          comparison = (a.sold ?? 0).compareTo(b.sold ?? 0);
          break;
        default:
          comparison = a.name.compareTo(b.name);
      }
      
      return sortBy == 'desc' ? -comparison : comparison;
    });
    
    // Apply pagination
    if (offset != null) {
      if (offset >= filteredProducts.length) {
        return Result.success([]);
      }
      filteredProducts = filteredProducts.skip(offset).toList();
    }
    
    filteredProducts = filteredProducts.take(limit).toList();
    
    return Result.success(filteredProducts);
  }

  // Additional helper method for barcode search
  Future<Result<ProductEntity?>> getProductByBarcode(String barcode) async {
    try {
      final product = _products.firstWhere(
        (p) => p.barcode == barcode,
      );
      return Result.success(product);
    } catch (e) {
      return Result.success(null); // Not found is not an error
    }
  }

  // Additional helper method for low stock products
  Future<Result<List<ProductEntity>>> getLowStockProducts({
    int? threshold,
    int? limit,
  }) async {
    var lowStockProducts = _products.where((p) {
      final thresholdValue = threshold ?? p.lowStockThreshold ?? 10;
      return p.stock <= thresholdValue;
    }).toList();
    
    // Sort by stock level (lowest first)
    lowStockProducts.sort((a, b) => a.stock.compareTo(b.stock));
    
    if (limit != null) {
      lowStockProducts = lowStockProducts.take(limit).toList();
    }
    
    return Result.success(lowStockProducts);
  }
}

class DummyTransactionRepository implements TransactionRepository {
  static final List<TransactionEntity> _transactions = [
    TransactionEntity(
      id: 1,
      paymentMethod: 'Cash',
      customerName: 'John Doe',
      customerPhone: '+1-555-0123',
      description: 'Regular sale',
      createdById: 'demo-user',
      receivedAmount: 5000,
      returnAmount: 0,
      totalAmount: 5000,
      totalOrderedProduct: 3,
      createdAt: DateTime.now().subtract(Duration(hours: 2)).toIso8601String(),
      updatedAt: DateTime.now().subtract(Duration(hours: 2)).toIso8601String(),
    ),
    TransactionEntity(
      id: 2,
      paymentMethod: 'Card',
      customerName: 'Jane Smith',
      customerPhone: '+1-555-0456',
      description: 'Credit card payment',
      createdById: 'demo-user',
      receivedAmount: 7500,
      returnAmount: 0,
      totalAmount: 7500,
      totalOrderedProduct: 2,
      createdAt: DateTime.now().subtract(Duration(hours: 5)).toIso8601String(),
      updatedAt: DateTime.now().subtract(Duration(hours: 5)).toIso8601String(),
    ),
    TransactionEntity(
      id: 3,
      paymentMethod: 'Mobile Payment',
      customerName: 'Bob Wilson',
      description: 'Mobile payment via app',
      createdById: 'demo-user',
      receivedAmount: 3200,
      returnAmount: 200,
      totalAmount: 3000,
      totalOrderedProduct: 1,
      createdAt: DateTime.now().subtract(Duration(days: 1)).toIso8601String(),
      updatedAt: DateTime.now().subtract(Duration(days: 1)).toIso8601String(),
    ),
    TransactionEntity(
      id: 4,
      paymentMethod: 'Cash',
      customerName: 'Alice Johnson',
      customerPhone: '+1-555-0789',
      description: 'Morning purchase',
      createdById: 'demo-user',
      receivedAmount: 1250,
      returnAmount: 50,
      totalAmount: 1200,
      totalOrderedProduct: 4,
      createdAt: DateTime.now().subtract(Duration(hours: 8)).toIso8601String(),
      updatedAt: DateTime.now().subtract(Duration(hours: 8)).toIso8601String(),
    ),
    TransactionEntity(
      id: 5,
      paymentMethod: 'Card',
      customerName: 'Mike Davis',
      description: 'Bulk purchase',
      createdById: 'demo-user',
      receivedAmount: 12000,
      returnAmount: 0,
      totalAmount: 12000,
      totalOrderedProduct: 8,
      createdAt: DateTime.now().subtract(Duration(days: 2)).toIso8601String(),
      updatedAt: DateTime.now().subtract(Duration(days: 2)).toIso8601String(),
    ),
  ];

  @override
  Future<Result<int>> syncAllUserTransactions(String userId) async {
    // For demo purposes, return success
    return Result.success(_transactions.length);
  }

  @override
  Future<Result<TransactionEntity>> getTransaction(int transactionId) async {
    try {
      final transaction = _transactions.firstWhere(
        (t) => t.id == transactionId,
      );
      return Result.success(transaction);
    } catch (e) {
      return Result.error(null);
    }
  }

  @override
  Future<Result<int>> createTransaction(TransactionEntity transaction) async {
    final newId = _transactions.length + 1;
    final newTransaction = transaction.copyWith(
      id: newId,
      createdAt: DateTime.now().toIso8601String(),
      updatedAt: DateTime.now().toIso8601String(),
    );
    _transactions.add(newTransaction);
    return Result.success(newId);
  }

  @override
  Future<Result<void>> updateTransaction(TransactionEntity transaction) async {
    final index = _transactions.indexWhere((t) => t.id == transaction.id);
    if (index != -1) {
      _transactions[index] = transaction.copyWith(
        updatedAt: DateTime.now().toIso8601String(),
      );
    }
    return Result.success(null);
  }

  @override
  Future<Result<void>> deleteTransaction(int transactionId) async {
    _transactions.removeWhere((t) => t.id == transactionId);
    return Result.success(null);
  }

  @override
  Future<Result<List<TransactionEntity>>> getUserTransactions(
    String userId, {
    String orderBy = 'createdAt',
    String sortBy = 'desc',
    int limit = 20,
    int? offset,
    String? contains,
  }) async {
    var filteredTransactions = _transactions.where((t) => t.createdById == userId).toList();
    
    // Apply contains filter if provided
    if (contains != null && contains.isNotEmpty) {
      filteredTransactions = filteredTransactions.where((t) {
        return t.customerName?.toLowerCase().contains(contains.toLowerCase()) == true ||
               t.description?.toLowerCase().contains(contains.toLowerCase()) == true ||
               t.paymentMethod.toLowerCase().contains(contains.toLowerCase());
      }).toList();
    }
    
    // Sort transactions
    filteredTransactions.sort((a, b) {
      int comparison = 0;
      switch (orderBy) {
        case 'createdAt':
          comparison = DateTime.parse(a.createdAt!).compareTo(DateTime.parse(b.createdAt!));
          break;
        case 'totalAmount':
          comparison = a.totalAmount.compareTo(b.totalAmount);
          break;
        case 'customerName':
          comparison = (a.customerName ?? '').compareTo(b.customerName ?? '');
          break;
        default:
          comparison = DateTime.parse(a.createdAt!).compareTo(DateTime.parse(b.createdAt!));
      }
      
      return sortBy == 'desc' ? -comparison : comparison;
    });
    
    // Apply pagination
    if (offset != null) {
      if (offset >= filteredTransactions.length) {
        return Result.success([]);
      }
      filteredTransactions = filteredTransactions.skip(offset).toList();
    }
    
    filteredTransactions = filteredTransactions.take(limit).toList();
    
    return Result.success(filteredTransactions);
  }
}

class DummyQueuedActionRepository implements QueuedActionRepository {
  @override
  noSuchMethod(Invocation invocation) => Future.value(null);
}

// Service Locator
void setupServiceLocator() async {
  // For web, skip database initialization to avoid SQLite issues
  if (!kIsWeb) {
    sl.registerSingleton<AppDatabase>(AppDatabase());
  }
  sl.registerSingleton<FirebaseFirestore>(FirebaseFirestore.instance);

  // Datasources
  // Local Datasources (skip for web since they use SQLite)
  if (!kIsWeb) {
    sl.registerLazySingleton(() => ProductLocalDatasourceImpl(sl<AppDatabase>()));
    sl.registerLazySingleton(() => TransactionLocalDatasourceImpl(sl<AppDatabase>()));
    sl.registerLazySingleton(() => UserLocalDatasourceImpl(sl<AppDatabase>()));
    sl.registerLazySingleton(() => QueuedActionLocalDatasourceImpl(sl<AppDatabase>()));
    sl.registerLazySingleton(() => PaymentLocalDatasourceImpl(sl<AppDatabase>()));
    sl.registerLazySingleton(() => DiscountLocalDatasourceImpl(sl<AppDatabase>()));
    sl.registerLazySingleton(() => ShiftLocalDatasourceImpl(sl<AppDatabase>()));
    sl.registerLazySingleton(() => CashMovementLocalDatasourceImpl(sl<AppDatabase>()));
    sl.registerLazySingleton(() => ZReportLocalDatasourceImpl(sl<AppDatabase>()));
    sl.registerLazySingleton(() => AppointmentLocalDatasourceImpl(sl<AppDatabase>()));
    sl.registerLazySingleton(() => CustomerLocalDatasourceImpl(sl<AppDatabase>()));
  }
  
  // Remote Datasources
  sl.registerLazySingleton(() => ProductRemoteDatasourceImpl(sl<FirebaseFirestore>()));
  sl.registerLazySingleton(() => TransactionRemoteDatasourceImpl(sl<FirebaseFirestore>()));
  sl.registerLazySingleton(() => UserRemoteDatasourceImpl(sl<FirebaseFirestore>()));

  // Repositories (simplified - use mock/minimal implementations for web)
  if (!kIsWeb) {
    sl.registerLazySingleton(
      () => ProductRepositoryImpl(
        productLocalDatasource: sl<ProductLocalDatasourceImpl>(),
        productRemoteDatasource: sl<ProductRemoteDatasourceImpl>(),
        queuedActionLocalDatasource: sl<QueuedActionLocalDatasourceImpl>(),
      ),
    );
    sl.registerLazySingleton(
      () => TransactionRepositoryImpl(
        transactionLocalDatasource: sl<TransactionLocalDatasourceImpl>(),
        transactionRemoteDatasource: sl<TransactionRemoteDatasourceImpl>(),
        queuedActionLocalDatasource: sl<QueuedActionLocalDatasourceImpl>(),
      ),
    );
    sl.registerLazySingleton(
      () => UserRepositoryImpl(
        userLocalDatasource: sl<UserLocalDatasourceImpl>(),
        userRemoteDatasource: sl<UserRemoteDatasourceImpl>(),
        queuedActionLocalDatasource: sl<QueuedActionLocalDatasourceImpl>(),
      ),
    );
    sl.registerLazySingleton(
      () => QueuedActionRepositoryImpl(
        queuedActionLocalDatasource: sl<QueuedActionLocalDatasourceImpl>(),
        userRemoteDatasource: sl<UserRemoteDatasourceImpl>(),
        transactionRemoteDatasource: sl<TransactionRemoteDatasourceImpl>(),
        productRemoteDatasource: sl<ProductRemoteDatasourceImpl>(),
      ),
    );
    sl.registerLazySingleton(() => PaymentRepositoryImpl(paymentLocalDatasource: sl<PaymentLocalDatasourceImpl>()));
    sl.registerLazySingleton(() => DiscountRepositoryImpl(discountLocalDatasource: sl<DiscountLocalDatasourceImpl>()));
    sl.registerLazySingleton(() => ShiftRepositoryImpl(
          shiftLocalDatasource: sl<ShiftLocalDatasourceImpl>(),
          cashMovementLocalDatasource: sl<CashMovementLocalDatasourceImpl>(),
          zReportLocalDatasource: sl<ZReportLocalDatasourceImpl>(),
        ));
    sl.registerLazySingleton(() => AppointmentRepositoryImpl(appointmentLocalDatasource: sl<AppointmentLocalDatasourceImpl>()));
    sl.registerLazySingleton(() => CustomerRepositoryImpl(local: sl<CustomerLocalDatasourceImpl>()));
  }

  // Providers (web-compatible versions)
  try {
    if (!kIsWeb) {
      sl.registerLazySingleton(
        () => MainProvider(
          userRepository: sl<UserRepositoryImpl>(),
          productRepository: sl<ProductRepositoryImpl>(),
          transactionRepository: sl<TransactionRepositoryImpl>(),
          queuedActionRepository: sl<QueuedActionRepositoryImpl>(),
        ),
      );
      sl.registerLazySingleton(() => AuthProvider(userRepository: sl<UserRepositoryImpl>()));
      sl.registerLazySingleton(() => HomeProvider(transactionRepository: sl<TransactionRepositoryImpl>()));
      sl.registerLazySingleton(() => ProductsProvider(productRepository: sl<ProductRepositoryImpl>()));
      sl.registerLazySingleton(() => TransactionsProvider(transactionRepository: sl<TransactionRepositoryImpl>()));
      sl.registerLazySingleton(() => AccountProvider(userRepository: sl<UserRepositoryImpl>()));
      sl.registerLazySingleton(() => ProductFormProvider(productRepository: sl<ProductRepositoryImpl>()));
      sl.registerLazySingleton(() => ProductDetailProvider(productRepository: sl<ProductRepositoryImpl>()));
      sl.registerLazySingleton(() => TransactionDetailProvider(transactionRepository: sl<TransactionRepositoryImpl>()));
      sl.registerLazySingleton(() => ShiftProvider(shiftRepository: sl<ShiftRepositoryImpl>()));
    }
    
    // Always register theme provider and dashboard manager
    sl.registerLazySingleton(() => ThemeProvider());
    sl.registerLazySingleton(() => SimpleDashboardManager());
  } catch (e) {
    debugPrint('Service locator setup error: $e');
  }
}

// All providers (web-compatible version)
final List<SingleChildWidget> providers = [
  // Always include theme and dashboard providers
  ChangeNotifierProvider(create: (_) => sl<ThemeProvider>()),
  ChangeNotifierProvider(create: (_) => sl<SimpleDashboardManager>()),
  
  // Skip other providers for web since they depend on SQLite repositories
  if (!kIsWeb) ...[
    ChangeNotifierProvider(create: (_) => sl<AuthProvider>()),
    ChangeNotifierProvider(create: (_) => sl<MainProvider>()),
    ChangeNotifierProvider(create: (_) => sl<HomeProvider>()),
    ChangeNotifierProvider(create: (_) => sl<ProductsProvider>()),
    ChangeNotifierProvider(create: (_) => sl<TransactionsProvider>()),
    ChangeNotifierProvider(create: (_) => sl<AccountProvider>()),
    ChangeNotifierProvider(create: (_) => sl<ProductFormProvider>()),
    ChangeNotifierProvider(create: (_) => sl<ProductDetailProvider>()),
  ]
];
