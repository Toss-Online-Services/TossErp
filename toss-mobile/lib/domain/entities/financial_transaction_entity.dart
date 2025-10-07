import 'package:equatable/equatable.dart';

enum TransactionType { revenue, expense, asset, liability, equity }
enum TransactionCategory {
  // Revenue categories
  sales,
  service,
  interest,
  otherIncome,
  
  // Expense categories
  costOfGoodsSold,
  operatingExpenses,
  marketing,
  rent,
  utilities,
  salaries,
  insurance,
  depreciation,
  otherExpenses,
  
  // Asset categories
  cash,
  inventory,
  equipment,
  accountsReceivable,
  
  // Liability categories
  accountsPayable,
  loans,
  taxes,
  
  // Equity categories
  capital,
  retainedEarnings,
}

class FinancialTransactionEntity extends Equatable {
  final int? id;
  final String transactionNumber;
  final TransactionType type;
  final TransactionCategory category;
  final int amount; // in cents
  final String description;
  final DateTime transactionDate;
  final int? referenceId; // ID of related transaction, purchase order, etc.
  final String? referenceType; // 'sale', 'purchase', 'expense', etc.
  final String? accountCode;
  final String? notes;
  final String? createdById;
  final String? createdAt;
  final String? updatedAt;

  const FinancialTransactionEntity({
    this.id,
    required this.transactionNumber,
    required this.type,
    required this.category,
    required this.amount,
    required this.description,
    required this.transactionDate,
    this.referenceId,
    this.referenceType,
    this.accountCode,
    this.notes,
    this.createdById,
    this.createdAt,
    this.updatedAt,
  });

  FinancialTransactionEntity copyWith({
    int? id,
    String? transactionNumber,
    TransactionType? type,
    TransactionCategory? category,
    int? amount,
    String? description,
    DateTime? transactionDate,
    int? referenceId,
    String? referenceType,
    String? accountCode,
    String? notes,
    String? createdById,
    String? createdAt,
    String? updatedAt,
  }) {
    return FinancialTransactionEntity(
      id: id ?? this.id,
      transactionNumber: transactionNumber ?? this.transactionNumber,
      type: type ?? this.type,
      category: category ?? this.category,
      amount: amount ?? this.amount,
      description: description ?? this.description,
      transactionDate: transactionDate ?? this.transactionDate,
      referenceId: referenceId ?? this.referenceId,
      referenceType: referenceType ?? this.referenceType,
      accountCode: accountCode ?? this.accountCode,
      notes: notes ?? this.notes,
      createdById: createdById ?? this.createdById,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  bool get isRevenue => type == TransactionType.revenue;
  bool get isExpense => type == TransactionType.expense;
  bool get isAsset => type == TransactionType.asset;
  bool get isLiability => type == TransactionType.liability;
  bool get isEquity => type == TransactionType.equity;

  double get amountInDollars => amount / 100.0;

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'transactionNumber': transactionNumber,
      'type': type.name,
      'category': category.name,
      'amount': amount,
      'description': description,
      'transactionDate': transactionDate.toIso8601String(),
      'referenceId': referenceId,
      'referenceType': referenceType,
      'accountCode': accountCode,
      'notes': notes,
      'createdById': createdById,
      'createdAt': createdAt,
      'updatedAt': updatedAt,
    };
  }

  @override
  List<Object?> get props => [
    id,
    transactionNumber,
    type,
    category,
    amount,
    description,
    transactionDate,
    referenceId,
    referenceType,
    accountCode,
    notes,
    createdById,
    createdAt,
    updatedAt,
  ];
}
