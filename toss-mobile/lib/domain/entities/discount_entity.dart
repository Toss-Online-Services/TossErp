import 'package:equatable/equatable.dart';

// Discount Types
enum DiscountType {
  percentage,
  fixedAmount,
  bogo, // Buy One Get One
  buyXGetY,
  freeShipping,
  customerSpecific,
  loyaltyPoints,
}

// Discount Scope
enum DiscountScope {
  item,
  category,
  brand,
  cart,
  shipping,
  total,
}

// Discount Application Rules
enum DiscountApplication {
  automatic,
  couponCode,
  loyaltyTier,
  customerGroup,
  manual,
}

// BOGO Types
enum BogoType {
  buyOneGetOne,
  buyOneGetOneFree,
  buyTwoGetOne,
  buyXGetY,
  buyXGetYPercent,
}

// Main Discount Entity
class DiscountEntity extends Equatable {
  final String id;
  final String name;
  final String description;
  final DiscountType type;
  final DiscountScope scope;
  final DiscountApplication application;
  final double value; // Percentage (0-100) or fixed amount
  final double? minimumPurchase;
  final double? maximumDiscount;
  final int? maxUsesPerCustomer;
  final int? totalMaxUses;
  final int currentUses;
  final DateTime startDate;
  final DateTime endDate;
  final bool isActive;
  final bool isStackable;
  final int priority; // Higher number = higher priority
  final List<String> applicableItems; // Product IDs
  final List<String> applicableCategories; // Category IDs
  final List<String> applicableBrands; // Brand IDs
  final List<String> excludedItems; // Excluded product IDs
  final List<String> applicableCustomers; // Customer IDs
  final List<String> applicableCustomerGroups; // Customer group IDs
  final List<String> applicableLoyaltyTiers; // Loyalty tier IDs
  final String? couponCode;
  final BogoConfig? bogoConfig;
  final TimeBasedConfig? timeConfig;
  final LocationConfig? locationConfig;
  final Map<String, dynamic> metadata;
  final DateTime createdAt;
  final DateTime? updatedAt;
  final String createdBy;

  const DiscountEntity({
    required this.id,
    required this.name,
    required this.description,
    required this.type,
    required this.scope,
    required this.application,
    required this.value,
    this.minimumPurchase,
    this.maximumDiscount,
    this.maxUsesPerCustomer,
    this.totalMaxUses,
    this.currentUses = 0,
    required this.startDate,
    required this.endDate,
    this.isActive = true,
    this.isStackable = false,
    this.priority = 0,
    this.applicableItems = const [],
    this.applicableCategories = const [],
    this.applicableBrands = const [],
    this.excludedItems = const [],
    this.applicableCustomers = const [],
    this.applicableCustomerGroups = const [],
    this.applicableLoyaltyTiers = const [],
    this.couponCode,
    this.bogoConfig,
    this.timeConfig,
    this.locationConfig,
    this.metadata = const {},
    required this.createdAt,
    this.updatedAt,
    required this.createdBy,
  });

  @override
  List<Object?> get props => [
        id,
        name,
        description,
        type,
        scope,
        application,
        value,
        minimumPurchase,
        maximumDiscount,
        maxUsesPerCustomer,
        totalMaxUses,
        currentUses,
        startDate,
        endDate,
        isActive,
        isStackable,
        priority,
        applicableItems,
        applicableCategories,
        applicableBrands,
        excludedItems,
        applicableCustomers,
        applicableCustomerGroups,
        applicableLoyaltyTiers,
        couponCode,
        bogoConfig,
        timeConfig,
        locationConfig,
        metadata,
        createdAt,
        updatedAt,
        createdBy,
      ];

