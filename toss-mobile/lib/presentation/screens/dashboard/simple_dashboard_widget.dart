import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../../simple_dashboard_manager.dart';

/// Simple dashboard widget that uses SimpleDashboardManager
class SimpleDashboardWidget extends StatefulWidget {
  const SimpleDashboardWidget({super.key});

  @override
  State<SimpleDashboardWidget> createState() => _SimpleDashboardWidgetState();
}

class _SimpleDashboardWidgetState extends State<SimpleDashboardWidget> {
  late Future<void> _initializationFuture;

  @override
  void initState() {
    super.initState();
    // Get the dashboard manager and initialize it
    final dashboardManager = Provider.of<SimpleDashboardManager>(context, listen: false);
    _initializationFuture = dashboardManager.initialize();
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<void>(
      future: _initializationFuture,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          debugPrint('SimpleDashboardWidget: Waiting for initialization...');
          return const Center(
            child: CircularProgressIndicator(),
          );
        }

        if (snapshot.hasError) {
          debugPrint('SimpleDashboardWidget: Initialization error: ${snapshot.error}');
          return Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Icon(Icons.error, size: 48, color: Colors.red),
                const SizedBox(height: 16),
                Text('Error loading dashboard: ${snapshot.error}'),
                const SizedBox(height: 16),
                ElevatedButton(
                  onPressed: () {
                    setState(() {
                      final dashboardManager = Provider.of<SimpleDashboardManager>(context, listen: false);
                      _initializationFuture = dashboardManager.initialize();
                    });
                  },
                  child: const Text('Retry'),
                ),
              ],
            ),
          );
        }

        debugPrint('SimpleDashboardWidget: Initialization complete, building dashboard');
        return Consumer<SimpleDashboardManager>(
          builder: (context, dashboardManager, child) {
            debugPrint('SimpleDashboardWidget: Building dashboard with ${dashboardManager.enabledWidgets.length} widgets');
            return Scaffold(
              body: CustomScrollView(
                slivers: [
                  SliverPadding(
                    padding: const EdgeInsets.all(16),
                    sliver: SliverToBoxAdapter(
                      child: Container(
                        padding: const EdgeInsets.all(16),
                        decoration: BoxDecoration(
                          color: Theme.of(context).colorScheme.surface,
                          borderRadius: BorderRadius.circular(12),
                          border: Border.all(
                            color: Theme.of(context).colorScheme.outline.withValues(alpha: 0.5),
                          ),
                        ),
                        child: Row(
                          children: [
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    dashboardManager.currentLayout.name,
                                    style: Theme.of(context).textTheme.titleLarge,
                                  ),
                                  const SizedBox(height: 4),
                                  Text(
                                    dashboardManager.currentLayout.description,
                                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                                      color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.7),
                                    ),
                                  ),
                                ],
                              ),
                            ),
                            ElevatedButton.icon(
                              onPressed: () => _showLayoutSelector(context, dashboardManager),
                              icon: const Icon(Icons.tune),
                              label: const Text('Change Layout'),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ),
                  
                  // Dashboard widgets grid
                  SliverPadding(
                    padding: const EdgeInsets.fromLTRB(16, 0, 16, 120), // Extra bottom padding for cart panel
                    sliver: SliverGrid(
                      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                        crossAxisCount: 2,
                        childAspectRatio: 1.2,
                        crossAxisSpacing: 12,
                        mainAxisSpacing: 12,
                      ),
                      delegate: SliverChildBuilderDelegate(
                        (context, index) {
                          final widget = dashboardManager.enabledWidgets[index];
                          return _buildWidgetCard(context, widget);
                        },
                        childCount: dashboardManager.enabledWidgets.length,
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

  Widget _buildWidgetCard(BuildContext context, SimpleDashboardWidgetConfig widget) {
    return Card(
      elevation: 2,
      child: InkWell(
        onTap: () => _showWidgetDetails(context, widget),
        borderRadius: BorderRadius.circular(12),
        child: Container(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Icon(
                    _getWidgetIcon(widget.widget),
                    color: Theme.of(context).colorScheme.primary,
                    size: 24,
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      _getWidgetTitle(widget.widget),
                      style: Theme.of(context).textTheme.titleMedium,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              Expanded(
                child: _buildWidgetContent(context, widget.widget),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildWidgetContent(BuildContext context, DashboardWidgetType widget) {
    switch (widget) {
      case DashboardWidgetType.salesSummary:
        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'R12,450',
              style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                color: Colors.green,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              'Today\'s Sales',
              style: Theme.of(context).textTheme.bodySmall,
            ),
            const Spacer(),
            Row(
              children: [
                const Icon(Icons.trending_up, size: 16, color: Colors.green),
                const SizedBox(width: 4),
                Text(
                  '+12%',
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: Colors.green,
                  ),
                ),
              ],
            ),
          ],
        );

      case DashboardWidgetType.todaysRevenue:
        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'R8,320',
              style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                color: Colors.blue,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              'Revenue',
              style: Theme.of(context).textTheme.bodySmall,
            ),
            const Spacer(),
            Text(
              '24 transactions',
              style: Theme.of(context).textTheme.bodySmall,
            ),
          ],
        );

      case DashboardWidgetType.inventoryStatus:
        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              '156',
              style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              'Items in Stock',
              style: Theme.of(context).textTheme.bodySmall,
            ),
            const Spacer(),
            Row(
              children: [
                const Icon(Icons.warning, size: 16, color: Colors.orange),
                const SizedBox(width: 4),
                Text(
                  '3 low stock',
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: Colors.orange,
                  ),
                ),
              ],
            ),
          ],
        );

      case DashboardWidgetType.quickActions:
        return Wrap(
          spacing: 8,
          runSpacing: 8,
          children: [
            _buildQuickActionChip(context, Icons.add, 'Add Sale'),
            _buildQuickActionChip(context, Icons.inventory, 'Stock'),
            _buildQuickActionChip(context, Icons.receipt, 'Report'),
          ],
        );

      default:
        return Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(
                _getWidgetIcon(widget),
                size: 32,
                color: Theme.of(context).colorScheme.primary.withValues(alpha: 0.5),
              ),
              const SizedBox(height: 8),
              Text(
                'Coming Soon',
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                  color: Theme.of(context).colorScheme.onSurface.withValues(alpha: 0.5),
                ),
              ),
            ],
          ),
        );
    }
  }

  Widget _buildQuickActionChip(BuildContext context, IconData icon, String label) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.primaryContainer,
        borderRadius: BorderRadius.circular(16),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 12),
          const SizedBox(width: 4),
          Text(
            label,
            style: Theme.of(context).textTheme.bodySmall,
          ),
        ],
      ),
    );
  }

  void _showLayoutSelector(BuildContext context, SimpleDashboardManager manager) {
    showModalBottomSheet(
      context: context,
      builder: (context) => Container(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Select Dashboard Layout',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            ...manager.allLayouts.map((layout) => ListTile(
              leading: Icon(
                layout.isDefault ? Icons.dashboard : Icons.view_module,
                color: manager.currentLayout.id == layout.id
                    ? Theme.of(context).colorScheme.primary
                    : null,
              ),
              title: Text(layout.name),
              subtitle: Text(layout.description),
              trailing: manager.currentLayout.id == layout.id
                  ? Icon(
                      Icons.check,
                      color: Theme.of(context).colorScheme.primary,
                    )
                  : null,
              onTap: () {
                manager.applyLayout(layout);
                Navigator.pop(context);
              },
            )),
          ],
        ),
      ),
    );
  }

  void _showWidgetDetails(BuildContext context, SimpleDashboardWidgetConfig widget) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Row(
          children: [
            Icon(_getWidgetIcon(widget.widget)),
            const SizedBox(width: 8),
            Text(_getWidgetTitle(widget.widget)),
          ],
        ),
        content: Text(_getWidgetDescription(widget.widget)),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Close'),
          ),
        ],
      ),
    );
  }

  IconData _getWidgetIcon(DashboardWidgetType widget) {
    switch (widget) {
      case DashboardWidgetType.salesSummary:
        return Icons.trending_up;
      case DashboardWidgetType.todaysRevenue:
        return Icons.attach_money;
      case DashboardWidgetType.inventoryStatus:
        return Icons.inventory_2;
      case DashboardWidgetType.pendingOrders:
        return Icons.pending_actions;
      case DashboardWidgetType.topProducts:
        return Icons.star;
      case DashboardWidgetType.recentTransactions:
        return Icons.receipt_long;
      case DashboardWidgetType.quickActions:
        return Icons.flash_on;
      case DashboardWidgetType.weatherWidget:
        return Icons.wb_sunny;
      case DashboardWidgetType.stockAlerts:
        return Icons.warning;
      case DashboardWidgetType.performanceChart:
        return Icons.bar_chart;
      case DashboardWidgetType.customerCount:
        return Icons.people;
      case DashboardWidgetType.categoryBreakdown:
        return Icons.pie_chart;
      case DashboardWidgetType.profitLoss:
        return Icons.account_balance;
      case DashboardWidgetType.cashFlow:
        return Icons.account_balance_wallet;
      case DashboardWidgetType.taskList:
        return Icons.task_alt;
    }
  }

  String _getWidgetTitle(DashboardWidgetType widget) {
    switch (widget) {
      case DashboardWidgetType.salesSummary:
        return 'Sales Summary';
      case DashboardWidgetType.todaysRevenue:
        return 'Today\'s Revenue';
      case DashboardWidgetType.inventoryStatus:
        return 'Inventory Status';
      case DashboardWidgetType.pendingOrders:
        return 'Pending Orders';
      case DashboardWidgetType.topProducts:
        return 'Top Products';
      case DashboardWidgetType.recentTransactions:
        return 'Recent Transactions';
      case DashboardWidgetType.quickActions:
        return 'Quick Actions';
      case DashboardWidgetType.weatherWidget:
        return 'Weather';
      case DashboardWidgetType.stockAlerts:
        return 'Stock Alerts';
      case DashboardWidgetType.performanceChart:
        return 'Performance Chart';
      case DashboardWidgetType.customerCount:
        return 'Customer Count';
      case DashboardWidgetType.categoryBreakdown:
        return 'Category Breakdown';
      case DashboardWidgetType.profitLoss:
        return 'Profit & Loss';
      case DashboardWidgetType.cashFlow:
        return 'Cash Flow';
      case DashboardWidgetType.taskList:
        return 'Task List';
    }
  }

  String _getWidgetDescription(DashboardWidgetType widget) {
    switch (widget) {
      case DashboardWidgetType.salesSummary:
        return 'Overview of daily, weekly, and monthly sales performance with key metrics and trends.';
      case DashboardWidgetType.todaysRevenue:
        return 'Real-time revenue tracking for the current day with comparison to previous periods.';
      case DashboardWidgetType.inventoryStatus:
        return 'Current inventory levels, stock alerts, and low-stock notifications.';
      case DashboardWidgetType.pendingOrders:
        return 'List of orders waiting to be processed or fulfilled.';
      case DashboardWidgetType.topProducts:
        return 'Best-selling products ranked by sales volume or revenue.';
      case DashboardWidgetType.recentTransactions:
        return 'Latest transactions and payment activities.';
      case DashboardWidgetType.quickActions:
        return 'Shortcuts to frequently used features and actions.';
      case DashboardWidgetType.weatherWidget:
        return 'Current weather conditions and forecast.';
      case DashboardWidgetType.stockAlerts:
        return 'Critical stock level alerts and notifications.';
      case DashboardWidgetType.performanceChart:
        return 'Visual charts showing business performance metrics.';
      case DashboardWidgetType.customerCount:
        return 'Number of customers served today and customer analytics.';
      case DashboardWidgetType.categoryBreakdown:
        return 'Sales breakdown by product categories.';
      case DashboardWidgetType.profitLoss:
        return 'Profit and loss summary with financial insights.';
      case DashboardWidgetType.cashFlow:
        return 'Cash flow tracking and financial health indicators.';
      case DashboardWidgetType.taskList:
        return 'Important tasks and reminders for business operations.';
    }
  }
}
