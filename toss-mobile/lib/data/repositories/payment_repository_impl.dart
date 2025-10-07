import '../../core/errors/errors.dart';
import '../../core/usecase/usecase.dart';
import '../../domain/entities/payment_entity.dart';
import '../../domain/repositories/payment_repository.dart';
import '../datasources/local/payment_local_datasource_impl.dart';
import '../models/payment_model.dart';

class PaymentRepositoryImpl extends PaymentRepository {
  final PaymentLocalDatasourceImpl paymentLocalDatasource;

  PaymentRepositoryImpl({required this.paymentLocalDatasource});

  @override
  Future<Result<int>> createPayment(PaymentEntity payment) async {
    try {
      final id = await paymentLocalDatasource.createPayment(PaymentModel(
        id: payment.id ?? DateTime.now().millisecondsSinceEpoch,
        transactionId: payment.transactionId,
        method: payment.method.name, // Convert enum to string
        amount: payment.amount,
        reference: payment.reference,
        createdAt: payment.createdAt.toIso8601String(), // Convert DateTime to string
        updatedAt: payment.updatedAt ?? DateTime.now().toIso8601String(),
      ));
      return Result.success(id);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> deletePayment(int id) async {
    try {
      await paymentLocalDatasource.deletePayment(id);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<PaymentEntity>>> getTransactionPayments(int transactionId) async {
    try {
      final rows = await paymentLocalDatasource.getTransactionPayments(transactionId);
      return Result.success(rows
          .map((e) => PaymentEntity.legacy(
                id: e.id,
                transactionId: e.transactionId,
                methodString: e.method, // Use legacy constructor that accepts String
                amount: e.amount,
                reference: e.reference,
                createdAtString: e.createdAt, // Use legacy constructor that accepts String
                updatedAt: e.updatedAt,
              ))
          .toList());
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}


