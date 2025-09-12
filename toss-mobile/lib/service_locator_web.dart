import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/foundation.dart';
import 'package:get_it/get_it.dart';
import 'package:provider/provider.dart';
import 'package:provider/single_child_widget.dart';

import 'presentation/providers/theme/theme_provider.dart';
import 'presentation/providers/home/home_provider.dart';
import 'domain/repositories/transaction_repository.dart';
import 'domain/entities/transaction_entity.dart';
import 'core/usecase/usecase.dart';

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

// Web-specific Service Locator (minimal dependencies)
void setupServiceLocatorWeb() {
  try {
    // Only register what we need for web
    sl.registerSingleton<FirebaseFirestore>(FirebaseFirestore.instance);
    sl.registerLazySingleton(() => ThemeProvider());
    sl.registerLazySingleton<TransactionRepository>(() => WebTransactionRepository());
    sl.registerLazySingleton(() => HomeProvider(transactionRepository: sl<TransactionRepository>()));
    
    debugPrint('Web service locator setup complete');
  } catch (e) {
    debugPrint('Web service locator setup error: $e');
  }
}

// Web-compatible providers (minimal)
final List<SingleChildWidget> webProviders = [
  ChangeNotifierProvider(create: (_) => sl<ThemeProvider>()),
  ChangeNotifierProvider(create: (_) => sl<HomeProvider>()),
];
