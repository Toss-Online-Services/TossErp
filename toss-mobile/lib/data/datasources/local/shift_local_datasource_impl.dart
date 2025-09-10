import 'package:sqflite/sqflite.dart';

import '../../../app/database/app_database.dart';
import '../../models/shift_model.dart';
import '../interfaces/shift_datasource.dart';

class ShiftLocalDatasourceImpl extends ShiftDatasource {
  final AppDatabase _appDatabase;

  ShiftLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createShift(ShiftModel shift) async {
    await _appDatabase.database.insert(
      AppDatabaseConfig.shiftTableName,
      shift.toJson(),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
    return shift.id;
  }

  @override
  Future<void> updateShift(ShiftModel shift) async {
    await _appDatabase.database.update(
      AppDatabaseConfig.shiftTableName,
      shift.toJson(),
      where: 'id = ?',
      whereArgs: [shift.id],
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
  }

  @override
  Future<ShiftModel?> getOpenShift(String userId) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.shiftTableName,
      where: 'userId = ? AND status = ?',
      whereArgs: [userId, 'open'],
      orderBy: 'startedAt DESC',
      limit: 1,
    );
    if (res.isEmpty) return null;
    return ShiftModel.fromJson(res.first);
  }

  @override
  Future<ShiftModel?> getShift(int id) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.shiftTableName,
      where: 'id = ?',
      whereArgs: [id],
      limit: 1,
    );
    if (res.isEmpty) return null;
    return ShiftModel.fromJson(res.first);
  }
}


