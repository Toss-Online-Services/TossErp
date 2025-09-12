import 'package:equatable/equatable.dart';

enum LocationType { store, warehouse, kiosk, popup, outlet, franchise }
enum LocationStatus { active, inactive, maintenance, closed, suspended }

class LocationEntity extends Equatable {
  final String id;
  final String name;
  final String code; // Unique location code (e.g., STORE001)
  final LocationType type;
  final LocationStatus status;
  final String address;
  final String city;
  final String region;
  final String country;
  final String? phone;
  final String? email;
  final String? managerId; // Employee ID of the location manager
  final Map<String, dynamic>? coordinates; // lat, lng for GPS
  final String? timezone;
  final Map<String, dynamic> operatingHours; // Business hours
  final List<String> paymentMethods; // Enabled payment methods for this location
  final Map<String, dynamic> settings; // Location-specific configurations
  final double? floorArea; // Square meters
  final int? maxCapacity; // Customer capacity
  final bool hasInventory; // Whether this location manages inventory
  final bool canTransferInventory; // Can send/receive transfers
  final List<String> connectedLocations; // Locations this can transfer to/from
  final DateTime createdAt;
  final DateTime? updatedAt;
  final String? parentLocationId; // For hierarchical structures
  final Map<String, dynamic>? taxConfiguration; // Location-specific tax settings
  final bool requiresAuthorization; // Needs HQ approval for operations
  final double? dailySalesLimit; // Maximum daily sales allowed
  final Map<String, dynamic>? customFields; // Additional business-specific data

  const LocationEntity({
    required this.id,
    required this.name,
    required this.code,
    required this.type,
    this.status = LocationStatus.active,
    required this.address,
    required this.city,
    required this.region,
    this.country = 'Ghana',
    this.phone,
    this.email,
    this.managerId,
    this.coordinates,
    this.timezone,
    this.operatingHours = const {
      'monday': {'open': '08:00', 'close': '18:00'},
      'tuesday': {'open': '08:00', 'close': '18:00'},
      'wednesday': {'open': '08:00', 'close': '18:00'},
      'thursday': {'open': '08:00', 'close': '18:00'},
      'friday': {'open': '08:00', 'close': '18:00'},
      'saturday': {'open': '09:00', 'close': '17:00'},
      'sunday': {'open': 'closed', 'close': 'closed'},
    },
    this.paymentMethods = const ['cash', 'card', 'mobileMoney'],
    this.settings = const {},
    this.floorArea,
    this.maxCapacity,
    this.hasInventory = true,
    this.canTransferInventory = true,
    this.connectedLocations = const [],
    required this.createdAt,
    this.updatedAt,
    this.parentLocationId,
    this.taxConfiguration,
    this.requiresAuthorization = false,
    this.dailySalesLimit,
    this.customFields,
  });

