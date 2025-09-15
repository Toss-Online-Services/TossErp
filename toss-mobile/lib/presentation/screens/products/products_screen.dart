import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:get_it/get_it.dart';
import 'package:go_router/go_router.dart';

import '../../../domain/entities/product_entity.dart';
import '../../../domain/entities/product_category_entity.dart';
import '../../providers/products/products_provider.dart';
import '../../widgets/app_empty_state.dart';
import '../../widgets/app_progress_indicator.dart';
import '../../widgets/product_search_field.dart';
import 'components/products_card.dart';

class ProductsScreen extends StatefulWidget {
  const ProductsScreen({super.key});

  @override
  State<ProductsScreen> createState() => _ProductsScreenState();
}

class _ProductsScreenState extends State<ProductsScreen> {
  late ProductsProvider _productsProvider;
  late TextEditingController _searchController;
  List<ProductEntity> _filteredProducts = [];
  int? _selectedCategoryId;

  @override
  void initState() {
    super.initState();
    _productsProvider = GetIt.instance<ProductsProvider>();
    _searchController = TextEditingController();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _loadProducts();
    });
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _loadProducts() async {
    await _productsProvider.getAllProducts();
    _filterProducts();
  }

  void _filterProducts() {
    List<ProductEntity> filtered = _productsProvider.allProducts ?? [];
    
    if (_selectedCategoryId != null) {
      filtered = filtered.where((product) => product.categoryId == _selectedCategoryId).toList();
    }
    
    setState(() {
      _filteredProducts = filtered;
    });
  }

  void _onCategorySelected(int? categoryId) {
    setState(() {
      _selectedCategoryId = categoryId;
    });
    _filterProducts();
  }

  Widget _buildCategoryChips() {
    // For now, we'll create some sample categories since we don't have category provider
    final categories = [
      const ProductCategoryEntity(id: 1, name: 'Food'),
      const ProductCategoryEntity(id: 2, name: 'Beverages'),
      const ProductCategoryEntity(id: 3, name: 'Household'),
      const ProductCategoryEntity(id: 4, name: 'Airtime'),
    ];
    
    return Container(
      height: 50,
      padding: const EdgeInsets.symmetric(horizontal: 16.0),
      child: ListView(
        scrollDirection: Axis.horizontal,
        children: [
          // All products chip
          Padding(
            padding: const EdgeInsets.only(right: 8.0),
            child: FilterChip(
              label: const Text('All'),
              selected: _selectedCategoryId == null,
              onSelected: (_) => _onCategorySelected(null),
              backgroundColor: Colors.grey[200],
              selectedColor: Theme.of(context).primaryColor,
              labelStyle: TextStyle(
                color: _selectedCategoryId == null ? Colors.white : Colors.black,
              ),
            ),
          ),
          // Category chips
          ...categories.map((category) {
            final isSelected = _selectedCategoryId == category.id;
            return Padding(
              padding: const EdgeInsets.only(right: 8.0),
              child: FilterChip(
                label: Text(category.name),
                selected: isSelected,
                onSelected: (_) => _onCategorySelected(category.id),
                backgroundColor: Colors.grey[200],
                selectedColor: Theme.of(context).primaryColor,
                labelStyle: TextStyle(
                  color: isSelected ? Colors.white : Colors.black,
                ),
              ),
            );
          }).toList(),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Products'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: Consumer<ProductsProvider>(
        builder: (context, productsProvider, child) {
          if (productsProvider.isLoadingMore && (productsProvider.allProducts?.isEmpty ?? true)) {
            return const Center(child: AppProgressIndicator());
          }

          return RefreshIndicator(
            onRefresh: _loadProducts,
            child: Column(
              children: [
                // Search bar
                Padding(
                  padding: const EdgeInsets.all(16.0),
                  child: ProductSearchField(
                    controller: _searchController,
                  ),
                ),
                
                // Category filter chips
                _buildCategoryChips(),
                
                // Products grid
                Expanded(
                  child: _filteredProducts.isEmpty
                      ? const AppEmptyState(
                          title: 'No products found',
                          subtitle: 'Try changing your filters or add some products',
                        )
                      : GridView.builder(
                          padding: const EdgeInsets.all(16.0),
                          gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                            crossAxisCount: 2,
                            crossAxisSpacing: 16.0,
                            mainAxisSpacing: 16.0,
                            childAspectRatio: 0.8,
                          ),
                          itemCount: _filteredProducts.length,
                          itemBuilder: (context, index) {
                            final product = _filteredProducts[index];
                            return ProductsCard(
                              product: product,
                              onTap: () {
                                context.push('/products/product-detail/${product.id}');
                              },
                            );
                          },
                        ),
                ),
              ],
            ),
          );
        },
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () {
          // TODO: Navigate to add product screen
        },
        tooltip: 'Add Product',
        child: const Icon(Icons.add),
      ),
    );
  }
}