import 'package:equatable/equatable.dart';

enum LoyaltyTransactionType { earn, redeem, expire, bonus, adjustment }
enum LoyaltyPointsSource { purchase, referral, birthday, anniversary, promotion, bonus, manual }

class LoyaltyTransactionEntity extends Equatable {
  final int? id;
  final String customerId;
  final LoyaltyTransactionType type;
  final LoyaltyPointsSource source;
  final int points;
  final int? transactionId; // Reference to sales transaction
  final String? description;
  final DateTime createdAt;
  final DateTime? expiryDate;
  final bool isActive;
  final String? createdById;

  const LoyaltyTransactionEntity({
    this.id,
    required this.customerId,
    required this.type,
    required this.source,
    required this.points,
    this.transactionId,
    this.description,
    required this.createdAt,
    this.expiryDate,
    this.isActive = true,
    this.createdById,
  });

  LoyaltyTransactionEntity copyWith({
    int? id,
    String? customerId,
    LoyaltyTransactionType? type,
    LoyaltyPointsSource? source,
    int? points,
    int? transactionId,
    String? description,
    DateTime? createdAt,
    DateTime? expiryDate,
    bool? isActive,
    String? createdById,
  }) {
    return LoyaltyTransactionEntity(
      id: id ?? this.id,
      customerId: customerId ?? this.customerId,
      type: type ?? this.type,
      source: source ?? this.source,
      points: points ?? this.points,
      transactionId: transactionId ?? this.transactionId,
      description: description ?? this.description,
      createdAt: createdAt ?? this.createdAt,
      expiryDate: expiryDate ?? this.expiryDate,
      isActive: isActive ?? this.isActive,
      createdById: createdById ?? this.createdById,
    );
  }

  bool get isExpired => expiryDate != null && DateTime.now().isAfter(expiryDate!);
  bool get isExpiringSoon {
    if (expiryDate == null) return false;
    final daysUntilExpiry = expiryDate!.difference(DateTime.now()).inDays;
    return daysUntilExpiry <= 30 && daysUntilExpiry >= 0;
  }

  @override
  List<Object?> get props => [
    id,
    customerId,
    type,
    source,
    points,
    transactionId,
    description,
    createdAt,
    expiryDate,
    isActive,
    createdById,
  ];
}
