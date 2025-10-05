import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../domain/entities/inventory_movement_entity.dart';
import '../../providers/inventory/inventory_provider.dart';

class BatchManagementScreen extends StatefulWidget {
  const BatchManagementScreen({super.key});

  @override
  State<BatchManagementScreen> createState() => _BatchManagementScreenState();
}

class _BatchManagementScreenState extends State<BatchManagementScreen> {
  final TextEditingController _searchController = TextEditingController();
  String _selectedFilter = 'all';

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<InventoryProvider>().loadAllMovements();
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
        title: const Text('Batch Management'),
        actions: [
          IconButton(
            icon: const Icon(Icons.add),
            onPressed: () {
              _showCreateBatchDialog();
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

          return Column(
            children: [
              _buildSearchAndFilters(provider),
              Expanded(
                child: _buildBatchList(provider),
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
              hintText: 'Search batches...',
              prefixIcon: const Icon(Icons.search),
              suffixIcon: _searchController.text.isNotEmpty
                  ? IconButton(
                      icon: const Icon(Icons.clear),
                      onPressed: () {
                        _searchController.clear();
                        _filterBatches(provider);
                      },
                    )
                  : null,
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
              ),
            ),
            onChanged: (value) {
              _filterBatches(provider);
            },
          ),
          const SizedBox(height: 16),
          
          // Filter chips
          SingleChildScrollView(
            scrollDirection: Axis.horizontal,
            child: Row(
              children: [
                _buildFilterChip(
                  'All',
                  _selectedFilter == 'all',
                  () {
                    setState(() {
                      _selectedFilter = 'all';
                    });
                    _filterBatches(provider);
                  },
                ),
                _buildFilterChip(
                  'With Batches',
                  _selectedFilter == 'with_batches',
                  () {
                    setState(() {
                      _selectedFilter = 'with_batches';
                    });
                    _filterBatches(provider);
                  },
                ),
                _buildFilterChip(
                  'Expiring Soon',
                  _selectedFilter == 'expiring',
                  () {
                    setState(() {
                      _selectedFilter = 'expiring';
                    });
                    _filterBatches(provider);
                  },
                ),
                _buildFilterChip(
                  'Low Stock',
                  _selectedFilter == 'low_stock',
                  () {
                    setState(() {
                      _selectedFilter = 'low_stock';
                    });
                    _filterBatches(provider);
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

  Widget _buildBatchList(InventoryProvider provider) {
    final movements = provider.movements;
    final batchGroups = _groupMovementsByBatch(movements);

    if (batchGroups.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.inventory_2_outlined,
              size: 64,
              color: Colors.grey[400],
            ),
            const SizedBox(height: 16),
            Text(
              'No batch data found',
              style: Theme.of(context).textTheme.titleLarge?.copyWith(
                color: Colors.grey[600],
              ),
            ),
            const SizedBox(height: 8),
            Text(
              'Create inventory movements with batch IDs to see batch information',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: Colors.grey[500],
              ),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 16),
            ElevatedButton.icon(
              onPressed: () {
                _showCreateBatchDialog();
              },
              icon: const Icon(Icons.add),
              label: const Text('Create Batch'),
            ),
          ],
        ),
      );
    }

    return ListView.builder(
      padding: const EdgeInsets.all(16),
      itemCount: batchGroups.length,
      itemBuilder: (context, index) {
        final entry = batchGroups.entries.elementAt(index);
        final batchId = entry.key;
        final movements = entry.value;
        
        return _buildBatchCard(batchId, movements);
      },
    );
  }

  Widget _buildBatchCard(String batchId, List<InventoryMovementEntity> movements) {
    final totalQuantity = movements.fold(0.0, (sum, m) => sum + m.quantity);
    final totalValue = movements.fold(0.0, (sum, m) => sum + m.totalValue);
    final firstMovement = movements.first;
    final lastMovement = movements.last;
    
    // Calculate current stock (simplified)
    final currentStock = movements
        .where((m) => m.type == InventoryMovementType.purchase)
        .fold(0.0, (sum, m) => sum + m.quantity) -
        movements
            .where((m) => m.type == InventoryMovementType.sale)
            .fold(0.0, (sum, m) => sum + m.quantity);

    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ExpansionTile(
        title: Text(
          batchId,
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Product: ${firstMovement.productId}'),
            Text('Current Stock: ${currentStock.toStringAsFixed(2)}'),
            Text('Total Value: \$${totalValue.toStringAsFixed(2)}'),
          ],
        ),
        leading: CircleAvatar(
          backgroundColor: _getBatchStatusColor(currentStock),
          child: Text(
            batchId.substring(0, 1).toUpperCase(),
            style: const TextStyle(
              color: Colors.white,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        children: [
          Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _buildBatchInfoRow('Batch ID', batchId),
                _buildBatchInfoRow('Product ID', firstMovement.productId),
                _buildBatchInfoRow('Current Stock', '${currentStock.toStringAsFixed(2)} units'),
                _buildBatchInfoRow('Total Value', '\$${totalValue.toStringAsFixed(2)}'),
                _buildBatchInfoRow('First Movement', _formatDate(firstMovement.createdAt)),
                _buildBatchInfoRow('Last Movement', _formatDate(lastMovement.createdAt)),
                _buildBatchInfoRow('Total Movements', '${movements.length}'),
                const SizedBox(height: 16),
                const Text(
                  'Movement History:',
                  style: TextStyle(fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 8),
                ...movements.take(5).map((movement) => _buildMovementItem(movement)),
                if (movements.length > 5)
                  TextButton(
                    onPressed: () {
                      _showBatchDetailsDialog(batchId, movements);
                    },
                    child: Text('View all ${movements.length} movements'),
                  ),
                const SizedBox(height: 16),
                Row(
                  children: [
                    Expanded(
                      child: OutlinedButton.icon(
                        onPressed: () {
                          _showBatchTransferDialog(batchId, currentStock);
                        },
                        icon: const Icon(Icons.swap_horiz),
                        label: const Text('Transfer'),
                      ),
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: OutlinedButton.icon(
                        onPressed: () {
                          _showBatchAdjustmentDialog(batchId, currentStock);
                        },
                        icon: const Icon(Icons.edit),
                        label: const Text('Adjust'),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildBatchInfoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 120,
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

  Widget _buildMovementItem(InventoryMovementEntity movement) {
    return Container(
      margin: const EdgeInsets.only(bottom: 4),
      padding: const EdgeInsets.all(8),
      decoration: BoxDecoration(
        color: Colors.grey[100],
        borderRadius: BorderRadius.circular(4),
      ),
      child: Row(
        children: [
          Icon(
            _getMovementTypeIcon(movement.type),
            size: 16,
            color: _getMovementTypeColor(movement.type),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              '${movement.type.name.toUpperCase()}: ${movement.quantity} units',
              style: const TextStyle(fontSize: 12),
            ),
          ),
          Text(
            _formatDate(movement.createdAt),
            style: TextStyle(
              fontSize: 12,
              color: Colors.grey[600],
            ),
          ),
        ],
      ),
    );
  }

  Color _getBatchStatusColor(double currentStock) {
    if (currentStock <= 0) return Colors.red;
    if (currentStock <= 10) return Colors.orange;
    return Colors.green;
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

  Map<String, List<InventoryMovementEntity>> _groupMovementsByBatch(
    List<InventoryMovementEntity> movements,
  ) {
    final Map<String, List<InventoryMovementEntity>> groups = {};
    
    for (final movement in movements) {
      if (movement.batchId != null && movement.batchId!.isNotEmpty) {
        if (!groups.containsKey(movement.batchId)) {
          groups[movement.batchId!] = [];
        }
        groups[movement.batchId!]!.add(movement);
      }
    }
    
    // Sort movements within each batch by date
    for (final batch in groups.values) {
      batch.sort((a, b) => b.createdAt.compareTo(a.createdAt));
    }
    
    return groups;
  }

  void _filterBatches(InventoryProvider provider) {
    // This would implement filtering logic
    // For now, just trigger a rebuild
    setState(() {});
  }

  void _showCreateBatchDialog() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Create New Batch'),
        content: const Text('Batch creation functionality will be implemented in the next phase.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('OK'),
          ),
        ],
      ),
    );
  }

  void _showBatchDetailsDialog(String batchId, List<InventoryMovementEntity> movements) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text('Batch Details: $batchId'),
        content: SizedBox(
          width: double.maxFinite,
          height: 400,
          child: ListView.builder(
            itemCount: movements.length,
            itemBuilder: (context, index) {
              final movement = movements[index];
              return ListTile(
                leading: Icon(
                  _getMovementTypeIcon(movement.type),
                  color: _getMovementTypeColor(movement.type),
                ),
                title: Text(movement.type.name.toUpperCase()),
                subtitle: Text('${movement.quantity} units - ${_formatDate(movement.createdAt)}'),
                trailing: Text('\$${movement.totalValue.toStringAsFixed(2)}'),
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

  void _showBatchTransferDialog(String batchId, double currentStock) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text('Transfer Batch: $batchId'),
        content: Text('Current stock: ${currentStock.toStringAsFixed(2)} units\n\nBatch transfer functionality will be implemented in the next phase.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('OK'),
          ),
        ],
      ),
    );
  }

  void _showBatchAdjustmentDialog(String batchId, double currentStock) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text('Adjust Batch: $batchId'),
        content: Text('Current stock: ${currentStock.toStringAsFixed(2)} units\n\nBatch adjustment functionality will be implemented in the next phase.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('OK'),
          ),
        ],
      ),
    );
  }
}
