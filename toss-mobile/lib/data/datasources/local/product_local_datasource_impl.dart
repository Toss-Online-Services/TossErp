import 'package:sqflite/sqflite.dart';

import '../../../app/database/app_database.dart';
import '../../models/product_model.dart';
import '../interfaces/product_datasource.dart';

class ProductLocalDatasourceImpl extends ProductDatasource {
  final AppDatabase _appDatabase;

  ProductLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createProduct(ProductModel product) async {
    await _appDatabase.database.insert(
      AppDatabaseConfig.productTableName,
      product.toJson(),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );

    // The id has been generated in models
    return product.id;
  }

  @override
  Future<void> updateProduct(ProductModel product) async {
    await _appDatabase.database.update(
      AppDatabaseConfig.productTableName,
      product.toJson(),
      where: 'id = ?',
      whereArgs: [product.id],
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
  }

  @override
  Future<void> deleteProduct(int id) async {
    await _appDatabase.database.delete(
      AppDatabaseConfig.productTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<ProductModel?> getProduct(int id) async {
    var res = await _appDatabase.database.query(
      AppDatabaseConfig.productTableName,
      where: 'id = ?',
      whereArgs: [id],
    );

    if (res.isEmpty) return null;

    return ProductModel.fromJson(res.first);
  }

  @override
  Future<List<ProductModel>> getAllUserProducts(String userId) async {
    var res = await _appDatabase.database.query(
      AppDatabaseConfig.productTableName,
      where: 'createdById = ?',
      whereArgs: [userId],
    );

    return res.map((e) => ProductModel.fromJson(e)).toList();
  }

  @override
  Future<List<ProductModel>> getUserProducts(
    String userId, {
    String orderBy = 'createdAt',
    String sortBy = 'DESC',
    int limit = 10,
    int? offset,
    String? contains,
  }) async {
    String where = 'createdById = ?';
    List<Object?> args = [userId];
    if (contains != null && contains.trim().isNotEmpty) {
      final term = "%${contains.trim()}%";
      where +=
          ' AND (name LIKE ? OR description LIKE ? OR CAST(id AS TEXT) LIKE ? OR CAST(price AS TEXT) LIKE ? OR CAST(stock AS TEXT) LIKE ?)';
      args.addAll([term, term, term, term, term]);
    }

    var res = await _appDatabase.database.query(
      AppDatabaseConfig.productTableName,
      where: where,
      whereArgs: args,
      orderBy: '$orderBy $sortBy',
      limit: limit,
      offset: offset,
    );

    return res.map((e) => ProductModel.fromJson(e)).toList();
  }
    /// Seeds the local database with realistic spaza shop sample products for testing
    Future<void> seedSampleProducts() async {
      final sampleProducts = [
        ProductModel(
          id: 1001,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Albany Brown Bread 700g',
          imageUrl: 'https://images.unsplash.com/photo-1542834369-f10ebf06d3cb',
          stock: 40,
          sold: 0,
          price: 1999,
          description: 'Daily fresh bread loaf',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 1002,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Maize Meal 10kg',
          imageUrl: 'https://images.unsplash.com/photo-1615486364033-8f2d7b8a9f2e',
          stock: 25,
          sold: 0,
          price: 12999,
          description: 'Staple maize meal bag',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 1003,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Sunfoil Cooking Oil 2L',
          imageUrl: 'https://images.unsplash.com/photo-1505577058444-a3dab90d4253',
          stock: 20,
          sold: 0,
          price: 7499,
          description: 'Popular sunflower oil',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 1004,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Coca-Cola 2.25L',
          imageUrl: 'https://images.unsplash.com/photo-1589391886645-d51941baf7fb',
          stock: 36,
          sold: 0,
          price: 2499,
          description: 'Cold drink 2.25L',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 1005,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Airtime Voucher R10',
          imageUrl: 'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9',
          stock: 9999,
          sold: 0,
          price: 1000,
          description: 'Prepaid airtime PIN (demo)',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 1006,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Paraffin 1L',
          imageUrl: 'https://images.unsplash.com/photo-1600959907703-28ffe1108f7f',
          stock: 18,
          sold: 0,
          price: 2999,
          description: 'Lighting paraffin 1L',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
      ];
      for (final product in sampleProducts) {
        await createProduct(product);
      }
    }
}
