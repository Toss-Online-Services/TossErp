import 'package:sqflite/sqflite.dart';

import '../../../app/database/app_database.dart';
import '../../models/payment_model.dart';
import '../interfaces/payment_datasource.dart';

class PaymentLocalDatasourceImpl extends PaymentDatasource {
  final AppDatabase _appDatabase;

  PaymentLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createPayment(PaymentModel payment) async {
    await _appDatabase.database.insert(
      AppDatabaseConfig.paymentTableName,
      payment.toJson(),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
    return payment.id;
  }

  @override
  Future<void> deletePayment(int id) async {
    await _appDatabase.database.delete(
      AppDatabaseConfig.paymentTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<List<PaymentModel>> getTransactionPayments(int transactionId) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.paymentTableName,
      where: 'transactionId = ?',
      whereArgs: [transactionId],
      orderBy: 'createdAt ASC',
    );
    return res.map((e) => PaymentModel.fromJson(e)).toList();
  }
}


