import '../../core/usecase/usecase.dart';
import '../entities/discount_entity.dart';

abstract class DiscountRepository {
  Future<Result<int>> createDiscount(DiscountEntity discount);
  Future<Result<void>> deleteDiscount(int id);
  Future<Result<List<DiscountEntity>>> getTransactionDiscounts(int transactionId);
  Future<Result<List<DiscountEntity>>> getOrderedProductDiscounts(int orderedProductId);
}


