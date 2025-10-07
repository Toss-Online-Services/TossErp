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
        transactionId: int.tryParse(payment.transactionId ?? '0') ?? 0,
        method: payment.method.name, // Convert enum to string
        amount: payment.amount,
        reference: payment.reference,
        createdAt: payment.createdAt.toIso8601String(), // Convert DateTime to string
        updatedAt: payment.updatedAt?.toIso8601String() ?? DateTime.now().toIso8601String(),
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
          .map((e) => PaymentEntity(
                id: e.id,
                transactionId: e.transactionId?.toString(),
                method: PaymentMethod.values.firstWhere(
                  (method) => method.name == e.method,
                  orElse: () => PaymentMethod.cash,
                ),
                amount: e.amount,
                reference: e.reference,
                paymentDate: e.createdAt != null ? DateTime.parse(e.createdAt!) : DateTime.now(),
                createdAt: e.createdAt != null ? DateTime.parse(e.createdAt!) : DateTime.now(),
                updatedAt: e.updatedAt != null ? DateTime.parse(e.updatedAt!) : null,
              ))
          .toList());
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<PaymentEntity>>> getAllPayments() async {
    try {
      // Stub implementation - return empty list for now
      return Result.success(<PaymentEntity>[]);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<PaymentEntity?>> getPaymentById(int id) async {
    try {
      // Stub implementation - return null for now
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<PaymentEntity>>> getPaymentsBySale(int saleId) async {
    try {
      // Stub implementation - return empty list for now
      return Result.success(<PaymentEntity>[]);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<PaymentEntity>>> getPaymentsByMethod(PaymentMethod method) async {
    try {
      // Stub implementation - return empty list for now
      return Result.success(<PaymentEntity>[]);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<PaymentEntity>>> getPaymentsByStatus(PaymentStatus status) async {
    try {
      // Stub implementation - return empty list for now
      return Result.success(<PaymentEntity>[]);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<PaymentEntity>>> getPaymentsByDateRange(DateTime startDate, DateTime endDate) async {
    try {
      // Stub implementation - return empty list for now
      return Result.success(<PaymentEntity>[]);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> updatePayment(PaymentEntity payment) async {
    try {
      // Stub implementation - do nothing for now
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> refundPayment(int id, String reason) async {
    try {
      // Stub implementation - do nothing for now
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<Map<String, dynamic>>> getPaymentsSummary(DateTime startDate, DateTime endDate) async {
    try {
      // Stub implementation - return empty summary for now
      return Result.success(<String, dynamic>{});
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}


