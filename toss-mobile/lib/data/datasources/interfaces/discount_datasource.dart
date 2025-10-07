import '../../models/discount_model.dart';

abstract class DiscountDatasource {
  Future<int> createDiscount(DiscountModel discount);

  Future<void> deleteDiscount(int id);

  Future<List<DiscountModel>> getTransactionDiscounts(int transactionId);

  Future<List<DiscountModel>> getOrderedProductDiscounts(int orderedProductId);
}


