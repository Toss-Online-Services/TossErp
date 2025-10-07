import 'package:sqflite/sqflite.dart';

import '../../../app/database/app_database.dart';
import '../../models/cash_movement_model.dart';
import '../interfaces/cash_movement_datasource.dart';

class CashMovementLocalDatasourceImpl extends CashMovementDatasource {
  final AppDatabase _appDatabase;

  CashMovementLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createCashMovement(CashMovementModel movement) async {
    await _appDatabase.database.insert(
      AppDatabaseConfig.cashMovementTableName,
      movement.toJson(),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
    return movement.id;
  }

  @override
  Future<List<CashMovementModel>> getShiftMovements(int shiftId) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.cashMovementTableName,
      where: 'shiftId = ?',
      whereArgs: [shiftId],
      orderBy: 'createdAt ASC',
    );
    return res.map((e) => CashMovementModel.fromJson(e)).toList();
  }
}


