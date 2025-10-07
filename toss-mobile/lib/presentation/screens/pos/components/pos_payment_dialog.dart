import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

enum PaymentMethodType {
  cash('Cash', Icons.money),
  card('Card', Icons.credit_card),
  digital('Digital Wallet', Icons.wallet),
  bankTransfer('Bank Transfer', Icons.account_balance),
  cheque('Cheque', Icons.receipt_long),
  giftCard('Gift Card', Icons.card_giftcard),
  loyaltyPoints('Loyalty Points', Icons.stars);

  const PaymentMethodType(this.displayName, this.icon);
  final String displayName;
  final IconData icon;
}

class PaymentMethod {
  final PaymentMethodType type;
  final double amount;
  final String? reference;
  final Map<String, dynamic>? metadata;

  PaymentMethod({
    required this.type,
    required this.amount,
    this.reference,
    this.metadata,
  });
}

class POSPaymentDialog extends StatefulWidget {
  final double total;
  final Function(List<PaymentMethod> payments, double changeAmount) onPaymentComplete;
  final String? customerLoyaltyPoints;

  const POSPaymentDialog({
    super.key,
    required this.total,
    required this.onPaymentComplete,
    this.customerLoyaltyPoints,
  });

  @override
  State<POSPaymentDialog> createState() => _POSPaymentDialogState();
}

class _POSPaymentDialogState extends State<POSPaymentDialog> with TickerProviderStateMixin {
  late TabController _tabController;
  List<PaymentMethod> _payments = [];
  PaymentMethodType _selectedMethod = PaymentMethodType.cash;
  final TextEditingController _amountController = TextEditingController();
  final TextEditingController _referenceController = TextEditingController();
  final TextEditingController _cashReceivedController = TextEditingController();
  
