import '../../widgets/app_image.dart';
// import '../../widgets/customizable_dashboard.dart'; // Temporarily disabled
import 'dart:async';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../app/const/const.dart';
import '../../../app/themes/app_sizes.dart';
import '../../../app/utilities/currency_formatter.dart';
import '../../../domain/entities/product_entity.dart';
import '../../../service_locator.dart';
import '../../providers/home/home_provider.dart';
import '../../providers/main/main_provider.dart';
import '../../providers/products/products_provider.dart';
import '../../widgets/app_button.dart';
import '../../widgets/app_empty_state.dart';
import '../../widgets/app_loading_more_indicator.dart';
import '../../widgets/app_progress_indicator.dart';
import '../../widgets/product_search_field.dart';
import '../../widgets/most_used_product_chips.dart';
import '../products/components/products_card.dart';
import 'components/simple_cart_panel.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  final mainProvider = sl<MainProvider>();
  final homeProvider = sl<HomeProvider>();
  final productProvider = sl<ProductsProvider>();

  final scrollController = ScrollController();

  final searchFieldController = TextEditingController();
  Timer? _searchDebounce;

  @override
  void initState() {
    scrollController.addListener(scrollListener);
    WidgetsBinding.instance.addPostFrameCallback((_) => onRefresh());
    super.initState();
  }

  @override
  void dispose() {
    scrollController.removeListener(scrollListener);
    scrollController.dispose();
    searchFieldController.dispose();
    _searchDebounce?.cancel();
    super.dispose();
  }

  void scrollListener() async {
    // Automatically load more data on end of scroll position
    if (scrollController.offset == scrollController.position.maxScrollExtent) {
      await productProvider.getAllProducts(offset: productProvider.allProducts?.length);
    }
  }

  Future<void> onRefresh() async {
    await productProvider.getAllProducts();
    await mainProvider.checkIsHasQueuedActions();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          // Main body
          body(),
          
          // Simple cart at bottom
          Positioned(
            bottom: 0,
            left: 0,
            right: 0,
            child: Consumer<HomeProvider>(
              builder: (context, provider, child) {
                if (provider.orderedProducts.isEmpty) {
                  return const SizedBox.shrink();
                }
                
                return GestureDetector(
                  onTap: () {
                    // Show full cart in bottom sheet
                    showModalBottomSheet(
                      context: context,
                      isScrollControlled: true,
                      isDismissible: true,
                      enableDrag: true,
                      backgroundColor: Colors.transparent,
                      builder: (context) => DraggableScrollableSheet(
                        initialChildSize: 0.7,
                        minChildSize: 0.3,
                        maxChildSize: 0.9,
                        snap: true,
                        snapSizes: const [0.3, 0.7, 0.9],
                        builder: (context, scrollController) => Container(
                          decoration: BoxDecoration(
                            color: Theme.of(context).colorScheme.surfaceContainerLowest,
                            borderRadius: const BorderRadius.only(
                              topLeft: Radius.circular(AppSizes.radius * 2),
                              topRight: Radius.circular(AppSizes.radius * 2),
                            ),
                            boxShadow: [
                              BoxShadow(
                                color: Colors.black.withOpacity(0.1),
                                blurRadius: 10,
                                offset: const Offset(0, -2),
                              ),
                            ],
                          ),
                          child: const SimpleCartPanel(),
                        ),
                      ),
                    );
                  },
                  child: Container(
                    height: 88,
                    padding: const EdgeInsets.all(AppSizes.padding),
                    decoration: BoxDecoration(
                      color: Theme.of(context).colorScheme.surfaceContainerLowest,
                      borderRadius: const BorderRadius.only(
                        topLeft: Radius.circular(AppSizes.radius * 2),
                        topRight: Radius.circular(AppSizes.radius * 2),
                      ),
                      boxShadow: [
                        BoxShadow(
                          color: Theme.of(context).colorScheme.shadow.withValues(alpha: 0.1),
                          offset: const Offset(0, -2),
                          blurRadius: 8,
                        ),
                      ],
                    ),
                    child: Row(
                      children: [
                        // Cart Icon with Badge
                        Stack(
                          children: [
                            Icon(
                              Icons.shopping_cart,
                              color: Theme.of(context).colorScheme.primary,
                              size: 28,
                            ),
                            Positioned(
                              right: 0,
                              top: 0,
                              child: Container(
                                padding: const EdgeInsets.all(2),
                                decoration: BoxDecoration(
                                  color: Theme.of(context).colorScheme.error,
                                  borderRadius: BorderRadius.circular(10),
                                ),
                                constraints: const BoxConstraints(
                                  minWidth: 16,
                                  minHeight: 16,
                                ),
                                child: Text(
                                  provider.orderedProducts.length.toString(),
                                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                    color: Theme.of(context).colorScheme.onError,
                                    fontSize: 10,
                                    fontWeight: FontWeight.bold,
                                  ),
                                  textAlign: TextAlign.center,
                                ),
                              ),
                            ),
                          ],
                        ),
                        
                        const SizedBox(width: 12),
                        
                        // Cart Summary
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              Text(
                                '${provider.orderedProducts.length} item${provider.orderedProducts.length != 1 ? 's' : ''}',
                                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                              const SizedBox(height: 2),
                              Text(
                                CurrencyFormatter.format(provider.getTotalAmount()),
                                style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                                  color: Theme.of(context).colorScheme.primary,
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                            ],
                          ),
                        ),
                        
                        // Arrow indicator
                        Icon(
                          Icons.keyboard_arrow_up,
                          color: Theme.of(context).colorScheme.onSurface.withOpacity(0.7),
                        ),
                      ],
                    ),
                  ),
                );
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget title() {
    return Consumer<MainProvider>(
      builder: (context, provider, _) {
        return Row(
          children: [
            AppImage(
              image: provider.user?.imageUrl ?? '',
              borderRadius: BorderRadius.circular(100),
              width: 30,
              height: 30,
              backgroundColor: Theme.of(context).colorScheme.surface,
              errorWidget: Icon(
                Icons.person,
                color: Theme.of(context).colorScheme.surfaceContainerHighest,
              ),
            ),
            const SizedBox(width: 6),
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  provider.user?.name ?? '',
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    fontWeight: FontWeight.bold,
                    height: 0,
                  ),
                ),
                Text(
                  provider.user?.email ?? '',
                  style: Theme.of(context).textTheme.labelSmall?.copyWith(
                    fontSize: 10,
                    color: Theme.of(context).colorScheme.outline,
                  ),
                ),
              ],
            ),
          ],
        );
      },
    );
  }

  Widget syncButton() {
    return Consumer<MainProvider>(
      builder: (context, provider, _) {
        return Padding(
          padding: const EdgeInsets.only(right: AppSizes.padding / 4),
          child: AppButton(
            height: 26,
            borderRadius: BorderRadius.circular(4),
            padding: const EdgeInsets.symmetric(horizontal: AppSizes.padding / 2),
            buttonColor: provider.isHasQueuedActions && !provider.isSyncronizing
                ? Theme.of(context).colorScheme.surfaceContainer
                : Theme.of(context).colorScheme.shadow.withValues(alpha: 0.06),
            child: Row(
              children: [
                Icon(
                  provider.isSyncronizing
                      ? Icons.sync
                      : provider.isHasQueuedActions
                      ? Icons.cloud_done_sharp
                      : Icons.sync_problem_sharp,
                  size: 12,
                  color: provider.isHasQueuedActions && !provider.isSyncronizing
                      ? Theme.of(context).colorScheme.primary
                      : Theme.of(context).colorScheme.outline,
                ),
                const SizedBox(width: AppSizes.padding / 4),
                Text(
                  provider.isSyncronizing
                      ? 'Syncronizing'
                      : provider.isHasQueuedActions
                      ? 'Synced'
                      : 'Pending',
                  style: Theme.of(context).textTheme.labelSmall?.copyWith(
                    fontSize: 10,
                    fontWeight: FontWeight.bold,
                    color: provider.isHasQueuedActions && !provider.isSyncronizing
                        ? Theme.of(context).colorScheme.primary
                        : Theme.of(context).colorScheme.outline,
                  ),
                ),
              ],
            ),
            onTap: () {
              provider.checkAndSyncAllData(context);
            },
          ),
        );
      },
    );
  }

  Widget networkInfo() {
    return Selector<MainProvider, bool>(
      selector: (a, b) => b.isHasInternet,
      builder: (context, isHasInternet, _) {
        return Padding(
          padding: const EdgeInsets.only(right: AppSizes.padding),
          child: AppButton(
            height: 26,
            borderRadius: BorderRadius.circular(4),
            padding: const EdgeInsets.symmetric(horizontal: AppSizes.padding / 2),
            buttonColor: isHasInternet
                ? Theme.of(context).colorScheme.surfaceContainer
                : Theme.of(context).colorScheme.shadow.withValues(alpha: 0.06),
            child: Icon(
              isHasInternet ? Icons.wifi_rounded : Icons.wifi_off_rounded,
              size: 12,
              color: isHasInternet ? Theme.of(context).colorScheme.primary : Theme.of(context).colorScheme.outline,
            ),
            onTap: () {
              ScaffoldMessenger.of(context).showSnackBar(
                SnackBar(content: Text(isHasInternet ? ONLINE_MESSAGE : OFFLINE_MESSAGE)),
              );
            },
          ),
        );
      },
    );
  }

  Widget body() {
    return Scaffold(
      appBar: AppBar(
        title: const Text('TOSS POS'),
        elevation: 0,
        shadowColor: Colors.transparent,
        backgroundColor: Theme.of(context).colorScheme.primary,
        foregroundColor: Theme.of(context).colorScheme.onPrimary,
        actions: [
          syncButton(),
          networkInfo(),
        ],
      ),
      body: SafeArea(
        bottom: false, // Let us handle bottom padding manually
        child: _buildProductsView(),
      ),
    );
  }

  Widget _buildProductsView() {
    return Consumer2<ProductsProvider, HomeProvider>(
      builder: (context, provider, homeProvider, _) {
        final mediaQuery = MediaQuery.of(context);
        final hasCartItems = homeProvider.orderedProducts.isNotEmpty;
        final bottomPadding = mediaQuery.padding.bottom + 
                              56.0 + // Bottom navigation height
                              (hasCartItems ? 88.0 : 0.0); // Cart height when visible
        
        return RefreshIndicator(
          onRefresh: () => onRefresh(),
          child: Scrollbar(
            child: CustomScrollView(
              controller: scrollController,
              // Disable scroll when data is null or empty
              physics: (provider.allProducts?.isEmpty ?? true) ? const NeverScrollableScrollPhysics() : null,
              slivers: [
                SliverAppBar(
                  floating: true,
                  snap: true,
                  automaticallyImplyLeading: false,
                  collapsedHeight: 80,
                  expandedHeight: 80,
                  titleSpacing: 0,
                  backgroundColor: Theme.of(context).colorScheme.surface,
                  title: Padding(
                    padding: const EdgeInsets.symmetric(horizontal: AppSizes.padding),
                    child: ProductSearchField(
                      controller: searchFieldController,
                      showScanButton: true,
                      showAddServiceButton: true,
                      onScan: () async {
                        final code = await context.push<String>('/scan');
                        if (code != null && code.isNotEmpty) {
                          searchFieldController.text = code;
                          productProvider.allProducts = null;
                          await productProvider.getAllProducts(contains: code);
                        }
                      },
                      onAddService: () async {
                        String? name;
                        int? price;
                        final nameCtl = TextEditingController();
                        final priceCtl = TextEditingController();
                          await showDialog(
                          context: context,
                          builder: (_) {
                            return AlertDialog(
                              title: const Text('Add Custom Service'),
                              content: Column(
                                mainAxisSize: MainAxisSize.min,
                                children: [
                                  TextField(controller: nameCtl, decoration: const InputDecoration(labelText: 'Service name')),
                                  const SizedBox(height: 8),
                                  TextField(
                                    controller: priceCtl,
                                    keyboardType: TextInputType.number,
                                    decoration: const InputDecoration(labelText: 'Price (cents)'),
                                  ),
                                ],
                              ),
                              actions: [
                                TextButton(onPressed: () => Navigator.pop(context), child: const Text('Cancel')),
                                ElevatedButton(
                                  onPressed: () {
                                    name = nameCtl.text.trim();
                                    price = int.tryParse(priceCtl.text.trim());
                                    if ((name?.isEmpty ?? true) || price == null) return;
                                    Navigator.pop(context);
                                  },
                                  child: const Text('Add'),
                                ),
                              ],
                            );
                          },
                        );
                        if ((name?.isNotEmpty ?? false) && (price != null)) {
                          homeProvider.addCustomService(name: name!, price: price!);
                        }
                      },
                    ),
                  ),
                ),
                SliverToBoxAdapter(
                  child: MostUsedProductChips(
                    onSelect: (p) async {
                      searchFieldController.text = p.name;
                      productProvider.allProducts = null;
                      await productProvider.getAllProducts(contains: p.name);
                    },
                  ),
                ),
                SliverLayoutBuilder(
                  builder: (context, constraint) {
                    if (provider.allProducts == null) {
                      return SliverFillRemaining(
                        hasScrollBody: false,
                        fillOverscroll: true,
                        child: Padding(
                          padding: EdgeInsets.only(bottom: bottomPadding),
                          child: const AppProgressIndicator(),
                        ),
                      );
                    }

                    if (provider.allProducts!.isEmpty) {
                      return SliverFillRemaining(
                        hasScrollBody: false,
                        fillOverscroll: true,
                        child: Padding(
                          padding: EdgeInsets.only(bottom: bottomPadding),
                          child: AppEmptyState(
                            subtitle: 'No products available, add product to continue',
                            buttonText: 'Add Product',
                            onTapButton: () => context.push('/products/product-create'),
                          ),
                        ),
                      );
                    }

                    return SliverPadding(
                      padding: const EdgeInsets.fromLTRB(AppSizes.padding, 2, AppSizes.padding, AppSizes.padding),
                      sliver: SliverGrid.builder(
                        gridDelegate: const SliverGridDelegateWithMaxCrossAxisExtent(
                          maxCrossAxisExtent: 190,
                          childAspectRatio: 1 / 1.6,
                          crossAxisSpacing: AppSizes.padding,
                          mainAxisSpacing: AppSizes.padding,
                        ),
                        itemCount: provider.allProducts!.length,
                        itemBuilder: (context, i) {
                          return productCard(provider.allProducts![i]);
                        },
                      ),
                    );
                  },
                ),
                SliverPadding(
                  padding: EdgeInsets.only(bottom: bottomPadding),
                  sliver: SliverToBoxAdapter(
                    child: AppLoadingMoreIndicator(isLoading: provider.isLoadingMore),
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  // Removed PLU icon inline per request (search box handles PLU)

  Widget productCard(ProductEntity product) {
    return ProductsCard(
      product: product,
      onTap: () {
        if (product.stock == 0) {
          // Show alternatives for out-of-stock products
          _showAlternativesDialog(product);
          return;
        }

        // Show quantity selector for easier quantity selection
        _showQuantitySelectorDialog(product);
      },
      onLongPress: () {
        // Show detailed product information
        _showProductDetailsDialog(product);
      },
    );
  }

  void _showProductDetailsDialog(ProductEntity product) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(
          product.name,
          style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
        ),
        content: SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              // Product image
              ClipRRect(
                borderRadius: BorderRadius.circular(8),
                child: AppImage(
                  image: product.imageUrl,
                  width: double.infinity,
                  height: 120,
                  fit: BoxFit.cover,
                ),
              ),
              const SizedBox(height: 16),
              
              // Product details
              _buildDetailRow('Price', 'R ${(product.price / 100).toStringAsFixed(2)}'),
              if (product.sku != null)
                _buildDetailRow('SKU', product.sku!),
              if (product.barcode != null)
                _buildDetailRow('Barcode', product.barcode!),
              _buildDetailRow('Stock', '${product.stock} ${product.unit ?? 'units'}'),
              if (product.category != null)
                _buildDetailRow('Category', product.category!.name),
              if (product.type != ProductType.physical)
                _buildDetailRow('Type', product.type.name.toUpperCase()),
              if (product.description != null && product.description!.isNotEmpty) ...[
                const SizedBox(height: 8),
                const Text(
                  'Description:',
                  style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14),
                ),
                const SizedBox(height: 4),
                Text(
                  product.description!,
                  style: const TextStyle(fontSize: 13),
                ),
              ],
              
              // Custom attributes if any
              if (product.customAttributes != null && product.customAttributes!.isNotEmpty) ...[
                const SizedBox(height: 12),
                const Text(
                  'Additional Details:',
                  style: TextStyle(fontWeight: FontWeight.bold, fontSize: 14),
                ),
                const SizedBox(height: 4),
                ...product.customAttributes!.entries.map((entry) => 
                  _buildDetailRow(entry.key, entry.value.toString()),
                ),
              ],
              
              // Stock status
              const SizedBox(height: 12),
              Container(
                padding: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  color: product.isOutOfStock 
                      ? Colors.red[50] 
                      : product.isLowStock 
                          ? Colors.orange[50] 
                          : Colors.green[50],
                  borderRadius: BorderRadius.circular(6),
                  border: Border.all(
                    color: product.isOutOfStock 
                        ? Colors.red[200]! 
                        : product.isLowStock 
                            ? Colors.orange[200]! 
                            : Colors.green[200]!,
                  ),
                ),
                child: Row(
                  children: [
                    Icon(
                      product.isOutOfStock 
                          ? Icons.remove_shopping_cart
                          : product.isLowStock 
                              ? Icons.warning_amber
                              : Icons.check_circle,
                      color: product.isOutOfStock 
                          ? Colors.red[600] 
                          : product.isLowStock 
                              ? Colors.orange[600] 
                              : Colors.green[600],
                      size: 20,
                    ),
                    const SizedBox(width: 8),
                    Text(
                      product.isOutOfStock 
                          ? 'Out of Stock' 
                          : product.isLowStock 
                              ? 'Low Stock Warning' 
                              : 'In Stock',
                      style: TextStyle(
                        color: product.isOutOfStock 
                            ? Colors.red[600] 
                            : product.isLowStock 
                                ? Colors.orange[600] 
                                : Colors.green[600],
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
        actions: [
          if (!product.isOutOfStock) ...[
            TextButton(
              onPressed: () {
                Navigator.of(context).pop();
                homeProvider.onAddOrderedProduct(product, 1);
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(
                    content: Text('${product.name} added to cart'),
                    duration: const Duration(seconds: 1),
                    backgroundColor: Theme.of(context).colorScheme.primary,
                  ),
                );
              },
              child: const Text('Add to Cart'),
            ),
          ] else ...[
            TextButton(
              onPressed: () {
                Navigator.of(context).pop();
                _showAlternativesDialog(product);
              },
              child: const Text('View Alternatives'),
            ),
          ],
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Close'),
          ),
        ],
      ),
    );
  }

  Widget _buildDetailRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 2),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 80,
            child: Text(
              '$label:',
              style: const TextStyle(
                fontWeight: FontWeight.w500,
                fontSize: 13,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(fontSize: 13),
            ),
          ),
        ],
      ),
    );
  }

  void _showAlternativesDialog(ProductEntity outOfStockProduct) {
    final alternatives = productProvider.getAlternativeProducts(outOfStockProduct);
    
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              '${outOfStockProduct.name}',
              style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 4),
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
              decoration: BoxDecoration(
                color: Colors.red,
                borderRadius: BorderRadius.circular(4),
              ),
              child: const Text(
                'OUT OF STOCK',
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 12,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ],
        ),
        content: alternatives.isEmpty 
          ? const Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Icon(Icons.sentiment_dissatisfied, size: 48, color: Colors.grey),
                SizedBox(height: 16),
                Text(
                  'No alternatives available at the moment.',
                  textAlign: TextAlign.center,
                  style: TextStyle(color: Colors.grey),
                ),
              ],
            )
          : Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Here are some similar products:',
                  style: TextStyle(fontWeight: FontWeight.w500),
                ),
                const SizedBox(height: 12),
                ...alternatives.map((alternative) => 
                  _buildAlternativeItem(alternative),
                ),
              ],
            ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Close'),
          ),
        ],
      ),
    );
  }

  Widget _buildAlternativeItem(ProductEntity product) {
    return InkWell(
      onTap: () {
        Navigator.of(context).pop(); // Close dialog
        // Add alternative product to cart
        homeProvider.onAddOrderedProduct(product, 1);
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('${product.name} added to cart'),
            duration: const Duration(seconds: 1),
            backgroundColor: Theme.of(context).colorScheme.primary,
          ),
        );
      },
      child: Container(
        margin: const EdgeInsets.only(bottom: 8),
        padding: const EdgeInsets.all(8),
        decoration: BoxDecoration(
          border: Border.all(color: Colors.grey[300]!),
          borderRadius: BorderRadius.circular(8),
        ),
        child: Row(
          children: [
            // Small product image
            ClipRRect(
              borderRadius: BorderRadius.circular(4),
              child: AppImage(
                image: product.imageUrl,
                width: 40,
                height: 40,
                fit: BoxFit.cover,
              ),
            ),
            const SizedBox(width: 12),
            
            // Product details
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    product.name,
                    style: const TextStyle(
                      fontWeight: FontWeight.w500,
                      fontSize: 14,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: 2),
                  Row(
                    children: [
                      Text(
                        'R ${(product.price / 100).toStringAsFixed(2)}',
                        style: TextStyle(
                          color: Theme.of(context).primaryColor,
                          fontWeight: FontWeight.bold,
                          fontSize: 13,
                        ),
                      ),
                      const SizedBox(width: 8),
                      Text(
                        '${product.stock} in stock',
                        style: TextStyle(
                          color: Colors.green[600],
                          fontSize: 11,
                        ),
                      ),
                    ],
                  ),
                  if (product.category != null)
                    Text(
                      product.category!.name,
                      style: TextStyle(
                        color: Colors.grey[600],
                        fontSize: 10,
                      ),
                    ),
                ],
              ),
            ),
            
            // Add button
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
              decoration: BoxDecoration(
                color: Theme.of(context).primaryColor,
                borderRadius: BorderRadius.circular(4),
              ),
              child: const Text(
                'Add',
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 12,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  void _showQuantitySelectorDialog(ProductEntity product) {
    int selectedQuantity = 1;
    
    showDialog(
      context: context,
      builder: (context) => StatefulBuilder(
        builder: (context, setState) => AlertDialog(
          title: Row(
            children: [
              Icon(
                Icons.add_shopping_cart,
                color: Theme.of(context).primaryColor,
                size: 24,
              ),
              const SizedBox(width: 8),
              const Text('Add to Cart'),
            ],
          ),
          content: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Product info
              Row(
                children: [
                  Container(
                    width: 40,
                    height: 40,
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(8),
                      color: Colors.grey[100],
                    ),
                    child: ClipRRect(
                      borderRadius: BorderRadius.circular(8),
                      child: product.imageUrl.isNotEmpty
                          ? Image.network(
                              product.imageUrl,
                              fit: BoxFit.cover,
                              errorBuilder: (context, error, stackTrace) => 
                                Icon(Icons.inventory_2, color: Colors.grey[400]),
                            )
                          : Icon(Icons.inventory_2, color: Colors.grey[400]),
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          product.name,
                          style: const TextStyle(
                            fontWeight: FontWeight.w600,
                            fontSize: 16,
                          ),
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'R ${(product.price / 100).toStringAsFixed(2)} each',
                          style: TextStyle(
                            color: Theme.of(context).primaryColor,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
              
              const SizedBox(height: 16),
              
              // Stock info
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                decoration: BoxDecoration(
                  color: product.isLowStock 
                      ? Colors.orange[50] 
                      : Colors.green[50],
                  borderRadius: BorderRadius.circular(4),
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(
                      product.isLowStock ? Icons.warning_amber : Icons.check_circle,
                      color: product.isLowStock ? Colors.orange[600] : Colors.green[600],
                      size: 16,
                    ),
                    const SizedBox(width: 4),
                    Text(
                      '${product.stock} in stock',
                      style: TextStyle(
                        color: product.isLowStock ? Colors.orange[600] : Colors.green[600],
                        fontSize: 12,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ],
                ),
              ),
              
              const SizedBox(height: 20),
              
              // Quantity selector
              const Text(
                'Select Quantity:',
                style: TextStyle(
                  fontWeight: FontWeight.w600,
                  fontSize: 14,
                ),
              ),
              const SizedBox(height: 12),
              
              // Quantity controls
              Container(
                decoration: BoxDecoration(
                  border: Border.all(color: Colors.grey[300]!),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    // Decrease button
                    Material(
                      color: Colors.transparent,
                      child: InkWell(
                        onTap: selectedQuantity > 1 
                            ? () => setState(() => selectedQuantity--) 
                            : null,
                        borderRadius: const BorderRadius.only(
                          topLeft: Radius.circular(8),
                          bottomLeft: Radius.circular(8),
                        ),
                        child: Container(
                          padding: const EdgeInsets.all(12),
                          child: Icon(
                            Icons.remove,
                            color: selectedQuantity > 1 
                                ? Theme.of(context).primaryColor 
                                : Colors.grey[400],
                          ),
                        ),
                      ),
                    ),
                    
                    // Quantity display
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
                      decoration: BoxDecoration(
                        border: Border.symmetric(
                          vertical: BorderSide(color: Colors.grey[300]!),
                        ),
                      ),
                      child: Text(
                        selectedQuantity.toString(),
                        style: const TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                    
                    // Increase button
                    Material(
                      color: Colors.transparent,
                      child: InkWell(
                        onTap: selectedQuantity < product.stock 
                            ? () => setState(() => selectedQuantity++) 
                            : null,
                        borderRadius: const BorderRadius.only(
                          topRight: Radius.circular(8),
                          bottomRight: Radius.circular(8),
                        ),
                        child: Container(
                          padding: const EdgeInsets.all(12),
                          child: Icon(
                            Icons.add,
                            color: selectedQuantity < product.stock 
                                ? Theme.of(context).primaryColor 
                                : Colors.grey[400],
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
              
              const SizedBox(height: 12),
              
              // Quick quantity buttons
              Wrap(
                spacing: 8,
                children: [1, 2, 3, 5, 10].where((qty) => qty <= product.stock).map((qty) => 
                  OutlinedButton(
                    onPressed: () => setState(() => selectedQuantity = qty),
                    style: OutlinedButton.styleFrom(
                      foregroundColor: selectedQuantity == qty 
                          ? Colors.white 
                          : Theme.of(context).primaryColor,
                      backgroundColor: selectedQuantity == qty 
                          ? Theme.of(context).primaryColor 
                          : null,
                      minimumSize: const Size(40, 32),
                      padding: const EdgeInsets.symmetric(horizontal: 8),
                    ),
                    child: Text(qty.toString()),
                  ),
                ).toList(),
              ),
              
              const SizedBox(height: 16),
              
              // Total price
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Theme.of(context).primaryColor.withOpacity(0.1),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    const Text(
                      'Total:',
                      style: TextStyle(
                        fontWeight: FontWeight.w600,
                        fontSize: 16,
                      ),
                    ),
                    Text(
                      'R ${((product.price * selectedQuantity) / 100).toStringAsFixed(2)}',
                      style: TextStyle(
                        color: Theme.of(context).primaryColor,
                        fontWeight: FontWeight.bold,
                        fontSize: 18,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('Cancel'),
            ),
            ElevatedButton.icon(
              onPressed: () {
                Navigator.of(context).pop();
                homeProvider.onAddOrderedProduct(product, selectedQuantity);
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(
                    content: Text('$selectedQuantity x ${product.name} added to cart'),
                    duration: const Duration(seconds: 2),
                    backgroundColor: Theme.of(context).colorScheme.primary,
                  ),
                );
              },
              icon: const Icon(Icons.add_shopping_cart),
              label: const Text('Add to Cart'),
            ),
          ],
        ),
      ),
    );
  }
}
