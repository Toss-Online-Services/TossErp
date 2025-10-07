import 'dart:async';
import 'dart:typed_data';
import 'package:flutter/services.dart';
import 'package:pdf/pdf.dart';
import 'package:pdf/widgets.dart' as pw;
import 'package:printing/printing.dart';

import '../../domain/entities/receipt_entity.dart';
import '../../domain/entities/sales_transaction_entity.dart';
// Using an internal mock repository defined below for now

class ReceiptService {
  static final ReceiptService _instance = ReceiptService._internal();
  factory ReceiptService() => _instance;
  ReceiptService._internal();

  final ReceiptRepository _receiptRepository = ReceiptRepository();
  final StreamController<ReceiptEntity> _receiptController = 
      StreamController<ReceiptEntity>.broadcast();

  Stream<ReceiptEntity> get receiptStream => _receiptController.stream;

  // Generate and process receipt
  Future<ReceiptEntity> generateReceipt({
    required SalesTransactionEntity transaction,
    required ReceiptType type,
    required ReceiptSettings settings,
    ReceiptFormat format = ReceiptFormat.thermal,
    List<DeliveryMethod> deliveryMethods = const [DeliveryMethod.print],
    String? customerEmail,
    String? customerPhone,
  }) async {
    try {
      final receipt = await _createReceiptEntity(
        transaction: transaction,
        type: type,
        format: format,
        settings: settings,
      );

      // Save receipt to repository
      final savedReceipt = await _receiptRepository.saveReceipt(receipt);

      // Process delivery methods
      for (final deliveryMethod in deliveryMethods) {
        await _processDelivery(savedReceipt, deliveryMethod, customerEmail, customerPhone);
      }

      _receiptController.add(savedReceipt);
      return savedReceipt;
    } catch (e) {
      throw Exception('Failed to generate receipt: $e');
    }
  }

  // Create receipt entity from transaction
  Future<ReceiptEntity> _createReceiptEntity({
    required SalesTransactionEntity transaction,
    required ReceiptType type,
    required ReceiptFormat format,
    required ReceiptSettings settings,
  }) async {
    final lineItems = transaction.items.map((item) => ReceiptLineItem(
      id: item.id?.toString() ?? DateTime.now().millisecondsSinceEpoch.toString(),
      productId: item.productId.toString(),
      productName: item.productName,
      sku: item.productId.toString(), // Use productId as SKU fallback
      quantity: item.quantity,
      unitPrice: item.unitPrice.toDouble() / 100, // Convert from cents
      totalPrice: item.totalPrice.toDouble() / 100, // Convert from cents
      discount: item.discountAmount.toDouble() / 100, // Convert from cents
      tax: 0.0, // No tax field in entity, default to 0
    )).toList();

    final totals = ReceiptTotals(
      subtotal: transaction.subtotal.toDouble() / 100, // Convert from cents
      totalDiscount: transaction.discountAmount.toDouble() / 100, // Convert from cents
      totalTax: transaction.taxAmount.toDouble() / 100, // Convert from cents
      total: transaction.total.toDouble() / 100, // Convert from cents
      amountPaid: transaction.amountPaid.toDouble() / 100, // Convert from cents
      change: transaction.changeAmount.toDouble() / 100, // Convert from cents
      currency: 'GHS',
    );

    final customer = transaction.customerId != null
        ? await _getReceiptCustomer(transaction.customerId!)
        : null;

    // Get payment method from payments list if available
    final payment = transaction.payments.isNotEmpty
        ? ReceiptPayment(
            method: transaction.payments.first.method.toString(), // Convert enum to string
            amount: transaction.payments.first.amount.toDouble() / 100,
            reference: transaction.payments.first.reference,
          )
        : null;

    return ReceiptEntity(
      id: DateTime.now().millisecondsSinceEpoch.toString(),
      receiptNumber: _generateReceiptNumber(),
      type: type,
      transactionId: transaction.id?.toString() ?? '',
      cashierId: transaction.createdById ?? 'system', // Use createdById as cashier
      locationId: 'default', // Default location since not in entity
      receiptData: {
        'transactionNumber': transaction.transactionNumber,
        'status': transaction.status.toString(),
        'type': transaction.type.toString(),
      }, // Create simple data map
      lineItems: lineItems,
      totals: totals,
      customer: customer,
      payment: payment,
      settings: settings,
      createdAt: DateTime.now(),
    );
  }

