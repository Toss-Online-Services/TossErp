import '../../core/usecase/usecase.dart';
import '../entities/sale_entity.dart';
import '../entities/payment_entity.dart';
import '../entities/receipt_entity.dart';
import '../repositories/sale_repository.dart';
import '../repositories/payment_repository.dart';
import '../repositories/receipt_repository.dart';

// Sale Use Cases
class GetAllSales extends UseCase<List<SaleEntity>, void> {
  final SaleRepository repository;

  GetAllSales(this.repository);

  @override
  Future<Result<List<SaleEntity>>> call(void params) {
    return repository.getAllSales();
  }
}

class GetSaleById extends UseCase<SaleEntity?, int> {
  final SaleRepository repository;

  GetSaleById(this.repository);

  @override
  Future<Result<SaleEntity?>> call(int params) {
    return repository.getSaleById(params);
  }
}

class GetSalesByStatus extends UseCase<List<SaleEntity>, SaleStatus> {
  final SaleRepository repository;

  GetSalesByStatus(this.repository);

  @override
  Future<Result<List<SaleEntity>>> call(SaleStatus params) {
    return repository.getSalesByStatus(params);
  }
}

class GetSalesByDateRange extends UseCase<List<SaleEntity>, DateRangeParams> {
  final SaleRepository repository;

  GetSalesByDateRange(this.repository);

  @override
  Future<Result<List<SaleEntity>>> call(DateRangeParams params) {
    return repository.getSalesByDateRange(params.startDate, params.endDate);
  }
}

class CreateSale extends UseCase<int, SaleEntity> {
  final SaleRepository repository;

  CreateSale(this.repository);

  @override
  Future<Result<int>> call(SaleEntity params) {
    return repository.createSale(params);
  }
}

class UpdateSale extends UseCase<void, SaleEntity> {
  final SaleRepository repository;

  UpdateSale(this.repository);

  @override
  Future<Result<void>> call(SaleEntity params) {
    return repository.updateSale(params);
  }
}

class CompleteSale extends UseCase<void, int> {
  final SaleRepository repository;

  CompleteSale(this.repository);

  @override
  Future<Result<void>> call(int params) {
    return repository.completeSale(params);
  }
}

class CancelSale extends UseCase<void, CancelSaleParams> {
  final SaleRepository repository;

  CancelSale(this.repository);

  @override
  Future<Result<void>> call(CancelSaleParams params) {
    return repository.cancelSale(params.saleId, params.reason);
  }
}

class GetTodaySales extends UseCase<SaleEntity?, void> {
  final SaleRepository repository;

  GetTodaySales(this.repository);

  @override
  Future<Result<SaleEntity?>> call(void params) {
    return repository.getTodaySales();
  }
}

class GetSalesSummary extends UseCase<Map<String, dynamic>, DateRangeParams> {
  final SaleRepository repository;

  GetSalesSummary(this.repository);

  @override
  Future<Result<Map<String, dynamic>>> call(DateRangeParams params) {
    return repository.getSalesSummary(params.startDate, params.endDate);
  }
}

// Payment Use Cases
class GetAllPayments extends UseCase<List<PaymentEntity>, void> {
  final PaymentRepository repository;

  GetAllPayments(this.repository);

  @override
  Future<Result<List<PaymentEntity>>> call(void params) {
    return repository.getAllPayments();
  }
}

class GetPaymentsBySale extends UseCase<List<PaymentEntity>, int> {
  final PaymentRepository repository;

  GetPaymentsBySale(this.repository);

  @override
  Future<Result<List<PaymentEntity>>> call(int params) {
    return repository.getPaymentsBySale(params);
  }
}

class CreatePayment extends UseCase<int, PaymentEntity> {
  final PaymentRepository repository;

  CreatePayment(this.repository);

  @override
  Future<Result<int>> call(PaymentEntity params) {
    return repository.createPayment(params);
  }
}

class ProcessSplitPayment extends UseCase<List<int>, List<PaymentEntity>> {
  final PaymentRepository repository;

  ProcessSplitPayment(this.repository);

  @override
  Future<Result<List<int>>> call(List<PaymentEntity> params) async {
    List<int> paymentIds = [];
    
    for (final payment in params) {
      final result = await repository.createPayment(payment);
      if (result.isSuccess && result.data != null) {
        paymentIds.add(result.data!);
      } else {
        return Result.error(result.error ?? APIError(message: 'Payment processing failed'));
      }
    }
    
    return Result.success(paymentIds);
  }
}

class RefundPayment extends UseCase<void, RefundPaymentParams> {
  final PaymentRepository repository;

  RefundPayment(this.repository);

  @override
  Future<Result<void>> call(RefundPaymentParams params) {
    return repository.refundPayment(params.paymentId, params.reason);
  }
}

class GetPaymentsSummary extends UseCase<Map<String, dynamic>, DateRangeParams> {
  final PaymentRepository repository;

  GetPaymentsSummary(this.repository);

  @override
  Future<Result<Map<String, dynamic>>> call(DateRangeParams params) {
    return repository.getPaymentsSummary(params.startDate, params.endDate);
  }
}

// Receipt Use Cases
class GetAllReceipts extends UseCase<List<ReceiptEntity>, void> {
  final ReceiptRepository repository;

  GetAllReceipts(this.repository);

  @override
  Future<Result<List<ReceiptEntity>>> call(void params) {
    return repository.getAllReceipts();
  }
}

class GetReceiptBySale extends UseCase<ReceiptEntity?, int> {
  final ReceiptRepository repository;

  GetReceiptBySale(this.repository);

  @override
  Future<Result<ReceiptEntity?>> call(int params) {
    return repository.getReceiptBySale(params);
  }
}

class CreateReceipt extends UseCase<int, ReceiptEntity> {
  final ReceiptRepository repository;

  CreateReceipt(this.repository);

  @override
  Future<Result<int>> call(ReceiptEntity params) {
    return repository.createReceipt(params);
  }
}

class GenerateReceiptPdf extends UseCase<String, int> {
  final ReceiptRepository repository;

  GenerateReceiptPdf(this.repository);

  @override
  Future<Result<String>> call(int params) {
    return repository.generateReceiptPdf(params);
  }
}

class EmailReceipt extends UseCase<void, EmailReceiptParams> {
  final ReceiptRepository repository;

  EmailReceipt(this.repository);

  @override
  Future<Result<void>> call(EmailReceiptParams params) {
    return repository.emailReceipt(params.receiptId, params.email);
  }
}

class MarkReceiptAsPrinted extends UseCase<void, int> {
  final ReceiptRepository repository;

  MarkReceiptAsPrinted(this.repository);

  @override
  Future<Result<void>> call(int params) {
    return repository.markReceiptAsPrinted(params);
  }
}

// Parameter classes
class DateRangeParams {
  final DateTime startDate;
  final DateTime endDate;

  DateRangeParams({
    required this.startDate,
    required this.endDate,
  });
}

class CancelSaleParams {
  final int saleId;
  final String reason;

  CancelSaleParams({
    required this.saleId,
    required this.reason,
  });
}

class RefundPaymentParams {
  final int paymentId;
  final String reason;

  RefundPaymentParams({
    required this.paymentId,
    required this.reason,
  });
}

class EmailReceiptParams {
  final int receiptId;
  final String email;

  EmailReceiptParams({
    required this.receiptId,
    required this.email,
  });
}

