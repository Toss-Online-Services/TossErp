import 'dart:convert';
import 'package:crypto/crypto.dart';
import 'package:local_auth/local_auth.dart';

import '../entities/employee_entity.dart' as domain;
import '../../domain/entities/employee_entity.dart';

class AuthenticationResult {
  final bool success;
  final domain.EmployeeEntity? employee;
  final String? error;
  final AuthenticationMethod method;

  const AuthenticationResult({
    required this.success,
    this.employee,
    this.error,
    required this.method,
  });

  factory AuthenticationResult.success(domain.EmployeeEntity employee, AuthenticationMethod method) {
    return AuthenticationResult(
      success: true,
      employee: employee,
      method: method,
    );
  }

  factory AuthenticationResult.failure(String error, AuthenticationMethod method) {
    return AuthenticationResult(
      success: false,
      error: error,
      method: method,
    );
  }
}

enum AuthenticationMethod {
  pin,
  biometric,
  employeeNumber,
  qrCode,
  nfcCard,
}

class EmployeeAuthenticationService {
  final LocalAuthentication _localAuth = LocalAuthentication();
  static domain.EmployeeEntity? _currentEmployee;
  static WorkShift? _currentShift;

  // Current session
  static domain.EmployeeEntity? get currentEmployee => _currentEmployee;
  static WorkShift? get currentShift => _currentShift;
  static bool get isLoggedIn => _currentEmployee != null;

  /// Authenticate employee using PIN
  Future<AuthenticationResult> authenticateWithPin(String employeeNumber, String pin) async {
    try {
      // Hash the PIN for comparison
      final hashedPin = _hashPin(pin);
      
      // TODO: Get employee from repository
      final employee = await _getEmployeeByNumber(employeeNumber);
      
      if (employee == null) {
        return AuthenticationResult.failure(
          'Employee not found',
          AuthenticationMethod.pin,
        );
      }

      if (employee.status != EmployeeStatus.active) {
        return AuthenticationResult.failure(
          'Employee account is not active',
          AuthenticationMethod.pin,
        );
      }

      if (employee.pin == null || employee.pin != hashedPin) {
        return AuthenticationResult.failure(
          'Invalid PIN',
          AuthenticationMethod.pin,
        );
      }

      await _setCurrentEmployee(employee);
      return AuthenticationResult.success(employee, AuthenticationMethod.pin);
      
    } catch (e) {
      return AuthenticationResult.failure(
        'Authentication failed: $e',
        AuthenticationMethod.pin,
      );
    }
  }

  /// Authenticate employee using biometric data
  Future<AuthenticationResult> authenticateWithBiometric([String? employeeId]) async {
    try {
      final isAvailable = await _localAuth.isDeviceSupported();
      if (!isAvailable) {
        return AuthenticationResult.failure(
          'Biometric authentication not available on this device',
          AuthenticationMethod.biometric,
        );
      }

      final availableBiometrics = await _localAuth.getAvailableBiometrics();
      if (availableBiometrics.isEmpty) {
        return AuthenticationResult.failure(
          'No biometric methods configured on this device',
          AuthenticationMethod.biometric,
        );
      }

      // Perform biometric authentication
      final isAuthenticated = await _localAuth.authenticate(
        localizedReason: 'Authenticate to access the POS system',
        options: const AuthenticationOptions(
          biometricOnly: false,
          stickyAuth: true,
        ),
      );

      if (!isAuthenticated) {
        return AuthenticationResult.failure(
          'Biometric authentication failed',
          AuthenticationMethod.biometric,
        );
      }

      // If specific employee ID provided, verify they have biometric setup
      if (employeeId != null) {
        final employee = await _getEmployeeById(employeeId);
        if (employee == null) {
          return AuthenticationResult.failure(
            'Employee not found',
            AuthenticationMethod.biometric,
          );
        }

        if (employee.biometricType == domain.BiometricType.none || employee.biometricId == null) {
          return AuthenticationResult.failure(
            'Biometric authentication not setup for this employee',
            AuthenticationMethod.biometric,
          );
        }

        await _setCurrentEmployee(employee);
        return AuthenticationResult.success(employee, AuthenticationMethod.biometric);
      }

      // TODO: Match biometric with employee database
      // For now, return a mock successful authentication
      final mockEmployee = domain.EmployeeEntity(
        id: '1',
        employeeNumber: 'EMP001',
        name: 'John Doe',
        phone: '+1234567890',
        role: UserRole.cashier,
        permissions: PermissionSet.forRole(UserRole.cashier),
        biometricType: domain.BiometricType.fingerprint,
        biometricId: 'bio_123',
        hireDate: DateTime.now().subtract(const Duration(days: 30)),
        createdAt: DateTime.now(),
        updatedAt: DateTime.now(),
      );

      await _setCurrentEmployee(mockEmployee);
      return AuthenticationResult.success(mockEmployee, AuthenticationMethod.biometric);

    } catch (e) {
      return AuthenticationResult.failure(
        'Biometric authentication error: $e',
        AuthenticationMethod.biometric,
      );
    }
  }

