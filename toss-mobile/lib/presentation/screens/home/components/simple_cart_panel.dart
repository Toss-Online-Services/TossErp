import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../../app/themes/app_sizes.dart';
import '../../../../app/utilities/currency_formatter.dart';
import '../../../providers/home/home_provider.dart';
import '../../../widgets/app_empty_state.dart';
import 'simple_cart_item.dart';
import 'payment_modal_sheet.dart';

class SimpleCartPanel extends StatelessWidget {
  const SimpleCartPanel({super.key});

  @override
  Widget build(BuildContext context) {
    return Consumer<HomeProvider>(
      builder: (context, provider, child) {
        if (provider.orderedProducts.isEmpty) {
          return Column(
            children: [
              // Top section with drag handle and arrow
              Stack(
                clipBehavior: Clip.none,
                children: [
                  // Reserve vertical space so the close button isn't clipped
                  const SizedBox(height: 56),
                  // Drag Handle - Centered at top
                  Container(
                    width: double.infinity,
                    padding: const EdgeInsets.symmetric(vertical: 8),
                    child: Center(
                      child: Container(
                        width: 40,
                        height: 4,
                        decoration: BoxDecoration(
                          color: Theme.of(context).colorScheme.outline.withOpacity(0.6),
                          borderRadius: BorderRadius.circular(2),
                        ),
                      ),
                    ),
                  ),
                  
                  // Down arrow positioned at top-right corner
                  Positioned(
                    top: 8,
                    right: 8,
                    child: Container(
                      decoration: BoxDecoration(
                        color: Theme.of(context).colorScheme.primaryContainer,
                        borderRadius: BorderRadius.circular(20),
                        boxShadow: [
                          BoxShadow(
                            color: Colors.black.withOpacity(0.2),
                            blurRadius: 8,
                            offset: const Offset(0, 3),
                          ),
                        ],
                        border: Border.all(
                          color: Theme.of(context).colorScheme.primary.withOpacity(0.3),
                          width: 2,
                        ),
                      ),
                      child: InkWell(
                        onTap: () => Navigator.of(context).pop(),
                        borderRadius: BorderRadius.circular(20),
                        child: Container(
                          width: 44,
                          height: 44,
                          padding: const EdgeInsets.all(8),
                          child: Icon(
                            Icons.keyboard_arrow_down,
                            size: 28,
                            color: Theme.of(context).colorScheme.onPrimaryContainer,
                          ),
                        ),
                      ),
                    ),
                  ),
                ],
              ),
              
              // Enhanced Header for empty cart too
              Stack(
                children: [
                  Container(
                    width: double.infinity,
                    padding: const EdgeInsets.fromLTRB(AppSizes.padding, 16, AppSizes.padding, 8),
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
                        Row(
                          children: [
                            Icon(
                              Icons.shopping_cart_outlined,
                              color: Theme.of(context).colorScheme.outline,
                              size: 24,
                            ),
                            const SizedBox(width: 8),
                            Text(
                              'Cart',
                              style: Theme.of(context).textTheme.titleLarge?.copyWith(
                                fontWeight: FontWeight.w600,
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                ],
              ),
              
              // Empty state content
              Expanded(
                child: Container(
                  padding: const EdgeInsets.all(AppSizes.padding),
                  child: const AppEmptyState(
                    title: 'Cart is Empty',
                    subtitle: 'Add some products to get started',
                  ),
                ),
              ),
            ],
          );
        }

        return Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Top section with drag handle and arrow
            Stack(
              clipBehavior: Clip.none,
              children: [
                // Reserve vertical space so the close button isn't clipped
                const SizedBox(height: 56),
                // Drag Handle - Centered at top
                Container(
                  width: double.infinity,
                  padding: const EdgeInsets.symmetric(vertical: 8),
                  child: Center(
                    child: Container(
                      width: 40,
                      height: 4,
                      decoration: BoxDecoration(
                        color: Theme.of(context).colorScheme.outline.withOpacity(0.6),
                        borderRadius: BorderRadius.circular(2),
                      ),
                    ),
                  ),
                ),
                
                // Down arrow positioned at top-right corner
                Positioned(
                  top: 8,
                  right: 8,
                  child: Container(
                    decoration: BoxDecoration(
                      color: Theme.of(context).colorScheme.primaryContainer,
                      borderRadius: BorderRadius.circular(20),
                      boxShadow: [
                        BoxShadow(
                          color: Colors.black.withOpacity(0.2),
                          blurRadius: 8,
                          offset: const Offset(0, 3),
                        ),
                      ],
                      border: Border.all(
                        color: Theme.of(context).colorScheme.primary.withOpacity(0.3),
                        width: 2,
                      ),
                    ),
                    child: InkWell(
                      onTap: () => Navigator.of(context).pop(),
                      borderRadius: BorderRadius.circular(20),
                      child: Container(
                        width: 44,
                        height: 44,
                        padding: const EdgeInsets.all(8),
                        child: Icon(
                          Icons.keyboard_arrow_down,
                          size: 28,
                          color: Theme.of(context).colorScheme.onPrimaryContainer,
                        ),
                      ),
                    ),
                  ),
                ),
              ],
            ),
            
            // Enhanced Header with close button and drag indicators
            Stack(
              children: [
                Container(
                  width: double.infinity,
                  padding: const EdgeInsets.fromLTRB(AppSizes.padding, 16, AppSizes.padding, 8),
                  decoration: BoxDecoration(
                    color: Theme.of(context).colorScheme.surface,
                    borderRadius: const BorderRadius.only(
                      topLeft: Radius.circular(AppSizes.radius * 2),
                      topRight: Radius.circular(AppSizes.radius * 2),
                    ),
                  ),
                  child: Column(
                    children: [
                      // Cart Header with clear button
                      Row(
                        children: [
                          Icon(
                            Icons.shopping_cart,
                            color: Theme.of(context).colorScheme.primary,
                            size: 24,
                          ),
                          const SizedBox(width: 8),
                          Builder(
                            builder: (context) {
                              // Use total quantity for clarity if available; fallback to unique items count
                              final totalQty = context.read<HomeProvider>().getTotalQuantity();
                              final n = totalQty > 0 ? totalQty : provider.orderedProducts.length;
                              final label = n == 1 ? 'item' : 'items';
                              return Text(
                                'Cart ($n $label)',
                                style: Theme.of(context).textTheme.titleLarge?.copyWith(
                                      fontWeight: FontWeight.w600,
                                    ),
                              );
                            },
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
                    ],
                  ),
                ),
              ],
            ),
            
            // Cart Items List
            Flexible(
              child: ListView.builder(
                shrinkWrap: true,
                padding: const EdgeInsets.symmetric(horizontal: AppSizes.padding / 2),
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
                  
                  const SizedBox(height: 16),
                  
                  // Checkout Button
                  SizedBox(
                    width: double.infinity,
                    height: 56,
                    child: ElevatedButton(
                      onPressed: () => _proceedToCheckout(context, provider),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Theme.of(context).colorScheme.primary,
                        foregroundColor: Theme.of(context).colorScheme.onPrimary,
                        elevation: 2,
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(AppSizes.radius),
                        ),
                      ),
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          const Icon(Icons.payment),
                          const SizedBox(width: 8),
                          Text(
                            'Proceed to Checkout',
                            style: Theme.of(context).textTheme.titleMedium?.copyWith(
                              color: Theme.of(context).colorScheme.onPrimary,
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                        ],
                      ),
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

  void _proceedToCheckout(BuildContext context, HomeProvider provider) {
    Navigator.of(context).pop(); // Close cart first
    _showPaymentModal(context, provider);
  }

  void _showPaymentModal(BuildContext context, HomeProvider provider) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      isDismissible: true,
      enableDrag: true,
      backgroundColor: Colors.transparent,
      builder: (context) => PaymentModalSheet(
        cartProvider: provider,
        totalAmount: provider.getFinalTotalAmount().toDouble(),
        items: provider.orderedProducts,
      ),
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
