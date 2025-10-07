import '../../core/usecase/usecase.dart';
import '../entities/payment_entity.dart';

abstract class PaymentRepository {
  Future<Result<List<PaymentEntity>>> getAllPayments();
  Future<Result<PaymentEntity?>> getPaymentById(int id);
  Future<Result<List<PaymentEntity>>> getPaymentsBySale(int saleId);
  Future<Result<List<PaymentEntity>>> getPaymentsByMethod(PaymentMethod method);
  Future<Result<List<PaymentEntity>>> getPaymentsByStatus(PaymentStatus status);
  Future<Result<List<PaymentEntity>>> getPaymentsByDateRange(DateTime startDate, DateTime endDate);
  Future<Result<int>> createPayment(PaymentEntity payment);
  Future<Result<void>> updatePayment(PaymentEntity payment);
  Future<Result<void>> deletePayment(int id);
  Future<Result<void>> refundPayment(int id, String reason);
  Future<Result<Map<String, dynamic>>> getPaymentsSummary(DateTime startDate, DateTime endDate);
}
