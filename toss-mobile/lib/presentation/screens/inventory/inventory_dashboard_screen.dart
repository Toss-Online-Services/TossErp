import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../domain/entities/inventory_movement_entity.dart';
import '../../providers/inventory/inventory_provider.dart';
import 'stock_adjustment_screen.dart';
import 'inventory_transfer_screen.dart';
import 'batch_management_screen.dart';
import 'low_stock_alerts_screen.dart';

class InventoryDashboardScreen extends StatefulWidget {
  const InventoryDashboardScreen({super.key});

  @override
  State<InventoryDashboardScreen> createState() => _InventoryDashboardScreenState();
}

class _InventoryDashboardScreenState extends State<InventoryDashboardScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<InventoryProvider>().loadAllMovements();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Inventory Dashboard'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () {
              context.read<InventoryProvider>().loadAllMovements();
            },
          ),
        ],
      ),
      body: Consumer<InventoryProvider>(
        builder: (context, provider, child) {
          if (provider.isLoading) {
            return const Center(child: CircularProgressIndicator());
          }

          if (provider.error != null) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(
                    Icons.error_outline,
                    size: 64,
                    color: Colors.red[300],
                  ),
                  const SizedBox(height: 16),
                  Text(
                    'Error: ${provider.error}',
                    style: Theme.of(context).textTheme.bodyLarge,
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 16),
                  ElevatedButton(
                    onPressed: () {
                      provider.clearError();
                      provider.loadAllMovements();
                    },
                    child: const Text('Retry'),
                  ),
                ],
              ),
            );
          }

          return SingleChildScrollView(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _buildStatsCards(provider),
                const SizedBox(height: 24),
                _buildQuickActions(provider),
                const SizedBox(height: 24),
                _buildRecentMovements(provider),
                const SizedBox(height: 24),
                _buildMovementTypeChart(provider),
                const SizedBox(height: 24),
                _buildAlertsSection(provider),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildStatsCards(InventoryProvider provider) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Inventory Overview',
          style: Theme.of(context).textTheme.titleLarge,
        ),
        const SizedBox(height: 16),
        Row(
          children: [
            Expanded(
              child: _buildStatCard(
                'Total Movements',
                provider.totalMovements.toString(),
                Icons.inventory,
                Colors.blue,
              ),
            ),
            const SizedBox(width: 8),
            Expanded(
              child: _buildStatCard(
                'Total Value',
                '\$${provider.totalValue.toStringAsFixed(2)}',
                Icons.attach_money,
                Colors.green,
              ),
            ),
          ],
        ),
        const SizedBox(height: 8),
        Row(
          children: [
            Expanded(
              child: _buildStatCard(
                'Purchases',
                provider.purchaseMovements.toString(),
                Icons.shopping_cart,
                Colors.orange,
              ),
            ),
            const SizedBox(width: 8),
            Expanded(
              child: _buildStatCard(
                'Sales',
                provider.saleMovements.toString(),
                Icons.point_of_sale,
                Colors.purple,
              ),
            ),
          ],
        ),
        const SizedBox(height: 8),
        Row(
          children: [
            Expanded(
              child: _buildStatCard(
                'Adjustments',
                provider.adjustmentMovements.toString(),
                Icons.edit,
                Colors.red,
              ),
            ),
            const SizedBox(width: 8),
            Expanded(
              child: _buildStatCard(
                'Transfers',
                provider.transferMovements.toString(),
                Icons.swap_horiz,
                Colors.teal,
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildStatCard(String title, String value, IconData icon, Color color) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          children: [
            Icon(icon, color: color, size: 24),
            const SizedBox(height: 4),
            Text(
              value,
              style: Theme.of(context).textTheme.titleLarge?.copyWith(
                fontWeight: FontWeight.bold,
                color: color,
              ),
            ),
            Text(
              title,
              style: Theme.of(context).textTheme.bodySmall,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildQuickActions(InventoryProvider provider) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Quick Actions',
          style: Theme.of(context).textTheme.titleLarge,
        ),
        const SizedBox(height: 16),
        GridView.count(
          shrinkWrap: true,
          physics: const NeverScrollableScrollPhysics(),
          crossAxisCount: 2,
          crossAxisSpacing: 8,
          mainAxisSpacing: 8,
          childAspectRatio: 1.5,
          children: [
            _buildActionCard(
              'Stock Adjustment',
              Icons.edit,
              Colors.orange,
              () => _navigateToStockAdjustment(),
            ),
            _buildActionCard(
              'Inventory Transfer',
              Icons.swap_horiz,
              Colors.blue,
              () => _navigateToInventoryTransfer(),
            ),
            _buildActionCard(
              'Batch Management',
              Icons.inventory_2,
              Colors.green,
              () => _navigateToBatchManagement(),
            ),
            _buildActionCard(
              'Low Stock Alerts',
              Icons.warning,
              Colors.red,
              () => _navigateToLowStockAlerts(),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildActionCard(String title, IconData icon, Color color, VoidCallback onTap) {
    return Card(
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(8),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(icon, color: color, size: 32),
              const SizedBox(height: 8),
              Text(
                title,
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  fontWeight: FontWeight.w500,
                ),
                textAlign: TextAlign.center,
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildRecentMovements(InventoryProvider provider) {
    final recentMovements = provider.getRecentMovements(limit: 5);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              'Recent Movements',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            TextButton(
              onPressed: () {
                // TODO: Navigate to full movements list
              },
              child: const Text('View All'),
            ),
          ],
        ),
        const SizedBox(height: 16),
        if (recentMovements.isEmpty)
          Card(
            child: Padding(
              padding: const EdgeInsets.all(32),
              child: Column(
                children: [
                  Icon(
                    Icons.inventory_outlined,
                    size: 48,
                    color: Colors.grey[400],
                  ),
                  const SizedBox(height: 16),
                  Text(
                    'No movements yet',
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      color: Colors.grey[600],
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    'Start by creating your first inventory movement',
                    style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                      color: Colors.grey[500],
                    ),
                    textAlign: TextAlign.center,
                  ),
                ],
              ),
            ),
          )
        else
          ...recentMovements.map((movement) => _buildMovementCard(movement)),
      ],
    );
  }

  Widget _buildMovementCard(InventoryMovementEntity movement) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: _getMovementTypeColor(movement.type),
          child: Icon(
            _getMovementTypeIcon(movement.type),
            color: Colors.white,
            size: 20,
          ),
        ),
        title: Text(
          movement.reason,
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Product: ${movement.productId}'),
            Text('Quantity: ${movement.quantity}'),
            if (movement.notes != null) Text('Notes: ${movement.notes}'),
          ],
        ),
        trailing: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Text(
              '\$${movement.totalValue.toStringAsFixed(2)}',
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                color: Colors.green,
              ),
            ),
            Text(
              _formatDate(movement.createdAt),
              style: Theme.of(context).textTheme.bodySmall,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildMovementTypeChart(InventoryProvider provider) {
    final typeCounts = provider.getMovementsByTypeCount();

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Movement Types',
          style: Theme.of(context).textTheme.titleLarge,
        ),
        const SizedBox(height: 16),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              children: typeCounts.entries.map((entry) {
                final type = entry.key;
                final count = entry.value;
                final percentage = provider.totalMovements > 0 
                    ? (count / provider.totalMovements * 100) 
                    : 0.0;

                return Padding(
                  padding: const EdgeInsets.only(bottom: 12),
                  child: Row(
                    children: [
                      Icon(
                        _getMovementTypeIcon(type),
                        color: _getMovementTypeColor(type),
                        size: 20,
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              type.name.toUpperCase(),
                              style: const TextStyle(fontWeight: FontWeight.w500),
                            ),
                            LinearProgressIndicator(
                              value: percentage / 100,
                              backgroundColor: Colors.grey[300],
                              valueColor: AlwaysStoppedAnimation<Color>(
                                _getMovementTypeColor(type),
                              ),
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(width: 12),
                      Text(
                        '$count',
                        style: const TextStyle(fontWeight: FontWeight.bold),
                      ),
                    ],
                  ),
                );
              }).toList(),
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildAlertsSection(InventoryProvider provider) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Alerts & Notifications',
          style: Theme.of(context).textTheme.titleLarge,
        ),
        const SizedBox(height: 16),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              children: [
                ListTile(
                  leading: const Icon(Icons.warning, color: Colors.orange),
                  title: const Text('Low Stock Items'),
                  subtitle: const Text('Check items that need restocking'),
                  trailing: const Icon(Icons.arrow_forward_ios),
                  onTap: () => _navigateToLowStockAlerts(),
                ),
                const Divider(),
                ListTile(
                  leading: const Icon(Icons.schedule, color: Colors.red),
                  title: const Text('Expired Products'),
                  subtitle: const Text('Review expired inventory'),
                  trailing: const Icon(Icons.arrow_forward_ios),
                  onTap: () {
                    provider.loadExpiredProductMovements();
                  },
                ),
                const Divider(),
                ListTile(
                  leading: const Icon(Icons.analytics, color: Colors.blue),
                  title: const Text('Inventory Valuation'),
                  subtitle: const Text('View current inventory value'),
                  trailing: const Icon(Icons.arrow_forward_ios),
                  onTap: () async {
                    final valuation = await provider.loadInventoryValuation();
                    if (valuation != null && mounted) {
                      _showValuationDialog(valuation);
                    }
                  },
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }

  Color _getMovementTypeColor(InventoryMovementType type) {
    switch (type) {
      case InventoryMovementType.purchase:
        return Colors.green;
      case InventoryMovementType.sale:
        return Colors.blue;
      case InventoryMovementType.adjustment:
        return Colors.orange;
      case InventoryMovementType.transfer:
        return Colors.purple;
    }
  }

  IconData _getMovementTypeIcon(InventoryMovementType type) {
    switch (type) {
      case InventoryMovementType.purchase:
        return Icons.shopping_cart;
      case InventoryMovementType.sale:
        return Icons.point_of_sale;
      case InventoryMovementType.adjustment:
        return Icons.edit;
      case InventoryMovementType.transfer:
        return Icons.swap_horiz;
    }
  }

  String _formatDate(String dateString) {
    try {
      final date = DateTime.parse(dateString);
      return '${date.day}/${date.month}/${date.year}';
    } catch (e) {
      return dateString;
    }
  }

  void _navigateToStockAdjustment() {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => const StockAdjustmentScreen(),
      ),
    );
  }

  void _navigateToInventoryTransfer() {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => const InventoryTransferScreen(),
      ),
    );
  }

  void _navigateToBatchManagement() {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => const BatchManagementScreen(),
      ),
    );
  }

  void _navigateToLowStockAlerts() {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => const LowStockAlertsScreen(),
      ),
    );
  }

  void _showValuationDialog(Map<String, double> valuation) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Inventory Valuation'),
        content: SizedBox(
          width: double.maxFinite,
          child: ListView.builder(
            shrinkWrap: true,
            itemCount: valuation.length,
            itemBuilder: (context, index) {
              final entry = valuation.entries.elementAt(index);
              return ListTile(
                title: Text(entry.key),
                trailing: Text(
                  '\$${entry.value.toStringAsFixed(2)}',
                  style: const TextStyle(fontWeight: FontWeight.bold),
                ),
              );
            },
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Close'),
          ),
        ],
      ),
    );
  }
}
