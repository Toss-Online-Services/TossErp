import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../domain/entities/receipt_entity.dart';
import '../../data/services/receipt_service.dart';
import '../widgets/common/custom_app_bar.dart';

class ReceiptPreviewScreen extends StatefulWidget {
  final ReceiptEntity receipt;

  const ReceiptPreviewScreen({
    super.key,
    required this.receipt,
  });

  @override
  State<ReceiptPreviewScreen> createState() => _ReceiptPreviewScreenState();
}

class _ReceiptPreviewScreenState extends State<ReceiptPreviewScreen> {
  final ReceiptService _receiptService = ReceiptService();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(
        title: 'Receipt Preview',
        actions: [
          IconButton(
            icon: const Icon(Icons.print),
            onPressed: _reprintReceipt,
            tooltip: 'Reprint',
          ),
          IconButton(
            icon: const Icon(Icons.share),
            onPressed: _shareReceipt,
            tooltip: 'Share',
          ),
          PopupMenuButton(
            itemBuilder: (context) => [
              const PopupMenuItem(
                value: 'email',
                child: ListTile(
                  leading: Icon(Icons.email),
                  title: Text('Send by Email'),
                ),
              ),
              const PopupMenuItem(
                value: 'sms',
                child: ListTile(
                  leading: Icon(Icons.sms),
                  title: Text('Send by SMS'),
                ),
              ),
              const PopupMenuItem(
                value: 'whatsapp',
                child: ListTile(
                  leading: Icon(Icons.message),
                  title: Text('Send by WhatsApp'),
                ),
              ),
              const PopupMenuItem(
                value: 'copy',
                child: ListTile(
                  leading: Icon(Icons.copy),
                  title: Text('Copy Receipt Number'),
                ),
              ),
            ],
            onSelected: _handleMenuAction,
          ),
        ],
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          children: [
            // Receipt Preview Card
            Card(
              elevation: 4,
              child: Container(
                width: double.infinity,
                padding: const EdgeInsets.all(24.0),
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(8),
                  color: Colors.white,
                ),
                child: _buildReceiptContent(),
              ),
            ),
            
            const SizedBox(height: 24),
            
            // Receipt Information
            _buildReceiptInfo(),
            
            const SizedBox(height: 24),
            
            // Action Buttons
            _buildActionButtons(),
          ],
        ),
      ),
    );
  }

  Widget _buildReceiptContent() {
    final receipt = widget.receipt;
    
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        // Header
        Center(
          child: Column(
            children: [
              Text(
                receipt.settings.businessName.toUpperCase(),
                style: const TextStyle(
                  fontSize: 20,
                  fontWeight: FontWeight.bold,
                ),
              ),
              if (receipt.settings.businessAddress != null) ...[
                const SizedBox(height: 4),
                Text(
                  receipt.settings.businessAddress!,
                  style: const TextStyle(fontSize: 14),
                  textAlign: TextAlign.center,
                ),
              ],
              if (receipt.settings.businessPhone != null) ...[
                const SizedBox(height: 4),
                Text(
                  'Tel: ${receipt.settings.businessPhone}',
                  style: const TextStyle(fontSize: 14),
                ),
              ],
              if (receipt.settings.taxNumber != null) ...[
                const SizedBox(height: 4),
                Text(
                  'Tax ID: ${receipt.settings.taxNumber}',
                  style: const TextStyle(fontSize: 12, color: Colors.grey),
                ),
              ],
            ],
          ),
        ),
        
        const SizedBox(height: 20),
        const Divider(),
        const SizedBox(height: 16),
        
        // Transaction Details
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Receipt #: ${receipt.receiptNumber}'),
                Text('Date: ${_formatDate(receipt.createdAt)}'),
                Text('Time: ${_formatTime(receipt.createdAt)}'),
                if (receipt.isReprint)
                  const Text(
                    'REPRINT',
                    style: TextStyle(
                      color: Colors.orange,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
              ],
            ),
            Column(
              crossAxisAlignment: CrossAxisAlignment.end,
              children: [
                Text('Transaction: ${receipt.transactionId}'),
                Text('Cashier: ${receipt.cashierId}'),
                if (receipt.customer?.name != null)
                  Text('Customer: ${receipt.customer!.name}'),
                Text(
                  receipt.type.name.toUpperCase(),
                  style: TextStyle(
                    color: _getReceiptTypeColor(receipt.type),
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ],
            ),
          ],
        ),
        
        const SizedBox(height: 20),
        const Divider(),
        const SizedBox(height: 16),
        
        // Items
        ...receipt.lineItems.map((item) => _buildItemRow(item)),
        
        const SizedBox(height: 16),
        const Divider(),
        const SizedBox(height: 16),
        
        // Totals
        _buildTotalsSection(),
        
        const SizedBox(height: 20),
        
        // Payment Information
        if (receipt.payment != null) ...[
          const Divider(),
          const SizedBox(height: 16),
          _buildPaymentSection(),
          const SizedBox(height: 20),
        ],
        
        // Footer
        if (receipt.settings.footerMessage != null) ...[
          const Divider(),
          const SizedBox(height: 16),
          Center(
            child: Text(
              receipt.settings.footerMessage!,
              style: const TextStyle(fontSize: 12),
              textAlign: TextAlign.center,
            ),
          ),
          const SizedBox(height: 8),
        ],
        
        Center(
          child: Text(
            'Thank you for your business!',
            style: const TextStyle(
              fontWeight: FontWeight.bold,
              fontSize: 16,
            ),
          ),
        ),
        
        if (receipt.settings.showBarcode) ...[
          const SizedBox(height: 16),
          Center(
            child: Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                border: Border.all(color: Colors.grey),
                borderRadius: BorderRadius.circular(4),
              ),
              child: Text(
                receipt.receiptNumber,
                style: const TextStyle(
                  fontFamily: 'monospace',
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ),
        ],
      ],
    );
  }

  Widget _buildItemRow(ReceiptLineItem item) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      item.productName,
                      style: const TextStyle(fontWeight: FontWeight.w500),
                    ),
                    if (item.sku != null)
                      Text(
                        'SKU: ${item.sku}',
                        style: const TextStyle(
                          fontSize: 12,
                          color: Colors.grey,
                        ),
                      ),
                  ],
                ),
              ),
              Text('GHS ${item.totalPrice.toStringAsFixed(2)}'),
            ],
          ),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                '${item.quantity} x GHS ${item.unitPrice.toStringAsFixed(2)}',
                style: const TextStyle(
                  fontSize: 12,
                  color: Colors.grey,
                ),
              ),
              if (item.discount != null && item.discount! > 0)
                Text(
                  'Discount: -GHS ${item.discount!.toStringAsFixed(2)}',
                  style: const TextStyle(
                    fontSize: 12,
                    color: Colors.red,
                  ),
                ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildTotalsSection() {
    final totals = widget.receipt.totals;
    
    return Column(
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text('Subtotal:'),
            Text('GHS ${totals.subtotal.toStringAsFixed(2)}'),
          ],
        ),
        if (totals.totalDiscount > 0) ...[
          const SizedBox(height: 4),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Discount:'),
              Text(
                '-GHS ${totals.totalDiscount.toStringAsFixed(2)}',
                style: const TextStyle(color: Colors.red),
              ),
            ],
          ),
        ],
        if (totals.totalTax > 0) ...[
          const SizedBox(height: 4),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Tax:'),
              Text('GHS ${totals.totalTax.toStringAsFixed(2)}'),
            ],
          ),
        ],
        const SizedBox(height: 8),
        Container(
          padding: const EdgeInsets.all(8),
          decoration: BoxDecoration(
            color: Colors.grey[100],
            borderRadius: BorderRadius.circular(4),
          ),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text(
                'TOTAL:',
                style: TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                ),
              ),
              Text(
                'GHS ${totals.total.toStringAsFixed(2)}',
                style: const TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildPaymentSection() {
    final payment = widget.receipt.payment!;
    final totals = widget.receipt.totals;
    
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'Payment Information',
          style: TextStyle(
            fontWeight: FontWeight.bold,
            fontSize: 16,
          ),
        ),
        const SizedBox(height: 8),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text('Payment Method:'),
            Text(payment.method),
          ],
        ),
        const SizedBox(height: 4),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text('Amount Paid:'),
            Text('GHS ${totals.amountPaid.toStringAsFixed(2)}'),
          ],
        ),
        if (totals.change > 0) ...[
          const SizedBox(height: 4),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Change:'),
              Text(
                'GHS ${totals.change.toStringAsFixed(2)}',
                style: const TextStyle(color: Colors.green),
              ),
            ],
          ),
        ],
        if (payment.reference != null) ...[
          const SizedBox(height: 4),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Reference:'),
              Text(payment.reference!),
            ],
          ),
        ],
      ],
    );
  }

  Widget _buildReceiptInfo() {
    final receipt = widget.receipt;
    
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Receipt Information',
              style: Theme.of(context).textTheme.titleMedium,
            ),
            const SizedBox(height: 12),
            _buildInfoRow('Receipt ID', receipt.id),
            _buildInfoRow('Format', _getFormatDisplayName(receipt.format)),
            _buildInfoRow('Location', receipt.locationId),
            if (receipt.originalReceiptId != null)
              _buildInfoRow('Original Receipt', receipt.originalReceiptId!),
            _buildInfoRow('Created', '${_formatDate(receipt.createdAt)} ${_formatTime(receipt.createdAt)}'),
          ],
        ),
      ),
    );
  }

  Widget _buildInfoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 2.0),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(
            label,
            style: const TextStyle(color: Colors.grey),
          ),
          Flexible(
            child: Text(
              value,
              style: const TextStyle(fontWeight: FontWeight.w500),
              textAlign: TextAlign.end,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildActionButtons() {
    return Column(
      children: [
        Row(
          children: [
            Expanded(
              child: ElevatedButton.icon(
                onPressed: _reprintReceipt,
                icon: const Icon(Icons.print),
                label: const Text('Reprint'),
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: ElevatedButton.icon(
                onPressed: _shareReceipt,
                icon: const Icon(Icons.share),
                label: const Text('Share'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.blue,
                  foregroundColor: Colors.white,
                ),
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
        Row(
          children: [
            Expanded(
              child: OutlinedButton.icon(
                onPressed: () => _sendReceipt(DeliveryMethod.email),
                icon: const Icon(Icons.email),
                label: const Text('Email'),
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: OutlinedButton.icon(
                onPressed: () => _sendReceipt(DeliveryMethod.sms),
                icon: const Icon(Icons.sms),
                label: const Text('SMS'),
              ),
            ),
          ],
        ),
      ],
    );
  }

  void _handleMenuAction(String action) {
    switch (action) {
      case 'email':
        _sendReceipt(DeliveryMethod.email);
        break;
      case 'sms':
        _sendReceipt(DeliveryMethod.sms);
        break;
      case 'whatsapp':
        _sendReceipt(DeliveryMethod.whatsapp);
        break;
      case 'copy':
        _copyReceiptNumber();
        break;
    }
  }

  Future<void> _reprintReceipt() async {
    try {
      await _receiptService.reprintReceipt(widget.receipt.id);
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Receipt sent to printer'),
            backgroundColor: Colors.green,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Failed to print receipt: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  Future<void> _shareReceipt() async {
    try {
      // In a real implementation, you would generate a shareable format
      await Future.delayed(const Duration(milliseconds: 500));
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Receipt shared successfully'),
            backgroundColor: Colors.green,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Failed to share receipt: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  Future<void> _sendReceipt(DeliveryMethod method) async {
    String? contact;
    String title, hint;
    
    switch (method) {
      case DeliveryMethod.email:
        title = 'Email Address';
        hint = 'Enter email address';
        break;
      case DeliveryMethod.sms:
      case DeliveryMethod.whatsapp:
        title = 'Phone Number';
        hint = 'Enter phone number';
        break;
      default:
        return;
    }
    
    contact = await _showContactDialog(title, hint);
    
    if (contact != null && contact.isNotEmpty) {
      try {
        // Mock implementation
        await Future.delayed(const Duration(seconds: 1));
        
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text('Receipt sent via ${method.name}'),
              backgroundColor: Colors.green,
            ),
          );
        }
      } catch (e) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text('Failed to send receipt: $e'),
              backgroundColor: Colors.red,
            ),
          );
        }
      }
    }
  }

  Future<String?> _showContactDialog(String title, String hint) async {
    final controller = TextEditingController();
    
    return showDialog<String>(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(title),
        content: TextField(
          controller: controller,
          decoration: InputDecoration(hintText: hint),
          keyboardType: title.contains('Email') 
              ? TextInputType.emailAddress 
              : TextInputType.phone,
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, controller.text.trim()),
            child: const Text('Send'),
          ),
        ],
      ),
    );
  }

  void _copyReceiptNumber() {
    Clipboard.setData(ClipboardData(text: widget.receipt.receiptNumber));
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('Receipt number copied to clipboard'),
        duration: Duration(seconds: 2),
      ),
    );
  }

  String _formatDate(DateTime dateTime) {
    return '${dateTime.day.toString().padLeft(2, '0')}/${dateTime.month.toString().padLeft(2, '0')}/${dateTime.year}';
  }

  String _formatTime(DateTime dateTime) {
    return '${dateTime.hour.toString().padLeft(2, '0')}:${dateTime.minute.toString().padLeft(2, '0')}';
  }

  String _getFormatDisplayName(ReceiptFormat format) {
    switch (format) {
      case ReceiptFormat.thermal:
        return 'Thermal';
      case ReceiptFormat.pos58:
        return 'POS 58mm';
      case ReceiptFormat.pos80:
        return 'POS 80mm';
      case ReceiptFormat.a4:
        return 'A4 PDF';
    }
  }

  Color _getReceiptTypeColor(ReceiptType type) {
    switch (type) {
      case ReceiptType.sale:
        return Colors.green;
      case ReceiptType.refund:
        return Colors.red;
      case ReceiptType.layaway:
        return Colors.blue;
      case ReceiptType.quote:
        return Colors.orange;
    }
  }
}
