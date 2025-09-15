import '../../../widgets/app_image.dart';
import 'package:flutter/material.dart';

import '../../../../app/themes/app_sizes.dart';
import '../../../../app/utilities/currency_formatter.dart';
import '../../../../domain/entities/product_entity.dart';

class ProductsCard extends StatefulWidget {
  final ProductEntity product;
  final Function()? onTap;
  final bool isSelected;
  final bool isSelectionMode;

  const ProductsCard({
    super.key,
    required this.product,
    this.onTap,
    this.isSelected = false,
    this.isSelectionMode = false,
  });

  @override
  State<ProductsCard> createState() => _ProductsCardState();
}

class _ProductsCardState extends State<ProductsCard> {
  bool _pressed = false;

  @override
  Widget build(BuildContext context) {
    final product = widget.product;
    final primary = Theme.of(context).colorScheme.primary;
    return GestureDetector(
      onTapDown: (_) => setState(() => _pressed = true),
      onTapCancel: () => setState(() => _pressed = false),
      onTapUp: (_) => setState(() => _pressed = false),
      onTap: widget.onTap,
      child: AnimatedScale(
        scale: _pressed ? 0.98 : 1.0,
        duration: const Duration(milliseconds: 130),
        curve: Curves.easeOut,
        child: AnimatedContainer(
          duration: const Duration(milliseconds: 200),
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            color: Theme.of(context).colorScheme.surface,
            borderRadius: BorderRadius.circular(12),
            boxShadow: [
              BoxShadow(
                color: primary.withValues(alpha: _pressed ? 0.10 : 0.06),
                blurRadius: _pressed ? 18 : 12,
                offset: const Offset(0, 6),
              )
            ],
            border: Border.all(
              width: 0.6,
              color: Theme.of(context).colorScheme.surfaceContainerHighest,
            ),
            gradient: LinearGradient(
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
              colors: [
                Theme.of(context).colorScheme.surface,
                Theme.of(context).colorScheme.surfaceContainerLowest,
              ],
            ),
          ),
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 220, maxHeight: 260),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Stack(
                  children: [
                    AspectRatio(
                      aspectRatio: 1,
                      child: Hero(
                        tag: 'item_${product.id}',
                        child: AppImage(
                          image: product.imageUrl,
                          borderRadius: BorderRadius.circular(8),
                          border: Border.all(width: 0.5, color: Theme.of(context).colorScheme.surfaceContainerHighest),
                          backgroundColor: Theme.of(context).colorScheme.surfaceContainerLowest,
                          errorWidget: Icon(
                            Icons.image,
                            color: Theme.of(context).colorScheme.surfaceDim,
                            size: 32,
                          ),
                        ),
                      ),
                    ),
                    // Out of stock badge overlay
                    if (product.stock == 0)
                      Positioned.fill(
                        child: Container(
                          alignment: Alignment.center,
                          decoration: BoxDecoration(
                            color: Colors.white70,
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: Container(
                            padding: const EdgeInsets.symmetric(
                              vertical: AppSizes.padding / 4,
                              horizontal: AppSizes.padding / 2,
                            ),
                            decoration: BoxDecoration(
                              color: Theme.of(context).colorScheme.surfaceContainerLowest,
                              borderRadius: BorderRadius.circular(6),
                            ),
                            child: Row(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Icon(
                                  Icons.remove_circle,
                                  color: Theme.of(context).colorScheme.outline,
                                  size: 12,
                                ),
                                const SizedBox(width: 6),
                                Text(
                                  'Out of stock',
                                  overflow: TextOverflow.ellipsis,
                                  style: Theme.of(context).textTheme.labelSmall?.copyWith(
                                        color: Theme.of(context).colorScheme.outline,
                                        fontWeight: FontWeight.bold,
                                      ),
                                ),
                              ],
                            ),
                          ),
                        ),
                      ),
                  ],
                ),
                const SizedBox(height: 10),
                Text(
                  product.name,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                  style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                        fontWeight: FontWeight.w700,
                      ),
                ),
                const SizedBox(height: 8),
                Row(
                  children: [
                    Icon(
                      Icons.inventory_2,
                      size: 10,
                      color: Theme.of(context).colorScheme.outline,
                    ),
                    const SizedBox(width: 6),
                    Text(
                      'Stock ${product.stock}  |  Sold ${product.sold}',
                      style: Theme.of(context).textTheme.labelSmall,
                    ),
                  ],
                ),
                const Spacer(),
                Align(
                  alignment: Alignment.bottomRight,
                  child: AnimatedContainer(
                    duration: const Duration(milliseconds: 200),
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(
                      gradient: LinearGradient(colors: [primary, primary.withValues(alpha: 0.7)]),
                      borderRadius: BorderRadius.circular(20),
                      boxShadow: [
                        BoxShadow(color: primary.withValues(alpha: 0.18), blurRadius: 10, offset: const Offset(0, 6)),
                      ],
                    ),
                    child: Text(
                      CurrencyFormatter.format(product.price),
                      style: Theme.of(context).textTheme.labelMedium?.copyWith(color: Colors.white, fontWeight: FontWeight.bold),
                    ),
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


