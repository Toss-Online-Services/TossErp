import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../app/themes/app_sizes.dart';
import '../../../../app/utilities/currency_formatter.dart';
import '../../../../domain/entities/ordered_product_entity.dart';
import '../../../widgets/app_image.dart';

class EnhancedCartItem extends StatefulWidget {
  final OrderedProductEntity orderedProduct;
  final Function(int) onQuantityChanged;
  final VoidCallback onRemove;
  final bool showStockWarning;
  final bool allowEdit;

  const EnhancedCartItem({
    super.key,
    required this.orderedProduct,
    required this.onQuantityChanged,
    required this.onRemove,
    this.showStockWarning = true,
    this.allowEdit = true,
  });

  @override
  State<EnhancedCartItem> createState() => _EnhancedCartItemState();
}

class _EnhancedCartItemState extends State<EnhancedCartItem> with TickerProviderStateMixin {
  late AnimationController _scaleController;
  late AnimationController _slideController;
  late Animation<double> _scaleAnimation;
  late Animation<Offset> _slideAnimation;

  final TextEditingController _quantityController = TextEditingController();
  bool _isEditingQuantity = false;

  @override
  void initState() {
    super.initState();
    
    _scaleController = AnimationController(
      duration: const Duration(milliseconds: 200),
      vsync: this,
    );
    
    _slideController = AnimationController(
      duration: const Duration(milliseconds: 300),
      vsync: this,
    );

    _scaleAnimation = Tween<double>(
      begin: 1.0,
      end: 0.95,
    ).animate(CurvedAnimation(
      parent: _scaleController,
      curve: Curves.easeInOut,
    ));

    _slideAnimation = Tween<Offset>(
      begin: Offset.zero,
      end: const Offset(-0.1, 0),
    ).animate(CurvedAnimation(
      parent: _slideController,
      curve: Curves.easeInOut,
    ));

    _quantityController.text = widget.orderedProduct.quantity.toString();
  }

  @override
  void dispose() {
    _scaleController.dispose();
    _slideController.dispose();
    _quantityController.dispose();
    super.dispose();
  }

  void _handleQuantityIncrement() {
    if (widget.orderedProduct.quantity < widget.orderedProduct.stock) {
      _scaleController.forward().then((_) => _scaleController.reverse());
      widget.onQuantityChanged(widget.orderedProduct.quantity + 1);
      _quantityController.text = (widget.orderedProduct.quantity + 1).toString();
      HapticFeedback.lightImpact();
    } else {
      HapticFeedback.heavyImpact();
      _showStockLimitMessage();
    }
  }

  void _handleQuantityDecrement() {
    if (widget.orderedProduct.quantity > 1) {
      _scaleController.forward().then((_) => _scaleController.reverse());
      widget.onQuantityChanged(widget.orderedProduct.quantity - 1);
      _quantityController.text = (widget.orderedProduct.quantity - 1).toString();
      HapticFeedback.lightImpact();
    } else {
      // If quantity is 1, show remove confirmation
      _showRemoveConfirmation();
    }
  }

  void _handleQuantityEdit() {
    setState(() => _isEditingQuantity = true);
  }

  void _handleQuantitySubmit() {
    final newQuantity = int.tryParse(_quantityController.text) ?? widget.orderedProduct.quantity;
    
    if (newQuantity <= 0) {
      _showRemoveConfirmation();
      return;
    }
    
    if (newQuantity > widget.orderedProduct.stock) {
      _showStockLimitMessage();
      _quantityController.text = widget.orderedProduct.quantity.toString();
      setState(() => _isEditingQuantity = false);
      return;
    }
    
    widget.onQuantityChanged(newQuantity);
    setState(() => _isEditingQuantity = false);
    HapticFeedback.selectionClick();
  }

