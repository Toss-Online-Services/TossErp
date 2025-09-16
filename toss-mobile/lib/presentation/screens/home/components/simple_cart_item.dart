import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../../app/themes/app_sizes.dart';
import '../../../../app/utilities/currency_formatter.dart';
import '../../../../domain/entities/ordered_product_entity.dart';
import '../../../widgets/app_image.dart';

class SimpleCartItem extends StatefulWidget {
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

  @override
  State<SimpleCartItem> createState() => _SimpleCartItemState();
}

class _SimpleCartItemState extends State<SimpleCartItem> {
  late TextEditingController _quantityController;
  late FocusNode _quantityFocusNode;
  bool _isEditing = false;

  @override
  void initState() {
    super.initState();
    _quantityController = TextEditingController(text: widget.orderedProduct.quantity.toString());
    _quantityFocusNode = FocusNode();
  }

  @override
  void dispose() {
    _quantityController.dispose();
    _quantityFocusNode.dispose();
    super.dispose();
  }

  void _updateQuantity(int newQuantity) {
    if (newQuantity <= 0) {
      widget.onRemove();
    } else {
      widget.onQuantityChanged(newQuantity);
    }
  }

  void _startEditing() {
    setState(() {
      _isEditing = true;
    });
    _quantityFocusNode.requestFocus();
    _quantityController.selection = TextSelection(
      baseOffset: 0,
      extentOffset: _quantityController.text.length,
    );
  }

  void _finishEditing() {
    setState(() {
      _isEditing = false;
    });
    _quantityFocusNode.unfocus();
    
    final int? newQuantity = int.tryParse(_quantityController.text);
    if (newQuantity != null && newQuantity > 0) {
      if (newQuantity <= widget.orderedProduct.stock) {
        _updateQuantity(newQuantity);
      } else {
        // Exceeds stock, show warning and revert
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Cannot set quantity to $newQuantity. Only ${widget.orderedProduct.stock} in stock'),
            duration: const Duration(seconds: 2),
            backgroundColor: Colors.orange,
            behavior: SnackBarBehavior.floating,
          ),
        );
        _quantityController.text = widget.orderedProduct.quantity.toString();
      }
    } else {
      // Invalid input, revert to current quantity
      _quantityController.text = widget.orderedProduct.quantity.toString();
    }
  }

  @override
  Widget build(BuildContext context) {
    final isOutOfStock = widget.orderedProduct.quantity > widget.orderedProduct.stock;
    
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
                image: widget.orderedProduct.imageUrl,
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
                    widget.orderedProduct.name,
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
                        CurrencyFormatter.format(widget.orderedProduct.price),
                        style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: Theme.of(context).colorScheme.primary,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                      
                      if (widget.showStockWarning && isOutOfStock) ...[
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
                    'Total: ${CurrencyFormatter.format(widget.orderedProduct.price * widget.orderedProduct.quantity)}',
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
                    onPressed: widget.allowEdit ? () => _updateQuantity(widget.orderedProduct.quantity - 1) : null,
                    icon: Icon(
                      widget.orderedProduct.quantity <= 1 ? Icons.delete_outline : Icons.remove,
                      size: 18,
                      color: widget.orderedProduct.quantity <= 1 
                        ? Colors.red 
                        : Theme.of(context).colorScheme.primary,
                    ),
                    padding: const EdgeInsets.all(4),
                    constraints: const BoxConstraints(
                      minWidth: 32,
                      minHeight: 32,
                    ),
                  ),
                  
                  // Editable Quantity Field
                  GestureDetector(
                    onTap: widget.allowEdit ? _startEditing : null,
                    onLongPress: widget.allowEdit ? () {
                      // Long press to remove all
                      showDialog(
                        context: context,
                        builder: (context) => AlertDialog(
                          title: const Text('Remove Item'),
                          content: Text('Remove all ${widget.orderedProduct.name} from cart?'),
                          actions: [
                            TextButton(
                              onPressed: () => Navigator.of(context).pop(),
                              child: const Text('Cancel'),
                            ),
                            TextButton(
                              onPressed: () {
                                Navigator.of(context).pop();
                                widget.onRemove();
                              },
                              style: TextButton.styleFrom(foregroundColor: Colors.red),
                              child: const Text('Remove'),
                            ),
                          ],
                        ),
                      );
                    } : null,
                    child: Container(
                      constraints: const BoxConstraints(minWidth: 40),
                      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                      child: _isEditing 
                        ? SizedBox(
                            width: 50,
                            child: TextField(
                              controller: _quantityController,
                              focusNode: _quantityFocusNode,
                              keyboardType: TextInputType.number,
                              textAlign: TextAlign.center,
                              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                                fontWeight: FontWeight.bold,
                              ),
                              inputFormatters: [
                                FilteringTextInputFormatter.digitsOnly,
                                LengthLimitingTextInputFormatter(3), // Max 999
                              ],
                              decoration: const InputDecoration(
                                border: InputBorder.none,
                                contentPadding: EdgeInsets.zero,
                                isDense: true,
                              ),
                              onSubmitted: (_) => _finishEditing(),
                              onTapOutside: (_) => _finishEditing(),
                            ),
                          )
                        : Container(
                            padding: const EdgeInsets.symmetric(vertical: 4),
                            child: Text(
                              widget.orderedProduct.quantity.toString(),
                              textAlign: TextAlign.center,
                              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ),
                    ),
                  ),
                  
                  // Increase Button
                  IconButton(
                    onPressed: widget.allowEdit && !isOutOfStock 
                      ? () => _updateQuantity(widget.orderedProduct.quantity + 1) 
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
