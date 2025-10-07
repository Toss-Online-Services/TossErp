enum UserRole {
  owner,
  manager,
  supervisor,
  cashier,
  stockClerk,
  trainee,
}

enum EmployeeStatus {
  active,
  inactive,
  terminated,
  onLeave,
}

enum BiometricType {
  none,
  fingerprint,
  faceId,
  iris,
}

class PermissionSet {
  // Sales permissions
  final bool canProcessSales;
  final bool canProcessReturns;
  final bool canApplyDiscounts;
  final bool canVoidTransactions;
  final bool canAccessCustomerData;
  final bool canModifyPrices;
  
  // Inventory permissions
  final bool canManageInventory;
  final bool canReceiveStock;
  final bool canAdjustStock;
  final bool canTransferStock;
  final bool canViewInventoryReports;
  
  // Employee management permissions
  final bool canManageEmployees;
  final bool canViewEmployeeReports;
  final bool canModifyPermissions;
  final bool canAccessTimeTracking;
  
  // System permissions
  final bool canAccessReports;
  final bool canExportData;
  final bool canAccessSystemSettings;
  final bool canManageLocations;
  final bool canBackupRestore;
  
  // Cash management permissions
  final bool canOpenCashDrawer;
  final bool canPerformCashCount;
  final bool canManageCashDrops;
  final bool canViewCashReports;

  const PermissionSet({
    this.canProcessSales = false,
    this.canProcessReturns = false,
    this.canApplyDiscounts = false,
    this.canVoidTransactions = false,
    this.canAccessCustomerData = false,
    this.canModifyPrices = false,
    this.canManageInventory = false,
    this.canReceiveStock = false,
    this.canAdjustStock = false,
    this.canTransferStock = false,
    this.canViewInventoryReports = false,
    this.canManageEmployees = false,
    this.canViewEmployeeReports = false,
    this.canModifyPermissions = false,
    this.canAccessTimeTracking = false,
    this.canAccessReports = false,
    this.canExportData = false,
    this.canAccessSystemSettings = false,
    this.canManageLocations = false,
    this.canBackupRestore = false,
    this.canOpenCashDrawer = false,
    this.canPerformCashCount = false,
    this.canManageCashDrops = false,
    this.canViewCashReports = false,
  });

  // Predefined permission sets for common roles
  static const PermissionSet owner = PermissionSet(
    canProcessSales: true,
    canProcessReturns: true,
    canApplyDiscounts: true,
    canVoidTransactions: true,
    canAccessCustomerData: true,
    canModifyPrices: true,
    canManageInventory: true,
    canReceiveStock: true,
    canAdjustStock: true,
    canTransferStock: true,
    canViewInventoryReports: true,
    canManageEmployees: true,
    canViewEmployeeReports: true,
    canModifyPermissions: true,
    canAccessTimeTracking: true,
    canAccessReports: true,
    canExportData: true,
    canAccessSystemSettings: true,
    canManageLocations: true,
    canBackupRestore: true,
    canOpenCashDrawer: true,
    canPerformCashCount: true,
    canManageCashDrops: true,
    canViewCashReports: true,
  );

  static const PermissionSet manager = PermissionSet(
    canProcessSales: true,
    canProcessReturns: true,
    canApplyDiscounts: true,
    canVoidTransactions: true,
    canAccessCustomerData: true,
    canModifyPrices: true,
    canManageInventory: true,
    canReceiveStock: true,
    canAdjustStock: true,
    canTransferStock: true,
    canViewInventoryReports: true,
    canManageEmployees: false,
    canViewEmployeeReports: true,
    canModifyPermissions: false,
    canAccessTimeTracking: true,
    canAccessReports: true,
    canExportData: true,
    canAccessSystemSettings: false,
    canManageLocations: false,
    canBackupRestore: false,
    canOpenCashDrawer: true,
    canPerformCashCount: true,
    canManageCashDrops: true,
    canViewCashReports: true,
  );

  static const PermissionSet supervisor = PermissionSet(
    canProcessSales: true,
    canProcessReturns: true,
    canApplyDiscounts: true,
    canVoidTransactions: true,
    canAccessCustomerData: true,
    canModifyPrices: false,
    canManageInventory: true,
    canReceiveStock: true,
    canAdjustStock: true,
    canTransferStock: false,
    canViewInventoryReports: true,
    canManageEmployees: false,
    canViewEmployeeReports: false,
    canModifyPermissions: false,
    canAccessTimeTracking: true,
    canAccessReports: false,
    canExportData: false,
    canAccessSystemSettings: false,
    canManageLocations: false,
    canBackupRestore: false,
    canOpenCashDrawer: true,
    canPerformCashCount: true,
    canManageCashDrops: false,
    canViewCashReports: false,
  );

