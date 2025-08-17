import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../data/models/stock_item.dart';
import '../../providers/stock_providers.dart';

class StockDashboardScreen extends ConsumerStatefulWidget {
  const StockDashboardScreen({super.key});

  @override
  ConsumerState<StockDashboardScreen> createState() => _StockDashboardScreenState();
}

class _StockDashboardScreenState extends ConsumerState<StockDashboardScreen> {
  final TextEditingController _searchController = TextEditingController();

  @override
  void initState() {
    super.initState();
    // Initialize data loading
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref.read(stockControllerProvider.notifier).loadInitialData();
    });
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final stockState = ref.watch(stockControllerProvider);
    final stockController = ref.read(stockControllerProvider.notifier);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Stock Management'),
        actions: [
          IconButton(
            icon: const Icon(Icons.add),
            onPressed: () {
              // TODO: Navigate to add item screen
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(content: Text('Add item screen coming soon!')),
              );
            },
          ),
          IconButton(
            icon: const Icon(Icons.filter_list),
            onPressed: () => _showFilterDialog(context, ref),
          ),
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () => stockController.refresh(),
          ),
        ],
      ),
      body: RefreshIndicator(
        onRefresh: () => stockController.refresh(),
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            children: [
              // Search Bar
              TextField(
                controller: _searchController,
                decoration: InputDecoration(
                  hintText: 'Search items by name or SKU...',
                  prefixIcon: const Icon(Icons.search),
                  suffixIcon: _searchController.text.isNotEmpty
                      ? IconButton(
                          icon: const Icon(Icons.clear),
                          onPressed: () {
                            _searchController.clear();
                            stockController.searchItems('');
                          },
                        )
                      : null,
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                ),
                onChanged: (value) {
                  stockController.searchItems(value);
                },
              ),
              const SizedBox(height: 24),
              
              // Error Display
              if (stockState.error != null)
                Container(
                  width: double.infinity,
                  padding: const EdgeInsets.all(12),
                  margin: const EdgeInsets.only(bottom: 16),
                  decoration: BoxDecoration(
                    color: Colors.red[50],
                    border: Border.all(color: Colors.red[300]!),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Row(
                    children: [
                      Icon(Icons.error, color: Colors.red[700]),
                      const SizedBox(width: 8),
                      Expanded(
                        child: Text(
                          stockState.error!,
                          style: TextStyle(color: Colors.red[700]),
                        ),
                      ),
                      IconButton(
                        icon: const Icon(Icons.close),
                        onPressed: () => stockController.clearError(),
                      ),
                    ],
                  ),
                ),
              
              // Loading Indicator
              if (stockState.isLoading && stockState.overview == null)
                const Center(child: CircularProgressIndicator())
              else ...[
                // Stock Overview Cards
                _buildOverviewCards(stockState.overview),
                const SizedBox(height: 24),
            const SizedBox(height: 24),
            
                // Quick Actions
                _buildQuickActions(stockController),
                const SizedBox(height: 24),
                
                // Recent Stock Movements
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Recent Stock Movements',
                        style: Theme.of(context).textTheme.titleLarge?.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 16),
                      Expanded(
                        child: _buildMovementsList(stockState.recentMovements),
                      ),
                    ],
                  ),
                ),
              ],
          ],
        ),
      ),
    );
  }

  Widget _buildOverviewCards(StockOverview? overview) {
    if (overview == null) {
      return const SizedBox.shrink();
    }

    return Column(
      children: [
        Row(
          children: [
            Expanded(
              child: _buildOverviewCard(
                context,
                title: 'Total Items',
                value: overview.totalItems.toString(),
                icon: Icons.inventory,
                color: Colors.blue,
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: _buildOverviewCard(
                context,
                title: 'Low Stock',
                value: overview.lowStockItems.toString(),
                icon: Icons.warning,
                color: Colors.orange,
              ),
            ),
          ],
        ),
        const SizedBox(height: 16),
        Row(
          children: [
            Expanded(
              child: _buildOverviewCard(
                context,
                title: 'Out of Stock',
                value: overview.outOfStockItems.toString(),
                icon: Icons.error,
                color: Colors.red,
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: _buildOverviewCard(
                context,
                title: 'Total Value',
                value: 'R ${overview.totalValue.toStringAsFixed(0)}',
                icon: Icons.attach_money,
                color: Colors.green,
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildOverviewCard(
    BuildContext context, {
    required String title,
    required String value,
    required IconData icon,
    required Color color,
  }) {
    return Card(
      elevation: 2,
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          children: [
            Icon(
              icon,
              size: 32,
              color: color,
            ),
            const SizedBox(height: 8),
            Text(
              value,
              style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                fontWeight: FontWeight.bold,
                color: color,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              title,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: Colors.grey[600],
              ),
              textAlign: TextAlign.center,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildQuickActions(StockController stockController) {
    return Row(
      children: [
        Expanded(
          child: _buildActionButton(
            context,
            icon: Icons.add_box,
            label: 'Add Item',
            onTap: () {
              // TODO: Navigate to add item screen
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(content: Text('Add item screen coming soon!')),
              );
            },
          ),
        ),
        const SizedBox(width: 16),
        Expanded(
          child: _buildActionButton(
            context,
            icon: Icons.input,
            label: 'Stock In',
            onTap: () {
              // TODO: Show stock in dialog
              _showStockAdjustmentDialog(context, stockController, MovementType.stockIn);
            },
          ),
        ),
        const SizedBox(width: 16),
        Expanded(
          child: _buildActionButton(
            context,
            icon: Icons.output,
            label: 'Stock Out',
            onTap: () {
              // TODO: Show stock out dialog
              _showStockAdjustmentDialog(context, stockController, MovementType.stockOut);
            },
          ),
        ),
      ],
    );
  }

  Widget _buildActionButton(
    BuildContext context, {
    required IconData icon,
    required String label,
    required VoidCallback onTap,
  }) {
    return ElevatedButton.icon(
      onPressed: onTap,
      icon: Icon(icon),
      label: Text(label),
      style: ElevatedButton.styleFrom(
        padding: const EdgeInsets.symmetric(vertical: 12),
      ),
    );
  }

  Widget _buildMovementsList(List<StockMovement> movements) {
    if (movements.isEmpty) {
      return const Center(
        child: Text(
          'No recent movements',
          style: TextStyle(color: Colors.grey),
        ),
      );
    }

    return ListView.builder(
      itemCount: movements.length,
      itemBuilder: (context, index) {
        final movement = movements[index];
        return _buildMovementCard(movement);
      },
    );
  }

  Widget _buildMovementCard(StockMovement movement) {
    final isIn = movement.type == 'IN';
    
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: isIn ? Colors.green[100] : Colors.red[100],
          child: Icon(
            isIn ? Icons.input : Icons.output,
            color: isIn ? Colors.green[700] : Colors.red[700],
          ),
        ),
        title: Text(
          movement.itemName,
          style: const TextStyle(fontWeight: FontWeight.w600),
        ),
        subtitle: Text('${movement.reason} • ${_formatDate(movement.date)}'),
        trailing: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Text(
              '${movement.type} ${movement.quantity}',
              style: TextStyle(
                fontWeight: FontWeight.bold,
                color: isIn ? Colors.green[700] : Colors.red[700],
              ),
            ),
            Text(
              isIn ? 'Stock In' : 'Stock Out',
              style: TextStyle(
                fontSize: 12,
                color: isIn ? Colors.green[600] : Colors.red[600],
              ),
            ),
          ],
        ),
      ),
    );
  }

  void _showFilterDialog(BuildContext context, WidgetRef ref) {
    // TODO: Implement filter dialog
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Filter Options'),
        content: const Text('Filter dialog coming soon!'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Close'),
          ),
        ],
      ),
    );
  }

  void _showStockAdjustmentDialog(
    BuildContext context,
    StockController stockController,
    MovementType type,
  ) {
    // TODO: Implement stock adjustment dialog
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text('${type.displayName} Adjustment'),
        content: const Text('Stock adjustment dialog coming soon!'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Adjust'),
          ),
        ],
      ),
    );
  }

  String _formatDate(DateTime date) {
    final now = DateTime.now();
    final difference = now.difference(date).inDays;
    
    if (difference == 0) {
      return 'Today';
    } else if (difference == 1) {
      return 'Yesterday';
    } else if (difference < 7) {
      return '$difference days ago';
    } else {
      return '${date.day}/${date.month}/${date.year}';
    }
  }
}