  DiscountEntity copyWith({
    String? id,
    String? name,
    String? description,
    DiscountType? type,
    DiscountScope? scope,
    DiscountApplication? application,
    double? value,
    double? minimumPurchase,
    double? maximumDiscount,
    int? maxUsesPerCustomer,
    int? totalMaxUses,
    int? currentUses,
    DateTime? startDate,
    DateTime? endDate,
    bool? isActive,
    bool? isStackable,
    int? priority,
    List<String>? applicableItems,
    List<String>? applicableCategories,
    List<String>? applicableBrands,
    List<String>? excludedItems,
    List<String>? applicableCustomers,
    List<String>? applicableCustomerGroups,
    List<String>? applicableLoyaltyTiers,
    String? couponCode,
    BogoConfig? bogoConfig,
    TimeBasedConfig? timeConfig,
    LocationConfig? locationConfig,
    Map<String, dynamic>? metadata,
    DateTime? createdAt,
    DateTime? updatedAt,
    String? createdBy,
  }) {
    return DiscountEntity(
      id: id ?? this.id,
      name: name ?? this.name,
      description: description ?? this.description,
      type: type ?? this.type,
      scope: scope ?? this.scope,
      application: application ?? this.application,
      value: value ?? this.value,
      minimumPurchase: minimumPurchase ?? this.minimumPurchase,
      maximumDiscount: maximumDiscount ?? this.maximumDiscount,
      maxUsesPerCustomer: maxUsesPerCustomer ?? this.maxUsesPerCustomer,
      totalMaxUses: totalMaxUses ?? this.totalMaxUses,
      currentUses: currentUses ?? this.currentUses,
      startDate: startDate ?? this.startDate,
      endDate: endDate ?? this.endDate,
      isActive: isActive ?? this.isActive,
      isStackable: isStackable ?? this.isStackable,
      priority: priority ?? this.priority,
      applicableItems: applicableItems ?? this.applicableItems,
      applicableCategories: applicableCategories ?? this.applicableCategories,
      applicableBrands: applicableBrands ?? this.applicableBrands,
      excludedItems: excludedItems ?? this.excludedItems,
      applicableCustomers: applicableCustomers ?? this.applicableCustomers,
      applicableCustomerGroups: applicableCustomerGroups ?? this.applicableCustomerGroups,
      applicableLoyaltyTiers: applicableLoyaltyTiers ?? this.applicableLoyaltyTiers,
      couponCode: couponCode ?? this.couponCode,
      bogoConfig: bogoConfig ?? this.bogoConfig,
      timeConfig: timeConfig ?? this.timeConfig,
      locationConfig: locationConfig ?? this.locationConfig,
      metadata: metadata ?? this.metadata,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      createdBy: createdBy ?? this.createdBy,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
      'type': type.name,
      'scope': scope.name,
      'application': application.name,
      'value': value,
      'minimumPurchase': minimumPurchase,
      'maximumDiscount': maximumDiscount,
      'maxUsesPerCustomer': maxUsesPerCustomer,
      'totalMaxUses': totalMaxUses,
      'currentUses': currentUses,
      'startDate': startDate.toIso8601String(),
      'endDate': endDate.toIso8601String(),
      'isActive': isActive,
      'isStackable': isStackable,
      'priority': priority,
      'applicableItems': applicableItems,
      'applicableCategories': applicableCategories,
      'applicableBrands': applicableBrands,
      'excludedItems': excludedItems,
      'applicableCustomers': applicableCustomers,
      'applicableCustomerGroups': applicableCustomerGroups,
      'applicableLoyaltyTiers': applicableLoyaltyTiers,
      'couponCode': couponCode,
      'bogoConfig': bogoConfig?.toJson(),
      'timeConfig': timeConfig?.toJson(),
      'locationConfig': locationConfig?.toJson(),
      'metadata': metadata,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt?.toIso8601String(),
      'createdBy': createdBy,
    };
  }

