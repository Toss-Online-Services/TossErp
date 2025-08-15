import 'package:sqflite/sqflite.dart';
import 'package:path/path.dart';
import 'package:pos_store/shared/models/item.dart';
import 'package:pos_store/shared/models/stock_level.dart';
import 'package:pos_store/shared/models/stock_movement.dart';
import 'package:pos_store/shared/models/sale.dart';
import 'package:pos_store/shared/models/sale_item.dart';

class DatabaseService {
  static DatabaseService? _instance;
  static Database? _database;

  DatabaseService._();

  static DatabaseService get instance {
    _instance ??= DatabaseService._();
    return _instance!;
  }

  static Future<void> initialize() async {
    final databasePath = await getDatabasesPath();
    final path = join(databasePath, 'pos_store.db');

    _database = await openDatabase(
      path,
      version: 1,
      onCreate: _onCreate,
      onUpgrade: _onUpgrade,
    );
  }

  static Future<void> _onCreate(Database db, int version) async {
    // Items table
    await db.execute('''
      CREATE TABLE items (
        id TEXT PRIMARY KEY,
        sku TEXT NOT NULL,
        name TEXT NOT NULL,
        description TEXT,
        category TEXT,
        unit TEXT,
        standard_rate REAL,
        minimum_price REAL,
        weight_per_unit REAL,
        length REAL,
        width REAL,
        height REAL,
        is_active INTEGER DEFAULT 1,
        created_at TEXT,
        created_by TEXT,
        updated_at TEXT,
        updated_by TEXT
      )
    ''');

    // Stock levels table
    await db.execute('''
      CREATE TABLE stock_levels (
        id TEXT PRIMARY KEY,
        item_id TEXT NOT NULL,
        warehouse_id TEXT NOT NULL,
        bin_id TEXT,
        quantity REAL DEFAULT 0,
        reserved_quantity REAL DEFAULT 0,
        unit_cost REAL DEFAULT 0,
        last_movement_date TEXT,
        last_updated TEXT,
        FOREIGN KEY (item_id) REFERENCES items (id)
      )
    ''');

    // Stock movements table
    await db.execute('''
      CREATE TABLE stock_movements (
        id TEXT PRIMARY KEY,
        tenant_id TEXT NOT NULL,
        item_id TEXT NOT NULL,
        warehouse_id TEXT NOT NULL,
        bin_id TEXT,
        movement_type TEXT NOT NULL,
        quantity REAL NOT NULL,
        unit_cost REAL,
        reference_number TEXT,
        reference_type TEXT,
        reason TEXT,
        movement_date TEXT,
        batch_id TEXT,
        created_by TEXT,
        created_at TEXT,
        FOREIGN KEY (item_id) REFERENCES items (id)
      )
    ''');

    // Sales table
    await db.execute('''
      CREATE TABLE sales (
        id TEXT PRIMARY KEY,
        sale_number TEXT NOT NULL,
        customer_id TEXT,
        customer_name TEXT,
        total_amount REAL NOT NULL,
        discount_amount REAL DEFAULT 0,
        tax_amount REAL DEFAULT 0,
        final_amount REAL NOT NULL,
        payment_method TEXT,
        status TEXT DEFAULT 'pending',
        created_by TEXT,
        created_at TEXT,
        updated_at TEXT
      )
    ''');

    // Sale items table
    await db.execute('''
      CREATE TABLE sale_items (
        id TEXT PRIMARY KEY,
        sale_id TEXT NOT NULL,
        item_id TEXT NOT NULL,
        quantity REAL NOT NULL,
        unit_price REAL NOT NULL,
        total_price REAL NOT NULL,
        discount_amount REAL DEFAULT 0,
        tax_amount REAL DEFAULT 0,
        FOREIGN KEY (sale_id) REFERENCES sales (id),
        FOREIGN KEY (item_id) REFERENCES items (id)
      )
    ''');

    // Sync queue table
    await db.execute('''
      CREATE TABLE sync_queue (
        id TEXT PRIMARY KEY,
        table_name TEXT NOT NULL,
        record_id TEXT NOT NULL,
        operation TEXT NOT NULL,
        data TEXT,
        created_at TEXT,
        synced_at TEXT
      )
    ''');

    // Create indexes
    await db.execute('CREATE INDEX idx_items_sku ON items (sku)');
    await db.execute('CREATE INDEX idx_items_category ON items (category)');
    await db.execute('CREATE INDEX idx_stock_levels_item_warehouse ON stock_levels (item_id, warehouse_id)');
    await db.execute('CREATE INDEX idx_stock_movements_item ON stock_movements (item_id)');
    await db.execute('CREATE INDEX idx_stock_movements_date ON stock_movements (movement_date)');
    await db.execute('CREATE INDEX idx_sales_number ON sales (sale_number)');
    await db.execute('CREATE INDEX idx_sales_date ON sales (created_at)');
    await db.execute('CREATE INDEX idx_sync_queue_table ON sync_queue (table_name)');
    await db.execute('CREATE INDEX idx_sync_queue_synced ON sync_queue (synced_at)');
  }

