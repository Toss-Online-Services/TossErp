import '../../../domain/entities/customer_entity.dart';
import '../../../domain/entities/customer_message_entity.dart';

class CustomerCommunicationService {
  /// Generate SMS receipt content
  static String generateReceiptSMS({
    required String customerName,
    required String storeName,
    required DateTime transactionDate,
    required String transactionNumber,
    required int totalAmount,
    required int pointsEarned,
    required int pointsBalance,
  }) {
    final formattedAmount = (totalAmount / 100).toStringAsFixed(2);
    
    return '''
$storeName
Receipt: $transactionNumber
Date: ${_formatDate(transactionDate)}

Thank you, $customerName!
Total: \$${formattedAmount}
Points Earned: $pointsEarned
Points Balance: $pointsBalance

Visit again soon!
'''.trim();
  }

  /// Generate loyalty points SMS
  static String generateLoyaltyPointsSMS({
    required String customerName,
    required String storeName,
    required int pointsEarned,
    required int totalPoints,
    String? specialMessage,
  }) {
    var message = '''
Hi $customerName!

You earned $pointsEarned points today at $storeName!
Total Points: $totalPoints

Thank you for your loyalty!
''';

    if (specialMessage != null) {
      message += '\n$specialMessage';
    }

    return message.trim();
  }

  /// Generate birthday SMS
  static String generateBirthdaySMS({
    required String customerName,
    required String storeName,
    required int bonusPoints,
    String? specialOffer,
  }) {
    var message = '''
Happy Birthday, $customerName! 🎉

$storeName wishes you a wonderful day!
Enjoy $bonusPoints bonus points from us!
''';

    if (specialOffer != null) {
      message += '\n$specialOffer';
    }

    message += '\n\nCelebrate with us today!';

    return message.trim();
  }

  /// Generate anniversary SMS
  static String generateAnniversarySMS({
    required String customerName,
    required String storeName,
    required int yearsSinceJoined,
    required int bonusPoints,
    String? specialOffer,
  }) {
    var message = '''
Congratulations, $customerName! 🎊

It's been $yearsSinceJoined year${yearsSinceJoined > 1 ? 's' : ''} since you joined $storeName!
Enjoy $bonusPoints anniversary points!
''';

    if (specialOffer != null) {
      message += '\n$specialOffer';
    }

    message += '\n\nThank you for being a loyal customer!';

    return message.trim();
  }

  /// Generate promotion SMS
  static String generatePromotionSMS({
    required String customerName,
    required String storeName,
    required String promotionTitle,
    required String promotionDescription,
    String? promoCode,
    DateTime? validUntil,
  }) {
    var message = '''
Hi $customerName!

$promotionTitle at $storeName

$promotionDescription
''';

    if (promoCode != null) {
      message += '\nPromo Code: $promoCode';
    }

    if (validUntil != null) {
      message += '\nValid until: ${_formatDate(validUntil)}';
    }

    message += '\n\nDon\'t miss out!';

    return message.trim();
  }

  /// Generate low stock alert for customer (for favorite products)
  static String generateLowStockAlertSMS({
    required String customerName,
    required String storeName,
    required String productName,
    required int remainingStock,
  }) {
    return '''
Hi $customerName!

Your favorite item "$productName" is running low at $storeName.
Only $remainingStock left in stock.

Visit us soon to secure yours!
'''.trim();
  }

  /// Generate inactive customer re-engagement SMS
  static String generateReEngagementSMS({
    required String customerName,
    required String storeName,
    required int daysSinceLastVisit,
    required int currentPoints,
    String? specialOffer,
  }) {
    var message = '''
We miss you, $customerName! 😊

It's been $daysSinceLastVisit days since your last visit to $storeName.
You have $currentPoints points waiting for you!
''';

    if (specialOffer != null) {
      message += '\n$specialOffer';
    }

    message += '\n\nCome back and see what\'s new!';

    return message.trim();
  }

  /// Generate appointment reminder SMS
  static String generateAppointmentReminderSMS({
    required String customerName,
    required String storeName,
    required DateTime appointmentDate,
    required String serviceType,
  }) {
    return '''
Hi $customerName!

Reminder: Your $serviceType appointment at $storeName is scheduled for ${_formatDateTime(appointmentDate)}.

See you soon!

Reply CANCEL to cancel.
'''.trim();
  }

