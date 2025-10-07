import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:intl/intl.dart';

import '../../domain/entities/sync_entity.dart';
import '../../data/services/sync_service.dart';
import '../../core/utils/status_color_util.dart';

class SyncQueueScreen extends StatefulWidget {
  const SyncQueueScreen({super.key});

  @override
  State<SyncQueueScreen> createState() => _SyncQueueScreenState();
}

class _SyncQueueScreenState extends State<SyncQueueScreen> {
  late final SyncService _syncService;
  
  List<SyncQueueEntity> _queueItems = [];
  bool _isLoading = false;
  String _filterStatus = 'all';
  SyncEntityType? _filterEntityType;
  String _sortBy = 'priority';

  @override
  void initState() {
    super.initState();
    _syncService = Provider.of<SyncService>(context, listen: false);
    _loadQueueItems();
  }

  Future<void> _loadQueueItems() async {
    setState(() => _isLoading = true);
    
    try {
      final items = await _syncService.getQueueItems();
      setState(() {
        _queueItems = _filterAndSortItems(items);
      });
    } catch (e) {
      _showErrorSnackBar('Failed to load queue items: $e');
    } finally {
      setState(() => _isLoading = false);
    }
  }

  List<SyncQueueEntity> _filterAndSortItems(List<SyncQueueEntity> items) {
    var filtered = items.where((item) {
      // Filter by status
      if (_filterStatus != 'all' && item.status.name != _filterStatus) {
        return false;
      }
      
      // Filter by entity type
      if (_filterEntityType != null && item.entityType != _filterEntityType) {
        return false;
      }
      
      return true;
    }).toList();

    // Sort items
    filtered.sort((a, b) {
      switch (_sortBy) {
        case 'priority':
          return b.priority.compareTo(a.priority);
        case 'createdAt':
          return b.createdAt.compareTo(a.createdAt);
        case 'entityType':
          return a.entityType.name.compareTo(b.entityType.name);
        case 'operation':
          return a.operationType.name.compareTo(b.operationType.name);
        default:
          return 0;
      }
    });

    return filtered;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Column(
        children: [
          _buildFilterBar(),
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : _queueItems.isEmpty
                    ? _buildEmptyState()
                    : _buildQueueList(),
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: _showAddToQueueDialog,
        child: const Icon(Icons.add),
      ),
    );
  }

