import '../../../domain/entities/customer_entity.dart';
import '../../../domain/entities/loyalty_transaction_entity.dart';
import '../../../domain/entities/loyalty_program_entity.dart';

class LoyaltyService {
  static const Map<CustomerTier, LoyaltyTierConfigEntity> defaultTierConfigs = {
    CustomerTier.bronze: LoyaltyTierConfigEntity(
      tier: CustomerTier.bronze,
      name: 'Bronze',
      color: '#CD7F32',
      spendingThreshold: 0,
      pointsMultiplier: 1.0,
      discountPercentage: 0.0,
      benefits: ['Basic loyalty points', 'Birthday bonus'],
    ),
    CustomerTier.silver: LoyaltyTierConfigEntity(
      tier: CustomerTier.silver,
      name: 'Silver',
      color: '#C0C0C0',
      spendingThreshold: 10000, // $100 in cents
      pointsMultiplier: 1.2,
      discountPercentage: 2.0,
      benefits: ['20% more points', '2% discount', 'Birthday bonus', 'Anniversary bonus'],
    ),
    CustomerTier.gold: LoyaltyTierConfigEntity(
      tier: CustomerTier.gold,
      name: 'Gold',
      color: '#FFD700',
      spendingThreshold: 50000, // $500 in cents
      pointsMultiplier: 1.5,
      discountPercentage: 5.0,
      benefits: ['50% more points', '5% discount', 'Priority support', 'Special promotions'],
    ),
    CustomerTier.platinum: LoyaltyTierConfigEntity(
      tier: CustomerTier.platinum,
      name: 'Platinum',
      color: '#E5E4E2',
      spendingThreshold: 100000, // $1000 in cents
      pointsMultiplier: 2.0,
      discountPercentage: 8.0,
      benefits: ['Double points', '8% discount', 'Exclusive events', 'Personal shopping'],
    ),
    CustomerTier.vip: LoyaltyTierConfigEntity(
      tier: CustomerTier.vip,
      name: 'VIP',
      color: '#800080',
      spendingThreshold: 250000, // $2500 in cents
      pointsMultiplier: 2.5,
      discountPercentage: 10.0,
      benefits: ['2.5x points', '10% discount', 'VIP events', 'Concierge service'],
    ),
  };

  static final LoyaltyProgramEntity defaultProgram = LoyaltyProgramEntity(
    id: 1,
    name: 'TOSS Rewards',
    description: 'Earn points with every purchase and enjoy exclusive benefits',
    tierConfigs: defaultTierConfigs,
    pointsPerCent: 1, // 1 point per cent spent
    pointsValue: 1, // 1 point = 1 cent value
    pointsExpiryDays: 365, // Points expire after 1 year
    enableReferrals: true,
    referralPoints: 500, // 500 points for successful referral
    enableBirthdayBonus: true,
    birthdayBonusPoints: 200,
    enableAnniversaryBonus: true,
    anniversaryBonusPoints: 300,
  );

  /// Calculate points earned for a purchase
  static int calculatePointsEarned(int amountInCents, CustomerTier tier, [LoyaltyProgramEntity? program]) {
    final loyaltyProgram = program ?? defaultProgram;
    return loyaltyProgram.calculatePointsForPurchase(amountInCents, tier);
  }

  /// Calculate customer tier based on total spending
  static CustomerTier calculateCustomerTier(double totalSpent, [LoyaltyProgramEntity? program]) {
    final loyaltyProgram = program ?? defaultProgram;
    return loyaltyProgram.calculateTier(totalSpent);
  }

  /// Get tier configuration for a specific tier
  static LoyaltyTierConfigEntity getTierConfig(CustomerTier tier, [LoyaltyProgramEntity? program]) {
    final loyaltyProgram = program ?? defaultProgram;
    return loyaltyProgram.tierConfigs[tier] ?? defaultTierConfigs[tier]!;
  }

