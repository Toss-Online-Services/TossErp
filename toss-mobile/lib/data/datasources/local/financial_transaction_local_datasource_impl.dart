import 'package:sqflite/sqflite.dart';
import '../../../app/database/app_database.dart';
import '../../../domain/entities/financial_transaction_entity.dart';
import '../interfaces/financial_transaction_datasource.dart';

class FinancialTransactionLocalDatasourceImpl implements FinancialTransactionDatasource {
  final AppDatabase _appDatabase;

  FinancialTransactionLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createFinancialTransaction(FinancialTransactionEntity transaction) async {
    final id = await _appDatabase.database.insert(
      AppDatabaseConfig.financialTransactionTableName,
      _transactionToMap(transaction),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
    return id;
  }

  @override
  Future<List<FinancialTransactionEntity>> getAllFinancialTransactions() async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.financialTransactionTableName,
      orderBy: 'transactionDate DESC',
    );
    return res.map((row) => _mapToTransaction(row)).toList();
  }

  @override
  Future<FinancialTransactionEntity?> getFinancialTransactionById(int id) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.financialTransactionTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
    if (res.isNotEmpty) {
      return _mapToTransaction(res.first);
    }
    return null;
  }

  @override
  Future<List<FinancialTransactionEntity>> getTransactionsByType(TransactionType type) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.financialTransactionTableName,
      where: 'type = ?',
      whereArgs: [type.name],
      orderBy: 'transactionDate DESC',
    );
    return res.map((row) => _mapToTransaction(row)).toList();
  }

  @override
  Future<List<FinancialTransactionEntity>> getTransactionsByCategory(TransactionCategory category) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.financialTransactionTableName,
      where: 'category = ?',
      whereArgs: [category.name],
      orderBy: 'transactionDate DESC',
    );
    return res.map((row) => _mapToTransaction(row)).toList();
  }

  @override
  Future<List<FinancialTransactionEntity>> getTransactionsByDateRange(
    DateTime startDate,
    DateTime endDate,
  ) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.financialTransactionTableName,
      where: 'transactionDate >= ? AND transactionDate <= ?',
      whereArgs: [startDate.toIso8601String(), endDate.toIso8601String()],
      orderBy: 'transactionDate DESC',
    );
    return res.map((row) => _mapToTransaction(row)).toList();
  }

  @override
  Future<List<FinancialTransactionEntity>> getTransactionsByReference(
    int referenceId,
    String referenceType,
  ) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.financialTransactionTableName,
      where: 'referenceId = ? AND referenceType = ?',
      whereArgs: [referenceId, referenceType],
      orderBy: 'transactionDate DESC',
    );
    return res.map((row) => _mapToTransaction(row)).toList();
  }

  @override
  Future<void> updateFinancialTransaction(FinancialTransactionEntity transaction) async {
    await _appDatabase.database.update(
      AppDatabaseConfig.financialTransactionTableName,
      _transactionToMap(transaction),
      where: 'id = ?',
      whereArgs: [transaction.id],
    );
  }

  @override
  Future<void> deleteFinancialTransaction(int id) async {
    await _appDatabase.database.delete(
      AppDatabaseConfig.financialTransactionTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<void> seedSampleFinancialTransactions() async {
    final sampleTransactions = [
      FinancialTransactionEntity(
        id: DateTime.now().millisecondsSinceEpoch,
        transactionNumber: 'FT-2024-001',
        type: TransactionType.revenue,
        category: TransactionCategory.sales,
        amount: 500000, // $5000.00 in cents
        description: 'Daily sales revenue',
        transactionDate: DateTime.now().subtract(const Duration(days: 1)),
        referenceId: 1,
        referenceType: 'sale',
        accountCode: 'REV-001',
        notes: 'Regular sales transaction',
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(days: 1)).toIso8601String(),
        updatedAt: DateTime.now().subtract(const Duration(days: 1)).toIso8601String(),
      ),
      FinancialTransactionEntity(
        id: DateTime.now().millisecondsSinceEpoch + 1,
        transactionNumber: 'FT-2024-002',
        type: TransactionType.expense,
        category: TransactionCategory.operatingExpenses,
        amount: 150000, // $1500.00 in cents
        description: 'Monthly rent payment',
        transactionDate: DateTime.now().subtract(const Duration(days: 2)),
        referenceId: 1,
        referenceType: 'expense',
        accountCode: 'EXP-001',
        notes: 'Store rent for current month',
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(days: 2)).toIso8601String(),
        updatedAt: DateTime.now().subtract(const Duration(days: 2)).toIso8601String(),
      ),
      FinancialTransactionEntity(
        id: DateTime.now().millisecondsSinceEpoch + 2,
        transactionNumber: 'FT-2024-003',
        type: TransactionType.expense,
        category: TransactionCategory.costOfGoodsSold,
        amount: 200000, // $2000.00 in cents
        description: 'Inventory purchase',
        transactionDate: DateTime.now().subtract(const Duration(days: 3)),
        referenceId: 1,
        referenceType: 'purchase',
        accountCode: 'COGS-001',
        notes: 'Stock replenishment',
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(days: 3)).toIso8601String(),
        updatedAt: DateTime.now().subtract(const Duration(days: 3)).toIso8601String(),
      ),
    ];

    for (final transaction in sampleTransactions) {
      await createFinancialTransaction(transaction);
    }
  }

  Map<String, dynamic> _transactionToMap(FinancialTransactionEntity transaction) {
    return {
      'id': transaction.id,
      'transactionNumber': transaction.transactionNumber,
      'type': transaction.type.name,
      'category': transaction.category.name,
      'amount': transaction.amount,
      'description': transaction.description,
      'transactionDate': transaction.transactionDate.toIso8601String(),
      'referenceId': transaction.referenceId,
      'referenceType': transaction.referenceType,
      'accountCode': transaction.accountCode,
      'notes': transaction.notes,
      'createdById': transaction.createdById,
      'createdAt': transaction.createdAt,
      'updatedAt': transaction.updatedAt,
    };
  }

  FinancialTransactionEntity _mapToTransaction(Map<String, dynamic> map) {
    return FinancialTransactionEntity(
      id: map['id'] as int,
      transactionNumber: map['transactionNumber'] as String,
      type: TransactionType.values.firstWhere(
        (e) => e.name == map['type'],
        orElse: () => TransactionType.revenue,
      ),
      category: TransactionCategory.values.firstWhere(
        (e) => e.name == map['category'],
        orElse: () => TransactionCategory.sales,
      ),
      amount: map['amount'] as int,
      description: map['description'] as String,
      transactionDate: DateTime.parse(map['transactionDate'] as String),
      referenceId: map['referenceId'] as int?,
      referenceType: map['referenceType'] as String?,
      accountCode: map['accountCode'] as String?,
      notes: map['notes'] as String?,
      createdById: map['createdById'] as String?,
      createdAt: map['createdAt'] as String?,
      updatedAt: map['updatedAt'] as String?,
    );
  }
}