  /// Generate order ready pickup SMS
  static String generateOrderReadySMS({
    required String customerName,
    required String storeName,
    required String orderNumber,
    String? specialInstructions,
  }) {
    var message = '''
Hi $customerName!

Your order #$orderNumber is ready for pickup at $storeName!
''';

    if (specialInstructions != null) {
      message += '\n$specialInstructions';
    }

    message += '\n\nWe\'ll hold it for 3 days.';

    return message.trim();
  }

  /// Generate payment reminder SMS
  static String generatePaymentReminderSMS({
    required String customerName,
    required String storeName,
    required String invoiceNumber,
    required int amountDue,
    required DateTime dueDate,
  }) {
    final formattedAmount = (amountDue / 100).toStringAsFixed(2);
    
    return '''
Hi $customerName,

Payment reminder from $storeName
Invoice: $invoiceNumber
Amount Due: \$${formattedAmount}
Due Date: ${_formatDate(dueDate)}

Please visit us or call to arrange payment.
'''.trim();
  }

  /// Check if message should be sent based on customer preferences
  static bool shouldSendMessage(CustomerEntity customer, MessageType messageType, MessageChannel channel) {
    if (!customer.isActive) return false;
    
    // Check if customer accepts this communication channel
    switch (customer.preferredCommunication) {
      case PreferredCommunication.none:
        return false;
      case PreferredCommunication.sms:
        return channel == MessageChannel.sms;
      case PreferredCommunication.whatsapp:
        return channel == MessageChannel.whatsapp;
      case PreferredCommunication.voice:
        return channel == MessageChannel.voice;
      case PreferredCommunication.print:
        return channel == MessageChannel.print;
    }
  }

  /// Get optimal message channel for customer
  static MessageChannel getOptimalMessageChannel(CustomerEntity customer) {
    if (customer.phone == null || customer.phone!.isEmpty) {
      return MessageChannel.print;
    }
    
    switch (customer.preferredCommunication) {
      case PreferredCommunication.sms:
        return MessageChannel.sms;
      case PreferredCommunication.whatsapp:
        return MessageChannel.whatsapp;
      case PreferredCommunication.voice:
        return MessageChannel.voice;
      case PreferredCommunication.print:
        return MessageChannel.print;
      case PreferredCommunication.none:
        return MessageChannel.print; // Fallback to print receipt
    }
  }

  /// Create a customer message entity
  static CustomerMessageEntity createMessage({
    required String customerId,
    required MessageType type,
    required MessageChannel channel,
    required String content,
    DateTime? scheduledAt,
    Map<String, dynamic>? metadata,
  }) {
    return CustomerMessageEntity(
      customerId: customerId,
      type: type,
      channel: channel,
      content: content,
      scheduledAt: scheduledAt ?? DateTime.now(),
      metadata: metadata,
    );
  }

  /// Format date for SMS display
  static String _formatDate(DateTime date) {
    return '${date.day}/${date.month}/${date.year}';
  }

  /// Format datetime for SMS display
  static String _formatDateTime(DateTime dateTime) {
    return '${_formatDate(dateTime)} at ${dateTime.hour.toString().padLeft(2, '0')}:${dateTime.minute.toString().padLeft(2, '0')}';
  }

  /// Validate phone number for SMS sending
  static bool isValidForSMS(String? phone) {
    if (phone == null || phone.isEmpty) return false;
    // Basic validation - should be enhanced based on local requirements
    final cleanPhone = phone.replaceAll(RegExp(r'[^\d\+]'), '');
    return cleanPhone.length >= 8 && cleanPhone.length <= 15;
  }

  /// Format phone number for display
  static String formatPhoneForDisplay(String phone) {
    final cleanPhone = phone.replaceAll(RegExp(r'[^\d]'), '');
    if (cleanPhone.length == 10) {
      return '(${cleanPhone.substring(0, 3)}) ${cleanPhone.substring(3, 6)}-${cleanPhone.substring(6)}';
    }
    return phone; // Return original if format doesn't match
  }
}
