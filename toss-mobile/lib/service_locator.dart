import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/foundation.dart';
import 'package:get_it/get_it.dart';
import 'package:provider/provider.dart';
import 'package:provider/single_child_widget.dart';

import 'app/database/app_database.dart';
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

final GetIt sl = GetIt.instance;

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
    }
    
    // Always register theme provider
    sl.registerLazySingleton(() => ThemeProvider());
  } catch (e) {
    debugPrint('Service locator setup error: $e');
  }
}

// All providers (web-compatible version)
final List<SingleChildWidget> providers = [
  // Only include providers that work on web
  ChangeNotifierProvider(create: (_) => ThemeProvider()),
  
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
