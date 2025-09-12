import '../../core/errors/errors.dart';
import '../../core/usecase/usecase.dart';
import '../../domain/entities/discount_entity.dart';
import '../../domain/repositories/discount_repository.dart';
import '../datasources/local/discount_local_datasource_impl.dart';
import '../models/discount_model.dart';

class DiscountRepositoryImpl extends DiscountRepository {
  final DiscountLocalDatasourceImpl discountLocalDatasource;

  DiscountRepositoryImpl({required this.discountLocalDatasource});

  // Helper method to convert DiscountModel to DiscountEntity
  DiscountEntity _modelToEntity(DiscountModel model) {
    return DiscountEntity(
      id: model.id.toString(),
      name: model.code ?? 'Discount',
      description: model.reason ?? 'Applied discount',
      type: _parseDiscountType(model.type),
      scope: _parseDiscountScope(model.scope),
      application: DiscountApplication.manual,
      value: model.value.toDouble(),
      startDate: DateTime.parse(model.createdAt ?? DateTime.now().toIso8601String()),
      endDate: DateTime.now().add(const Duration(days: 365)), // Default to 1 year
      createdAt: DateTime.parse(model.createdAt ?? DateTime.now().toIso8601String()),
      createdBy: 'system',
      transactionId: model.transactionId?.toString(),
      orderedProductId: model.orderedProductId?.toString(),
      couponCode: model.code,
    );
  }

  DiscountType _parseDiscountType(String type) {
    switch (type.toLowerCase()) {
      case 'percentage':
        return DiscountType.percentage;
      case 'fixed':
        return DiscountType.fixedAmount;
      default:
        return DiscountType.fixedAmount;
    }
  }

  DiscountScope _parseDiscountScope(String scope) {
    switch (scope.toLowerCase()) {
      case 'line':
        return DiscountScope.item;
      case 'cart':
        return DiscountScope.cart;
      default:
        return DiscountScope.cart;
    }
  }

  @override
  Future<Result<int>> createDiscount(DiscountEntity discount) async {
    try {
      final id = await discountLocalDatasource.createDiscount(DiscountModel(
        id: DateTime.now().millisecondsSinceEpoch, // Generate ID
        transactionId: discount.transactionId != null ? int.tryParse(discount.transactionId!) : null,
        orderedProductId: discount.orderedProductId != null ? int.tryParse(discount.orderedProductId!) : null,
        scope: discount.scope.name, // Convert enum to string
        type: discount.type.name, // Convert enum to string
        value: discount.value.toInt(), // Convert double to int
        code: discount.couponCode, // Map couponCode to code
        reason: discount.description, // Map description to reason
        createdAt: discount.createdAt.toIso8601String(),
        updatedAt: discount.updatedAt?.toIso8601String() ?? DateTime.now().toIso8601String(),
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
      return Result.success(rows.map((e) => _modelToEntity(e)).toList());
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<DiscountEntity>>> getTransactionDiscounts(int transactionId) async {
    try {
      final rows = await discountLocalDatasource.getTransactionDiscounts(transactionId);
      return Result.success(rows.map((e) => _modelToEntity(e)).toList());
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}


