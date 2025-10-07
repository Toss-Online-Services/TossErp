import 'package:flutter/foundation.dart';
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

    if (kDebugMode) {
      debugPrint('💾 ProductLocalDatasource: Querying for userId: $userId');
      debugPrint('💾 ProductLocalDatasource: WHERE clause: $where');
      debugPrint('💾 ProductLocalDatasource: Args: $args');
    }

    var res = await _appDatabase.database.query(
      AppDatabaseConfig.productTableName,
      where: where,
      whereArgs: args,
      orderBy: '$orderBy $sortBy',
      limit: limit,
      offset: offset,
    );

    if (kDebugMode) {
      debugPrint('💾 ProductLocalDatasource: Found ${res.length} products in database');
      if (res.isNotEmpty) {
        debugPrint('💾 First product from DB: ${res.first}');
      }
    }

    return res.map((e) => ProductModel.fromJson(e)).toList();
  }
    /// Seeds the local database with realistic spaza shop sample products for testing
    Future<void> seedSampleProducts() async {
      final sampleProducts = [
        // Food Category (categoryId: 1)
        ProductModel(
          id: 1001,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Albany Brown Bread 700g',
          imageUrl: 'https://images.unsplash.com/photo-1542834369-f10ebf06d3cb?w=400',
          stock: 40,
          sold: 0,
          price: 1999,
          categoryId: 1,
          description: 'Daily fresh bread loaf',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 1002,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Maize Meal 10kg',
          imageUrl: 'https://images.unsplash.com/photo-1615486364033-8f2d7b8a9f2e?w=400',
          stock: 25,
          sold: 0,
          price: 12999,
          categoryId: 1,
          description: 'Staple maize meal bag',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 1003,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Rice 2kg',
          imageUrl: 'https://images.unsplash.com/photo-1536304993881-ff6e9eefa2a6?w=400',
          stock: 30,
          sold: 0,
          price: 4599,
          categoryId: 1,
          description: 'Premium white rice',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 1004,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Eggs (30 pack)',
          imageUrl: 'https://images.unsplash.com/photo-1518569656558-1f25e69d93d7?w=400',
          stock: 15,
          sold: 0,
          price: 6999,
          categoryId: 1,
          description: 'Fresh large eggs',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),

        // Beverages Category (categoryId: 2)
        ProductModel(
          id: 2001,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Coca-Cola 2.25L',
          imageUrl: 'https://images.unsplash.com/photo-1589391886645-d51941baf7fb?w=400',
          stock: 36,
          sold: 0,
          price: 2499,
          categoryId: 2,
          description: 'Cold drink 2.25L',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 2002,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Fanta Orange 2L',
          imageUrl: 'https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?w=400',
          stock: 28,
          sold: 0,
          price: 2299,
          categoryId: 2,
          description: 'Orange flavored soda',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 2003,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Milk 1L',
          imageUrl: 'https://images.unsplash.com/photo-1563636619-e9143da7973b?w=400',
          stock: 22,
          sold: 0,
          price: 1899,
          categoryId: 2,
          description: 'Fresh whole milk',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 2004,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Mageu 1L',
          imageUrl: 'https://images.unsplash.com/photo-1566740933430-b5e70b06d2d5?w=400',
          stock: 20,
          sold: 0,
          price: 1599,
          categoryId: 2,
          description: 'Traditional fermented drink',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),

        // Household Category (categoryId: 3)
        ProductModel(
          id: 3001,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Sunfoil Cooking Oil 2L',
          imageUrl: 'https://images.unsplash.com/photo-1505577058444-a3dab90d4253?w=400',
          stock: 20,
          sold: 0,
          price: 7499,
          categoryId: 3,
          description: 'Popular sunflower oil',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 3002,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Paraffin 1L',
          imageUrl: 'https://images.unsplash.com/photo-1600959907703-28ffe1108f7f?w=400',
          stock: 18,
          sold: 0,
          price: 2999,
          categoryId: 3,
          description: 'Lighting paraffin 1L',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 3003,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Candles (Pack of 6)',
          imageUrl: 'https://images.unsplash.com/photo-1543883658-3efdbf9fb6dd?w=400',
          stock: 50,
          sold: 0,
          price: 899,
          categoryId: 3,
          description: 'Emergency candles',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 3004,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Soap Bar',
          imageUrl: 'https://images.unsplash.com/photo-1585128903675-62ba048be788?w=400',
          stock: 35,
          sold: 0,
          price: 599,
          categoryId: 3,
          description: 'Personal hygiene soap',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),

        // Airtime Category (categoryId: 4)
        ProductModel(
          id: 4001,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Airtime Voucher R10',
          imageUrl: 'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400',
          stock: 9999,
          sold: 0,
          price: 1000,
          categoryId: 4,
          description: 'Prepaid airtime R10',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 4002,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Airtime Voucher R20',
          imageUrl: 'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400',
          stock: 9999,
          sold: 0,
          price: 2000,
          categoryId: 4,
          description: 'Prepaid airtime R20',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 4003,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Airtime Voucher R50',
          imageUrl: 'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=400',
          stock: 9999,
          sold: 0,
          price: 5000,
          categoryId: 4,
          description: 'Prepaid airtime R50',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
        ProductModel(
          id: 4004,
          createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
          name: 'Data Bundle 1GB',
          imageUrl: 'https://images.unsplash.com/photo-1593642532973-d31b6557fa68?w=400',
          stock: 9999,
          sold: 0,
          price: 8999,
          categoryId: 4,
          description: 'Mobile data 1GB bundle',
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        ),
      ];

      if (kDebugMode) {
        debugPrint('🌱 Starting to seed ${sampleProducts.length} sample products...');
      }

      // Clear existing sample products first to avoid duplicates
      try {
        final existingProducts = await getAllUserProducts('e51VrUAK7WdXpa75V641428qX0u2');
        if (existingProducts.isNotEmpty) {
          if (kDebugMode) {
            debugPrint('🗑️ Clearing ${existingProducts.length} existing products');
          }
          for (final product in existingProducts) {
            await deleteProduct(product.id);
          }
        }
      } catch (e) {
        if (kDebugMode) {
          debugPrint('⚠️ Error clearing existing products: $e');
        }
      }

      // Add the sample products
      int seeded = 0;
      for (final product in sampleProducts) {
        try {
          await createProduct(product);
          seeded++;
          if (kDebugMode) {
            debugPrint('✅ Seeded product: ${product.name} (Category: ${product.categoryId})');
          }
        } catch (e) {
          if (kDebugMode) {
            debugPrint('❌ Failed to seed product ${product.name}: $e');
          }
        }
      }

      if (kDebugMode) {
        debugPrint('🌱 Successfully seeded $seeded out of ${sampleProducts.length} sample products');
      }
    }
}