  LocationEntity copyWith({
    String? id,
    String? name,
    String? code,
    LocationType? type,
    LocationStatus? status,
    String? address,
    String? city,
    String? region,
    String? country,
    String? phone,
    String? email,
    String? managerId,
    Map<String, dynamic>? coordinates,
    String? timezone,
    Map<String, dynamic>? operatingHours,
    List<String>? paymentMethods,
    Map<String, dynamic>? settings,
    double? floorArea,
    int? maxCapacity,
    bool? hasInventory,
    bool? canTransferInventory,
    List<String>? connectedLocations,
    DateTime? createdAt,
    DateTime? updatedAt,
    String? parentLocationId,
    Map<String, dynamic>? taxConfiguration,
    bool? requiresAuthorization,
    double? dailySalesLimit,
    Map<String, dynamic>? customFields,
  }) {
    return LocationEntity(
      id: id ?? this.id,
      name: name ?? this.name,
      code: code ?? this.code,
      type: type ?? this.type,
      status: status ?? this.status,
      address: address ?? this.address,
      city: city ?? this.city,
      region: region ?? this.region,
      country: country ?? this.country,
      phone: phone ?? this.phone,
      email: email ?? this.email,
      managerId: managerId ?? this.managerId,
      coordinates: coordinates ?? this.coordinates,
      timezone: timezone ?? this.timezone,
      operatingHours: operatingHours ?? this.operatingHours,
      paymentMethods: paymentMethods ?? this.paymentMethods,
      settings: settings ?? this.settings,
      floorArea: floorArea ?? this.floorArea,
      maxCapacity: maxCapacity ?? this.maxCapacity,
      hasInventory: hasInventory ?? this.hasInventory,
      canTransferInventory: canTransferInventory ?? this.canTransferInventory,
      connectedLocations: connectedLocations ?? this.connectedLocations,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      parentLocationId: parentLocationId ?? this.parentLocationId,
      taxConfiguration: taxConfiguration ?? this.taxConfiguration,
      requiresAuthorization: requiresAuthorization ?? this.requiresAuthorization,
      dailySalesLimit: dailySalesLimit ?? this.dailySalesLimit,
      customFields: customFields ?? this.customFields,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'code': code,
      'type': type.name,
      'status': status.name,
      'address': address,
      'city': city,
      'region': region,
      'country': country,
      'phone': phone,
      'email': email,
      'managerId': managerId,
      'coordinates': coordinates,
      'timezone': timezone,
      'operatingHours': operatingHours,
      'paymentMethods': paymentMethods,
      'settings': settings,
      'floorArea': floorArea,
      'maxCapacity': maxCapacity,
      'hasInventory': hasInventory,
      'canTransferInventory': canTransferInventory,
      'connectedLocations': connectedLocations,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt?.toIso8601String(),
      'parentLocationId': parentLocationId,
      'taxConfiguration': taxConfiguration,
      'requiresAuthorization': requiresAuthorization,
      'dailySalesLimit': dailySalesLimit,
      'customFields': customFields,
    };
  }

  factory LocationEntity.fromJson(Map<String, dynamic> json) {
    return LocationEntity(
      id: json['id'],
      name: json['name'],
      code: json['code'],
      type: LocationType.values.firstWhere((e) => e.name == json['type']),
      status: LocationStatus.values.firstWhere((e) => e.name == json['status']),
      address: json['address'],
      city: json['city'],
      region: json['region'],
      country: json['country'] ?? 'Ghana',
      phone: json['phone'],
      email: json['email'],
      managerId: json['managerId'],
      coordinates: json['coordinates'],
      timezone: json['timezone'],
      operatingHours: json['operatingHours'] ?? const {},
      paymentMethods: List<String>.from(json['paymentMethods'] ?? []),
      settings: json['settings'] ?? const {},
      floorArea: json['floorArea']?.toDouble(),
      maxCapacity: json['maxCapacity'],
      hasInventory: json['hasInventory'] ?? true,
      canTransferInventory: json['canTransferInventory'] ?? true,
      connectedLocations: List<String>.from(json['connectedLocations'] ?? []),
      createdAt: DateTime.parse(json['createdAt']),
      updatedAt: json['updatedAt'] != null ? DateTime.parse(json['updatedAt']) : null,
      parentLocationId: json['parentLocationId'],
      taxConfiguration: json['taxConfiguration'],
      requiresAuthorization: json['requiresAuthorization'] ?? false,
      dailySalesLimit: json['dailySalesLimit']?.toDouble(),
      customFields: json['customFields'],
    );
  }

  bool get isOpen {
    if (status != LocationStatus.active) return false;
    
    final now = DateTime.now();
    final dayName = _getDayName(now.weekday);
    final todayHours = operatingHours[dayName];
    
    if (todayHours == null || 
        todayHours['open'] == 'closed' || 
        todayHours['close'] == 'closed') {
      return false;
    }
    
    try {
      final openTime = _parseTime(todayHours['open']);
      final closeTime = _parseTime(todayHours['close']);
      final currentTime = now.hour * 60 + now.minute;
      
      return currentTime >= openTime && currentTime <= closeTime;
    } catch (e) {
      return false;
    }
  }

