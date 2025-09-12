import '../../domain/entities/discount_entity.dart';

abstract class DiscountRepository {
  Future<List<DiscountEntity>> getAllDiscounts();
  Future<DiscountEntity?> getDiscountById(String id);
  Future<String> createDiscount(DiscountEntity discount);
  Future<void> updateDiscount(DiscountEntity discount);
  Future<void> deleteDiscount(String id);
  Future<List<DiscountEntity>> getActiveDiscounts();
  Future<List<DiscountEntity>> getDiscountsByType(DiscountType type);
}
