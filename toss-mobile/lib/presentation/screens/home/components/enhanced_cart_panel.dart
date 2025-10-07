import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';

import '../../../../app/themes/app_sizes.dart';
import '../../../../app/utilities/currency_formatter.dart';
import '../../../providers/home/home_provider.dart';
import '../../../widgets/app_empty_state.dart';
import 'enhanced_cart_item.dart';

class EnhancedCartPanel extends StatefulWidget {
  const EnhancedCartPanel({super.key});

  @override
  State<EnhancedCartPanel> createState() => _EnhancedCartPanelState();
}

class _EnhancedCartPanelState extends State<EnhancedCartPanel> with TickerProviderStateMixin {
  late AnimationController _headerAnimationController;
  late Animation<double> _headerAnimation;
  
  bool _showBulkActions = false;
  final Set<int> _selectedItems = {};

  @override
  void initState() {
    super.initState();
    
    _headerAnimationController = AnimationController(
      duration: const Duration(milliseconds: 300),
      vsync: this,
    );
    
    _headerAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _headerAnimationController,
      curve: Curves.easeInOut,
    ));
  }

  @override
  void dispose() {
    _headerAnimationController.dispose();
    super.dispose();
  }

  void _toggleBulkActions() {
    setState(() {
      _showBulkActions = !_showBulkActions;
      _selectedItems.clear();
    });
    
    if (_showBulkActions) {
      _headerAnimationController.forward();
    } else {
      _headerAnimationController.reverse();
    }
    
    HapticFeedback.selectionClick();
  }

  void _selectAllItems(HomeProvider provider) {
    setState(() {
      if (_selectedItems.length == provider.orderedProducts.length) {
        _selectedItems.clear();
      } else {
        _selectedItems.clear();
        for (int i = 0; i < provider.orderedProducts.length; i++) {
          _selectedItems.add(i);
        }
      }
    });
    HapticFeedback.selectionClick();
  }

  void _removeSelectedItems(HomeProvider provider) {
    if (_selectedItems.isEmpty) return;
    
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Remove Selected Items'),
        content: Text('Remove ${_selectedItems.length} item(s) from cart?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              Navigator.of(context).pop();
              
              // Remove items in reverse order to maintain indices
              final sortedIndices = _selectedItems.toList()..sort((a, b) => b.compareTo(a));
              for (final index in sortedIndices) {
                if (index < provider.orderedProducts.length) {
                  provider.onRemoveOrderedProduct(provider.orderedProducts[index]);
                }
              }
              
              setState(() {
                _selectedItems.clear();
                _showBulkActions = false;
              });
              _headerAnimationController.reverse();
              
              HapticFeedback.mediumImpact();
              ScaffoldMessenger.of(context).showSnackBar(
                SnackBar(
                  content: Text('Removed ${sortedIndices.length} item(s) from cart'),
                  backgroundColor: Colors.green,
                ),
              );
            },
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: const Text('Remove'),
          ),
        ],
      ),
    );
  }

  void _clearCart(HomeProvider provider) {
    if (provider.orderedProducts.isEmpty) return;
    
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Clear Cart'),
        content: const Text('Remove all items from cart?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              Navigator.of(context).pop();
              provider.onClearCart();
              
              setState(() {
                _selectedItems.clear();
                _showBulkActions = false;
              });
              _headerAnimationController.reverse();
              
              HapticFeedback.heavyImpact();
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('Cart cleared'),
                  backgroundColor: Colors.orange,
                ),
              );
            },
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: const Text('Clear All'),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        return Container(
          height: constraints.maxHeight,
          padding: const EdgeInsets.only(top: 8),
          child: Column(
            children: [
              _buildCartHeader(),
              Expanded(
                child: _buildCartItems(),
              ),
              _buildCartSummary(),
            ],
          ),
        );
      },
    );
  }

  Widget _buildCartHeader() {
    return Consumer<HomeProvider>(
      builder: (context, provider, _) {
        if (provider.orderedProducts.isEmpty) return const SizedBox.shrink();
        
        return AnimatedBuilder(
          animation: _headerAnimation,
          builder: (context, child) {
            return Container(
              padding: const EdgeInsets.symmetric(
                horizontal: AppSizes.padding,
                vertical: AppSizes.padding / 2,
              ),
              child: Column(
                children: [
                  // Main Header Row
                  Row(
                    children: [
                      Text(
                        'Cart (${provider.orderedProducts.length})',
                        style: Theme.of(context).textTheme.titleMedium?.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const Spacer(),
                      
                      // Bulk Actions Toggle
                      Material(
                        color: Colors.transparent,
                        child: InkWell(
                          onTap: _toggleBulkActions,
                          borderRadius: BorderRadius.circular(20),
                          child: Container(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 12,
                              vertical: 6,
                            ),
                            decoration: BoxDecoration(
                              color: _showBulkActions
                                ? Theme.of(context).colorScheme.primary.withOpacity(0.1)
                                : Theme.of(context).colorScheme.surfaceVariant.withOpacity(0.5),
                              borderRadius: BorderRadius.circular(20),
                            ),
                            child: Row(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Icon(
                                  _showBulkActions ? Icons.close : Icons.checklist,
                                  size: 16,
                                  color: _showBulkActions
                                    ? Theme.of(context).colorScheme.primary
                                    : Theme.of(context).colorScheme.onSurfaceVariant,
                                ),
                                const SizedBox(width: 4),
                                Text(
                                  _showBulkActions ? 'Done' : 'Select',
                                  style: Theme.of(context).textTheme.labelSmall?.copyWith(
                                    color: _showBulkActions
                                      ? Theme.of(context).colorScheme.primary
                                      : Theme.of(context).colorScheme.onSurfaceVariant,
                                    fontWeight: FontWeight.w500,
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ),
                      ),
                      
                      const SizedBox(width: 8),
                      
                      // Clear Cart Button
                      Material(
                        color: Colors.transparent,
                        child: InkWell(
                          onTap: () => _clearCart(provider),
                          borderRadius: BorderRadius.circular(20),
                          child: Container(
                            padding: const EdgeInsets.all(8),
                            decoration: BoxDecoration(
                              color: Colors.red.withOpacity(0.1),
                              borderRadius: BorderRadius.circular(20),
                            ),
                            child: Icon(
                              Icons.delete_outline,
                              size: 16,
                              color: Colors.red,
                            ),
                          ),
                        ),
                      ),
                    ],
                  ),
                  
                  // Bulk Actions Row
                  if (_showBulkActions)
                    Transform.translate(
                      offset: Offset(0, _headerAnimation.value * 0),
                      child: Opacity(
                        opacity: _headerAnimation.value,
                        child: Container(
                          margin: const EdgeInsets.only(top: 8),
                          child: Row(
                            children: [
                              // Select All
                              Material(
                                color: Colors.transparent,
                                child: InkWell(
                                  onTap: () => _selectAllItems(provider),
                                  borderRadius: BorderRadius.circular(16),
                                  child: Container(
                                    padding: const EdgeInsets.symmetric(
                                      horizontal: 8,
                                      vertical: 4,
                                    ),
                                    decoration: BoxDecoration(
                                      color: Theme.of(context).colorScheme.surfaceVariant.withOpacity(0.5),
                                      borderRadius: BorderRadius.circular(16),
                                    ),
                                    child: Row(
                                      mainAxisSize: MainAxisSize.min,
                                      children: [
                                        Icon(
                                          _selectedItems.length == provider.orderedProducts.length
                                            ? Icons.check_box
                                            : Icons.check_box_outline_blank,
                                          size: 16,
                                          color: Theme.of(context).colorScheme.primary,
                                        ),
                                        const SizedBox(width: 4),
                                        Text(
                                          'Select All',
                                          style: Theme.of(context).textTheme.labelSmall,
                                        ),
                                      ],
                                    ),
                                  ),
                                ),
                              ),
                              
                              const SizedBox(width: 8),
                              
                              // Selected Count
                              if (_selectedItems.isNotEmpty)
                                Container(
                                  padding: const EdgeInsets.symmetric(
                                    horizontal: 8,
                                    vertical: 4,
                                  ),
                                  decoration: BoxDecoration(
                                    color: Theme.of(context).colorScheme.primary.withOpacity(0.1),
                                    borderRadius: BorderRadius.circular(16),
                                  ),
                                  child: Text(
                                    '${_selectedItems.length} selected',
                                    style: Theme.of(context).textTheme.labelSmall?.copyWith(
                                      color: Theme.of(context).colorScheme.primary,
                                      fontWeight: FontWeight.w500,
                                    ),
                                  ),
                                ),
                              
                              const Spacer(),
                              
                              // Remove Selected
                              if (_selectedItems.isNotEmpty)
                                Material(
                                  color: Colors.transparent,
                                  child: InkWell(
                                    onTap: () => _removeSelectedItems(provider),
                                    borderRadius: BorderRadius.circular(16),
                                    child: Container(
                                      padding: const EdgeInsets.symmetric(
                                        horizontal: 8,
                                        vertical: 4,
                                      ),
                                      decoration: BoxDecoration(
                                        color: Colors.red.withOpacity(0.1),
                                        borderRadius: BorderRadius.circular(16),
                                      ),
                                      child: Row(
                                        mainAxisSize: MainAxisSize.min,
                                        children: [
                                          Icon(
                                            Icons.delete_outline,
                                            size: 16,
                                            color: Colors.red,
                                          ),
                                          const SizedBox(width: 4),
                                          Text(
                                            'Remove',
                                            style: Theme.of(context).textTheme.labelSmall?.copyWith(
                                              color: Colors.red,
                                              fontWeight: FontWeight.w500,
                                            ),
                                          ),
                                        ],
                                      ),
                                    ),
                                  ),
                                ),
                            ],
                          ),
                        ),
                      ),
                    ),
                ],
              ),
            );
          },
        );
      },
    );
  }

  Widget _buildCartItems() {
    return Consumer<HomeProvider>(
      builder: (context, provider, _) {
        if (provider.orderedProducts.isEmpty) {
          return const Center(
            child: AppEmptyState(
              title: 'Empty Cart',
              subtitle: 'Add products to get started',
            ),
          );
        }

        return Scrollbar(
          child: ListView.builder(
            itemCount: provider.orderedProducts.length,
            padding: const EdgeInsets.symmetric(horizontal: AppSizes.padding),
            itemBuilder: (context, i) {
              final isSelected = _selectedItems.contains(i);
              
              return GestureDetector(
                onTap: _showBulkActions ? () {
                  setState(() {
                    if (isSelected) {
                      _selectedItems.remove(i);
                    } else {
                      _selectedItems.add(i);
                    }
                  });
                  HapticFeedback.selectionClick();
                } : null,
                child: Stack(
                  children: [
                    AnimatedContainer(
                      duration: const Duration(milliseconds: 200),
                      decoration: BoxDecoration(
                        borderRadius: BorderRadius.circular(AppSizes.radius),
                        border: _showBulkActions && isSelected 
                          ? Border.all(
                              color: Theme.of(context).colorScheme.primary,
                              width: 2,
                            )
                          : null,
                      ),
                      child: EnhancedCartItem(
                        orderedProduct: provider.orderedProducts[i],
                        onQuantityChanged: (quantity) {
                          provider.onChangedOrderedProductQuantity(i, quantity);
                        },
                        onRemove: () {
                          provider.onRemoveOrderedProduct(provider.orderedProducts[i]);
                        },
                        allowEdit: !_showBulkActions,
                      ),
                    ),
                    
                    // Selection Indicator
                    if (_showBulkActions)
                      Positioned(
                        top: 8,
                        right: 8,
                        child: AnimatedScale(
                          scale: isSelected ? 1.0 : 0.8,
                          duration: const Duration(milliseconds: 200),
                          child: Container(
                            width: 24,
                            height: 24,
                            decoration: BoxDecoration(
                              color: isSelected 
                                ? Theme.of(context).colorScheme.primary
                                : Theme.of(context).colorScheme.surface,
                              border: Border.all(
                                color: isSelected 
                                  ? Theme.of(context).colorScheme.primary
                                  : Theme.of(context).colorScheme.outline,
                                width: 2,
                              ),
                              borderRadius: BorderRadius.circular(12),
                            ),
                            child: isSelected 
                              ? Icon(
                                  Icons.check,
                                  size: 16,
                                  color: Theme.of(context).colorScheme.onPrimary,
                                )
                              : null,
                          ),
                        ),
                      ),
                  ],
                ),
              );
            },
          ),
        );
      },
    );
  }

  Widget _buildCartSummary() {
    return Consumer<HomeProvider>(
      builder: (context, provider, _) {
        if (provider.orderedProducts.isEmpty) return const SizedBox.shrink();
        
        return Container(
          padding: const EdgeInsets.fromLTRB(AppSizes.padding, 8, AppSizes.padding, 12),
          decoration: BoxDecoration(
            border: Border(
              top: BorderSide(
                width: 1,
                color: Theme.of(context).colorScheme.surfaceContainer,
              ),
            ),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              // AI Suggestion Banner
              if (provider.getUpsellSuggestion() != null) ...[
                Container(
                  width: double.infinity,
                  padding: const EdgeInsets.all(8),
                  margin: const EdgeInsets.only(bottom: 6),
                  decoration: BoxDecoration(
                    gradient: LinearGradient(
                      colors: [
                        Theme.of(context).colorScheme.primaryContainer.withOpacity(0.7),
                        Theme.of(context).colorScheme.secondaryContainer.withOpacity(0.7),
                      ],
                      begin: Alignment.topLeft,
                      end: Alignment.bottomRight,
                    ),
                    borderRadius: BorderRadius.circular(AppSizes.radius),
                  ),
                  child: Row(
                    children: [
                      Container(
                        padding: const EdgeInsets.all(3),
                        decoration: BoxDecoration(
                          color: Theme.of(context).colorScheme.primary.withOpacity(0.1),
                          borderRadius: BorderRadius.circular(12),
                        ),
                        child: Icon(
                          Icons.lightbulb_outline,
                          size: 12,
                          color: Theme.of(context).colorScheme.primary,
                        ),
                      ),
                      const SizedBox(width: 6),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              'Smart Suggestion',
                              style: Theme.of(context).textTheme.labelSmall?.copyWith(
                                fontWeight: FontWeight.bold,
                                color: Theme.of(context).colorScheme.primary,
                                fontSize: 10,
                              ),
                            ),
                            Text(
                              provider.getUpsellSuggestion()!,
                              style: Theme.of(context).textTheme.labelSmall?.copyWith(fontSize: 10),
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis,
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ],
              
              // Summary Rows
              _buildSummaryRow(
                context,
                'Subtotal (${provider.orderedProducts.length} items)',
                CurrencyFormatter.format(provider.getTotalAmount()),
                isBold: true,
              ),
              
              if (provider.discountPercent != null || provider.discountAmount > 0) ...[
                const SizedBox(height: 1),
                _buildSummaryRow(
                  context,
                  'Discount',
                  provider.discountPercent != null
                      ? '-${provider.discountPercent!.toStringAsFixed(0)}%'
                      : '-${CurrencyFormatter.format(provider.discountAmount)}',
                  color: Colors.green,
                ),
              ],
              
              if (provider.taxPercent != null && provider.taxPercent! > 0) ...[
                const SizedBox(height: 1),
                _buildSummaryRow(
                  context,
                  'Tax (${provider.taxPercent!.toStringAsFixed(0)}%)',
                  '+${CurrencyFormatter.format(provider.getTaxAmount())}',
                  color: Theme.of(context).colorScheme.outline,
                ),
              ],
              
              const SizedBox(height: 4),
              Container(
                height: 1,
                color: Theme.of(context).colorScheme.outline.withOpacity(0.2),
              ),
              const SizedBox(height: 4),
              
              _buildSummaryRow(
                context,
                'Total Payable',
                CurrencyFormatter.format(provider.getFinalTotalAmount()),
                isBold: true,
                isLarge: true,
                color: Theme.of(context).colorScheme.primary,
              ),
            ],
          ),
        );
      },
    );
  }

  Widget _buildSummaryRow(
    BuildContext context,
    String label,
    String value, {
    bool isBold = false,
    bool isLarge = false,
    Color? color,
  }) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(
          label,
          style: isLarge 
            ? Theme.of(context).textTheme.titleSmall?.copyWith(
                fontWeight: isBold ? FontWeight.bold : FontWeight.normal,
                color: color,
              )
            : Theme.of(context).textTheme.bodyMedium?.copyWith(
                fontWeight: isBold ? FontWeight.bold : FontWeight.normal,
                color: color,
              ),
        ),
        Text(
          value,
          style: isLarge 
            ? Theme.of(context).textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
                color: color ?? Theme.of(context).colorScheme.primary,
              )
            : Theme.of(context).textTheme.bodyMedium?.copyWith(
                fontWeight: isBold ? FontWeight.bold : FontWeight.normal,
                color: color,
              ),
        ),
      ],
    );
  }
}
