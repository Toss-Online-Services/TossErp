import '../../core/usecase/usecase.dart';
import '../entities/receipt_entity.dart';

abstract class ReceiptRepository {
  Future<Result<List<ReceiptEntity>>> getAllReceipts();
  Future<Result<ReceiptEntity?>> getReceiptById(int id);
  Future<Result<ReceiptEntity?>> getReceiptByNumber(String receiptNumber);
  Future<Result<ReceiptEntity?>> getReceiptBySale(int saleId);
  Future<Result<List<ReceiptEntity>>> getReceiptsByDateRange(DateTime startDate, DateTime endDate);
  Future<Result<int>> createReceipt(ReceiptEntity receipt);
  Future<Result<void>> updateReceipt(ReceiptEntity receipt);
  Future<Result<void>> deleteReceipt(int id);
  Future<Result<void>> markReceiptAsPrinted(int id);
  Future<Result<void>> markReceiptAsEmailed(int id);
  Future<Result<String>> generateReceiptPdf(int id);
  Future<Result<void>> emailReceipt(int id, String email);
}

