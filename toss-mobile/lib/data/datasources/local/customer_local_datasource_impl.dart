import 'package:sqflite/sqflite.dart';

import '../../../app/database/app_database.dart';
import '../../models/customer_model.dart';
import '../interfaces/customer_datasource.dart';

class CustomerLocalDatasourceImpl extends CustomerDatasource {
  final AppDatabase _appDatabase;
  CustomerLocalDatasourceImpl(this._appDatabase);

  @override
  Future<CustomerModel?> getCustomer(String idOrPhone) async {
    final db = _appDatabase.database;
    final res = await db.query(
      AppDatabaseConfig.customerTableName,
      where: 'id = ? OR phone = ?',
      whereArgs: [idOrPhone, idOrPhone],
      limit: 1,
    );
    if (res.isEmpty) return null;
    return CustomerModel.fromJson(res.first);
  }

  @override
  Future<List<CustomerModel>> searchCustomers(String query) async {
    final db = _appDatabase.database;
    final q = '%${query.trim()}%';
    final res = await db.query(
      AppDatabaseConfig.customerTableName,
      where: 'name LIKE ? OR phone LIKE ? OR id LIKE ?',
      whereArgs: [q, q, q],
      limit: 20,
    );
    return res.map((e) => CustomerModel.fromJson(e)).toList();
  }

  @override
  Future<String> upsertCustomer(CustomerModel customer) async {
    final db = _appDatabase.database;
    await db.insert(AppDatabaseConfig.customerTableName, customer.toJson(), conflictAlgorithm: ConflictAlgorithm.replace);
    return customer.id;
  }

  @override
  Future<void> updatePoints(String id, int delta) async {
    final db = _appDatabase.database;
    await db.rawUpdate(
      "UPDATE '${AppDatabaseConfig.customerTableName}' SET pointsBalance = COALESCE(pointsBalance,0) + ?, updatedAt = CURRENT_TIMESTAMP WHERE id = ?",
      [delta, id],
    );
  }
}



