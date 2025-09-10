import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../app/themes/app_sizes.dart';
import '../../../domain/entities/product_entity.dart';
import '../../../service_locator.dart';
import '../../providers/products/products_provider.dart';
import '../../widgets/app_button.dart';
import '../../widgets/app_empty_state.dart';
import '../../widgets/app_loading_more_indicator.dart';
import '../../widgets/app_progress_indicator.dart';
import '../../widgets/app_text_field.dart';
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
        title: const Text('Products'),
        elevation: 0,
        shadowColor: Colors.transparent,
        actions: [addButton()],
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
                            child: searchField(),
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

  Widget searchField() {
    return AppTextField(
      controller: searchFieldController,
      hintText: 'Search Products...',
      type: AppTextFieldType.search,
      textInputAction: TextInputAction.search,
      onEditingComplete: () {
        FocusScope.of(context).unfocus();
        productProvider.allProducts = null;
        productProvider.getAllProducts(contains: searchFieldController.text);
      },
      onTapClearButton: () {
        productProvider.getAllProducts(contains: searchFieldController.text);
      },
    );
  }

  Widget productCard(ProductEntity product) {
    return ProductsCard(
      product: product,
      onTap: () => context.go('/products/product-detail/${product.id}'),
    );
  }
}
