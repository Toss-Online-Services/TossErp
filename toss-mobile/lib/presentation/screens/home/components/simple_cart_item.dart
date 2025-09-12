import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../app/themes/app_sizes.dart';
import '../../../../app/utilities/currency_formatter.dart';
import '../../../../domain/entities/ordered_product_entity.dart';
import '../../../widgets/app_image.dart';

class SimpleCartItem extends StatelessWidget {
  final OrderedProductEntity orderedProduct;
  final Function(int) onQuantityChanged;
  final VoidCallback onRemove;
  final bool showStockWarning;
  final bool allowEdit;

  const SimpleCartItem({
    super.key,
    required this.orderedProduct,
    required this.onQuantityChanged,
    required this.onRemove,
    this.showStockWarning = true,
    this.allowEdit = true,
  });

  void _updateQuantity(int newQuantity) {
    if (newQuantity <= 0) {
      onRemove();
    } else {
      onQuantityChanged(newQuantity);
    }
  }

  @override
  Widget build(BuildContext context) {
    final isOutOfStock = orderedProduct.quantity > orderedProduct.stock;
    
    return Card(
      margin: const EdgeInsets.symmetric(
        horizontal: AppSizes.padding / 2,
        vertical: AppSizes.padding / 4,
      ),
      elevation: 1,
      child: Padding(
        padding: const EdgeInsets.all(AppSizes.padding / 2),
        child: Row(
          children: [
            // Product Image
            ClipRRect(
              borderRadius: BorderRadius.circular(AppSizes.radius / 2),
              child: AppImage(
                image: orderedProduct.imageUrl,
                width: 48,
                height: 48,
                backgroundColor: Theme.of(context).colorScheme.surfaceVariant,
                errorWidget: Container(
                  width: 48,
                  height: 48,
                  color: Theme.of(context).colorScheme.surfaceVariant,
                  child: Icon(
                    Icons.inventory_2_outlined,
                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                    size: 20,
                  ),
                ),
              ),
            ),
            
            const SizedBox(width: AppSizes.padding / 2),
            
            // Product Details
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(
                    orderedProduct.name,
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  
                  const SizedBox(height: 2),
                  
                  Row(
                    children: [
                      Text(
                        CurrencyFormatter.format(orderedProduct.price),
                        style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: Theme.of(context).colorScheme.primary,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                      
                      if (showStockWarning && isOutOfStock) ...[
                        const SizedBox(width: 8),
                        Container(
                          padding: const EdgeInsets.symmetric(
                            horizontal: 6,
                            vertical: 2,
                          ),
                          decoration: BoxDecoration(
                            color: Colors.orange.withOpacity(0.1),
                            borderRadius: BorderRadius.circular(4),
                          ),
                          child: Text(
                            'Low Stock',
                            style: Theme.of(context).textTheme.bodySmall?.copyWith(
                              color: Colors.orange.shade700,
                              fontSize: 10,
                            ),
                          ),
                        ),
                      ],
                    ],
                  ),
                  
                  const SizedBox(height: 4),
                  
                  Text(
                    'Total: ${CurrencyFormatter.format(orderedProduct.price * orderedProduct.quantity)}',
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      fontWeight: FontWeight.w600,
                      color: Theme.of(context).colorScheme.onSurface,
                    ),
                  ),
                ],
              ),
            ),
            
            const SizedBox(width: AppSizes.padding / 2),
            
            // Quantity Controls
            Container(
              decoration: BoxDecoration(
                color: Theme.of(context).colorScheme.surfaceVariant.withOpacity(0.3),
                borderRadius: BorderRadius.circular(16),
              ),
              child: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  // Decrease Button
                  IconButton(
                    onPressed: allowEdit ? () => _updateQuantity(orderedProduct.quantity - 1) : null,
                    icon: Icon(
                      orderedProduct.quantity <= 1 ? Icons.delete_outline : Icons.remove,
                      size: 18,
                      color: orderedProduct.quantity <= 1 
                        ? Colors.red 
                        : Theme.of(context).colorScheme.primary,
                    ),
                    padding: const EdgeInsets.all(4),
                    constraints: const BoxConstraints(
                      minWidth: 32,
                      minHeight: 32,
                    ),
                  ),
                  
                  // Quantity Display
                  Container(
                    constraints: const BoxConstraints(minWidth: 24),
                    child: Text(
                      orderedProduct.quantity.toString(),
                      textAlign: TextAlign.center,
                      style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                  
                  // Increase Button
                  IconButton(
                    onPressed: allowEdit && !isOutOfStock 
                      ? () => _updateQuantity(orderedProduct.quantity + 1) 
                      : null,
                    icon: Icon(
                      Icons.add,
                      size: 18,
                      color: isOutOfStock
                        ? Colors.grey
                        : Theme.of(context).colorScheme.primary,
                    ),
                    padding: const EdgeInsets.all(4),
                    constraints: const BoxConstraints(
                      minWidth: 32,
                      minHeight: 32,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
