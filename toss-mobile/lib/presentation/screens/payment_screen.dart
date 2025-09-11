import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import 'package:vibration/vibration.dart';

import '../../domain/entities/payment_entity.dart';
import '../../data/services/payment_service.dart';

class PaymentScreen extends StatefulWidget {
  final String orderId;
  final double totalAmount;
  final String currency;
  final Function(PaymentResult) onPaymentComplete;

  const PaymentScreen({
    Key? key,
    required this.orderId,
    required this.totalAmount,
    this.currency = 'GHS',
    required this.onPaymentComplete,
  }) : super(key: key);

  @override
  State<PaymentScreen> createState() => _PaymentScreenState();
}

class _PaymentScreenState extends State<PaymentScreen>
    with TickerProviderStateMixin {
  final PaymentService _paymentService = PaymentService();
  
  late TabController _tabController;
  PaymentMethod _selectedMethod = PaymentMethod.cash;
  bool _isProcessing = false;
  
  // Cash payment
  final TextEditingController _cashReceivedController = TextEditingController();
  
  // Card payment
  final TextEditingController _cardNumberController = TextEditingController();
  final TextEditingController _expiryController = TextEditingController();
  final TextEditingController _cvvController = TextEditingController();
  final TextEditingController _cardHolderController = TextEditingController();
  
  // Mobile money payment
  final TextEditingController _phoneNumberController = TextEditingController();
  MobileMoneyProvider _selectedProvider = MobileMoneyProvider.mtn;
  final TextEditingController _voucherController = TextEditingController();
  
  // Split payment
  final List<Map<String, dynamic>> _splitPayments = [];
  double _splitTotal = 0.0;
  
  // QR/NFC Scanner
  MobileScannerController? _scannerController;
  bool _isScanning = false;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 6, vsync: this);
    _paymentService.initializeGateways();
  }

  @override
  void dispose() {
    _tabController.dispose();
    _cashReceivedController.dispose();
    _cardNumberController.dispose();
    _expiryController.dispose();
    _cvvController.dispose();
    _cardHolderController.dispose();
    _phoneNumberController.dispose();
    _voucherController.dispose();
    _scannerController?.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Payment'),
        backgroundColor: Theme.of(context).primaryColor,
        foregroundColor: Colors.white,
        bottom: TabBar(
          controller: _tabController,
          isScrollable: true,
          tabs: const [
            Tab(icon: Icon(Icons.money), text: 'Cash'),
            Tab(icon: Icon(Icons.credit_card), text: 'Card'),
            Tab(icon: Icon(Icons.phone_android), text: 'Mobile Money'),
            Tab(icon: Icon(Icons.qr_code), text: 'QR Code'),
            Tab(icon: Icon(Icons.nfc), text: 'NFC'),
            Tab(icon: Icon(Icons.call_split), text: 'Split'),
          ],
        ),
      ),
      body: Column(
        children: [
          // Amount display
          Container(
            width: double.infinity,
            padding: const EdgeInsets.all(16.0),
            color: Theme.of(context).primaryColor.withOpacity(0.1),
            child: Column(
              children: [
                Text(
                  'Total Amount',
                  style: Theme.of(context).textTheme.titleMedium,
                ),
                const SizedBox(height: 8),
                Text(
                  '${widget.currency} ${widget.totalAmount.toStringAsFixed(2)}',
                  style: Theme.of(context).textTheme.headlineMedium?.copyWith(
                    fontWeight: FontWeight.bold,
                    color: Theme.of(context).primaryColor,
                  ),
                ),
              ],
            ),
          ),
          
          // Payment method tabs
          Expanded(
            child: TabBarView(
              controller: _tabController,
              children: [
                _buildCashPayment(),
                _buildCardPayment(),
                _buildMobileMoneyPayment(),
                _buildQRPayment(),
                _buildNFCPayment(),
                _buildSplitPayment(),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCashPayment() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Cash Payment',
                    style: Theme.of(context).textTheme.titleLarge,
                  ),
                  const SizedBox(height: 16),
                  TextFormField(
                    controller: _cashReceivedController,
                    decoration: const InputDecoration(
                      labelText: 'Cash Received',
                      prefixText: 'GHS ',
                      border: OutlineInputBorder(),
                    ),
                    keyboardType: TextInputType.number,
                    inputFormatters: [
                      FilteringTextInputFormatter.allow(
                        RegExp(r'^\d+\.?\d{0,2}'),
                      ),
                    ],
                    onChanged: (value) {
                      setState(() {});
                    },
                  ),
                  const SizedBox(height: 16),
                  if (_cashReceivedController.text.isNotEmpty)
                    _buildChangeDisplay(),
                ],
              ),
            ),
          ),
          const Spacer(),
          ElevatedButton(
            onPressed: _canProcessCashPayment() && !_isProcessing
                ? _processCashPayment
                : null,
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.green,
              foregroundColor: Colors.white,
              padding: const EdgeInsets.all(16.0),
            ),
            child: _isProcessing
                ? const CircularProgressIndicator(color: Colors.white)
                : const Text('Complete Cash Payment'),
          ),
        ],
      ),
    );
  }

  Widget _buildChangeDisplay() {
    final cashReceived = double.tryParse(_cashReceivedController.text) ?? 0.0;
    final change = cashReceived - widget.totalAmount;
    
    return Container(
      padding: const EdgeInsets.all(12.0),
      decoration: BoxDecoration(
        color: change >= 0 ? Colors.green.shade50 : Colors.red.shade50,
        border: Border.all(
          color: change >= 0 ? Colors.green : Colors.red,
          width: 1,
        ),
        borderRadius: BorderRadius.circular(8.0),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(
            change >= 0 ? 'Change:' : 'Short:',
            style: TextStyle(
              fontWeight: FontWeight.bold,
              color: change >= 0 ? Colors.green.shade700 : Colors.red.shade700,
            ),
          ),
          Text(
            'GHS ${change.abs().toStringAsFixed(2)}',
            style: TextStyle(
              fontWeight: FontWeight.bold,
              fontSize: 18,
              color: change >= 0 ? Colors.green.shade700 : Colors.red.shade700,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCardPayment() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Card Payment',
                      style: Theme.of(context).textTheme.titleLarge,
                    ),
                    const SizedBox(height: 16),
                    TextFormField(
                      controller: _cardNumberController,
                      decoration: const InputDecoration(
                        labelText: 'Card Number',
                        border: OutlineInputBorder(),
                        hintText: '1234 5678 9012 3456',
                      ),
                      keyboardType: TextInputType.number,
                      inputFormatters: [
                        FilteringTextInputFormatter.digitsOnly,
                        LengthLimitingTextInputFormatter(16),
                        _CardNumberInputFormatter(),
                      ],
                    ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        Expanded(
                          child: TextFormField(
                            controller: _expiryController,
                            decoration: const InputDecoration(
                              labelText: 'MM/YY',
                              border: OutlineInputBorder(),
                              hintText: '12/25',
                            ),
                            keyboardType: TextInputType.number,
                            inputFormatters: [
                              FilteringTextInputFormatter.digitsOnly,
                              LengthLimitingTextInputFormatter(4),
                              _ExpiryDateInputFormatter(),
                            ],
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: TextFormField(
                            controller: _cvvController,
                            decoration: const InputDecoration(
                              labelText: 'CVV',
                              border: OutlineInputBorder(),
                              hintText: '123',
                            ),
                            keyboardType: TextInputType.number,
                            inputFormatters: [
                              FilteringTextInputFormatter.digitsOnly,
                              LengthLimitingTextInputFormatter(4),
                            ],
                            obscureText: true,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _cardHolderController,
                      decoration: const InputDecoration(
                        labelText: 'Cardholder Name',
                        border: OutlineInputBorder(),
                      ),
                      textCapitalization: TextCapitalization.words,
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: _canProcessCardPayment() && !_isProcessing
                  ? _processCardPayment
                  : null,
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.blue,
                foregroundColor: Colors.white,
                padding: const EdgeInsets.all(16.0),
              ),
              child: _isProcessing
                  ? const CircularProgressIndicator(color: Colors.white)
                  : const Text('Process Card Payment'),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildMobileMoneyPayment() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Mobile Money Payment',
                    style: Theme.of(context).textTheme.titleLarge,
                  ),
                  const SizedBox(height: 16),
                  DropdownButtonFormField<MobileMoneyProvider>(
                    value: _selectedProvider,
                    decoration: const InputDecoration(
                      labelText: 'Network Provider',
                      border: OutlineInputBorder(),
                    ),
                    items: MobileMoneyProvider.values.map((provider) {
                      return DropdownMenuItem(
                        value: provider,
                        child: Row(
                          children: [
                            _getProviderIcon(provider),
                            const SizedBox(width: 8),
                            Text(_getProviderName(provider)),
                          ],
                        ),
                      );
                    }).toList(),
                    onChanged: (value) {
                      setState(() {
                        _selectedProvider = value!;
                      });
                    },
                  ),
                  const SizedBox(height: 12),
                  TextFormField(
                    controller: _phoneNumberController,
                    decoration: const InputDecoration(
                      labelText: 'Phone Number',
                      border: OutlineInputBorder(),
                      hintText: '0241234567',
                      prefixText: '+233 ',
                    ),
                    keyboardType: TextInputType.phone,
                    inputFormatters: [
                      FilteringTextInputFormatter.digitsOnly,
                      LengthLimitingTextInputFormatter(10),
                    ],
                  ),
                  const SizedBox(height: 12),
                  TextFormField(
                    controller: _voucherController,
                    decoration: const InputDecoration(
                      labelText: 'Voucher (Optional)',
                      border: OutlineInputBorder(),
                      hintText: 'Enter voucher code if available',
                    ),
                  ),
                ],
              ),
            ),
          ),
          const Spacer(),
          ElevatedButton(
            onPressed: _canProcessMobileMoneyPayment() && !_isProcessing
                ? _processMobileMoneyPayment
                : null,
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.orange,
              foregroundColor: Colors.white,
              padding: const EdgeInsets.all(16.0),
            ),
            child: _isProcessing
                ? const CircularProgressIndicator(color: Colors.white)
                : const Text('Send Payment Request'),
          ),
        ],
      ),
    );
  }

  Widget _buildQRPayment() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'QR Code Payment',
                    style: Theme.of(context).textTheme.titleLarge,
                  ),
                  const SizedBox(height: 16),
                  const Text(
                    'Scan the customer\'s payment QR code to process payment.',
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: 16),
          if (_isScanning)
            Expanded(
              child: _buildQRScanner(),
            )
          else
            Expanded(
              child: Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Icon(
                      Icons.qr_code_scanner,
                      size: 120,
                      color: Colors.grey.shade400,
                    ),
                    const SizedBox(height: 16),
                    ElevatedButton.icon(
                      onPressed: _startQRScanning,
                      icon: const Icon(Icons.qr_code_scanner),
                      label: const Text('Start QR Scanner'),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.purple,
                        foregroundColor: Colors.white,
                        padding: const EdgeInsets.all(16.0),
                      ),
                    ),
                  ],
                ),
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildNFCPayment() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'NFC Payment',
                    style: Theme.of(context).textTheme.titleLarge,
                  ),
                  const SizedBox(height: 16),
                  const Text(
                    'Hold the customer\'s contactless card or device near the NFC reader.',
                  ),
                ],
              ),
            ),
          ),
          const Spacer(),
          Container(
            height: 200,
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [
                  Colors.blue.shade100,
                  Colors.blue.shade300,
                ],
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
              ),
              borderRadius: BorderRadius.circular(16.0),
            ),
            child: Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(
                    Icons.nfc,
                    size: 80,
                    color: Colors.blue.shade700,
                  ),
                  const SizedBox(height: 16),
                  Text(
                    'Hold device here',
                    style: TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                      color: Colors.blue.shade700,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    'Waiting for NFC device...',
                    style: TextStyle(
                      color: Colors.blue.shade600,
                    ),
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: 16),
          ElevatedButton(
            onPressed: !_isProcessing ? _processNFCPayment : null,
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.indigo,
              foregroundColor: Colors.white,
              padding: const EdgeInsets.all(16.0),
            ),
            child: _isProcessing
                ? const CircularProgressIndicator(color: Colors.white)
                : const Text('Simulate NFC Payment'),
          ),
        ],
      ),
    );
  }

  Widget _buildSplitPayment() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Split Payment',
                    style: Theme.of(context).textTheme.titleLarge,
                  ),
                  const SizedBox(height: 8),
                  Text(
                    'Total: GHS ${widget.totalAmount.toStringAsFixed(2)}',
                    style: Theme.of(context).textTheme.titleMedium,
                  ),
                  Text(
                    'Split Total: GHS ${_splitTotal.toStringAsFixed(2)}',
                    style: TextStyle(
                      color: _splitTotal == widget.totalAmount 
                          ? Colors.green 
                          : Colors.orange,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    'Remaining: GHS ${(widget.totalAmount - _splitTotal).toStringAsFixed(2)}',
                    style: const TextStyle(color: Colors.red),
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: 16),
          if (_splitPayments.isNotEmpty) ...[
            Expanded(
              child: ListView.builder(
                itemCount: _splitPayments.length,
                itemBuilder: (context, index) {
                  final split = _splitPayments[index];
                  return Card(
                    child: ListTile(
                      leading: _getPaymentMethodIcon(split['method']),
                      title: Text(_getPaymentMethodName(split['method'])),
                      subtitle: Text('GHS ${split['amount'].toStringAsFixed(2)}'),
                      trailing: IconButton(
                        icon: const Icon(Icons.delete),
                        onPressed: () => _removeSplitPayment(index),
                      ),
                    ),
                  );
                },
              ),
            ),
          ] else
            const Expanded(
              child: Center(
                child: Text(
                  'No split payments added yet.\nTap "Add Split Payment" to begin.',
                  textAlign: TextAlign.center,
                  style: TextStyle(fontSize: 16, color: Colors.grey),
                ),
              ),
            ),
          ElevatedButton.icon(
            onPressed: _addSplitPayment,
            icon: const Icon(Icons.add),
            label: const Text('Add Split Payment'),
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.teal,
              foregroundColor: Colors.white,
            ),
          ),
          const SizedBox(height: 8),
          ElevatedButton(
            onPressed: _canProcessSplitPayment() && !_isProcessing
                ? _processSplitPayments
                : null,
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.green,
              foregroundColor: Colors.white,
              padding: const EdgeInsets.all(16.0),
            ),
            child: _isProcessing
                ? const CircularProgressIndicator(color: Colors.white)
                : const Text('Complete Split Payment'),
          ),
        ],
      ),
    );
  }

  Widget _buildQRScanner() {
    return MobileScanner(
      controller: _scannerController,
      onDetect: (capture) {
        final List<Barcode> barcodes = capture.barcodes;
        for (final barcode in barcodes) {
          if (barcode.rawValue != null) {
            _processQRPayment(barcode.rawValue!);
            break;
          }
        }
      },
    );
  }

  // Payment processing methods
  bool _canProcessCashPayment() {
    final cashReceived = double.tryParse(_cashReceivedController.text) ?? 0.0;
    return cashReceived >= widget.totalAmount;
  }

  bool _canProcessCardPayment() {
    return _cardNumberController.text.length >= 13 &&
        _expiryController.text.length == 5 &&
        _cvvController.text.length >= 3;
  }

  bool _canProcessMobileMoneyPayment() {
    return _phoneNumberController.text.length >= 9;
  }

  bool _canProcessSplitPayment() {
    return _splitPayments.isNotEmpty && _splitTotal == widget.totalAmount;
  }

  Future<void> _processCashPayment() async {
    setState(() {
      _isProcessing = true;
    });

    final cashReceived = double.tryParse(_cashReceivedController.text) ?? 0.0;
    
    final result = await _paymentService.processCashPayment(
      orderId: widget.orderId,
      amount: widget.totalAmount,
      currency: widget.currency,
      cashReceived: cashReceived,
    );

    setState(() {
      _isProcessing = false;
    });

    if (result.isSuccess) {
      _vibrateFeedback();
      widget.onPaymentComplete(result);
    } else {
      _showErrorDialog(result.message ?? 'Payment failed');
    }
  }

  Future<void> _processCardPayment() async {
    setState(() {
      _isProcessing = true;
    });

    final result = await _paymentService.processCardPayment(
      orderId: widget.orderId,
      amount: widget.totalAmount,
      currency: widget.currency,
      cardNumber: _cardNumberController.text.replaceAll(' ', ''),
      expiryDate: _expiryController.text,
      cvv: _cvvController.text,
      cardHolderName: _cardHolderController.text,
    );

    setState(() {
      _isProcessing = false;
    });

    if (result.isSuccess) {
      _vibrateFeedback();
      widget.onPaymentComplete(result);
    } else {
      _showErrorDialog(result.message ?? 'Payment failed');
    }
  }

  Future<void> _processMobileMoneyPayment() async {
    setState(() {
      _isProcessing = true;
    });

    final result = await _paymentService.processMobileMoneyPayment(
      orderId: widget.orderId,
      amount: widget.totalAmount,
      currency: widget.currency,
      phoneNumber: _phoneNumberController.text,
      provider: _selectedProvider,
      voucher: _voucherController.text.isEmpty ? null : _voucherController.text,
    );

    setState(() {
      _isProcessing = false;
    });

    if (result.isSuccess) {
      _vibrateFeedback();
      widget.onPaymentComplete(result);
    } else {
      _showErrorDialog(result.message ?? 'Payment failed');
    }
  }

  void _startQRScanning() {
    setState(() {
      _isScanning = true;
      _scannerController = MobileScannerController();
    });
  }

  Future<void> _processQRPayment(String qrData) async {
    setState(() {
      _isScanning = false;
      _isProcessing = true;
    });

    _scannerController?.dispose();
    _scannerController = null;

    final result = await _paymentService.processQRPayment(
      orderId: widget.orderId,
      amount: widget.totalAmount,
      currency: widget.currency,
      qrData: qrData,
    );

    setState(() {
      _isProcessing = false;
    });

    if (result.isSuccess) {
      _vibrateFeedback();
      widget.onPaymentComplete(result);
    } else {
      _showErrorDialog(result.message ?? 'Payment failed');
    }
  }

  Future<void> _processNFCPayment() async {
    setState(() {
      _isProcessing = true;
    });

    // Simulate NFC data
    final nfcData = 'nfc_${DateTime.now().millisecondsSinceEpoch}';

    final result = await _paymentService.processNFCPayment(
      orderId: widget.orderId,
      amount: widget.totalAmount,
      currency: widget.currency,
      nfcData: nfcData,
    );

    setState(() {
      _isProcessing = false;
    });

    if (result.isSuccess) {
      _vibrateFeedback();
      widget.onPaymentComplete(result);
    } else {
      _showErrorDialog(result.message ?? 'Payment failed');
    }
  }

  void _addSplitPayment() {
    showDialog(
      context: context,
      builder: (context) => _SplitPaymentDialog(
        remainingAmount: widget.totalAmount - _splitTotal,
        onPaymentAdded: (method, amount) {
          setState(() {
            _splitPayments.add({
              'method': method,
              'amount': amount,
              'metadata': {},
            });
            _splitTotal += amount;
          });
        },
      ),
    );
  }

  void _removeSplitPayment(int index) {
    setState(() {
      _splitTotal -= _splitPayments[index]['amount'];
      _splitPayments.removeAt(index);
    });
  }

  Future<void> _processSplitPayments() async {
    setState(() {
      _isProcessing = true;
    });

    final result = await _paymentService.processSplitPayment(
      orderId: widget.orderId,
      totalAmount: widget.totalAmount,
      currency: widget.currency,
      paymentSplits: _splitPayments,
    );

    setState(() {
      _isProcessing = false;
    });

    if (result.isSuccess) {
      _vibrateFeedback();
      widget.onPaymentComplete(result);
    } else {
      _showErrorDialog(result.message ?? 'Payment failed');
    }
  }

  // Helper methods
  void _vibrateFeedback() {
    Vibration.hasVibrator().then((hasVibrator) {
      if (hasVibrator == true) {
        Vibration.vibrate(duration: 200);
      }
    });
  }

  void _showErrorDialog(String message) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Payment Failed'),
        content: Text(message),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('OK'),
          ),
        ],
      ),
    );
  }

  Icon _getProviderIcon(MobileMoneyProvider provider) {
    switch (provider) {
      case MobileMoneyProvider.mtn:
        return const Icon(Icons.phone_android, color: Colors.yellow);
      case MobileMoneyProvider.vodafone:
        return const Icon(Icons.phone_android, color: Colors.red);
      case MobileMoneyProvider.airteltigo:
        return const Icon(Icons.phone_android, color: Colors.blue);
      case MobileMoneyProvider.telecel:
        return const Icon(Icons.phone_android, color: Colors.green);
      default:
        return const Icon(Icons.phone_android, color: Colors.grey);
    }
  }

  String _getProviderName(MobileMoneyProvider provider) {
    switch (provider) {
      case MobileMoneyProvider.mtn:
        return 'MTN Mobile Money';
      case MobileMoneyProvider.vodafone:
        return 'Vodafone Cash';
      case MobileMoneyProvider.airteltigo:
        return 'AirtelTigo Money';
      case MobileMoneyProvider.telecel:
        return 'Telecel Cash';
      default:
        return 'Other';
    }
  }

  Icon _getPaymentMethodIcon(PaymentMethod method) {
    switch (method) {
      case PaymentMethod.cash:
        return const Icon(Icons.money, color: Colors.green);
      case PaymentMethod.card:
        return const Icon(Icons.credit_card, color: Colors.blue);
      case PaymentMethod.mobileMoney:
        return const Icon(Icons.phone_android, color: Colors.orange);
      default:
        return const Icon(Icons.payment, color: Colors.grey);
    }
  }

  String _getPaymentMethodName(PaymentMethod method) {
    switch (method) {
      case PaymentMethod.cash:
        return 'Cash';
      case PaymentMethod.card:
        return 'Card';
      case PaymentMethod.mobileMoney:
        return 'Mobile Money';
      default:
        return 'Other';
    }
  }
}

