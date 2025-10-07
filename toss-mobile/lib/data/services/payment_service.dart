import 'dart:async';
import 'dart:math';

import '../../domain/entities/payment_entity.dart';

class PaymentService {
  static final PaymentService _instance = PaymentService._internal();
  factory PaymentService() => _instance;
  PaymentService._internal();

  final List<PaymentGatewayConfig> _gateways = [];
  final StreamController<PaymentTransaction> _paymentController = 
      StreamController<PaymentTransaction>.broadcast();

  // Stream of payment updates
  Stream<PaymentTransaction> get paymentUpdates => _paymentController.stream;

  // Initialize payment gateways
  void initializeGateways() {
    _gateways.clear();
    
    // Paystack (Ghana)
    _gateways.add(
      const PaymentGatewayConfig(
        id: 'paystack',
        name: 'Paystack',
        gatewayType: 'paystack',
        credentials: {
          'publicKey': 'pk_test_xxx',
          'secretKey': 'sk_test_xxx',
        },
        supportedMethods: [
          PaymentMethod.card,
          PaymentMethod.mobileMoney,
          PaymentMethod.bankTransfer,
          PaymentMethod.qrPayment,
        ],
        supportedCurrencies: ['GHS', 'NGN', 'USD'],
      ),
    );

    // Flutterwave
    _gateways.add(
      const PaymentGatewayConfig(
        id: 'flutterwave',
        name: 'Flutterwave',
        gatewayType: 'flutterwave',
        credentials: {
          'publicKey': 'FLWPUBK_TEST-xxx',
          'secretKey': 'FLWSECK_TEST-xxx',
        },
        supportedMethods: [
          PaymentMethod.card,
          PaymentMethod.mobileMoney,
          PaymentMethod.bankTransfer,
        ],
        supportedCurrencies: ['GHS', 'NGN', 'USD', 'KES'],
      ),
    );

    // Local mobile money gateway
    _gateways.add(
      const PaymentGatewayConfig(
        id: 'local_momo',
        name: 'Local Mobile Money',
        gatewayType: 'local',
        credentials: {},
        supportedMethods: [PaymentMethod.mobileMoney],
        supportedCurrencies: ['GHS'],
      ),
    );
  }

  // Get available payment methods
  List<PaymentMethod> getAvailablePaymentMethods({
    double? amount,
    String currency = 'GHS',
  }) {
    final methods = <PaymentMethod>{};
    
    // Always available
    methods.add(PaymentMethod.cash);
    
    for (final gateway in _gateways.where((g) => g.isEnabled)) {
      if (gateway.supportedCurrencies.contains(currency)) {
        if (amount == null || 
            (gateway.minimumAmount == null || amount >= gateway.minimumAmount!) &&
            (gateway.maximumAmount == null || amount <= gateway.maximumAmount!)) {
          methods.addAll(gateway.supportedMethods);
        }
      }
    }
    
    // Add offline methods
    methods.addAll([
      PaymentMethod.storeCredit,
      PaymentMethod.loyaltyPoints,
      PaymentMethod.voucher,
      PaymentMethod.creditAccount,
    ]);
    
    return methods.toList();
  }