  // Process delivery method
  Future<void> _processDelivery(
    ReceiptEntity receipt,
    DeliveryMethod method,
    String? email,
    String? phone,
  ) async {
    try {
      switch (method) {
        case DeliveryMethod.print:
          await _printReceipt(receipt);
          break;
        case DeliveryMethod.email:
          if (email != null) {
            await _emailReceipt(receipt, email);
          }
          break;
        case DeliveryMethod.sms:
          if (phone != null) {
            await _smsReceipt(receipt, phone);
          }
          break;
        case DeliveryMethod.whatsapp:
          if (phone != null) {
            await _whatsappReceipt(receipt, phone);
          }
          break;
        case DeliveryMethod.all:
          await _printReceipt(receipt);
          if (email != null) await _emailReceipt(receipt, email);
          if (phone != null) await _smsReceipt(receipt, phone);
          break;
      }
    } catch (e) {
      // Log delivery failure
      await _receiptRepository.addDeliveryAttempt(
        receipt.id,
        ReceiptDelivery(
          id: DateTime.now().millisecondsSinceEpoch.toString(),
          method: method,
          status: ReceiptStatus.failed,
          destination: email ?? phone,
          attemptedAt: DateTime.now(),
          errorMessage: e.toString(),
        ),
      );
    }
  }

  // Print receipt (thermal or PDF)
  Future<void> _printReceipt(ReceiptEntity receipt) async {
    try {
      switch (receipt.format) {
        case ReceiptFormat.thermal:
        case ReceiptFormat.pos58:
        case ReceiptFormat.pos80:
          await _printThermalReceipt(receipt);
          break;
        case ReceiptFormat.a4:
          await _printPdfReceipt(receipt);
          break;
      }

      await _receiptRepository.addDeliveryAttempt(
        receipt.id,
        ReceiptDelivery(
          id: DateTime.now().millisecondsSinceEpoch.toString(),
          method: DeliveryMethod.print,
          status: ReceiptStatus.printed,
          attemptedAt: DateTime.now(),
          completedAt: DateTime.now(),
        ),
      );
    } catch (e) {
      throw Exception('Failed to print receipt: $e');
    }
  }

  // Generate thermal receipt content
  Future<void> _printThermalReceipt(ReceiptEntity receipt) async {
    final thermalContent = _generateThermalReceiptContent(receipt);
    
    // Get default printer
    final printer = await _receiptRepository.getDefaultPrinter();
    if (printer == null) {
      throw Exception('No default printer configured');
    }

    // Send to thermal printer (mock implementation)
    await _sendToThermalPrinter(printer, thermalContent);
  }

  // Generate PDF receipt
  Future<void> _printPdfReceipt(ReceiptEntity receipt) async {
    final pdf = await _generatePdfReceipt(receipt);
    
    // Print using the printing package
    await Printing.layoutPdf(
      onLayout: (PdfPageFormat format) async => pdf.save(),
    );
  }

  // Generate PDF document
  Future<pw.Document> _generatePdfReceipt(ReceiptEntity receipt) async {
    final pdf = pw.Document();

    pdf.addPage(
      pw.Page(
        pageFormat: PdfPageFormat.a4,
        build: (pw.Context context) {
          return pw.Column(
            crossAxisAlignment: pw.CrossAxisAlignment.start,
            children: [
              // Header
              _buildPdfHeader(receipt),
              pw.SizedBox(height: 20),
              
              // Transaction details
              _buildPdfTransactionDetails(receipt),
              pw.SizedBox(height: 20),
              
              // Items table
              _buildPdfItemsTable(receipt),
              pw.SizedBox(height: 20),
              
              // Totals
              _buildPdfTotals(receipt),
              pw.SizedBox(height: 20),
              
              // Payment info
              if (receipt.payment != null)
                _buildPdfPaymentInfo(receipt),
              
              pw.Spacer(),
              
              // Footer
              _buildPdfFooter(receipt),
            ],
          );
        },
      ),
    );

    return pdf;
  }

