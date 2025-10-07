import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:intl/intl.dart';
import 'dart:convert';

import '../../domain/entities/sync_entity.dart';
import '../../data/services/sync_service.dart';

class SyncConflictsScreen extends StatefulWidget {
  const SyncConflictsScreen({super.key});

  @override
  State<SyncConflictsScreen> createState() => _SyncConflictsScreenState();
}

class _SyncConflictsScreenState extends State<SyncConflictsScreen> {
  late final SyncService _syncService;
  
  List<SyncConflict> _conflicts = [];
  bool _isLoading = false;
  SyncEntityType? _filterEntityType;

  @override
  void initState() {
    super.initState();
    _syncService = Provider.of<SyncService>(context, listen: false);
    _loadConflicts();
  }

  Future<void> _loadConflicts() async {
    setState(() => _isLoading = true);
    
    try {
      final conflicts = await _syncService.getPendingConflicts();
      setState(() {
        _conflicts = _filterConflicts(conflicts);
      });
    } catch (e) {
      _showErrorSnackBar('Failed to load conflicts: $e');
    } finally {
      setState(() => _isLoading = false);
    }
  }

  List<SyncConflict> _filterConflicts(List<SyncConflict> conflicts) {
    return conflicts.where((conflict) {
      // Status filter removed; all returned conflicts are pending/unresolved
      // Filter by entity type
      if (_filterEntityType != null && conflict.entityType != _filterEntityType) {
        return false;
      }
      
      return true;
    }).toList();
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
                : _conflicts.isEmpty
                    ? _buildEmptyState()
                    : _buildConflictsList(),
          ),
        ],
      ),
      floatingActionButton: _conflicts.isNotEmpty
          ? FloatingActionButton.extended(
              onPressed: _resolveAllConflicts,
              icon: const Icon(Icons.auto_fix_high),
              label: const Text('Auto Resolve'),
            )
          : null,
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
      child: Row(
        children: [
          // Removed status filter; only entity type filter remains
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
                  _conflicts = _filterConflicts(_conflicts);
                });
              },
            ),
          ),
          const SizedBox(width: 16),
          ElevatedButton.icon(
            onPressed: _loadConflicts,
            icon: const Icon(Icons.refresh),
            label: const Text('Refresh'),
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
            Icons.check_circle,
            size: 64,
            color: Colors.green[400],
          ),
          const SizedBox(height: 16),
          Text(
            'No conflicts found',
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
              color: Colors.green[600],
            ),
          ),
          const SizedBox(height: 8),
          Text(
            'All data is synchronized properly',
            style: Theme.of(context).textTheme.bodyMedium?.copyWith(
              color: Colors.grey[500],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildConflictsList() {
    return RefreshIndicator(
      onRefresh: _loadConflicts,
      child: ListView.builder(
        padding: const EdgeInsets.all(16),
        itemCount: _conflicts.length,
        itemBuilder: (context, index) {
          final conflict = _conflicts[index];
          return _buildConflictItem(conflict);
        },
      ),
    );
  }
  
  Widget _buildConflictItem(SyncConflict conflict) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      elevation: 2,
      child: ExpansionTile(
        leading: const CircleAvatar(
          backgroundColor: Colors.redAccent,
          child: Icon(Icons.warning, color: Colors.white, size: 20),
        ),
        title: Text(
          '${_getEntityDisplayName(conflict.entityType)} Conflict',
          style: const TextStyle(fontWeight: FontWeight.w600),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('ID: ${conflict.entityId}'),
            if (conflict.suggestedStrategy != null)
              Text('Suggested: ${_getResolutionDisplayName(conflict.suggestedStrategy!)}'),
            Text('Detected: ${DateFormat('MMM dd, yyyy HH:mm').format(conflict.detectedAt)}'),
          ],
        ),
        trailing: PopupMenuButton<String>(
          onSelected: (value) => _handleQuickResolve(conflict, value),
          itemBuilder: (context) => const [
            PopupMenuItem(
              value: 'localWins',
              child: ListTile(
                leading: Icon(Icons.phone_android, color: Colors.blue),
                title: Text('Keep Local'),
                dense: true,
              ),
            ),
            PopupMenuItem(
              value: 'remoteWins',
              child: ListTile(
                leading: Icon(Icons.cloud, color: Colors.green),
                title: Text('Keep Remote'),
                dense: true,
              ),
            ),
            PopupMenuItem(
              value: 'merge',
              child: ListTile(
                leading: Icon(Icons.merge, color: Colors.orange),
                title: Text('Merge'),
                dense: true,
              ),
            ),
            PopupMenuItem(
              value: 'manual',
              child: ListTile(
                leading: Icon(Icons.edit, color: Colors.purple),
                title: Text('Manual'),
                dense: true,
              ),
            ),
          ],
        ),
        children: [
          Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Expanded(
                      child: _buildDataView(
                        'Local Data',
                        conflict.localData,
                        Icons.phone_android,
                        Colors.blue,
                      ),
                    ),
                    const SizedBox(width: 16),
                    Expanded(
                      child: _buildDataView(
                        'Remote Data',
                        conflict.remoteData,
                        Icons.cloud,
                        Colors.green,
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 24),
                _buildResolutionButtons(conflict),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildDataView(
    String title,
    Map<String, dynamic> data,
    IconData icon,
    Color color,
  ) {
    return Container(
      decoration: BoxDecoration(
        border: Border.all(color: color.withOpacity(0.3)),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: double.infinity,
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              color: color.withOpacity(0.1),
              borderRadius: const BorderRadius.only(
                topLeft: Radius.circular(8),
                topRight: Radius.circular(8),
              ),
            ),
            child: Row(
              children: [
                Icon(icon, color: color, size: 18),
                const SizedBox(width: 8),
                Text(
                  title,
                  style: TextStyle(
                    fontWeight: FontWeight.w600,
                    color: color,
                  ),
                ),
              ],
            ),
          ),
          Container(
            width: double.infinity,
            padding: const EdgeInsets.all(12),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: _buildDataFields(data),
            ),
          ),
        ],
      ),
    );
  }

  List<Widget> _buildDataFields(Map<String, dynamic> data) {
    final widgets = <Widget>[];
    final sortedEntries = data.entries.toList()
      ..sort((a, b) => a.key.compareTo(b.key));

    for (final entry in sortedEntries.take(5)) {
      widgets.add(
        Padding(
          padding: const EdgeInsets.only(bottom: 4),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              SizedBox(
                width: 80,
                child: Text(
                  '${entry.key}:',
                  style: const TextStyle(
                    fontWeight: FontWeight.w500,
                    fontSize: 12,
                  ),
                ),
              ),
              Expanded(
                child: Text(
                  entry.value?.toString() ?? 'null',
                  style: const TextStyle(fontSize: 12),
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                ),
              ),
            ],
          ),
        ),
      );
    }

    if (data.length > 5) {
      widgets.add(
        Text(
          '... and ${data.length - 5} more fields',
          style: TextStyle(
            fontSize: 12,
            color: Colors.grey[600],
            fontStyle: FontStyle.italic,
          ),
        ),
      );
    }

    return widgets;
  }

  Widget _buildResolutionButtons(SyncConflict conflict) {
    return Wrap(
      spacing: 8,
      runSpacing: 8,
      children: [
        ElevatedButton.icon(
          onPressed: () => _resolveConflict(conflict, ConflictResolutionStrategy.localWins),
          icon: const Icon(Icons.phone_android, size: 18),
          label: const Text('Keep Local'),
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.blue,
            foregroundColor: Colors.white,
          ),
        ),
        ElevatedButton.icon(
          onPressed: () => _resolveConflict(conflict, ConflictResolutionStrategy.remoteWins),
          icon: const Icon(Icons.cloud, size: 18),
          label: const Text('Keep Remote'),
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.green,
            foregroundColor: Colors.white,
          ),
        ),
        ElevatedButton.icon(
          onPressed: () => _resolveConflict(conflict, ConflictResolutionStrategy.merge),
          icon: const Icon(Icons.merge, size: 18),
          label: const Text('Merge'),
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.orange,
            foregroundColor: Colors.white,
          ),
        ),
        ElevatedButton.icon(
          onPressed: () => _showManualResolutionDialog(conflict),
          icon: const Icon(Icons.edit, size: 18),
          label: const Text('Manual'),
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.purple,
            foregroundColor: Colors.white,
          ),
        ),
        OutlinedButton.icon(
          onPressed: () => _showDetailedView(conflict),
          icon: const Icon(Icons.visibility, size: 18),
          label: const Text('View Details'),
        ),
      ],
    );
  }

  Future<void> _resolveConflict(SyncConflict conflict, ConflictResolutionStrategy resolution) async {
    try {
      await _syncService.resolveConflict(conflict.id, resolution);
      _showSuccessSnackBar('Conflict resolved successfully');
      _loadConflicts();
    } catch (e) {
      _showErrorSnackBar('Failed to resolve conflict: $e');
    }
  }

  Future<void> _handleQuickResolve(SyncConflict conflict, String resolutionType) async {
    ConflictResolutionStrategy resolution;
    switch (resolutionType) {
      case 'localWins':
        resolution = ConflictResolutionStrategy.localWins;
        break;
      case 'remoteWins':
        resolution = ConflictResolutionStrategy.remoteWins;
        break;
      case 'merge':
        resolution = ConflictResolutionStrategy.merge;
        break;
      case 'manual':
        _showManualResolutionDialog(conflict);
        return;
      default:
        return;
    }

    await _resolveConflict(conflict, resolution);
  }

  Future<void> _resolveAllConflicts() async {
    final unresolvedConflicts = List<SyncConflict>.from(_conflicts);

    if (unresolvedConflicts.isEmpty) {
      _showSuccessSnackBar('No unresolved conflicts found');
      return;
    }

    final confirmed = await _showConfirmDialog(
      'Auto Resolve Conflicts',
      'This will automatically resolve ${unresolvedConflicts.length} conflicts using the default resolution strategy. Continue?',
    );

    if (!confirmed) return;

    try {
      for (final c in unresolvedConflicts) {
        final strategy = c.suggestedStrategy ?? ConflictResolutionStrategy.remoteWins;
        await _syncService.resolveConflict(c.id, strategy);
      }
      _showSuccessSnackBar('All conflicts resolved automatically');
      _loadConflicts();
    } catch (e) {
      _showErrorSnackBar('Failed to resolve all conflicts: $e');
    }
  }

  void _showManualResolutionDialog(SyncConflict conflict) {
    showDialog(
      context: context,
      builder: (context) => _ManualResolutionDialog(
        conflict: conflict,
        onResolve: (mergedData) async {
          try {
            await _syncService.resolveConflict(
              conflict.id,
              ConflictResolutionStrategy.manual,
              customData: mergedData,
            );
            _showSuccessSnackBar('Conflict resolved with custom data');
            _loadConflicts();
          } catch (e) {
            _showErrorSnackBar('Failed to resolve conflict: $e');
          }
        },
      ),
    );
  }

  void _showDetailedView(SyncConflict conflict) {
    showDialog(
      context: context,
      builder: (context) => Dialog(
        child: Container(
          padding: const EdgeInsets.all(16),
          constraints: const BoxConstraints(maxWidth: 800, maxHeight: 600),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Text(
                    'Conflict Details',
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
                      _buildDetailRow('Conflict ID', conflict.id),
                      _buildDetailRow('Entity Type', _getEntityDisplayName(conflict.entityType)),
                      _buildDetailRow('Entity ID', conflict.entityId),
                      if (conflict.suggestedStrategy != null)
                        _buildDetailRow('Suggested', _getResolutionDisplayName(conflict.suggestedStrategy!)),
                      _buildDetailRow(
                        'Detected At',
                        DateFormat('MMM dd, yyyy HH:mm:ss').format(conflict.detectedAt),
                      ),
                      const SizedBox(height: 24),
                      Text(
                        'Local Data:',
                        style: Theme.of(context).textTheme.titleMedium,
                      ),
                      const SizedBox(height: 8),
                      Container(
                        width: double.infinity,
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: Colors.blue.withOpacity(0.1),
                          borderRadius: BorderRadius.circular(8),
                          border: Border.all(color: Colors.blue.withOpacity(0.3)),
                        ),
                        child: SelectableText(
                          const JsonEncoder.withIndent('  ').convert(conflict.localData),
                          style: const TextStyle(
                            fontFamily: 'monospace',
                            fontSize: 12,
                          ),
                        ),
                      ),
                      const SizedBox(height: 16),
                      Text(
                        'Remote Data:',
                        style: Theme.of(context).textTheme.titleMedium,
                      ),
                      const SizedBox(height: 8),
                      Container(
                        width: double.infinity,
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: Colors.green.withOpacity(0.1),
                          borderRadius: BorderRadius.circular(8),
                          border: Border.all(color: Colors.green.withOpacity(0.3)),
                        ),
                        child: SelectableText(
                          const JsonEncoder.withIndent('  ').convert(conflict.remoteData),
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

  Widget _buildDetailRow(String label, String value) {
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
          Expanded(child: Text(value)),
        ],
      ),
    );
  }

  String _getResolutionDisplayName(ConflictResolutionStrategy resolution) {
    switch (resolution) {
      case ConflictResolutionStrategy.localWins:
        return 'Local Wins';
      case ConflictResolutionStrategy.remoteWins:
        return 'Remote Wins';
      case ConflictResolutionStrategy.merge:
        return 'Merged';
      case ConflictResolutionStrategy.manual:
        return 'Manual';
      case ConflictResolutionStrategy.keepBoth:
        return 'Keep Both';
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
            child: const Text('Continue'),
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

class _ManualResolutionDialog extends StatefulWidget {
  final SyncConflict conflict;
  final Function(Map<String, dynamic>) onResolve;

  const _ManualResolutionDialog({
    required this.conflict,
    required this.onResolve,
  });

  @override
  State<_ManualResolutionDialog> createState() => _ManualResolutionDialogState();
}

class _ManualResolutionDialogState extends State<_ManualResolutionDialog> {
  late Map<String, dynamic> _mergedData;
  final Map<String, TextEditingController> _controllers = {};

  @override
  void initState() {
    super.initState();
    _mergedData = Map<String, dynamic>.from(widget.conflict.localData);
    
    // Initialize controllers for all fields
    final allKeys = <String>{
      ...widget.conflict.localData.keys,
      ...widget.conflict.remoteData.keys,
    };

    for (final key in allKeys) {
      final value = _mergedData[key]?.toString() ?? '';
      _controllers[key] = TextEditingController(text: value);
    }
  }

  @override
  void dispose() {
    for (final controller in _controllers.values) {
      controller.dispose();
    }
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Dialog(
      child: Container(
        padding: const EdgeInsets.all(16),
        constraints: const BoxConstraints(maxWidth: 800, maxHeight: 600),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Text(
                  'Manual Conflict Resolution',
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
                  children: _buildFieldEditors(),
                ),
              ),
            ),
            const SizedBox(height: 16),
            Row(
              mainAxisAlignment: MainAxisAlignment.end,
              children: [
                TextButton(
                  onPressed: () => Navigator.of(context).pop(),
                  child: const Text('Cancel'),
                ),
                const SizedBox(width: 8),
                ElevatedButton(
                  onPressed: _saveResolution,
                  child: const Text('Resolve'),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  List<Widget> _buildFieldEditors() {
    final allKeys = <String>{
      ...widget.conflict.localData.keys,
      ...widget.conflict.remoteData.keys,
    };

    return allKeys.map((key) {
      final localValue = widget.conflict.localData[key];
      final remoteValue = widget.conflict.remoteData[key];
      final hasConflict = localValue != remoteValue;

      return Container(
        margin: const EdgeInsets.only(bottom: 16),
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          border: Border.all(
            color: hasConflict ? Colors.red.withOpacity(0.3) : Colors.grey.withOpacity(0.3),
          ),
          borderRadius: BorderRadius.circular(8),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Text(
                  key,
                  style: const TextStyle(fontWeight: FontWeight.w600),
                ),
                if (hasConflict) ...[
                  const SizedBox(width: 8),
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                    decoration: BoxDecoration(
                      color: Colors.red,
                      borderRadius: BorderRadius.circular(10),
                    ),
                    child: const Text(
                      'CONFLICT',
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 10,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ],
              ],
            ),
            const SizedBox(height: 8),
            if (hasConflict) ...[
              Row(
                children: [
                  Expanded(
                    child: Container(
                      padding: const EdgeInsets.all(8),
                      decoration: BoxDecoration(
                        color: Colors.blue.withOpacity(0.1),
                        borderRadius: BorderRadius.circular(4),
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text(
                            'Local:',
                            style: TextStyle(
                              fontWeight: FontWeight.w500,
                              color: Colors.blue,
                            ),
                          ),
                          Text(localValue?.toString() ?? 'null'),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Container(
                      padding: const EdgeInsets.all(8),
                      decoration: BoxDecoration(
                        color: Colors.green.withOpacity(0.1),
                        borderRadius: BorderRadius.circular(4),
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          const Text(
                            'Remote:',
                            style: TextStyle(
                              fontWeight: FontWeight.w500,
                              color: Colors.green,
                            ),
                          ),
                          Text(remoteValue?.toString() ?? 'null'),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              Row(
                children: [
                  ElevatedButton(
                    onPressed: () => _setFieldValue(key, localValue),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.blue,
                      foregroundColor: Colors.white,
                    ),
                    child: const Text('Use Local'),
                  ),
                  const SizedBox(width: 8),
                  ElevatedButton(
                    onPressed: () => _setFieldValue(key, remoteValue),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.green,
                      foregroundColor: Colors.white,
                    ),
                    child: const Text('Use Remote'),
                  ),
                ],
              ),
              const SizedBox(height: 8),
            ],
            TextFormField(
              controller: _controllers[key],
              decoration: const InputDecoration(
                labelText: 'Resolved Value',
                border: OutlineInputBorder(),
              ),
              maxLines: null,
            ),
          ],
        ),
      );
    }).toList();
  }

  void _setFieldValue(String key, dynamic value) {
    _controllers[key]?.text = value?.toString() ?? '';
  }

  void _saveResolution() {
    final resolvedData = <String, dynamic>{};
    
    for (final entry in _controllers.entries) {
      final key = entry.key;
      final value = entry.value.text;
      
      // Try to parse the value to the appropriate type
      if (value.isEmpty) {
        resolvedData[key] = null;
      } else if (value == 'true' || value == 'false') {
        resolvedData[key] = value == 'true';
      } else if (RegExp(r'^\d+$').hasMatch(value)) {
        resolvedData[key] = int.tryParse(value) ?? value;
      } else if (RegExp(r'^\d+\.\d+$').hasMatch(value)) {
        resolvedData[key] = double.tryParse(value) ?? value;
      } else {
        resolvedData[key] = value;
      }
    }

    widget.onResolve(resolvedData);
    Navigator.of(context).pop();
  }
}
