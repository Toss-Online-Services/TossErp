import 'package:sqflite/sqflite.dart';

import '../../../app/database/app_database.dart';
import '../../models/queued_action_model.dart';
import '../interfaces/queued_action_datasource.dart';

class QueuedActionLocalDatasourceImpl extends QueuedActionDatasource {
  final AppDatabase _appDatabase;

  QueuedActionLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createQueuedAction(QueuedActionModel queue) async {
    await _appDatabase.database.insert(
      AppDatabaseConfig.queuedActionTableName,
      queue.toJson(),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );

    // The id has been generated in models
    return queue.id;
  }

  @override
  Future<void> deleteQueuedAction(int id) async {
    await _appDatabase.database.delete(
      AppDatabaseConfig.queuedActionTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<QueuedActionModel?> getQueuedAction(int id) async {
    var res = await _appDatabase.database.query(
      AppDatabaseConfig.queuedActionTableName,
      where: 'id = ?',
      whereArgs: [id],
    );

    if (res.isEmpty) return null;

    return QueuedActionModel.fromJson(res.first);
  }

  @override
  Future<List<QueuedActionModel>> getAllUserQueuedAction() async {
    var res = await _appDatabase.database.query(
      AppDatabaseConfig.queuedActionTableName,
    );

    return res.map((e) => QueuedActionModel.fromJson(e)).toList();
  }
    /// Seeds the local database with a realistic sample queued action for testing
    Future<void> seedSampleQueuedActions() async {
      final sampleQueuedAction = QueuedActionModel(
        id: 9001,
        repository: 'ProductRepositoryImpl',
        method: 'createProduct',
        param: '{"id":1003,"createdById":"e51VrUAK7WdXpa75V641428qX0u2","name":"USB-C Hub","imageUrl":"https://images.unsplash.com/photo-1506744038136-46273834b3fb","stock":20,"sold":0,"price":4999,"description":"Multiport USB-C hub for laptops.","createdAt":"2025-09-08T10:15:00Z","updatedAt":"2025-09-08T10:15:00Z"}',
        isCritical: true,
        createdAt: '2025-09-08T10:15:00Z',
      );
      await createQueuedAction(sampleQueuedAction);
    }
}
