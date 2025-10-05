import '../../core/usecase/usecase.dart';
import '../entities/expense_entity.dart';

abstract class ExpenseRepository {
  Future<Result<List<ExpenseEntity>>> getAllExpenses();
  Future<Result<ExpenseEntity?>> getExpenseById(int id);
  Future<Result<List<ExpenseEntity>>> getExpensesByStatus(ExpenseStatus status);
  Future<Result<List<ExpenseEntity>>> getExpensesByType(ExpenseType type);
  Future<Result<List<ExpenseEntity>>> getExpensesByDateRange(DateTime startDate, DateTime endDate);
  Future<Result<List<ExpenseEntity>>> getOverdueExpenses();
  Future<Result<int>> createExpense(ExpenseEntity expense);
  Future<Result<void>> updateExpense(ExpenseEntity expense);
  Future<Result<void>> deleteExpense(int id);
  Future<Result<void>> updateExpenseStatus(int id, ExpenseStatus status);
  Future<Result<void>> seedSampleExpenses();
}