  void _showStockLimitMessage() {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Only ${widget.orderedProduct.stock} items available in stock'),
        backgroundColor: Colors.orange,
        duration: const Duration(seconds: 2),
      ),
    );
  }

  void _showRemoveConfirmation() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Remove Item'),
        content: Text('Remove "${widget.orderedProduct.name}" from cart?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              Navigator.of(context).pop();
              widget.onRemove();
              HapticFeedback.mediumImpact();
            },
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: const Text('Remove'),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final isLowStock = widget.orderedProduct.quantity >= widget.orderedProduct.stock * 0.8;
    final isOutOfStock = widget.orderedProduct.quantity >= widget.orderedProduct.stock;
    final itemTotal = widget.orderedProduct.price * widget.orderedProduct.quantity;

    return AnimatedBuilder(
      animation: Listenable.merge([_scaleAnimation, _slideAnimation]),
      builder: (context, child) {
        return Transform.scale(
          scale: _scaleAnimation.value,
          child: SlideTransition(
            position: _slideAnimation,
            child: Container(
              margin: const EdgeInsets.only(bottom: AppSizes.padding / 2),
              decoration: BoxDecoration(
                color: Theme.of(context).colorScheme.surface,
                borderRadius: BorderRadius.circular(AppSizes.radius),
                border: Border.all(
                  color: isOutOfStock 
                    ? Colors.red.withOpacity(0.3)
                    : isLowStock 
                      ? Colors.orange.withOpacity(0.3)
                      : Theme.of(context).colorScheme.outline.withOpacity(0.1),
                  width: 1,
                ),
                boxShadow: [
                  BoxShadow(
                    color: Theme.of(context).colorScheme.shadow.withOpacity(0.05),
                    offset: const Offset(0, 2),
                    blurRadius: 4,
                  ),
                ],
              ),
              child: ClipRRect(
                borderRadius: BorderRadius.circular(AppSizes.radius),
                child: Material(
                  color: Colors.transparent,
                  child: InkWell(
                    onTap: widget.allowEdit ? _handleQuantityEdit : null,
                    child: Padding(
                      padding: const EdgeInsets.all(AppSizes.padding / 2),
                      child: Row(
                        children: [
                          // Product Image
                          Hero(
                            tag: 'product-${widget.orderedProduct.productId}',
                            child: AppImage(
                              image: widget.orderedProduct.imageUrl ?? '',
                              width: 56,
                              height: 56,
                              borderRadius: BorderRadius.circular(AppSizes.radius / 2),
                              backgroundColor: Theme.of(context).colorScheme.surfaceVariant,
                              errorWidget: Container(
                                width: 56,
                                height: 56,
                                decoration: BoxDecoration(
                                  color: Theme.of(context).colorScheme.surfaceVariant,
                                  borderRadius: BorderRadius.circular(AppSizes.radius / 2),
                                ),
                                child: Icon(
                                  Icons.inventory_2_outlined,
                                  color: Theme.of(context).colorScheme.onSurfaceVariant,
                                  size: 24,
                                ),
                              ),
                            ),
                          ),
                          const SizedBox(width: AppSizes.padding / 2),
                          
                          // Product Details
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  widget.orderedProduct.name,
                                  style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                                    fontWeight: FontWeight.w600,
                                  ),
                                  maxLines: 2,
                                  overflow: TextOverflow.ellipsis,
                                ),
                                const SizedBox(height: 2),
                                Row(
                                  children: [
                                    Text(
                                      CurrencyFormatter.format(widget.orderedProduct.price),
                                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                        color: Theme.of(context).colorScheme.primary,
                                        fontWeight: FontWeight.w500,
                                      ),
                                    ),
                                    Text(
                                      ' each',
                                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                        color: Theme.of(context).colorScheme.outline,
                                      ),
                                    ),
                                  ],
                                ),
                                if (widget.showStockWarning && (isLowStock || isOutOfStock))
                                  Padding(
                                    padding: const EdgeInsets.only(top: 2),
                                    child: Row(
                                      children: [
                                        Icon(
                                          isOutOfStock ? Icons.error : Icons.warning,
                                          size: 12,
                                          color: isOutOfStock ? Colors.red : Colors.orange,
                                        ),
                                        const SizedBox(width: 4),
                                        Text(
                                          isOutOfStock 
                                            ? 'Out of stock' 
                                            : 'Low stock (${widget.orderedProduct.stock} left)',
                                          style: Theme.of(context).textTheme.labelSmall?.copyWith(
                                            color: isOutOfStock ? Colors.red : Colors.orange,
                                            fontWeight: FontWeight.w500,
                                          ),
                                        ),
                                      ],
                                    ),
                                  ),
                              ],
                            ),
                          ),
                          
                          // Quantity Controls
                          Column(
                            crossAxisAlignment: CrossAxisAlignment.end,
                            children: [
                              // Total Price
                              Text(
                                CurrencyFormatter.format(itemTotal),
                                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                                  fontWeight: FontWeight.bold,
                                  color: Theme.of(context).colorScheme.primary,
                                ),
                              ),
                              const SizedBox(height: 8),
                              
                              // Quantity Controls
                              Container(
                                decoration: BoxDecoration(
                                  color: Theme.of(context).colorScheme.surfaceVariant.withOpacity(0.5),
                                  borderRadius: BorderRadius.circular(20),
                                ),
                                child: Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    // Decrease Button
                                    Material(
                                      color: Colors.transparent,
                                      child: InkWell(
                                        onTap: widget.allowEdit ? _handleQuantityDecrement : null,
                                        borderRadius: BorderRadius.circular(20),
                                        child: Container(
                                          width: 32,
                                          height: 32,
                                          decoration: BoxDecoration(
                                            color: widget.orderedProduct.quantity <= 1 
                                              ? Colors.red.withOpacity(0.1)
                                              : Theme.of(context).colorScheme.primary.withOpacity(0.1),
                                            borderRadius: BorderRadius.circular(20),
                                          ),
                                          child: Icon(
                                            widget.orderedProduct.quantity <= 1 
                                              ? Icons.delete_outline 
                                              : Icons.remove,
                                            size: 16,
                                            color: widget.orderedProduct.quantity <= 1 
                                              ? Colors.red 
                                              : Theme.of(context).colorScheme.primary,
                                          ),
                                        ),
                                      ),
                                    ),
                                    
                                    // Quantity Display/Edit
                                    Container(
                                      width: 48,
                                      height: 32,
                                      alignment: Alignment.center,
                                      child: _isEditingQuantity ? 
                                        TextField(
                                          controller: _quantityController,
                                          keyboardType: TextInputType.number,
                                          textAlign: TextAlign.center,
                                          style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                            fontWeight: FontWeight.bold,
                                          ),
                                          decoration: const InputDecoration(
                                            border: InputBorder.none,
                                            contentPadding: EdgeInsets.zero,
                                          ),
                                          onSubmitted: (_) => _handleQuantitySubmit(),
                                          onEditingComplete: _handleQuantitySubmit,
                                          autofocus: true,
                                        ) :
                                        GestureDetector(
                                          onTap: widget.allowEdit ? _handleQuantityEdit : null,
                                          child: Text(
                                            widget.orderedProduct.quantity.toString(),
                                            style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                              fontWeight: FontWeight.bold,
                                            ),
                                          ),
                                        ),
                                    ),
                                    
                                    // Increase Button
                                    Material(
                                      color: Colors.transparent,
                                      child: InkWell(
                                        onTap: widget.allowEdit ? _handleQuantityIncrement : null,
                                        borderRadius: BorderRadius.circular(20),
                                        child: Container(
                                          width: 32,
                                          height: 32,
                                          decoration: BoxDecoration(
                                            color: isOutOfStock
                                              ? Colors.grey.withOpacity(0.1)
                                              : Theme.of(context).colorScheme.primary.withOpacity(0.1),
                                            borderRadius: BorderRadius.circular(20),
                                          ),
                                          child: Icon(
                                            Icons.add,
                                            size: 16,
                                            color: isOutOfStock
                                              ? Colors.grey
                                              : Theme.of(context).colorScheme.primary,
                                          ),
                                        ),
                                      ),
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
              ),
            ),
          ),
        );
      },
    );
  }
}
