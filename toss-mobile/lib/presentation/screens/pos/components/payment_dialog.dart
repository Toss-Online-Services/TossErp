import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../domain/entities/payment_entity.dart';
import '../../../../domain/entities/sales_transaction_entity.dart';
import '../../../../domain/entities/customer_entity.dart';

class PaymentDialog extends StatefulWidget {
  final List<SalesTransactionItemEntity> cartItems;
  final CustomerEntity? customer;
  final Function(List<PaymentEntity>) onPaymentCompleted;

  const PaymentDialog({
    super.key,
    required this.cartItems,
    this.customer,
    required this.onPaymentCompleted,
  });

  @override
  State<PaymentDialog> createState() => _PaymentDialogState();
}

class _PaymentDialogState extends State<PaymentDialog>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  final List<PaymentEntity> _payments = [];
  final TextEditingController _cashReceivedController = TextEditingController();
  final TextEditingController _cardReferenceController = TextEditingController();
  final TextEditingController _customAmountController = TextEditingController();

  // Payment calculation
  late int _totalAmount;
  late int _totalTax;
  late int _totalDiscount;
  late int _subtotal;
  int _loyaltyPointsUsed = 0;
  int _loyaltyPointsEarned = 0;
  bool _usePartialLoyaltyPoints = false;

  // Payment method tracking
  PaymentMethod _selectedPaymentMethod = PaymentMethod.cash;
  int _remainingAmount = 0;
  int _changeAmount = 0;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
    _calculateTotals();
    _calculateLoyaltyPoints();
  }

  @override
  void dispose() {
    _tabController.dispose();
    _cashReceivedController.dispose();
    _cardReferenceController.dispose();
    _customAmountController.dispose();
    super.dispose();
  }

  void _calculateTotals() {
    _subtotal = widget.cartItems.fold(0, (sum, item) => sum + item.totalPrice);
    _totalDiscount = widget.cartItems.fold(0, (sum, item) => sum + item.discountAmount);
    _totalTax = (_subtotal * 0.1).round(); // 10% tax rate
    _totalAmount = _subtotal - _totalDiscount + _totalTax;
    _remainingAmount = _totalAmount;

    // Apply loyalty points discount if customer selected
    if (widget.customer != null && _loyaltyPointsUsed > 0) {
      final loyaltyDiscount = _loyaltyPointsUsed; // 1 point = $0.01
      _remainingAmount = (_totalAmount - loyaltyDiscount).clamp(0, _totalAmount);
    }

    // Calculate paid amount
    final paidAmount = _payments.fold(0, (sum, payment) => sum + payment.amount);
    _remainingAmount = (_totalAmount - paidAmount - _loyaltyPointsUsed).clamp(0, _totalAmount);
  }

  void _calculateLoyaltyPoints() {
    if (widget.customer == null) return;
    
    // Earn 1 point per dollar spent (after discounts and tax)
    _loyaltyPointsEarned = ((_totalAmount - _totalDiscount) / 100).floor();
  }

  @override
  Widget build(BuildContext context) {
    return Dialog(
      child: Container(
        width: MediaQuery.of(context).size.width * 0.9,
        height: MediaQuery.of(context).size.height * 0.8,
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            _buildHeader(),
            const SizedBox(height: 16),
            _buildOrderSummary(),
            const SizedBox(height: 16),
            if (widget.customer != null) _buildLoyaltySection(),
            _buildPaymentTabs(),
            const SizedBox(height: 16),
            _buildPaymentSummary(),
            const SizedBox(height: 16),
            _buildActionButtons(),
          ],
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Row(
      children: [
        Icon(
          Icons.payment,
          color: Theme.of(context).colorScheme.primary,
        ),
        const SizedBox(width: 8),
        Text(
          'Payment',
          style: Theme.of(context).textTheme.titleLarge?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const Spacer(),
        IconButton(
          onPressed: () => Navigator.of(context).pop(),
          icon: const Icon(Icons.close),
        ),
      ],
    );
  }

  Widget _buildOrderSummary() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceVariant.withOpacity(0.3),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Column(
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Subtotal:'),
              Text('\$${(_subtotal / 100).toStringAsFixed(2)}'),
            ],
          ),
          if (_totalDiscount > 0)
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text('Discount:', style: TextStyle(color: Colors.red)),
                Text('-\$${(_totalDiscount / 100).toStringAsFixed(2)}',
                    style: const TextStyle(color: Colors.red)),
              ],
            ),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Tax:'),
              Text('\$${(_totalTax / 100).toStringAsFixed(2)}'),
            ],
          ),
          const Divider(),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Total:',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              Text(
                '\$${(_totalAmount / 100).toStringAsFixed(2)}',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                  color: Theme.of(context).colorScheme.primary,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildLoyaltySection() {
    return Container(
      margin: const EdgeInsets.only(bottom: 16),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.amber.withOpacity(0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Colors.amber.withOpacity(0.3)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              const Icon(Icons.stars, color: Colors.amber),
              const SizedBox(width: 8),
              Text(
                'Loyalty Points',
                style: Theme.of(context).textTheme.titleSmall?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text('Available: ${widget.customer!.loyaltyPoints} points'),
              Text('Will Earn: $_loyaltyPointsEarned points'),
            ],
          ),
          if (widget.customer!.loyaltyPoints > 0) ...[
            const SizedBox(height: 12),
            Row(
              children: [
                Checkbox(
                  value: _usePartialLoyaltyPoints,
                  onChanged: (value) {
                    setState(() {
                      _usePartialLoyaltyPoints = value ?? false;
                      if (_usePartialLoyaltyPoints) {
                        _loyaltyPointsUsed = (widget.customer!.loyaltyPoints)
                            .clamp(0, _remainingAmount);
                      } else {
                        _loyaltyPointsUsed = 0;
                      }
                      _calculateTotals();
                    });
                  },
                ),
                const Text('Use '),
                Expanded(
                  child: TextFormField(
                    initialValue: _loyaltyPointsUsed.toString(),
                    enabled: _usePartialLoyaltyPoints,
                    keyboardType: TextInputType.number,
                    inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                    decoration: const InputDecoration(
                      border: OutlineInputBorder(),
                      contentPadding: EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    ),
                    onChanged: (value) {
                      final points = int.tryParse(value) ?? 0;
                      setState(() {
                        _loyaltyPointsUsed = points.clamp(0, 
                            (widget.customer!.loyaltyPoints).clamp(0, _totalAmount));
                        _calculateTotals();
                      });
                    },
                  ),
                ),
                const Text(' points'),
              ],
            ),
            if (_loyaltyPointsUsed > 0)
              Padding(
                padding: const EdgeInsets.only(top: 8),
                child: Text(
                  'Loyalty Discount: -\$${(_loyaltyPointsUsed / 100).toStringAsFixed(2)}',
                  style: const TextStyle(
                    color: Colors.green,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ),
          ],
        ],
      ),
    );
  }

  Widget _buildPaymentTabs() {
    return Expanded(
      child: Column(
        children: [
          TabBar(
            controller: _tabController,
            tabs: const [
              Tab(icon: Icon(Icons.money), text: 'Cash'),
              Tab(icon: Icon(Icons.credit_card), text: 'Card'),
              Tab(icon: Icon(Icons.more_horiz), text: 'Other'),
            ],
          ),
          Expanded(
            child: TabBarView(
              controller: _tabController,
              children: [
                _buildCashPaymentTab(),
                _buildCardPaymentTab(),
                _buildOtherPaymentTab(),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCashPaymentTab() {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          TextField(
            controller: _cashReceivedController,
            keyboardType: TextInputType.number,
            inputFormatters: [
              FilteringTextInputFormatter.allow(RegExp(r'^\d+\.?\d{0,2}')),
            ],
            decoration: InputDecoration(
              labelText: 'Cash Received',
              prefixText: '\$',
              border: const OutlineInputBorder(),
              suffixIcon: IconButton(
                icon: const Icon(Icons.calculate),
                onPressed: () {
                  _cashReceivedController.text = (_remainingAmount / 100).toStringAsFixed(2);
                  _calculateChange();
                },
              ),
            ),
            onChanged: (value) => _calculateChange(),
          ),
          const SizedBox(height: 16),
          // Quick amount buttons
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: [
              _buildQuickAmountButton(5.00),
              _buildQuickAmountButton(10.00),
              _buildQuickAmountButton(20.00),
              _buildQuickAmountButton(50.00),
              _buildQuickAmountButton(100.00),
            ],
          ),
          const SizedBox(height: 16),
          if (_changeAmount > 0)
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: Colors.green.withOpacity(0.1),
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: Colors.green.withOpacity(0.3)),
              ),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    'Change:',
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    '\$${(_changeAmount / 100).toStringAsFixed(2)}',
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                      color: Colors.green,
                    ),
                  ),
                ],
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildCardPaymentTab() {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          // Card payment options
          ListTile(
            leading: const Icon(Icons.credit_card),
            title: const Text('Credit Card'),
            trailing: Radio<PaymentMethod>(
              value: PaymentMethod.card,
              groupValue: _selectedPaymentMethod,
              onChanged: (value) {
                setState(() {
                  _selectedPaymentMethod = value!;
                });
              },
            ),
          ),
          ListTile(
            leading: const Icon(Icons.contactless),
            title: const Text('Contactless'),
            trailing: Radio<PaymentMethod>(
              value: PaymentMethod.nfc,
              groupValue: _selectedPaymentMethod,
              onChanged: (value) {
                setState(() {
                  _selectedPaymentMethod = value!;
                });
              },
            ),
          ),
          const SizedBox(height: 16),
          TextField(
            controller: _cardReferenceController,
            decoration: const InputDecoration(
              labelText: 'Reference Number (Optional)',
              border: OutlineInputBorder(),
            ),
          ),
          const SizedBox(height: 16),
          ElevatedButton.icon(
            onPressed: () => _processCardPayment(),
            icon: const Icon(Icons.payment),
            label: Text('Process \$${(_remainingAmount / 100).toStringAsFixed(2)}'),
          ),
        ],
      ),
    );
  }

  Widget _buildOtherPaymentTab() {
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          ListTile(
            leading: const Icon(Icons.qr_code),
            title: const Text('QR Payment'),
            onTap: () => _processQRPayment(),
          ),
          ListTile(
            leading: const Icon(Icons.account_balance),
            title: const Text('Bank Transfer'),
            onTap: () => _processBankTransfer(),
          ),
          ListTile(
            leading: const Icon(Icons.phone_android),
            title: const Text('Mobile Money'),
            onTap: () => _processMobileMoneyPayment(),
          ),
          const SizedBox(height: 16),
          TextField(
            controller: _customAmountController,
            keyboardType: TextInputType.number,
            decoration: const InputDecoration(
              labelText: 'Custom Amount',
              prefixText: '\$',
              border: OutlineInputBorder(),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildQuickAmountButton(double amount) {
    return ElevatedButton(
      onPressed: () {
        _cashReceivedController.text = amount.toStringAsFixed(2);
        _calculateChange();
      },
      style: ElevatedButton.styleFrom(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      ),
      child: Text('\$${amount.toStringAsFixed(0)}'),
    );
  }

  Widget _buildPaymentSummary() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.primaryContainer.withOpacity(0.3),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'Remaining:',
                style: Theme.of(context).textTheme.titleMedium,
              ),
              if (_payments.isNotEmpty)
                Text(
                  '${_payments.length} payment(s) added',
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: Colors.grey[600],
                  ),
                ),
            ],
          ),
          Text(
            '\$${(_remainingAmount / 100).toStringAsFixed(2)}',
            style: Theme.of(context).textTheme.titleLarge?.copyWith(
              fontWeight: FontWeight.bold,
              color: _remainingAmount > 0 
                  ? Colors.red 
                  : Theme.of(context).colorScheme.primary,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildActionButtons() {
    final canComplete = _remainingAmount <= 0;
    
    return Row(
      children: [
        Expanded(
          child: OutlinedButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
        ),
        const SizedBox(width: 16),
        Expanded(
          flex: 2,
          child: ElevatedButton(
            onPressed: canComplete ? _completePayment : null,
            child: Text(canComplete ? 'Complete Payment' : 'Add Payment'),
          ),
        ),
      ],
    );
  }

  void _calculateChange() {
    final cashReceived = (double.tryParse(_cashReceivedController.text) ?? 0) * 100;
    setState(() {
      _changeAmount = (cashReceived - _remainingAmount).clamp(0, double.infinity).toInt();
    });
  }

  void _processCardPayment() {
    // TODO: Integrate with actual card processing
    final payment = PaymentEntity(
      id: DateTime.now().millisecondsSinceEpoch.toString(),
      transactionId: '',
      amount: _remainingAmount,
      method: _selectedPaymentMethod,
      status: PaymentStatus.completed,
      reference: _cardReferenceController.text,
      createdAt: DateTime.now(),
    );

    setState(() {
      _payments.add(payment);
      _calculateTotals();
    });

    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Card payment processed successfully')),
    );
  }

  void _processQRPayment() {
    // TODO: Implement QR payment processing
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('QR Payment not implemented yet')),
    );
  }

  void _processBankTransfer() {
    // TODO: Implement bank transfer processing
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Bank Transfer not implemented yet')),
    );
  }

  void _processMobileMoneyPayment() {
    // TODO: Implement mobile money processing
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Mobile Money not implemented yet')),
    );
  }

  void _completePayment() {
    // Add cash payment if cash tab is active and amount entered
    if (_tabController.index == 0 && _cashReceivedController.text.isNotEmpty) {
      final cashAmount = (double.tryParse(_cashReceivedController.text) ?? 0) * 100;
      if (cashAmount > 0) {
        final cashPayment = PaymentEntity(
          id: DateTime.now().millisecondsSinceEpoch.toString(),
          transactionId: '',
          amount: _remainingAmount.clamp(0, cashAmount.toInt()),
          method: PaymentMethod.cash,
          status: PaymentStatus.completed,
          reference: 'Cash payment',
          createdAt: DateTime.now(),
        );
        _payments.add(cashPayment);
      }
    }

    // Add loyalty points payment if used
    if (_loyaltyPointsUsed > 0) {
      final loyaltyPayment = PaymentEntity(
        id: '${DateTime.now().millisecondsSinceEpoch}_loyalty',
        transactionId: '',
        amount: _loyaltyPointsUsed,
        method: PaymentMethod.loyaltyPoints,
        status: PaymentStatus.completed,
        reference: 'Loyalty points redeemed',
        createdAt: DateTime.now(),
      );
      _payments.add(loyaltyPayment);
    }

    widget.onPaymentCompleted(_payments);
    Navigator.of(context).pop();
  }
}
