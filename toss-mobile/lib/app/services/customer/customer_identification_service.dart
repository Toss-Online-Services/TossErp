import 'package:crypto/crypto.dart';
import 'dart:convert';
import '../../../domain/entities/customer_entity.dart';

class CustomerIdentificationService {
  static const String _secretKey = 'your-app-secret-key'; // Should be in secure storage

  /// Generate a QR code for customer identification
  static String generateCustomerQRCode(String customerId) {
    final timestamp = DateTime.now().millisecondsSinceEpoch;
    final data = '$customerId:$timestamp';
    final bytes = utf8.encode(data + _secretKey);
    final digest = sha256.convert(bytes);
    return '$data:${digest.toString().substring(0, 8)}';
  }

  /// Validate a QR code and extract customer ID
  static String? validateCustomerQRCode(String qrCode) {
    try {
      final parts = qrCode.split(':');
      if (parts.length != 3) return null;
      
      final customerId = parts[0];
      final timestamp = int.parse(parts[1]);
      final hash = parts[2];
      
      // Check if QR code is not too old (24 hours)
      final now = DateTime.now().millisecondsSinceEpoch;
      if (now - timestamp > 24 * 60 * 60 * 1000) return null;
      
      // Validate hash
      final data = '$customerId:$timestamp';
      final bytes = utf8.encode(data + _secretKey);
      final digest = sha256.convert(bytes);
      final expectedHash = digest.toString().substring(0, 8);
      
      if (hash == expectedHash) return customerId;
      return null;
    } catch (e) {
      return null;
    }
  }

  /// Generate a membership card number
  static String generateMembershipNumber(String prefix) {
    final timestamp = DateTime.now().millisecondsSinceEpoch.toString();
    final random = (timestamp.hashCode % 10000).toString().padLeft(4, '0');
    return '$prefix$random';
  }

  /// Validate phone number format (basic validation)
  static bool isValidPhoneNumber(String phone) {
    // Basic phone validation - should be enhanced based on local requirements
    final phoneRegex = RegExp(r'^\+?[\d\s\-\(\)]{8,15}$');
    return phoneRegex.hasMatch(phone.trim());
  }

  /// Generate customer ID from phone number
  static String generateCustomerIdFromPhone(String phone) {
    final cleanPhone = phone.replaceAll(RegExp(r'[^\d]'), '');
    final bytes = utf8.encode(cleanPhone + _secretKey);
    final digest = sha256.convert(bytes);
    return digest.toString().substring(0, 16);
  }

  /// Search customers by multiple criteria
  static bool matchesSearchCriteria(CustomerEntity customer, String searchTerm) {
    final term = searchTerm.toLowerCase().trim();
    
    return (customer.name?.toLowerCase().contains(term) ?? false) ||
           (customer.phone?.contains(term) ?? false) ||
           (customer.membershipNumber?.toLowerCase().contains(term) ?? false) ||
           (customer.qrCode?.toLowerCase().contains(term) ?? false);
  }

  /// Get customer display name (fallback to phone if no name)
  static String getCustomerDisplayName(CustomerEntity customer) {
    if (customer.name != null && customer.name!.isNotEmpty) {
      return customer.name!;
    }
    if (customer.phone != null && customer.phone!.isNotEmpty) {
      return customer.phone!;
    }
    return customer.membershipNumber ?? 'Customer ${customer.id.substring(0, 8)}';
  }

  /// Check if customer can be contacted via SMS
  static bool canSendSMS(CustomerEntity customer) {
    return customer.phone != null && 
           customer.phone!.isNotEmpty &&
           [PreferredCommunication.sms, PreferredCommunication.whatsapp]
               .contains(customer.preferredCommunication) &&
           customer.isActive;
  }

  /// Check if customer can receive WhatsApp messages
  static bool canSendWhatsApp(CustomerEntity customer) {
    return customer.phone != null && 
           customer.phone!.isNotEmpty &&
           customer.preferredCommunication == PreferredCommunication.whatsapp &&
           customer.isActive;
  }
}
