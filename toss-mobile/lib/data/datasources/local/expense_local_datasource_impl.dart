import 'package:sqflite/sqflite.dart';
import '../../../app/database/app_database.dart';
import '../../../domain/entities/expense_entity.dart';
import '../interfaces/expense_datasource.dart';

class ExpenseLocalDatasourceImpl implements ExpenseDatasource {
  final AppDatabase _appDatabase;

  ExpenseLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createExpense(ExpenseEntity expense) async {
    final id = await _appDatabase.database.insert(
      AppDatabaseConfig.expenseTableName,
      _expenseToMap(expense),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
    return id;
  }

  @override
  Future<List<ExpenseEntity>> getAllExpenses() async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.expenseTableName,
      orderBy: 'expenseDate DESC',
    );
    return res.map((row) => _mapToExpense(row)).toList();
  }

  @override
  Future<ExpenseEntity?> getExpenseById(int id) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.expenseTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
    if (res.isNotEmpty) {
      return _mapToExpense(res.first);
    }
    return null;
  }

  @override
  Future<List<ExpenseEntity>> getExpensesByStatus(ExpenseStatus status) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.expenseTableName,
      where: 'status = ?',
      whereArgs: [status.name],
      orderBy: 'expenseDate DESC',
    );
    return res.map((row) => _mapToExpense(row)).toList();
  }

  @override
  Future<List<ExpenseEntity>> getExpensesByType(ExpenseType type) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.expenseTableName,
      where: 'type = ?',
      whereArgs: [type.name],
      orderBy: 'expenseDate DESC',
    );
    return res.map((row) => _mapToExpense(row)).toList();
  }

  @override
  Future<List<ExpenseEntity>> getExpensesByDateRange(
    DateTime startDate,
    DateTime endDate,
  ) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.expenseTableName,
      where: 'expenseDate >= ? AND expenseDate <= ?',
      whereArgs: [startDate.toIso8601String(), endDate.toIso8601String()],
      orderBy: 'expenseDate DESC',
    );
    return res.map((row) => _mapToExpense(row)).toList();
  }

  @override
  Future<List<ExpenseEntity>> getOverdueExpenses() async {
    final now = DateTime.now().toIso8601String();
    final res = await _appDatabase.database.rawQuery('''
      SELECT * FROM ${AppDatabaseConfig.expenseTableName}
      WHERE dueDate < ? AND status != 'paid'
      ORDER BY dueDate ASC
    ''', [now]);
    
    return res.map((row) => _mapToExpense(row)).toList();
  }

  @override
  Future<void> updateExpense(ExpenseEntity expense) async {
    await _appDatabase.database.update(
      AppDatabaseConfig.expenseTableName,
      _expenseToMap(expense),
      where: 'id = ?',
      whereArgs: [expense.id],
    );
  }

  @override
  Future<void> deleteExpense(int id) async {
    await _appDatabase.database.delete(
      AppDatabaseConfig.expenseTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<void> updateExpenseStatus(int id, ExpenseStatus status) async {
    await _appDatabase.database.update(
      AppDatabaseConfig.expenseTableName,
      {'status': status.name, 'updatedAt': DateTime.now().toIso8601String()},
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<void> seedSampleExpenses() async {
    final sampleExpenses = [
      ExpenseEntity(
        id: DateTime.now().millisecondsSinceEpoch,
        expenseNumber: 'EXP-2024-001',
        description: 'Office supplies purchase',
        type: ExpenseType.supplies,
        status: ExpenseStatus.approved,
        amount: 50000, // $500.00 in cents
        expenseDate: DateTime.now().subtract(const Duration(days: 1)),
        dueDate: DateTime.now().add(const Duration(days: 30)),
        vendor: 'Office Depot',
        invoiceNumber: 'INV-001',
        category: 'Office Supplies',
        notes: 'Monthly office supplies order',
        approvedBy: 'manager1',
        approvedAt: DateTime.now().subtract(const Duration(hours: 12)),
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(days: 1)).toIso8601String(),
        updatedAt: DateTime.now().subtract(const Duration(hours: 12)).toIso8601String(),
      ),
      ExpenseEntity(
        id: DateTime.now().millisecondsSinceEpoch + 1,
        expenseNumber: 'EXP-2024-002',
        description: 'Marketing campaign',
        type: ExpenseType.marketing,
        status: ExpenseStatus.pending,
        amount: 200000, // $2000.00 in cents
        expenseDate: DateTime.now().subtract(const Duration(days: 2)),
        dueDate: DateTime.now().add(const Duration(days: 15)),
        vendor: 'Marketing Agency',
        invoiceNumber: 'INV-002',
        category: 'Marketing',
        notes: 'Q1 marketing campaign',
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(days: 2)).toIso8601String(),
        updatedAt: DateTime.now().subtract(const Duration(days: 2)).toIso8601String(),
      ),
      ExpenseEntity(
        id: DateTime.now().millisecondsSinceEpoch + 2,
        expenseNumber: 'EXP-2024-003',
        description: 'Equipment maintenance',
        type: ExpenseType.maintenance,
        status: ExpenseStatus.paid,
        amount: 75000, // $750.00 in cents
        expenseDate: DateTime.now().subtract(const Duration(days: 5)),
        dueDate: DateTime.now().subtract(const Duration(days: 1)),
        vendor: 'Tech Services Inc',
        invoiceNumber: 'INV-003',
        category: 'Maintenance',
        notes: 'POS system maintenance',
        approvedBy: 'manager1',
        approvedAt: DateTime.now().subtract(const Duration(days: 4)),
        paidBy: 'user1',
        paidAt: DateTime.now().subtract(const Duration(days: 1)),
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(days: 5)).toIso8601String(),
        updatedAt: DateTime.now().subtract(const Duration(days: 1)).toIso8601String(),
      ),
    ];

    for (final expense in sampleExpenses) {
      await createExpense(expense);
    }
  }

  Map<String, dynamic> _expenseToMap(ExpenseEntity expense) {
    return {
      'id': expense.id,
      'expenseNumber': expense.expenseNumber,
      'description': expense.description,
      'type': expense.type.name,
      'status': expense.status.name,
      'amount': expense.amount,
      'expenseDate': expense.expenseDate.toIso8601String(),
      'dueDate': expense.dueDate?.toIso8601String(),
      'vendor': expense.vendor,
      'invoiceNumber': expense.invoiceNumber,
      'receiptUrl': expense.receiptUrl,
      'category': expense.category,
      'notes': expense.notes,
      'approvedBy': expense.approvedBy,
      'approvedAt': expense.approvedAt?.toIso8601String(),
      'paidBy': expense.paidBy,
      'paidAt': expense.paidAt?.toIso8601String(),
      'createdById': expense.createdById,
      'createdAt': expense.createdAt,
      'updatedAt': expense.updatedAt,
    };
  }

  ExpenseEntity _mapToExpense(Map<String, dynamic> map) {
    return ExpenseEntity(
      id: map['id'] as int,
      expenseNumber: map['expenseNumber'] as String,
      description: map['description'] as String,
      type: ExpenseType.values.firstWhere(
        (e) => e.name == map['type'],
        orElse: () => ExpenseType.other,
      ),
      status: ExpenseStatus.values.firstWhere(
        (e) => e.name == map['status'],
        orElse: () => ExpenseStatus.pending,
      ),
      amount: map['amount'] as int,
      expenseDate: DateTime.parse(map['expenseDate'] as String),
      dueDate: map['dueDate'] != null ? DateTime.parse(map['dueDate'] as String) : null,
      vendor: map['vendor'] as String?,
      invoiceNumber: map['invoiceNumber'] as String?,
      receiptUrl: map['receiptUrl'] as String?,
      category: map['category'] as String?,
      notes: map['notes'] as String?,
      approvedBy: map['approvedBy'] as String?,
      approvedAt: map['approvedAt'] != null ? DateTime.parse(map['approvedAt'] as String) : null,
      paidBy: map['paidBy'] as String?,
      paidAt: map['paidAt'] != null ? DateTime.parse(map['paidAt'] as String) : null,
      createdById: map['createdById'] as String?,
      createdAt: map['createdAt'] as String?,
      updatedAt: map['updatedAt'] as String?,
    );
  }
}
