import 'package:sqflite/sqflite.dart';

import '../../../app/database/app_database.dart';
import '../../models/appointment_model.dart';
import '../interfaces/appointment_datasource.dart';

class AppointmentLocalDatasourceImpl extends AppointmentDatasource {
  final AppDatabase _appDatabase;

  AppointmentLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createAppointment(AppointmentModel appointment) async {
    await _appDatabase.database.insert(
      AppDatabaseConfig.appointmentTableName,
      appointment.toJson(),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
    return appointment.id;
  }

  @override
  Future<void> updateAppointment(AppointmentModel appointment) async {
    await _appDatabase.database.update(
      AppDatabaseConfig.appointmentTableName,
      appointment.toJson(),
      where: 'id = ?',
      whereArgs: [appointment.id],
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
  }

  @override
  Future<List<AppointmentModel>> getAppointmentsByDate(String dateIsoPrefix) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.appointmentTableName,
      where: 'scheduledAt LIKE ?',
      whereArgs: ['$dateIsoPrefix%'],
      orderBy: 'scheduledAt ASC',
    );
    return res.map((e) => AppointmentModel.fromJson(e)).toList();
  }

  @override
  Future<void> linkAppointmentToTransaction(int appointmentId, int transactionId) async {
    await _appDatabase.database.update(
      AppDatabaseConfig.appointmentTableName,
      {'linkedTransactionId': transactionId, 'status': 'completed'},
      where: 'id = ?',
      whereArgs: [appointmentId],
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
  }
}


