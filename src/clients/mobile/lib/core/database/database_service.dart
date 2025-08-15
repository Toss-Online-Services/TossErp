import 'package:sqflite/sqflite.dart';
import 'package:path/path.dart';
import 'package:flutter/foundation.dart';

class DatabaseService {
  static Database? _database;
  
  static Future<void> initialize() async {
    if (_database != null) return;
    
    // Skip database initialization on web for now
    if (kIsWeb) {
      print('Database initialization skipped on web platform');
      return;
    }
    
    try {
      final databasePath = await getDatabasesPath();
      final path = join(databasePath, 'pos_store.db');
      
      _database = await openDatabase(
        path,
        version: 1,
        onCreate: _onCreate,
      );
    } catch (e) {
      print('Database initialization failed: $e');
    }
  }
  
  static Future<void> _onCreate(Database db, int version) async {
    // Items table
    await db.execute('''
      CREATE TABLE items (
        id TEXT PRIMARY KEY,
        sku TEXT UNIQUE NOT NULL,
        name TEXT NOT NULL,
        description TEXT,
        category TEXT,
        standardRate REAL NOT NULL,
        costPrice REAL,
        isActive INTEGER NOT NULL DEFAULT 1,
        createdAt TEXT NOT NULL,
        updatedAt TEXT NOT NULL
      )
    ''');
    
    // Stock levels table
    await db.execute('''
      CREATE TABLE stock_levels (
        id TEXT PRIMARY KEY,
        itemId TEXT NOT NULL,
        warehouseId TEXT NOT NULL,
        binId TEXT,
        quantity REAL NOT NULL DEFAULT 0,
        unitCost REAL NOT NULL DEFAULT 0,
        reorderPoint REAL DEFAULT 0,
        maxStock REAL,
        createdAt TEXT NOT NULL,
        updatedAt TEXT NOT NULL,
        FOREIGN KEY (itemId) REFERENCES items (id)
      )
    ''');
    
    // Stock movements table
    await db.execute('''
      CREATE TABLE stock_movements (
        id TEXT PRIMARY KEY,
        itemId TEXT NOT NULL,
        warehouseId TEXT NOT NULL,
        movementType TEXT NOT NULL,
        quantity REAL NOT NULL,
        unitCost REAL NOT NULL,
        referenceNumber TEXT,
        referenceType TEXT,
        reason TEXT,
        binId TEXT,
        batchId TEXT,
        createdBy TEXT NOT NULL,
        createdAt TEXT NOT NULL,
        FOREIGN KEY (itemId) REFERENCES items (id)
      )
    ''');
    
    // Sales table
    await db.execute('''
      CREATE TABLE sales (
        id TEXT PRIMARY KEY,
        saleNumber TEXT UNIQUE NOT NULL,
        customerId TEXT,
        totalAmount REAL NOT NULL,
        discountAmount REAL DEFAULT 0,
        taxAmount REAL DEFAULT 0,
        finalAmount REAL NOT NULL,
        paymentMethod TEXT,
        status TEXT NOT NULL,
        completedBy TEXT,
        completedAt TEXT,
        createdAt TEXT NOT NULL
      )
    ''');
    
    // Sale items table
    await db.execute('''
      CREATE TABLE sale_items (
        id TEXT PRIMARY KEY,
        saleId TEXT NOT NULL,
        itemId TEXT NOT NULL,
        quantity REAL NOT NULL,
        unitPrice REAL NOT NULL,
        totalPrice REAL NOT NULL,
        discountAmount REAL DEFAULT 0,
        createdAt TEXT NOT NULL,
        FOREIGN KEY (saleId) REFERENCES sales (id),
        FOREIGN KEY (itemId) REFERENCES items (id)
      )
    ''');
    
    // Sync queue table
    await db.execute('''
      CREATE TABLE sync_queue (
        id TEXT PRIMARY KEY,
        tableName TEXT NOT NULL,
        recordId TEXT NOT NULL,
        operation TEXT NOT NULL,
        data TEXT NOT NULL,
        status TEXT NOT NULL DEFAULT 'pending',
        retryCount INTEGER DEFAULT 0,
        createdAt TEXT NOT NULL,
        processedAt TEXT
      )
    ''');
    
    // Create indexes
    await db.execute('CREATE INDEX idx_items_sku ON items (sku)');
    await db.execute('CREATE INDEX idx_items_category ON items (category)');
    await db.execute('CREATE INDEX idx_stock_levels_item ON stock_levels (itemId)');
    await db.execute('CREATE INDEX idx_stock_levels_warehouse ON stock_levels (warehouseId)');
    await db.execute('CREATE INDEX idx_stock_movements_item ON stock_movements (itemId)');
    await db.execute('CREATE INDEX idx_stock_movements_date ON stock_movements (createdAt)');
    await db.execute('CREATE INDEX idx_sales_number ON sales (saleNumber)');
    await db.execute('CREATE INDEX idx_sales_date ON sales (createdAt)');
    await db.execute('CREATE INDEX idx_sync_queue_status ON sync_queue (status)');
  }
  
  Database? get database {
    return _database;
  }
  
  // Basic CRUD operations
  Future<List<Map<String, dynamic>>> query(String table, {String? where, List<Object?>? whereArgs}) async {
    if (database == null) return [];
    return await database!.query(table, where: where, whereArgs: whereArgs);
  }
  
  Future<int> insert(String table, Map<String, Object?> values) async {
    if (database == null) return 0;
    return await database!.insert(table, values);
  }
  
  Future<int> update(String table, Map<String, Object?> values, {String? where, List<Object?>? whereArgs}) async {
    if (database == null) return 0;
    return await database!.update(table, values, where: where, whereArgs: whereArgs);
  }
  
  Future<int> delete(String table, {String? where, List<Object?>? whereArgs}) async {
    if (database == null) return 0;
    return await database!.delete(table, where: where, whereArgs: whereArgs);
  }
  
  Future<void> close() async {
    if (database != null) {
      await database!.close();
      _database = null;
    }
  }
}
