import '../../core/usecase/usecase.dart';
import '../../core/errors/errors.dart';
import '../entities/financial_transaction_entity.dart';
import '../repositories/financial_transaction_repository.dart';

class GetAllFinancialTransactions extends UseCase<Result<List<FinancialTransactionEntity>>, void> {
  final FinancialTransactionRepository _repository;

  GetAllFinancialTransactions(this._repository);

  @override
  Future<Result<List<FinancialTransactionEntity>>> call(void params) async {
    try {
      return await _repository.getAllFinancialTransactions();
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetFinancialTransactionById extends UseCase<Result<FinancialTransactionEntity?>, int> {
  final FinancialTransactionRepository _repository;

  GetFinancialTransactionById(this._repository);

  @override
  Future<Result<FinancialTransactionEntity?>> call(int params) async {
    try {
      return await _repository.getFinancialTransactionById(params);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetFinancialTransactionsByType extends UseCase<Result<List<FinancialTransactionEntity>>, TransactionType> {
  final FinancialTransactionRepository _repository;

  GetFinancialTransactionsByType(this._repository);

  @override
  Future<Result<List<FinancialTransactionEntity>>> call(TransactionType params) async {
    try {
      return await _repository.getFinancialTransactionsByType(params);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetFinancialTransactionsByCategory extends UseCase<Result<List<FinancialTransactionEntity>>, TransactionCategory> {
  final FinancialTransactionRepository _repository;

  GetFinancialTransactionsByCategory(this._repository);

  @override
  Future<Result<List<FinancialTransactionEntity>>> call(TransactionCategory params) async {
    try {
      return await _repository.getFinancialTransactionsByCategory(params);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetFinancialTransactionsByDateRange extends UseCase<Result<List<FinancialTransactionEntity>>, DateRangeParams> {
  final FinancialTransactionRepository _repository;

  GetFinancialTransactionsByDateRange(this._repository);

  @override
  Future<Result<List<FinancialTransactionEntity>>> call(DateRangeParams params) async {
    try {
      return await _repository.getFinancialTransactionsByDateRange(params.startDate, params.endDate);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class CreateFinancialTransaction extends UseCase<Result<int>, FinancialTransactionEntity> {
  final FinancialTransactionRepository _repository;

  CreateFinancialTransaction(this._repository);

  @override
  Future<Result<int>> call(FinancialTransactionEntity params) async {
    try {
      return await _repository.createFinancialTransaction(params);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class UpdateFinancialTransaction extends UseCase<Result<void>, FinancialTransactionEntity> {
  final FinancialTransactionRepository _repository;

  UpdateFinancialTransaction(this._repository);

  @override
  Future<Result<void>> call(FinancialTransactionEntity params) async {
    try {
      return await _repository.updateFinancialTransaction(params);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class DeleteFinancialTransaction extends UseCase<Result<void>, int> {
  final FinancialTransactionRepository _repository;

  DeleteFinancialTransaction(this._repository);

  @override
  Future<Result<void>> call(int params) async {
    try {
      return await _repository.deleteFinancialTransaction(params);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class SeedSampleFinancialTransactions extends UseCase<Result<void>, void> {
  final FinancialTransactionRepository _repository;

  SeedSampleFinancialTransactions(this._repository);

  @override
  Future<Result<void>> call(void params) async {
    try {
      return await _repository.seedSampleFinancialTransactions();
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class DateRangeParams {
  final DateTime startDate;
  final DateTime endDate;

  DateRangeParams({required this.startDate, required this.endDate});
}
