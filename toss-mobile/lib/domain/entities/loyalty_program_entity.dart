import 'package:equatable/equatable.dart';
import 'customer_entity.dart';

class LoyaltyProgramEntity extends Equatable {
  final int? id;
  final String name;
  final String? description;
  final bool isActive;
  final Map<CustomerTier, LoyaltyTierConfigEntity> tierConfigs;
  final int pointsPerCent; // Points earned per cent spent
  final int pointsValue; // Value of 1 point in cents
  final int? pointsExpiryDays; // Days before points expire
  final bool enableReferrals;
  final int? referralPoints;
  final bool enableBirthdayBonus;
  final int? birthdayBonusPoints;
  final bool enableAnniversaryBonus;
  final int? anniversaryBonusPoints;
  final String? createdById;
  final String? createdAt;
  final String? updatedAt;

  const LoyaltyProgramEntity({
    this.id,
    required this.name,
    this.description,
    this.isActive = true,
    this.tierConfigs = const {},
    this.pointsPerCent = 1,
    this.pointsValue = 1,
    this.pointsExpiryDays,
    this.enableReferrals = true,
    this.referralPoints = 100,
    this.enableBirthdayBonus = true,
    this.birthdayBonusPoints = 50,
    this.enableAnniversaryBonus = true,
    this.anniversaryBonusPoints = 100,
    this.createdById,
    this.createdAt,
    this.updatedAt,
  });

  LoyaltyProgramEntity copyWith({
    int? id,
    String? name,
    String? description,
    bool? isActive,
    Map<CustomerTier, LoyaltyTierConfigEntity>? tierConfigs,
    int? pointsPerCent,
    int? pointsValue,
    int? pointsExpiryDays,
    bool? enableReferrals,
    int? referralPoints,
    bool? enableBirthdayBonus,
    int? birthdayBonusPoints,
    bool? enableAnniversaryBonus,
    int? anniversaryBonusPoints,
    String? createdById,
    String? createdAt,
    String? updatedAt,
  }) {
    return LoyaltyProgramEntity(
      id: id ?? this.id,
      name: name ?? this.name,
      description: description ?? this.description,
      isActive: isActive ?? this.isActive,
      tierConfigs: tierConfigs ?? this.tierConfigs,
      pointsPerCent: pointsPerCent ?? this.pointsPerCent,
      pointsValue: pointsValue ?? this.pointsValue,
      pointsExpiryDays: pointsExpiryDays ?? this.pointsExpiryDays,
      enableReferrals: enableReferrals ?? this.enableReferrals,
      referralPoints: referralPoints ?? this.referralPoints,
      enableBirthdayBonus: enableBirthdayBonus ?? this.enableBirthdayBonus,
      birthdayBonusPoints: birthdayBonusPoints ?? this.birthdayBonusPoints,
      enableAnniversaryBonus: enableAnniversaryBonus ?? this.enableAnniversaryBonus,
      anniversaryBonusPoints: anniversaryBonusPoints ?? this.anniversaryBonusPoints,
      createdById: createdById ?? this.createdById,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  int calculatePointsForPurchase(int amountInCents, CustomerTier tier) {
    final tierConfig = tierConfigs[tier];
    final multiplier = tierConfig?.pointsMultiplier ?? 1.0;
    return ((amountInCents * pointsPerCent * multiplier) / 100).round();
  }

  CustomerTier calculateTier(double totalSpent) {
    // Sort tiers by spending threshold
    final sortedTiers = tierConfigs.entries.toList()
      ..sort((a, b) => (a.value.spendingThreshold ?? 0).compareTo(b.value.spendingThreshold ?? 0));
    
    for (int i = sortedTiers.length - 1; i >= 0; i--) {
      final tierEntry = sortedTiers[i];
      if (totalSpent >= (tierEntry.value.spendingThreshold ?? 0)) {
        return tierEntry.key;
      }
    }
    
    return CustomerTier.bronze; // Default tier
  }

  @override
  List<Object?> get props => [
    id,
    name,
    description,
    isActive,
    tierConfigs,
    pointsPerCent,
    pointsValue,
    pointsExpiryDays,
    enableReferrals,
    referralPoints,
    enableBirthdayBonus,
    birthdayBonusPoints,
    enableAnniversaryBonus,
    anniversaryBonusPoints,
    createdById,
    createdAt,
    updatedAt,
  ];
}

class LoyaltyTierConfigEntity extends Equatable {
  final CustomerTier tier;
  final String name;
  final String color;
  final double? spendingThreshold; // Amount to reach this tier
  final double pointsMultiplier; // Points multiplier for this tier
  final double discountPercentage; // Additional discount for this tier
  final List<String> benefits;

  const LoyaltyTierConfigEntity({
    required this.tier,
    required this.name,
    required this.color,
    this.spendingThreshold,
    this.pointsMultiplier = 1.0,
    this.discountPercentage = 0.0,
    this.benefits = const [],
  });

  LoyaltyTierConfigEntity copyWith({
    CustomerTier? tier,
    String? name,
    String? color,
    double? spendingThreshold,
    double? pointsMultiplier,
    double? discountPercentage,
    List<String>? benefits,
  }) {
    return LoyaltyTierConfigEntity(
      tier: tier ?? this.tier,
      name: name ?? this.name,
      color: color ?? this.color,
      spendingThreshold: spendingThreshold ?? this.spendingThreshold,
      pointsMultiplier: pointsMultiplier ?? this.pointsMultiplier,
      discountPercentage: discountPercentage ?? this.discountPercentage,
      benefits: benefits ?? this.benefits,
    );
  }

  @override
  List<Object?> get props => [
    tier,
    name,
    color,
    spendingThreshold,
    pointsMultiplier,
    discountPercentage,
    benefits,
  ];
}