  // PDF Receipt Sections
  pw.Widget _buildPdfHeader(ReceiptEntity receipt) {
    return pw.Column(
      crossAxisAlignment: pw.CrossAxisAlignment.center,
      children: [
        pw.Text(
          receipt.settings.businessName,
          style: pw.TextStyle(
            fontSize: 24,
            fontWeight: pw.FontWeight.bold,
          ),
        ),
        if (receipt.settings.businessAddress != null) ...[
          pw.SizedBox(height: 5),
          pw.Text(receipt.settings.businessAddress!),
        ],
        if (receipt.settings.businessPhone != null) ...[
          pw.SizedBox(height: 5),
          pw.Text('Tel: ${receipt.settings.businessPhone}'),
        ],
        if (receipt.settings.taxNumber != null) ...[
          pw.SizedBox(height: 5),
          pw.Text('Tax ID: ${receipt.settings.taxNumber}'),
        ],
      ],
    );
  }

  pw.Widget _buildPdfTransactionDetails(ReceiptEntity receipt) {
    return pw.Row(
      mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
      children: [
        pw.Column(
          crossAxisAlignment: pw.CrossAxisAlignment.start,
          children: [
            pw.Text('Receipt #: ${receipt.receiptNumber}'),
            pw.Text('Date: ${_formatDate(receipt.createdAt)}'),
            pw.Text('Time: ${_formatTime(receipt.createdAt)}'),
          ],
        ),
        pw.Column(
          crossAxisAlignment: pw.CrossAxisAlignment.end,
          children: [
            pw.Text('Transaction: ${receipt.transactionId}'),
            pw.Text('Cashier: ${receipt.cashierId}'),
            if (receipt.customer?.name != null)
              pw.Text('Customer: ${receipt.customer!.name}'),
          ],
        ),
      ],
    );
  }

  pw.Widget _buildPdfItemsTable(ReceiptEntity receipt) {
    return pw.Table(
      border: pw.TableBorder.all(),
      children: [
        // Header row
        pw.TableRow(
          decoration: const pw.BoxDecoration(color: PdfColors.grey300),
          children: [
            pw.Padding(
              padding: const pw.EdgeInsets.all(8),
              child: pw.Text('Item', style: pw.TextStyle(fontWeight: pw.FontWeight.bold)),
            ),
            pw.Padding(
              padding: const pw.EdgeInsets.all(8),
              child: pw.Text('Qty', style: pw.TextStyle(fontWeight: pw.FontWeight.bold)),
            ),
            pw.Padding(
              padding: const pw.EdgeInsets.all(8),
              child: pw.Text('Price', style: pw.TextStyle(fontWeight: pw.FontWeight.bold)),
            ),
            pw.Padding(
              padding: const pw.EdgeInsets.all(8),
              child: pw.Text('Total', style: pw.TextStyle(fontWeight: pw.FontWeight.bold)),
            ),
          ],
        ),
        // Data rows
        ...receipt.lineItems.map((item) => pw.TableRow(
          children: [
            pw.Padding(
              padding: const pw.EdgeInsets.all(8),
              child: pw.Column(
                crossAxisAlignment: pw.CrossAxisAlignment.start,
                children: [
                  pw.Text(item.productName),
                  if (item.sku != null)
                    pw.Text(
                      'SKU: ${item.sku}',
                      style: const pw.TextStyle(fontSize: 10, color: PdfColors.grey),
                    ),
                ],
              ),
            ),
            pw.Padding(
              padding: const pw.EdgeInsets.all(8),
              child: pw.Text('${item.quantity}'),
            ),
            pw.Padding(
              padding: const pw.EdgeInsets.all(8),
              child: pw.Text('GHS ${item.unitPrice.toStringAsFixed(2)}'),
            ),
            pw.Padding(
              padding: const pw.EdgeInsets.all(8),
              child: pw.Text('GHS ${item.totalPrice.toStringAsFixed(2)}'),
            ),
          ],
        )),
      ],
    );
  }

