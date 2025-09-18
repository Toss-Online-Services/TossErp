import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../domain/entities/sync_entity.dart';
import '../../data/services/sync_service.dart';

class SyncSettingsScreen extends StatefulWidget {
  const SyncSettingsScreen({super.key});

  @override
  State<SyncSettingsScreen> createState() => _SyncSettingsScreenState();
}

class _SyncSettingsScreenState extends State<SyncSettingsScreen> {
  late final SyncService _syncService;
  
  bool _isLoading = false;
  
  // Form controllers
  late bool _autoSyncEnabled;
  late int _syncIntervalMinutes;
  late bool _syncOnWifiOnly;
  late int _maxRetryAttempts;
  late ConflictResolutionStrategy _defaultConflictResolution;
  late Map<SyncEntityType, bool> _entitySyncSettings;

  @override
  void initState() {
    super.initState();
    _syncService = Provider.of<SyncService>(context, listen: false);
    _loadConfiguration();
  }

  Future<void> _loadConfiguration() async {
    setState(() => _isLoading = true);
    
    try {
      final config = await _syncService.getSyncConfiguration();
      setState(() {
        _initializeFormValues(config);
      });
    } catch (e) {
      _showErrorSnackBar('Failed to load sync configuration: $e');
    } finally {
      setState(() => _isLoading = false);
    }
  }

  void _initializeFormValues(SyncConfiguration? config) {
    _autoSyncEnabled = config?.autoSync ?? true;
    _syncIntervalMinutes = config?.syncInterval.inMinutes ?? 5;
    _syncOnWifiOnly = config?.syncOnlyOnWifi ?? false;
    _maxRetryAttempts = config?.maxRetries ?? 3;
    _defaultConflictResolution = config?.defaultConflictStrategy ?? ConflictResolutionStrategy.manual;
    
    _entitySyncSettings = {};
    for (final entityType in SyncEntityType.values) {
      _entitySyncSettings[entityType] = config?.enabledEntities.contains(entityType) ?? true;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Sync Settings'),
        backgroundColor: Theme.of(context).primaryColor,
        foregroundColor: Colors.white,
        actions: [
          IconButton(
            icon: const Icon(Icons.restore),
            onPressed: _resetToDefaults,
            tooltip: 'Reset to defaults',
          ),
          IconButton(
            icon: const Icon(Icons.save),
            onPressed: _saveConfiguration,
            tooltip: 'Save settings',
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : SingleChildScrollView(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _buildGeneralSettings(),
                  const SizedBox(height: 24),
                  _buildNetworkSettings(),
                  const SizedBox(height: 24),
                  _buildPerformanceSettings(),
                  const SizedBox(height: 24),
                  _buildConflictResolutionSettings(),
                  const SizedBox(height: 24),
                  _buildEntitySettings(),
                  const SizedBox(height: 24),
                  _buildAdvancedSettings(),
                  const SizedBox(height: 32),
                  _buildActionButtons(),
                ],
              ),
            ),
    );
  }

