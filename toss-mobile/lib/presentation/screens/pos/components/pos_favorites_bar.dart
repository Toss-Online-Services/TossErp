import 'package:flutter/material.dart';
import '../../../../domain/entities/product_entity.dart';

class POSFavoritesBar extends StatelessWidget {
  final List<ProductEntity> favoriteProducts;
  final Function(ProductEntity) onProductTap;

  const POSFavoritesBar({
    super.key,
    required this.favoriteProducts,
    required this.onProductTap,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 80,
      padding: const EdgeInsets.symmetric(horizontal: 8),
      child: ListView.builder(
        scrollDirection: Axis.horizontal,
        itemCount: favoriteProducts.length,
        itemBuilder: (context, index) {
          final product = favoriteProducts[index];
          return Container(
            width: 120,
            margin: const EdgeInsets.only(right: 8),
            child: Card(
              child: InkWell(
                onTap: () => onProductTap(product),
                child: Padding(
                  padding: const EdgeInsets.all(8.0),
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Icon(Icons.favorite, size: 20),
                      const SizedBox(height: 4),
                      Text(
                        product.name,
                        style: const TextStyle(fontSize: 10),
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        textAlign: TextAlign.center,
                      ),
                    ],
                  ),
                ),
              ),
            ),
          );
        },
      ),
    );
  }
}