  /// Calculate points value in cents
  static int calculatePointsValue(int points, [LoyaltyProgramEntity? program]) {
    final loyaltyProgram = program ?? defaultProgram;
    return points * loyaltyProgram.pointsValue;
  }

  /// Check if customer qualifies for tier upgrade
  static bool qualifiesForTierUpgrade(CustomerEntity customer, [LoyaltyProgramEntity? program]) {
    final loyaltyProgram = program ?? defaultProgram;
    final calculatedTier = loyaltyProgram.calculateTier(customer.totalSpent);
    return calculatedTier.index > customer.tier.index;
  }

  /// Get next tier requirements
  static Map<String, dynamic>? getNextTierRequirements(CustomerTier currentTier, double currentSpent, [LoyaltyProgramEntity? program]) {
    final loyaltyProgram = program ?? defaultProgram;
    
    // Find the next tier
    final tiers = CustomerTier.values;
    final currentIndex = tiers.indexOf(currentTier);
    if (currentIndex >= tiers.length - 1) return null; // Already at highest tier
    
    final nextTier = tiers[currentIndex + 1];
    final nextTierConfig = loyaltyProgram.tierConfigs[nextTier];
    if (nextTierConfig?.spendingThreshold == null) return null;
    
    final amountNeeded = nextTierConfig!.spendingThreshold! - currentSpent;
    
    return {
      'nextTier': nextTier,
      'nextTierConfig': nextTierConfig,
      'amountNeeded': amountNeeded,
      'progress': currentSpent / nextTierConfig.spendingThreshold!,
    };
  }

  /// Create a loyalty transaction for points earned
  static LoyaltyTransactionEntity createEarnTransaction({
    required String customerId,
    required int points,
    required LoyaltyPointsSource source,
    int? transactionId,
    String? description,
    int? expiryDays,
  }) {
    return LoyaltyTransactionEntity(
      customerId: customerId,
      type: LoyaltyTransactionType.earn,
      source: source,
      points: points,
      transactionId: transactionId,
      description: description,
      createdAt: DateTime.now(),
      expiryDate: expiryDays != null 
          ? DateTime.now().add(Duration(days: expiryDays))
          : null,
    );
  }

  /// Create a loyalty transaction for points redeemed
  static LoyaltyTransactionEntity createRedeemTransaction({
    required String customerId,
    required int points,
    int? transactionId,
    String? description,
  }) {
    return LoyaltyTransactionEntity(
      customerId: customerId,
      type: LoyaltyTransactionType.redeem,
      source: LoyaltyPointsSource.purchase, // Default source for redemption
      points: -points, // Negative for redemption
      transactionId: transactionId,
      description: description,
      createdAt: DateTime.now(),
    );
  }

  /// Check if customer has sufficient points for redemption
  static bool hasSufficientPoints(CustomerEntity customer, int pointsToRedeem) {
    return customer.pointsBalance >= pointsToRedeem;
  }

  /// Calculate discount from tier
  static int calculateTierDiscount(int amountInCents, CustomerTier tier, [LoyaltyProgramEntity? program]) {
    final loyaltyProgram = program ?? defaultProgram;
    final tierConfig = loyaltyProgram.tierConfigs[tier];
    if (tierConfig == null) return 0;
    
    return (amountInCents * tierConfig.discountPercentage / 100).round();
  }

  /// Format points display
  static String formatPoints(int points) {
    if (points >= 1000000) {
      return '${(points / 1000000).toStringAsFixed(1)}M pts';
    } else if (points >= 1000) {
      return '${(points / 1000).toStringAsFixed(1)}K pts';
    } else {
      return '$points pts';
    }
  }

  /// Format tier display name
  static String formatTierName(CustomerTier tier) {
    return getTierConfig(tier).name;
  }

  /// Get tier color
  static String getTierColor(CustomerTier tier) {
    return getTierConfig(tier).color;
  }
}
