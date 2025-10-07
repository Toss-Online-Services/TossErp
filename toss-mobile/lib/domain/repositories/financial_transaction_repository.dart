import '../../core/usecase/usecase.dart';
import '../entities/financial_transaction_entity.dart';

abstract class FinancialTransactionRepository {
  Future<Result<List<FinancialTransactionEntity>>> getAllFinancialTransactions();
  Future<Result<FinancialTransactionEntity?>> getFinancialTransactionById(int id);
  Future<Result<List<FinancialTransactionEntity>>> getFinancialTransactionsByType(TransactionType type);
  Future<Result<List<FinancialTransactionEntity>>> getFinancialTransactionsByCategory(TransactionCategory category);
  Future<Result<List<FinancialTransactionEntity>>> getFinancialTransactionsByDateRange(DateTime startDate, DateTime endDate);
  Future<Result<int>> createFinancialTransaction(FinancialTransactionEntity transaction);
  Future<Result<void>> updateFinancialTransaction(FinancialTransactionEntity transaction);
  Future<Result<void>> deleteFinancialTransaction(int id);
  Future<Result<void>> seedSampleFinancialTransactions();
}
