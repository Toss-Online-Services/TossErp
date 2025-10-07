import '../../../domain/entities/financial_transaction_entity.dart';

abstract class FinancialTransactionDatasource {
  Future<int> createFinancialTransaction(FinancialTransactionEntity transaction);
  Future<List<FinancialTransactionEntity>> getAllFinancialTransactions();
  Future<FinancialTransactionEntity?> getFinancialTransactionById(int id);
  Future<List<FinancialTransactionEntity>> getTransactionsByType(TransactionType type);
  Future<List<FinancialTransactionEntity>> getTransactionsByCategory(TransactionCategory category);
  Future<List<FinancialTransactionEntity>> getTransactionsByDateRange(
    DateTime startDate,
    DateTime endDate,
  );
  Future<List<FinancialTransactionEntity>> getTransactionsByReference(
    int referenceId,
    String referenceType,
  );
  Future<void> updateFinancialTransaction(FinancialTransactionEntity transaction);
  Future<void> deleteFinancialTransaction(int id);
  Future<void> seedSampleFinancialTransactions();
}