  factory DiscountEntity.fromJson(Map<String, dynamic> json) {
    return DiscountEntity(
      id: json['id'],
      name: json['name'],
      description: json['description'],
      type: DiscountType.values.firstWhere((e) => e.name == json['type']),
      scope: DiscountScope.values.firstWhere((e) => e.name == json['scope']),
      application: DiscountApplication.values.firstWhere((e) => e.name == json['application']),
      value: json['value'].toDouble(),
      minimumPurchase: json['minimumPurchase']?.toDouble(),
      maximumDiscount: json['maximumDiscount']?.toDouble(),
      maxUsesPerCustomer: json['maxUsesPerCustomer'],
      totalMaxUses: json['totalMaxUses'],
      currentUses: json['currentUses'] ?? 0,
      startDate: DateTime.parse(json['startDate']),
      endDate: DateTime.parse(json['endDate']),
      isActive: json['isActive'] ?? true,
      isStackable: json['isStackable'] ?? false,
      priority: json['priority'] ?? 0,
      applicableItems: List<String>.from(json['applicableItems'] ?? []),
      applicableCategories: List<String>.from(json['applicableCategories'] ?? []),
      applicableBrands: List<String>.from(json['applicableBrands'] ?? []),
      excludedItems: List<String>.from(json['excludedItems'] ?? []),
      applicableCustomers: List<String>.from(json['applicableCustomers'] ?? []),
      applicableCustomerGroups: List<String>.from(json['applicableCustomerGroups'] ?? []),
      applicableLoyaltyTiers: List<String>.from(json['applicableLoyaltyTiers'] ?? []),
      couponCode: json['couponCode'],
      bogoConfig: json['bogoConfig'] != null ? BogoConfig.fromJson(json['bogoConfig']) : null,
      timeConfig: json['timeConfig'] != null ? TimeBasedConfig.fromJson(json['timeConfig']) : null,
      locationConfig: json['locationConfig'] != null ? LocationConfig.fromJson(json['locationConfig']) : null,
      metadata: Map<String, dynamic>.from(json['metadata'] ?? {}),
      createdAt: DateTime.parse(json['createdAt']),
      updatedAt: json['updatedAt'] != null ? DateTime.parse(json['updatedAt']) : null,
      createdBy: json['createdBy'],
    );
  }

  // Validation methods
  bool isValidForDate(DateTime date) {
    return date.isAfter(startDate) && date.isBefore(endDate) && isActive;
  }

  bool isValidForCustomer(String customerId) {
    if (applicableCustomers.isEmpty) return true;
    return applicableCustomers.contains(customerId);
  }

  bool canBeUsed(int currentCustomerUses) {
    if (maxUsesPerCustomer != null && currentCustomerUses >= maxUsesPerCustomer!) {
      return false;
    }
    if (totalMaxUses != null && currentUses >= totalMaxUses!) {
      return false;
    }
    return true;
  }
}

// BOGO Configuration
class BogoConfig extends Equatable {
  final BogoType type;
  final int buyQuantity;
  final int getQuantity;
  final double getDiscountPercent; // For buyXGetYPercent
  final bool requireSameProduct;
  final List<String> getProducts; // Specific products for "get" items
  final String? getCategory; // Category for "get" items
  final double? maxGetValue; // Maximum value for free items

  const BogoConfig({
    required this.type,
    required this.buyQuantity,
    required this.getQuantity,
    this.getDiscountPercent = 100, // 100% = free
    this.requireSameProduct = true,
    this.getProducts = const [],
    this.getCategory,
    this.maxGetValue,
  });

  @override
  List<Object?> get props => [
        type,
        buyQuantity,
        getQuantity,
        getDiscountPercent,
        requireSameProduct,
        getProducts,
        getCategory,
        maxGetValue,
      ];

  Map<String, dynamic> toJson() {
    return {
      'type': type.name,
      'buyQuantity': buyQuantity,
      'getQuantity': getQuantity,
      'getDiscountPercent': getDiscountPercent,
      'requireSameProduct': requireSameProduct,
      'getProducts': getProducts,
      'getCategory': getCategory,
      'maxGetValue': maxGetValue,
    };
  }

  factory BogoConfig.fromJson(Map<String, dynamic> json) {
    return BogoConfig(
      type: BogoType.values.firstWhere((e) => e.name == json['type']),
      buyQuantity: json['buyQuantity'],
      getQuantity: json['getQuantity'],
      getDiscountPercent: json['getDiscountPercent']?.toDouble() ?? 100,
      requireSameProduct: json['requireSameProduct'] ?? true,
      getProducts: List<String>.from(json['getProducts'] ?? []),
      getCategory: json['getCategory'],
      maxGetValue: json['maxGetValue']?.toDouble(),
    );
  }
}

