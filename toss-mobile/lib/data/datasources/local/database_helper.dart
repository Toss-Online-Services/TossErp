import 'package:sqflite/sqflite.dart';
import 'package:path/path.dart';

/// Local SQLite database helper for offline functionality
class DatabaseHelper {
  static final DatabaseHelper _instance = DatabaseHelper._internal();
  factory DatabaseHelper() => _instance;
  DatabaseHelper._internal();

  static Database? _database;

  Future<Database> get database async {
    if (_database != null) return _database!;
    _database = await _initDatabase();
    return _database!;
  }

  Future<Database> _initDatabase() async {
    String path = join(await getDatabasesPath(), 'toss_offline.db');
    
    return await openDatabase(
      path,
      version: 1,
      onCreate: _onCreate,
      onUpgrade: _onUpgrade,
    );
  }

  Future<void> _onCreate(Database db, int version) async {
    // Sync Queue Table
    await db.execute('''
      CREATE TABLE sync_queue (
        id TEXT PRIMARY KEY,
        type TEXT NOT NULL,
        data TEXT NOT NULL,
        status TEXT NOT NULL,
        retry_count INTEGER DEFAULT 0,
        created_at TEXT NOT NULL,
        completed_at TEXT,
        error_message TEXT
      )
    ''');

    // Offline Sales Table
    await db.execute('''
      CREATE TABLE offline_sales (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        sale_number TEXT UNIQUE NOT NULL,
        customer_id INTEGER,
        customer_name TEXT,
        total_amount INTEGER NOT NULL,
        subtotal INTEGER NOT NULL,
        tax_amount INTEGER NOT NULL,
        discount_amount INTEGER DEFAULT 0,
        status TEXT NOT NULL,
        sale_date TEXT NOT NULL,
        synced INTEGER DEFAULT 0,
        created_at TEXT NOT NULL
      )
    ''');

    // Offline Sale Items Table
    await db.execute('''
      CREATE TABLE offline_sale_items (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        sale_id INTEGER NOT NULL,
        product_id INTEGER NOT NULL,
        product_name TEXT NOT NULL,
        product_sku TEXT,
        quantity INTEGER NOT NULL,
        unit_price INTEGER NOT NULL,
        line_total INTEGER NOT NULL,
        discount INTEGER DEFAULT 0,
        FOREIGN KEY (sale_id) REFERENCES offline_sales (id) ON DELETE CASCADE
      )
    ''');

    // Offline Payments Table
    await db.execute('''
      CREATE TABLE offline_payments (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        sale_id INTEGER NOT NULL,
        method TEXT NOT NULL,
        amount INTEGER NOT NULL,
        status TEXT NOT NULL,
        payment_date TEXT NOT NULL,
        transaction_id TEXT,
        FOREIGN KEY (sale_id) REFERENCES offline_sales (id) ON DELETE CASCADE
      )
    ''');

    // Cached Products Table
    await db.execute('''
      CREATE TABLE cached_products (
        id INTEGER PRIMARY KEY,
        name TEXT NOT NULL,
        sku TEXT,
        barcode TEXT,
        selling_price INTEGER NOT NULL,
        cost_price INTEGER,
        category TEXT,
        is_active INTEGER DEFAULT 1,
        stock_on_hand INTEGER DEFAULT 0,
        cached_at TEXT NOT NULL
      )
    ''');

    // Cached Customers Table
    await db.execute('''
      CREATE TABLE cached_customers (
        id INTEGER PRIMARY KEY,
        name TEXT NOT NULL,
        email TEXT,
        phone TEXT,
        loyalty_points INTEGER DEFAULT 0,
        credit_limit INTEGER,
        is_active INTEGER DEFAULT 1,
        cached_at TEXT NOT NULL
      )
    ''');

    // Create indexes for performance
    await db.execute('CREATE INDEX idx_sync_queue_status ON sync_queue(status)');
    await db.execute('CREATE INDEX idx_sync_queue_created ON sync_queue(created_at)');
    await db.execute('CREATE INDEX idx_offline_sales_synced ON offline_sales(synced)');
    await db.execute('CREATE INDEX idx_offline_sales_date ON offline_sales(sale_date)');
    await db.execute('CREATE INDEX idx_cached_products_active ON cached_products(is_active)');
    await db.execute('CREATE INDEX idx_cached_products_sku ON cached_products(sku)');
    await db.execute('CREATE INDEX idx_cached_products_barcode ON cached_products(barcode)');
  }

  Future<void> _onUpgrade(Database db, int oldVersion, int newVersion) async {
    // Handle database migrations
    if (oldVersion < newVersion) {
      // Migration logic would go here
    }
  }

  /// Cache products for offline use
  Future<void> cacheProducts(List<Map<String, dynamic>> products) async {
    final db = await database;
    final batch = db.batch();

    for (final product in products) {
      batch.insert(
        'cached_products',
        {
          ...product,
          'cached_at': DateTime.now().toIso8601String(),
        },
        conflictAlgorithm: ConflictAlgorithm.replace,
      );
    }

    await batch.commit(noResult: true);
  }

  /// Cache customers for offline use
  Future<void> cacheCustomers(List<Map<String, dynamic>> customers) async {
    final db = await database;
    final batch = db.batch();

    for (final customer in customers) {
      batch.insert(
        'cached_customers',
        {
          ...customer,
          'cached_at': DateTime.now().toIso8601String(),
        },
        conflictAlgorithm: ConflictAlgorithm.replace,
      );
    }

    await batch.commit(noResult: true);
  }

  /// Get all offline sales that need syncing
  Future<List<Map<String, dynamic>>> getPendingSales() async {
    final db = await database;
    return await db.query(
      'offline_sales',
      where: 'synced = ?',
      whereArgs: [0],
    );
  }

  /// Mark sale as synced
  Future<void> markSaleSynced(int saleId, int? remoteId) async {
    final db = await database;
    await db.update(
      'offline_sales',
      {'synced': 1, 'remote_id': remoteId},
      where: 'id = ?',
      whereArgs: [saleId],
    );
  }

  /// Clear old cached data (older than 7 days)
  Future<void> clearOldCache() async {
    final db = await database;
    final cutoffDate = DateTime.now().subtract(const Duration(days: 7));
    
    await db.delete(
      'cached_products',
      where: 'cached_at < ?',
      whereArgs: [cutoffDate.toIso8601String()],
    );
    
    await db.delete(
      'cached_customers',
      where: 'cached_at < ?',
      whereArgs: [cutoffDate.toIso8601String()],
    );
  }

  /// Get database statistics
  Future<Map<String, int>> getStats() async {
    final db = await database;
    
    final pendingSync = Sqflite.firstIntValue(
      await db.rawQuery('SELECT COUNT(*) FROM sync_queue WHERE status = ?', ['pending'])
    ) ?? 0;
    
    final offlineSales = Sqflite.firstIntValue(
      await db.rawQuery('SELECT COUNT(*) FROM offline_sales WHERE synced = 0')
    ) ?? 0;
    
    final cachedProducts = Sqflite.firstIntValue(
      await db.rawQuery('SELECT COUNT(*) FROM cached_products')
    ) ?? 0;
    
    final cachedCustomers = Sqflite.firstIntValue(
      await db.rawQuery('SELECT COUNT(*) FROM cached_customers')
    ) ?? 0;

    return {
      'pending_sync': pendingSync,
      'offline_sales': offlineSales,
      'cached_products': cachedProducts,
      'cached_customers': cachedCustomers,
    };
  }

  /// Close database connection
  Future<void> close() async {
    _connectivitySubscription?.cancel();
    final db = await database;
    await db.close();
  }
}

