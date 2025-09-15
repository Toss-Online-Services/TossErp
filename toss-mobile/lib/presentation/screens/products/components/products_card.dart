import 'package:flutter/material.dart';

import '../../../../domain/entities/product_entity.dart';
import '../../../widgets/app_image.dart';

class ProductsCard extends StatelessWidget {
  final ProductEntity product;
  final VoidCallback? onTap;
  final VoidCallback? onLongPress;

  const ProductsCard({
    super.key,
    required this.product,
    this.onTap,
    this.onLongPress,
  });

  @override
  Widget build(BuildContext context) {
    final isOutOfStock = product.stock == 0;
    final isLowStock = product.isLowStock && !isOutOfStock;
    
    return Card(
      elevation: 4,
      shadowColor: Colors.black26,
      child: Material(
        borderRadius: BorderRadius.circular(12),
        child: InkWell(
          onTap: isOutOfStock ? null : onTap,
          onLongPress: onLongPress,
          borderRadius: BorderRadius.circular(12),
          child: Container(
            padding: const EdgeInsets.all(8.0),
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(12),
              color: isOutOfStock ? Colors.grey[100] : null,
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Product Image with overlays
                Expanded(
                  flex: 3,
                  child: Container(
                    width: double.infinity,
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(8),
                      color: Colors.grey[50],
                    ),
                    child: Stack(
                      children: [
                        ClipRRect(
                          borderRadius: BorderRadius.circular(8),
                          child: AppImage(
                            image: product.imageUrl,
                            fit: BoxFit.cover,
                            width: double.infinity,
                            height: double.infinity,
                          ),
                        ),
                        
                        // Stock status overlay
                        if (isOutOfStock)
                          Container(
                            decoration: BoxDecoration(
                              borderRadius: BorderRadius.circular(8),
                              color: Colors.black54,
                            ),
                            child: Center(
                              child: Column(
                                mainAxisAlignment: MainAxisAlignment.center,
                                children: [
                                  Container(
                                    padding: const EdgeInsets.symmetric(
                                      horizontal: 8, vertical: 4,
                                    ),
                                    decoration: BoxDecoration(
                                      color: Colors.red,
                                      borderRadius: BorderRadius.circular(4),
                                    ),
                                    child: const Text(
                                      'OUT OF STOCK',
                                      style: TextStyle(
                                        color: Colors.white,
                                        fontSize: 10,
                                        fontWeight: FontWeight.bold,
                                      ),
                                    ),
                                  ),
                                  const SizedBox(height: 4),
                                  const Text(
                                    'See alternatives below',
                                    style: TextStyle(
                                      color: Colors.white,
                                      fontSize: 9,
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          ),
                          
                        // Low stock warning
                        if (isLowStock)
                          Positioned(
                            top: 4,
                            right: 4,
                            child: Container(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 6, vertical: 2,
                              ),
                              decoration: BoxDecoration(
                                color: Colors.orange,
                                borderRadius: BorderRadius.circular(3),
                              ),
                              child: const Text(
                                'LOW',
                                style: TextStyle(
                                  color: Colors.white,
                                  fontSize: 8,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ),
                          ),
                          
                        // Category badge
                        if (product.category != null)
                          Positioned(
                            top: 4,
                            left: 4,
                            child: Container(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 6, vertical: 2,
                              ),
                              decoration: BoxDecoration(
                                color: Theme.of(context).primaryColor.withOpacity(0.8),
                                borderRadius: BorderRadius.circular(3),
                              ),
                              child: Text(
                                product.category!.name,
                                style: const TextStyle(
                                  color: Colors.white,
                                  fontSize: 8,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ),
                          ),
                      ],
                    ),
                  ),
                ),
                
                const SizedBox(height: 8.0),
                
                // Product Info
                Expanded(
                  flex: 3,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      // Product Name and SKU
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              product.name,
                              style: Theme.of(context).textTheme.titleMedium?.copyWith(
                                fontWeight: FontWeight.w600,
                                fontSize: 13,
                              ),
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis,
                            ),
                            
                            if (product.sku != null) ...[
                              const SizedBox(height: 1),
                              Text(
                                'SKU: ${product.sku}',
                                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                  color: Colors.grey[600],
                                  fontSize: 9,
                                ),
                              ),
                            ],
                          ],
                        ),
                      ),
                      
                      const SizedBox(height: 4.0),
                      
                      // Product details in compact format
                      if (product.description != null || product.unit != null) ...[
                        Container(
                          width: double.infinity,
                          padding: const EdgeInsets.symmetric(
                            vertical: 2, horizontal: 4,
                          ),
                          decoration: BoxDecoration(
                            color: Colors.grey[100],
                            borderRadius: BorderRadius.circular(4),
                          ),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              if (product.unit != null)
                                Text(
                                  'Unit: ${product.unit}',
                                  style: TextStyle(
                                    fontSize: 9,
                                    color: Colors.grey[700],
                                    fontWeight: FontWeight.w500,
                                  ),
                                ),
                              if (product.description != null && product.description!.isNotEmpty)
                                Text(
                                  product.description!,
                                  style: TextStyle(
                                    fontSize: 8,
                                    color: Colors.grey[600],
                                  ),
                                  maxLines: 1,
                                  overflow: TextOverflow.ellipsis,
                                ),
                            ],
                          ),
                        ),
                        const SizedBox(height: 4.0),
                      ],
                      
                      // Price - Make it prominent for POS
                      Container(
                        width: double.infinity,
                        padding: const EdgeInsets.symmetric(
                          vertical: 6, horizontal: 8,
                        ),
                        decoration: BoxDecoration(
                          color: isOutOfStock 
                              ? Colors.grey[200] 
                              : Theme.of(context).primaryColor.withOpacity(0.1),
                          borderRadius: BorderRadius.circular(6),
                          border: Border.all(
                            color: isOutOfStock 
                                ? Colors.grey[400]! 
                                : Theme.of(context).primaryColor.withOpacity(0.3),
                          ),
                        ),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Expanded(
                              child: Text(
                                'R ${(product.price / 100).toStringAsFixed(2)}',
                                style: Theme.of(context).textTheme.titleLarge?.copyWith(
                                  color: isOutOfStock 
                                      ? Colors.grey[600] 
                                      : Theme.of(context).primaryColor,
                                  fontWeight: FontWeight.bold,
                                  fontSize: 14,
                                ),
                              ),
                            ),
                            // Stock info with icon
                            Row(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Icon(
                                  isOutOfStock 
                                      ? Icons.remove_shopping_cart
                                      : isLowStock 
                                          ? Icons.warning_amber
                                          : Icons.inventory_2,
                                  size: 12,
                                  color: isOutOfStock 
                                      ? Colors.grey[600]
                                      : isLowStock 
                                          ? Colors.orange[700]
                                          : Colors.green[700],
                                ),
                                const SizedBox(width: 2),
                                Text(
                                  '${product.stock}',
                                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                    color: isOutOfStock 
                                        ? Colors.grey[600]
                                        : isLowStock 
                                            ? Colors.orange[700]
                                            : Colors.green[700],
                                    fontWeight: FontWeight.w600,
                                    fontSize: 11,
                                  ),
                                ),
                              ],
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
      ),
    );
  }
}