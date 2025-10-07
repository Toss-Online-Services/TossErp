import 'package:sqflite/sqflite.dart';

import '../../../app/database/app_database.dart';
import '../../models/discount_model.dart';
import '../interfaces/discount_datasource.dart';

class DiscountLocalDatasourceImpl extends DiscountDatasource {
  final AppDatabase _appDatabase;

  DiscountLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createDiscount(DiscountModel discount) async {
    await _appDatabase.database.insert(
      AppDatabaseConfig.discountTableName,
      discount.toJson(),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
    return discount.id;
  }

  @override
  Future<void> deleteDiscount(int id) async {
    await _appDatabase.database.delete(
      AppDatabaseConfig.discountTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<List<DiscountModel>> getTransactionDiscounts(int transactionId) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.discountTableName,
      where: 'transactionId = ?',
      whereArgs: [transactionId],
      orderBy: 'createdAt ASC',
    );
    return res.map((e) => DiscountModel.fromJson(e)).toList();
  }

  @override
  Future<List<DiscountModel>> getOrderedProductDiscounts(int orderedProductId) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.discountTableName,
      where: 'orderedProductId = ?',
      whereArgs: [orderedProductId],
      orderBy: 'createdAt ASC',
    );
    return res.map((e) => DiscountModel.fromJson(e)).toList();
  }
}


