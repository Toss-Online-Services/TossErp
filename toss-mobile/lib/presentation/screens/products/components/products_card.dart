import 'package:flutter/material.dart';

import '../../../../domain/entities/product_entity.dart';
import '../../../widgets/app_image.dart';

class ProductsCard extends StatelessWidget {
  final ProductEntity product;
  final VoidCallback? onTap;
  final VoidCallback? onDoubleTap;

  const ProductsCard({
    super.key,
    required this.product,
    this.onTap,
    this.onDoubleTap,
  });

  @override
  Widget build(BuildContext context) {
    final isOutOfStock = product.stock == 0;
    final isLowStock = product.isLowStock && !isOutOfStock;
    final isDarkMode = Theme.of(context).brightness == Brightness.dark;
    final screenWidth = MediaQuery.of(context).size.width;
    
    // Responsive sizing based on screen width
    final isTablet = screenWidth > 600;
    final cardPadding = isTablet ? 12.0 : 8.0;
    final borderRadius = isTablet ? 16.0 : 12.0;
    
    return LayoutBuilder(
      builder: (context, constraints) {
        final cardWidth = constraints.maxWidth;
        final imageHeight = cardWidth * 0.75; // Responsive image height
        
        return Card(
          elevation: 4,
          shadowColor: Colors.black26,
          child: Material(
            borderRadius: BorderRadius.circular(borderRadius),
            child: GestureDetector(
              onTap: isOutOfStock ? null : onTap,
              onDoubleTap: isOutOfStock ? null : onDoubleTap,
              child: Container(
                padding: EdgeInsets.all(cardPadding),
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(borderRadius),
                  color: isOutOfStock 
                    ? (isDarkMode ? Colors.grey[800] : Colors.grey[100]) 
                    : null,
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisSize: MainAxisSize.min, // Make column take minimum space
                  children: [
                    // Product Image with overlays
                    Container(
                      width: double.infinity,
                      height: imageHeight,
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(8),
                        color: isDarkMode ? Colors.grey[800] : Colors.grey[50],
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
                    
                    SizedBox(height: cardPadding),
                    
                    // Product Info with responsive layout
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        // Product Name and SKU
                        Text(
                          product.name,
                          style: Theme.of(context).textTheme.titleMedium?.copyWith(
                            fontWeight: FontWeight.w600,
                            fontSize: isTablet ? 15 : 13,
                          ),
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                        ),
                        
                        if (product.sku != null) ...[
                          const SizedBox(height: 2),
                          Text(
                            'SKU: ${product.sku}',
                            style: Theme.of(context).textTheme.bodySmall?.copyWith(
                              color: isDarkMode ? Colors.grey[400] : Colors.grey[600],
                              fontSize: isTablet ? 10 : 9,
                            ),
                          ),
                        ],
                        
                        const SizedBox(height: 8),
                        
                        // Product details in compact format
                        if (product.description != null || product.unit != null) ...[
                          Container(
                            width: double.infinity,
                            padding: EdgeInsets.symmetric(
                              vertical: isTablet ? 4 : 2, 
                              horizontal: isTablet ? 6 : 4,
                            ),
                            decoration: BoxDecoration(
                              color: isDarkMode ? Colors.grey[700] : Colors.grey[100],
                              borderRadius: BorderRadius.circular(4),
                            ),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                if (product.unit != null)
                                  Text(
                                    'Unit: ${product.unit}',
                                    style: TextStyle(
                                      fontSize: isTablet ? 10 : 9,
                                      color: isDarkMode ? Colors.grey[300] : Colors.grey[700],
                                      fontWeight: FontWeight.w500,
                                    ),
                                  ),
                                if (product.description != null && product.description!.isNotEmpty)
                                  Text(
                                    product.description!,
                                    style: TextStyle(
                                      fontSize: isTablet ? 9 : 8,
                                      color: isDarkMode ? Colors.grey[400] : Colors.grey[600],
                                    ),
                                    maxLines: 1,
                                    overflow: TextOverflow.ellipsis,
                                  ),
                              ],
                            ),
                          ),
                          const SizedBox(height: 8),
                        ],
                        
                        // Price with better responsive design
                        Container(
                          width: double.infinity,
                          padding: EdgeInsets.symmetric(
                            vertical: isTablet ? 8 : 6, 
                            horizontal: isTablet ? 10 : 8,
                          ),
                          decoration: BoxDecoration(
                            color: isOutOfStock 
                                ? (isDarkMode ? Colors.grey[700] : Colors.grey[200])
                                : Theme.of(context).primaryColor.withOpacity(0.1),
                            borderRadius: BorderRadius.circular(6),
                            border: Border.all(
                              color: isOutOfStock 
                                  ? (isDarkMode ? Colors.grey[600]! : Colors.grey[400]!)
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
                                        ? (isDarkMode ? Colors.grey[400] : Colors.grey[600])
                                        : Theme.of(context).primaryColor,
                                    fontWeight: FontWeight.bold,
                                    fontSize: isTablet ? 16 : 14,
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
                                    size: isTablet ? 14 : 12,
                                    color: isOutOfStock 
                                        ? (isDarkMode ? Colors.grey[400] : Colors.grey[600])
                                        : isLowStock 
                                            ? Colors.orange[700]
                                            : Colors.green[700],
                                  ),
                                  const SizedBox(width: 2),
                                  Text(
                                    '${product.stock}',
                                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                      color: isOutOfStock 
                                          ? (isDarkMode ? Colors.grey[400] : Colors.grey[600])
                                          : isLowStock 
                                              ? Colors.orange[700]
                                              : Colors.green[700],
                                      fontWeight: FontWeight.w600,
                                      fontSize: isTablet ? 12 : 11,
                                    ),
                                  ),
                                ],
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          ),
        );
      },
    );
  }
}