  Widget _buildFilterBar() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.1),
            blurRadius: 4,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        children: [
          Row(
            children: [
              Expanded(
                child: DropdownButtonFormField<String>(
                  value: _filterStatus,
                  decoration: const InputDecoration(
                    labelText: 'Status',
                    border: OutlineInputBorder(),
                    contentPadding: EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                  ),
                  items: [
                    const DropdownMenuItem(value: 'all', child: Text('All')),
                    ...SyncStatus.values.map((status) =>
                      DropdownMenuItem(
                        value: status.name,
                        child: Text(_getStatusDisplayName(status)),
                      ),
                    ),
                  ],
                  onChanged: (value) {
                    setState(() {
                      _filterStatus = value!;
                      _queueItems = _filterAndSortItems(_queueItems);
                    });
                  },
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: DropdownButtonFormField<SyncEntityType?>(
                  value: _filterEntityType,
                  decoration: const InputDecoration(
                    labelText: 'Entity Type',
                    border: OutlineInputBorder(),
                    contentPadding: EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                  ),
                  items: [
                    const DropdownMenuItem(value: null, child: Text('All')),
                    ...SyncEntityType.values.map((type) =>
                      DropdownMenuItem(
                        value: type,
                        child: Text(_getEntityDisplayName(type)),
                      ),
                    ),
                  ],
                  onChanged: (value) {
                    setState(() {
                      _filterEntityType = value;
                      _queueItems = _filterAndSortItems(_queueItems);
                    });
                  },
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(
                child: DropdownButtonFormField<String>(
                  value: _sortBy,
                  decoration: const InputDecoration(
                    labelText: 'Sort By',
                    border: OutlineInputBorder(),
                    contentPadding: EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                  ),
                  items: const [
                    DropdownMenuItem(value: 'priority', child: Text('Priority')),
                    DropdownMenuItem(value: 'createdAt', child: Text('Created Date')),
                    DropdownMenuItem(value: 'entityType', child: Text('Entity Type')),
                    DropdownMenuItem(value: 'operation', child: Text('Operation')),
                  ],
                  onChanged: (value) {
                    setState(() {
                      _sortBy = value!;
                      _queueItems = _filterAndSortItems(_queueItems);
                    });
                  },
                ),
              ),
              const SizedBox(width: 16),
              ElevatedButton.icon(
                onPressed: _loadQueueItems,
                icon: const Icon(Icons.refresh),
                label: const Text('Refresh'),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildEmptyState() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            Icons.queue,
            size: 64,
            color: Colors.grey[400],
          ),
          const SizedBox(height: 16),
          Text(
            'No items in sync queue',
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
              color: Colors.grey[600],
            ),
          ),
          const SizedBox(height: 8),
          Text(
            'All changes have been synchronized',
            style: Theme.of(context).textTheme.bodyMedium?.copyWith(
              color: Colors.grey[500],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildQueueList() {
    return RefreshIndicator(
      onRefresh: _loadQueueItems,
      child: ListView.builder(
        padding: const EdgeInsets.all(16),
        itemCount: _queueItems.length,
        itemBuilder: (context, index) {
          final item = _queueItems[index];
          return _buildQueueItem(item);
        },
      ),
    );
  }

  Widget _buildQueueItem(SyncQueueEntity item) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ExpansionTile(
        leading: CircleAvatar(
          backgroundColor: getStatusColor(item.status),
          child: Icon(
            _getOperationIcon(item.operationType),
            color: Colors.white,
            size: 20,
          ),
        ),
        title: Text(
          '${_getEntityDisplayName(item.entityType)} - ${_getOperationDisplayName(item.operationType)}',
          style: const TextStyle(fontWeight: FontWeight.w500),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Status: ${_getStatusDisplayName(item.status)}'),
            Text('Priority: ${item.priority}'),
            Text('Created: ${DateFormat('MMM dd, yyyy HH:mm').format(item.createdAt)}'),
          ],
        ),
        trailing: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            if (item.attemptCount > 0)
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                decoration: BoxDecoration(
                  color: Colors.orange,
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Text(
                  'Retry ${item.attemptCount}',
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 10,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            PopupMenuButton<String>(
              onSelected: (value) => _handleItemAction(item, value),
              itemBuilder: (context) => [
                if (item.status == SyncStatus.pending)
                  const PopupMenuItem(
                    value: 'retry',
                    child: ListTile(
                      leading: Icon(Icons.refresh),
                      title: Text('Retry Now'),
                    ),
                  ),
                if (item.status == SyncStatus.failed)
                  const PopupMenuItem(
                    value: 'retry',
                    child: ListTile(
                      leading: Icon(Icons.refresh),
                      title: Text('Retry'),
                    ),
                  ),
                const PopupMenuItem(
                  value: 'delete',
                  child: ListTile(
                    leading: Icon(Icons.delete, color: Colors.red),
                    title: Text('Remove'),
                  ),
                ),
                const PopupMenuItem(
                  value: 'details',
                  child: ListTile(
                    leading: Icon(Icons.info),
                    title: Text('View Details'),
                  ),
                ),
              ],
            ),
          ],
        ),
        children: [
          Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _buildDetailRow('Entity ID', item.entityId),
                _buildDetailRow('Location ID', item.locationId),
                if (item.lastAttemptAt != null)
                  _buildDetailRow(
                    'Last Attempt',
                    DateFormat('MMM dd, yyyy HH:mm').format(item.lastAttemptAt!),
                  ),
                if (item.retryAfter != null)
                  _buildDetailRow(
                    'Retry After',
                    DateFormat('MMM dd, yyyy HH:mm').format(item.retryAfter!),
                  ),
                if (item.errorMessage != null)
                  _buildDetailRow('Error', item.errorMessage!, isError: true),
                const SizedBox(height: 16),
                Text(
                  'Data Preview:',
                  style: Theme.of(context).textTheme.titleSmall,
                ),
                const SizedBox(height: 8),
                Container(
                  width: double.infinity,
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: Colors.grey[100],
                    borderRadius: BorderRadius.circular(8),
                    border: Border.all(color: Colors.grey[300]!),
                  ),
                  child: Text(
                    _formatDataPreview(item.data),
                    style: const TextStyle(
                      fontFamily: 'monospace',
                      fontSize: 12,
                    ),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildDetailRow(String label, String value, {bool isError = false}) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 120,
            child: Text(
              '$label:',
              style: const TextStyle(fontWeight: FontWeight.w500),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: TextStyle(
                color: isError ? Colors.red : null,
              ),
            ),
          ),
        ],
      ),
    );
  }

  String _formatDataPreview(Map<String, dynamic> data) {
    final preview = <String, dynamic>{};
    
    // Show only key fields for preview
    const keyFields = ['id', 'name', 'title', 'description', 'amount', 'quantity'];
    
    for (final field in keyFields) {
      if (data.containsKey(field)) {
        preview[field] = data[field];
      }
    }
    
    if (preview.isEmpty) {
      // If no key fields found, show first 3 fields
      final keys = data.keys.take(3).toList();
      for (final key in keys) {
        preview[key] = data[key];
      }
    }
    
    return preview.entries
        .map((e) => '${e.key}: ${e.value}')
        .join('\n');
  }

  Future<void> _handleItemAction(SyncQueueEntity item, String action) async {
    switch (action) {
      case 'retry':
        await _retryItem(item);
        break;
      case 'delete':
        await _deleteItem(item);
        break;
      case 'details':
        _showItemDetails(item);
        break;
    }
  }

  Future<void> _retryItem(SyncQueueEntity item) async {
    try {
      await _syncService.retryQueueItem(item.id);
      _showSuccessSnackBar('Item queued for retry');
      _loadQueueItems();
    } catch (e) {
      _showErrorSnackBar('Failed to retry item: $e');
    }
  }

  Future<void> _deleteItem(SyncQueueEntity item) async {
    final confirmed = await _showConfirmDialog(
      'Remove Item',
      'Are you sure you want to remove this item from the queue?',
    );

    if (!confirmed) return;

    try {
      await _syncService.removeFromQueue(item.id);
      _showSuccessSnackBar('Item removed from queue');
      _loadQueueItems();
    } catch (e) {
      _showErrorSnackBar('Failed to remove item: $e');
    }
  }

  void _showItemDetails(SyncQueueEntity item) {
    showDialog(
      context: context,
      builder: (context) => Dialog(
        child: Container(
          padding: const EdgeInsets.all(16),
          constraints: const BoxConstraints(maxWidth: 500, maxHeight: 600),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Text(
                    'Queue Item Details',
                    style: Theme.of(context).textTheme.headlineSmall,
                  ),
                  const Spacer(),
                  IconButton(
                    onPressed: () => Navigator.of(context).pop(),
                    icon: const Icon(Icons.close),
                  ),
                ],
              ),
              const Divider(),
              Expanded(
                child: SingleChildScrollView(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      _buildDetailRow('ID', item.id),
                      _buildDetailRow('Entity Type', _getEntityDisplayName(item.entityType)),
                      _buildDetailRow('Operation', _getOperationDisplayName(item.operationType)),
                      _buildDetailRow('Status', _getStatusDisplayName(item.status)),
                      _buildDetailRow('Priority', item.priority.toString()),
                      _buildDetailRow('Entity ID', item.entityId),
                      _buildDetailRow('Location ID', item.locationId),
                      _buildDetailRow('Retry Count', item.attemptCount.toString()),
                      _buildDetailRow(
                        'Created At',
                        DateFormat('MMM dd, yyyy HH:mm:ss').format(item.createdAt),
                      ),
                      if (item.lastAttemptAt != null)
                        _buildDetailRow(
                          'Last Attempt',
                          DateFormat('MMM dd, yyyy HH:mm:ss').format(item.lastAttemptAt!),
                        ),
                      if (item.retryAfter != null)
                        _buildDetailRow(
                          'Retry After',
                          DateFormat('MMM dd, yyyy HH:mm:ss').format(item.retryAfter!),
                        ),
                      if (item.dependsOnId != null)
                        _buildDetailRow('Depends On', item.dependsOnId!),
                      if (item.errorMessage != null)
                        _buildDetailRow('Error', item.errorMessage!, isError: true),
                      const SizedBox(height: 16),
                      Text(
                        'Full Data:',
                        style: Theme.of(context).textTheme.titleSmall,
                      ),
                      const SizedBox(height: 8),
                      Container(
                        width: double.infinity,
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: Colors.grey[100],
                          borderRadius: BorderRadius.circular(8),
                          border: Border.all(color: Colors.grey[300]!),
                        ),
                        child: SelectableText(
                          item.data.toString(),
                          style: const TextStyle(
                            fontFamily: 'monospace',
                            fontSize: 12,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  void _showAddToQueueDialog() {
    // This would open a dialog to manually add items to the queue
    // Implementation depends on specific requirements
    _showSuccessSnackBar('Add to queue feature coming soon');
  }

  IconData _getOperationIcon(SyncOperationType operation) {
    switch (operation) {
      case SyncOperationType.create:
        return Icons.add;
      case SyncOperationType.update:
        return Icons.edit;
      case SyncOperationType.delete:
        return Icons.delete;
      case SyncOperationType.transfer:
        return Icons.swap_horiz;
      case SyncOperationType.payment:
        return Icons.payment;
    }
  }

  String _getStatusDisplayName(SyncStatus status) {
    switch (status) {
      case SyncStatus.pending:
        return 'Pending';
      case SyncStatus.inProgress:
        return 'Processing';
      case SyncStatus.completed:
        return 'Completed';
      case SyncStatus.failed:
        return 'Failed';
      case SyncStatus.conflict:
        return 'Conflict';
      case SyncStatus.retrying:
        return 'Retrying';
    }
  }

  String _getOperationDisplayName(SyncOperationType operation) {
    switch (operation) {
      case SyncOperationType.create:
        return 'Create';
      case SyncOperationType.update:
        return 'Update';
      case SyncOperationType.delete:
        return 'Delete';
      case SyncOperationType.transfer:
        return 'Transfer';
      case SyncOperationType.payment:
        return 'Payment';
    }
  }

  String _getEntityDisplayName(SyncEntityType entityType) {
    switch (entityType) {
      case SyncEntityType.transaction:
        return 'Transaction';
      case SyncEntityType.product:
        return 'Product';
      case SyncEntityType.customer:
        return 'Customer';
      case SyncEntityType.inventory:
        return 'Inventory';
      case SyncEntityType.discount:
        return 'Discount';
      case SyncEntityType.employee:
        return 'Employee';
      case SyncEntityType.location:
        return 'Location';
      case SyncEntityType.transfer:
        return 'Transfer';
    }
  }

  Future<bool> _showConfirmDialog(String title, String message) async {
    final result = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(title),
        content: Text(message),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () => Navigator.of(context).pop(true),
            child: const Text('Confirm'),
          ),
        ],
      ),
    );
    return result ?? false;
  }

  void _showSuccessSnackBar(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: Colors.green,
        behavior: SnackBarBehavior.floating,
      ),
    );
  }

  void _showErrorSnackBar(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: Colors.red,
        behavior: SnackBarBehavior.floating,
      ),
    );
  }
}
