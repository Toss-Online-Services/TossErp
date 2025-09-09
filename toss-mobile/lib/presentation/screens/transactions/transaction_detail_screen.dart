import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../app/themes/app_colors.dart';
import '../../../app/themes/app_sizes.dart';
import '../../../app/utilities/currency_formatter.dart';
import '../../../app/utilities/date_formatter.dart';
import '../../../app/utilities/external_launcher.dart';
import '../../../core/extensions/string_casing_extension.dart';
import '../../../domain/entities/ordered_product_entity.dart';
import '../../../domain/entities/transaction_entity.dart';
import '../../../service_locator.dart';
import '../../providers/transactions/transaction_detail_provider.dart';
import '../../widgets/app_empty_state.dart';
import '../../widgets/app_progress_indicator.dart';
import '../error_handler_screen.dart';
import '../../widgets/app_dialog.dart';

class TransactionDetailScreen extends StatelessWidget {
  final int id;

  const TransactionDetailScreen({super.key, required this.id});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(elevation: 0),
      body: FutureBuilder(
        future: sl<TransactionDetailProvider>().getTransactionDetail(id),
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const AppProgressIndicator();
          }

          if (snapshot.hasError) {
            return ErrorScreen(errorMessage: snapshot.error.toString());
          }

          if (snapshot.data == null) {
            return const AppEmptyState(title: 'Not Found');
          }

