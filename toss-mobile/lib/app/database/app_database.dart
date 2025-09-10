import 'dart:async';
import 'dart:io';

import 'package:flutter/foundation.dart';
import 'package:path/path.dart';
import 'package:sqflite/sqflite.dart';

import '../utilities/console_log.dart';

class AppDatabase {
  /// Make [AppDatabase] to be singleton
  static final AppDatabase _instance = AppDatabase._();

  factory AppDatabase() => _instance;

  AppDatabase._();

  late Database database;

  Future<void> init() async {
    // Get the path to the database
    String path = join(await getDatabasesPath(), AppDatabaseConfig.dbPath);

    if (kDebugMode) {
      // Only for development purpose
      // await dropDatabase(path);
    }

    // Create database if not exists
    File databaseFile = File(path);

    if (!await databaseFile.exists()) await databaseFile.create();

    // Open database
    database = await openDatabase(path);

    // Create tables
    await Future.wait([
      database.execute(AppDatabaseConfig.createUserTable),
      database.execute(AppDatabaseConfig.createProductTable),
      database.execute(AppDatabaseConfig.createTransactionTable),
      database.execute(AppDatabaseConfig.createOrderedProductTable),
      database.execute(AppDatabaseConfig.createQueuedActionTable),
      database.execute(AppDatabaseConfig.createPaymentTable),
      database.execute(AppDatabaseConfig.createDiscountTable),
      database.execute(AppDatabaseConfig.createShiftTable),
      database.execute(AppDatabaseConfig.createCashMovementTable),
      database.execute(AppDatabaseConfig.createZReportTable),
      database.execute(AppDatabaseConfig.createAppointmentTable),
      database.execute(AppDatabaseConfig.createCustomerTable),
    ]);

    // Lightweight migration: ensure newly added columns exist
    await _ensureColumnExists(
      table: AppDatabaseConfig.transactionTableName,
      column: 'customerPhone',
      addColumnSql: "ALTER TABLE '${AppDatabaseConfig.transactionTableName}' ADD COLUMN 'customerPhone' TEXT",
    );
    await _ensureColumnExists(
      table: AppDatabaseConfig.queuedActionTableName,
      column: 'status',
      addColumnSql: "ALTER TABLE '${AppDatabaseConfig.queuedActionTableName}' ADD COLUMN 'status' TEXT DEFAULT 'pending'",
    );
    await _ensureColumnExists(
      table: AppDatabaseConfig.queuedActionTableName,
      column: 'retryCount',
      addColumnSql: "ALTER TABLE '${AppDatabaseConfig.queuedActionTableName}' ADD COLUMN 'retryCount' INTEGER DEFAULT 0",
    );
    await _ensureColumnExists(
      table: AppDatabaseConfig.queuedActionTableName,
      column: 'lastError',
      addColumnSql: "ALTER TABLE '${AppDatabaseConfig.queuedActionTableName}' ADD COLUMN 'lastError' TEXT",
    );
    await _ensureColumnExists(
      table: AppDatabaseConfig.queuedActionTableName,
      column: 'nextRetryAt',
      addColumnSql: "ALTER TABLE '${AppDatabaseConfig.queuedActionTableName}' ADD COLUMN 'nextRetryAt' DATETIME",
    );
  }

  Future<void> _ensureColumnExists({
    required String table,
    required String column,
    required String addColumnSql,
  }) async {
    try {
      final res = await database.rawQuery("PRAGMA table_info('$table')");
      final hasColumn = res.any((row) => (row['name'] as String?) == column);
      if (!hasColumn) {
        await database.execute(addColumnSql);
        cl('[AppDatabase] Added missing column $column to $table');
      }
    } catch (e) {
      cl('[AppDatabase] Column check failed for $table.$column: $e');
    }
  }

  // Only for testing
  Future<void> initTestDatabase({required Database testDatabase}) async {
    // Ensure this func only can be run in debug mode and completely removed in release mode
    assert(
      () {
        database = testDatabase;
        return true;
      }(),
      "[AppDatabase].initTestDatabase should only be used in unit tests.",
    );

    if (!kDebugMode) return;

    // Create tables
    await Future.wait([
      database.execute(AppDatabaseConfig.createUserTable),
      database.execute(AppDatabaseConfig.createProductTable),
      database.execute(AppDatabaseConfig.createTransactionTable),
      database.execute(AppDatabaseConfig.createOrderedProductTable),
      database.execute(AppDatabaseConfig.createQueuedActionTable),
    ]);
  }

  Future<void> dropDatabase(String path) async {
    // Check if the database file exists
    File databaseFile = File(path);

    if (await databaseFile.exists()) {
      // Delete the database file
      await databaseFile.delete();

      cl('[AppDatabase].dropDatabase = Database deleted successfully.');
    } else {
      cl('[AppDatabase].dropDatabase = Database does not exist.');
    }
  }
}

class AppDatabaseConfig {
  static const String dbPath = 'app_database.db';
  static const int version = 1;

  static const String userTableName = 'User';
  static const String productTableName = 'Product';
  static const String transactionTableName = 'Transaction';
  static const String orderedProductTableName = 'OrderedProduct';
  static const String queuedActionTableName = 'QueuedAction';
  static const String paymentTableName = 'Payment';
  static const String discountTableName = 'Discount';
  static const String shiftTableName = 'Shift';
  static const String cashMovementTableName = 'CashMovement';
  static const String zReportTableName = 'ZReport';
  static const String appointmentTableName = 'Appointment';
  static const String customerTableName = 'Customer';

  static String createUserTable =
      '''
CREATE TABLE IF NOT EXISTS '$userTableName' (
    'id' TEXT NOT NULL,
    'email' TEXT,
    'phone' TEXT,
    'name' TEXT,
    'gender' TEXT,
    'birthdate' TEXT,
    'imageUrl' TEXT,
    'createdAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    'updatedAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY ('id')
);
''';