  pw.Widget _buildPdfTotals(ReceiptEntity receipt) {
    return pw.Column(
      crossAxisAlignment: pw.CrossAxisAlignment.end,
      children: [
        pw.Row(
          mainAxisAlignment: pw.MainAxisAlignment.end,
          children: [
            pw.Text('Subtotal: '),
            pw.Text('GHS ${receipt.totals.subtotal.toStringAsFixed(2)}'),
          ],
        ),
        if (receipt.totals.totalDiscount > 0) ...[
          pw.SizedBox(height: 5),
          pw.Row(
            mainAxisAlignment: pw.MainAxisAlignment.end,
            children: [
              pw.Text('Discount: '),
              pw.Text('-GHS ${receipt.totals.totalDiscount.toStringAsFixed(2)}'),
            ],
          ),
        ],
        if (receipt.totals.totalTax > 0) ...[
          pw.SizedBox(height: 5),
          pw.Row(
            mainAxisAlignment: pw.MainAxisAlignment.end,
            children: [
              pw.Text('Tax: '),
              pw.Text('GHS ${receipt.totals.totalTax.toStringAsFixed(2)}'),
            ],
          ),
        ],
        pw.SizedBox(height: 10),
        pw.Container(
          padding: const pw.EdgeInsets.all(8),
          decoration: pw.BoxDecoration(
            border: pw.Border.all(),
            color: PdfColors.grey100,
          ),
          child: pw.Row(
            mainAxisAlignment: pw.MainAxisAlignment.end,
            children: [
              pw.Text(
                'Total: GHS ${receipt.totals.total.toStringAsFixed(2)}',
                style: pw.TextStyle(
                  fontSize: 16,
                  fontWeight: pw.FontWeight.bold,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }

  pw.Widget _buildPdfPaymentInfo(ReceiptEntity receipt) {
    return pw.Column(
      crossAxisAlignment: pw.CrossAxisAlignment.start,
      children: [
        pw.Text(
          'Payment Information',
          style: pw.TextStyle(fontWeight: pw.FontWeight.bold),
        ),
        pw.SizedBox(height: 5),
        pw.Text('Method: ${receipt.payment!.method}'),
        pw.Text('Amount Paid: GHS ${receipt.totals.amountPaid.toStringAsFixed(2)}'),
        if (receipt.totals.change > 0)
          pw.Text('Change: GHS ${receipt.totals.change.toStringAsFixed(2)}'),
        if (receipt.payment!.reference != null)
          pw.Text('Reference: ${receipt.payment!.reference}'),
      ],
    );
  }

  pw.Widget _buildPdfFooter(ReceiptEntity receipt) {
    return pw.Column(
      crossAxisAlignment: pw.CrossAxisAlignment.center,
      children: [
        if (receipt.settings.footerMessage != null) ...[
          pw.Text(receipt.settings.footerMessage!),
          pw.SizedBox(height: 10),
        ],
        pw.Text(
          'Thank you for your business!',
          style: pw.TextStyle(fontWeight: pw.FontWeight.bold),
        ),
        pw.SizedBox(height: 10),
        if (receipt.settings.showBarcode)
          pw.BarcodeWidget(
            barcode: pw.Barcode.code128(),
            data: receipt.receiptNumber,
            width: 200,
            height: 50,
          ),
      ],
    );
  }

  // Generate thermal receipt content
  String _generateThermalReceiptContent(ReceiptEntity receipt) {
    final buffer = StringBuffer();
    final settings = receipt.settings;
    final width = settings.paperWidth == 58 ? 32 : 48; // Characters per line

    // Header
    buffer.writeln(_centerText(settings.businessName.toUpperCase(), width));
    if (settings.businessAddress != null) {
      buffer.writeln(_centerText(settings.businessAddress!, width));
    }
    if (settings.businessPhone != null) {
      buffer.writeln(_centerText('Tel: ${settings.businessPhone}', width));
    }
    buffer.writeln('=' * width);

    // Transaction info
    buffer.writeln('Receipt #: ${receipt.receiptNumber}');
    buffer.writeln('Date: ${_formatDate(receipt.createdAt)}');
    buffer.writeln('Time: ${_formatTime(receipt.createdAt)}');
    if (receipt.customer?.name != null) {
      buffer.writeln('Customer: ${receipt.customer!.name}');
    }
    buffer.writeln('-' * width);

    // Items
    for (final item in receipt.lineItems) {
      buffer.writeln(item.productName);
      if (item.sku != null) {
        buffer.writeln('  SKU: ${item.sku}');
      }
      
      final qtyPriceTotal = '${item.quantity}x ${item.unitPrice.toStringAsFixed(2)} = ${item.totalPrice.toStringAsFixed(2)}';
      buffer.writeln('  $qtyPriceTotal');
      
      if (item.discount != null && item.discount! > 0) {
        buffer.writeln('  Discount: -${item.discount!.toStringAsFixed(2)}');
      }
    }

    buffer.writeln('-' * width);

    // Totals
    buffer.writeln(_formatLineItem('Subtotal:', 'GHS ${receipt.totals.subtotal.toStringAsFixed(2)}', width));
    
    if (receipt.totals.totalDiscount > 0) {
      buffer.writeln(_formatLineItem('Discount:', '-GHS ${receipt.totals.totalDiscount.toStringAsFixed(2)}', width));
    }
    
    if (receipt.totals.totalTax > 0) {
      buffer.writeln(_formatLineItem('Tax:', 'GHS ${receipt.totals.totalTax.toStringAsFixed(2)}', width));
    }
    
    buffer.writeln('=' * width);
    buffer.writeln(_formatLineItem('TOTAL:', 'GHS ${receipt.totals.total.toStringAsFixed(2)}', width));
    buffer.writeln('=' * width);

    // Payment
    if (receipt.payment != null) {
      buffer.writeln(_formatLineItem('Payment:', receipt.payment!.method, width));
      buffer.writeln(_formatLineItem('Amount Paid:', 'GHS ${receipt.totals.amountPaid.toStringAsFixed(2)}', width));
      
      if (receipt.totals.change > 0) {
        buffer.writeln(_formatLineItem('Change:', 'GHS ${receipt.totals.change.toStringAsFixed(2)}', width));
      }
      
      if (receipt.payment!.reference != null) {
        buffer.writeln('Ref: ${receipt.payment!.reference}');
      }
    }

    buffer.writeln('-' * width);

    // Footer
    if (settings.footerMessage != null) {
      buffer.writeln(_centerText(settings.footerMessage!, width));
    }
    
    buffer.writeln(_centerText('Thank you for your business!', width));
    
    if (settings.showBarcode) {
      buffer.writeln('');
      buffer.writeln(_centerText('[${receipt.receiptNumber}]', width));
    }

    // Cut command
    buffer.write('\x1D\x56\x41\x10'); // Partial cut command for ESC/POS

    return buffer.toString();
  }

  // Email receipt
  Future<void> _emailReceipt(ReceiptEntity receipt, String email) async {
    try {
      // Generate PDF for email
      final pdf = await _generatePdfReceipt(receipt);
      final pdfBytes = await pdf.save();

      // Mock email service
      await _sendEmailWithAttachment(
        to: email,
        subject: 'Receipt #${receipt.receiptNumber} - ${receipt.settings.businessName}',
        body: _generateEmailBody(receipt),
        attachment: pdfBytes,
        attachmentName: 'receipt_${receipt.receiptNumber}.pdf',
      );

      await _receiptRepository.addDeliveryAttempt(
        receipt.id,
        ReceiptDelivery(
          id: DateTime.now().millisecondsSinceEpoch.toString(),
          method: DeliveryMethod.email,
          status: ReceiptStatus.emailed,
          destination: email,
          attemptedAt: DateTime.now(),
          completedAt: DateTime.now(),
        ),
      );
    } catch (e) {
      throw Exception('Failed to email receipt: $e');
    }
  }

  // SMS receipt
  Future<void> _smsReceipt(ReceiptEntity receipt, String phone) async {
    try {
      final message = _generateSmsMessage(receipt);
      
      // Mock SMS service
      await _sendSms(phone, message);

      await _receiptRepository.addDeliveryAttempt(
        receipt.id,
        ReceiptDelivery(
          id: DateTime.now().millisecondsSinceEpoch.toString(),
          method: DeliveryMethod.sms,
          status: ReceiptStatus.sms,
          destination: phone,
          attemptedAt: DateTime.now(),
          completedAt: DateTime.now(),
        ),
      );
    } catch (e) {
      throw Exception('Failed to send SMS receipt: $e');
    }
  }

  // WhatsApp receipt
  Future<void> _whatsappReceipt(ReceiptEntity receipt, String phone) async {
    try {
      final message = _generateWhatsAppMessage(receipt);
      
      // Mock WhatsApp service
      await _sendWhatsApp(phone, message);

      await _receiptRepository.addDeliveryAttempt(
        receipt.id,
        ReceiptDelivery(
          id: DateTime.now().millisecondsSinceEpoch.toString(),
          method: DeliveryMethod.whatsapp,
          status: ReceiptStatus.sms, // Using SMS status for WhatsApp
          destination: phone,
          attemptedAt: DateTime.now(),
          completedAt: DateTime.now(),
        ),
      );
    } catch (e) {
      throw Exception('Failed to send WhatsApp receipt: $e');
    }
  }

  // Helper methods
  String _centerText(String text, int width) {
    if (text.length >= width) return text;
    final padding = (width - text.length) ~/ 2;
    return ' ' * padding + text;
  }

  String _formatLineItem(String label, String value, int width) {
    final totalLength = label.length + value.length;
    if (totalLength >= width) {
      return '$label $value';
    }
    final spacesNeeded = width - totalLength;
    return '$label${' ' * spacesNeeded}$value';
  }

  String _formatDate(DateTime dateTime) {
    return '${dateTime.day.toString().padLeft(2, '0')}/${dateTime.month.toString().padLeft(2, '0')}/${dateTime.year}';
  }

  String _formatTime(DateTime dateTime) {
    return '${dateTime.hour.toString().padLeft(2, '0')}:${dateTime.minute.toString().padLeft(2, '0')}';
  }

  String _generateReceiptNumber() {
    final timestamp = DateTime.now().millisecondsSinceEpoch;
    return 'R${timestamp.toString().substring(8)}';
  }

  Future<ReceiptCustomer?> _getReceiptCustomer(String customerId) async {
    // Mock implementation - in real app, fetch from customer repository
    return const ReceiptCustomer(
      id: 'customer_1',
      name: 'John Doe',
      phone: '+233200123456',
      email: 'john.doe@example.com',
      loyaltyNumber: 'LOY001',
      loyaltyPoints: 150,
    );
  }

  String _generateEmailBody(ReceiptEntity receipt) {
    return '''
Dear ${receipt.customer?.name ?? 'Valued Customer'},

Thank you for your purchase at ${receipt.settings.businessName}.

Receipt Details:
- Receipt Number: ${receipt.receiptNumber}
- Date: ${_formatDate(receipt.createdAt)}
- Total Amount: GHS ${receipt.totals.total.toStringAsFixed(2)}

Please find your detailed receipt attached as a PDF.

Thank you for your business!

Best regards,
${receipt.settings.businessName}
${receipt.settings.businessPhone ?? ''}
''';
  }

  String _generateSmsMessage(ReceiptEntity receipt) {
    return '''
${receipt.settings.businessName}
Receipt #${receipt.receiptNumber}
${_formatDate(receipt.createdAt)} ${_formatTime(receipt.createdAt)}
Total: GHS ${receipt.totals.total.toStringAsFixed(2)}
Thank you for your business!
''';
  }

  String _generateWhatsAppMessage(ReceiptEntity receipt) {
    return '''
🧾 *Receipt from ${receipt.settings.businessName}*

📋 Receipt #${receipt.receiptNumber}
📅 ${_formatDate(receipt.createdAt)} at ${_formatTime(receipt.createdAt)}

💰 Total: *GHS ${receipt.totals.total.toStringAsFixed(2)}*

Thank you for choosing us! 🙏
''';
  }

  // Mock service methods
  Future<void> _sendToThermalPrinter(PrinterConfig printer, String content) async {
    // Mock thermal printer implementation
    await Future.delayed(const Duration(milliseconds: 500));
    print('Sending to thermal printer ${printer.name}: $content');
  }

  Future<void> _sendEmailWithAttachment(
      {required String to,
      required String subject,
      required String body,
      required Uint8List attachment,
      required String attachmentName}) async {
    // Mock email service implementation
    await Future.delayed(const Duration(seconds: 1));
    print('Email sent to $to with attachment $attachmentName');
  }

  Future<void> _sendSms(String phone, String message) async {
    // Mock SMS service implementation
    await Future.delayed(const Duration(milliseconds: 300));
    print('SMS sent to $phone: $message');
  }

  Future<void> _sendWhatsApp(String phone, String message) async {
    // Mock WhatsApp service implementation
    await Future.delayed(const Duration(milliseconds: 500));
    print('WhatsApp sent to $phone: $message');
  }

  // Receipt management methods
  Future<List<ReceiptEntity>> getReceiptHistory({
    String? customerId,
    DateTime? startDate,
    DateTime? endDate,
    ReceiptType? type,
  }) async {
    return await _receiptRepository.getReceiptHistory(
      customerId: customerId,
      startDate: startDate,
      endDate: endDate,
      type: type,
    );
  }

  Future<ReceiptEntity?> getReceiptById(String receiptId) async {
    return await _receiptRepository.getReceiptById(receiptId);
  }

  Future<void> reprintReceipt(String receiptId) async {
    final originalReceipt = await _receiptRepository.getReceiptById(receiptId);
    if (originalReceipt == null) {
      throw Exception('Receipt not found');
    }

    // Create reprint receipt
    final reprintReceipt = ReceiptEntity(
      id: DateTime.now().millisecondsSinceEpoch.toString(),
      receiptNumber: originalReceipt.receiptNumber,
      type: originalReceipt.type,
      format: originalReceipt.format,
      transactionId: originalReceipt.transactionId,
      customerId: originalReceipt.customerId,
      cashierId: originalReceipt.cashierId,
      locationId: originalReceipt.locationId,
      receiptData: originalReceipt.receiptData,
      lineItems: originalReceipt.lineItems,
      totals: originalReceipt.totals,
      customer: originalReceipt.customer,
      payment: originalReceipt.payment,
      settings: originalReceipt.settings,
      createdAt: DateTime.now(),
      isReprint: true,
      originalReceiptId: originalReceipt.id,
    );

    await _printReceipt(reprintReceipt);
    await _receiptRepository.saveReceipt(reprintReceipt);
  }

  void dispose() {
    _receiptController.close();
  }
}

// Mock Receipt Repository
class ReceiptRepository {
  final List<ReceiptEntity> _receipts = [];
  final List<PrinterConfig> _printers = [];

  Future<ReceiptEntity> saveReceipt(ReceiptEntity receipt) async {
    await Future.delayed(const Duration(milliseconds: 200));
    _receipts.add(receipt);
    return receipt;
  }

  Future<List<ReceiptEntity>> getReceiptHistory({
    String? customerId,
    DateTime? startDate,
    DateTime? endDate,
    ReceiptType? type,
  }) async {
    await Future.delayed(const Duration(milliseconds: 300));
    
    var filtered = _receipts.where((receipt) {
      if (customerId != null && receipt.customerId != customerId) return false;
      if (type != null && receipt.type != type) return false;
      if (startDate != null && receipt.createdAt.isBefore(startDate)) return false;
      if (endDate != null && receipt.createdAt.isAfter(endDate)) return false;
      return true;
    }).toList();

    filtered.sort((a, b) => b.createdAt.compareTo(a.createdAt));
    return filtered;
  }

  Future<ReceiptEntity?> getReceiptById(String receiptId) async {
    await Future.delayed(const Duration(milliseconds: 200));
    try {
      return _receipts.firstWhere((receipt) => receipt.id == receiptId);
    } catch (e) {
      return null;
    }
  }

  Future<void> addDeliveryAttempt(String receiptId, ReceiptDelivery delivery) async {
    await Future.delayed(const Duration(milliseconds: 100));
    // In real implementation, update the receipt's deliveries list
  }

  Future<PrinterConfig?> getDefaultPrinter() async {
    await Future.delayed(const Duration(milliseconds: 100));
    try {
      return _printers.firstWhere((printer) => printer.isDefault);
    } catch (e) {
      return null;
    }
  }

  Future<List<PrinterConfig>> getAllPrinters() async {
    await Future.delayed(const Duration(milliseconds: 200));
    return List.from(_printers);
  }

  Future<void> savePrinter(PrinterConfig printer) async {
    await Future.delayed(const Duration(milliseconds: 200));
    
    // If this is set as default, remove default from others
    if (printer.isDefault) {
      for (int i = 0; i < _printers.length; i++) {
        if (_printers[i].isDefault) {
          _printers[i] = PrinterConfig(
            id: _printers[i].id,
            name: _printers[i].name,
            type: _printers[i].type,
            connectionString: _printers[i].connectionString,
            paperWidth: _printers[i].paperWidth,
            defaultFormat: _printers[i].defaultFormat,
            isDefault: false,
            isActive: _printers[i].isActive,
            settings: _printers[i].settings,
            createdAt: _printers[i].createdAt,
            lastUsedAt: _printers[i].lastUsedAt,
          );
        }
      }
    }
    
    _printers.add(printer);
  }
}
