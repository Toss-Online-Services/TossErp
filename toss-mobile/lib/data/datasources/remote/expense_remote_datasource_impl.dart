import 'package:cloud_firestore/cloud_firestore.dart';
import '../../../domain/entities/expense_entity.dart';
import '../interfaces/expense_datasource.dart';

class ExpenseRemoteDatasourceImpl implements ExpenseDatasource {
  final FirebaseFirestore _firestore;

  ExpenseRemoteDatasourceImpl(this._firestore);

  @override
  Future<int> createExpense(ExpenseEntity expense) async {
    final docRef = await _firestore
        .collection('expenses')
        .add(_expenseToMap(expense));
    return docRef.id.hashCode;
  }

  @override
  Future<List<ExpenseEntity>> getAllExpenses() async {
    final snapshot = await _firestore
        .collection('expenses')
        .orderBy('expenseDate', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToExpense(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<ExpenseEntity?> getExpenseById(int id) async {
    final snapshot = await _firestore
        .collection('expenses')
        .where('id', isEqualTo: id)
        .limit(1)
        .get();
    
    if (snapshot.docs.isNotEmpty) {
      final doc = snapshot.docs.first;
      return _mapToExpense(doc.data(), doc.id);
    }
    return null;
  }

  @override
  Future<List<ExpenseEntity>> getExpensesByStatus(ExpenseStatus status) async {
    final snapshot = await _firestore
        .collection('expenses')
        .where('status', isEqualTo: status.name)
        .orderBy('expenseDate', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToExpense(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<ExpenseEntity>> getExpensesByType(ExpenseType type) async {
    final snapshot = await _firestore
        .collection('expenses')
        .where('type', isEqualTo: type.name)
        .orderBy('expenseDate', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToExpense(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<ExpenseEntity>> getExpensesByDateRange(
    DateTime startDate,
    DateTime endDate,
  ) async {
    final snapshot = await _firestore
        .collection('expenses')
        .where('expenseDate', isGreaterThanOrEqualTo: startDate.toIso8601String())
        .where('expenseDate', isLessThanOrEqualTo: endDate.toIso8601String())
        .orderBy('expenseDate', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToExpense(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<ExpenseEntity>> getOverdueExpenses() async {
    final now = DateTime.now().toIso8601String();
    final snapshot = await _firestore
        .collection('expenses')
        .where('dueDate', isLessThan: now)
        .where('status', isNotEqualTo: ExpenseStatus.paid.name)
        .orderBy('dueDate')
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToExpense(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<void> updateExpense(ExpenseEntity expense) async {
    await _firestore
        .collection('expenses')
        .doc(expense.id.toString())
        .update(_expenseToMap(expense));
  }

  @override
  Future<void> deleteExpense(int id) async {
    await _firestore
        .collection('expenses')
        .doc(id.toString())
        .delete();
  }

  @override
  Future<void> updateExpenseStatus(int id, ExpenseStatus status) async {
    await _firestore
        .collection('expenses')
        .doc(id.toString())
        .update({
      'status': status.name,
      'updatedAt': DateTime.now().toIso8601String(),
    });
  }

  @override
  Future<void> seedSampleExpenses() async {
    // Remote seeding is typically not done in production
    // This method is kept for consistency with the interface
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

  ExpenseEntity _mapToExpense(Map<String, dynamic> map, String documentId) {
    return ExpenseEntity(
      id: map['id'] as int? ?? documentId.hashCode,
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
