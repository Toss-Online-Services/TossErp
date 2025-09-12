import '../../domain/entities/receipt_entity.dart';

abstract class ReceiptRepository {
  Future<List<ReceiptEntity>> getAllReceipts();
  Future<ReceiptEntity?> getReceiptById(String id);
  Future<String> createReceipt(ReceiptEntity receipt);
  Future<void> updateReceipt(ReceiptEntity receipt);
  Future<void> deleteReceipt(String id);
  Future<List<ReceiptEntity>> getReceiptsByDateRange(DateTime start, DateTime end);
  Future<List<ReceiptEntity>> getReceiptsByType(ReceiptType type);
}
