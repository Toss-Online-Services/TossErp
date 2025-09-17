import 'dart:async';
import 'dart:convert';
import 'dart:math';
import 'package:http/http.dart' as http;

import '../../domain/entities/payment_entity.dart';

class PaymentRepository {
  static final PaymentRepository _instance = PaymentRepository._internal();
  factory PaymentRepository() => _instance;
  PaymentRepository._internal();

  final Map<String, PaymentGatewayConfig> _gateways = {};

  // Initialize with default payment gateways
  void initializeGateways() {
    // Paystack configuration for Ghana
    _gateways['paystack'] = const PaymentGatewayConfig(
      id: 'paystack',
      name: 'Paystack',
      gatewayType: 'paystack',
      credentials: {
        'publicKey': 'pk_test_b53c945da7c5cc08b17b99ee8c49ca9e83fc6a45',
        'secretKey': 'sk_test_xxx', // Should be stored securely
        'baseUrl': 'https://api.paystack.co',
      },
      supportedMethods: [
        PaymentMethod.card,
        PaymentMethod.mobileMoney,
        PaymentMethod.bankTransfer,
        PaymentMethod.qrPayment,
      ],
      supportedCurrencies: ['GHS', 'NGN', 'USD', 'ZAR'],
      minimumAmount: 1.0,
      maximumAmount: 10000.0,
    );

    // Flutterwave configuration
    _gateways['flutterwave'] = const PaymentGatewayConfig(
      id: 'flutterwave',
      name: 'Flutterwave',
      gatewayType: 'flutterwave',
      credentials: {
        'publicKey': 'FLWPUBK_TEST-XXXXXX',
        'secretKey': 'FLWSECK_TEST-XXXXXX',
        'baseUrl': 'https://api.flutterwave.com/v3',
      },
      supportedMethods: [
        PaymentMethod.card,
        PaymentMethod.mobileMoney,
        PaymentMethod.bankTransfer,
      ],
      supportedCurrencies: ['GHS', 'NGN', 'USD', 'KES', 'UGX'],
      minimumAmount: 1.0,
      maximumAmount: 50000.0,
    );

    // Local Mobile Money Gateway (for direct USSD integration)
    _gateways['local_momo'] = const PaymentGatewayConfig(
      id: 'local_momo',
      name: 'Direct Mobile Money',
      gatewayType: 'local',
      credentials: {
        'apiKey': 'local_momo_key',
        'endpoint': 'https://local-momo-api.example.com',
      },
      supportedMethods: [PaymentMethod.mobileMoney],
      supportedCurrencies: ['GHS'],
      minimumAmount: 1.0,
      maximumAmount: 5000.0,
    );

    // Stripe configuration (for international cards)
    _gateways['stripe'] = const PaymentGatewayConfig(
      id: 'stripe',
      name: 'Stripe',
      gatewayType: 'stripe',
      credentials: {
        'publicKey': 'pk_test_XXXXXX',
        'secretKey': 'sk_test_XXXXXX',
        'baseUrl': 'https://api.stripe.com/v1',
      },
      supportedMethods: [
        PaymentMethod.card,
        PaymentMethod.nfc,
      ],
      supportedCurrencies: ['USD', 'EUR', 'GBP', 'GHS'],
      minimumAmount: 0.5,
      maximumAmount: 99999.0,
    );
  }

  // Get gateway configuration
  PaymentGatewayConfig? getGateway(String gatewayId) {
    return _gateways[gatewayId];
  }

  // Get available gateways for a payment method
  List<PaymentGatewayConfig> getAvailableGateways({
    required PaymentMethod method,
    required String currency,
    double? amount,
  }) {
    return _gateways.values
        .where((gateway) =>
            gateway.isEnabled &&
            gateway.supportedMethods.contains(method) &&
            gateway.supportedCurrencies.contains(currency) &&
            (amount == null ||
                (gateway.minimumAmount == null ||
                    amount >= gateway.minimumAmount!) &&
                (gateway.maximumAmount == null ||
                    amount <= gateway.maximumAmount!)))
        .toList();
  }

