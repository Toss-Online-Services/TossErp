import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';
import 'package:mobile_scanner/mobile_scanner.dart';

import '../../../app/themes/app_sizes.dart';
import '../../../domain/entities/product_entity.dart';
import '../../../service_locator.dart';
import '../../providers/products/products_provider.dart';
import '../../widgets/app_button.dart';
import '../../widgets/app_empty_state.dart';
import '../../widgets/app_loading_more_indicator.dart';
import '../../widgets/app_progress_indicator.dart';
import '../../widgets/product_search_field.dart';
import '../../widgets/most_used_product_chips.dart';
import 'components/products_card.dart';

class ProductsScreen extends StatefulWidget {
  const ProductsScreen({super.key});

  @override
  State<ProductsScreen> createState() => _ProductsScreenState();
}

class _ProductsScreenState extends State<ProductsScreen> {
  final productProvider = sl<ProductsProvider>();

  final scrollController = ScrollController();
  final searchFieldController = TextEditingController();
  
  // Bulk selection state
  bool _isSelectionMode = false;
  final Set<int> _selectedProductIds = <int>{};

  @override
  void initState() {
    scrollController.addListener(scrollListener);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      productProvider.getAllProducts();
    });
    super.initState();
  }

  @override
  void dispose() {
    scrollController.removeListener(scrollListener);
    scrollController.dispose();
    searchFieldController.dispose();
    super.dispose();
  }

  void scrollListener() async {
    // Automatically load more data on end of scroll position
    if (scrollController.offset == scrollController.position.maxScrollExtent) {
      await productProvider.getAllProducts(
        offset: productProvider.allProducts?.length,
        contains: searchFieldController.text,
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: _isSelectionMode 
            ? Text('${_selectedProductIds.length} selected')
            : const Text('Items'),
        elevation: 0,
        shadowColor: Colors.transparent,
        leading: _isSelectionMode 
            ? IconButton(
                onPressed: _exitSelectionMode,
                icon: const Icon(Icons.close),
              )
            : null,
        actions: _isSelectionMode 
            ? [
                if (_selectedProductIds.isNotEmpty) ...[
                  IconButton(
                    onPressed: _showBulkActionsMenu,
                    icon: const Icon(Icons.more_vert),
                    tooltip: 'Bulk Actions',
                  ),
                ],
              ]
            : [
                IconButton(
                  onPressed: _startBarcodeScanning,
                  icon: const Icon(Icons.qr_code_scanner),
                  tooltip: 'Scan Barcode',
                ),
                IconButton(
                  onPressed: _enterSelectionMode,
                  icon: const Icon(Icons.checklist),
                  tooltip: 'Select Multiple',
                ),
                addButton(),
              ],
      ),
      body: Consumer<ProductsProvider>(
        builder: (context, provider, _) {
          final hasLow = provider.lowStockProducts.isNotEmpty;
          return Column(
            children: [
              if (hasLow)
                MaterialBanner(
                  content: Text('Low stock on ${provider.lowStockProducts.length} item(s).'),
                  leading: const Icon(Icons.warning_amber_rounded, color: Colors.orange),
                  actions: [
                    TextButton(
                      onPressed: () {
                        final names = provider.lowStockProducts.take(5).map((e) => e.name).join(', ');
                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(content: Text('Low stock: $names')),
                        );
                      },
                      child: const Text('VIEW'),
                    )
                  ],
                ),
              Expanded(
                child: RefreshIndicator(
                  onRefresh: () => productProvider.getAllProducts(),
                  displacement: 60,
                  child: Scrollbar(
                    child: CustomScrollView(
                      controller: scrollController,
                      physics: (provider.allProducts?.isEmpty ?? true) ? const NeverScrollableScrollPhysics() : null,
                      slivers: [
                        SliverAppBar(
                          floating: true,
                          snap: true,
                          automaticallyImplyLeading: false,
                          collapsedHeight: 70,
                          titleSpacing: 0,
                          title: Padding(
                            padding: const EdgeInsets.symmetric(horizontal: AppSizes.padding),
                            child: ProductSearchField(controller: searchFieldController),
                          ),
                        ),
                        SliverToBoxAdapter(
                          child: MostUsedProductChips(
                            onSelect: (p) {
                              searchFieldController.text = p.name;
                              productProvider.allProducts = null;
                              productProvider.getAllProducts(contains: p.name);
                            },
                          ),
                        ),
                        SliverLayoutBuilder(
                          builder: (context, constraint) {
                            if (provider.allProducts == null) {
                              return const SliverFillRemaining(
                                hasScrollBody: false,
                                fillOverscroll: true,
                                child: AppProgressIndicator(),
                              );
                            }

                            if (provider.allProducts!.isEmpty) {
                              return SliverFillRemaining(
                                hasScrollBody: false,
                                fillOverscroll: true,
                                child: AppEmptyState(
                                  subtitle: 'No products available, add product to continue',
                                  buttonText: 'Add Product',
                                  onTapButton: () => context.push('/products/product-create'),
                                ),
                              );
                            }

                            final width = MediaQuery.of(context).size.width;
                            final double extent = width >= 1280
                                ? 260
                                : width >= 1024
                                    ? 240
                                    : width >= 800
                                        ? 220
                                        : width >= 600
                                            ? 200
                                            : 170;

                            // Build most-used chips (by sold)
                            final topCount = width >= 1280
                                ? 18
                                : width >= 1024
                                    ? 14
                                    : width >= 800
                                        ? 12
                                        : width >= 600
                                            ? 10
                                            : 6;
                            final mostUsed = [...provider.allProducts!]
                              ..sort((a, b) => (b.sold ?? 0).compareTo(a.sold ?? 0));
                            final chips = mostUsed.take(topCount).toList();

                            return SliverList(
                              delegate: SliverChildListDelegate([
                                if (chips.isNotEmpty)
                                  Padding(
                                    padding: const EdgeInsets.fromLTRB(AppSizes.padding, 8, AppSizes.padding, 4),
                                    child: width < 600
                                        ? SingleChildScrollView(
                                            scrollDirection: Axis.horizontal,
                                            child: Row(
                                              children: [
                                                for (final p in chips)
                                                  Padding(
                                                    padding: const EdgeInsets.only(right: 8),
                                                    child: ActionChip(
                                                      label: Text(p.name, overflow: TextOverflow.ellipsis),
                                                      onPressed: () {
                                                        searchFieldController.text = p.name;
                                                        productProvider.allProducts = null;
                                                        productProvider.getAllProducts(contains: p.name);
                                                      },
                                                    ),
                                                  ),
                                              ],
                                            ),
                                          )
                                        : Wrap(
                                            spacing: 8,
                                            runSpacing: 8,
                                            children: [
                                              for (final p in chips)
                                                ActionChip(
                                                  label: Text(p.name, overflow: TextOverflow.ellipsis),
                                                  onPressed: () {
                                                    searchFieldController.text = p.name;
                                                    productProvider.allProducts = null;
                                                    productProvider.getAllProducts(contains: p.name);
                                                  },
                                                ),
                                            ],
                                          ),
                                  ),
                                Padding(
                                  padding: const EdgeInsets.fromLTRB(AppSizes.padding, 2, AppSizes.padding, AppSizes.padding),
                                  child: GridView.builder(
                                    shrinkWrap: true,
                                    physics: const NeverScrollableScrollPhysics(),
                                    gridDelegate: SliverGridDelegateWithMaxCrossAxisExtent(
                                      maxCrossAxisExtent: extent,
                                      childAspectRatio: 0.72,
                                      crossAxisSpacing: AppSizes.padding / 2,
                                      mainAxisSpacing: AppSizes.padding / 2,
                                    ),
                                    itemCount: provider.allProducts!.length,
                                    itemBuilder: (context, i) {
                                      return productCard(provider.allProducts![i]);
                                    },
                                  ),
                                ),
                              ]),
                            );
                          },
                        ),
                        SliverToBoxAdapter(
                          child: AppLoadingMoreIndicator(isLoading: provider.isLoadingMore),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            ],
          );
        },
      ),
    );
  }

  Widget addButton() {
    return Padding(
      padding: const EdgeInsets.only(right: AppSizes.padding),
      child: AppButton(
        height: 26,
        borderRadius: BorderRadius.circular(4),
        padding: const EdgeInsets.symmetric(horizontal: AppSizes.padding / 2),
        buttonColor: Theme.of(context).colorScheme.surfaceContainer,
        child: Row(
          children: [
            Icon(
              Icons.add,
              size: 12,
              color: Theme.of(context).colorScheme.primary,
            ),
            const SizedBox(width: AppSizes.padding / 4),
            Text(
              'Add Product',
              style: Theme.of(context).textTheme.labelSmall?.copyWith(
                fontSize: 10,
                fontWeight: FontWeight.bold,
                color: Theme.of(context).colorScheme.primary,
              ),
            ),
          ],
        ),
        onTap: () => context.go('/products/product-create'),
      ),
    );
  }

  // Search field now reused via ProductSearchField

  Widget productCard(ProductEntity product) {
    return ProductsCard(
      product: product,
      onTap: _isSelectionMode 
          ? () => _toggleProductSelection(product.id!)
          : () => context.go('/products/product-detail/${product.id}'),
      isSelected: _selectedProductIds.contains(product.id),
      isSelectionMode: _isSelectionMode,
    );
  }

  // Selection mode functionality
  void _enterSelectionMode() {
    setState(() {
      _isSelectionMode = true;
      _selectedProductIds.clear();
    });
  }

  void _exitSelectionMode() {
    setState(() {
      _isSelectionMode = false;
      _selectedProductIds.clear();
    });
  }

  void _toggleProductSelection(int productId) {
    setState(() {
      if (_selectedProductIds.contains(productId)) {
        _selectedProductIds.remove(productId);
      } else {
        _selectedProductIds.add(productId);
      }
    });
  }

  void _showBulkActionsMenu() {
    showModalBottomSheet(
      context: context,
      builder: (context) => Container(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Bulk Actions (${_selectedProductIds.length} items)',
              style: Theme.of(context).textTheme.titleLarge?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            ListTile(
              leading: const Icon(Icons.edit),
              title: const Text('Edit Prices'),
              onTap: () {
                Navigator.pop(context);
                _showBulkPriceEdit();
              },
            ),
            ListTile(
              leading: const Icon(Icons.inventory),
              title: const Text('Update Stock'),
              onTap: () {
                Navigator.pop(context);
                _showBulkStockUpdate();
              },
            ),
            ListTile(
              leading: const Icon(Icons.local_offer),
              title: const Text('Apply Discount'),
              onTap: () {
                Navigator.pop(context);
                _showBulkDiscountDialog();
              },
            ),
            ListTile(
              leading: const Icon(Icons.category),
              title: const Text('Change Category'),
              onTap: () {
                Navigator.pop(context);
                _showBulkCategoryChange();
              },
            ),
            ListTile(
              leading: const Icon(Icons.delete, color: Colors.red),
              title: const Text('Delete Products', style: TextStyle(color: Colors.red)),
              onTap: () {
                Navigator.pop(context);
                _showBulkDeleteConfirmation();
              },
            ),
          ],
        ),
      ),
    );
  }

  void _showBulkPriceEdit() {
    final priceController = TextEditingController();
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Bulk Price Update'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text('Update prices for ${_selectedProductIds.length} products'),
            const SizedBox(height: 16),
            TextField(
              controller: priceController,
              decoration: const InputDecoration(
                labelText: 'New Price (R)',
                border: OutlineInputBorder(),
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
              Navigator.pop(context);
              _applyBulkPriceUpdate(priceController.text);
            },
            child: const Text('Update'),
          ),
        ],
      ),
    );
  }

  void _showBulkStockUpdate() {
    final stockController = TextEditingController();
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Bulk Stock Update'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text('Update stock for ${_selectedProductIds.length} products'),
            const SizedBox(height: 16),
            TextField(
              controller: stockController,
              decoration: const InputDecoration(
                labelText: 'Stock Quantity',
                border: OutlineInputBorder(),
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
              Navigator.pop(context);
              _applyBulkStockUpdate(stockController.text);
            },
            child: const Text('Update'),
          ),
        ],
      ),
    );
  }

  void _showBulkDiscountDialog() {
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Bulk discount feature coming soon!')),
    );
  }

  void _showBulkCategoryChange() {
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Bulk category change coming soon!')),
    );
  }

  void _showBulkDeleteConfirmation() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Delete Products'),
        content: Text('Are you sure you want to delete ${_selectedProductIds.length} products? This action cannot be undone.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: Colors.red),
            onPressed: () {
              Navigator.pop(context);
              _applyBulkDelete();
            },
            child: const Text('Delete', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );
  }

  void _applyBulkPriceUpdate(String priceText) async {
    final price = double.tryParse(priceText);
    if (price == null || price <= 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please enter a valid price')),
      );
      return;
    }

    final priceInCents = (price * 100).round();
    
    try {
      await productProvider.bulkUpdatePricesByIds(_selectedProductIds, price);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Updated prices for ${_selectedProductIds.length} products to R${price.toStringAsFixed(2)}')),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error updating prices: $e')),
      );
    }
    
    _exitSelectionMode();
  }

  void _applyBulkStockUpdate(String stockText) async {
    final stock = int.tryParse(stockText);
    if (stock == null || stock < 0) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please enter a valid stock quantity')),
      );
      return;
    }
    
    try {
      await productProvider.bulkUpdateStockByIds(_selectedProductIds, stock);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Updated stock for ${_selectedProductIds.length} products to $stock units')),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error updating stock: $e')),
      );
    }
    
    _exitSelectionMode();
  }

  void _applyBulkDelete() async {
    try {
      await productProvider.bulkDeleteProductsByIds(_selectedProductIds);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Deleted ${_selectedProductIds.length} products')),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error deleting products: $e')),
      );
    }
    
    _exitSelectionMode();
  }

  // Barcode scanning functionality
  Future<void> _startBarcodeScanning() async {
    try {
      final result = await context.push('/barcode-scanner');
      if (result != null && result is String) {
        await _searchProductByBarcode(result);
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error starting barcode scanner: $e')),
        );
      }
    }
  }

  Future<void> _searchProductByBarcode(String barcode) async {
    if (!mounted) return;
    
    // Show loading indicator
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => const Center(
        child: CircularProgressIndicator(),
      ),
    );

    try {
      // Search for product by barcode
      searchFieldController.text = barcode;
      productProvider.allProducts = null;
      await productProvider.getAllProducts(contains: barcode);

      if (mounted) {
        Navigator.of(context).pop(); // Close loading dialog
        
        // Check if any products were found
        final foundProducts = productProvider.allProducts ?? [];
        if (foundProducts.isEmpty) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text('No product found with barcode: $barcode'),
              action: SnackBarAction(
                label: 'Add Product',
                onPressed: () => context.push('/products/product-create?barcode=$barcode'),
              ),
            ),
          );
        } else {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text('Found ${foundProducts.length} product(s) with barcode: $barcode'),
            ),
          );
        }
      }
    } catch (e) {
      if (mounted) {
        Navigator.of(context).pop(); // Close loading dialog
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error searching for product: $e')),
        );
      }
    }
  }
}
