import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../app/themes/app_sizes.dart';
import '../../../../app/utilities/currency_formatter.dart';
import '../../../../domain/entities/ordered_product_entity.dart';
import '../../../../domain/entities/receipt_entity.dart';
import '../../../../domain/entities/sales_transaction_entity.dart';
import '../../../../domain/entities/payment_entity.dart';
import '../../../providers/home/home_provider.dart';
import '../../receipt_preview_screen.dart';

class PaymentModalSheet extends StatefulWidget {
  final HomeProvider cartProvider;
  final double totalAmount;
  final List<OrderedProductEntity> items;

  const PaymentModalSheet({
    super.key,
    required this.cartProvider,
    required this.totalAmount,
    required this.items,
  });

  @override
  State<PaymentModalSheet> createState() => _PaymentModalSheetState();
}

class _PaymentModalSheetState extends State<PaymentModalSheet> {
  String selectedPaymentMethod = 'cash';
  final TextEditingController amountController = TextEditingController();
  final TextEditingController customerNameController = TextEditingController();
  final TextEditingController customerPhoneController = TextEditingController();
  bool isProcessing = false;
  
  double get receivedAmount => double.tryParse(amountController.text) ?? 0.0;
  double get changeAmount => receivedAmount - widget.totalAmount;

  @override
  void initState() {
    super.initState();
    // Default to exact amount for non-cash payments
    if (selectedPaymentMethod != 'cash') {
      amountController.text = widget.totalAmount.toStringAsFixed(2);
    }
  }

