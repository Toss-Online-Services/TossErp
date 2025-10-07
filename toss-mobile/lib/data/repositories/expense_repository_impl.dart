import '../../core/usecase/usecase.dart';
import '../../core/errors/errors.dart';
import '../../domain/entities/expense_entity.dart';
import '../../domain/repositories/expense_repository.dart';
import '../datasources/interfaces/expense_datasource.dart';
import '../datasources/local/queued_action_local_datasource_impl.dart';
import '../models/queued_action_model.dart';

class ExpenseRepositoryImpl implements ExpenseRepository {
  final ExpenseDatasource _localDatasource;
  final ExpenseDatasource _remoteDatasource;
  final QueuedActionLocalDatasourceImpl _queuedActionDatasource;

  ExpenseRepositoryImpl(
    this._localDatasource,
    this._remoteDatasource,
    this._queuedActionDatasource,
  );

  @override
  Future<Result<List<ExpenseEntity>>> getAllExpenses() async {
    try {
      final expenses = await _localDatasource.getAllExpenses();
      return Result.success(expenses);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<ExpenseEntity?>> getExpenseById(int id) async {
    try {
      final expense = await _localDatasource.getExpenseById(id);
      return Result.success(expense);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<ExpenseEntity>>> getExpensesByStatus(ExpenseStatus status) async {
    try {
      final expenses = await _localDatasource.getExpensesByStatus(status);
      return Result.success(expenses);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<ExpenseEntity>>> getExpensesByType(ExpenseType type) async {
    try {
      final expenses = await _localDatasource.getExpensesByType(type);
      return Result.success(expenses);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<ExpenseEntity>>> getExpensesByDateRange(DateTime startDate, DateTime endDate) async {
    try {
      final expenses = await _localDatasource.getExpensesByDateRange(startDate, endDate);
      return Result.success(expenses);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<ExpenseEntity>>> getOverdueExpenses() async {
    try {
      final expenses = await _localDatasource.getOverdueExpenses();
      return Result.success(expenses);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<int>> createExpense(ExpenseEntity expense) async {
    try {
      // Create locally first
      final id = await _localDatasource.createExpense(expense);
      
      // Queue for remote sync
      final queuedAction = QueuedActionModel(
        id: DateTime.now().millisecondsSinceEpoch,
        repository: 'expense',
        method: 'create',
        param: expense.toMap().toString(),
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
  Future<Result<void>> updateExpense(ExpenseEntity expense) async {
    try {
      // Update locally first
      await _localDatasource.updateExpense(expense);
      
      // Queue for remote sync
      final queuedAction = QueuedActionModel(
        id: DateTime.now().millisecondsSinceEpoch,
        repository: 'expense',
        method: 'update',
        param: expense.toMap().toString(),
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
  Future<Result<void>> deleteExpense(int id) async {
    try {
      // Delete locally first
      await _localDatasource.deleteExpense(id);
      
      // Queue for remote sync
      final queuedAction = QueuedActionModel(
        id: DateTime.now().millisecondsSinceEpoch,
        repository: 'expense',
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
  Future<Result<void>> updateExpenseStatus(int id, ExpenseStatus status) async {
    try {
      // Update locally first
      await _localDatasource.updateExpenseStatus(id, status);
      
      // Queue for remote sync
      final queuedAction = QueuedActionModel(
        id: DateTime.now().millisecondsSinceEpoch,
        repository: 'expense',
        method: 'updateStatus',
        param: '{"id": $id, "status": "${status.name}"}',
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
  Future<Result<void>> seedSampleExpenses() async {
    try {
      await _localDatasource.seedSampleExpenses();
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}
