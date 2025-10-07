import 'package:sqflite/sqflite.dart';
import '../../../app/database/app_database.dart';
import '../../models/ordered_product_model.dart';
import '../../models/product_model.dart';
import '../../models/transaction_model.dart';
import '../../models/user_model.dart';
import '../interfaces/transaction_datasource.dart';

class TransactionLocalDatasourceImpl extends TransactionDatasource {
  final AppDatabase _appDatabase;

  TransactionLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createTransaction(TransactionModel transaction) async {
    return await _appDatabase.database.transaction((trx) async {
      await trx.insert(
        AppDatabaseConfig.transactionTableName,
        transaction.toJson()
          ..remove('orderedProducts')
          ..remove('createdBy'),
        conflictAlgorithm: ConflictAlgorithm.replace,
      );
      if (transaction.orderedProducts?.isNotEmpty ?? false) {
        var batch = trx.batch();
        final bool isReturnTx = (transaction.description ?? '').contains('return=true') || (transaction.totalAmount < 0);
        for (var orderedProduct in transaction.orderedProducts!) {
          orderedProduct.transactionId = transaction.id;
          batch.insert(
            AppDatabaseConfig.orderedProductTableName,
            orderedProduct.toJson(),
            conflictAlgorithm: ConflictAlgorithm.replace,
          );
          var rawProduct = await trx.query(
            AppDatabaseConfig.productTableName,
            where: 'id = ?',
            whereArgs: [orderedProduct.productId],
          );
          if (rawProduct.isEmpty) continue;
          var product = ProductModel.fromJson(rawProduct.first);
          int stock = isReturnTx ? product.stock + orderedProduct.quantity : product.stock - orderedProduct.quantity;
          int sold = isReturnTx ? product.sold - orderedProduct.quantity : product.sold + orderedProduct.quantity;
          batch.update(
            AppDatabaseConfig.productTableName,
            {'stock': stock, 'sold': sold},
            where: 'id = ?',
            whereArgs: [product.id],
            conflictAlgorithm: ConflictAlgorithm.replace,
          );
        }
        await batch.commit(noResult: true);
      }
      return transaction.id;
    });
  }

  @override
  Future<void> updateTransaction(TransactionModel transaction) async {
    return await _appDatabase.database.transaction((trx) async {
      await trx.update(
        AppDatabaseConfig.transactionTableName,
        transaction.toJson()
          ..remove('orderedProducts')
          ..remove('createdBy'),
        where: 'id = ?',
        whereArgs: [transaction.id],
        conflictAlgorithm: ConflictAlgorithm.replace,
      );
      if (transaction.orderedProducts?.isNotEmpty ?? false) {
        var batch = trx.batch();
        for (var orderedProduct in transaction.orderedProducts!) {
          batch.update(
            AppDatabaseConfig.orderedProductTableName,
            orderedProduct.toJson(),
            where: 'id = ?',
            whereArgs: [orderedProduct.id],
          );
          var rawProduct = await trx.query(
            AppDatabaseConfig.productTableName,
            where: 'id = ?',
            whereArgs: [orderedProduct.productId],
          );
          if (rawProduct.isEmpty) continue;
          var product = ProductModel.fromJson(rawProduct.first);
          int stock = product.stock - orderedProduct.quantity;
          int sold = product.sold + orderedProduct.quantity;
          batch.update(
            AppDatabaseConfig.productTableName,
            {'stock': stock, 'sold': sold},
            where: 'id = ?',
            whereArgs: [product.id],
            conflictAlgorithm: ConflictAlgorithm.replace,
          );
        }
        await batch.commit(noResult: true);
      }
    });
  }

  @override
  Future<void> deleteTransaction(int id) async {
    await _appDatabase.database.transaction((trx) async {
      var orderedProducts = await trx.query(
        AppDatabaseConfig.orderedProductTableName,
        where: 'transactionId = ?',
        whereArgs: [id],
      );
      for (var orderedProductMap in orderedProducts) {
        var orderedProduct = OrderedProductModel.fromJson(orderedProductMap);
        var productResults = await trx.query(
          AppDatabaseConfig.productTableName,
          where: 'id = ?',
          whereArgs: [orderedProduct.productId],
        );
        if (productResults.isNotEmpty) {
          var product = ProductModel.fromJson(productResults.first);
          int revertedStock = product.stock + orderedProduct.quantity;
          int revertedSold = product.sold - orderedProduct.quantity;
          await trx.update(
            AppDatabaseConfig.productTableName,
            {'stock': revertedStock, 'sold': revertedSold},
            where: 'id = ?',
            whereArgs: [orderedProduct.productId],
          );
        }
      }
      await trx.delete(
        AppDatabaseConfig.orderedProductTableName,
        where: 'transactionId = ?',
        whereArgs: [id],
      );
      await trx.delete(
        AppDatabaseConfig.transactionTableName,
        where: 'id = ?',
        whereArgs: [id],
      );
    });
  }

