import 'package:flutter/foundation.dart';

import '../../../app/services/auth/auth_service.dart';
import '../../../domain/entities/product_entity.dart';
import '../../../domain/repositories/product_repository.dart';
import '../../../domain/usecases/params/base_params.dart';
import '../../../domain/usecases/product_usecases.dart';

class ProductsProvider extends ChangeNotifier {
  final ProductRepository productRepository;

  ProductsProvider({required this.productRepository});

  List<ProductEntity>? allProducts;

  // Low stock detection state
  final int lowStockThreshold = 5; // default threshold; could be made configurable
  List<ProductEntity> lowStockProducts = [];

  bool isLoadingMore = false;

  Future<void> getAllProducts({int? offset, String? contains}) async {
    if (offset != null) {
      isLoadingMore = true;
      notifyListeners();
    }

    var params = BaseParams(
      param: AuthService().getAuthData()!.uid,
      offset: offset,
      contains: contains,
    );

    var res = await GetUserProductsUsecase(productRepository).call(params);

    if (res.isSuccess) {
      if (offset == null) {
        allProducts = res.data ?? [];
      } else {
        allProducts?.addAll(res.data ?? []);
      }

      // Update low stock list on refresh loads
      if (offset == null) {
        _computeLowStock();
      }

      isLoadingMore = false;
      notifyListeners();
    } else {
      throw res.error?.message ?? 'Failed to load data';
    }
  }

  void _computeLowStock() {
    final list = allProducts ?? [];
    lowStockProducts = list.where((p) => p.stock <= lowStockThreshold).toList();
  }
}
