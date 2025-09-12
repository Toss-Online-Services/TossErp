import 'package:flutter/material.dart';
import '../../../../domain/entities/product_entity.dart';

class POSProductGrid extends StatelessWidget {
  final List<ProductEntity> products;
  final Function(ProductEntity) onProductTap;
  final String? selectedCategory;

  const POSProductGrid({
    super.key,
    required this.products,
    required this.onProductTap,
    this.selectedCategory,
  });

  @override
  Widget build(BuildContext context) {
    return GridView.builder(
      padding: const EdgeInsets.all(8.0),
      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 4,
        childAspectRatio: 0.8,
        crossAxisSpacing: 8.0,
        mainAxisSpacing: 8.0,
      ),
      itemCount: products.length,
      itemBuilder: (context, index) {
        final product = products[index];
        return _ProductGridItem(
          product: product,
          onTap: () => onProductTap(product),
        );
      },
    );
  }
}

class _ProductGridItem extends StatelessWidget {
  final ProductEntity product;
  final VoidCallback onTap;

  const _ProductGridItem({
    required this.product,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      elevation: 2,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(8),
        child: Padding(
          padding: const EdgeInsets.all(8.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Container(
                  width: double.infinity,
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(4),
                    color: Colors.grey[200],
                  ),
                  child: product.imageUrl != null
                      ? ClipRRect(
                          borderRadius: BorderRadius.circular(4),
                          child: Image.network(
                            product.imageUrl!,
                            fit: BoxFit.cover,
                            errorBuilder: (context, error, stackTrace) {
                              return const Icon(Icons.image_not_supported);
                            },
                          ),
                        )
                      : const Icon(Icons.inventory_2_outlined, size: 32),
                ),
              ),
              const SizedBox(height: 4),
              Text(
                product.name,
                style: const TextStyle(
                  fontSize: 12,
                  fontWeight: FontWeight.w500,
                ),
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
              Text(
                '${product.price.toStringAsFixed(2)}',
                style: TextStyle(
                  fontSize: 11,
                  color: Theme.of(context).primaryColor,
                  fontWeight: FontWeight.bold,
                ),
              ),
              if (product.stockQuantity != null && product.stockQuantity! <= 5)
                Container(
                  margin: const EdgeInsets.only(top: 2),
                  padding: const EdgeInsets.symmetric(horizontal: 4, vertical: 1),
                  decoration: BoxDecoration(
                    color: Colors.orange[100],
                    borderRadius: BorderRadius.circular(2),
                  ),
                  child: Text(
                    'Low Stock',
                    style: TextStyle(
                      fontSize: 8,
                      color: Colors.orange[800],
                    ),
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}