// Time-Based Configuration
class TimeBasedConfig extends Equatable {
  final List<int> daysOfWeek; // 1-7 (Monday-Sunday)
  final TimeOfDay? startTime;
  final TimeOfDay? endTime;
  final List<String> excludedDates; // ISO date strings
  final bool isHappyHour;
  final String? seasonalPeriod; // e.g., "summer", "winter", "holiday"

  const TimeBasedConfig({
    this.daysOfWeek = const [],
    this.startTime,
    this.endTime,
    this.excludedDates = const [],
    this.isHappyHour = false,
    this.seasonalPeriod,
  });

  @override
  List<Object?> get props => [
        daysOfWeek,
        startTime,
        endTime,
        excludedDates,
        isHappyHour,
        seasonalPeriod,
      ];

  Map<String, dynamic> toJson() {
    return {
      'daysOfWeek': daysOfWeek,
      'startTime': startTime != null ? '${startTime!.hour}:${startTime!.minute}' : null,
      'endTime': endTime != null ? '${endTime!.hour}:${endTime!.minute}' : null,
      'excludedDates': excludedDates,
      'isHappyHour': isHappyHour,
      'seasonalPeriod': seasonalPeriod,
    };
  }

  factory TimeBasedConfig.fromJson(Map<String, dynamic> json) {
    TimeOfDay? parseTime(String? timeString) {
      if (timeString == null) return null;
      final parts = timeString.split(':');
      return TimeOfDay(hour: int.parse(parts[0]), minute: int.parse(parts[1]));
    }

    return TimeBasedConfig(
      daysOfWeek: List<int>.from(json['daysOfWeek'] ?? []),
      startTime: parseTime(json['startTime']),
      endTime: parseTime(json['endTime']),
      excludedDates: List<String>.from(json['excludedDates'] ?? []),
      isHappyHour: json['isHappyHour'] ?? false,
      seasonalPeriod: json['seasonalPeriod'],
    );
  }

  bool isValidForDateTime(DateTime dateTime) {
    // Check day of week
    if (daysOfWeek.isNotEmpty && !daysOfWeek.contains(dateTime.weekday)) {
      return false;
    }

    // Check time range
    if (startTime != null && endTime != null) {
      final currentTime = TimeOfDay.fromDateTime(dateTime);
      final start = startTime!;
      final end = endTime!;

      // Handle overnight time ranges
      if (start.hour > end.hour || (start.hour == end.hour && start.minute > end.minute)) {
        // Overnight range
        return (currentTime.hour > start.hour || 
                (currentTime.hour == start.hour && currentTime.minute >= start.minute)) ||
               (currentTime.hour < end.hour || 
                (currentTime.hour == end.hour && currentTime.minute <= end.minute));
      } else {
        // Same day range
        return (currentTime.hour > start.hour || 
                (currentTime.hour == start.hour && currentTime.minute >= start.minute)) &&
               (currentTime.hour < end.hour || 
                (currentTime.hour == end.hour && currentTime.minute <= end.minute));
      }
    }

    // Check excluded dates
    final dateString = dateTime.toIso8601String().split('T')[0];
    if (excludedDates.contains(dateString)) {
      return false;
    }

    return true;
  }
}

// Helper class for TimeOfDay conversion
class TimeOfDay {
  final int hour;
  final int minute;

  const TimeOfDay({required this.hour, required this.minute});

  factory TimeOfDay.fromDateTime(DateTime dateTime) {
    return TimeOfDay(hour: dateTime.hour, minute: dateTime.minute);
  }

  @override
  String toString() => '${hour.toString().padLeft(2, '0')}:${minute.toString().padLeft(2, '0')}';
}

// Location Configuration
class LocationConfig extends Equatable {
  final List<String> applicableLocations; // Location IDs
  final List<String> excludedLocations; // Excluded location IDs
  final bool isLocationSpecific;

  const LocationConfig({
    this.applicableLocations = const [],
    this.excludedLocations = const [],
    this.isLocationSpecific = false,
  });

  @override
  List<Object?> get props => [
        applicableLocations,
        excludedLocations,
        isLocationSpecific,
      ];

  Map<String, dynamic> toJson() {
    return {
      'applicableLocations': applicableLocations,
      'excludedLocations': excludedLocations,
      'isLocationSpecific': isLocationSpecific,
    };
  }

