import 'package:flutter/material.dart';
import '../../../domain/entities/financial_transaction_entity.dart';
import '../../../domain/usecases/financial_usecases.dart';

class FinancialProvider extends ChangeNotifier {
  final GetAllFinancialTransactions _getAllFinancialTransactions;
  final GetFinancialTransactionById _getFinancialTransactionById;
  final GetFinancialTransactionsByType _getFinancialTransactionsByType;
  final GetFinancialTransactionsByCategory _getFinancialTransactionsByCategory;
  final GetFinancialTransactionsByDateRange _getFinancialTransactionsByDateRange;
  final CreateFinancialTransaction _createFinancialTransaction;
  final UpdateFinancialTransaction _updateFinancialTransaction;
  final DeleteFinancialTransaction _deleteFinancialTransaction;
  final SeedSampleFinancialTransactions _seedSampleFinancialTransactions;

  FinancialProvider(
    this._getAllFinancialTransactions,
    this._getFinancialTransactionById,
    this._getFinancialTransactionsByType,
    this._getFinancialTransactionsByCategory,
    this._getFinancialTransactionsByDateRange,
    this._createFinancialTransaction,
    this._updateFinancialTransaction,
    this._deleteFinancialTransaction,
    this._seedSampleFinancialTransactions,
  );

  List<FinancialTransactionEntity> _transactions = [];
  FinancialTransactionEntity? _selectedTransaction;
  bool _isLoading = false;
  String? _error;

  List<FinancialTransactionEntity> get transactions => _transactions;
  FinancialTransactionEntity? get selectedTransaction => _selectedTransaction;
  bool get isLoading => _isLoading;
  String? get error => _error;

  Future<void> loadAllTransactions() async {
    _setLoading(true);
    _clearError();

    final result = await _getAllFinancialTransactions(null);
    if (result.isSuccess) {
      _transactions = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load transactions');
    }
    _setLoading(false);
  }

  Future<void> loadTransactionById(int id) async {
    _setLoading(true);
    _clearError();

    final result = await _getFinancialTransactionById(id);
    if (result.isSuccess) {
      _selectedTransaction = result.data;
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load transaction');
    }
    _setLoading(false);
  }

  Future<void> loadTransactionsByType(TransactionType type) async {
    _setLoading(true);
    _clearError();

    final result = await _getFinancialTransactionsByType(type);
    if (result.isSuccess) {
      _transactions = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load transactions by type');
    }
    _setLoading(false);
  }

  Future<void> loadTransactionsByCategory(TransactionCategory category) async {
    _setLoading(true);
    _clearError();

    final result = await _getFinancialTransactionsByCategory(category);
    if (result.isSuccess) {
      _transactions = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load transactions by category');
    }
    _setLoading(false);
  }

  Future<void> loadTransactionsByDateRange(DateTime startDate, DateTime endDate) async {
    _setLoading(true);
    _clearError();

    final result = await _getFinancialTransactionsByDateRange(DateRangeParams(
      startDate: startDate,
      endDate: endDate,
    ));
    if (result.isSuccess) {
      _transactions = result.data ?? [];
      notifyListeners();
    } else {
      _setError(result.error?.message ?? 'Failed to load transactions by date range');
    }
    _setLoading(false);
  }

  Future<bool> createTransaction(FinancialTransactionEntity transaction) async {
    _setLoading(true);
    _clearError();

    final result = await _createFinancialTransaction(transaction);
    if (result.isSuccess) {
      await loadAllTransactions(); // Refresh the list
      _setLoading(false);
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to create transaction');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> updateTransaction(FinancialTransactionEntity transaction) async {
    _setLoading(true);
    _clearError();

    final result = await _updateFinancialTransaction(transaction);
    if (result.isSuccess) {
      await loadAllTransactions(); // Refresh the list
      _setLoading(false);
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to update transaction');
      _setLoading(false);
      return false;
    }
  }

  Future<bool> deleteTransaction(int id) async {
    _setLoading(true);
    _clearError();

    final result = await _deleteFinancialTransaction(id);
    if (result.isSuccess) {
      await loadAllTransactions(); // Refresh the list
      _setLoading(false);
      return true;
    } else {
      _setError(result.error?.message ?? 'Failed to delete transaction');
      _setLoading(false);
      return false;
    }
  }

  Future<void> seedSampleData() async {
    _setLoading(true);
    _clearError();

    final result = await _seedSampleFinancialTransactions(null);
    if (result.isSuccess) {
      await loadAllTransactions(); // Refresh the list
    } else {
      _setError(result.error?.message ?? 'Failed to seed sample data');
    }
    _setLoading(false);
  }

  void selectTransaction(FinancialTransactionEntity? transaction) {
    _selectedTransaction = transaction;
    notifyListeners();
  }

  void clearSelection() {
    _selectedTransaction = null;
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
  int getTotalIncome() {
    return _transactions
        .where((t) => t.type == TransactionType.revenue)
        .fold(0, (sum, t) => sum + t.amount);
  }

  int getTotalExpenses() {
    return _transactions
        .where((t) => t.type == TransactionType.expense)
        .fold(0, (sum, t) => sum + t.amount);
  }

  int getNetProfit() {
    return getTotalIncome() - getTotalExpenses();
  }

  Map<TransactionCategory, int> getTransactionsByCategory() {
    final Map<TransactionCategory, int> categoryTotals = {};
    for (final transaction in _transactions) {
      categoryTotals[transaction.category] = 
          (categoryTotals[transaction.category] ?? 0) + transaction.amount;
    }
    return categoryTotals;
  }
}
