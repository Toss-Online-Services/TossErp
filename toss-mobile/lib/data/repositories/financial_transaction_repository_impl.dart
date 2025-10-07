import '../../core/usecase/usecase.dart';
import '../../core/errors/errors.dart';
import '../../domain/entities/financial_transaction_entity.dart';
import '../../domain/repositories/financial_transaction_repository.dart';
import '../datasources/interfaces/financial_transaction_datasource.dart';
import '../datasources/local/queued_action_local_datasource_impl.dart';
import '../models/queued_action_model.dart';

class FinancialTransactionRepositoryImpl implements FinancialTransactionRepository {
  final FinancialTransactionDatasource _localDatasource;
  final FinancialTransactionDatasource _remoteDatasource;
  final QueuedActionLocalDatasourceImpl _queuedActionDatasource;

  FinancialTransactionRepositoryImpl(
    this._localDatasource,
    this._remoteDatasource,
    this._queuedActionDatasource,
  );

  @override
  Future<Result<List<FinancialTransactionEntity>>> getAllFinancialTransactions() async {
    try {
      final transactions = await _localDatasource.getAllFinancialTransactions();
      return Result.success(transactions);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<FinancialTransactionEntity?>> getFinancialTransactionById(int id) async {
    try {
      final transaction = await _localDatasource.getFinancialTransactionById(id);
      return Result.success(transaction);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<FinancialTransactionEntity>>> getFinancialTransactionsByType(TransactionType type) async {
    try {
      final transactions = await _localDatasource.getTransactionsByType(type);
      return Result.success(transactions);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<FinancialTransactionEntity>>> getFinancialTransactionsByCategory(TransactionCategory category) async {
    try {
      final transactions = await _localDatasource.getTransactionsByCategory(category);
      return Result.success(transactions);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<FinancialTransactionEntity>>> getFinancialTransactionsByDateRange(DateTime startDate, DateTime endDate) async {
    try {
      final transactions = await _localDatasource.getTransactionsByDateRange(startDate, endDate);
      return Result.success(transactions);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<int>> createFinancialTransaction(FinancialTransactionEntity transaction) async {
    try {
      // Create locally first
      final id = await _localDatasource.createFinancialTransaction(transaction);
      
      // Queue for remote sync
      final queuedAction = QueuedActionModel(
        id: DateTime.now().millisecondsSinceEpoch,
        repository: 'financial_transaction',
        method: 'create',
        param: transaction.toMap().toString(),
        isCritical: true,
        createdAt: DateTime.now().toIso8601String(),
      );
      await _queuedActionDatasource.createQueuedAction(queuedAction);
      
      return Result.success(id);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> updateFinancialTransaction(FinancialTransactionEntity transaction) async {
    try {
      // Update locally first
      await _localDatasource.updateFinancialTransaction(transaction);
      
      // Queue for remote sync
      final queuedAction = QueuedActionModel(
        id: DateTime.now().millisecondsSinceEpoch,
        repository: 'financial_transaction',
        method: 'update',
        param: transaction.toMap().toString(),
        isCritical: true,
        createdAt: DateTime.now().toIso8601String(),
      );
      await _queuedActionDatasource.createQueuedAction(queuedAction);
      
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> deleteFinancialTransaction(int id) async {
    try {
      // Delete locally first
      await _localDatasource.deleteFinancialTransaction(id);
      
      // Queue for remote sync
      final queuedAction = QueuedActionModel(
        id: DateTime.now().millisecondsSinceEpoch,
        repository: 'financial_transaction',
        method: 'delete',
        param: id.toString(),
        isCritical: true,
        createdAt: DateTime.now().toIso8601String(),
      );
      await _queuedActionDatasource.createQueuedAction(queuedAction);
      
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> seedSampleFinancialTransactions() async {
    try {
      await _localDatasource.seedSampleFinancialTransactions();
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}
