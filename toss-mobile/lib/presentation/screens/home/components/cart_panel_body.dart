import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../../app/themes/app_sizes.dart';
import '../../../../app/utilities/currency_formatter.dart';
import '../../../providers/home/home_provider.dart';
import '../../../widgets/app_empty_state.dart';
import 'order_card.dart';

class CartPanelBody extends StatefulWidget {
  const CartPanelBody({super.key});

  @override
  State<CartPanelBody> createState() => _CartPanelBodyState();
}

class _CartPanelBodyState extends State<CartPanelBody> {
  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      padding: const EdgeInsets.symmetric(vertical: 62),
      physics: const NeverScrollableScrollPhysics(),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          orderList(),
          orderTotal(),
        ],
      ),
    );
  }

  Widget orderList() {
    return Consumer<HomeProvider>(
      builder: (context, provider, _) {
        if (provider.orderedProducts.isEmpty) {
          return SizedBox(
            height: AppSizes.screenHeight(context) - 272,
            child: const AppEmptyState(
              title: 'Empty',
              subtitle: 'No products added to cart',
            ),
          );
        }

        return SizedBox(
          height: AppSizes.screenHeight(context) - 272,
          child: Scrollbar(
            child: ListView.builder(
              itemCount: provider.orderedProducts.length,
              padding: const EdgeInsets.all(AppSizes.padding),
              itemBuilder: (context, i) {
                return Padding(
                  padding: const EdgeInsets.only(bottom: AppSizes.padding),
                  child: OrderCard(
                    name: provider.orderedProducts[i].name,
                    imageUrl: provider.orderedProducts[i].imageUrl,
                    stock: provider.orderedProducts[i].stock,
                    price: provider.orderedProducts[i].price,
                    initialQuantity: provider.orderedProducts[i].quantity,
                    onChangedQuantity: (val) {
                      provider.onChangedOrderedProductQuantity(i, val);
                    },
                    onTapRemove: () {
                      provider.onRemoveOrderedProduct(provider.orderedProducts[i]);
                    },
                  ),
                );
              },
            ),
          ),
        );
      },
    );
  }

  Widget orderTotal() {
    return Consumer<HomeProvider>(
      builder: (context, provider, _) {
        return Container(
          padding: const EdgeInsets.all(AppSizes.padding),
          decoration: BoxDecoration(
            border: Border(
              top: BorderSide(
                width: 1,
                color: Theme.of(context).colorScheme.surfaceContainer,
              ),
            ),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              if (provider.getUpsellSuggestion() != null) ...[
                Container(
                  padding: const EdgeInsets.all(8),
                  margin: const EdgeInsets.only(bottom: 8),
                  decoration: BoxDecoration(
                    color: Theme.of(context).colorScheme.surface,
                    borderRadius: BorderRadius.circular(6),
                  ),
                  child: Row(
                    children: [
                      Icon(Icons.lightbulb_outline, size: 16, color: Theme.of(context).colorScheme.primary),
                      const SizedBox(width: 6),
                      Expanded(
                        child: Text(
                          provider.getUpsellSuggestion()!,
                          style: Theme.of(context).textTheme.labelMedium,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    'Subtotal (${provider.orderedProducts.length})',
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.bold),
                  ),
                  Text(
                    CurrencyFormatter.format(provider.getTotalAmount()),
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.bold),
                  ),
                ],
              ),
              const SizedBox(height: 6),
              if (provider.discountPercent != null || provider.discountAmount > 0) ...[
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text('Discount', style: Theme.of(context).textTheme.bodyMedium),
                    Text(
                      provider.discountPercent != null
                          ? '-${provider.discountPercent!.toStringAsFixed(0)}%'
                          : '-${CurrencyFormatter.format(provider.discountAmount)}',
                      style: Theme.of(context).textTheme.bodyMedium,
                    ),
                  ],
                ),
                const SizedBox(height: 6),
              ],
              if (provider.taxPercent != null && provider.taxPercent! > 0) ...[
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text('Tax (${provider.taxPercent!.toStringAsFixed(0)}%)', style: Theme.of(context).textTheme.bodyMedium),
                    Text(
                      '+${CurrencyFormatter.format(provider.getTaxAmount())}',
                      style: Theme.of(context).textTheme.bodyMedium,
                    ),
                  ],
                ),
                const SizedBox(height: 6),
              ],
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text('Payable', style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.bold)),
                  Text(
                    CurrencyFormatter.format(provider.getFinalTotalAmount()),
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.bold),
                  ),
                ],
              ),
            ],
          ),
        );
      },
    );
  }
}