  @override
  Future<TransactionModel?> getTransaction(int id) async {
    return await _appDatabase.database.transaction((trx) async {
      var rawTransactions = await trx.query(
        AppDatabaseConfig.transactionTableName,
        where: 'id = ?',
        whereArgs: [id],
      );
      if (rawTransactions.isEmpty) {
        return null;
      }
      var transaction = TransactionModel.fromJson(rawTransactions.first);
      var rawOrderedProducts = await trx.query(
        AppDatabaseConfig.orderedProductTableName,
        where: 'transactionId = ?',
        whereArgs: [id],
      );
      var orderedProducts = rawOrderedProducts.map((e) => OrderedProductModel.fromJson(e)).toList();
      transaction.orderedProducts = orderedProducts;
      var rawCreatedBy = await trx.query(
        AppDatabaseConfig.userTableName,
        where: 'id = ?',
        whereArgs: [transaction.createdById],
      );
      if (rawCreatedBy.isNotEmpty) {
        transaction.createdBy = UserModel.fromJson(rawCreatedBy.first);
      }
      return transaction;
    });
  }

  @override
  Future<List<TransactionModel>> getAllUserTransactions(String userId) async {
    return await _appDatabase.database.transaction((trx) async {
      var rawTransactions = await trx.query(
        AppDatabaseConfig.transactionTableName,
        where: 'createdById = ?',
        whereArgs: [userId],
        orderBy: 'createdAt DESC',
      );
      var transactions = rawTransactions.map((e) => TransactionModel.fromJson(e)).toList();
      for (var transaction in transactions) {
        var rawOrderedProducts = await trx.query(
          AppDatabaseConfig.orderedProductTableName,
          where: 'transactionId = ?',
          whereArgs: [transaction.id],
        );
        var orderedProducts = rawOrderedProducts.map((e) => OrderedProductModel.fromJson(e)).toList();
        transaction.orderedProducts = orderedProducts;
        var rawCreatedBy = await trx.query(
          AppDatabaseConfig.userTableName,
          where: 'id = ?',
          whereArgs: [transaction.createdById],
        );
        if (rawCreatedBy.isNotEmpty) {
          transaction.createdBy = UserModel.fromJson(rawCreatedBy.first);
        }
      }
      return transactions;
    });
  }

  @override
  Future<List<TransactionModel>> getUserTransactions(
    String userId, {
    String orderBy = 'createdAt',
    String sortBy = 'DESC',
    int limit = 10,
    int? offset,
    String? contains,
  }) async {
    return await _appDatabase.database.transaction((trx) async {
      var rawTransactions = await trx.query(
        AppDatabaseConfig.transactionTableName,
        where: 'createdById = ? AND id LIKE ?',
        whereArgs: [userId, "%${contains ?? ''}%"],
        orderBy: '$orderBy $sortBy',
        limit: limit,
        offset: offset,
      );
      var transactions = rawTransactions.map((e) => TransactionModel.fromJson(e)).toList();
      for (var transaction in transactions) {
        var rawOrderedProducts = await trx.query(
          AppDatabaseConfig.orderedProductTableName,
          where: 'transactionId = ?',
          whereArgs: [transaction.id],
        );
        var orderedProducts = rawOrderedProducts.map((e) => OrderedProductModel.fromJson(e)).toList();
        transaction.orderedProducts = orderedProducts;
        var rawCreatedBy = await trx.query(
          AppDatabaseConfig.userTableName,
          where: 'id = ?',
          whereArgs: [transaction.createdById],
        );
        if (rawCreatedBy.isNotEmpty) {
          transaction.createdBy = UserModel.fromJson(rawCreatedBy.first);
        }
      }
      return transactions;
    });
  }

  /// Seeds the local database with a realistic sample transaction for testing
  Future<void> seedSampleTransactions() async {
    final sampleTransaction = TransactionModel(
      id: 5001,
      paymentMethod: 'Credit Card',
      customerName: 'Jane Smith',
      description: 'Online purchase',
      createdById: 'e51VrUAK7WdXpa75V641428qX0u2',
      createdBy: UserModel(
        id: 'e51VrUAK7WdXpa75V641428qX0u2',
        name: 'John Doe',
      ),
      orderedProducts: [
        OrderedProductModel(
          id: 1,
          transactionId: 5001,
          productId: 1001,
          name: 'Wireless Mouse',
          quantity: 2,
          stock: 50,
          imageUrl: 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8',
          price: 2999,
        ),
        OrderedProductModel(
          id: 2,
          transactionId: 5001,
          productId: 1002,
          name: 'Mechanical Keyboard',
          quantity: 1,
          stock: 30,
          imageUrl: 'https://images.unsplash.com/photo-1519389950473-47ba0277781c',
          price: 7999,
        ),
      ],
      receivedAmount: 13997,
      returnAmount: 0,
      totalAmount: 13997,
      totalOrderedProduct: 3,
      createdAt: '2025-09-08T10:10:00Z',
      updatedAt: '2025-09-08T10:10:00Z',
    );
    await createTransaction(sampleTransaction);
  }
}
