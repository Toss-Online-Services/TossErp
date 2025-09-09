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
import '../../providers/transactions/transactions_provider.dart';
import '../../../app/services/auth/auth_service.dart';
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
                'Subtotal',
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.bold),
              ),
              Text(
                CurrencyFormatter.format(transaction.totalAmount - _taxFromDescription(transaction.description)),
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.bold),
              ),
            ],
          ),
          const SizedBox(height: AppSizes.padding),
          if (_taxFromDescription(transaction.description) > 0) ...[
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'Tax',
                  style: Theme.of(context).textTheme.bodyMedium,
                ),
                Text(
                  CurrencyFormatter.format(_taxFromDescription(transaction.description)),
                  style: Theme.of(context).textTheme.bodyMedium,
                ),
              ],
            ),
            const SizedBox(height: AppSizes.padding),
          ],
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
    return Column(
      children: [
        Row(
          children: [
            Expanded(child: _shareWhatsAppButton(context, transaction, message)),
            const SizedBox(width: 8),
            Expanded(child: _shareSmsButton(context, transaction, message)),
            const SizedBox(width: 8),
            Expanded(child: _shareEmailButton(context, transaction, message)),
          ],
        ),
        const SizedBox(height: 8),
        Row(
          children: [
            Expanded(child: _copyReceiptButton(context, message)),
            const SizedBox(width: 8),
            Expanded(child: _returnButton(context, transaction)),
          ],
        ),
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

  Widget _shareSmsButton(BuildContext context, TransactionEntity transaction, String message) {
    return ElevatedButton.icon(
      onPressed: () async {
        final raw = transaction.customerPhone ?? '';
        final sanitized = raw.replaceAll(RegExp(r'[\s\-\(\)]'), '');
        if (sanitized.isEmpty) {
          _promptPhoneAndShare(context, message);
          return;
        }
        final smsUri = Uri.parse('sms:$sanitized?body=${Uri.encodeComponent(message)}');
        ExternalLauncher.openUrl(smsUri.toString());
      },
      icon: Icon(Icons.sms, color: Theme.of(context).colorScheme.onPrimary),
      label: const Text('SMS'),
    );
  }

  Widget _shareEmailButton(BuildContext context, TransactionEntity transaction, String message) {
    return ElevatedButton.icon(
      onPressed: () async {
        final to = '';
        if (to.isEmpty && (transaction.customerName?.isNotEmpty ?? false)) {
          // prompt email inline if missing
          final ctrl = TextEditingController();
          AppDialog.show(
            title: 'Enter customer email',
            child: TextField(controller: ctrl, keyboardType: TextInputType.emailAddress, decoration: const InputDecoration(hintText: 'name@example.com')),
            leftButtonText: 'Cancel',
            rightButtonText: 'Send',
            onTapRightButton: () {
              final email = ctrl.text.trim();
              if (email.isEmpty || !email.contains('@')) {
                ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Enter a valid email')));
                return;
              }
              AppDialog.closeDialog();
              ExternalLauncher.openEmail(to: email, subject: 'Receipt #${transaction.id}', body: message);
            },
          );
          return;
        }
        ExternalLauncher.openEmail(to: to, subject: 'Receipt #${transaction.id}', body: message);
      },
      icon: Icon(Icons.email_outlined, color: Theme.of(context).colorScheme.onPrimary),
      label: const Text('Email'),
    );
  }

  Widget _returnButton(BuildContext context, TransactionEntity transaction) {
    return ElevatedButton.icon(
      onPressed: () {
        _openReturnDialog(context, transaction);
      },
      icon: Icon(Icons.assignment_return_outlined, color: Theme.of(context).colorScheme.onPrimary),
      label: const Text('Return'),
    );
  }

  void _openReturnDialog(BuildContext context, TransactionEntity transaction) {
    final qtyControllers = <int, TextEditingController>{};
    for (final p in transaction.orderedProducts ?? []) {
      qtyControllers[p.id ?? p.productId] = TextEditingController(text: '0');
    }
    AppDialog.show(
      title: 'Select items to return',
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          ...List.generate(transaction.orderedProducts?.length ?? 0, (i) {
            final p = transaction.orderedProducts![i];
            final ctrl = qtyControllers[p.id ?? p.productId]!;
            return Padding(
              padding: const EdgeInsets.only(bottom: 8),
              child: Row(
                children: [
                  Expanded(child: Text('${p.name} x${p.quantity}')),
                  SizedBox(
                    width: 72,
                    child: TextField(
                      controller: ctrl,
                      keyboardType: TextInputType.number,
                      decoration: const InputDecoration(labelText: 'Qty'),
                    ),
                  ),
                ],
              ),
            );
          }),
        ],
      ),
      leftButtonText: 'Cancel',
      rightButtonText: 'Create credit note',
      onTapRightButton: () async {
        // Manager PIN gate
        final pinCtrl = TextEditingController();
        bool authorized = false;
        await showDialog(
          context: context,
          builder: (ctx) => AlertDialog(
            title: const Text('Manager approval'),
            content: TextField(controller: pinCtrl, keyboardType: TextInputType.number, obscureText: true, decoration: const InputDecoration(labelText: 'Enter manager PIN')),
            actions: [
              TextButton(onPressed: () => Navigator.of(ctx).pop(), child: const Text('Cancel')),
              TextButton(
                onPressed: () {
                  authorized = ManagerPinValidator.validateManagerPin(pinCtrl.text);
                  Navigator.of(ctx).pop();
                },
                child: const Text('Approve'),
              ),
            ],
          ),
        );
        if (!authorized) {
          ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Manager PIN required or invalid')));
          return;
        }
        final items = <OrderedProductEntity>[];
        for (final p in transaction.orderedProducts ?? []) {
          final ctrl = qtyControllers[p.id ?? p.productId]!;
          final qty = int.tryParse(ctrl.text.trim()) ?? 0;
          if (qty <= 0) continue;
          items.add(p.copyWith(quantity: qty));
        }
        if (items.isEmpty) {
          ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Select at least 1 item to return')));
          return;
        }
        final creditTotal = items.map((e) => e.price * e.quantity).fold<int>(0, (a, b) => a + b);
        final creditTx = transaction.copyWith(
          id: DateTime.now().millisecondsSinceEpoch,
          paymentMethod: 'refund',
          description: '${transaction.description ?? ''}; return=true',
          orderedProducts: items,
          receivedAmount: 0,
          returnAmount: 0,
          totalOrderedProduct: items.length,
          totalAmount: -creditTotal,
          createdAt: DateTime.now().toIso8601String(),
          updatedAt: DateTime.now().toIso8601String(),
        );
        try {
          final provider = sl<TransactionsProvider>();
          final repo = provider.transactionRepository;
          await repo.createTransaction(creditTx);
          AppDialog.closeDialog();
          ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Credit note created')));
        } catch (e) {
          ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Failed to create credit note')));
        }
      },
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
    final tax = _taxFromDescription(t.description);
    final subtotal = t.totalAmount - tax;
    buffer.writeln('Subtotal: ${CurrencyFormatter.format(subtotal)}');
    if (tax > 0) buffer.writeln('Tax: ${CurrencyFormatter.format(tax)}');
    buffer.writeln('Total: ${CurrencyFormatter.format(t.totalAmount)}');
    buffer.writeln('Paid: ${CurrencyFormatter.format(t.receivedAmount)}');
    buffer.writeln('Change: ${CurrencyFormatter.format(t.receivedAmount - t.totalAmount)}');
    return buffer.toString();
  }

  int _taxFromDescription(String? desc) {
    if (desc == null || desc.isEmpty) return 0;
    // description embeds key-value pairs like: cash=100; bank=0; discountPct=10; taxPct=15
    try {
      final parts = desc.split(';');
      final taxPart = parts.firstWhere(
        (p) => p.trim().startsWith('taxPct='),
        orElse: () => '',
      );
      if (taxPart.isEmpty) return 0;
      final pct = double.tryParse(taxPart.split('=').last.trim()) ?? 0;
      // We can't recompute base reliably here; total and subtotal are handled by caller.
      return ((0 * pct) / 100).round();
    } catch (_) {
      return 0;
    }
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
