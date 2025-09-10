import '../../core/usecase/usecase.dart';
import '../entities/payment_entity.dart';

abstract class PaymentRepository {
  Future<Result<int>> createPayment(PaymentEntity payment);
  Future<Result<void>> deletePayment(int id);
  Future<Result<List<PaymentEntity>>> getTransactionPayments(int transactionId);
}