  static const PermissionSet cashier = PermissionSet(
    canProcessSales: true,
    canProcessReturns: true,
    canApplyDiscounts: false,
    canVoidTransactions: false,
    canAccessCustomerData: true,
    canModifyPrices: false,
    canManageInventory: false,
    canReceiveStock: false,
    canAdjustStock: false,
    canTransferStock: false,
    canViewInventoryReports: false,
    canManageEmployees: false,
    canViewEmployeeReports: false,
    canModifyPermissions: false,
    canAccessTimeTracking: false,
    canAccessReports: false,
    canExportData: false,
    canAccessSystemSettings: false,
    canManageLocations: false,
    canBackupRestore: false,
    canOpenCashDrawer: true,
    canPerformCashCount: false,
    canManageCashDrops: false,
    canViewCashReports: false,
  );

  static const PermissionSet stockClerk = PermissionSet(
    canProcessSales: false,
    canProcessReturns: false,
    canApplyDiscounts: false,
    canVoidTransactions: false,
    canAccessCustomerData: false,
    canModifyPrices: false,
    canManageInventory: true,
    canReceiveStock: true,
    canAdjustStock: true,
    canTransferStock: true,
    canViewInventoryReports: true,
    canManageEmployees: false,
    canViewEmployeeReports: false,
    canModifyPermissions: false,
    canAccessTimeTracking: false,
    canAccessReports: false,
    canExportData: false,
    canAccessSystemSettings: false,
    canManageLocations: false,
    canBackupRestore: false,
    canOpenCashDrawer: false,
    canPerformCashCount: false,
    canManageCashDrops: false,
    canViewCashReports: false,
  );

  static const PermissionSet trainee = PermissionSet(
    canProcessSales: false,
    canProcessReturns: false,
    canApplyDiscounts: false,
    canVoidTransactions: false,
    canAccessCustomerData: false,
    canModifyPrices: false,
    canManageInventory: false,
    canReceiveStock: false,
    canAdjustStock: false,
    canTransferStock: false,
    canViewInventoryReports: false,
    canManageEmployees: false,
    canViewEmployeeReports: false,
    canModifyPermissions: false,
    canAccessTimeTracking: false,
    canAccessReports: false,
    canExportData: false,
    canAccessSystemSettings: false,
    canManageLocations: false,
    canBackupRestore: false,
    canOpenCashDrawer: false,
    canPerformCashCount: false,
    canManageCashDrops: false,
    canViewCashReports: false,
  );

  static PermissionSet forRole(UserRole role) {
    switch (role) {
      case UserRole.owner:
        return owner;
      case UserRole.manager:
        return manager;
      case UserRole.supervisor:
        return supervisor;
      case UserRole.cashier:
        return cashier;
      case UserRole.stockClerk:
        return stockClerk;
      case UserRole.trainee:
        return trainee;
    }
  }

  Map<String, dynamic> toMap() {
    return {
      'canProcessSales': canProcessSales,
      'canProcessReturns': canProcessReturns,
      'canApplyDiscounts': canApplyDiscounts,
      'canVoidTransactions': canVoidTransactions,
      'canAccessCustomerData': canAccessCustomerData,
      'canModifyPrices': canModifyPrices,
      'canManageInventory': canManageInventory,
      'canReceiveStock': canReceiveStock,
      'canAdjustStock': canAdjustStock,
      'canTransferStock': canTransferStock,
      'canViewInventoryReports': canViewInventoryReports,
      'canManageEmployees': canManageEmployees,
      'canViewEmployeeReports': canViewEmployeeReports,
      'canModifyPermissions': canModifyPermissions,
      'canAccessTimeTracking': canAccessTimeTracking,
      'canAccessReports': canAccessReports,
      'canExportData': canExportData,
      'canAccessSystemSettings': canAccessSystemSettings,
      'canManageLocations': canManageLocations,
      'canBackupRestore': canBackupRestore,
      'canOpenCashDrawer': canOpenCashDrawer,
      'canPerformCashCount': canPerformCashCount,
      'canManageCashDrops': canManageCashDrops,
      'canViewCashReports': canViewCashReports,
    };
  }

