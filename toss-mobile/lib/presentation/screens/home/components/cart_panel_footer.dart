import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../../app/themes/app_sizes.dart';
import '../../../../app/utilities/console_log.dart';
import '../../../../app/utilities/currency_formatter.dart';
import '../../../../service_locator.dart';
import '../../../providers/home/home_provider.dart';
import '../../../widgets/app_button.dart';
import '../../../widgets/app_dialog.dart';
import '../../../widgets/app_drop_down.dart';
import '../../../widgets/app_text_field.dart';

class CartPanelFooter extends StatefulWidget {
  const CartPanelFooter({super.key});

  @override
  State<CartPanelFooter> createState() => _CartPanelFooterState();
}

class _CartPanelFooterState extends State<CartPanelFooter> {
  final _homeProvider = sl<HomeProvider>();

  final _amountControlller = TextEditingController();
  final _customerControlller = TextEditingController();
  final _customerPhoneController = TextEditingController();
  final _customerEmailController = TextEditingController();
  final _descriptionControlller = TextEditingController();

  @override
  void dispose() {
    _amountControlller.dispose();
    _customerControlller.dispose();
    _customerPhoneController.dispose();
    _customerEmailController.dispose();
    _descriptionControlller.dispose();
    super.dispose();
  }

  Widget _summaryRow(BuildContext context, String label, String value, {bool isBold = false, Color? color}) {
    final style = isBold
        ? Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.bold, color: color)
        : Theme.of(context).textTheme.bodyMedium?.copyWith(color: color);
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 2),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(label, style: Theme.of(context).textTheme.bodySmall),
          Text(value, style: style),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      width: AppSizes.screenWidth(context),
      padding: const EdgeInsets.fromLTRB(AppSizes.padding, 0, AppSizes.padding, AppSizes.padding),
      color: Theme.of(context).colorScheme.surfaceContainerLowest,
      child: Row(
        children: [
          Consumer<HomeProvider>(
            builder: (context, provider, _) {
              return AnimatedContainer(
                width: provider.isPanelExpanded ? AppSizes.screenWidth(context) / 3 : 0,
                duration: const Duration(milliseconds: 300),
                curve: Curves.easeInOut,
                child: SingleChildScrollView(
                  scrollDirection: Axis.horizontal,
                  child: SizedBox(
                    width: AppSizes.screenWidth(context) / 3 - AppSizes.padding / 2,
                    child: backButton(),
                  ),
                ),
              );
            },
          ),
          Expanded(
            flex: 2,
            child: receiptButton(),
          ),
        ],
      ),
    );
  }

  Widget backButton() {
    return AppButton(
      text: 'Back',
      buttonColor: Theme.of(context).colorScheme.surface,
      borderColor: Theme.of(context).colorScheme.primary,
      textColor: Theme.of(context).colorScheme.primary,
      onTap: () {
        _homeProvider.onChangedIsPanelExpanded(false);
        _homeProvider.panelController.close();
      },
    );
  }

  Widget receiptButton() {
    return Consumer<HomeProvider>(
      builder: (context, provider, _) {
        return AppButton(
          text: !provider.isPanelExpanded
              ? provider.orderedProducts.isNotEmpty
                    ? "${provider.orderedProducts.length} Products = ${CurrencyFormatter.format(provider.getTotalAmount())}"
                    : 'Transaction'
              : 'Pay',
          enabled: provider.orderedProducts.isNotEmpty,
          onTap: () {
            if (_homeProvider.isPanelExpanded) {
              AppDialog.show(child: additionalInfoDialog(), showButtons: false);
            } else {
              /// Expands cart panel
              _homeProvider.onChangedIsPanelExpanded(!_homeProvider.isPanelExpanded);

              if (!_homeProvider.isPanelExpanded) {
                _homeProvider.panelController.close();
              } else {
                _homeProvider.panelController.open();
              }
            }
          },
        );
      },
    );
  }

  Widget additionalInfoDialog() {
    return Consumer<HomeProvider>(
      builder: (context, provider, _) {
        return Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Row(
              children: [
                Expanded(
                  child: AppTextField(
                    autofocus: true,
                    keyboardType: TextInputType.number,
                    controller: _amountControlller,
                    labelText: 'Cash Received',
                    hintText: '0',
                    onChanged: (val) {
                      provider.onChangedCashAmount(int.tryParse(val) ?? 0);
                    },
                  ),
                ),
                const SizedBox(width: AppSizes.padding / 2),
                Expanded(
                  child: AppTextField(
                    keyboardType: TextInputType.number,
                    labelText: 'Bank Received',
                    hintText: '0',
                    onChanged: (val) {
                      provider.onChangedBankAmount(int.tryParse(val) ?? 0);
                    },
                  ),
                ),
              ],
            ),
            const SizedBox(height: AppSizes.padding),
            // AI suggestion banner (rules-based, local)
            Builder(builder: (context) {
              final tip = provider.getUpsellSuggestion();
              if (tip == null) return const SizedBox.shrink();
              return Container(
                width: double.infinity,
                padding: const EdgeInsets.all(12),
                margin: const EdgeInsets.only(bottom: AppSizes.padding),
                decoration: BoxDecoration(
                  color: Theme.of(context).colorScheme.secondaryContainer,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Icon(Icons.lightbulb_outline, color: Theme.of(context).colorScheme.onSecondaryContainer),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        'Suggestion: $tip',
                        style: Theme.of(context).textTheme.bodySmall,
                      ),
                    ),
                  ],
                ),
              );
            }),
            
            AppDropDown(
              labelText: 'Payment Method',
              selectedValue: _homeProvider.selectedPaymentMethod,
              dropdownItems: const [
                DropdownMenuItem(
                  value: 'bank',
                  child: Text('Bank'),
                ),
                DropdownMenuItem(
                  value: 'cash',
                  child: Text('Cash'),
                ),
                DropdownMenuItem(
                  value: 'split',
                  child: Text('Split (Cash + Bank)'),
                ),
                DropdownMenuItem(
                  value: 'invoice',
                  child: Text('Invoice (Pay later)'),
                ),
              ],
              onChanged: provider.onChangedPaymentMethod,
            ),
            const SizedBox(height: AppSizes.padding),
            // Order summary: Subtotal, Tax, Total, Due/Change
            Builder(builder: (context) {
              final subtotal = provider.getTotalAmount();
              final tax = provider.getTaxAmount();
              final total = provider.getFinalTotalAmount();
              final received = provider.cashAmount + provider.bankAmount;
              final due = (total - received).clamp(0, 1 << 30);
              final change = (received - total).clamp(0, 1 << 30);
              return Container(
                width: double.infinity,
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Theme.of(context).colorScheme.surface,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Column(
                  children: [
                    _summaryRow(context, 'Subtotal', CurrencyFormatter.format(subtotal)),
                    if (tax > 0) _summaryRow(context, 'Tax', CurrencyFormatter.format(tax)),
                    const Divider(height: 16),
                    _summaryRow(context, 'Total to Pay', CurrencyFormatter.format(total), isBold: true),
                    const SizedBox(height: 4),
                    _summaryRow(
                      context,
                      received >= total ? 'Change' : 'Amount Due',
                      CurrencyFormatter.format(received >= total ? change : due),
                      color: received >= total ? Colors.green : Colors.red,
                    ),
                  ],
                ),
              );
            }),
            const SizedBox(height: AppSizes.padding),
            AppTextField(
              keyboardType: TextInputType.number,
              labelText: 'Tax % (optional)',
              hintText: '0-100',
              onChanged: (val) {
                final parsed = double.tryParse(val);
                provider.onChangedTaxPercent(parsed);
              },
            ),
            const SizedBox(height: AppSizes.padding),
            Row(
              children: [
                Expanded(
                  child: AppTextField(
                    keyboardType: TextInputType.number,
                    labelText: 'Discount Amount (optional)',
                    hintText: '0',
                    onChanged: (val) {
                      provider.onChangedDiscountAmount(int.tryParse(val) ?? 0);
                    },
                  ),
                ),
                const SizedBox(width: AppSizes.padding / 2),
                Expanded(
                  child: AppTextField(
                    keyboardType: TextInputType.number,
                    labelText: 'Discount % (optional)',
                    hintText: '0-100',
                    onChanged: (val) {
                      final parsed = double.tryParse(val);
                      provider.onChangedDiscountPercent(parsed);
                    },
                  ),
                ),
              ],
            ),
            const SizedBox(height: AppSizes.padding),
            AppTextField(
              controller: _customerControlller,
              labelText: 'Customer Name (Optional)',
              hintText: 'e.g. Jhone Doe',
              onChanged: provider.onChangedCustomerName,
            ),
            const SizedBox(height: AppSizes.padding),
            AppTextField(
              controller: _customerPhoneController,
              keyboardType: TextInputType.phone,
              labelText: 'Customer Phone (Optional)',
              hintText: '+27XXXXXXXXX',
              onChanged: provider.onChangedCustomerPhone,
            ),
            const SizedBox(height: AppSizes.padding / 2),
            Row(
              children: [
                Expanded(
                  child: AppButton(
                    text: 'Attach Customer',
                    onTap: () async {
                      final phone = _customerPhoneController.text.trim();
                      if (phone.isEmpty) {
                        ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Enter phone to attach customer')));
                        return;
                      }
                      await provider.findAndAttachCustomerByPhone(phone, name: _customerControlller.text.trim());
                    },
                  ),
                ),
              ],
            ),
            if (provider.selectedCustomerId != null) ...[
              const SizedBox(height: AppSizes.padding / 2),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text('Points Balance', style: Theme.of(context).textTheme.bodySmall),
                  Text('${provider.selectedCustomerPointsBalance}', style: Theme.of(context).textTheme.bodyMedium),
                ],
              ),
              const SizedBox(height: AppSizes.padding / 2),
              AppTextField(
                keyboardType: TextInputType.number,
                labelText: 'Redeem Points (optional)',
                hintText: '0',
                onChanged: (val) {
                  provider.onChangedPointsToRedeem(int.tryParse(val) ?? 0);
                },
              ),
            ],
            const SizedBox(height: AppSizes.padding),
            AppTextField(
              controller: _customerEmailController,
              keyboardType: TextInputType.emailAddress,
              labelText: 'Customer Email (Optional)',
              hintText: 'name@example.com',
              onChanged: provider.onChangedCustomerEmail,
            ),
            const SizedBox(height: AppSizes.padding),
            AppTextField(
              controller: _descriptionControlller,
              labelText: 'Description (Optional)',
              hintText: 'Description...',
              onChanged: provider.onChangedDescription,
            ),
            const SizedBox(height: AppSizes.padding * 1.5),
            Row(
              children: [
                Expanded(
                  child: AppButton(
                    text: 'Cancel',
                    buttonColor: Theme.of(context).colorScheme.surface,
                    borderColor: Theme.of(context).colorScheme.primary,
                    textColor: Theme.of(context).colorScheme.primary,
                    onTap: () {
                      context.pop();
                    },
                  ),
                ),
                const SizedBox(width: AppSizes.padding / 2),
                Expanded(
                  flex: 2,
                  child: AppButton(
                    text: () {
                      final total = provider.getFinalTotalAmount();
                      final received = provider.cashAmount + provider.bankAmount;
                      final due = (total - received).clamp(0, 1 << 30);
                      if (provider.selectedPaymentMethod == 'invoice') return 'Create Invoice';
                      return provider.isPaymentCovered()
                          ? 'Pay ${CurrencyFormatter.format(total)}'
                          : 'Pay ${CurrencyFormatter.format(total)} (Due ${CurrencyFormatter.format(due)})';
                    }(),
                    enabled: provider.isPaymentCovered(),
                    onTap: () {
                      context.pop();
                      onPay();
                    },
                  ),
                ),
              ],
            ),
          ],
        );
      },
    );
  }

  void onPay() async {
    final router = GoRouter.of(context);
    final messenger = ScaffoldMessenger.of(context);

    AppDialog.showDialogProgress();

    var res = await _homeProvider.createTransaction();

    AppDialog.closeDialog();

    if (res.isSuccess) {
      router.go('/transactions/transaction-detail/${res.data}');
      messenger.showSnackBar(const SnackBar(content: Text('Transaction created')));
    } else {
      cl('[createTransaction].error ${res.error}');
      AppDialog.showErrorDialog(error: res.error?.message);
    }
  }
}
