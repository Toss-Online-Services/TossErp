import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/pos/pos_provider.dart';
import '../../providers/inventory/inventory_provider.dart';
import '../../../domain/entities/payment_entity.dart';
import 'components/pos_product_grid.dart';
import 'components/pos_cart_widget.dart';
import 'components/pos_customer_selector.dart';
import 'components/pos_payment_dialog.dart';
import 'components/pos_quick_actions.dart';
import 'components/pos_favorites_bar.dart';

class POSMainScreen extends StatefulWidget {
  const POSMainScreen({super.key});

  @override
  State<POSMainScreen> createState() => _POSMainScreenState();
}

class _POSMainScreenState extends State<POSMainScreen> {
  final TextEditingController _searchController = TextEditingController();
  final TextEditingController _barcodeController = TextEditingController();
  String _searchQuery = '';

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<InventoryProvider>().loadAllInventory();
      context.read<POSProvider>().loadTodaySales();
    });
  }

  @override
  void dispose() {
    _searchController.dispose();
    _barcodeController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final posProvider = context.watch<POSProvider>();
    final inventoryProvider = context.watch<InventoryProvider>();

    return Scaffold(
      appBar: _buildAppBar(context, posProvider),
      body: Row(
        children: [
          // Left side - Products
          Expanded(
            flex: 2,
            child: Column(
              children: [
                _buildSearchBar(),
                _buildCategoryTabs(),
                POSFavoritesBar(
                  onProductTap: (product) {
                    posProvider.addItem(product, 1);
                  },
                ),
                Expanded(
                  child: POSProductGrid(
                    searchQuery: _searchQuery,
                    onProductTap: (product) {
                      posProvider.addItem(product, 1);
                    },
                  ),
                ),
              ],
            ),
          ),
          // Right side - Cart and Checkout
          Expanded(
            flex: 1,
            child: Container(
              decoration: BoxDecoration(
                color: Colors.grey[100],
                border: Border(
                  left: BorderSide(color: Colors.grey[300]!),
                ),
              ),
              child: Column(
                children: [
                  _buildCartHeader(posProvider),
                  _buildCustomerSection(posProvider),
                  Expanded(
                    child: _buildCartSection(posProvider),
                  ),
                  _buildCartSummary(posProvider),
                  _buildCheckoutButtons(context, posProvider),
                ],
              ),
            ),
          ),
        ],
      ),
      floatingActionButton: _buildQuickActions(context, posProvider),
    );
  }

  PreferredSizeWidget _buildAppBar(BuildContext context, POSProvider provider) {
    return AppBar(
      title: const Text('Point of Sale'),
      actions: [
        // Today's sales summary
        Container(
          margin: const EdgeInsets.symmetric(horizontal: 16),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              Text(
                'Today: ${provider.todayTransactionCount} transactions',
                style: const TextStyle(fontSize: 12),
              ),
              Text(
                'R${(provider.todayRevenue / 100).toStringAsFixed(2)}',
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ],
          ),
        ),
        // Barcode scanner
        IconButton(
          icon: const Icon(Icons.qr_code_scanner),
          onPressed: () => _showBarcodeScanner(context),
          tooltip: 'Scan Barcode',
        ),
        // End shift / Reports
        IconButton(
          icon: const Icon(Icons.receipt_long),
          onPressed: () => _showEndOfDayReport(context),
          tooltip: 'End of Day Report',
        ),
      ],
    );
  }

  Widget _buildSearchBar() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: TextField(
        controller: _searchController,
        onChanged: (value) {
          setState(() {
            _searchQuery = value;
          });
        },
        decoration: InputDecoration(
          hintText: 'Search products...',
          prefixIcon: const Icon(Icons.search),
          suffixIcon: _searchQuery.isNotEmpty
              ? IconButton(
                  icon: const Icon(Icons.clear),
                  onPressed: () {
                    _searchController.clear();
                    setState(() {
                      _searchQuery = '';
                    });
                  },
                )
              : null,
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(12),
          ),
          filled: true,
          fillColor: Colors.white,
        ),
      ),
    );
  }

  Widget _buildCategoryTabs() {
    return Container(
      height: 50,
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: ListView(
        scrollDirection: Axis.horizontal,
        children: [
          _buildCategoryChip('All', true),
          _buildCategoryChip('Food', false),
          _buildCategoryChip('Beverages', false),
          _buildCategoryChip('Snacks', false),
          _buildCategoryChip('Electronics', false),
          _buildCategoryChip('Household', false),
        ],
      ),
    );
  }

  Widget _buildCategoryChip(String label, bool isSelected) {
    return Padding(
      padding: const EdgeInsets.only(right: 8),
      child: FilterChip(
        label: Text(label),
        selected: isSelected,
        onSelected: (selected) {
          // TODO: Implement category filtering
        },
        backgroundColor: Colors.white,
        selectedColor: Theme.of(context).primaryColor.withOpacity(0.2),
      ),
    );
  }

  Widget _buildCartHeader(POSProvider provider) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).primaryColor,
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.1),
            blurRadius: 4,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          const Text(
            'Current Sale',
            style: TextStyle(
              color: Colors.white,
              fontSize: 18,
              fontWeight: FontWeight.bold,
            ),
          ),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(20),
            ),
            child: Text(
              '${provider.itemCount} items',
              style: TextStyle(
                color: Theme.of(context).primaryColor,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCustomerSection(POSProvider provider) {
    return Container(
      margin: const EdgeInsets.all(16),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Colors.grey[300]!),
      ),
      child: Row(
        children: [
          const Icon(Icons.person_outline, color: Colors.grey),
          const SizedBox(width: 12),
          Expanded(
            child: provider.selectedCustomerName != null
                ? Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        provider.selectedCustomerName!,
                        style: const TextStyle(fontWeight: FontWeight.bold),
                      ),
                      if (provider.selectedCustomerPhone != null)
                        Text(
                          provider.selectedCustomerPhone!,
                          style: TextStyle(
                            fontSize: 12,
                            color: Colors.grey[600],
                          ),
                        ),
                    ],
                  )
                : const Text(
                    'Walk-in Customer',
                    style: TextStyle(color: Colors.grey),
                  ),
          ),
          TextButton(
            onPressed: () => _showCustomerSelector(context, provider),
            child: Text(
              provider.selectedCustomerName != null ? 'Change' : 'Select',
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCartSection(POSProvider provider) {
    if (provider.cartItems.isEmpty) {
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
              style: TextStyle(fontSize: 18, color: Colors.grey),
            ),
            Text(
              'Add products to start',
              style: TextStyle(color: Colors.grey),
            ),
          ],
        ),
      );
    }

    return ListView.builder(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      itemCount: provider.cartItems.length,
      itemBuilder: (context, index) {
        final item = provider.cartItems[index];
        return _buildCartItem(provider, item, index);
      },
    );
  }

  Widget _buildCartItem(POSProvider provider, item, int index) {
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
                        item.productName,
                        style: const TextStyle(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      if (item.productSku != null)
                        Text(
                          'SKU: ${item.productSku}',
                          style: TextStyle(
                            fontSize: 12,
                            color: Colors.grey[600],
                          ),
                        ),
                    ],
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.delete_outline, color: Colors.red),
                  onPressed: () => provider.removeItem(index),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                // Quantity controls
                Container(
                  decoration: BoxDecoration(
                    border: Border.all(color: Colors.grey[300]!),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Row(
                    children: [
                      IconButton(
                        icon: const Icon(Icons.remove, size: 18),
                        onPressed: () {
                          provider.updateItemQuantity(
                            index,
                            item.quantity - 1,
                          );
                        },
                        padding: const EdgeInsets.all(4),
                        constraints: const BoxConstraints(),
                      ),
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 12),
                        child: Text(
                          '${item.quantity}',
                          style: const TextStyle(fontWeight: FontWeight.bold),
                        ),
                      ),
                      IconButton(
                        icon: const Icon(Icons.add, size: 18),
                        onPressed: () {
                          provider.updateItemQuantity(
                            index,
                            item.quantity + 1,
                          );
                        },
                        padding: const EdgeInsets.all(4),
                        constraints: const BoxConstraints(),
                      ),
                    ],
                  ),
                ),
                const SizedBox(width: 16),
                // Price
                Text(
                  'R${(item.unitPrice / 100).toStringAsFixed(2)}',
                  style: TextStyle(color: Colors.grey[600]),
                ),
                const Spacer(),
                // Line total
                Text(
                  'R${(item.lineTotal / 100).toStringAsFixed(2)}',
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 16,
                  ),
                ),
              ],
            ),
            // Discount if any
            if (item.discount > 0)
              Padding(
                padding: const EdgeInsets.only(top: 8),
                child: Text(
                  'Discount: -R${(item.discount / 100).toStringAsFixed(2)}',
                  style: const TextStyle(
                    color: Colors.green,
                    fontSize: 12,
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildCartSummary(POSProvider provider) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.05),
            blurRadius: 4,
            offset: const Offset(0, -2),
          ),
        ],
      ),
      child: Column(
        children: [
          _buildSummaryRow(
            'Subtotal',
            'R${(provider.cartSubtotal / 100).toStringAsFixed(2)}',
          ),
          if (provider.cartDiscount > 0)
            _buildSummaryRow(
              'Discount',
              '-R${(provider.cartDiscount / 100).toStringAsFixed(2)}',
              color: Colors.green,
            ),
          _buildSummaryRow(
            'Tax (${(provider.taxRate * 100).toStringAsFixed(0)}%)',
            'R${(provider.cartTaxAmount / 100).toStringAsFixed(2)}',
          ),
          const Divider(height: 24),
          _buildSummaryRow(
            'Total',
            'R${(provider.cartTotal / 100).toStringAsFixed(2)}',
            isBold: true,
            fontSize: 20,
          ),
          if (provider.payments.isNotEmpty) ...[
            const SizedBox(height: 8),
            _buildSummaryRow(
              'Paid',
              'R${(provider.paymentTotal / 100).toStringAsFixed(2)}',
              color: Colors.blue,
            ),
            _buildSummaryRow(
              'Remaining',
              'R${(provider.remainingAmount / 100).toStringAsFixed(2)}',
              color: provider.isFullyPaid ? Colors.green : Colors.red,
              isBold: true,
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildSummaryRow(
    String label,
    String value, {
    Color? color,
    bool isBold = false,
    double? fontSize,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(
            label,
            style: TextStyle(
              fontWeight: isBold ? FontWeight.bold : FontWeight.normal,
              fontSize: fontSize,
              color: color,
            ),
          ),
          Text(
            value,
            style: TextStyle(
              fontWeight: isBold ? FontWeight.bold : FontWeight.normal,
              fontSize: fontSize,
              color: color,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCheckoutButtons(BuildContext context, POSProvider provider) {
    final canCheckout = provider.cartItems.isNotEmpty;

    return Container(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          SizedBox(
            width: double.infinity,
            height: 56,
            child: ElevatedButton.icon(
              onPressed: canCheckout
                  ? () => _showPaymentDialog(context, provider)
                  : null,
              icon: const Icon(Icons.payment),
              label: const Text(
                'Payment',
                style: TextStyle(fontSize: 18),
              ),
              style: ElevatedButton.styleFrom(
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
              ),
            ),
          ),
          const SizedBox(height: 8),
          Row(
            children: [
              Expanded(
                child: OutlinedButton(
                  onPressed: canCheckout
                      ? () => _showDiscountDialog(context, provider)
                      : null,
                  child: const Text('Discount'),
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: OutlinedButton(
                  onPressed: canCheckout
                      ? () {
                          showDialog(
                            context: context,
                            builder: (context) => AlertDialog(
                              title: const Text('Clear Cart'),
                              content: const Text(
                                'Are you sure you want to clear the cart?',
                              ),
                              actions: [
                                TextButton(
                                  onPressed: () => Navigator.pop(context),
                                  child: const Text('Cancel'),
                                ),
                                ElevatedButton(
                                  onPressed: () {
                                    provider.clearCart();
                                    Navigator.pop(context);
                                  },
                                  style: ElevatedButton.styleFrom(
                                    backgroundColor: Colors.red,
                                  ),
                                  child: const Text('Clear'),
                                ),
                              ],
                            ),
                          );
                        }
                      : null,
                  child: const Text('Clear'),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildQuickActions(BuildContext context, POSProvider provider) {
    return POSQuickActions(
      onHoldSale: () {
        // TODO: Implement hold sale
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Sale held')),
        );
      },
      onRecallSale: () {
        // TODO: Implement recall sale
      },
      onParkSale: () {
        // TODO: Implement park sale
      },
    );
  }

  void _showBarcodeScanner(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Scan Barcode'),
        content: TextField(
          controller: _barcodeController,
          decoration: const InputDecoration(
            hintText: 'Enter or scan barcode',
            prefixIcon: Icon(Icons.qr_code_scanner),
          ),
          autofocus: true,
          onSubmitted: (value) {
            // TODO: Search product by barcode and add to cart
            Navigator.pop(context);
            _barcodeController.clear();
          },
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
        ],
      ),
    );
  }

  void _showCustomerSelector(BuildContext context, POSProvider provider) {
    // TODO: Implement POSCustomerSelector integration
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Select Customer'),
        content: const Text('Customer selector coming soon...'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
        ],
      ),
    );
  }

  void _showPaymentDialog(BuildContext context, POSProvider provider) async {
    final result = await showDialog(
      context: context,
      builder: (context) => const POSPaymentDialog(),
    );

    if (result == true && context.mounted) {
      // Process transaction
      final success = await provider.processTransaction(
        cashierId: 1, // TODO: Get from auth
        cashierName: 'Cashier', // TODO: Get from auth
        warehouseId: 1, // TODO: Get from settings
        warehouseName: 'Main Store', // TODO: Get from settings
      );

      if (success && context.mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Transaction completed successfully!'),
            backgroundColor: Colors.green,
          ),
        );
        // Show receipt options
        _showReceiptOptions(context, provider);
      } else if (provider.error != null && context.mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Error: ${provider.error}'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  void _showReceiptOptions(BuildContext context, POSProvider provider) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Transaction Complete'),
        content: const Text('Would you like to print or email the receipt?'),
        actions: [
          TextButton(
            onPressed: () {
              Navigator.pop(context);
              provider.clearCart();
            },
            child: const Text('Skip'),
          ),
          TextButton(
            onPressed: () async {
              await provider.emailReceiptToCustomer();
              if (context.mounted) Navigator.pop(context);
              provider.clearCart();
            },
            child: const Text('Email'),
          ),
          ElevatedButton(
            onPressed: () async {
              await provider.generateReceiptPdf();
              if (context.mounted) Navigator.pop(context);
              provider.clearCart();
            },
            child: const Text('Print'),
          ),
        ],
      ),
    );
  }

  void _showDiscountDialog(BuildContext context, POSProvider provider) {
    final percentController = TextEditingController();
    final amountController = TextEditingController();

    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Apply Discount'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(
              controller: percentController,
              decoration: const InputDecoration(
                labelText: 'Discount Percentage (%)',
                suffixIcon: Icon(Icons.percent),
              ),
              keyboardType: TextInputType.number,
            ),
            const SizedBox(height: 16),
            const Text('OR'),
            const SizedBox(height: 16),
            TextField(
              controller: amountController,
              decoration: const InputDecoration(
                labelText: 'Discount Amount (R)',
                prefixText: 'R ',
              ),
              keyboardType: TextInputType.number,
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () {
              if (percentController.text.isNotEmpty) {
                final percent = double.tryParse(percentController.text) ?? 0;
                provider.setCartDiscountPercentage(percent);
              } else if (amountController.text.isNotEmpty) {
                final amount = double.tryParse(amountController.text) ?? 0;
                provider.setCartDiscount((amount * 100).round());
              }
              Navigator.pop(context);
            },
            child: const Text('Apply'),
          ),
        ],
      ),
    );
  }

  void _showEndOfDayReport(BuildContext context) {
    // TODO: Implement end of day report
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('End of Day Report'),
        content: const Text('Report generation coming soon...'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Close'),
          ),
        ],
      ),
    );
  }
}

