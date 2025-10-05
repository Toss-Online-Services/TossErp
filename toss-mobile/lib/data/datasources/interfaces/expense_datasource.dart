import '../../../domain/entities/expense_entity.dart';

abstract class ExpenseDatasource {
  Future<int> createExpense(ExpenseEntity expense);
  Future<List<ExpenseEntity>> getAllExpenses();
  Future<ExpenseEntity?> getExpenseById(int id);
  Future<List<ExpenseEntity>> getExpensesByStatus(ExpenseStatus status);
  Future<List<ExpenseEntity>> getExpensesByType(ExpenseType type);
  Future<List<ExpenseEntity>> getExpensesByDateRange(
    DateTime startDate,
    DateTime endDate,
  );
  Future<List<ExpenseEntity>> getOverdueExpenses();
  Future<void> updateExpense(ExpenseEntity expense);
  Future<void> deleteExpense(int id);
  Future<void> updateExpenseStatus(int id, ExpenseStatus status);
  Future<void> seedSampleExpenses();
}
