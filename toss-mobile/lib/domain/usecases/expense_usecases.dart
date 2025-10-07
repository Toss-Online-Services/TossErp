import '../../core/usecase/usecase.dart';
import '../../core/errors/errors.dart';
import '../entities/expense_entity.dart';
import '../repositories/expense_repository.dart';

class GetAllExpenses extends UseCase<Result<List<ExpenseEntity>>, void> {
  final ExpenseRepository _repository;

  GetAllExpenses(this._repository);

  @override
  Future<Result<List<ExpenseEntity>>> call(void params) async {
    try {
      return await _repository.getAllExpenses();
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetExpenseById extends UseCase<Result<ExpenseEntity?>, int> {
  final ExpenseRepository _repository;

  GetExpenseById(this._repository);

  @override
  Future<Result<ExpenseEntity?>> call(int params) async {
    try {
      return await _repository.getExpenseById(params);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetExpensesByStatus extends UseCase<Result<List<ExpenseEntity>>, ExpenseStatus> {
  final ExpenseRepository _repository;

  GetExpensesByStatus(this._repository);

  @override
  Future<Result<List<ExpenseEntity>>> call(ExpenseStatus params) async {
    try {
      return await _repository.getExpensesByStatus(params);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetExpensesByType extends UseCase<Result<List<ExpenseEntity>>, ExpenseType> {
  final ExpenseRepository _repository;

  GetExpensesByType(this._repository);

  @override
  Future<Result<List<ExpenseEntity>>> call(ExpenseType params) async {
    try {
      return await _repository.getExpensesByType(params);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetExpensesByDateRange extends UseCase<Result<List<ExpenseEntity>>, DateRangeParams> {
  final ExpenseRepository _repository;

  GetExpensesByDateRange(this._repository);

  @override
  Future<Result<List<ExpenseEntity>>> call(DateRangeParams params) async {
    try {
      return await _repository.getExpensesByDateRange(params.startDate, params.endDate);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class GetOverdueExpenses extends UseCase<Result<List<ExpenseEntity>>, void> {
  final ExpenseRepository _repository;

  GetOverdueExpenses(this._repository);

  @override
  Future<Result<List<ExpenseEntity>>> call(void params) async {
    try {
      return await _repository.getOverdueExpenses();
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class CreateExpense extends UseCase<Result<int>, ExpenseEntity> {
  final ExpenseRepository _repository;

  CreateExpense(this._repository);

  @override
  Future<Result<int>> call(ExpenseEntity params) async {
    try {
      return await _repository.createExpense(params);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class UpdateExpense extends UseCase<Result<void>, ExpenseEntity> {
  final ExpenseRepository _repository;

  UpdateExpense(this._repository);

  @override
  Future<Result<void>> call(ExpenseEntity params) async {
    try {
      return await _repository.updateExpense(params);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class DeleteExpense extends UseCase<Result<void>, int> {
  final ExpenseRepository _repository;

  DeleteExpense(this._repository);

  @override
  Future<Result<void>> call(int params) async {
    try {
      return await _repository.deleteExpense(params);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class UpdateExpenseStatus extends UseCase<Result<void>, UpdateExpenseStatusParams> {
  final ExpenseRepository _repository;

  UpdateExpenseStatus(this._repository);

  @override
  Future<Result<void>> call(UpdateExpenseStatusParams params) async {
    try {
      return await _repository.updateExpenseStatus(params.id, params.status);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}

class SeedSampleExpenses extends UseCase<Result<void>, void> {
  final ExpenseRepository _repository;

  SeedSampleExpenses(this._repository);

  @override
  Future<Result<void>> call(void params) async {
    try {
      return await _repository.seedSampleExpenses();
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

class UpdateExpenseStatusParams {
  final int id;
  final ExpenseStatus status;

  UpdateExpenseStatusParams({required this.id, required this.status});
}
