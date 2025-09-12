import '../../widgets/app_image.dart';
// import '../../widgets/customizable_dashboard.dart'; // Temporarily disabled
import '../dashboard/simple_dashboard_widget.dart';
import 'dart:async';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../app/const/const.dart';
import '../../../app/themes/app_sizes.dart';
import '../../../app/utilities/currency_formatter.dart';
import '../../../domain/entities/product_entity.dart';
import '../../../service_locator.dart';
import '../../../simple_dashboard_manager.dart';
import '../../providers/home/home_provider.dart';
import '../../providers/main/main_provider.dart';
import '../../providers/products/products_provider.dart';
import '../../widgets/app_button.dart';
import '../../widgets/app_dialog.dart';
import '../../widgets/app_empty_state.dart';
import '../../widgets/app_loading_more_indicator.dart';
import '../../widgets/app_progress_indicator.dart';
import '../../widgets/product_search_field.dart';
import '../../widgets/most_used_product_chips.dart';
import '../products/components/products_card.dart';
import 'components/simple_cart_panel.dart';
import 'components/order_card.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> with TickerProviderStateMixin {
  final mainProvider = sl<MainProvider>();
  final homeProvider = sl<HomeProvider>();
  final productProvider = sl<ProductsProvider>();
  final dashboardManager = sl<SimpleDashboardManager>();

  final scrollController = ScrollController();

  final searchFieldController = TextEditingController();
  Timer? _searchDebounce;

  late TabController _tabController;

  @override
  void initState() {
    _tabController = TabController(length: 2, vsync: this);
    scrollController.addListener(scrollListener);
    WidgetsBinding.instance.addPostFrameCallback((_) => onRefresh());
    super.initState();
  }

  @override
  void dispose() {
    _tabController.dispose();
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
        title: title(),
        elevation: 0,
        shadowColor: Colors.transparent,
        actions: [
          syncButton(),
          networkInfo(),
        ],
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(
              icon: Icon(Icons.dashboard),
              text: 'Dashboard',
            ),
            Tab(
              icon: Icon(Icons.shopping_cart),
              text: 'Products',
            ),
          ],
        ),
      ),
      body: SafeArea(
        bottom: false, // Let us handle bottom padding manually
        child: TabBarView(
          controller: _tabController,
          children: [
            // const CustomizableDashboard(), // Temporarily disabled
            const SimpleDashboardWidget(),
            _buildProductsView(),
          ],
        ),
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
                  collapsedHeight: 70,
                  titleSpacing: 0,
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
                          maxCrossAxisExtent: 200,
                          childAspectRatio: 1 / 1.5,
                          crossAxisSpacing: AppSizes.padding / 2,
                          mainAxisSpacing: AppSizes.padding / 2,
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
        if (product.stock == 0) return;

        int currentQty =
            homeProvider.orderedProducts.where((e) => e.productId == product.id).firstOrNull?.quantity ?? 0;

        AppDialog.show(
          title: 'Enter Amount',
          child: OrderCard(
            name: product.name,
            imageUrl: product.imageUrl,
            stock: product.stock,
            price: product.price,
            initialQuantity: currentQty,
            onChangedQuantity: (val) {
              currentQty = val;
            },
          ),
          rightButtonText: 'Add To Cart',
          leftButtonText: 'Cancel',
          onTapLeftButton: () {
            context.pop();
          },
          onTapRightButton: () {
            homeProvider.onAddOrderedProduct(product, currentQty == 0 ? 1 : currentQty);
            context.pop();
          },
        );
      },
    );
  }
}