  // Paystack payment methods
  Future<PaymentResult> processPaystackPayment({
    required String orderId,
    required double amount,
    required String currency,
    required String email,
    required PaymentMethod method,
    Map<String, dynamic> metadata = const {},
  }) async {
    try {
  final gateway = _gateways['paystack']!;
      final baseUrl = gateway.credentials['baseUrl']!;

      // Initialize transaction
      final initResponse = await http.post(
        Uri.parse('$baseUrl/transaction/initialize'),
        headers: {
          'Authorization': 'Bearer ${gateway.credentials['secretKey']}',
          'Content-Type': 'application/json',
        },
        body: jsonEncode({
          'email': email,
          'amount': (amount * 100).toInt(), // Convert to kobo/pesewas
          'currency': currency,
          'reference': orderId,
          'callback_url': 'https://toss.app/payment/callback',
          'metadata': {
            ...metadata,
            'order_id': orderId,
            'payment_method': method.toString(),
          },
          'channels': _getPaystackChannels(method),
        }),
      );

      if (initResponse.statusCode == 200) {
        final responseData = jsonDecode(initResponse.body);
        
        if (responseData['status'] == true) {
          final reference = responseData['data']['reference'];

          // For demo purposes, simulate payment completion
          // In real implementation, you would redirect to authorizationUrl
          await Future.delayed(const Duration(seconds: 2));
          
          // Verify payment
          return await _verifyPaystackPayment(reference);
        } else {
          return PaymentResult(
            result: TransactionResult.declined,
            message: responseData['message'] ?? 'Payment initialization failed',
          );
        }
      } else {
        return const PaymentResult(
          result: TransactionResult.networkError,
          message: 'Failed to initialize payment',
        );
      }
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.networkError,
        message: 'Payment error: $e',
      );
    }
  }

  Future<PaymentResult> _verifyPaystackPayment(String reference) async {
    try {
      final gateway = _gateways['paystack']!;
      final baseUrl = gateway.credentials['baseUrl']!;

      final verifyResponse = await http.get(
        Uri.parse('$baseUrl/transaction/verify/$reference'),
        headers: {
          'Authorization': 'Bearer ${gateway.credentials['secretKey']}',
        },
      );

      if (verifyResponse.statusCode == 200) {
        final responseData = jsonDecode(verifyResponse.body);
        
        if (responseData['status'] == true &&
            responseData['data']['status'] == 'success') {
          final transaction = PaymentTransaction(
            id: _generateTransactionId(),
            orderId: responseData['data']['reference'],
            method: PaymentMethod.card, // Determine actual method from response
            amount: responseData['data']['amount'] / 100.0, // Convert from kobo
            currency: responseData['data']['currency'],
            status: PaymentStatus.completed,
            createdAt: DateTime.parse(responseData['data']['created_at']),
            completedAt: DateTime.parse(responseData['data']['paid_at']),
            reference: reference,
            authorizationCode: responseData['data']['authorization']['authorization_code'],
            gatewayTransactionId: responseData['data']['id'].toString(),
            metadata: responseData['data']['metadata'] ?? {},
          );

          return PaymentResult(
            result: TransactionResult.success,
            transaction: transaction,
            message: 'Payment completed successfully',
            reference: reference,
          );
        } else {
          return PaymentResult(
            result: TransactionResult.declined,
            message: responseData['data']['gateway_response'] ?? 'Payment failed',
          );
        }
      } else {
        return const PaymentResult(
          result: TransactionResult.networkError,
          message: 'Failed to verify payment',
        );
      }
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.networkError,
        message: 'Verification error: $e',
      );
    }
  }

  // Flutterwave payment methods
  Future<PaymentResult> processFlutterwavePayment({
    required String orderId,
    required double amount,
    required String currency,
    required String customerEmail,
    required String customerName,
    required String customerPhone,
    required PaymentMethod method,
    Map<String, dynamic> metadata = const {},
  }) async {
    try {
  final gateway = _gateways['flutterwave']!;
      final baseUrl = gateway.credentials['baseUrl']!;

      final paymentData = {
        'tx_ref': orderId,
        'amount': amount,
        'currency': currency,
        'redirect_url': 'https://toss.app/payment/callback',
        'payment_options': _getFlutterwavePaymentOptions(method),
        'customer': {
          'email': customerEmail,
          'name': customerName,
          'phonenumber': customerPhone,
        },
        'customizations': {
          'title': 'TOSS Payment',
          'description': 'Payment for order $orderId',
          'logo': 'https://toss.app/logo.png',
        },
        'meta': {
          ...metadata,
          'order_id': orderId,
          'payment_method': method.toString(),
        },
      };

      final response = await http.post(
        Uri.parse('$baseUrl/payments'),
        headers: {
          'Authorization': 'Bearer ${gateway.credentials['secretKey']}',
          'Content-Type': 'application/json',
        },
        body: jsonEncode(paymentData),
      );

      if (response.statusCode == 200) {
        final responseData = jsonDecode(response.body);
        
        if (responseData['status'] == 'success') {
          // final paymentLink = responseData['data']['link']; // Not used in demo flow
          
          // For demo purposes, simulate payment completion
          await Future.delayed(const Duration(seconds: 3));
          
          // Verify transaction
          return await _verifyFlutterwavePayment(orderId);
        } else {
          return PaymentResult(
            result: TransactionResult.declined,
            message: responseData['message'] ?? 'Payment initialization failed',
          );
        }
      } else {
        return const PaymentResult(
          result: TransactionResult.networkError,
          message: 'Failed to initialize payment',
        );
      }
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.networkError,
        message: 'Payment error: $e',
      );
    }
  }

  Future<PaymentResult> _verifyFlutterwavePayment(String txRef) async {
    try {
      final gateway = _gateways['flutterwave']!;
      final baseUrl = gateway.credentials['baseUrl']!;

      final verifyResponse = await http.get(
        Uri.parse('$baseUrl/transactions/verify_by_reference?tx_ref=$txRef'),
        headers: {
          'Authorization': 'Bearer ${gateway.credentials['secretKey']}',
        },
      );

      if (verifyResponse.statusCode == 200) {
        final responseData = jsonDecode(verifyResponse.body);
        
        if (responseData['status'] == 'success' &&
            responseData['data']['status'] == 'successful') {
          final transaction = PaymentTransaction(
            id: _generateTransactionId(),
            orderId: responseData['data']['tx_ref'],
            method: _mapFlutterwavePaymentMethod(responseData['data']['payment_type']),
            amount: responseData['data']['amount'].toDouble(),
            currency: responseData['data']['currency'],
            status: PaymentStatus.completed,
            createdAt: DateTime.parse(responseData['data']['created_at']),
            completedAt: DateTime.now(),
            reference: responseData['data']['flw_ref'],
            gatewayTransactionId: responseData['data']['id'].toString(),
            metadata: responseData['data']['meta'] ?? {},
            processingFee: responseData['data']['app_fee']?.toDouble(),
          );

          return PaymentResult(
            result: TransactionResult.success,
            transaction: transaction,
            message: 'Payment completed successfully',
            reference: responseData['data']['flw_ref'],
          );
        } else {
          return const PaymentResult(
            result: TransactionResult.declined,
            message: 'Payment was not successful',
          );
        }
      } else {
        return const PaymentResult(
          result: TransactionResult.networkError,
          message: 'Failed to verify payment',
        );
      }
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.networkError,
        message: 'Verification error: $e',
      );
    }
  }

  // Direct mobile money integration (Ghana)
  Future<PaymentResult> processDirectMobileMoneyPayment({
    required String orderId,
    required double amount,
    required String phoneNumber,
    required MobileMoneyProvider provider,
    String? voucher,
  }) async {
    try {
  // Using local momo gateway config if needed for future enhancements
  final _ = _gateways['local_momo']!;
      
      // Simulate USSD push
      await Future.delayed(const Duration(seconds: 1));
      
      final providerCode = _getMobileMoneyProviderCode(provider);
      final ussdCode = _generateUSSDCode(provider, amount, orderId);
      
      // In real implementation, this would trigger actual USSD push
      // For now, we simulate the process
      
      // Simulate user confirmation delay
      await Future.delayed(const Duration(seconds: 5));
      
      // Simulate success rate based on provider (85% success)
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
          message: _getMobileMoneyFailureMessage(failureReason, provider),
        );
      }

      final transaction = PaymentTransaction(
        id: _generateTransactionId(),
        orderId: orderId,
        method: PaymentMethod.mobileMoney,
        amount: amount,
        currency: 'GHS',
        status: PaymentStatus.completed,
        createdAt: DateTime.now(),
        completedAt: DateTime.now(),
        reference: _generateMobileMoneyReference(provider),
        gatewayTransactionId: _generateGatewayTransactionId(),
        metadata: {
          'phoneNumber': phoneNumber,
          'provider': provider.toString(),
          'providerCode': providerCode,
          'ussdCode': ussdCode,
          'voucher': voucher,
        },
        processingFee: amount * 0.01, // 1% fee
      );

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

  // Stripe payment for international cards
  Future<PaymentResult> processStripePayment({
    required String orderId,
    required double amount,
    required String currency,
    required String cardToken,
    Map<String, dynamic> metadata = const {},
  }) async {
    try {
      final gateway = _gateways['stripe']!;
      final baseUrl = gateway.credentials['baseUrl']!;

      final paymentData = {
        'amount': (amount * 100).toInt(), // Convert to cents
        'currency': currency.toLowerCase(),
        'source': cardToken,
        'description': 'TOSS Payment for order $orderId',
        'metadata': {
          ...metadata,
          'order_id': orderId,
        },
      };

      final response = await http.post(
        Uri.parse('$baseUrl/charges'),
        headers: {
          'Authorization': 'Bearer ${gateway.credentials['secretKey']}',
          'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: Uri(queryParameters: paymentData.map(
          (key, value) => MapEntry(key, value.toString()),
        )).query,
      );

      if (response.statusCode == 200) {
        final responseData = jsonDecode(response.body);
        
        if (responseData['status'] == 'succeeded') {
          final transaction = PaymentTransaction(
            id: _generateTransactionId(),
            orderId: orderId,
            method: PaymentMethod.card,
            amount: responseData['amount'] / 100.0,
            currency: responseData['currency'].toUpperCase(),
            status: PaymentStatus.completed,
            createdAt: DateTime.fromMillisecondsSinceEpoch(
              responseData['created'] * 1000,
            ),
            completedAt: DateTime.now(),
            reference: responseData['id'],
            gatewayTransactionId: responseData['balance_transaction'],
            metadata: responseData['metadata'] ?? {},
            processingFee: responseData['application_fee_amount']?.toDouble(),
          );

          return PaymentResult(
            result: TransactionResult.success,
            transaction: transaction,
            message: 'Card payment completed successfully',
            reference: responseData['id'],
          );
        } else {
          return PaymentResult(
            result: TransactionResult.declined,
            message: responseData['failure_message'] ?? 'Payment declined',
          );
        }
      } else {
        final errorData = jsonDecode(response.body);
        return PaymentResult(
          result: TransactionResult.declined,
          message: errorData['error']['message'] ?? 'Payment failed',
        );
      }
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.networkError,
        message: 'Payment error: $e',
      );
    }
  }

  // Helper methods
  List<String> _getPaystackChannels(PaymentMethod method) {
    switch (method) {
      case PaymentMethod.card:
        return ['card'];
      case PaymentMethod.mobileMoney:
        return ['mobile_money'];
      case PaymentMethod.bankTransfer:
        return ['bank_transfer'];
      case PaymentMethod.qrPayment:
        return ['qr'];
      default:
        return ['card', 'mobile_money'];
    }
  }

  String _getFlutterwavePaymentOptions(PaymentMethod method) {
    switch (method) {
      case PaymentMethod.card:
        return 'card';
      case PaymentMethod.mobileMoney:
        return 'mobilemoneyghana';
      case PaymentMethod.bankTransfer:
        return 'banktransfer';
      default:
        return 'card,mobilemoneyghana';
    }
  }

  PaymentMethod _mapFlutterwavePaymentMethod(String paymentType) {
    switch (paymentType.toLowerCase()) {
      case 'card':
        return PaymentMethod.card;
      case 'mobilemoney':
      case 'mobilemoneyghana':
        return PaymentMethod.mobileMoney;
      case 'banktransfer':
        return PaymentMethod.bankTransfer;
      default:
        return PaymentMethod.other;
    }
  }

  String _getMobileMoneyProviderCode(MobileMoneyProvider provider) {
    switch (provider) {
      case MobileMoneyProvider.mtn:
        return 'MTN';
      case MobileMoneyProvider.vodafone:
        return 'VOD';
      case MobileMoneyProvider.airteltigo:
        return 'ATL';
      case MobileMoneyProvider.telecel:
        return 'TEL';
      default:
        return 'OTHER';
    }
  }

  String _generateUSSDCode(MobileMoneyProvider provider, double amount, String orderId) {
    switch (provider) {
      case MobileMoneyProvider.mtn:
        return '*170*${amount.toStringAsFixed(0)}*${orderId.hashCode.abs() % 10000}#';
      case MobileMoneyProvider.vodafone:
        return '*110*${amount.toStringAsFixed(0)}*${orderId.hashCode.abs() % 10000}#';
      case MobileMoneyProvider.airteltigo:
        return '*185*${amount.toStringAsFixed(0)}*${orderId.hashCode.abs() % 10000}#';
      case MobileMoneyProvider.telecel:
        return '*177*${amount.toStringAsFixed(0)}*${orderId.hashCode.abs() % 10000}#';
      default:
        return '*000*${amount.toStringAsFixed(0)}*${orderId.hashCode.abs() % 10000}#';
    }
  }

  String _generateMobileMoneyReference(MobileMoneyProvider provider) {
    final prefix = _getMobileMoneyProviderCode(provider);
    final random = Random();
    final timestamp = DateTime.now().millisecondsSinceEpoch;
    return '$prefix${timestamp.toString().substring(8)}${random.nextInt(999)}';
  }

  String _getMobileMoneyFailureMessage(TransactionResult result, MobileMoneyProvider provider) {
    final providerName = _getMobileMoneyProviderCode(provider);
    
    switch (result) {
      case TransactionResult.insufficientFunds:
        return 'Insufficient balance in $providerName wallet';
      case TransactionResult.timeout:
        return '$providerName transaction timeout - please try again';
      case TransactionResult.declined:
        return '$providerName transaction declined';
      default:
        return '$providerName payment failed';
    }
  }

  String _generateTransactionId() {
    final now = DateTime.now();
    final random = Random();
    return 'TXN${now.millisecondsSinceEpoch}${random.nextInt(9999).toString().padLeft(4, '0')}';
  }

  String _generateGatewayTransactionId() {
    final random = Random();
    return 'GW${random.nextInt(999999999).toString().padLeft(9, '0')}';
  }

  // Payment refund methods
  Future<PaymentResult> refundPayment({
    required String gatewayType,
    required String transactionId,
    required double amount,
    String? reason,
  }) async {
    switch (gatewayType) {
      case 'paystack':
        return await _refundPaystackPayment(transactionId, amount, reason);
      case 'flutterwave':
        return await _refundFlutterwavePayment(transactionId, amount, reason);
      case 'stripe':
        return await _refundStripePayment(transactionId, amount, reason);
      default:
        return const PaymentResult(
          result: TransactionResult.unknown,
          message: 'Refund not supported for this gateway',
        );
    }
  }

  Future<PaymentResult> _refundPaystackPayment(
    String transactionId,
    double amount,
    String? reason,
  ) async {
    try {
      final gateway = _gateways['paystack']!;
      final baseUrl = gateway.credentials['baseUrl']!;

      final refundData = {
        'transaction': transactionId,
        'amount': (amount * 100).toInt(),
        if (reason != null) 'customer_note': reason,
      };

      final response = await http.post(
        Uri.parse('$baseUrl/refund'),
        headers: {
          'Authorization': 'Bearer ${gateway.credentials['secretKey']}',
          'Content-Type': 'application/json',
        },
        body: jsonEncode(refundData),
      );

      if (response.statusCode == 200) {
        final responseData = jsonDecode(response.body);
        
        if (responseData['status'] == true) {
          return PaymentResult(
            result: TransactionResult.success,
            message: 'Refund processed successfully',
            reference: responseData['data']['id'].toString(),
          );
        } else {
          return PaymentResult(
            result: TransactionResult.declined,
            message: responseData['message'] ?? 'Refund failed',
          );
        }
      } else {
        return const PaymentResult(
          result: TransactionResult.networkError,
          message: 'Failed to process refund',
        );
      }
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.networkError,
        message: 'Refund error: $e',
      );
    }
  }

  Future<PaymentResult> _refundFlutterwavePayment(
    String transactionId,
    double amount,
    String? reason,
  ) async {
    try {
      final gateway = _gateways['flutterwave']!;
      final baseUrl = gateway.credentials['baseUrl']!;

      final refundData = {
        'amount': amount,
        if (reason != null) 'comments': reason,
      };

      final response = await http.post(
        Uri.parse('$baseUrl/transactions/$transactionId/refund'),
        headers: {
          'Authorization': 'Bearer ${gateway.credentials['secretKey']}',
          'Content-Type': 'application/json',
        },
        body: jsonEncode(refundData),
      );

      if (response.statusCode == 200) {
        final responseData = jsonDecode(response.body);
        
        if (responseData['status'] == 'success') {
          return PaymentResult(
            result: TransactionResult.success,
            message: 'Refund processed successfully',
            reference: responseData['data']['id'].toString(),
          );
        } else {
          return PaymentResult(
            result: TransactionResult.declined,
            message: responseData['message'] ?? 'Refund failed',
          );
        }
      } else {
        return const PaymentResult(
          result: TransactionResult.networkError,
          message: 'Failed to process refund',
        );
      }
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.networkError,
        message: 'Refund error: $e',
      );
    }
  }

  Future<PaymentResult> _refundStripePayment(
    String chargeId,
    double amount,
    String? reason,
  ) async {
    try {
      final gateway = _gateways['stripe']!;
      final baseUrl = gateway.credentials['baseUrl']!;

      final refundData = {
        'charge': chargeId,
        'amount': (amount * 100).toInt(),
        if (reason != null) 'reason': reason,
      };

      final response = await http.post(
        Uri.parse('$baseUrl/refunds'),
        headers: {
          'Authorization': 'Bearer ${gateway.credentials['secretKey']}',
          'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: Uri(queryParameters: refundData.map(
          (key, value) => MapEntry(key, value.toString()),
        )).query,
      );

      if (response.statusCode == 200) {
        final responseData = jsonDecode(response.body);
        
        if (responseData['status'] == 'succeeded') {
          return PaymentResult(
            result: TransactionResult.success,
            message: 'Refund processed successfully',
            reference: responseData['id'],
          );
        } else {
          return PaymentResult(
            result: TransactionResult.declined,
            message: responseData['failure_reason'] ?? 'Refund failed',
          );
        }
      } else {
        final errorData = jsonDecode(response.body);
        return PaymentResult(
          result: TransactionResult.declined,
          message: errorData['error']['message'] ?? 'Refund failed',
        );
      }
    } catch (e) {
      return PaymentResult(
        result: TransactionResult.networkError,
        message: 'Refund error: $e',
      );
    }
  }
}