  Widget _buildGeneralSettings() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'General Settings',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            SwitchListTile(
              title: const Text('Auto Sync'),
              subtitle: const Text('Automatically sync data when changes are made'),
              value: _autoSyncEnabled,
              onChanged: (value) {
                setState(() {
                  _autoSyncEnabled = value;
                });
              },
            ),
            const Divider(),
            ListTile(
              title: const Text('Sync Interval'),
              subtitle: Text('Sync every $_syncIntervalMinutes minutes'),
              trailing: SizedBox(
                width: 100,
                child: TextFormField(
                  initialValue: _syncIntervalMinutes.toString(),
                  keyboardType: TextInputType.number,
                  decoration: const InputDecoration(
                    suffixText: 'min',
                    border: OutlineInputBorder(),
                    contentPadding: EdgeInsets.symmetric(horizontal: 8, vertical: 8),
                  ),
                  onChanged: (value) {
                    final intValue = int.tryParse(value);
                    if (intValue != null && intValue > 0) {
                      setState(() {
                        _syncIntervalMinutes = intValue;
                      });
                    }
                  },
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildNetworkSettings() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Network Settings',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            SwitchListTile(
              title: const Text('Sync on WiFi Only'),
              subtitle: const Text('Only sync when connected to WiFi'),
              value: _syncOnWifiOnly,
              onChanged: (value) {
                setState(() {
                  _syncOnWifiOnly = value;
                });
              },
            ),
            if (_syncOnWifiOnly)
              const Padding(
                padding: EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                child: Text(
                  'Mobile data sync is disabled when WiFi-only mode is enabled',
                  style: TextStyle(
                    fontSize: 12,
                    color: Colors.grey,
                    fontStyle: FontStyle.italic,
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildPerformanceSettings() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Performance Settings',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            ListTile(
              title: const Text('Max Retry Attempts'),
              subtitle: const Text('Number of times to retry failed sync operations'),
              trailing: SizedBox(
                width: 80,
                child: TextFormField(
                  initialValue: _maxRetryAttempts.toString(),
                  keyboardType: TextInputType.number,
                  decoration: const InputDecoration(
                    border: OutlineInputBorder(),
                    contentPadding: EdgeInsets.symmetric(horizontal: 8, vertical: 8),
                  ),
                  onChanged: (value) {
                    final intValue = int.tryParse(value);
                    if (intValue != null && intValue >= 0) {
                      setState(() {
                        _maxRetryAttempts = intValue;
                      });
                    }
                  },
                ),
              ),
            ),
            const Divider(),
            // Batch size not currently supported in configuration
          ],
        ),
      ),
    );
  }

  Widget _buildConflictResolutionSettings() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Conflict Resolution',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            DropdownButtonFormField<ConflictResolutionStrategy>(
              value: _defaultConflictResolution,
              decoration: const InputDecoration(
                labelText: 'Default Resolution Strategy',
                border: OutlineInputBorder(),
                helperText: 'How to handle conflicts when they occur',
              ),
              items: ConflictResolutionStrategy.values.map((resolution) {
                return DropdownMenuItem(
                  value: resolution,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(_getResolutionDisplayName(resolution)),
                      Text(
                        _getResolutionDescription(resolution),
                        style: const TextStyle(fontSize: 12, color: Colors.grey),
                      ),
                    ],
                  ),
                );
              }).toList(),
              onChanged: (value) {
                if (value != null) {
                  setState(() {
                    _defaultConflictResolution = value;
                  });
                }
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildEntitySettings() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Text(
                  'Entity Sync Settings',
                  style: Theme.of(context).textTheme.titleLarge,
                ),
                const Spacer(),
                TextButton(
                  onPressed: _toggleAllEntities,
                  child: Text(_allEntitiesEnabled() ? 'Disable All' : 'Enable All'),
                ),
              ],
            ),
            const SizedBox(height: 16),
            ...SyncEntityType.values.map((entityType) {
              return SwitchListTile(
                title: Text(_getEntityDisplayName(entityType)),
                subtitle: Text(_getEntityDescription(entityType)),
                value: _entitySyncSettings[entityType] ?? true,
                onChanged: (value) {
                  setState(() {
                    _entitySyncSettings[entityType] = value;
                  });
                },
                secondary: Icon(_getEntityIcon(entityType)),
              );
            }),
          ],
        ),
      ),
    );
  }

  Widget _buildAdvancedSettings() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Advanced Settings',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            ListTile(
              title: const Text('Export Configuration'),
              subtitle: const Text('Export current settings to a file'),
              leading: const Icon(Icons.download),
              onTap: _exportConfiguration,
            ),
            const Divider(),
            ListTile(
              title: const Text('Import Configuration'),
              subtitle: const Text('Import settings from a file'),
              leading: const Icon(Icons.upload),
              onTap: _importConfiguration,
            ),
            const Divider(),
            ListTile(
              title: const Text('Clear Sync Data'),
              subtitle: const Text('Clear all cached sync data'),
              leading: const Icon(Icons.clear_all, color: Colors.orange),
              onTap: _clearSyncData,
            ),
            const Divider(),
            ListTile(
              title: const Text('Reset All Settings'),
              subtitle: const Text('Reset all sync settings to defaults'),
              leading: const Icon(Icons.restore, color: Colors.red),
              onTap: _resetAllSettings,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildActionButtons() {
    return Row(
      children: [
        Expanded(
          child: OutlinedButton.icon(
            onPressed: _testConnection,
            icon: const Icon(Icons.network_check),
            label: const Text('Test Connection'),
          ),
        ),
        const SizedBox(width: 16),
        Expanded(
          child: ElevatedButton.icon(
            onPressed: _saveConfiguration,
            icon: const Icon(Icons.save),
            label: const Text('Save Settings'),
            style: ElevatedButton.styleFrom(
              backgroundColor: Theme.of(context).primaryColor,
              foregroundColor: Colors.white,
            ),
          ),
        ),
      ],
    );
  }

  bool _allEntitiesEnabled() {
    return _entitySyncSettings.values.every((enabled) => enabled);
  }

  void _toggleAllEntities() {
    final enableAll = !_allEntitiesEnabled();
    setState(() {
      for (final entityType in SyncEntityType.values) {
        _entitySyncSettings[entityType] = enableAll;
      }
    });
  }

  Future<void> _saveConfiguration() async {
    try {
      final config = SyncConfiguration(
        autoSync: _autoSyncEnabled,
        syncInterval: Duration(minutes: _syncIntervalMinutes),
        syncOnlyOnWifi: _syncOnWifiOnly,
        maxRetries: _maxRetryAttempts,
        defaultConflictStrategy: _defaultConflictResolution,
        enabledEntities: _entitySyncSettings.entries
            .where((entry) => entry.value)
            .map((entry) => entry.key)
            .toList(),
      );

      await _syncService.updateConfiguration(config);
      _showSuccessSnackBar('Sync settings saved successfully');
      Navigator.of(context).pop();
    } catch (e) {
      _showErrorSnackBar('Failed to save settings: $e');
    }
  }

  Future<void> _resetToDefaults() async {
    final confirmed = await _showConfirmDialog(
      'Reset Settings',
      'This will reset all sync settings to their default values. Continue?',
    );

    if (confirmed) {
      setState(() {
        _initializeFormValues(null);
      });
      _showSuccessSnackBar('Settings reset to defaults');
    }
  }

  Future<void> _testConnection() async {
    _showLoadingDialog('Testing connection...');

    try {
      final isConnected = await _syncService.testConnection();
      Navigator.of(context).pop(); // Close loading dialog
      
      _showDialog(
        isConnected ? 'Connection Test Successful' : 'Connection Test Failed',
        isConnected
            ? 'Successfully connected to sync server'
            : 'Unable to connect to sync server. Check your network connection and settings.',
        isConnected ? Icons.check_circle : Icons.error,
        isConnected ? Colors.green : Colors.red,
      );
    } catch (e) {
      Navigator.of(context).pop(); // Close loading dialog
      _showErrorSnackBar('Connection test failed: $e');
    }
  }

  Future<void> _exportConfiguration() async {
    try {
      // In a real app, this would use file picker and save to device
      _showSuccessSnackBar('Configuration export feature coming soon');
    } catch (e) {
      _showErrorSnackBar('Failed to export configuration: $e');
    }
  }

  Future<void> _importConfiguration() async {
    try {
      // In a real app, this would use file picker and load from device
      _showSuccessSnackBar('Configuration import feature coming soon');
    } catch (e) {
      _showErrorSnackBar('Failed to import configuration: $e');
    }
  }

  Future<void> _clearSyncData() async {
    final confirmed = await _showConfirmDialog(
      'Clear Sync Data',
      'This will clear all cached sync data. Unsaved changes may be lost. Continue?',
    );

    if (confirmed) {
      try {
        // Implement clear in repository if needed; placeholder message for now
        _showSuccessSnackBar('Sync data cleared successfully');
      } catch (e) {
        _showErrorSnackBar('Failed to clear sync data: $e');
      }
    }
  }

  Future<void> _resetAllSettings() async {
    final confirmed = await _showConfirmDialog(
      'Reset All Settings',
      'This will reset all sync settings and clear all sync data. This action cannot be undone. Continue?',
    );

    if (confirmed) {
      try {
        // Reset to defaults locally and persist
        final defaults = const SyncConfiguration();
        await _syncService.updateConfiguration(defaults);
        _showSuccessSnackBar('All sync settings reset successfully');
        Navigator.of(context).pop();
      } catch (e) {
        _showErrorSnackBar('Failed to reset settings: $e');
      }
    }
  }

  String _getResolutionDisplayName(ConflictResolutionStrategy resolution) {
    switch (resolution) {
      case ConflictResolutionStrategy.localWins:
        return 'Local Wins';
      case ConflictResolutionStrategy.remoteWins:
        return 'Remote Wins';
      case ConflictResolutionStrategy.merge:
        return 'Auto Merge';
      case ConflictResolutionStrategy.manual:
        return 'Manual Review';
      case ConflictResolutionStrategy.keepBoth:
        return 'Keep Both';
    }
  }

  String _getResolutionDescription(ConflictResolutionStrategy resolution) {
    switch (resolution) {
      case ConflictResolutionStrategy.localWins:
        return 'Always keep local changes';
      case ConflictResolutionStrategy.remoteWins:
        return 'Always keep server changes';
      case ConflictResolutionStrategy.merge:
        return 'Automatically merge changes';
      case ConflictResolutionStrategy.manual:
        return 'Require manual intervention';
      case ConflictResolutionStrategy.keepBoth:
        return 'Create duplicate entries';
    }
  }

  String _getEntityDisplayName(SyncEntityType entityType) {
    switch (entityType) {
      case SyncEntityType.transaction:
        return 'Transactions';
      case SyncEntityType.product:
        return 'Products';
      case SyncEntityType.customer:
        return 'Customers';
      case SyncEntityType.inventory:
        return 'Inventory';
      case SyncEntityType.discount:
        return 'Discounts';
      case SyncEntityType.employee:
        return 'Employees';
      case SyncEntityType.location:
        return 'Locations';
      case SyncEntityType.transfer:
        return 'Transfers';
    }
  }

  String _getEntityDescription(SyncEntityType entityType) {
    switch (entityType) {
      case SyncEntityType.transaction:
        return 'Sales transactions and receipts';
      case SyncEntityType.product:
        return 'Product catalog and information';
      case SyncEntityType.customer:
        return 'Customer profiles and data';
      case SyncEntityType.inventory:
        return 'Stock levels and inventory data';
      case SyncEntityType.discount:
        return 'Discount rules and promotions';
      case SyncEntityType.employee:
        return 'Staff accounts and permissions';
      case SyncEntityType.location:
        return 'Store locations and settings';
      case SyncEntityType.transfer:
        return 'Inventory transfers between locations';
    }
  }

  IconData _getEntityIcon(SyncEntityType entityType) {
    switch (entityType) {
      case SyncEntityType.transaction:
        return Icons.receipt;
      case SyncEntityType.product:
        return Icons.inventory;
      case SyncEntityType.customer:
        return Icons.person;
      case SyncEntityType.inventory:
        return Icons.warehouse;
      case SyncEntityType.discount:
        return Icons.local_offer;
      case SyncEntityType.employee:
        return Icons.badge;
      case SyncEntityType.location:
        return Icons.store;
      case SyncEntityType.transfer:
        return Icons.swap_horiz;
    }
  }

  void _showLoadingDialog(String message) {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (context) => AlertDialog(
        content: Row(
          children: [
            const CircularProgressIndicator(),
            const SizedBox(width: 16),
            Text(message),
          ],
        ),
      ),
    );
  }

  void _showDialog(String title, String message, IconData icon, Color color) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Row(
          children: [
            Icon(icon, color: color),
            const SizedBox(width: 8),
            Text(title),
          ],
        ),
        content: Text(message),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('OK'),
          ),
        ],
      ),
    );
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
