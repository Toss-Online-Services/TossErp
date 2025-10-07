import '../../models/payment_model.dart';

abstract class PaymentDatasource {
  Future<int> createPayment(PaymentModel payment);

  Future<void> deletePayment(int id);

  Future<List<PaymentModel>> getTransactionPayments(int transactionId);
}