  factory LocationConfig.fromJson(Map<String, dynamic> json) {
    return LocationConfig(
      applicableLocations: List<String>.from(json['applicableLocations'] ?? []),
      excludedLocations: List<String>.from(json['excludedLocations'] ?? []),
      isLocationSpecific: json['isLocationSpecific'] ?? false,
    );
  }

  bool isValidForLocation(String locationId) {
    if (!isLocationSpecific) return true;
    
    if (excludedLocations.contains(locationId)) return false;
    
    if (applicableLocations.isEmpty) return true;
    return applicableLocations.contains(locationId);
  }
}

// Applied Discount (for tracking applied discounts on transactions)
class AppliedDiscount extends Equatable {
  final String discountId;
  final String discountName;
  final DiscountType type;
  final double originalValue;
  final double appliedValue;
  final double discountAmount;
  final List<String> appliedToItems; // Item IDs
  final String? couponCode;
  final DateTime appliedAt;
  final Map<String, dynamic> metadata;

  const AppliedDiscount({
    required this.discountId,
    required this.discountName,
    required this.type,
    required this.originalValue,
    required this.appliedValue,
    required this.discountAmount,
    this.appliedToItems = const [],
    this.couponCode,
    required this.appliedAt,
    this.metadata = const {},
  });

  @override
  List<Object?> get props => [
        discountId,
        discountName,
        type,
        originalValue,
        appliedValue,
        discountAmount,
        appliedToItems,
        couponCode,
        appliedAt,
        metadata,
      ];

  Map<String, dynamic> toJson() {
    return {
      'discountId': discountId,
      'discountName': discountName,
      'type': type.name,
      'originalValue': originalValue,
      'appliedValue': appliedValue,
      'discountAmount': discountAmount,
      'appliedToItems': appliedToItems,
      'couponCode': couponCode,
      'appliedAt': appliedAt.toIso8601String(),
      'metadata': metadata,
    };
  }

  factory AppliedDiscount.fromJson(Map<String, dynamic> json) {
    return AppliedDiscount(
      discountId: json['discountId'],
      discountName: json['discountName'],
      type: DiscountType.values.firstWhere((e) => e.name == json['type']),
      originalValue: json['originalValue'].toDouble(),
      appliedValue: json['appliedValue'].toDouble(),
      discountAmount: json['discountAmount'].toDouble(),
      appliedToItems: List<String>.from(json['appliedToItems'] ?? []),
      couponCode: json['couponCode'],
      appliedAt: DateTime.parse(json['appliedAt']),
      metadata: Map<String, dynamic>.from(json['metadata'] ?? {}),
    );
  }
}

// Discount Usage Tracking
class DiscountUsage extends Equatable {
  final String id;
  final String discountId;
  final String? customerId;
  final String transactionId;
  final double discountAmount;
  final String? couponCode;
  final DateTime usedAt;
  final String locationId;
  final Map<String, dynamic> metadata;

  const DiscountUsage({
    required this.id,
    required this.discountId,
    this.customerId,
    required this.transactionId,
    required this.discountAmount,
    this.couponCode,
    required this.usedAt,
    required this.locationId,
    this.metadata = const {},
  });

  @override
  List<Object?> get props => [
        id,
        discountId,
        customerId,
        transactionId,
        discountAmount,
        couponCode,
        usedAt,
        locationId,
        metadata,
      ];

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'discountId': discountId,
      'customerId': customerId,
      'transactionId': transactionId,
      'discountAmount': discountAmount,
      'couponCode': couponCode,
      'usedAt': usedAt.toIso8601String(),
      'locationId': locationId,
      'metadata': metadata,
    };
  }

  factory DiscountUsage.fromJson(Map<String, dynamic> json) {
    return DiscountUsage(
      id: json['id'],
      discountId: json['discountId'],
      customerId: json['customerId'],
      transactionId: json['transactionId'],
      discountAmount: json['discountAmount'].toDouble(),
      couponCode: json['couponCode'],
      usedAt: DateTime.parse(json['usedAt']),
      locationId: json['locationId'],
      metadata: Map<String, dynamic>.from(json['metadata'] ?? {}),
    );
  }
}


