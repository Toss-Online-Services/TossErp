import 'package:equatable/equatable.dart';

enum ExpenseStatus { pending, approved, paid, rejected }
enum ExpenseType {
  operating,
  marketing,
  rent,
  utilities,
  salaries,
  insurance,
  maintenance,
  supplies,
  travel,
  other,
}

class ExpenseEntity extends Equatable {
  final int? id;
  final String expenseNumber;
  final String description;
  final ExpenseType type;
  final ExpenseStatus status;
  final int amount; // in cents
  final DateTime expenseDate;
  final DateTime? dueDate;
  final String? vendor;
  final String? invoiceNumber;
  final String? receiptUrl;
  final String? category;
  final String? notes;
  final String? approvedBy;
  final DateTime? approvedAt;
  final String? paidBy;
  final DateTime? paidAt;
  final String? createdById;
  final String? createdAt;
  final String? updatedAt;

  const ExpenseEntity({
    this.id,
    required this.expenseNumber,
    required this.description,
    required this.type,
    this.status = ExpenseStatus.pending,
    required this.amount,
    required this.expenseDate,
    this.dueDate,
    this.vendor,
    this.invoiceNumber,
    this.receiptUrl,
    this.category,
    this.notes,
    this.approvedBy,
    this.approvedAt,
    this.paidBy,
    this.paidAt,
    this.createdById,
    this.createdAt,
    this.updatedAt,
  });

  ExpenseEntity copyWith({
    int? id,
    String? expenseNumber,
    String? description,
    ExpenseType? type,
    ExpenseStatus? status,
    int? amount,
    DateTime? expenseDate,
    DateTime? dueDate,
    String? vendor,
    String? invoiceNumber,
    String? receiptUrl,
    String? category,
    String? notes,
    String? approvedBy,
    DateTime? approvedAt,
    String? paidBy,
    DateTime? paidAt,
    String? createdById,
    String? createdAt,
    String? updatedAt,
  }) {
    return ExpenseEntity(
      id: id ?? this.id,
      expenseNumber: expenseNumber ?? this.expenseNumber,
      description: description ?? this.description,
      type: type ?? this.type,
      status: status ?? this.status,
      amount: amount ?? this.amount,
      expenseDate: expenseDate ?? this.expenseDate,
      dueDate: dueDate ?? this.dueDate,
      vendor: vendor ?? this.vendor,
      invoiceNumber: invoiceNumber ?? this.invoiceNumber,
      receiptUrl: receiptUrl ?? this.receiptUrl,
      category: category ?? this.category,
      notes: notes ?? this.notes,
      approvedBy: approvedBy ?? this.approvedBy,
      approvedAt: approvedAt ?? this.approvedAt,
      paidBy: paidBy ?? this.paidBy,
      paidAt: paidAt ?? this.paidAt,
      createdById: createdById ?? this.createdById,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  bool get isPending => status == ExpenseStatus.pending;
  bool get isApproved => status == ExpenseStatus.approved;
  bool get isPaid => status == ExpenseStatus.paid;
  bool get isRejected => status == ExpenseStatus.rejected;
  bool get isOverdue => dueDate != null && DateTime.now().isAfter(dueDate!) && !isPaid;

  double get amountInDollars => amount / 100.0;

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'expenseNumber': expenseNumber,
      'description': description,
      'type': type.name,
      'status': status.name,
      'amount': amount,
      'expenseDate': expenseDate.toIso8601String(),
      'dueDate': dueDate?.toIso8601String(),
      'vendor': vendor,
      'invoiceNumber': invoiceNumber,
      'receiptUrl': receiptUrl,
      'category': category,
      'notes': notes,
      'approvedBy': approvedBy,
      'approvedAt': approvedAt?.toIso8601String(),
      'paidBy': paidBy,
      'paidAt': paidAt?.toIso8601String(),
      'createdById': createdById,
      'createdAt': createdAt,
      'updatedAt': updatedAt,
    };
  }

  @override
  List<Object?> get props => [
    id,
    expenseNumber,
    description,
    type,
    status,
    amount,
    expenseDate,
    dueDate,
    vendor,
    invoiceNumber,
    receiptUrl,
    category,
    notes,
    approvedBy,
    approvedAt,
    paidBy,
    paidAt,
    createdById,
    createdAt,
    updatedAt,
  ];
}
