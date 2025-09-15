import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:go_router/go_router.dart';

import 'package:toss_mobile/app/dashboard/dashboard_config.dart';
import '../../app/themes/app_sizes.dart';

/// Customizable dashboard widget for the home screen
class CustomizableDashboard extends StatelessWidget {
  const CustomizableDashboard({super.key});

  @override
  Widget build(BuildContext context) {
    return Consumer<DashboardManager>(
      builder: (context, dashboardManager, child) {
        final layout = dashboardManager.currentLayout;
        
        if (layout.widgets.isEmpty) {
          return _buildEmptyDashboard(context);
        }

        return Column(
          children: [
            _buildDashboardHeader(context, layout),
            const SizedBox(height: AppSizes.padding),
            Expanded(
              child: _buildDashboardGrid(context, layout),
            ),
          ],
        );
      },
    );
  }

  Widget _buildDashboardHeader(BuildContext context, DashboardLayout layout) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: AppSizes.padding),
      child: Row(
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Dashboard',
                  style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
                Text(
                  layout.name,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: Theme.of(context).primaryColor,
                  ),
                ),
              ],
            ),
          ),
          IconButton(
            icon: const Icon(Icons.tune),
            onPressed: () => context.go('/dashboard-layout'),
            tooltip: 'Customize Dashboard',
          ),
        ],
      ),
    );
  }

  Widget _buildDashboardGrid(BuildContext context, DashboardLayout layout) {
    return Padding(
      padding: EdgeInsets.all(layout.spacing),
      child: GridView.builder(
        gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
          crossAxisCount: layout.columns,
          crossAxisSpacing: layout.spacing,
          mainAxisSpacing: layout.spacing,
          childAspectRatio: _getAspectRatio(layout.columns),
        ),
        itemCount: layout.widgets.length,
        itemBuilder: (context, index) {
          final widgetConfig = layout.widgets[index];
          return _buildDashboardWidget(context, widgetConfig);
        },
      ),
    );
  }

  Widget _buildDashboardWidget(BuildContext context, DashboardWidgetConfig config) {
    final widgetInfo = AvailableWidgets.allWidgets[config.type]!;
    final color = config.color ?? widgetInfo.defaultColor;

    return Card(
      elevation: 2,
      child: InkWell(
        onTap: () => _handleWidgetTap(context, config.type),
        borderRadius: BorderRadius.circular(12),
        child: Container(
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(12),
            gradient: LinearGradient(
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
              colors: [
                color.withOpacity(0.1),
                color.withOpacity(0.05),
              ],
            ),
          ),
          child: Padding(
            padding: const EdgeInsets.all(AppSizes.padding),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.all(8),
                      decoration: BoxDecoration(
                        color: color.withOpacity(0.2),
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: Icon(
                        widgetInfo.icon,
                        color: color,
                        size: 20,
                      ),
                    ),
                    const Spacer(),
                    if (config.size == DashboardWidgetSize.large)
                      Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: 6,
                          vertical: 2,
                        ),
                        decoration: BoxDecoration(
                          color: color.withOpacity(0.1),
                          borderRadius: BorderRadius.circular(10),
                        ),
                        child: Text(
                          'LIVE',
                          style: TextStyle(
                            fontSize: 10,
                            fontWeight: FontWeight.bold,
                            color: color,
                          ),
                        ),
                      ),
                  ],
                ),
                const SizedBox(height: 8),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        widgetInfo.name,
                        style: Theme.of(context).textTheme.titleSmall?.copyWith(
                          fontWeight: FontWeight.w600,
                        ),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                      const SizedBox(height: 4),
                      Expanded(
                        child: _buildWidgetContent(context, config.type, color),
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

  Widget _buildWidgetContent(BuildContext context, DashboardWidget type, Color color) {
    switch (type) {
      case DashboardWidget.salesSummary:
        return _buildSalesContent(context, color);
      case DashboardWidget.todaysRevenue:
        return _buildRevenueContent(context, color);
      case DashboardWidget.inventoryStatus:
        return _buildInventoryContent(context, color);
      case DashboardWidget.recentTransactions:
        return _buildTransactionsContent(context, color);
      case DashboardWidget.topProducts:
        return _buildTopProductsContent(context, color);
      case DashboardWidget.quickActions:
        return _buildQuickActionsContent(context, color);
      case DashboardWidget.staffActivity:
        return _buildStaffContent(context, color);
      case DashboardWidget.customerInsights:
        return _buildCustomerContent(context, color);
      case DashboardWidget.performanceMetrics:
        return _buildPerformanceContent(context, color);
      case DashboardWidget.alerts:
        return _buildAlertsContent(context, color);
      default:
        return _buildPlaceholderContent(context, color);
    }
  }

  Widget _buildSalesContent(BuildContext context, Color color) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '₱24,560',
          style: Theme.of(context).textTheme.titleLarge?.copyWith(
            fontWeight: FontWeight.bold,
            color: color,
          ),
        ),
        Text(
          '+12.5% from yesterday',
          style: Theme.of(context).textTheme.bodySmall?.copyWith(
            color: Colors.green,
          ),
        ),
      ],
    );
  }

  Widget _buildRevenueContent(BuildContext context, Color color) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '₱156,890',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.bold,
            color: color,
          ),
        ),
        Text(
          'This month',
          style: Theme.of(context).textTheme.bodySmall,
        ),
      ],
    );
  }

  Widget _buildInventoryContent(BuildContext context, Color color) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            Text(
              '156',
              style: Theme.of(context).textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
                color: color,
              ),
            ),
            const SizedBox(width: 4),
            Text(
              'items',
              style: Theme.of(context).textTheme.bodySmall,
            ),
          ],
        ),
        Text(
          '12 low stock',
          style: Theme.of(context).textTheme.bodySmall?.copyWith(
            color: Colors.orange,
          ),
        ),
      ],
    );
  }

  Widget _buildTransactionsContent(BuildContext context, Color color) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '47',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.bold,
            color: color,
          ),
        ),
        Text(
          'Today',
          style: Theme.of(context).textTheme.bodySmall,
        ),
      ],
    );
  }

  Widget _buildTopProductsContent(BuildContext context, Color color) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Coffee Beans',
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
            fontWeight: FontWeight.w500,
          ),
          maxLines: 1,
          overflow: TextOverflow.ellipsis,
        ),
        Text(
          '45 sold today',
          style: Theme.of(context).textTheme.bodySmall?.copyWith(
            color: color,
          ),
        ),
      ],
    );
  }

  Widget _buildQuickActionsContent(BuildContext context, Color color) {
    return Row(
      children: [
        Expanded(
          child: _QuickActionButton(
            icon: Icons.add_shopping_cart,
            label: 'Sell',
            color: color,
            onTap: () => context.go('/home'),
          ),
        ),
        const SizedBox(width: 8),
        Expanded(
          child: _QuickActionButton(
            icon: Icons.inventory,
            label: 'Stock',
            color: color,
            onTap: () => context.go('/products'),
          ),
        ),
      ],
    );
  }

  Widget _buildStaffContent(BuildContext context, Color color) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '3 active',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.bold,
            color: color,
          ),
        ),
        Text(
          'staff members',
          style: Theme.of(context).textTheme.bodySmall,
        ),
      ],
    );
  }

  Widget _buildCustomerContent(BuildContext context, Color color) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '89',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.bold,
            color: color,
          ),
        ),
        Text(
          'customers today',
          style: Theme.of(context).textTheme.bodySmall,
        ),
      ],
    );
  }

  Widget _buildPerformanceContent(BuildContext context, Color color) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '94%',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.bold,
            color: color,
          ),
        ),
        Text(
          'efficiency',
          style: Theme.of(context).textTheme.bodySmall,
        ),
      ],
    );
  }

  Widget _buildAlertsContent(BuildContext context, Color color) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '2',
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.bold,
            color: color,
          ),
        ),
        Text(
          'alerts',
          style: Theme.of(context).textTheme.bodySmall?.copyWith(
            color: Colors.red,
          ),
        ),
      ],
    );
  }

  Widget _buildPlaceholderContent(BuildContext context, Color color) {
    return Center(
      child: Text(
        'Widget content',
        style: Theme.of(context).textTheme.bodySmall?.copyWith(
          color: color,
        ),
      ),
    );
  }

  Widget _buildEmptyDashboard(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(AppSizes.padding * 2),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.dashboard_customize_outlined,
              size: 80,
              color: Colors.grey[400],
            ),
            const SizedBox(height: 16),
            Text(
              'No Dashboard Widgets',
              style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                fontWeight: FontWeight.bold,
                color: Colors.grey[600],
              ),
            ),
            const SizedBox(height: 8),
            Text(
              'Customize your dashboard to add widgets and personalize your POS experience.',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: Colors.grey[600],
              ),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 24),
            ElevatedButton.icon(
              onPressed: () => context.go('/dashboard-layout'),
              icon: const Icon(Icons.tune),
              label: const Text('Customize Dashboard'),
            ),
          ],
        ),
      ),
    );
  }

  void _handleWidgetTap(BuildContext context, DashboardWidget type) {
    switch (type) {
      case DashboardWidget.salesSummary:
      case DashboardWidget.todaysRevenue:
      case DashboardWidget.performanceMetrics:
        context.go('/reports');
        break;
      case DashboardWidget.inventoryStatus:
      case DashboardWidget.topProducts:
        context.go('/products');
        break;
      case DashboardWidget.recentTransactions:
        context.go('/transactions');
        break;
      case DashboardWidget.staffActivity:
        context.go('/staff');
        break;
      case DashboardWidget.customerInsights:
        context.go('/customers');
        break;
      case DashboardWidget.alerts:
        // Could show a dialog or navigate to a notifications screen
        _showAlertsDialog(context);
        break;
      case DashboardWidget.quickActions:
        // Quick actions are handled by individual buttons
        break;
      default:
        break;
    }
  }

  void _showAlertsDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('System Alerts'),
        content: const Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('• Low stock: Coffee Beans (5 remaining)'),
            Text('• Low stock: Sugar Packets (8 remaining)'),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Close'),
          ),
          TextButton(
            onPressed: () {
              Navigator.pop(context);
              context.go('/products');
            },
            child: const Text('View Inventory'),
          ),
        ],
      ),
    );
  }

  double _getAspectRatio(int columns) {
    switch (columns) {
      case 1:
        return 2.5;
      case 2:
        return 1.4;
      case 3:
        return 1.0;
      case 4:
        return 0.8;
      case 5:
        return 0.7;
      default:
        return 1.2;
    }
  }
}

/// Quick action button widget
class _QuickActionButton extends StatelessWidget {
  final IconData icon;
  final String label;
  final Color color;
  final VoidCallback onTap;

  const _QuickActionButton({
    required this.icon,
    required this.label,
    required this.color,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(6),
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 8),
        decoration: BoxDecoration(
          color: color.withOpacity(0.1),
          borderRadius: BorderRadius.circular(6),
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              icon,
              size: 16,
              color: color,
            ),
            const SizedBox(height: 2),
            Text(
              label,
              style: TextStyle(
                fontSize: 10,
                fontWeight: FontWeight.w500,
                color: color,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
