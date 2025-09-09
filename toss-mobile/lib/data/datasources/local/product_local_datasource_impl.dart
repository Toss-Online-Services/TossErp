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
    var res = await _appDatabase.database.query(
      AppDatabaseConfig.productTableName,
      where: 'createdById = ? AND name LIKE ?',
      whereArgs: [userId, "%${contains ?? ''}%"],
      orderBy: '$orderBy $sortBy',
      limit: limit,
      offset: offset,
    );

    return res.map((e) => ProductModel.fromJson(e)).toList();
  }
    /// Seeds the local database with realistic sample products for testing
    Future<void> seedSampleProducts() async {
      final sampleProducts = [
        ProductModel(
          id: 1001,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Wireless Mouse',
          imageUrl: 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8',
          stock: 50,
          sold: 10,
          price: 2999,
          description: 'Ergonomic wireless mouse with adjustable DPI.',
          createdAt: '2025-09-08T10:05:00Z',
          updatedAt: '2025-09-08T10:05:00Z',
        ),
        ProductModel(
          id: 1002,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Mechanical Keyboard',
          imageUrl: 'https://images.unsplash.com/photo-1519389950473-47ba0277781c',
          stock: 30,
          sold: 5,
          price: 7999,
          description: 'RGB backlit mechanical keyboard, blue switches.',
          createdAt: '2025-09-08T10:06:00Z',
          updatedAt: '2025-09-08T10:06:00Z',
        ),
      ];
      for (final product in sampleProducts) {
        await createProduct(product);
      }
    }
}