  factory PermissionSet.fromMap(Map<String, dynamic> map) {
    return PermissionSet(
      canProcessSales: map['canProcessSales'] ?? false,
      canProcessReturns: map['canProcessReturns'] ?? false,
      canApplyDiscounts: map['canApplyDiscounts'] ?? false,
      canVoidTransactions: map['canVoidTransactions'] ?? false,
      canAccessCustomerData: map['canAccessCustomerData'] ?? false,
      canModifyPrices: map['canModifyPrices'] ?? false,
      canManageInventory: map['canManageInventory'] ?? false,
      canReceiveStock: map['canReceiveStock'] ?? false,
      canAdjustStock: map['canAdjustStock'] ?? false,
      canTransferStock: map['canTransferStock'] ?? false,
      canViewInventoryReports: map['canViewInventoryReports'] ?? false,
      canManageEmployees: map['canManageEmployees'] ?? false,
      canViewEmployeeReports: map['canViewEmployeeReports'] ?? false,
      canModifyPermissions: map['canModifyPermissions'] ?? false,
      canAccessTimeTracking: map['canAccessTimeTracking'] ?? false,
      canAccessReports: map['canAccessReports'] ?? false,
      canExportData: map['canExportData'] ?? false,
      canAccessSystemSettings: map['canAccessSystemSettings'] ?? false,
      canManageLocations: map['canManageLocations'] ?? false,
      canBackupRestore: map['canBackupRestore'] ?? false,
      canOpenCashDrawer: map['canOpenCashDrawer'] ?? false,
      canPerformCashCount: map['canPerformCashCount'] ?? false,
      canManageCashDrops: map['canManageCashDrops'] ?? false,
      canViewCashReports: map['canViewCashReports'] ?? false,
    );
  }
}

class WorkShift {
  final String id;
  final String employeeId;
  final DateTime startTime;
  final DateTime? endTime;
  final String locationId;
  final int? breakDuration; // in minutes
  final Map<String, dynamic> metrics; // sales, transactions, etc.

  const WorkShift({
    required this.id,
    required this.employeeId,
    required this.startTime,
    this.endTime,
    required this.locationId,
    this.breakDuration,
    this.metrics = const {},
  });

  Duration? get totalWorkTime {
    if (endTime == null) return null;
    final duration = endTime!.difference(startTime);
    if (breakDuration != null) {
      return duration - Duration(minutes: breakDuration!);
    }
    return duration;
  }

  bool get isActive => endTime == null;

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'employeeId': employeeId,
      'startTime': startTime.toIso8601String(),
      'endTime': endTime?.toIso8601String(),
      'locationId': locationId,
      'breakDuration': breakDuration,
      'metrics': metrics,
    };
  }

  factory WorkShift.fromMap(Map<String, dynamic> map) {
    return WorkShift(
      id: map['id'],
      employeeId: map['employeeId'],
      startTime: DateTime.parse(map['startTime']),
      endTime: map['endTime'] != null ? DateTime.parse(map['endTime']) : null,
      locationId: map['locationId'],
      breakDuration: map['breakDuration'],
      metrics: Map<String, dynamic>.from(map['metrics'] ?? {}),
    );
  }
}

class EmployeeEntity {
  final String id;
  final String employeeNumber;
  final String name;
  final String phone;
  final String email;
  final UserRole role;
  final EmployeeStatus status;
  final PermissionSet permissions;
  final String? pin; // 4-6 digit PIN for quick login
  final String? biometricId; // Reference to stored biometric data
  final BiometricType biometricType;
  final List<String> locationIds; // Locations this employee can work at
  final double hourlyRate;
  final DateTime hireDate;
  final DateTime? terminationDate;
  final WorkShift? currentShift;
  final Map<String, dynamic> performanceMetrics;
  final bool isTrainingMode;
  final DateTime createdAt;
  final DateTime updatedAt;

  const EmployeeEntity({
    required this.id,
    required this.employeeNumber,
    required this.name,
    required this.phone,
    this.email = '',
    required this.role,
    this.status = EmployeeStatus.active,
    required this.permissions,
    this.pin,
    this.biometricId,
    this.biometricType = BiometricType.none,
    this.locationIds = const [],
    this.hourlyRate = 0.0,
    required this.hireDate,
    this.terminationDate,
    this.currentShift,
    this.performanceMetrics = const {},
    this.isTrainingMode = false,
    required this.createdAt,
    required this.updatedAt,
  });

  bool get isActive => status == EmployeeStatus.active;
  bool canWorkAtLocation(String locationId) => locationIds.contains(locationId);
  bool get isCurrentlyWorking => currentShift?.isActive == true;