  static Future<void> _onUpgrade(Database db, int oldVersion, int newVersion) async {
    // Handle database upgrades here
  }

  Database get database {
    if (_database == null) {
      throw Exception('Database not initialized. Call DatabaseService.initialize() first.');
    }
    return _database!;
  }

  // Items operations
  Future<List<Item>> getItems() async {
    final List<Map<String, dynamic>> maps = await database.query('items');
    return List.generate(maps.length, (i) => Item.fromMap(maps[i]));
  }

  Future<Item?> getItemById(String id) async {
    final List<Map<String, dynamic>> maps = await database.query(
      'items',
      where: 'id = ?',
      whereArgs: [id],
    );
    if (maps.isNotEmpty) {
      return Item.fromMap(maps.first);
    }
    return null;
  }

  Future<Item?> getItemBySku(String sku) async {
    final List<Map<String, dynamic>> maps = await database.query(
      'items',
      where: 'sku = ?',
      whereArgs: [sku],
    );
    if (maps.isNotEmpty) {
      return Item.fromMap(maps.first);
    }
    return null;
  }

  Future<void> insertItem(Item item) async {
    await database.insert('items', item.toMap());
  }

  Future<void> updateItem(Item item) async {
    await database.update(
      'items',
      item.toMap(),
      where: 'id = ?',
      whereArgs: [item.id],
    );
  }

  // Stock levels operations
  Future<List<StockLevel>> getStockLevels() async {
    final List<Map<String, dynamic>> maps = await database.query('stock_levels');
    return List.generate(maps.length, (i) => StockLevel.fromMap(maps[i]));
  }

  Future<StockLevel?> getStockLevel(String itemId, String warehouseId) async {
    final List<Map<String, dynamic>> maps = await database.query(
      'stock_levels',
      where: 'item_id = ? AND warehouse_id = ?',
      whereArgs: [itemId, warehouseId],
    );
    if (maps.isNotEmpty) {
      return StockLevel.fromMap(maps.first);
    }
    return null;
  }

  Future<void> insertStockLevel(StockLevel stockLevel) async {
    await database.insert('stock_levels', stockLevel.toMap());
  }

  Future<void> updateStockLevel(StockLevel stockLevel) async {
    await database.update(
      'stock_levels',
      stockLevel.toMap(),
      where: 'id = ?',
      whereArgs: [stockLevel.id],
    );
  }

  // Stock movements operations
  Future<List<StockMovement>> getStockMovements() async {
    final List<Map<String, dynamic>> maps = await database.query('stock_movements');
    return List.generate(maps.length, (i) => StockMovement.fromMap(maps[i]));
  }

  Future<void> insertStockMovement(StockMovement movement) async {
    await database.insert('stock_movements', movement.toMap());
  }

  // Sales operations
  Future<List<Sale>> getSales() async {
    final List<Map<String, dynamic>> maps = await database.query('sales');
    return List.generate(maps.length, (i) => Sale.fromMap(maps[i]));
  }

  Future<void> insertSale(Sale sale) async {
    await database.insert('sales', sale.toMap());
  }

  Future<void> insertSaleItem(SaleItem saleItem) async {
    await database.insert('sale_items', saleItem.toMap());
  }

  // Sync queue operations
  Future<void> addToSyncQueue(String tableName, String recordId, String operation, Map<String, dynamic> data) async {
    await database.insert('sync_queue', {
      'id': DateTime.now().millisecondsSinceEpoch.toString(),
      'table_name': tableName,
      'record_id': recordId,
      'operation': operation,
      'data': data.toString(),
      'created_at': DateTime.now().toIso8601String(),
    });
  }

  Future<List<Map<String, dynamic>>> getPendingSyncItems() async {
    return await database.query(
      'sync_queue',
      where: 'synced_at IS NULL',
      orderBy: 'created_at ASC',
    );
  }

  Future<void> markAsSynced(String id) async {
    await database.update(
      'sync_queue',
      {'synced_at': DateTime.now().toIso8601String()},
      where: 'id = ?',
      whereArgs: [id],
    );
  }
}
