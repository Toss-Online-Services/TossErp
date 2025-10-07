import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../domain/entities/inventory_movement_entity.dart';
import '../../providers/inventory/inventory_provider.dart';

class LowStockAlertsScreen extends StatefulWidget {
  const LowStockAlertsScreen({super.key});

  @override
  State<LowStockAlertsScreen> createState() => _LowStockAlertsScreenState();
}

class _LowStockAlertsScreenState extends State<LowStockAlertsScreen> {
  final TextEditingController _searchController = TextEditingController();
  String _selectedFilter = 'all';

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<InventoryProvider>().loadLowStockMovements();
    });
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Low Stock Alerts'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () {
              context.read<InventoryProvider>().loadLowStockMovements();
            },
          ),
          IconButton(
            icon: const Icon(Icons.settings),
            onPressed: () {
              _showAlertSettingsDialog();
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
                      provider.loadLowStockMovements();
                    },
                    child: const Text('Retry'),
                  ),
                ],
              ),
            );
          }

          return Column(
            children: [
              _buildSearchAndFilters(provider),
              _buildStatsCards(provider),
              Expanded(
                child: _buildAlertsList(provider),
              ),
            ],
          );
        },
      ),
    );
  }

  Widget _buildSearchAndFilters(InventoryProvider provider) {
    return Container(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          // Search bar
          TextField(
            controller: _searchController,
            decoration: InputDecoration(
              hintText: 'Search products...',
              prefixIcon: const Icon(Icons.search),
              suffixIcon: _searchController.text.isNotEmpty
                  ? IconButton(
                      icon: const Icon(Icons.clear),
                      onPressed: () {
                        _searchController.clear();
                        _filterAlerts(provider);
                      },
                    )
                  : null,
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
              ),
            ),
            onChanged: (value) {
              _filterAlerts(provider);
            },
          ),
          const SizedBox(height: 16),
          
          // Filter chips
          SingleChildScrollView(
            scrollDirection: Axis.horizontal,
            child: Row(
              children: [
                _buildFilterChip(
                  'All Alerts',
                  _selectedFilter == 'all',
                  () {
                    setState(() {
                      _selectedFilter = 'all';
                    });
                    _filterAlerts(provider);
                  },
                ),
                _buildFilterChip(
                  'Critical',
                  _selectedFilter == 'critical',
                  () {
                    setState(() {
                      _selectedFilter = 'critical';
                    });
                    _filterAlerts(provider);
                  },
                ),
                _buildFilterChip(
                  'Warning',
                  _selectedFilter == 'warning',
                  () {
                    setState(() {
                      _selectedFilter = 'warning';
                    });
                    _filterAlerts(provider);
                  },
                ),
                _buildFilterChip(
                  'Out of Stock',
                  _selectedFilter == 'out_of_stock',
                  () {
                    setState(() {
                      _selectedFilter = 'out_of_stock';
                    });
                    _filterAlerts(provider);
                  },
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildFilterChip(String label, bool isSelected, VoidCallback onTap) {
    return Padding(
      padding: const EdgeInsets.only(right: 8),
      child: FilterChip(
        label: Text(label),
        selected: isSelected,
        onSelected: (_) => onTap(),
        selectedColor: Theme.of(context).primaryColor.withOpacity(0.2),
        checkmarkColor: Theme.of(context).primaryColor,
      ),
    );
  }

  Widget _buildStatsCards(InventoryProvider provider) {
    final movements = provider.movements;
    final criticalCount = movements.length; // Simplified
    final warningCount = (movements.length * 0.3).round(); // Simplified
    final outOfStockCount = (movements.length * 0.1).round(); // Simplified

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Row(
        children: [
          Expanded(
            child: _buildStatCard(
              'Critical',
              criticalCount.toString(),
              Icons.warning,
              Colors.red,
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: _buildStatCard(
              'Warning',
              warningCount.toString(),
              Icons.warning_amber,
              Colors.orange,
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: _buildStatCard(
              'Out of Stock',
              outOfStockCount.toString(),
              Icons.error,
              Colors.red[800]!,
            ),
          ),
        ],
      ),
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

  Widget _buildAlertsList(InventoryProvider provider) {
    final movements = provider.movements;

    if (movements.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.inventory_outlined,
              size: 64,
              color: Colors.grey[400],
            ),
            const SizedBox(height: 16),
            Text(
              'No low stock alerts',
              style: Theme.of(context).textTheme.titleLarge?.copyWith(
                color: Colors.grey[600],
              ),
            ),
            const SizedBox(height: 8),
            Text(
              'All products are well stocked',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: Colors.grey[500],
              ),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 16),
            ElevatedButton.icon(
              onPressed: () {
                provider.loadLowStockMovements();
              },
              icon: const Icon(Icons.refresh),
              label: const Text('Refresh'),
            ),
          ],
        ),
      );
    }

    return ListView.builder(
      padding: const EdgeInsets.all(16),
      itemCount: movements.length,
      itemBuilder: (context, index) {
        final movement = movements[index];
        return _buildAlertCard(movement);
      },
    );
  }

  Widget _buildAlertCard(InventoryMovementEntity movement) {
    // Simulate stock levels and alert types
    final currentStock = _simulateCurrentStock(movement);
    final reorderPoint = _simulateReorderPoint(movement);
    final alertType = _getAlertType(currentStock, reorderPoint);
    
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: _getAlertColor(alertType),
          child: Icon(
            _getAlertIcon(alertType),
            color: Colors.white,
            size: 20,
          ),
        ),
        title: Text(
          movement.productId,
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Current Stock: ${currentStock.toStringAsFixed(2)} units'),
            Text('Reorder Point: ${reorderPoint.toStringAsFixed(2)} units'),
            Text('Last Movement: ${_formatDate(movement.createdAt)}'),
            if (movement.notes != null) Text('Notes: ${movement.notes}'),
          ],
        ),
        trailing: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
              decoration: BoxDecoration(
                color: _getAlertColor(alertType).withOpacity(0.2),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Text(
                alertType.toUpperCase(),
                style: TextStyle(
                  fontSize: 10,
                  fontWeight: FontWeight.bold,
                  color: _getAlertColor(alertType),
                ),
              ),
            ),
            const SizedBox(height: 4),
            Text(
              '\$${movement.totalValue.toStringAsFixed(2)}',
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                color: Colors.green,
              ),
            ),
          ],
        ),
        onTap: () {
          _showProductDetailsDialog(movement, currentStock, reorderPoint);
        },
      ),
    );
  }

  Color _getAlertColor(String alertType) {
    switch (alertType) {
      case 'critical':
        return Colors.red;
      case 'warning':
        return Colors.orange;
      case 'out_of_stock':
        return Colors.red[800]!;
      default:
        return Colors.grey;
    }
  }

  IconData _getAlertIcon(String alertType) {
    switch (alertType) {
      case 'critical':
        return Icons.warning;
      case 'warning':
        return Icons.warning_amber;
      case 'out_of_stock':
        return Icons.error;
      default:
        return Icons.info;
    }
  }

  String _getAlertType(double currentStock, double reorderPoint) {
    if (currentStock <= 0) return 'out_of_stock';
    if (currentStock <= reorderPoint * 0.5) return 'critical';
    if (currentStock <= reorderPoint) return 'warning';
    return 'normal';
  }

  double _simulateCurrentStock(InventoryMovementEntity movement) {
    // This is a simplified simulation
    // In a real app, this would come from actual product stock data
    return (movement.quantity * 0.3).abs(); // Simulate low stock
  }

  double _simulateReorderPoint(InventoryMovementEntity movement) {
    // This is a simplified simulation
    // In a real app, this would come from actual product reorder point data
    return movement.quantity * 0.5;
  }

  String _formatDate(String dateString) {
    try {
      final date = DateTime.parse(dateString);
      return '${date.day}/${date.month}/${date.year}';
    } catch (e) {
      return dateString;
    }
  }

  void _filterAlerts(InventoryProvider provider) {
    // This would implement filtering logic
    // For now, just trigger a rebuild
    setState(() {});
  }

  void _showAlertSettingsDialog() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Alert Settings'),
        content: const Text('Alert settings functionality will be implemented in the next phase.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('OK'),
          ),
        ],
      ),
    );
  }

  void _showProductDetailsDialog(
    InventoryMovementEntity movement,
    double currentStock,
    double reorderPoint,
  ) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text('Product: ${movement.productId}'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildDetailRow('Current Stock', '${currentStock.toStringAsFixed(2)} units'),
            _buildDetailRow('Reorder Point', '${reorderPoint.toStringAsFixed(2)} units'),
            _buildDetailRow('Last Movement', _formatDate(movement.createdAt)),
            _buildDetailRow('Movement Type', movement.type.name.toUpperCase()),
            _buildDetailRow('Quantity', '${movement.quantity.toStringAsFixed(2)} units'),
            _buildDetailRow('Value', '\$${movement.totalValue.toStringAsFixed(2)}'),
            if (movement.notes != null) _buildDetailRow('Notes', movement.notes!),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () {
              Navigator.pop(context);
              _showReorderDialog(movement.productId, reorderPoint);
            },
            child: const Text('Create Reorder'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Close'),
          ),
        ],
      ),
    );
  }

  Widget _buildDetailRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 100,
            child: Text(
              '$label:',
              style: TextStyle(
                fontWeight: FontWeight.w500,
                color: Colors.grey[700],
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(fontWeight: FontWeight.w400),
            ),
          ),
        ],
      ),
    );
  }

  void _showReorderDialog(String productId, double reorderPoint) {
    final quantityController = TextEditingController();
    quantityController.text = (reorderPoint * 2).toStringAsFixed(0);

    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text('Create Reorder: $productId'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text('Reorder Point: ${reorderPoint.toStringAsFixed(2)} units'),
            const SizedBox(height: 16),
            TextField(
              controller: quantityController,
              decoration: const InputDecoration(
                labelText: 'Reorder Quantity',
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.number,
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              Navigator.pop(context);
              ScaffoldMessenger.of(context).showSnackBar(
                SnackBar(
                  content: Text('Reorder created for $productId'),
                  backgroundColor: Colors.green,
                ),
              );
            },
            child: const Text('Create Reorder'),
          ),
        ],
      ),
    );
  }
}