          return SingleChildScrollView(
            padding: const EdgeInsets.all(AppSizes.padding),
            child: Column(
              children: [
                status(context),
                const SizedBox(height: AppSizes.padding * 2),
                transactionDetail(context, snapshot.data!),
                const SizedBox(height: AppSizes.padding),
                paymentDetail(context, snapshot.data!),
                const SizedBox(height: AppSizes.padding),
                shareReceiptRow(context, snapshot.data!),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget status(BuildContext context) {
    return Column(
      children: [
        const Icon(
          Icons.check_circle_outline_rounded,
          color: AppColors.green,
          size: 60,
        ),
        const SizedBox(height: AppSizes.padding / 2),
        Text(
          'Transaction Created',
          textAlign: TextAlign.center,
          style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.bold),
        ),
      ],
    );
  }

  Widget transactionDetail(BuildContext context, TransactionEntity transaction) {
    return Container(
      padding: const EdgeInsets.all(AppSizes.padding),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        borderRadius: BorderRadius.circular(AppSizes.radius),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Transaction ID',
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.bold),
              ),
              Text(
                '${transaction.id ?? '-'}',
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.bold),
              ),
            ],
          ),
          const SizedBox(height: AppSizes.padding),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Payment Method',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
              Text(
                transaction.paymentMethod.toTitleCase(),
                style: Theme.of(context).textTheme.bodyMedium,
              ),
            ],
          ),
          const SizedBox(height: AppSizes.padding),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Created By',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
              Text(
                transaction.createdBy?.name ?? '-',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
            ],
          ),
          const SizedBox(height: AppSizes.padding),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Created At',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
              Text(
                DateFormatter.normalWithClock(transaction.createdAt ?? ''),
                style: Theme.of(context).textTheme.bodyMedium,
              ),
            ],
          ),
          const SizedBox(height: AppSizes.padding),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Customer Name',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
              Text(
                transaction.customerName ?? '-',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
            ],
          ),
          const SizedBox(height: AppSizes.padding),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Customer Phone',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
              Text(
                transaction.customerPhone ?? '-',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
            ],
          ),
          const SizedBox(height: AppSizes.padding),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Description',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
              Text(
                transaction.description ?? '-',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget paymentDetail(BuildContext context, TransactionEntity transaction) {
    return Container(
      padding: const EdgeInsets.all(AppSizes.padding),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        borderRadius: BorderRadius.circular(AppSizes.radius),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Ordered Products',
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.bold),
              ),
              Text(
                '${transaction.orderedProducts?.length ?? '0'}',
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.bold),
              ),
            ],
          ),
          const Divider(height: AppSizes.padding * 2),
          ...List.generate(transaction.orderedProducts?.length ?? 0, (i) {
            return Padding(
              padding: EdgeInsets.only(top: i == 0 ? 0 : AppSizes.padding / 2),
              child: product(context, transaction.orderedProducts![i]),
            );
          }),
          const Divider(height: AppSizes.padding * 2),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Total',
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.bold),
              ),
              Text(
                CurrencyFormatter.format(transaction.totalAmount),
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.bold),
              ),
            ],
          ),
          const SizedBox(height: AppSizes.padding),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Payment Received',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
              Text(
                CurrencyFormatter.format(transaction.receivedAmount),
                style: Theme.of(context).textTheme.bodyMedium,
              ),
            ],
          ),
          const SizedBox(height: AppSizes.padding),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'Change',
                style: Theme.of(context).textTheme.bodyMedium,
              ),
              Text(
                CurrencyFormatter.format(transaction.receivedAmount - transaction.totalAmount),
                style: Theme.of(context).textTheme.bodyMedium,
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget shareReceiptRow(BuildContext context, TransactionEntity transaction) {
    final message = _composeReceiptMessage(transaction);
    return Row(
      children: [
        Expanded(child: _shareWhatsAppButton(context, transaction, message)),
        const SizedBox(width: 8),
        Expanded(child: _copyReceiptButton(context, message)),
      ],
    );
  }

  Widget _shareWhatsAppButton(BuildContext context, TransactionEntity transaction, String message) {
    return ElevatedButton.icon(
      onPressed: () {
        final raw = transaction.customerPhone ?? '';
        final sanitized = raw.replaceAll(RegExp(r'[\s\-\(\)]'), '');
        if (sanitized.isEmpty) {
          _promptPhoneAndShare(context, message);
        } else if (!_isLikelyPhone(sanitized)) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Enter a valid phone incl. country code, e.g. +27XXXXXXXXX')),
          );
        } else {
          ExternalLauncher.openWhatsApp(phone: sanitized, message: message);
        }
      },
      icon: Icon(Icons.share, color: Theme.of(context).colorScheme.onPrimary),
      label: const Text('WhatsApp'),
    );
  }

  Widget _copyReceiptButton(BuildContext context, String message) {
    return ElevatedButton.icon(
      onPressed: () async {
        await Clipboard.setData(ClipboardData(text: message));
        ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Receipt copied')));
      },
      icon: Icon(Icons.copy, color: Theme.of(context).colorScheme.onPrimary),
      label: const Text('Copy'),
    );
  }

  void _promptPhoneAndShare(BuildContext context, String message) {
    final controller = TextEditingController();
    AppDialog.show(
      title: 'Enter customer phone',
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          TextField(
            controller: controller,
            keyboardType: TextInputType.phone,
            decoration: const InputDecoration(hintText: '+27XXXXXXXXX'),
          ),
          const SizedBox(height: 8),
          Text('We will only use this to send the receipt.',
              style: Theme.of(context).textTheme.labelSmall),
        ],
      ),
      leftButtonText: 'Cancel',
      rightButtonText: 'Send',
      onTapLeftButton: () => AppDialog.closeDialog(),
      onTapRightButton: () {
        final phone = controller.text.trim();
        final sanitized = phone.replaceAll(RegExp(r'[\s\-\(\)]'), '');
        if (sanitized.isEmpty || !_isLikelyPhone(sanitized)) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Enter a valid phone incl. country code, e.g. +27XXXXXXXXX')),
          );
          return;
        }
        AppDialog.closeDialog();
        ExternalLauncher.openWhatsApp(phone: sanitized, message: message);
      },
      enableRightButton: true,
    );
  }

  bool _isLikelyPhone(String input) {
    final trimmed = input.trim();
    // E.164-lite: optional '+', first digit 1-9, total length 8-15
    final regex = RegExp(r'^\+?[1-9]\d{7,14}$');
    return regex.hasMatch(trimmed);
  }

  String _composeReceiptMessage(TransactionEntity t) {
    final buffer = StringBuffer();
    buffer.writeln('Receipt #${t.id}');
    buffer.writeln('Date: ${DateFormatter.normalWithClock(t.createdAt ?? '')}');
    buffer.writeln('Payment: ${t.paymentMethod.toTitleCase()}');
    if ((t.customerName ?? '').isNotEmpty) buffer.writeln('Customer: ${t.customerName}');
    buffer.writeln('Items:');
    for (final p in t.orderedProducts ?? []) {
      buffer.writeln('- ${p.name} x${p.quantity} @ ${CurrencyFormatter.format(p.price)}');
    }
    buffer.writeln('Total: ${CurrencyFormatter.format(t.totalAmount)}');
    buffer.writeln('Paid: ${CurrencyFormatter.format(t.receivedAmount)}');
    buffer.writeln('Change: ${CurrencyFormatter.format(t.receivedAmount - t.totalAmount)}');
    return buffer.toString();
  }

  Widget product(BuildContext context, OrderedProductEntity order) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          order.name,
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.bold),
        ),
        const SizedBox(height: AppSizes.padding / 4),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              '${CurrencyFormatter.format(order.price)} x ${order.quantity}',
              style: Theme.of(context).textTheme.bodyMedium,
            ),
            Text(
              CurrencyFormatter.format((order.price) * order.quantity),
              style: Theme.of(context).textTheme.bodyMedium,
            ),
          ],
        ),
      ],
    );
  }
}