  String get displayName => name.isNotEmpty ? name : employeeNumber;
  String get roleDisplayName {
    switch (role) {
      case UserRole.owner:
        return 'Owner';
      case UserRole.manager:
        return 'Manager';
      case UserRole.supervisor:
        return 'Supervisor';
      case UserRole.cashier:
        return 'Cashier';
      case UserRole.stockClerk:
        return 'Stock Clerk';
      case UserRole.trainee:
        return 'Trainee';
    }
  }

  EmployeeEntity copyWith({
    String? id,
    String? employeeNumber,
    String? name,
    String? phone,
    String? email,
    UserRole? role,
    EmployeeStatus? status,
    PermissionSet? permissions,
    String? pin,
    String? biometricId,
    BiometricType? biometricType,
    List<String>? locationIds,
    double? hourlyRate,
    DateTime? hireDate,
    DateTime? terminationDate,
    WorkShift? currentShift,
    Map<String, dynamic>? performanceMetrics,
    bool? isTrainingMode,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) {
    return EmployeeEntity(
      id: id ?? this.id,
      employeeNumber: employeeNumber ?? this.employeeNumber,
      name: name ?? this.name,
      phone: phone ?? this.phone,
      email: email ?? this.email,
      role: role ?? this.role,
      status: status ?? this.status,
      permissions: permissions ?? this.permissions,
      pin: pin ?? this.pin,
      biometricId: biometricId ?? this.biometricId,
      biometricType: biometricType ?? this.biometricType,
      locationIds: locationIds ?? this.locationIds,
      hourlyRate: hourlyRate ?? this.hourlyRate,
      hireDate: hireDate ?? this.hireDate,
      terminationDate: terminationDate ?? this.terminationDate,
      currentShift: currentShift ?? this.currentShift,
      performanceMetrics: performanceMetrics ?? this.performanceMetrics,
      isTrainingMode: isTrainingMode ?? this.isTrainingMode,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'employeeNumber': employeeNumber,
      'name': name,
      'phone': phone,
      'email': email,
      'role': role.index,
      'status': status.index,
      'permissions': permissions.toMap(),
      'pin': pin,
      'biometricId': biometricId,
      'biometricType': biometricType.index,
      'locationIds': locationIds,
      'hourlyRate': hourlyRate,
      'hireDate': hireDate.toIso8601String(),
      'terminationDate': terminationDate?.toIso8601String(),
      'currentShift': currentShift?.toMap(),
      'performanceMetrics': performanceMetrics,
      'isTrainingMode': isTrainingMode,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt.toIso8601String(),
    };
  }

  factory EmployeeEntity.fromMap(Map<String, dynamic> map) {
    return EmployeeEntity(
      id: map['id'],
      employeeNumber: map['employeeNumber'],
      name: map['name'],
      phone: map['phone'],
      email: map['email'] ?? '',
      role: UserRole.values[map['role']],
      status: EmployeeStatus.values[map['status'] ?? 0],
      permissions: PermissionSet.fromMap(Map<String, dynamic>.from(map['permissions'] ?? {})),
      pin: map['pin'],
      biometricId: map['biometricId'],
      biometricType: BiometricType.values[map['biometricType'] ?? 0],
      locationIds: List<String>.from(map['locationIds'] ?? []),
      hourlyRate: map['hourlyRate']?.toDouble() ?? 0.0,
      hireDate: DateTime.parse(map['hireDate']),
      terminationDate: map['terminationDate'] != null ? DateTime.parse(map['terminationDate']) : null,
      currentShift: map['currentShift'] != null ? WorkShift.fromMap(Map<String, dynamic>.from(map['currentShift'])) : null,
      performanceMetrics: Map<String, dynamic>.from(map['performanceMetrics'] ?? {}),
      isTrainingMode: map['isTrainingMode'] ?? false,
      createdAt: DateTime.parse(map['createdAt']),
      updatedAt: DateTime.parse(map['updatedAt']),
    );
  }

  // Legacy constructor for backward compatibility
  factory EmployeeEntity.create({
    required String name,
    required String phone,
    String? email,
    required UserRole role,
    String? pin,
  }) {
    final now = DateTime.now();
    return EmployeeEntity(
      id: now.millisecondsSinceEpoch.toString(),
      employeeNumber: 'EMP${now.millisecondsSinceEpoch.toString().substring(8)}',
      name: name,
      phone: phone,
      email: email ?? '',
      role: role,
      permissions: PermissionSet.forRole(role),
      pin: pin,
      hireDate: now,
      createdAt: now,
      updatedAt: now,
    );
  }
}