  @override
  void dispose() {
    amountController.dispose();
    customerNameController.dispose();
    customerPhoneController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      height: MediaQuery.of(context).size.height * 0.85,
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        borderRadius: const BorderRadius.only(
          topLeft: Radius.circular(AppSizes.radius * 2),
          topRight: Radius.circular(AppSizes.radius * 2),
        ),
      ),
      child: Column(
        children: [
          // Header
          Container(
            padding: const EdgeInsets.fromLTRB(AppSizes.padding, 12, AppSizes.padding, 8),
            decoration: BoxDecoration(
              border: Border(
                bottom: BorderSide(
                  color: Theme.of(context).colorScheme.outline.withOpacity(0.2),
                ),
              ),
            ),
            child: Column(
              children: [
                // Drag Handle
                Container(
                  width: 40,
                  height: 4,
                  decoration: BoxDecoration(
                    color: Theme.of(context).colorScheme.outline.withOpacity(0.6),
                    borderRadius: BorderRadius.circular(2),
                  ),
                ),
                
                const SizedBox(height: 12),
                
                // Header with close button
                Row(
                  children: [
                    Icon(
                      Icons.payment,
                      color: Theme.of(context).colorScheme.primary,
                      size: 24,
                    ),
                    const SizedBox(width: 8),
                    Text(
                      'Payment',
                      style: Theme.of(context).textTheme.titleLarge?.copyWith(
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                    const Spacer(),
                    IconButton(
                      onPressed: () => Navigator.of(context).pop(),
                      icon: const Icon(Icons.close),
                    ),
                  ],
                ),
              ],
            ),
          ),
          
          // Content
          Expanded(
            child: SingleChildScrollView(
              padding: const EdgeInsets.all(AppSizes.padding),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Order Summary
                  _buildOrderSummary(),
                  
                  const SizedBox(height: 24),
                  
                  // Customer Information (Optional)
                  _buildCustomerInfo(),
                  
                  const SizedBox(height: 24),
                  
                  // Payment Methods
                  _buildPaymentMethods(),
                  
                  const SizedBox(height: 24),
                  
                  // Amount Input (for cash payments)
                  if (selectedPaymentMethod == 'cash') _buildAmountInput(),
                  
                  const SizedBox(height: 24),
                  
                  // Change calculation (for cash payments)
                  if (selectedPaymentMethod == 'cash') _buildChangeCalculation(),
                ],
              ),
            ),
          ),
          
          // Payment Button
          Container(
            padding: const EdgeInsets.all(AppSizes.padding),
            decoration: BoxDecoration(
              border: Border(
                top: BorderSide(
                  color: Theme.of(context).colorScheme.outline.withOpacity(0.2),
                ),
              ),
            ),
            child: _buildPaymentButton(),
          ),
        ],
      ),
    );
  }

  Widget _buildOrderSummary() {
    return Container(
      padding: const EdgeInsets.all(AppSizes.padding),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceContainer,
        borderRadius: BorderRadius.circular(AppSizes.radius),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Order Summary',
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.w600,
            ),
          ),
          const SizedBox(height: 12),
          ...widget.items.map((item) => Padding(
            padding: const EdgeInsets.only(bottom: 8),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Expanded(
                  child: Text(
                    '${item.name} x${item.quantity}',
                    style: Theme.of(context).textTheme.bodyMedium,
                  ),
                ),
                Text(
                  CurrencyFormatter.format(item.price * item.quantity),
                  style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ],
            ),
          )),
          const Divider(),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Total',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              Text(
                CurrencyFormatter.format(widget.totalAmount),
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

  Widget _buildCustomerInfo() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            Text(
              'Customer Information (Optional)',
              style: Theme.of(context).textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.w600,
              ),
            ),
            const Spacer(),
            TextButton.icon(
              onPressed: _showCustomerSearch,
              icon: const Icon(Icons.search, size: 18),
              label: const Text('Search'),
              style: TextButton.styleFrom(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                minimumSize: Size.zero,
                tapTargetSize: MaterialTapTargetSize.shrinkWrap,
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
        TextField(
          controller: customerNameController,
          decoration: InputDecoration(
            labelText: 'Customer Name',
            hintText: 'Enter customer name',
            prefixIcon: const Icon(Icons.person_outline),
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(AppSizes.radius),
            ),
          ),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: customerPhoneController,
          keyboardType: TextInputType.phone,
          decoration: InputDecoration(
            labelText: 'Phone Number',
            hintText: 'Enter phone number',
            prefixIcon: const Icon(Icons.phone_outlined),
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(AppSizes.radius),
            ),
          ),
        ),
      ],
    );
  }
  
  void _showCustomerSearch() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Search Customer'),
        content: const Text('Customer search functionality will be implemented here.\n\nThis will allow cashiers to:\n• Search by name or phone\n• View customer details\n• Select existing customer\n• Create new customer'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Close'),
          ),
          ElevatedButton(
            onPressed: () {
              Navigator.of(context).pop();
              // TODO: Navigate to customer management screen
            },
            child: const Text('Manage Customers'),
          ),
        ],
      ),
    );
  }

  Widget _buildPaymentMethods() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Payment Method',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.w600,
          ),
        ),
        const SizedBox(height: 12),
        Wrap(
          spacing: 12,
          runSpacing: 12,
          children: [
            _buildPaymentMethodCard(
              'cash',
              'Cash',
              Icons.money,
              Colors.green,
            ),
            _buildPaymentMethodCard(
              'card',
              'Card',
              Icons.credit_card,
              Colors.blue,
            ),
            _buildPaymentMethodCard(
              'mobile',
              'Mobile Money',
              Icons.smartphone,
              Colors.orange,
            ),
            _buildPaymentMethodCard(
              'bank',
              'Bank Transfer',
              Icons.account_balance,
              Colors.purple,
            ),
          ],
        ),
        const SizedBox(height: 16),
        // Loyalty Points Section
        Container(
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            color: Colors.amber[50],
            borderRadius: BorderRadius.circular(AppSizes.radius),
            border: Border.all(color: Colors.amber[200]!),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Icon(Icons.stars, color: Colors.amber[700], size: 20),
                  const SizedBox(width: 8),
                  Text(
                    'Loyalty Points',
                    style: Theme.of(context).textTheme.titleSmall?.copyWith(
                      fontWeight: FontWeight.w600,
                      color: Colors.amber[700],
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              Text(
                'Customer loyalty points integration will be implemented here.\n\nThis will allow:\n• View available points\n• Apply points to discount\n• Earn points from purchase',
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                  color: Colors.amber[700],
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildPaymentMethodCard(String method, String label, IconData icon, Color color) {
    final isSelected = selectedPaymentMethod == method;
    
    return GestureDetector(
      onTap: () {
        setState(() {
          selectedPaymentMethod = method;
          if (method != 'cash') {
            amountController.text = widget.totalAmount.toStringAsFixed(2);
          } else {
            amountController.clear();
          }
        });
      },
      child: Container(
        width: (MediaQuery.of(context).size.width - 48) / 2 - 6,
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: isSelected 
              ? color.withOpacity(0.1) 
              : Theme.of(context).colorScheme.surface,
          border: Border.all(
            color: isSelected 
                ? color 
                : Theme.of(context).colorScheme.outline.withOpacity(0.3),
            width: isSelected ? 2 : 1,
          ),
          borderRadius: BorderRadius.circular(AppSizes.radius),
        ),
        child: Column(
          children: [
            Icon(
              icon,
              color: isSelected ? color : Theme.of(context).colorScheme.outline,
              size: 32,
            ),
            const SizedBox(height: 8),
            Text(
              label,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: isSelected ? color : Theme.of(context).colorScheme.onSurface,
                fontWeight: isSelected ? FontWeight.w600 : FontWeight.normal,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildAmountInput() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Amount Received',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.w600,
          ),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: amountController,
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          inputFormatters: [
            FilteringTextInputFormatter.allow(RegExp(r'^\d+\.?\d{0,2}')),
          ],
          decoration: InputDecoration(
            labelText: 'Amount',
            hintText: 'Enter amount received',
            prefixIcon: const Icon(Icons.attach_money),
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(AppSizes.radius),
            ),
          ),
          onChanged: (value) => setState(() {}),
        ),
        const SizedBox(height: 12),
        // Quick amount buttons
        Wrap(
          spacing: 8,
          children: [
            _buildQuickAmountButton(widget.totalAmount),
            _buildQuickAmountButton(widget.totalAmount + 100),
            _buildQuickAmountButton(widget.totalAmount + 500),
            _buildQuickAmountButton(widget.totalAmount + 1000),
          ],
        ),
      ],
    );
  }

  Widget _buildQuickAmountButton(double amount) {
    return OutlinedButton(
      onPressed: () {
        amountController.text = amount.toStringAsFixed(2);
        setState(() {});
      },
      child: Text(CurrencyFormatter.format(amount)),
    );
  }

  Widget _buildChangeCalculation() {
    return Container(
      padding: const EdgeInsets.all(AppSizes.padding),
      decoration: BoxDecoration(
        color: changeAmount >= 0 
            ? Colors.green.withOpacity(0.1)
            : Colors.red.withOpacity(0.1),
        borderRadius: BorderRadius.circular(AppSizes.radius),
        border: Border.all(
          color: changeAmount >= 0 ? Colors.green : Colors.red,
          width: 1,
        ),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(
            changeAmount >= 0 ? 'Change:' : 'Still needed:',
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.w600,
            ),
          ),
          Text(
            CurrencyFormatter.format(changeAmount.abs()),
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
              color: changeAmount >= 0 ? Colors.green : Colors.red,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildPaymentButton() {
    final canProcess = _canProcessPayment();
    
    return SizedBox(
      width: double.infinity,
      height: 56,
      child: ElevatedButton(
        onPressed: canProcess && !isProcessing ? _processPayment : null,
        style: ElevatedButton.styleFrom(
          backgroundColor: Theme.of(context).colorScheme.primary,
          foregroundColor: Theme.of(context).colorScheme.onPrimary,
          disabledBackgroundColor: Theme.of(context).colorScheme.outline.withOpacity(0.3),
          elevation: 2,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppSizes.radius),
          ),
        ),
        child: isProcessing
            ? const SizedBox(
                width: 24,
                height: 24,
                child: CircularProgressIndicator(
                  strokeWidth: 2,
                  valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                ),
              )
            : Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Icon(Icons.check_circle),
                  const SizedBox(width: 8),
                  Text(
                    'Complete Payment',
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      color: Theme.of(context).colorScheme.onPrimary,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ],
              ),
      ),
    );
  }

  bool _canProcessPayment() {
    if (selectedPaymentMethod == 'cash') {
      return receivedAmount >= widget.totalAmount;
    }
    return true; // Other payment methods don't require amount validation
  }

  Future<void> _processPayment() async {
    setState(() => isProcessing = true);
    
    try {
      // Simulate payment processing
      await Future.delayed(const Duration(seconds: 2));
      
      // Create transaction record
      await _createTransaction();
      
      // Clear cart
      widget.cartProvider.onClearCart();
      
      // Show success and receipt
      if (mounted) {
        Navigator.of(context).pop(); // Close payment modal
        _showReceiptPreview();
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Payment failed: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    } finally {
      if (mounted) {
        setState(() => isProcessing = false);
      }
    }
  }

  Future<void> _createTransaction() async {
    try {
      // Create transaction number
      final transactionNumber = 'TXN${DateTime.now().millisecondsSinceEpoch}';
      
      // Convert items to sales transaction items
      final transactionItems = widget.items.map((item) => SalesTransactionItemEntity(
        transactionId: 0, // Will be set by repository
        productId: item.productId,
        productName: item.name,
        quantity: item.quantity,
        unitPrice: (item.price * 100).round(), // Convert to cents
        totalPrice: ((item.price * item.quantity) * 100).round(), // Convert to cents
      )).toList();
      
      // Create payment entity
      final payment = PaymentEntity(
        transactionId: null, // Will be set by repository
        method: _parsePaymentMethod(selectedPaymentMethod),
        amount: ((selectedPaymentMethod == 'cash' ? receivedAmount : widget.totalAmount) * 100).round(), // Convert to cents
        status: PaymentStatus.completed,
        paymentDate: DateTime.now(),
        createdAt: DateTime.now(),
        reference: 'PAY${DateTime.now().millisecondsSinceEpoch}',
      );
      
      // Create sales transaction entity
      final salesTransaction = SalesTransactionEntity(
        transactionNumber: transactionNumber,
        type: SalesTransactionType.sale,
        status: SalesTransactionStatus.completed,
        customerId: _getCustomerIdFromInput(),
        subtotal: (widget.totalAmount * 100).round(), // Convert to cents
        total: (widget.totalAmount * 100).round(), // Convert to cents
        amountPaid: ((selectedPaymentMethod == 'cash' ? receivedAmount : widget.totalAmount) * 100).round(), // Convert to cents
        changeAmount: (selectedPaymentMethod == 'cash' ? (changeAmount * 100).round() : 0), // Convert to cents
        items: transactionItems,
        payments: [payment],
        notes: _getTransactionNotes(),
        createdAt: DateTime.now(),
        createdById: 'current_user', // TODO: Get from auth service
      );
      
      // TODO: Save transaction using SalesTransactionRepository
      // For now, just log the transaction
      print('Sales Transaction created: ${salesTransaction.transactionNumber}');
      print('Total: ${widget.totalAmount}');
      print('Payment Method: ${selectedPaymentMethod}');
      print('Customer: ${customerNameController.text.trim()}');
      
      // Update customer visit if customer info provided
      if (customerNameController.text.trim().isNotEmpty || customerPhoneController.text.trim().isNotEmpty) {
        await _updateCustomerVisit(salesTransaction);
      }
      
    } catch (e) {
      print('Error creating transaction: $e');
      rethrow;
    }
  }
  
  PaymentMethod _parsePaymentMethod(String method) {
    switch (method.toLowerCase()) {
      case 'cash': return PaymentMethod.cash;
      case 'card': return PaymentMethod.card;
      case 'mobile': return PaymentMethod.mobileMoney;
      case 'bank': return PaymentMethod.bankTransfer;
      default: return PaymentMethod.other;
    }
  }
  
  String? _getCustomerIdFromInput() {
    // TODO: Implement customer lookup/creation logic
    // For now, return null for anonymous transactions
    if (customerNameController.text.trim().isEmpty && customerPhoneController.text.trim().isEmpty) {
      return null;
    }
    // In a real implementation, you would:
    // 1. Search for existing customer by phone
    // 2. Create new customer if not found
    // 3. Return the customer ID
    return 'temp_customer_${DateTime.now().millisecondsSinceEpoch}';
  }
  
  String? _getTransactionNotes() {
    final notes = <String>[];
    if (customerNameController.text.trim().isNotEmpty) {
      notes.add('Customer: ${customerNameController.text.trim()}');
    }
    if (customerPhoneController.text.trim().isNotEmpty) {
      notes.add('Phone: ${customerPhoneController.text.trim()}');
    }
    return notes.isEmpty ? null : notes.join(', ');
  }
  
  Future<void> _updateCustomerVisit(SalesTransactionEntity transaction) async {
    try {
      // TODO: Implement customer visit update using CustomerRepository
      // This would update customer's visit count, total spent, and last visit date
      print('Updating customer visit for transaction: ${transaction.transactionNumber}');
    } catch (e) {
      print('Error updating customer visit: $e');
      // Don't rethrow - customer visit update failure shouldn't fail the transaction
    }
  }

  void _showReceiptPreview() {
    final String transactionId = DateTime.now().millisecondsSinceEpoch.toString();
    final DateTime now = DateTime.now();
    
    // Create receipt line items
    final List<ReceiptLineItem> lineItems = widget.items.map((item) => ReceiptLineItem(
      id: item.id?.toString() ?? item.productId.toString(),
      productName: item.name,
      productSku: item.productId.toString(),
      quantity: item.quantity,
      unitPrice: item.price.toDouble(),
      lineTotal: (item.price * item.quantity).toDouble(),
      totalPrice: (item.price * item.quantity).toDouble(),
    )).toList();
    
    // Create receipt totals
    final ReceiptTotals totals = ReceiptTotals(
      subtotal: widget.totalAmount,
      totalDiscount: 0.0,
      totalTax: 0.0,
      total: widget.totalAmount,
      amountPaid: selectedPaymentMethod == 'cash' ? receivedAmount : widget.totalAmount,
      change: selectedPaymentMethod == 'cash' ? changeAmount : 0.0,
    );
    
    // Create customer info if provided
    final ReceiptCustomer? customer = (customerNameController.text.trim().isNotEmpty || 
                                     customerPhoneController.text.trim().isNotEmpty) 
        ? ReceiptCustomer(
            id: 'customer_${now.millisecondsSinceEpoch}',
            name: customerNameController.text.trim().isEmpty ? null : customerNameController.text.trim(),
            phone: customerPhoneController.text.trim().isEmpty ? null : customerPhoneController.text.trim(),
          )
        : null;
    
    // Create payment info
    final ReceiptPayment payment = ReceiptPayment(
      method: selectedPaymentMethod,
      amount: selectedPaymentMethod == 'cash' ? receivedAmount : widget.totalAmount,
    );
    
    // Create receipt settings
    const ReceiptSettings settings = ReceiptSettings(
      businessName: 'Toss POS',
      businessAddress: 'Your Business Address',
      businessPhone: '+233 XXX XXX XXX',
      showBarcode: true,
      printCustomerInfo: true,
    );
    
    // Create the receipt entity
    final ReceiptEntity receipt = ReceiptEntity(
      id: now.millisecondsSinceEpoch,
      receiptNumber: 'R${now.millisecondsSinceEpoch}',
      type: ReceiptType.sale,
      transactionId: transactionId,
      cashierId: 'current_user', // Replace with actual cashier ID
      locationId: 'default_location', // Replace with actual location ID
      receiptDate: now,
      createdAt: now,
      receiptData: {
        'paymentMethod': selectedPaymentMethod,
        'timestamp': now.toIso8601String(),
      },
      lineItems: lineItems,
      totals: totals,
      customer: customer,
      payment: payment,
      settings: settings,
    );
    
    Navigator.of(context).push(
      MaterialPageRoute(
        builder: (context) => ReceiptPreviewScreen(receipt: receipt),
      ),
    );
  }
}
