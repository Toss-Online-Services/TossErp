import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../../app/themes/app_sizes.dart';
import '../../../../app/utilities/currency_formatter.dart';
import '../../../providers/home/home_provider.dart';
import '../../../widgets/app_empty_state.dart';
import 'simple_cart_item.dart';

class SimpleCartPanel extends StatelessWidget {
  const SimpleCartPanel({super.key});

  @override
  Widget build(BuildContext context) {
    return Consumer<HomeProvider>(
      builder: (context, provider, child) {
        if (provider.orderedProducts.isEmpty) {
          return const Padding(
            padding: EdgeInsets.all(AppSizes.padding),
            child: AppEmptyState(
              title: 'Cart is Empty',
              subtitle: 'Add some products to get started',
            ),
          );
        }

        return Column(
          children: [
            // Cart Header
            Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSizes.padding,
                vertical: AppSizes.padding / 2,
              ),
              decoration: BoxDecoration(
                color: Theme.of(context).colorScheme.surfaceVariant.withOpacity(0.3),
                borderRadius: const BorderRadius.only(
                  topLeft: Radius.circular(AppSizes.radius * 2),
                  topRight: Radius.circular(AppSizes.radius * 2),
                ),
              ),
              child: Row(
                children: [
                  Icon(
                    Icons.shopping_cart,
                    color: Theme.of(context).colorScheme.onSurface,
                    size: 20,
                  ),
                  const SizedBox(width: 8),
                  Text(
                    'Cart (${provider.orderedProducts.length} items)',
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  const Spacer(),
                  
                  // Clear Cart Button
                  TextButton.icon(
                    onPressed: provider.orderedProducts.isNotEmpty 
                      ? () => _showClearCartDialog(context, provider)
                      : null,
                    icon: const Icon(Icons.clear_all, size: 16),
                    label: const Text('Clear'),
                    style: TextButton.styleFrom(
                      foregroundColor: Colors.red,
                      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    ),
                  ),
                ],
              ),
            ),
            
            // Cart Items List
            Expanded(
              child: ListView.builder(
                padding: const EdgeInsets.only(
                  top: AppSizes.padding / 2,
                  bottom: AppSizes.padding,
                ),
                itemCount: provider.orderedProducts.length,
                itemBuilder: (context, index) {
                  return SimpleCartItem(
                    orderedProduct: provider.orderedProducts[index],
                    onQuantityChanged: (quantity) {
                      provider.onChangedOrderedProductQuantity(index, quantity);
                    },
                    onRemove: () {
                      provider.onRemoveOrderedProduct(provider.orderedProducts[index]);
                    },
                  );
                },
              ),
            ),
            
            // Cart Summary
            Container(
              padding: const EdgeInsets.all(AppSizes.padding),
              decoration: BoxDecoration(
                color: Theme.of(context).colorScheme.surface,
                border: Border(
                  top: BorderSide(
                    color: Theme.of(context).colorScheme.outline.withOpacity(0.2),
                  ),
                ),
              ),
              child: Column(
                children: [
                  // Subtotal
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        'Subtotal:',
                        style: Theme.of(context).textTheme.bodyLarge,
                      ),
                      Text(
                        CurrencyFormatter.format(provider.getTotalAmount()),
                        style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ],
                  ),
                  
                  const SizedBox(height: 8),
                  
                  // Tax (if applicable)
                  if (provider.getTaxAmount() > 0) ...[
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(
                          'Tax:',
                          style: Theme.of(context).textTheme.bodyMedium,
                        ),
                        Text(
                          CurrencyFormatter.format(provider.getTaxAmount()),
                          style: Theme.of(context).textTheme.bodyMedium,
                        ),
                      ],
                    ),
                    const SizedBox(height: 8),
                  ],
                  
                  // Total
                  Container(
                    padding: const EdgeInsets.symmetric(vertical: 8),
                    decoration: BoxDecoration(
                      border: Border(
                        top: BorderSide(
                          color: Theme.of(context).colorScheme.outline.withOpacity(0.2),
                        ),
                      ),
                    ),
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(
                          'Total:',
                          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        Text(
                          CurrencyFormatter.format(provider.getFinalTotalAmount()),
                          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                            fontWeight: FontWeight.bold,
                            color: Theme.of(context).colorScheme.primary,
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ],
        );
      },
    );
  }

  void _showClearCartDialog(BuildContext context, HomeProvider provider) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Clear Cart'),
        content: const Text('Are you sure you want to remove all items from your cart?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              provider.onClearCart();
              Navigator.of(context).pop();
            },
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: const Text('Clear'),
          ),
        ],
      ),
    );
  }
}
