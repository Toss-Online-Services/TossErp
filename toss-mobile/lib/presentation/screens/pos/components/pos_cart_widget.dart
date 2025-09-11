import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../domain/entities/sales_transaction_entity.dart';
import '../../../../domain/entities/customer_entity.dart';

class POSCartWidget extends StatelessWidget {
  final List<SalesTransactionItemEntity> cartItems;
  final CustomerEntity? selectedCustomer;
  final Function(int index, int quantity) onItemQuantityChanged;
  final Function(int index) onItemRemoved;
  final Function(int index, int discountAmount) onItemDiscountChanged;

  const POSCartWidget({
    super.key,
    required this.cartItems,
    this.selectedCustomer,
    required this.onItemQuantityChanged,
    required this.onItemRemoved,
    required this.onItemDiscountChanged,
  });

  @override
  Widget build(BuildContext context) {
    if (cartItems.isEmpty) {
      return const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.shopping_cart_outlined,
              size: 80,
              color: Colors.grey,
            ),
            SizedBox(height: 16),
            Text(
              'Cart is empty',
              style: TextStyle(
                fontSize: 18,
                color: Colors.grey,
              ),
            ),
            SizedBox(height: 8),
            Text(
              'Add products to start a transaction',
              style: TextStyle(
                color: Colors.grey,
              ),
            ),
          ],
        ),
      );
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Container(
          padding: const EdgeInsets.all(16),
          child: Row(
            children: [
              Icon(
                Icons.shopping_cart,
                color: Theme.of(context).colorScheme.primary,
              ),
              const SizedBox(width: 8),
              Text(
                'Cart (${cartItems.length} items)',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              const Spacer(),
              if (cartItems.isNotEmpty)
                TextButton.icon(
                  onPressed: () => _showBulkActions(context),
                  icon: const Icon(Icons.more_vert, size: 18),
                  label: const Text('Actions'),
                  style: TextButton.styleFrom(
                    padding: const EdgeInsets.symmetric(horizontal: 8),
                  ),
                ),
            ],
          ),
        ),
        Expanded(
          child: ListView.builder(
            padding: const EdgeInsets.symmetric(horizontal: 16),
            itemCount: cartItems.length,
            itemBuilder: (context, index) {
              return _buildCartItem(context, index, cartItems[index]);
            },
          ),
        ),
      ],
    );
  }

  Widget _buildCartItem(BuildContext context, int index, SalesTransactionItemEntity item) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        item.displayName,
                        style: Theme.of(context).textTheme.titleSmall?.copyWith(
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        '\$${(item.unitPrice / 100).toStringAsFixed(2)} each',
                        style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: Colors.grey[600],
                        ),
                      ),
                    ],
                  ),
                ),
                // Quantity controls
                Container(
                  decoration: BoxDecoration(
                    border: Border.all(color: Colors.grey[300]!),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      IconButton(
                        onPressed: () {
                          if (item.quantity > 1) {
                            onItemQuantityChanged(index, item.quantity - 1);
                          } else {
                            onItemRemoved(index);
                          }
                        },
                        icon: Icon(
                          item.quantity == 1 ? Icons.delete : Icons.remove,
                          size: 18,
                        ),
                        visualDensity: VisualDensity.compact,
                      ),
                      Container(
                        width: 40,
                        alignment: Alignment.center,
                        child: Text(
                          '${item.quantity}',
                          style: Theme.of(context).textTheme.titleSmall?.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                      IconButton(
                        onPressed: () => onItemQuantityChanged(index, item.quantity + 1),
                        icon: const Icon(Icons.add, size: 18),
                        visualDensity: VisualDensity.compact,
                      ),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                // Discount button
                OutlinedButton.icon(
                  onPressed: () => _showDiscountDialog(context, index, item),
                  icon: const Icon(Icons.local_offer, size: 16),
                  label: Text(
                    item.discountAmount > 0 
                        ? '-\$${(item.discountAmount / 100).toStringAsFixed(2)}'
                        : 'Discount',
                  ),
                  style: OutlinedButton.styleFrom(
                    padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 4),
                    textStyle: const TextStyle(fontSize: 12),
                    foregroundColor: item.discountAmount > 0 ? Colors.red : null,
                  ),
                ),
                const Spacer(),
                // Total price
                Text(
                  '\$${(item.netPrice / 100).toStringAsFixed(2)}',
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.bold,
                    color: Theme.of(context).colorScheme.primary,
                  ),
                ),
              ],
            ),
            if (item.discountAmount > 0) ...[
              const SizedBox(height: 4),
              Row(
                children: [
                  Text(
                    'Original: \$${(item.totalPrice / 100).toStringAsFixed(2)}',
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      decoration: TextDecoration.lineThrough,
                      color: Colors.grey,
                    ),
                  ),
                  const SizedBox(width: 8),
                  Text(
                    'Saved: \$${(item.discountAmount / 100).toStringAsFixed(2)}',
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      color: Colors.green,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }

  void _showDiscountDialog(BuildContext context, int index, SalesTransactionItemEntity item) {
    final discountController = TextEditingController(
      text: item.discountAmount > 0 ? (item.discountAmount / 100).toStringAsFixed(2) : '',
    );
    
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text('Discount for ${item.productName}'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text('Original Price: \$${(item.totalPrice / 100).toStringAsFixed(2)}'),
            const SizedBox(height: 16),
            TextField(
              controller: discountController,
              keyboardType: TextInputType.number,
              inputFormatters: [
                FilteringTextInputFormatter.allow(RegExp(r'^\d+\.?\d{0,2}')),
              ],
              decoration: const InputDecoration(
                labelText: 'Discount Amount',
                prefixText: '\$',
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 16),
            // Quick discount buttons
            Wrap(
              spacing: 8,
              children: [
                _buildQuickDiscountButton(context, '5%', (item.totalPrice * 0.05).round()),
                _buildQuickDiscountButton(context, '10%', (item.totalPrice * 0.10).round()),
                _buildQuickDiscountButton(context, '15%', (item.totalPrice * 0.15).round()),
                _buildQuickDiscountButton(context, '20%', (item.totalPrice * 0.20).round()),
              ],
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              // Remove discount
              onItemDiscountChanged(index, 0);
              Navigator.of(context).pop();
            },
            child: const Text('Remove'),
          ),
          ElevatedButton(
            onPressed: () {
              final discountAmount = (double.tryParse(discountController.text) ?? 0) * 100;
              final maxDiscount = item.totalPrice;
              
              if (discountAmount > maxDiscount) {
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(
                    content: Text('Discount cannot be more than the item total'),
                  ),
                );
                return;
              }
              
              onItemDiscountChanged(index, discountAmount.round());
              Navigator.of(context).pop();
            },
            child: const Text('Apply'),
          ),
        ],
      ),
    );
  }

  Widget _buildQuickDiscountButton(BuildContext context, String label, int discountAmount) {
    return OutlinedButton(
      onPressed: () {
        Navigator.of(context).pop();
        final index = cartItems.indexWhere((item) => item.productId == cartItems[0].productId);
        if (index != -1) {
          onItemDiscountChanged(index, discountAmount);
        }
      },
      style: OutlinedButton.styleFrom(
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 4),
        textStyle: const TextStyle(fontSize: 12),
      ),
      child: Text(label),
    );
  }

  void _showBulkActions(BuildContext context) {
    showModalBottomSheet(
      context: context,
      builder: (context) => Container(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Bulk Actions',
              style: Theme.of(context).textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            ListTile(
              leading: const Icon(Icons.local_offer),
              title: const Text('Apply Discount to All'),
              onTap: () {
                Navigator.of(context).pop();
                _showBulkDiscountDialog(context);
              },
            ),
            ListTile(
              leading: const Icon(Icons.clear_all),
              title: const Text('Clear All Items'),
              onTap: () {
                Navigator.of(context).pop();
                _confirmClearAll(context);
              },
            ),
            ListTile(
              leading: const Icon(Icons.copy),
              title: const Text('Duplicate Cart'),
              onTap: () {
                Navigator.of(context).pop();
                _duplicateCart();
              },
            ),
          ],
        ),
      ),
    );
  }

  void _showBulkDiscountDialog(BuildContext context) {
    final discountController = TextEditingController();
    
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Apply Discount to All Items'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(
              controller: discountController,
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(
                labelText: 'Discount Percentage',
                suffixText: '%',
                border: OutlineInputBorder(),
              ),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () {
              final discountPercent = double.tryParse(discountController.text) ?? 0;
              if (discountPercent > 0 && discountPercent <= 100) {
                for (int i = 0; i < cartItems.length; i++) {
                  final discountAmount = (cartItems[i].totalPrice * discountPercent / 100).round();
                  onItemDiscountChanged(i, discountAmount);
                }
              }
              Navigator.of(context).pop();
            },
            child: const Text('Apply'),
          ),
        ],
      ),
    );
  }

  void _confirmClearAll(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Clear All Items'),
        content: const Text('Are you sure you want to remove all items from the cart?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () {
              Navigator.of(context).pop();
              // Clear all items by removing them one by one (in reverse order)
              for (int i = cartItems.length - 1; i >= 0; i--) {
                onItemRemoved(i);
              }
            },
            style: ElevatedButton.styleFrom(backgroundColor: Colors.red),
            child: const Text('Clear All'),
          ),
        ],
      ),
    );
  }

  void _duplicateCart() {
    // TODO: Implement cart duplication
  }
}