  String get displayAddress => '$address, $city, $region';

  bool get isMainLocation => parentLocationId == null;

  bool get canReceiveTransfers => hasInventory && canTransferInventory;

  String _getDayName(int weekday) {
    const days = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday'];
    return days[weekday - 1];
  }

  int _parseTime(String timeStr) {
    final parts = timeStr.split(':');
    return int.parse(parts[0]) * 60 + int.parse(parts[1]);
  }

  @override
  List<Object?> get props => [
    id,
    name,
    code,
    type,
    status,
    address,
    city,
    region,
    country,
    phone,
    email,
    managerId,
    coordinates,
    timezone,
    operatingHours,
    paymentMethods,
    settings,
    floorArea,
    maxCapacity,
    hasInventory,
    canTransferInventory,
    connectedLocations,
    createdAt,
    updatedAt,
    parentLocationId,
    taxConfiguration,
    requiresAuthorization,
    dailySalesLimit,
    customFields,
  ];
}

// Transfer-related entities
enum TransferStatus { pending, approved, shipped, received, cancelled, rejected }
enum TransferType { stock, emergency, rebalance, return, damaged }

class InventoryTransferEntity extends Equatable {
  final String id;
  final String transferNumber; // Unique transfer identifier
  final String fromLocationId;
  final String toLocationId;
  final TransferStatus status;
  final TransferType type;
  final List<TransferItemEntity> items;
  final String? notes;
  final String requestedById; // Employee who requested the transfer
  final String? approvedById; // Employee who approved the transfer
  final String? shippedById; // Employee who shipped the transfer
  final String? receivedById; // Employee who received the transfer
  final DateTime requestedAt;
  final DateTime? approvedAt;
  final DateTime? shippedAt;
  final DateTime? receivedAt;
  final DateTime? cancelledAt;
  final String? cancellationReason;
  final int totalItems; // Total number of items being transferred
  final double totalValue; // Total value of items being transferred
  final Map<String, dynamic>? metadata; // Additional transfer data

  const InventoryTransferEntity({
    required this.id,
    required this.transferNumber,
    required this.fromLocationId,
    required this.toLocationId,
    this.status = TransferStatus.pending,
    required this.type,
    required this.items,
    this.notes,
    required this.requestedById,
    this.approvedById,
    this.shippedById,
    this.receivedById,
    required this.requestedAt,
    this.approvedAt,
    this.shippedAt,
    this.receivedAt,
    this.cancelledAt,
    this.cancellationReason,
    required this.totalItems,
    required this.totalValue,
    this.metadata,
  });

  @override
  List<Object?> get props => [
    id,
    transferNumber,
    fromLocationId,
    toLocationId,
    status,
    type,
    items,
    notes,
    requestedById,
    approvedById,
    shippedById,
    receivedById,
    requestedAt,
    approvedAt,
    shippedAt,
    receivedAt,
    cancelledAt,
    cancellationReason,
    totalItems,
    totalValue,
    metadata,
  ];
}

class TransferItemEntity extends Equatable {
  final String id;
  final String productId;
  final String? batchId;
  final int requestedQuantity;
  final int? shippedQuantity;
  final int? receivedQuantity;
  final double unitCost;
  final DateTime? expiryDate;
  final String? serialNumbers; // For serialized items
  final String? notes;

  const TransferItemEntity({
    required this.id,
    required this.productId,
    this.batchId,
    required this.requestedQuantity,
    this.shippedQuantity,
    this.receivedQuantity,
    required this.unitCost,
    this.expiryDate,
    this.serialNumbers,
    this.notes,
  });

  @override
  List<Object?> get props => [
    id,
    productId,
    batchId,
    requestedQuantity,
    shippedQuantity,
    receivedQuantity,
    unitCost,
    expiryDate,
    serialNumbers,
    notes,
  ];
}
