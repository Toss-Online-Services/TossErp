import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:share_plus/share_plus.dart';

import '../../../../app/themes/app_sizes.dart';
import '../../../../app/utilities/currency_formatter.dart';
import '../../../../domain/entities/ordered_product_entity.dart';

class ReceiptPreviewScreen extends StatelessWidget {
  final String transactionId;
  final List<OrderedProductEntity> items;
  final double totalAmount;
  final String paymentMethod;
  final double amountReceived;
  final double change;
  final String? customerName;
  final String? customerPhone;

  const ReceiptPreviewScreen({
    super.key,
    required this.transactionId,
    required this.items,
    required this.totalAmount,
    required this.paymentMethod,
    required this.amountReceived,
    required this.change,
    this.customerName,
    this.customerPhone,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Theme.of(context).colorScheme.surfaceContainerLowest,
      appBar: AppBar(
        title: const Text('Receipt'),
        backgroundColor: Colors.transparent,
        elevation: 0,
        actions: [
          IconButton(
            onPressed: () => _shareReceipt(),
            icon: const Icon(Icons.share),
            tooltip: 'Share Receipt',
          ),
          IconButton(
            onPressed: () => _printReceipt(context),
            icon: const Icon(Icons.print),
            tooltip: 'Print Receipt',
          ),
        ],
      ),
      body: Column(
        children: [
          // Success Animation
          Container(
            padding: const EdgeInsets.all(AppSizes.padding),
            child: Column(
              children: [
                Container(
                  width: 80,
                  height: 80,
                  decoration: BoxDecoration(
                    color: Colors.green,
                    borderRadius: BorderRadius.circular(40),
                  ),
                  child: const Icon(
                    Icons.check,
                    color: Colors.white,
                    size: 40,
                  ),
                ),
                const SizedBox(height: 16),
                Text(
                  'Payment Successful!',
                  style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                    fontWeight: FontWeight.bold,
                    color: Colors.green,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  'Transaction ID: $transactionId',
                  style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    color: Theme.of(context).colorScheme.outline,
                  ),
                ),
              ],
            ),
          ),
          
          // Receipt Content
          Expanded(
            child: Container(
              margin: const EdgeInsets.all(AppSizes.padding),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(AppSizes.radius),
                boxShadow: [
                  BoxShadow(
                    color: Colors.black.withOpacity(0.1),
                    blurRadius: 10,
                    offset: const Offset(0, 4),
                  ),
                ],
              ),
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(AppSizes.padding * 1.5),
                child: _buildReceiptContent(context),
              ),
            ),
          ),
          
          // Action Buttons
          Container(
            padding: const EdgeInsets.all(AppSizes.padding),
            child: Row(
              children: [
                Expanded(
                  child: OutlinedButton(
                    onPressed: () => Navigator.of(context).popUntil((route) => route.isFirst),
                    style: OutlinedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(vertical: 16),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(AppSizes.radius),
                      ),
                    ),
                    child: const Text('New Sale'),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: ElevatedButton(
                    onPressed: () => _printReceipt(context),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Theme.of(context).colorScheme.primary,
                      foregroundColor: Theme.of(context).colorScheme.onPrimary,
                      padding: const EdgeInsets.symmetric(vertical: 16),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(AppSizes.radius),
                      ),
                    ),
                    child: const Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Icon(Icons.print, size: 20),
                        SizedBox(width: 8),
                        Text('Print'),
                      ],
                    ),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildReceiptContent(BuildContext context) {
    final now = DateTime.now();
    
    return Column(
      crossAxisAlignment: CrossAxisAlignment.center,
      children: [
        // Store Header
        Text(
          'TOSS ERP Store',
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          'Point of Sale System',
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
            color: Theme.of(context).colorScheme.outline,
          ),
        ),
        const SizedBox(height: 8),
        Text(
          'Tel: +27 123 456 7890',
          style: Theme.of(context).textTheme.bodySmall,
        ),
        const SizedBox(height: 24),
        
        // Receipt Details
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              'Receipt #:',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                fontWeight: FontWeight.w500,
              ),
            ),
            Text(
              transactionId.substring(transactionId.length - 8),
              style: Theme.of(context).textTheme.bodyMedium,
            ),
          ],
        ),
        const SizedBox(height: 8),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              'Date:',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                fontWeight: FontWeight.w500,
              ),
            ),
            Text(
              '${now.day}/${now.month}/${now.year} ${now.hour}:${now.minute.toString().padLeft(2, '0')}',
              style: Theme.of(context).textTheme.bodyMedium,
            ),
          ],
        ),
        
        if (customerName != null) ...[
          const SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Customer:',
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  fontWeight: FontWeight.w500,
                ),
              ),
              Text(
                customerName!,
                style: Theme.of(context).textTheme.bodyMedium,
              ),
            ],
          ),
        ],
        
        if (customerPhone != null) ...[
          const SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Phone:',
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  fontWeight: FontWeight.w500,
                ),
              ),
              Text(
                customerPhone!,
                style: Theme.of(context).textTheme.bodyMedium,
              ),
            ],
          ),
        ],
        
        const SizedBox(height: 24),
        
        // Divider
        Container(
          height: 1,
          color: Theme.of(context).colorScheme.outline.withOpacity(0.3),
        ),
        
        const SizedBox(height: 16),
        
        // Items
        ...items.map((item) => Padding(
          padding: const EdgeInsets.only(bottom: 12),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                flex: 3,
                child: Text(
                  item.name,
                  style: Theme.of(context).textTheme.bodyMedium,
                ),
              ),
              Expanded(
                child: Text(
                  '${item.quantity}x',
                  style: Theme.of(context).textTheme.bodyMedium,
                  textAlign: TextAlign.center,
                ),
              ),
              Expanded(
                child: Text(
                  CurrencyFormatter.format(item.price),
                  style: Theme.of(context).textTheme.bodyMedium,
                  textAlign: TextAlign.right,
                ),
              ),
              Expanded(
                child: Text(
                  CurrencyFormatter.format(item.price * item.quantity),
                  style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    fontWeight: FontWeight.w500,
                  ),
                  textAlign: TextAlign.right,
                ),
              ),
            ],
          ),
        )),
        
        const SizedBox(height: 16),
        
        // Divider
        Container(
          height: 1,
          color: Theme.of(context).colorScheme.outline.withOpacity(0.3),
        ),
        
        const SizedBox(height: 16),
        
        // Totals
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              'Subtotal:',
              style: Theme.of(context).textTheme.bodyLarge,
            ),
            Text(
              CurrencyFormatter.format(totalAmount),
              style: Theme.of(context).textTheme.bodyLarge,
            ),
          ],
        ),
        const SizedBox(height: 8),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              'Tax:',
              style: Theme.of(context).textTheme.bodyLarge,
            ),
            Text(
              'R 0.00',
              style: Theme.of(context).textTheme.bodyLarge,
            ),
          ],
        ),
        const SizedBox(height: 12),
        Container(
          height: 2,
          color: Theme.of(context).colorScheme.outline.withOpacity(0.3),
        ),
        const SizedBox(height: 12),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              'TOTAL:',
              style: Theme.of(context).textTheme.titleLarge?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            Text(
              CurrencyFormatter.format(totalAmount),
              style: Theme.of(context).textTheme.titleLarge?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
          ],
        ),
        
        const SizedBox(height: 24),
        
        // Payment Details
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              'Payment Method:',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                fontWeight: FontWeight.w500,
              ),
            ),
            Text(
              _formatPaymentMethod(paymentMethod),
              style: Theme.of(context).textTheme.bodyMedium,
            ),
          ],
        ),
        
        if (paymentMethod == 'cash') ...[
          const SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Amount Received:',
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  fontWeight: FontWeight.w500,
                ),
              ),
              Text(
                CurrencyFormatter.format(amountReceived),
                style: Theme.of(context).textTheme.bodyMedium,
              ),
            ],
          ),
          const SizedBox(height: 8),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Change:',
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  fontWeight: FontWeight.w500,
                ),
              ),
              Text(
                CurrencyFormatter.format(change),
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  color: Colors.green,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ],
          ),
        ],
        
        const SizedBox(height: 32),
        
        // Footer
        Text(
          'Thank you for your business!',
          style: Theme.of(context).textTheme.bodyLarge?.copyWith(
            fontWeight: FontWeight.w600,
          ),
          textAlign: TextAlign.center,
        ),
        const SizedBox(height: 8),
        Text(
          'Visit us again soon',
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
            color: Theme.of(context).colorScheme.outline,
          ),
          textAlign: TextAlign.center,
        ),
        
        const SizedBox(height: 24),
        
        // Barcode simulation
        Container(
          height: 50,
          width: 200,
          decoration: BoxDecoration(
            color: Colors.black,
            borderRadius: BorderRadius.circular(4),
          ),
          child: const Center(
            child: Text(
              '|||||| |||| ||| |||||| |||',
              style: TextStyle(
                color: Colors.white,
                fontFamily: 'monospace',
                letterSpacing: 2,
              ),
            ),
          ),
        ),
      ],
    );
  }

  String _formatPaymentMethod(String method) {
    switch (method) {
      case 'cash':
        return 'Cash';
      case 'card':
        return 'Credit/Debit Card';
      case 'mobile':
        return 'Mobile Money';
      case 'bank':
        return 'Bank Transfer';
      default:
        return method.toUpperCase();
    }
  }

  void _shareReceipt() {
    final receiptText = _generateReceiptText();
    Share.share(receiptText, subject: 'Receipt #${transactionId.substring(transactionId.length - 8)}');
  }

  String _generateReceiptText() {
    final now = DateTime.now();
    final buffer = StringBuffer();
    
    buffer.writeln('=================================');
    buffer.writeln('        TOSS ERP Store');
    buffer.writeln('      Point of Sale System');
    buffer.writeln('     Tel: +27 123 456 7890');
    buffer.writeln('=================================');
    buffer.writeln();
    buffer.writeln('Receipt #: ${transactionId.substring(transactionId.length - 8)}');
    buffer.writeln('Date: ${now.day}/${now.month}/${now.year} ${now.hour}:${now.minute.toString().padLeft(2, '0')}');
    
    if (customerName != null) {
      buffer.writeln('Customer: $customerName');
    }
    if (customerPhone != null) {
      buffer.writeln('Phone: $customerPhone');
    }
    
    buffer.writeln();
    buffer.writeln('---------------------------------');
    
    for (final item in items) {
      buffer.writeln('${item.name}');
      buffer.writeln('  ${item.quantity}x ${CurrencyFormatter.format(item.price)} = ${CurrencyFormatter.format(item.price * item.quantity)}');
    }
    
    buffer.writeln('---------------------------------');
    buffer.writeln('Subtotal: ${CurrencyFormatter.format(totalAmount)}');
    buffer.writeln('Tax: R 0.00');
    buffer.writeln('TOTAL: ${CurrencyFormatter.format(totalAmount)}');
    buffer.writeln();
    buffer.writeln('Payment: ${_formatPaymentMethod(paymentMethod)}');
    
    if (paymentMethod == 'cash') {
      buffer.writeln('Received: ${CurrencyFormatter.format(amountReceived)}');
      buffer.writeln('Change: ${CurrencyFormatter.format(change)}');
    }
    
    buffer.writeln();
    buffer.writeln('Thank you for your business!');
    buffer.writeln('Visit us again soon');
    buffer.writeln('=================================');
    
    return buffer.toString();
  }

  void _printReceipt(BuildContext context) {
    // TODO: Implement actual printing functionality
    // For now, show a placeholder message
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('Print functionality will be implemented with thermal printer support'),
        duration: Duration(seconds: 3),
      ),
    );
    
    // Copy receipt to clipboard as alternative
    Clipboard.setData(ClipboardData(text: _generateReceiptText()));
  }
}
