import 'package:flutter/material.dart';
import '../../../domain/entities/expense_entity.dart';
import '../../../domain/usecases/expense_usecases.dart';

class ExpenseProvider extends ChangeNotifier {
  final GetAllExpenses _getAllExpenses;
  final GetExpenseById _getExpenseById;
  final GetExpensesByStatus _getExpensesByStatus;
  final GetExpensesByType _getExpensesByType;
  final GetExpensesByDateRange _getExpensesByDateRange;
  final GetOverdueExpenses _getOverdueExpenses;
  final CreateExpense _createExpense;
  final UpdateExpense _updateExpense;
  final DeleteExpense _deleteExpense;
  final UpdateExpenseStatus _updateExpenseStatus;
  final SeedSampleExpenses _seedSampleExpenses;

  ExpenseProvider(
    this._getAllExpenses,
    this._getExpenseById,
    this._getExpensesByStatus,
    this._getExpensesByType,
    this._getExpensesByDateRange,
    this._getOverdueExpenses,
    this._createExpense,
    this._updateExpense,
    this._deleteExpense,
    this._updateExpenseStatus,
    this._seedSampleExpenses,
  );

  List<ExpenseEntity> _expenses = [];
  ExpenseEntity? _selectedExpense;
  bool _isLoading = false;
  String? _error;

  List<ExpenseEntity> get expenses => _expenses;
  ExpenseEntity? get selectedExpense => _selectedExpense;
  bool get isLoading => _isLoading;
  String? get error => _error;

  Future<void> loadAllExpenses() async {
    _setLoading(true);
    _clearError();

    final result = await _getAllExpenses(null);
    if (result.isSuccess) {
      _expenses = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load expenses');
    }
    _setLoading(false);
  }

  Future<void> loadExpenseById(int id) async {
    _setLoading(true);
    _clearError();

    final result = await _getExpenseById(id);
    if (result.isSuccess) {
      _selectedExpense = result.data;
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load expense');
    }
    _setLoading(false);
  }

  Future<void> loadExpensesByStatus(ExpenseStatus status) async {
    _setLoading(true);
    _clearError();

    final result = await _getExpensesByStatus(status);
    if (result.isSuccess) {
      _expenses = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load expenses by status');
    }
    _setLoading(false);
  }

  Future<void> loadExpensesByType(ExpenseType type) async {
    _setLoading(true);
    _clearError();

    final result = await _getExpensesByType(type);
    if (result.isSuccess) {
      _expenses = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load expenses by type');
    }
    _setLoading(false);
  }

  Future<void> loadExpensesByDateRange(DateTime startDate, DateTime endDate) async {
    _setLoading(true);
    _clearError();

    final result = await _getExpensesByDateRange(DateRangeParams(
      startDate: startDate,
      endDate: endDate,
    ));
    if (result.isSuccess) {
      _expenses = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load expenses by date range');
    }
    _setLoading(false);
  }

  Future<void> loadOverdueExpenses() async {
    _setLoading(true);
    _clearError();

    final result = await _getOverdueExpenses(null);
    if (result.isSuccess) {
      _expenses = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load overdue expenses');
    }
    _setLoading(false);
  }

  Future<bool> createExpense(ExpenseEntity expense) async {
    _setLoading(true);
    _clearError();

    final result = await _createExpense(expense);
    if (result.isSuccess) {
      await loadAllExpenses(); // Refresh the list
      _setLoading(false);
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to create expense');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> updateExpense(ExpenseEntity expense) async {
    _setLoading(true);
    _clearError();

    final result = await _updateExpense(expense);
    if (result.isSuccess) {
      await loadAllExpenses(); // Refresh the list
      _setLoading(false);
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to update expense');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> deleteExpense(int id) async {
    _setLoading(true);
    _clearError();

    final result = await _deleteExpense(id);
    if (result.isSuccess) {
      await loadAllExpenses(); // Refresh the list
      _setLoading(false);
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to delete expense');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> updateExpenseStatus(int id, ExpenseStatus status) async {
    _setLoading(true);
    _clearError();

    final result = await _updateExpenseStatus(UpdateExpenseStatusParams(id: id, status: status));
    if (result.isSuccess) {
      await loadAllExpenses(); // Refresh the list
      _setLoading(false);
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to update expense status');
      _setLoading(false);
      return false;
    }
  }

  Future<void> seedSampleData() async {
    _setLoading(true);
    _clearError();

    final result = await _seedSampleExpenses(null);
    if (result.isSuccess) {
      await loadAllExpenses(); // Refresh the list
    } else {
      _setError(result.error?.message ?? 'Failed to seed sample data');
    }
    _setLoading(false);
  }

  void selectExpense(ExpenseEntity? expense) {
    _selectedExpense = expense;
    notifyListeners();
  }

  void clearSelection() {
    _selectedExpense = null;
    notifyListeners();
  }

  void _setLoading(bool loading) {
    _isLoading = loading;
    notifyListeners();
  }

  void _setError(String error) {
    _error = error;
    notifyListeners();
  }

  void _clearError() {
    _error = null;
  }

  // Helper methods for calculations
  int getTotalExpenses() {
    return _expenses.fold(0, (sum, expense) => sum + expense.amount);
  }

  int getTotalExpensesByStatus(ExpenseStatus status) {
    return _expenses
        .where((expense) => expense.status == status)
        .fold(0, (sum, expense) => sum + expense.amount);
  }

  int getTotalExpensesByType(ExpenseType type) {
    return _expenses
        .where((expense) => expense.type == type)
        .fold(0, (sum, expense) => sum + expense.amount);
  }

  List<ExpenseEntity> getExpensesByStatus(ExpenseStatus status) {
    return _expenses.where((expense) => expense.status == status).toList();
  }

  List<ExpenseEntity> getExpensesByType(ExpenseType type) {
    return _expenses.where((expense) => expense.type == type).toList();
  }

  Map<ExpenseType, int> getExpensesByTypeSummary() {
    final Map<ExpenseType, int> typeTotals = {};
    for (final expense in _expenses) {
      typeTotals[expense.type] = 
          (typeTotals[expense.type] ?? 0) + expense.amount;
    }
    return typeTotals;
  }

  Map<ExpenseStatus, int> getExpensesByStatusSummary() {
    final Map<ExpenseStatus, int> statusTotals = {};
    for (final expense in _expenses) {
      statusTotals[expense.status] = 
          (statusTotals[expense.status] ?? 0) + expense.amount;
    }
    return statusTotals;
  }
}