  static String createProductTable =
      '''
CREATE TABLE IF NOT EXISTS '$productTableName' (
    'id' INTEGER NOT NULL,
    'createdById' TEXT,
    'name' TEXT,
    'imageUrl' TEXT,
    'stock' INTEGER,
    'sold' INTEGER,
    'price' INTEGER,
    'description' TEXT,
    createdAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    updatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY ('id'),
    FOREIGN KEY ('createdById') REFERENCES 'User' ('id')
);
''';

  static String createTransactionTable =
      '''
CREATE TABLE IF NOT EXISTS '$transactionTableName' (
    'id' INTEGER NOT NULL,
    'paymentMethod' TEXT,
    'customerName' TEXT,
    'customerPhone' TEXT,
    'description' TEXT,
    'createdById' TEXT,
    'receivedAmount' INTEGER,
    'returnAmount' INTEGER,
    'totalAmount' INTEGER,
    'totalOrderedProduct' INTEGER,
    'createdAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    'updatedAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY ('id'),
    FOREIGN KEY ('createdById') REFERENCES 'User' ('id')
);
''';

  static String createOrderedProductTable =
      '''
CREATE TABLE IF NOT EXISTS '$orderedProductTableName' (
    'id' INTEGER NOT NULL,
    'transactionId' INTEGER,
    'productId' INTEGER,
    'quantity' INTEGER,
    'stock' INTEGER,
    'name' TEXT,
    'imageUrl' TEXT,
    'price' INTEGER,
    'createdAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    'updatedAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY ('id'),
    FOREIGN KEY ('transactionId') REFERENCES 'Transaction' ('id'),
    FOREIGN KEY ('productId') REFERENCES 'Product' ('id')
);
''';

  static String createQueuedActionTable =
      '''
CREATE TABLE IF NOT EXISTS '$queuedActionTableName' (
    'id' INTEGER NOT NULL,
    'repository' TEXT,
    'method' TEXT,
    'param' TEXT,
    'isCritical' INTEGER,
    'status' TEXT DEFAULT 'pending',
    'retryCount' INTEGER DEFAULT 0,
    'lastError' TEXT,
    'nextRetryAt' DATETIME,
    'createdAt' DATETIME DEFAULT CURRENT_TIMESTAMP
);
''';

  static String createPaymentTable =
      '''
CREATE TABLE IF NOT EXISTS '$paymentTableName' (
    'id' INTEGER NOT NULL,
    'transactionId' INTEGER,
    'method' TEXT,
    'amount' INTEGER,
    'reference' TEXT,
    'createdAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    'updatedAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY ('id'),
    FOREIGN KEY ('transactionId') REFERENCES 'Transaction' ('id')
);
''';

  static String createDiscountTable =
      '''
CREATE TABLE IF NOT EXISTS '$discountTableName' (
    'id' INTEGER NOT NULL,
    'transactionId' INTEGER,
    'orderedProductId' INTEGER,
    'scope' TEXT,
    'type' TEXT,
    'value' INTEGER,
    'code' TEXT,
    'reason' TEXT,
    'createdAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    'updatedAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY ('id'),
    FOREIGN KEY ('transactionId') REFERENCES 'Transaction' ('id'),
    FOREIGN KEY ('orderedProductId') REFERENCES 'OrderedProduct' ('id')
);
''';

  static String createShiftTable =
      '''
CREATE TABLE IF NOT EXISTS '$shiftTableName' (
    'id' INTEGER NOT NULL,
    'userId' TEXT,
    'startedAt' DATETIME,
    'openingFloat' INTEGER,
    'endedAt' DATETIME,
    'closingCash' INTEGER,
    'expectedCash' INTEGER,
    'variance' INTEGER,
    'status' TEXT,
    'createdAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    'updatedAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY ('id'),
    FOREIGN KEY ('userId') REFERENCES 'User' ('id')
);
''';

  static String createCashMovementTable =
      '''
CREATE TABLE IF NOT EXISTS '$cashMovementTableName' (
    'id' INTEGER NOT NULL,
    'shiftId' INTEGER,
    'type' TEXT,
    'amount' INTEGER,
    'note' TEXT,
    'createdAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY ('id'),
    FOREIGN KEY ('shiftId') REFERENCES 'Shift' ('id')
);
''';

  static String createZReportTable =
      '''
CREATE TABLE IF NOT EXISTS '$zReportTableName' (
    'id' INTEGER NOT NULL,
    'shiftId' INTEGER,
    'summaryJson' TEXT,
    'createdAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY ('id'),
    FOREIGN KEY ('shiftId') REFERENCES 'Shift' ('id')
);
''';

  static String createAppointmentTable =
      '''
CREATE TABLE IF NOT EXISTS '$appointmentTableName' (
    'id' INTEGER NOT NULL,
    'customerName' TEXT,
    'customerPhone' TEXT,
    'serviceName' TEXT,
    'staffName' TEXT,
    'scheduledAt' DATETIME,
    'status' TEXT,
    'note' TEXT,
    'linkedTransactionId' INTEGER,
    'createdAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    'updatedAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY ('id'),
    FOREIGN KEY ('linkedTransactionId') REFERENCES 'Transaction' ('id')
);
''';

  static String createCustomerTable =
      '''
CREATE TABLE IF NOT EXISTS '$customerTableName' (
    'id' TEXT NOT NULL,
    'name' TEXT,
    'phone' TEXT,
    'pointsBalance' INTEGER,
    'createdAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    'updatedAt' DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY ('id')
);
''';
}