  /// Authenticate employee using employee number only (for manager override)
  Future<AuthenticationResult> authenticateWithEmployeeNumber(String employeeNumber) async {
    try {
      final employee = await _getEmployeeByNumber(employeeNumber);
      
      if (employee == null) {
        return AuthenticationResult.failure(
          'Employee not found',
          AuthenticationMethod.employeeNumber,
        );
      }

      if (employee.status != EmployeeStatus.active) {
        return AuthenticationResult.failure(
          'Employee account is not active',
          AuthenticationMethod.employeeNumber,
        );
      }

      // Only allow for manager+ roles
      if (employee.role != UserRole.manager && employee.role != UserRole.owner) {
        return AuthenticationResult.failure(
          'Insufficient permissions for employee number authentication',
          AuthenticationMethod.employeeNumber,
        );
      }

      await _setCurrentEmployee(employee);
      return AuthenticationResult.success(employee, AuthenticationMethod.employeeNumber);
      
    } catch (e) {
      return AuthenticationResult.failure(
        'Authentication failed: $e',
        AuthenticationMethod.employeeNumber,
      );
    }
  }

  /// Authenticate employee using QR code
  Future<AuthenticationResult> authenticateWithQRCode(String qrData) async {
    try {
      // Parse QR code data (expected format: "EMP:employeeId:signature")
      final parts = qrData.split(':');
      if (parts.length != 3 || parts[0] != 'EMP') {
        return AuthenticationResult.failure(
          'Invalid QR code format',
          AuthenticationMethod.qrCode,
        );
      }

      final employeeId = parts[1];
      final signature = parts[2];

      // Verify signature (basic implementation)
      final expectedSignature = _generateEmployeeSignature(employeeId);
      if (signature != expectedSignature) {
        return AuthenticationResult.failure(
          'Invalid QR code signature',
          AuthenticationMethod.qrCode,
        );
      }

      final employee = await _getEmployeeById(employeeId);
      if (employee == null) {
        return AuthenticationResult.failure(
          'Employee not found',
          AuthenticationMethod.qrCode,
        );
      }

      if (employee.status != EmployeeStatus.active) {
        return AuthenticationResult.failure(
          'Employee account is not active',
          AuthenticationMethod.qrCode,
        );
      }

      await _setCurrentEmployee(employee);
      return AuthenticationResult.success(employee, AuthenticationMethod.qrCode);
      
    } catch (e) {
      return AuthenticationResult.failure(
        'QR code authentication failed: $e',
        AuthenticationMethod.qrCode,
      );
    }
  }

  /// Start work shift for current employee
  Future<bool> startShift(String locationId) async {
    if (_currentEmployee == null) return false;

    try {
      final shift = WorkShift(
        id: DateTime.now().millisecondsSinceEpoch.toString(),
        employeeId: _currentEmployee!.id,
        startTime: DateTime.now(),
        locationId: locationId,
      );

      _currentShift = shift;

      // TODO: Save shift to database
      return true;
    } catch (e) {
      return false;
    }
  }

  /// End work shift for current employee
  Future<bool> endShift({int? breakDuration, Map<String, dynamic>? metrics}) async {
    if (_currentShift == null) return false;

    try {
      _currentShift = _currentShift!.copyWith(
        endTime: DateTime.now(),
        breakDuration: breakDuration,
        metrics: metrics ?? {},
      );

      // TODO: Save completed shift to database
      return true;
    } catch (e) {
      return false;
    }
  }

  /// Log out current employee
  Future<void> logout() async {
    // End shift if active
    if (_currentShift?.isActive == true) {
      await endShift();
    }
    
    _currentEmployee = null;
    _currentShift = null;
  }

  /// Check if current employee has specific permission
  bool hasPermission(bool Function(PermissionSet) permissionCheck) {
    if (_currentEmployee == null) return false;
    return permissionCheck(_currentEmployee!.permissions);
  }

