import '../../core/errors/errors.dart';
import '../../core/usecase/usecase.dart';
import '../../domain/entities/discount_entity.dart';
import '../../domain/repositories/discount_repository.dart';
import '../datasources/local/discount_local_datasource_impl.dart';
import '../models/discount_model.dart';

class DiscountRepositoryImpl extends DiscountRepository {
  final DiscountLocalDatasourceImpl discountLocalDatasource;

  DiscountRepositoryImpl({required this.discountLocalDatasource});

  @override
  Future<Result<int>> createDiscount(DiscountEntity discount) async {
    try {
      final id = await discountLocalDatasource.createDiscount(DiscountModel(
        id: discount.id ?? DateTime.now().millisecondsSinceEpoch,
        transactionId: discount.transactionId,
        orderedProductId: discount.orderedProductId,
        scope: discount.scope,
        type: discount.type,
        value: discount.value,
        code: discount.code,
        reason: discount.reason,
        createdAt: discount.createdAt ?? DateTime.now().toIso8601String(),
        updatedAt: discount.updatedAt ?? DateTime.now().toIso8601String(),
      ));
      return Result.success(id);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> deleteDiscount(int id) async {
    try {
      await discountLocalDatasource.deleteDiscount(id);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<DiscountEntity>>> getOrderedProductDiscounts(int orderedProductId) async {
    try {
      final rows = await discountLocalDatasource.getOrderedProductDiscounts(orderedProductId);
      return Result.success(rows
          .map((e) => DiscountEntity(
                id: e.id,
                transactionId: e.transactionId,
                orderedProductId: e.orderedProductId,
                scope: e.scope,
                type: e.type,
                value: e.value,
                code: e.code,
                reason: e.reason,
                createdAt: e.createdAt,
                updatedAt: e.updatedAt,
              ))
          .toList());
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<DiscountEntity>>> getTransactionDiscounts(int transactionId) async {
    try {
      final rows = await discountLocalDatasource.getTransactionDiscounts(transactionId);
      return Result.success(rows
          .map((e) => DiscountEntity(
                id: e.id,
                transactionId: e.transactionId,
                orderedProductId: e.orderedProductId,
                scope: e.scope,
                type: e.type,
                value: e.value,
                code: e.code,
                reason: e.reason,
                createdAt: e.createdAt,
                updatedAt: e.updatedAt,
              ))
          .toList());
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}