  // Numeric keypad controller
  String _displayAmount = '0.00';

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    _cashReceivedController.text = widget.total.toStringAsFixed(2);
    _displayAmount = widget.total.toStringAsFixed(2);
  }

  @override
  void dispose() {
    _tabController.dispose();
    _amountController.dispose();
    _referenceController.dispose();
    _cashReceivedController.dispose();
    super.dispose();
  }

  double get _totalPaid => _payments.fold(0, (sum, payment) => sum + payment.amount);
  double get _remainingAmount => widget.total - _totalPaid;
  double get _changeAmount => _totalPaid - widget.total;
  bool get _canComplete => _remainingAmount <= 0;

  @override
  Widget build(BuildContext context) {
    return Dialog(
      child: Container(
        width: MediaQuery.of(context).size.width * 0.9,
        height: MediaQuery.of(context).size.height * 0.8,
        padding: const EdgeInsets.all(20),
        child: Column(
          children: [
            _buildHeader(),
            const SizedBox(height: 20),
            Expanded(
              child: TabBarView(
                controller: _tabController,
                children: [
                  _buildQuickPaymentTab(),
                  _buildSplitPaymentTab(),
                ],
              ),
            ),
            _buildPaymentSummary(),
            const SizedBox(height: 20),
            _buildActionButtons(),
          ],
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Column(
      children: [
        Row(
          children: [
            Icon(
              Icons.payment,
              color: Theme.of(context).colorScheme.primary,
              size: 28,
            ),
            const SizedBox(width: 12),
            Text(
              'Payment',
              style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const Spacer(),
            IconButton(
              onPressed: () => Navigator.of(context).pop(),
              icon: const Icon(Icons.close),
            ),
          ],
        ),
        const SizedBox(height: 16),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
          decoration: BoxDecoration(
            color: Theme.of(context).colorScheme.primaryContainer,
            borderRadius: BorderRadius.circular(12),
          ),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Total Amount:',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w600,
                ),
              ),
              Text(
                '\$${widget.total.toStringAsFixed(2)}',
                style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.bold,
                  color: Theme.of(context).colorScheme.primary,
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 16),
        TabBar(
          controller: _tabController,
          tabs: const [
            Tab(
              icon: Icon(Icons.flash_on),
              text: 'Quick Pay',
            ),
            Tab(
              icon: Icon(Icons.call_split),
              text: 'Split Payment',
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildQuickPaymentTab() {
    return SingleChildScrollView(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const SizedBox(height: 20),
          Text(
            'Payment Method',
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.w600,
            ),
          ),
          const SizedBox(height: 12),
          _buildPaymentMethodGrid(),
          const SizedBox(height: 24),
          if (_selectedMethod == PaymentMethodType.cash) ...[
            _buildCashPaymentSection(),
          ] else ...[
            _buildOtherPaymentSection(),
          ],
        ],
      ),
    );
  }

  Widget _buildPaymentMethodGrid() {
    return GridView.count(
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      crossAxisCount: 3,
      childAspectRatio: 1.2,
      mainAxisSpacing: 12,
      crossAxisSpacing: 12,
      children: PaymentMethodType.values.map((method) {
        final isSelected = _selectedMethod == method;
        return GestureDetector(
          onTap: () {
            setState(() {
              _selectedMethod = method;
            });
          },
          child: Container(
            decoration: BoxDecoration(
              color: isSelected
                  ? Theme.of(context).colorScheme.primaryContainer
                  : Theme.of(context).colorScheme.surface,
              border: Border.all(
                color: isSelected
                    ? Theme.of(context).colorScheme.primary
                    : Theme.of(context).colorScheme.outline.withOpacity(0.5),
                width: isSelected ? 2 : 1,
              ),
              borderRadius: BorderRadius.circular(12),
            ),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(
                  method.icon,
                  color: isSelected
                      ? Theme.of(context).colorScheme.primary
                      : Theme.of(context).colorScheme.onSurface,
                  size: 32,
                ),
                const SizedBox(height: 8),
                Text(
                  method.displayName,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    fontWeight: isSelected ? FontWeight.w600 : FontWeight.normal,
                    color: isSelected
                        ? Theme.of(context).colorScheme.primary
                        : Theme.of(context).colorScheme.onSurface,
                  ),
                  textAlign: TextAlign.center,
                ),
              ],
            ),
          ),
        );
      }).toList(),
    );
  }

  Widget _buildCashPaymentSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Cash Received',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.w600,
          ),
        ),
        const SizedBox(height: 12),
        Row(
          children: [
            Expanded(
              flex: 2,
              child: TextField(
                controller: _cashReceivedController,
                keyboardType: TextInputType.number,
                inputFormatters: [
                  FilteringTextInputFormatter.allow(RegExp(r'^\d+\.?\d{0,2}')),
                ],
                decoration: const InputDecoration(
                  labelText: 'Amount Received',
                  prefixText: '\$',
                  border: OutlineInputBorder(),
                ),
                onChanged: (value) {
                  setState(() {
                    _displayAmount = value;
                  });
                },
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              flex: 3,
              child: _buildNumericKeypad(),
            ),
          ],
        ),
        const SizedBox(height: 16),
        _buildQuickAmountButtons(),
        const SizedBox(height: 16),
        _buildChangeCalculation(),
      ],
    );
  }

  Widget _buildOtherPaymentSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        TextField(
          controller: _amountController,
          keyboardType: TextInputType.number,
          inputFormatters: [
            FilteringTextInputFormatter.allow(RegExp(r'^\d+\.?\d{0,2}')),
          ],
          decoration: const InputDecoration(
            labelText: 'Amount',
            prefixText: '\$',
            border: OutlineInputBorder(),
          ),
        ),
        const SizedBox(height: 16),
        TextField(
          controller: _referenceController,
          decoration: InputDecoration(
            labelText: _getReferenceLabel(_selectedMethod),
            border: const OutlineInputBorder(),
          ),
        ),
        const SizedBox(height: 16),
        if (_selectedMethod == PaymentMethodType.loyaltyPoints)
          _buildLoyaltyPointsSection(),
      ],
    );
  }

  Widget _buildNumericKeypad() {
    return Container(
      padding: const EdgeInsets.all(8),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceVariant,
        borderRadius: BorderRadius.circular(12),
      ),
      child: Column(
        children: [
          for (int row = 0; row < 4; row++)
            Row(
              children: [
                for (int col = 0; col < 3; col++)
                  Expanded(
                    child: _buildKeypadButton(row, col),
                  ),
              ],
            ),
        ],
      ),
    );
  }

  Widget _buildKeypadButton(int row, int col) {
    String text;
    VoidCallback? onPressed;

    if (row == 3) {
      if (col == 0) {
        text = '.';
        onPressed = () => _addToAmount('.');
      } else if (col == 1) {
        text = '0';
        onPressed = () => _addToAmount('0');
      } else {
        text = '⌫';
        onPressed = _backspaceAmount;
      }
    } else {
      final number = row * 3 + col + 1;
      text = '$number';
      onPressed = () => _addToAmount('$number');
    }

    return Container(
      margin: const EdgeInsets.all(2),
      child: ElevatedButton(
        onPressed: onPressed,
        style: ElevatedButton.styleFrom(
          padding: const EdgeInsets.all(12),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(8),
          ),
        ),
        child: Text(
          text,
          style: const TextStyle(fontSize: 18, fontWeight: FontWeight.w600),
        ),
      ),
    );
  }

  void _addToAmount(String digit) {
    setState(() {
      if (_displayAmount == '0.00') {
        if (digit == '.') {
          _displayAmount = '0.';
        } else {
          _displayAmount = digit;
        }
      } else {
        _displayAmount += digit;
      }
      _cashReceivedController.text = _displayAmount;
    });
  }

  void _backspaceAmount() {
    setState(() {
      if (_displayAmount.length > 1) {
        _displayAmount = _displayAmount.substring(0, _displayAmount.length - 1);
      } else {
        _displayAmount = '0.00';
      }
      _cashReceivedController.text = _displayAmount;
    });
  }

  Widget _buildQuickAmountButtons() {
    final quickAmounts = [
      widget.total,
      widget.total + 5,
      widget.total + 10,
      widget.total + 20,
    ];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Quick Amounts',
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
            fontWeight: FontWeight.w600,
          ),
        ),
        const SizedBox(height: 8),
        Wrap(
          spacing: 8,
          children: quickAmounts.map((amount) {
            return OutlinedButton(
              onPressed: () {
                setState(() {
                  _displayAmount = amount.toStringAsFixed(2);
                  _cashReceivedController.text = _displayAmount;
                });
              },
              child: Text('\$${amount.toStringAsFixed(2)}'),
            );
          }).toList(),
        ),
      ],
    );
  }

  Widget _buildChangeCalculation() {
    final cashReceived = double.tryParse(_cashReceivedController.text) ?? 0;
    final change = cashReceived - widget.total;

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: change >= 0
            ? Colors.green.withOpacity(0.1)
            : Colors.red.withOpacity(0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: change >= 0 ? Colors.green : Colors.red,
          width: 1,
        ),
      ),
      child: Column(
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Cash Received:'),
              Text('\$${cashReceived.toStringAsFixed(2)}'),
            ],
          ),
          const SizedBox(height: 4),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Total Amount:'),
              Text('\$${widget.total.toStringAsFixed(2)}'),
            ],
          ),
          const Divider(),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                change >= 0 ? 'Change:' : 'Remaining:',
                style: const TextStyle(fontWeight: FontWeight.bold),
              ),
              Text(
                '\$${change.abs().toStringAsFixed(2)}',
                style: TextStyle(
                  fontWeight: FontWeight.bold,
                  color: change >= 0 ? Colors.green : Colors.red,
                  fontSize: 18,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildSplitPaymentTab() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const SizedBox(height: 20),
        Text(
          'Split Payment',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.w600,
          ),
        ),
        const SizedBox(height: 12),
        if (_payments.isNotEmpty) ...[
          Expanded(
            child: ListView.builder(
              itemCount: _payments.length,
              itemBuilder: (context, index) {
                return _buildPaymentItem(index);
              },
            ),
          ),
          const SizedBox(height: 16),
        ],
        _buildAddPaymentSection(),
      ],
    );
  }

  Widget _buildPaymentItem(int index) {
    final payment = _payments[index];
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: Icon(payment.type.icon),
        title: Text(payment.type.displayName),
        subtitle: Text(payment.reference ?? ''),
        trailing: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(
              '\$${payment.amount.toStringAsFixed(2)}',
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                fontSize: 16,
              ),
            ),
            IconButton(
              icon: const Icon(Icons.delete),
              onPressed: () {
                setState(() {
                  _payments.removeAt(index);
                });
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildAddPaymentSection() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceVariant,
        borderRadius: BorderRadius.circular(12),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Add Payment',
            style: Theme.of(context).textTheme.titleSmall?.copyWith(
              fontWeight: FontWeight.w600,
            ),
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              Expanded(
                child: DropdownButtonFormField<PaymentMethodType>(
                  value: _selectedMethod,
                  decoration: const InputDecoration(
                    labelText: 'Payment Method',
                    border: OutlineInputBorder(),
                  ),
                  items: PaymentMethodType.values.map((method) {
                    return DropdownMenuItem(
                      value: method,
                      child: Row(
                        children: [
                          Icon(method.icon, size: 20),
                          const SizedBox(width: 8),
                          Text(method.displayName),
                        ],
                      ),
                    );
                  }).toList(),
                  onChanged: (value) {
                    setState(() {
                      _selectedMethod = value!;
                    });
                  },
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: TextField(
                  controller: _amountController,
                  keyboardType: TextInputType.number,
                  inputFormatters: [
                    FilteringTextInputFormatter.allow(RegExp(r'^\d+\.?\d{0,2}')),
                  ],
                  decoration: InputDecoration(
                    labelText: 'Amount',
                    prefixText: '\$',
                    border: const OutlineInputBorder(),
                    suffixIcon: IconButton(
                      icon: const Icon(Icons.arrow_upward),
                      onPressed: () {
                        _amountController.text = _remainingAmount.toStringAsFixed(2);
                      },
                    ),
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          TextField(
            controller: _referenceController,
            decoration: InputDecoration(
              labelText: _getReferenceLabel(_selectedMethod),
              border: const OutlineInputBorder(),
            ),
          ),
          const SizedBox(height: 16),
          SizedBox(
            width: double.infinity,
            child: ElevatedButton.icon(
              onPressed: _addPayment,
              icon: const Icon(Icons.add),
              label: const Text('Add Payment'),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildLoyaltyPointsSection() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.orange.withOpacity(0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Colors.orange),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Row(
            children: [
              Icon(Icons.stars, color: Colors.orange),
              SizedBox(width: 8),
              Text(
                'Loyalty Points',
                style: TextStyle(fontWeight: FontWeight.bold),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Text('Available: ${widget.customerLoyaltyPoints ?? "0"} points'),
          const SizedBox(height: 8),
          const Text('1 point = \$0.01'),
        ],
      ),
    );
  }

  Widget _buildPaymentSummary() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceVariant,
        borderRadius: BorderRadius.circular(12),
      ),
      child: Column(
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Total Amount:'),
              Text('\$${widget.total.toStringAsFixed(2)}'),
            ],
          ),
          const SizedBox(height: 4),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Amount Paid:'),
              Text('\$${_totalPaid.toStringAsFixed(2)}'),
            ],
          ),
          const Divider(),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                _remainingAmount > 0 ? 'Remaining:' : 'Change:',
                style: const TextStyle(fontWeight: FontWeight.bold),
              ),
              Text(
                '\$${_remainingAmount.abs().toStringAsFixed(2)}',
                style: TextStyle(
                  fontWeight: FontWeight.bold,
                  color: _remainingAmount > 0 ? Colors.red : Colors.green,
                  fontSize: 16,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildActionButtons() {
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
          child: ElevatedButton.icon(
            onPressed: _canComplete ? _completePayment : null,
            icon: const Icon(Icons.check),
            label: const Text('Complete Payment'),
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.green,
              foregroundColor: Colors.white,
            ),
          ),
        ),
      ],
    );
  }

  String _getReferenceLabel(PaymentMethodType method) {
    switch (method) {
      case PaymentMethodType.cash:
        return 'Notes';
      case PaymentMethodType.card:
        return 'Card Reference';
      case PaymentMethodType.digital:
        return 'Transaction ID';
      case PaymentMethodType.bankTransfer:
        return 'Transfer Reference';
      case PaymentMethodType.cheque:
        return 'Cheque Number';
      case PaymentMethodType.giftCard:
        return 'Gift Card Number';
      case PaymentMethodType.loyaltyPoints:
        return 'Points Used';
    }
  }

  void _addPayment() {
    final amount = double.tryParse(_amountController.text) ?? 0;
    if (amount <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please enter a valid amount')),
      );
      return;
    }

    if (amount > _remainingAmount) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Amount cannot exceed remaining balance')),
      );
      return;
    }

    setState(() {
      _payments.add(PaymentMethod(
        type: _selectedMethod,
        amount: amount,
        reference: _referenceController.text.isEmpty ? null : _referenceController.text,
      ));
    });

    _amountController.clear();
    _referenceController.clear();
  }

  void _completePayment() {
    if (_selectedMethod == PaymentMethodType.cash && _payments.isEmpty) {
      // Quick cash payment
      final cashAmount = double.tryParse(_cashReceivedController.text) ?? 0;
      if (cashAmount >= widget.total) {
        final payments = [
          PaymentMethod(
            type: PaymentMethodType.cash,
            amount: widget.total,
            reference: 'Cash payment',
          ),
        ];
        widget.onPaymentComplete(payments, cashAmount - widget.total);
        Navigator.of(context).pop();
        return;
      }
    }

    if (_canComplete) {
      widget.onPaymentComplete(_payments, _changeAmount > 0 ? _changeAmount : 0);
      Navigator.of(context).pop();
    }
  }
}
