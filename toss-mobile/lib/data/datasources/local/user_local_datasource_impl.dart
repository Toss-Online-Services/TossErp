import 'package:sqflite/sqflite.dart';

import '../../../app/database/app_database.dart';
import '../../models/user_model.dart';
import '../interfaces/user_datasource.dart';

class UserLocalDatasourceImpl extends UserDatasource {
  final AppDatabase _appDatabase;

  UserLocalDatasourceImpl(this._appDatabase);

  @override
  Future<String> createUser(UserModel user) async {
    await _appDatabase.database.insert(
      AppDatabaseConfig.userTableName,
      user.toJson(),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );

    // The id is uid from GoogleSignIn credential
    return user.id;
  }

  @override
  Future<void> updateUser(UserModel user) async {
    await _appDatabase.database.update(
      AppDatabaseConfig.userTableName,
      user.toJson(),
      where: 'id = ?',
      whereArgs: [user.id],
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
  }

  @override
  Future<void> deleteUser(String id) async {
    await _appDatabase.database.delete(
      AppDatabaseConfig.userTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<UserModel?> getUser(String id) async {
    var res = await _appDatabase.database.query(
      AppDatabaseConfig.userTableName,
      where: 'id = ?',
      whereArgs: [id],
    );

    if (res.isEmpty) return null;

    return UserModel.fromJson(res.first);
  }
    /// Seeds the local database with a realistic sample user for testing
    Future<void> seedSampleUser() async {
      final sampleUser = UserModel(
        id: 'e51VrUAK7WdXpa75V641428qX0u2',
        email: 'john.doe@example.com',
        phone: '+1234567890',
        name: 'John Doe',
        gender: 'male',
        birthdate: '1990-01-01',
        imageUrl: 'https://randomuser.me/api/portraits/men/1.jpg',
        createdAt: '2025-09-08T10:00:00Z',
        updatedAt: '2025-09-08T10:00:00Z',
      );
      await createUser(sampleUser);
    }
}
