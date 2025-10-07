import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';

import '../../../../app/themes/app_sizes.dart';
import '../../../../app/utilities/currency_formatter.dart';
import '../../../providers/home/home_provider.dart';

class EnhancedCartHeader extends StatefulWidget {
  const EnhancedCartHeader({super.key});

  @override
  State<EnhancedCartHeader> createState() => _EnhancedCartHeaderState();
}

class _EnhancedCartHeaderState extends State<EnhancedCartHeader> 
    with TickerProviderStateMixin {
  
  late AnimationController _pulseController;
  late AnimationController _slideController;
  late Animation<double> _pulseAnimation;
  late Animation<Offset> _slideAnimation;
  
  @override
  void initState() {
    super.initState();
    
    _pulseController = AnimationController(
      duration: const Duration(milliseconds: 1000),
      vsync: this,
    );
    
    _slideController = AnimationController(
      duration: const Duration(milliseconds: 300),
      vsync: this,
    );
    
    _pulseAnimation = Tween<double>(
      begin: 1.0,
      end: 1.05,
    ).animate(CurvedAnimation(
      parent: _pulseController,
      curve: Curves.easeInOut,
    ));
    
    _slideAnimation = Tween<Offset>(
      begin: const Offset(0, -0.2),
      end: Offset.zero,
    ).animate(CurvedAnimation(
      parent: _slideController,
      curve: Curves.easeOutCubic,
    ));
    
    // Start animations
    _slideController.forward();
    _startPulseAnimation();
  }

  void _startPulseAnimation() {
    _pulseController.repeat(reverse: true);
  }

  @override
  void dispose() {
    _pulseController.dispose();
    _slideController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Consumer<HomeProvider>(
      builder: (context, provider, _) {
        final isCartEmpty = provider.orderedProducts.isEmpty;
        final itemCount = provider.orderedProducts.length;
        final total = provider.getFinalTotalAmount();
        
        return SlideTransition(
          position: _slideAnimation,
          child: LayoutBuilder(
            builder: (context, constraints) {
              return Container(
                height: 80,
                width: constraints.maxWidth,
                decoration: BoxDecoration(
                  color: Theme.of(context).colorScheme.surfaceContainerLowest,
                  borderRadius: const BorderRadius.only(
                    topLeft: Radius.circular(AppSizes.radius * 2),
                    topRight: Radius.circular(AppSizes.radius * 2),
                  ),
                  boxShadow: [
                    BoxShadow(
                      color: Theme.of(context).colorScheme.shadow.withOpacity(0.05),
                      offset: const Offset(0, -2),
                      blurRadius: 8,
                    ),
                  ],
                ),
                child: Material(
                  color: Colors.transparent,
                  child: InkWell(
                    onTap: isCartEmpty ? null : () {
                      HapticFeedback.lightImpact();
                      if (provider.isPanelExpanded) {
                        provider.panelController.close();
                      } else {
                        provider.panelController.open();
                      }
                    },
                    borderRadius: const BorderRadius.only(
                      topLeft: Radius.circular(AppSizes.radius * 2),
                      topRight: Radius.circular(AppSizes.radius * 2),
                    ),
                    child: Container(
                      width: constraints.maxWidth,
                      height: 80,
                      padding: const EdgeInsets.symmetric(
                        horizontal: AppSizes.padding,
                        vertical: 12,
                      ),
                      child: Row(
                        children: [
                          // Cart Icon with Badge
                          _buildCartIcon(itemCount, isCartEmpty),
                          
                          const SizedBox(width: 12),
                          
                          // Cart Content
                          Expanded(
                            child: _buildCartContent(provider, isCartEmpty, itemCount, total),
                          ),
                          
                          // Expand/Collapse Indicator
                          if (!isCartEmpty) _buildExpandIndicator(provider),
                        ],
                      ),
                    ),
                  ),
                ),
              );
            },
          ),
        );
      },
    );
  }

  Widget _buildCartIcon(int itemCount, bool isCartEmpty) {
    return AnimatedBuilder(
      animation: _pulseAnimation,
      builder: (context, child) {
        return Transform.scale(
          scale: isCartEmpty ? 1.0 : _pulseAnimation.value,
          child: Stack(
            children: [
              Container(
                width: 48,
                height: 48,
                decoration: BoxDecoration(
                  color: isCartEmpty 
                    ? Theme.of(context).colorScheme.surfaceVariant.withOpacity(0.5)
                    : Theme.of(context).colorScheme.primaryContainer,
                  borderRadius: BorderRadius.circular(24),
                ),
                child: Icon(
                  Icons.shopping_cart_rounded,
                  color: isCartEmpty 
                    ? Theme.of(context).colorScheme.onSurfaceVariant
                    : Theme.of(context).colorScheme.onPrimaryContainer,
                  size: 24,
                ),
              ),
              
              // Item Count Badge
              if (itemCount > 0)
                Positioned(
                  top: -2,
                  right: -2,
                  child: AnimatedScale(
                    scale: itemCount > 0 ? 1.0 : 0.0,
                    duration: const Duration(milliseconds: 200),
                    child: Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 6,
                        vertical: 2,
                      ),
                      decoration: BoxDecoration(
                        color: Theme.of(context).colorScheme.error,
                        borderRadius: BorderRadius.circular(10),
                        border: Border.all(
                          color: Theme.of(context).colorScheme.surface,
                          width: 2,
                        ),
                      ),
                      constraints: const BoxConstraints(
                        minWidth: 20,
                        minHeight: 20,
                      ),
                      child: Text(
                        itemCount > 99 ? '99+' : itemCount.toString(),
                        style: Theme.of(context).textTheme.labelSmall?.copyWith(
                          color: Theme.of(context).colorScheme.onError,
                          fontWeight: FontWeight.bold,
                          fontSize: 11,
                        ),
                        textAlign: TextAlign.center,
                      ),
                    ),
                  ),
                ),
            ],
          ),
        );
      },
    );
  }

  Widget _buildCartContent(HomeProvider provider, bool isCartEmpty, int itemCount, int total) {
    return AnimatedSwitcher(
      duration: const Duration(milliseconds: 300),
      child: isCartEmpty 
        ? _buildEmptyCartContent()
        : _buildFilledCartContent(itemCount, total),
    );
  }

  Widget _buildEmptyCartContent() {
    return Column(
      key: const ValueKey('empty'),
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Text(
          'Shopping Cart',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.w600,
            color: Theme.of(context).colorScheme.onSurfaceVariant,
          ),
        ),
        const SizedBox(height: 2),
        Text(
          'Add items to get started',
          style: Theme.of(context).textTheme.bodySmall?.copyWith(
            color: Theme.of(context).colorScheme.outline,
          ),
        ),
      ],
    );
  }

  Widget _buildFilledCartContent(int itemCount, int total) {
    return Column(
      key: const ValueKey('filled'),
      crossAxisAlignment: CrossAxisAlignment.start,
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Row(
          children: [
            Text(
              'Shopping Cart',
              style: Theme.of(context).textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.w600,
                color: Theme.of(context).colorScheme.onSurface,
              ),
            ),
            const SizedBox(width: 8),
            Container(
              padding: const EdgeInsets.symmetric(
                horizontal: 6,
                vertical: 2,
              ),
              decoration: BoxDecoration(
                color: Theme.of(context).colorScheme.secondaryContainer,
                borderRadius: BorderRadius.circular(10),
              ),
              child: Text(
                '$itemCount item${itemCount == 1 ? '' : 's'}',
                style: Theme.of(context).textTheme.labelSmall?.copyWith(
                  color: Theme.of(context).colorScheme.onSecondaryContainer,
                  fontWeight: FontWeight.w500,
                ),
              ),
            ),
          ],
        ),
        const SizedBox(height: 2),
        Row(
          children: [
            Icon(
              Icons.payments_rounded,
              size: 14,
              color: Theme.of(context).colorScheme.primary,
            ),
            const SizedBox(width: 4),
            Text(
              'Total: ${CurrencyFormatter.format(total)}',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                fontWeight: FontWeight.bold,
                color: Theme.of(context).colorScheme.primary,
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildExpandIndicator(HomeProvider provider) {
    return AnimatedRotation(
      turns: provider.isPanelExpanded ? 0.5 : 0.0,
      duration: const Duration(milliseconds: 300),
      child: Container(
        padding: const EdgeInsets.all(8),
        decoration: BoxDecoration(
          color: Theme.of(context).colorScheme.surfaceVariant.withOpacity(0.5),
          borderRadius: BorderRadius.circular(16),
        ),
        child: Icon(
          Icons.keyboard_arrow_up_rounded,
          size: 20,
          color: Theme.of(context).colorScheme.onSurfaceVariant,
        ),
      ),
    );
  }
}
