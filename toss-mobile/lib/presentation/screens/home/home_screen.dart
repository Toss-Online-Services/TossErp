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
                  onVerticalDragEnd: (details) {
                    // If dragging upward (negative velocity), open the cart
                    if (details.primaryVelocity != null && details.primaryVelocity! < -300) {
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
                    }
                  },
                  child: Container(
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
                    child: IntrinsicHeight(
                      child: Column(
                        children: [
                          // Drag Handle - Centered at top
                          Container(
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
                          
                          // Cart Content
                          Padding(
                            padding: const EdgeInsets.fromLTRB(AppSizes.padding, 0, AppSizes.padding, AppSizes.padding),
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
                                          provider.getTotalQuantity().toString(),
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
                                    mainAxisSize: MainAxisSize.min,
                                    children: [
                                      Text(
                                        '${provider.getTotalQuantity()} item${provider.getTotalQuantity() != 1 ? 's' : ''}',
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
                              ],
                            ),
                          ),
                        ],
                      ),
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

        // Check if product is already in cart
        var currentIndex = homeProvider.orderedProducts.indexWhere((e) => e.productId == product.id);
        
        if (currentIndex != -1) {
          // Product exists in cart, increase quantity by 1
          var currentQty = homeProvider.orderedProducts[currentIndex].quantity;
          var newQty = currentQty + 1;
          
          // Check if new quantity doesn't exceed stock
          if (newQty <= product.stock) {
            homeProvider.onChangedOrderedProductQuantity(currentIndex, newQty);
            
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text('${product.name} quantity increased to $newQty'),
                duration: const Duration(seconds: 1),
                backgroundColor: Theme.of(context).colorScheme.primary,
                behavior: SnackBarBehavior.floating,
                margin: const EdgeInsets.all(16),
              ),
            );
          } else {
            // Show stock limit message
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text('Cannot add more. Only ${product.stock} in stock'),
                duration: const Duration(seconds: 2),
                backgroundColor: Colors.orange,
                behavior: SnackBarBehavior.floating,
                margin: const EdgeInsets.all(16),
              ),
            );
          }
        } else {
          // Product not in cart, add with quantity 1
          homeProvider.onAddOrderedProduct(product, 1);
          
          // Show feedback
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text('${product.name} added to cart'),
              duration: const Duration(seconds: 1),
              backgroundColor: Theme.of(context).colorScheme.primary,
              behavior: SnackBarBehavior.floating,
              margin: const EdgeInsets.all(16),
            ),
          );
        }
      },
      onDoubleTap: () {
        if (product.stock == 0) {
          return; // Don't allow double tap for out of stock
        }

        // Check if product is already in cart
        var currentIndex = homeProvider.orderedProducts.indexWhere((e) => e.productId == product.id);
        
        if (currentIndex != -1) {
          // Product exists in cart, increase quantity by 1
          var currentQty = homeProvider.orderedProducts[currentIndex].quantity;
          var newQty = currentQty + 1;
          
          // Check if new quantity doesn't exceed stock
          if (newQty <= product.stock) {
            homeProvider.onChangedOrderedProductQuantity(currentIndex, newQty);
            
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text('${product.name} quantity increased to $newQty'),
                duration: const Duration(seconds: 1),
                backgroundColor: Theme.of(context).colorScheme.secondary,
                behavior: SnackBarBehavior.floating,
                margin: const EdgeInsets.all(16),
              ),
            );
          } else {
            // Show stock limit message
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text('Cannot add more. Only ${product.stock} in stock'),
                duration: const Duration(seconds: 2),
                backgroundColor: Colors.orange,
                behavior: SnackBarBehavior.floating,
                margin: const EdgeInsets.all(16),
              ),
            );
          }
        } else {
          // Product not in cart, add with quantity 2
          if (product.stock >= 2) {
            homeProvider.onAddOrderedProduct(product, 2);
            
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text('${product.name} added to cart with quantity 2'),
                duration: const Duration(seconds: 1),
                backgroundColor: Theme.of(context).colorScheme.secondary,
                behavior: SnackBarBehavior.floating,
                margin: const EdgeInsets.all(16),
              ),
            );
          } else {
            // Only 1 item in stock, add with quantity 1
            homeProvider.onAddOrderedProduct(product, 1);
            
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                content: Text('${product.name} added to cart (only 1 in stock)'),
                duration: const Duration(seconds: 1),
                backgroundColor: Theme.of(context).colorScheme.primary,
                behavior: SnackBarBehavior.floating,
                margin: const EdgeInsets.all(16),
              ),
            );
          }
        }
      },
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
}
