import 'package:cloud_firestore/cloud_firestore.dart';
import '../../../domain/entities/financial_transaction_entity.dart';
import '../interfaces/financial_transaction_datasource.dart';

class FinancialTransactionRemoteDatasourceImpl implements FinancialTransactionDatasource {
  final FirebaseFirestore _firestore;

  FinancialTransactionRemoteDatasourceImpl(this._firestore);

  @override
  Future<int> createFinancialTransaction(FinancialTransactionEntity transaction) async {
    final docRef = await _firestore
        .collection('financial_transactions')
        .add(transaction.toMap());
    return docRef.id.hashCode;
  }

  @override
  Future<FinancialTransactionEntity?> getFinancialTransactionById(int id) async {
    final snapshot = await _firestore
        .collection('financial_transactions')
        .where('id', isEqualTo: id)
        .limit(1)
        .get();
    
    if (snapshot.docs.isNotEmpty) {
      final doc = snapshot.docs.first;
      return _mapToTransaction(doc.data(), doc.id);
    }
    return null;
  }

  @override
  Future<List<FinancialTransactionEntity>> getAllFinancialTransactions() async {
    final snapshot = await _firestore
        .collection('financial_transactions')
        .orderBy('createdAt', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToTransaction(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<FinancialTransactionEntity>> getTransactionsByType(TransactionType type) async {
    final snapshot = await _firestore
        .collection('financial_transactions')
        .where('type', isEqualTo: type.name)
        .orderBy('transactionDate', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToTransaction(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<FinancialTransactionEntity>> getTransactionsByCategory(TransactionCategory category) async {
    final snapshot = await _firestore
        .collection('financial_transactions')
        .where('category', isEqualTo: category.name)
        .orderBy('transactionDate', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToTransaction(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<FinancialTransactionEntity>> getTransactionsByDateRange(DateTime startDate, DateTime endDate) async {
    final snapshot = await _firestore
        .collection('financial_transactions')
        .where('transactionDate', isGreaterThanOrEqualTo: startDate.toIso8601String())
        .where('transactionDate', isLessThanOrEqualTo: endDate.toIso8601String())
        .orderBy('transactionDate', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToTransaction(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<FinancialTransactionEntity>> getTransactionsByReference(int referenceId, String referenceType) async {
    final snapshot = await _firestore
        .collection('financial_transactions')
        .where('referenceId', isEqualTo: referenceId)
        .where('referenceType', isEqualTo: referenceType)
        .orderBy('transactionDate', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToTransaction(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<void> updateFinancialTransaction(FinancialTransactionEntity transaction) async {
    await _firestore
        .collection('financial_transactions')
        .doc(transaction.id.toString())
        .update(transaction.toMap());
  }

  @override
  Future<void> deleteFinancialTransaction(int id) async {
    await _firestore
        .collection('financial_transactions')
        .doc(id.toString())
        .delete();
  }

  @override
  Future<void> seedSampleFinancialTransactions() async {
    // Remote seeding is typically not done in production
    // This method is kept for consistency with the interface
  }

  FinancialTransactionEntity _mapToTransaction(Map<String, dynamic> map, String documentId) {
    return FinancialTransactionEntity(
      id: map['id'] as int? ?? documentId.hashCode,
      transactionNumber: map['transactionNumber'] as String,
      type: TransactionType.values.firstWhere((e) => e.name == map['type']),
      category: TransactionCategory.values.firstWhere((e) => e.name == map['category']),
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
