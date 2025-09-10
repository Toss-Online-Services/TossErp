import '../../../app/database/app_database.dart';
import '../../models/z_report_model.dart';
import '../interfaces/z_report_datasource.dart';

class ZReportLocalDatasourceImpl extends ZReportDatasource {
  final AppDatabase _appDatabase;

  ZReportLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createZReport(ZReportModel report) async {
    await _appDatabase.database.insert(
      AppDatabaseConfig.zReportTableName,
      report.toJson(),
    );
    return report.id;
  }

  @override
  Future<ZReportModel?> getZReport(int id) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.zReportTableName,
      where: 'id = ?',
      whereArgs: [id],
      limit: 1,
    );
    if (res.isEmpty) return null;
    return ZReportModel.fromJson(res.first);
  }

  @override
  Future<ZReportModel?> getShiftZReport(int shiftId) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.zReportTableName,
      where: 'shiftId = ?',
      whereArgs: [shiftId],
      limit: 1,
    );
    if (res.isEmpty) return null;
    return ZReportModel.fromJson(res.first);
  }
}