  /// Get available authentication methods for device
  Future<List<AuthenticationMethod>> getAvailableAuthMethods() async {
    final methods = <AuthenticationMethod>[
      AuthenticationMethod.pin,
      AuthenticationMethod.employeeNumber,
      AuthenticationMethod.qrCode,
    ];

    try {
      final isAvailable = await _localAuth.isDeviceSupported();
      if (isAvailable) {
        final availableBiometrics = await _localAuth.getAvailableBiometrics();
        if (availableBiometrics.isNotEmpty) {
          methods.insert(0, AuthenticationMethod.biometric);
        }
      }
    } catch (e) {
      // Biometric not available
    }

    return methods;
  }

  /// Setup biometric authentication for employee
  Future<bool> setupBiometricForEmployee(String employeeId) async {
    try {
      final isAvailable = await _localAuth.isDeviceSupported();
      if (!isAvailable) return false;

      final isAuthenticated = await _localAuth.authenticate(
        localizedReason: 'Setup biometric authentication for employee',
        options: const AuthenticationOptions(
          biometricOnly: true,
          stickyAuth: true,
        ),
      );

      if (!isAuthenticated) return false;

      // TODO: Store biometric template and associate with employee
      // This would typically involve encrypting and storing the biometric data
      // For now, we'll just generate a unique ID

      final biometricId = _generateBiometricId(employeeId);
      
      // TODO: Update employee record with biometric info
      return true;

    } catch (e) {
      return false;
    }
  }

  /// Generate QR code data for employee authentication
  String generateEmployeeQRCode(String employeeId) {
    final signature = _generateEmployeeSignature(employeeId);
    return 'EMP:$employeeId:$signature';
  }

  // Private helper methods

  Future<domain.EmployeeEntity?> _getEmployeeByNumber(String employeeNumber) async {
    // TODO: Implement database query
    // Mock data for now
    return domain.EmployeeEntity(
      id: '1',
      employeeNumber: employeeNumber,
      name: 'John Doe',
      phone: '+1234567890',
      role: UserRole.cashier,
      permissions: PermissionSet.forRole(UserRole.cashier),
      pin: _hashPin('1234'),
      hireDate: DateTime.now().subtract(const Duration(days: 30)),
      createdAt: DateTime.now(),
      updatedAt: DateTime.now(),
    );
  }

  Future<domain.EmployeeEntity?> _getEmployeeById(String employeeId) async {
    // TODO: Implement database query
    // Mock data for now
    return domain.EmployeeEntity(
      id: employeeId,
      employeeNumber: 'EMP001',
      name: 'John Doe',
      phone: '+1234567890',
      role: UserRole.cashier,
      permissions: PermissionSet.forRole(UserRole.cashier),
      hireDate: DateTime.now().subtract(const Duration(days: 30)),
      createdAt: DateTime.now(),
      updatedAt: DateTime.now(),
    );
  }

  Future<void> _setCurrentEmployee(domain.EmployeeEntity employee) async {
    _currentEmployee = employee;
    // TODO: Log authentication event
  }

  String _hashPin(String pin) {
    final bytes = utf8.encode(pin);
    final digest = sha256.convert(bytes);
    return digest.toString();
  }

  String _generateEmployeeSignature(String employeeId) {
    // Simple signature generation - in production, use proper cryptographic signing
    final data = 'EMP$employeeId${DateTime.now().day}';
    final bytes = utf8.encode(data);
    final digest = sha256.convert(bytes);
    return digest.toString().substring(0, 8);
  }

  String _generateBiometricId(String employeeId) {
    final timestamp = DateTime.now().millisecondsSinceEpoch;
    return 'bio_${employeeId}_$timestamp';
  }
}

// Extension method for WorkShift copyWith
extension WorkShiftExtension on WorkShift {
  WorkShift copyWith({
    String? id,
    String? employeeId,
    DateTime? startTime,
    DateTime? endTime,
    String? locationId,
    int? breakDuration,
    Map<String, dynamic>? metrics,
  }) {
    return WorkShift(
      id: id ?? this.id,
      employeeId: employeeId ?? this.employeeId,
      startTime: startTime ?? this.startTime,
      endTime: endTime ?? this.endTime,
      locationId: locationId ?? this.locationId,
      breakDuration: breakDuration ?? this.breakDuration,
      metrics: metrics ?? this.metrics,
    );
  }
}
