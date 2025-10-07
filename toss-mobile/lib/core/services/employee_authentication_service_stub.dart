// Temporary stub to allow compilation
class EmployeeAuthenticationService {
  Future<List<String>> getAvailableAuthMethods() async {
    return ['pin'];
  }
  
  Future<String> authenticateWithPin(String employeeNumber, String pin) async {
    return 'success';
  }
  
  Future<String> authenticateWithBiometric({String? employeeId}) async {
    return 'success';
  }
  
  Future<String> authenticateWithBadge(String badgeId) async {
    return 'success';
  }
  
  Future<bool> setupBiometricForEmployee(String employeeId) async {
    return true;
  }
  
  String generateEmployeeQRCode(String employeeId) {
    return 'QR_CODE_DATA_FOR_$employeeId';
  }
}

enum AuthenticationMethod { pin, biometric, badge }