  // Process cash payment
  Future<PaymentResult> processCashPayment({
    required String orderId,
    required double amount,
    String currency = 'GHS',
    String? cashierId,
    double? cashReceived,
    Map<String, dynamic> metadata = const {},
  }) async {
    try {
      final transaction = PaymentTransaction(
        id: _generateTransactionId(),
        orderId: orderId,
        method: PaymentMethod.cash,
        amount: amount,
        currency: currency,
        status: PaymentStatus.completed,
        createdAt: DateTime.now(),
        completedAt: DateTime.now(),
        metadata: {
          ...metadata,
          'cashierId': cashierId,
          'cashReceived': cashReceived,
          'change': cashReceived != null ? cashReceived - amount : 0.0,
        },
      );

      _paymentController.add(transaction);

      return PaymentResult(
        result: TransactionResult.success,
        transaction: transaction,
        message: 'Cash payment completed successfully',
      );
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.unknown,
        message: 'Cash payment failed: $e',
      );
    }
  }

  // Process card payment
  Future<PaymentResult> processCardPayment({
    required String orderId,
    required double amount,
    String currency = 'GHS',
    String? cardNumber,
    String? expiryDate,
    String? cvv,
    String? cardHolderName,
    bool isContactless = false,
    String? terminalId,
    Map<String, dynamic> metadata = const {},
  }) async {
    try {
      // Simulate processing delay
      await Future.delayed(const Duration(seconds: 2));

      // Mock card validation and processing
      final isValidCard = _validateCard(cardNumber, expiryDate, cvv);
      if (!isValidCard) {
        return const PaymentResult(
          result: TransactionResult.invalidCard,
          message: 'Invalid card details',
        );
      }

      // Simulate success/failure (90% success rate)
      final random = Random();
      final isSuccess = random.nextDouble() > 0.1;

      if (!isSuccess) {
        return const PaymentResult(
          result: TransactionResult.declined,
          message: 'Card payment declined by issuer',
        );
      }

      final cardType = _detectCardType(cardNumber);
      final transaction = PaymentTransaction(
        id: _generateTransactionId(),
        orderId: orderId,
        method: PaymentMethod.card,
        amount: amount,
        currency: currency,
        status: PaymentStatus.completed,
        createdAt: DateTime.now(),
        completedAt: DateTime.now(),
        authorizationCode: _generateAuthCode(),
        gatewayTransactionId: _generateGatewayTransactionId(),
        metadata: {
          ...metadata,
          'cardType': cardType.toString(),
          'lastFourDigits': cardNumber?.substring(cardNumber.length - 4),
          'cardHolderName': cardHolderName,
          'isContactless': isContactless,
          'terminalId': terminalId,
        },
        processingFee: amount * 0.025, // 2.5% processing fee
      );

      _paymentController.add(transaction);

      return PaymentResult(
        result: TransactionResult.success,
        transaction: transaction,
        message: 'Card payment completed successfully',
        reference: transaction.authorizationCode,
      );
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.networkError,
        message: 'Card payment failed: $e',
      );
    }
  }

  // Process mobile money payment
  Future<PaymentResult> processMobileMoneyPayment({
    required String orderId,
    required double amount,
    String currency = 'GHS',
    required String phoneNumber,
    required MobileMoneyProvider provider,
    String? voucher,
    Map<String, dynamic> metadata = const {},
  }) async {
    try {
      // Simulate processing delay
      await Future.delayed(const Duration(seconds: 3));

      // Validate phone number format
      if (!_validatePhoneNumber(phoneNumber)) {
        return const PaymentResult(
          result: TransactionResult.unknown,
          message: 'Invalid phone number format',
        );
      }

      // Simulate success/failure (85% success rate)
      final random = Random();
      final isSuccess = random.nextDouble() > 0.15;

      if (!isSuccess) {
        final failures = [
          TransactionResult.insufficientFunds,
          TransactionResult.timeout,
          TransactionResult.declined,
        ];
        final failureReason = failures[random.nextInt(failures.length)];
        
        return PaymentResult(
          result: failureReason,
          message: _getFailureMessage(failureReason),
        );
      }

      final transaction = PaymentTransaction(
        id: _generateTransactionId(),
        orderId: orderId,
        method: PaymentMethod.mobileMoney,
        amount: amount,
        currency: currency,
        status: PaymentStatus.completed,
        createdAt: DateTime.now(),
        completedAt: DateTime.now(),
        reference: _generateMomoReference(provider),
        gatewayTransactionId: _generateGatewayTransactionId(),
        metadata: {
          ...metadata,
          'phoneNumber': phoneNumber,
          'provider': provider.toString(),
          'voucher': voucher,
        },
        processingFee: amount * 0.01, // 1% processing fee
      );

      _paymentController.add(transaction);

      return PaymentResult(
        result: TransactionResult.success,
        transaction: transaction,
        message: 'Mobile money payment completed successfully',
        reference: transaction.reference,
      );
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.networkError,
        message: 'Mobile money payment failed: $e',
      );
    }
  }

  // Process QR payment
  Future<PaymentResult> processQRPayment({
    required String orderId,
    required double amount,
    String currency = 'GHS',
    required String qrData,
    Map<String, dynamic> metadata = const {},
  }) async {
    try {
      // Simulate QR processing delay
      await Future.delayed(const Duration(seconds: 1));

      // Parse QR data (simplified)
      final qrInfo = _parseQRData(qrData);
      if (qrInfo == null) {
        return const PaymentResult(
          result: TransactionResult.unknown,
          message: 'Invalid QR code data',
        );
      }

      // Simulate success (95% success rate for QR)
      final random = Random();
      final isSuccess = random.nextDouble() > 0.05;

      if (!isSuccess) {
        return const PaymentResult(
          result: TransactionResult.declined,
          message: 'QR payment declined',
        );
      }

      final transaction = PaymentTransaction(
        id: _generateTransactionId(),
        orderId: orderId,
        method: PaymentMethod.qrPayment,
        amount: amount,
        currency: currency,
        status: PaymentStatus.completed,
        createdAt: DateTime.now(),
        completedAt: DateTime.now(),
        reference: _generateQRReference(),
        gatewayTransactionId: _generateGatewayTransactionId(),
        metadata: {
          ...metadata,
          'qrData': qrData,
          'qrInfo': qrInfo,
        },
        processingFee: amount * 0.015, // 1.5% processing fee
      );

      _paymentController.add(transaction);

      return PaymentResult(
        result: TransactionResult.success,
        transaction: transaction,
        message: 'QR payment completed successfully',
        reference: transaction.reference,
      );
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.networkError,
        message: 'QR payment failed: $e',
      );
    }
  }

  // Process NFC payment
  Future<PaymentResult> processNFCPayment({
    required String orderId,
    required double amount,
    String currency = 'GHS',
    required String nfcData,
    Map<String, dynamic> metadata = const {},
  }) async {
    try {
      // Simulate NFC processing delay
      await Future.delayed(const Duration(milliseconds: 500));

      // Parse NFC data (simplified)
      final nfcInfo = _parseNFCData(nfcData);
      if (nfcInfo == null) {
        return const PaymentResult(
          result: TransactionResult.unknown,
          message: 'Invalid NFC data',
        );
      }

      // Simulate success (98% success rate for NFC)
      final random = Random();
      final isSuccess = random.nextDouble() > 0.02;

      if (!isSuccess) {
        return const PaymentResult(
          result: TransactionResult.declined,
          message: 'NFC payment declined',
        );
      }

      final transaction = PaymentTransaction(
        id: _generateTransactionId(),
        orderId: orderId,
        method: PaymentMethod.nfc,
        amount: amount,
        currency: currency,
        status: PaymentStatus.completed,
        createdAt: DateTime.now(),
        completedAt: DateTime.now(),
        reference: _generateNFCReference(),
        gatewayTransactionId: _generateGatewayTransactionId(),
        metadata: {
          ...metadata,
          'nfcData': nfcData,
          'nfcInfo': nfcInfo,
          'isContactless': true,
        },
        processingFee: amount * 0.02, // 2% processing fee
      );

      _paymentController.add(transaction);

      return PaymentResult(
        result: TransactionResult.success,
        transaction: transaction,
        message: 'Contactless payment completed successfully',
        reference: transaction.reference,
      );
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.networkError,
        message: 'NFC payment failed: $e',
      );
    }
  }

  // Process split payment
  Future<PaymentResult> processSplitPayment({
    required String orderId,
    required double totalAmount,
    String currency = 'GHS',
    required List<Map<String, dynamic>> paymentSplits,
  }) async {
    try {
      final transactions = <PaymentTransaction>[];
      double processedAmount = 0.0;

      for (final split in paymentSplits) {
        final method = split['method'] as PaymentMethod;
        final amount = split['amount'] as double;
        final metadata = split['metadata'] as Map<String, dynamic>? ?? {};

        PaymentResult result;

        switch (method) {
          case PaymentMethod.cash:
            result = await processCashPayment(
              orderId: orderId,
              amount: amount,
              currency: currency,
              metadata: metadata,
            );
            break;
          case PaymentMethod.card:
            result = await processCardPayment(
              orderId: orderId,
              amount: amount,
              currency: currency,
              cardNumber: metadata['cardNumber'],
              expiryDate: metadata['expiryDate'],
              cvv: metadata['cvv'],
              metadata: metadata,
            );
            break;
          case PaymentMethod.mobileMoney:
            result = await processMobileMoneyPayment(
              orderId: orderId,
              amount: amount,
              currency: currency,
              phoneNumber: metadata['phoneNumber'] ?? '',
              provider: metadata['provider'] ?? MobileMoneyProvider.other,
              metadata: metadata,
            );
            break;
          default:
            result = const PaymentResult(
              result: TransactionResult.unknown,
              message: 'Unsupported payment method for split payment',
            );
        }

        if (result.isSuccess && result.transaction != null) {
          transactions.add(result.transaction!);
          processedAmount += amount;
        } else {
          // If any payment fails, return failure
          return result;
        }
      }

      final splitPayment = SplitPayment(
        id: _generateTransactionId(),
        orderId: orderId,
        totalAmount: totalAmount,
        currency: currency,
        transactions: transactions,
        overallStatus: processedAmount >= totalAmount 
            ? PaymentStatus.completed 
            : PaymentStatus.pending,
        createdAt: DateTime.now(),
        completedAt: processedAmount >= totalAmount ? DateTime.now() : null,
      );

      return PaymentResult(
        result: TransactionResult.success,
        message: 'Split payment completed successfully',
        data: {'splitPayment': splitPayment},
      );
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.unknown,
        message: 'Split payment failed: $e',
      );
    }
  }

  // Process store credit payment
  Future<PaymentResult> processStoreCreditPayment({
    required String orderId,
    required double amount,
    String currency = 'GHS',
    required String customerId,
    Map<String, dynamic> metadata = const {},
  }) async {
    try {
      // Mock store credit balance check
      final availableCredit = await _getStoreCreditBalance(customerId);
      
      if (availableCredit < amount) {
        return PaymentResult(
          result: TransactionResult.insufficientFunds,
          message: 'Insufficient store credit balance. Available: $currency ${availableCredit.toStringAsFixed(2)}',
        );
      }

      // Process store credit payment
      final transaction = PaymentTransaction(
        id: _generateTransactionId(),
        orderId: orderId,
        method: PaymentMethod.storeCredit,
        amount: amount,
        currency: currency,
        status: PaymentStatus.completed,
        createdAt: DateTime.now(),
        completedAt: DateTime.now(),
        customerId: customerId,
        metadata: {
          ...metadata,
          'previousBalance': availableCredit,
          'newBalance': availableCredit - amount,
        },
      );

      // Update store credit balance (mock)
      await _updateStoreCreditBalance(customerId, availableCredit - amount);

      _paymentController.add(transaction);

      return PaymentResult(
        result: TransactionResult.success,
        transaction: transaction,
        message: 'Store credit payment completed successfully',
      );
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.unknown,
        message: 'Store credit payment failed: $e',
      );
    }
  }

  // Refund payment
  Future<PaymentResult> refundPayment({
    required String transactionId,
    required double amount,
    String? reason,
  }) async {
    try {
      // Simulate refund processing
      await Future.delayed(const Duration(seconds: 2));

      // Mock refund success (90% success rate)
      final random = Random();
      final isSuccess = random.nextDouble() > 0.1;

      if (!isSuccess) {
        return const PaymentResult(
          result: TransactionResult.declined,
          message: 'Refund declined by payment processor',
        );
      }

      final refundTransaction = PaymentTransaction(
        id: _generateTransactionId(),
        orderId: transactionId, // Using original transaction ID as order ID
        method: PaymentMethod.other, // Refund method
        amount: -amount, // Negative amount for refund
        currency: 'GHS',
        status: PaymentStatus.completed,
        createdAt: DateTime.now(),
        completedAt: DateTime.now(),
        reference: _generateRefundReference(),
        metadata: {
          'originalTransactionId': transactionId,
          'refundReason': reason,
        },
      );

      _paymentController.add(refundTransaction);

      return PaymentResult(
        result: TransactionResult.success,
        transaction: refundTransaction,
        message: 'Refund processed successfully',
        reference: refundTransaction.reference,
      );
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.networkError,
        message: 'Refund failed: $e',
      );
    }
  }

  // Helper methods
  String _generateTransactionId() {
    final now = DateTime.now();
    final random = Random();
    return 'TXN${now.millisecondsSinceEpoch}${random.nextInt(9999).toString().padLeft(4, '0')}';
  }

  String _generateAuthCode() {
    final random = Random();
    return random.nextInt(999999).toString().padLeft(6, '0');
  }

  String _generateGatewayTransactionId() {
    final random = Random();
    return 'GW${random.nextInt(999999999).toString().padLeft(9, '0')}';
  }

  String _generateMomoReference(MobileMoneyProvider provider) {
    final prefix = provider.toString().split('.').last.toUpperCase();
    final random = Random();
    return '$prefix${random.nextInt(99999999).toString().padLeft(8, '0')}';
  }

  String _generateQRReference() {
    final random = Random();
    return 'QR${random.nextInt(99999999).toString().padLeft(8, '0')}';
  }

  String _generateNFCReference() {
    final random = Random();
    return 'NFC${random.nextInt(99999999).toString().padLeft(8, '0')}';
  }

  String _generateRefundReference() {
    final random = Random();
    return 'REF${random.nextInt(99999999).toString().padLeft(8, '0')}';
  }

  bool _validateCard(String? cardNumber, String? expiryDate, String? cvv) {
    if (cardNumber == null || cardNumber.length < 13 || cardNumber.length > 19) {
      return false;
    }
    
    if (expiryDate == null || !RegExp(r'^\d{2}/\d{2}$').hasMatch(expiryDate)) {
      return false;
    }
    
    if (cvv == null || cvv.length < 3 || cvv.length > 4) {
      return false;
    }
    
    return true;
  }

  CardType _detectCardType(String? cardNumber) {
    if (cardNumber == null || cardNumber.isEmpty) return CardType.unknown;
    
    if (cardNumber.startsWith('4')) return CardType.visa;
    if (cardNumber.startsWith(RegExp(r'^5[1-5]'))) return CardType.mastercard;
    if (cardNumber.startsWith(RegExp(r'^3[47]'))) return CardType.amex;
    if (cardNumber.startsWith('6')) return CardType.discover;
    
    return CardType.local;
  }

  bool _validatePhoneNumber(String phoneNumber) {
    // Ghana phone number validation
    final cleanNumber = phoneNumber.replaceAll(RegExp(r'[^\d]'), '');
    return cleanNumber.length >= 9 && cleanNumber.length <= 13;
  }

  Map<String, dynamic>? _parseQRData(String qrData) {
    // Mock QR data parsing
    try {
      return {
        'type': 'payment',
        'provider': 'qr_provider',
        'account': qrData.hashCode.toString(),
      };
    } catch (e) {
      return null;
    }
  }

  Map<String, dynamic>? _parseNFCData(String nfcData) {
    // Mock NFC data parsing
    try {
      return {
        'type': 'contactless',
        'cardType': 'nfc_card',
        'identifier': nfcData.hashCode.toString(),
      };
    } catch (e) {
      return null;
    }
  }

  Future<double> _getStoreCreditBalance(String customerId) async {
    // Mock store credit balance lookup
    await Future.delayed(const Duration(milliseconds: 500));
    final random = Random();
    return random.nextDouble() * 1000; // Random balance between 0-1000
  }

  Future<void> _updateStoreCreditBalance(String customerId, double newBalance) async {
    // Mock store credit balance update
    await Future.delayed(const Duration(milliseconds: 300));
  }

  String _getFailureMessage(TransactionResult result) {
    switch (result) {
      case TransactionResult.insufficientFunds:
        return 'Insufficient funds in account';
      case TransactionResult.timeout:
        return 'Transaction timeout - please try again';
      case TransactionResult.declined:
        return 'Transaction declined';
      case TransactionResult.networkError:
        return 'Network error - please check connection';
      case TransactionResult.invalidCard:
        return 'Invalid card details';
      case TransactionResult.cancelled:
        return 'Transaction cancelled by user';
      default:
        return 'Transaction failed';
    }
  }

  void dispose() {
    _paymentController.close();
  }
}