// Split payment dialog
class _SplitPaymentDialog extends StatefulWidget {
  final double remainingAmount;
  final Function(PaymentMethod, double) onPaymentAdded;

  const _SplitPaymentDialog({
    Key? key,
    required this.remainingAmount,
    required this.onPaymentAdded,
  }) : super(key: key);

  @override
  State<_SplitPaymentDialog> createState() => _SplitPaymentDialogState();
}

class _SplitPaymentDialogState extends State<_SplitPaymentDialog> {
  PaymentMethod _selectedMethod = PaymentMethod.cash;
  final TextEditingController _amountController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Add Split Payment'),
      content: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text('Remaining Amount: GHS ${widget.remainingAmount.toStringAsFixed(2)}'),
          const SizedBox(height: 16),
          DropdownButtonFormField<PaymentMethod>(
            value: _selectedMethod,
            decoration: const InputDecoration(
              labelText: 'Payment Method',
              border: OutlineInputBorder(),
            ),
            items: [
              PaymentMethod.cash,
              PaymentMethod.card,
              PaymentMethod.mobileMoney,
            ].map((method) {
              return DropdownMenuItem(
                value: method,
                child: Text(method.toString().split('.').last),
              );
            }).toList(),
            onChanged: (value) {
              setState(() {
                _selectedMethod = value!;
              });
            },
          ),
          const SizedBox(height: 12),
          TextFormField(
            controller: _amountController,
            decoration: const InputDecoration(
              labelText: 'Amount',
              border: OutlineInputBorder(),
              prefixText: 'GHS ',
            ),
            keyboardType: TextInputType.number,
            inputFormatters: [
              FilteringTextInputFormatter.allow(RegExp(r'^\d+\.?\d{0,2}')),
            ],
          ),
        ],
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.of(context).pop(),
          child: const Text('Cancel'),
        ),
        ElevatedButton(
          onPressed: _canAddPayment() ? _addPayment : null,
          child: const Text('Add'),
        ),
      ],
    );
  }

  bool _canAddPayment() {
    final amount = double.tryParse(_amountController.text) ?? 0.0;
    return amount > 0 && amount <= widget.remainingAmount;
  }

  void _addPayment() {
    final amount = double.tryParse(_amountController.text) ?? 0.0;
    widget.onPaymentAdded(_selectedMethod, amount);
    Navigator.of(context).pop();
  }
}

// Input formatters
class _CardNumberInputFormatter extends TextInputFormatter {
  @override
  TextEditingValue formatEditUpdate(
    TextEditingValue oldValue,
    TextEditingValue newValue,
  ) {
    final text = newValue.text.replaceAll(' ', '');
    final buffer = StringBuffer();
    
    for (int i = 0; i < text.length; i++) {
      if (i % 4 == 0 && i != 0) {
        buffer.write(' ');
      }
      buffer.write(text[i]);
    }
    
    return newValue.copyWith(
      text: buffer.toString(),
      selection: TextSelection.collapsed(offset: buffer.length),
    );
  }
}

class _ExpiryDateInputFormatter extends TextInputFormatter {
  @override
  TextEditingValue formatEditUpdate(
    TextEditingValue oldValue,
    TextEditingValue newValue,
  ) {
    final text = newValue.text;
    if (text.length == 2 && !text.contains('/')) {
      return newValue.copyWith(
        text: '$text/',
        selection: const TextSelection.collapsed(offset: 3),
      );
    }
    return newValue;
  }
}
