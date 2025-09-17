import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:flutter/services.dart';

import '../../domain/entities/sync_entity.dart';
import '../../data/services/sync_service.dart';
import '../../data/repositories/connectivity_repository.dart';
import 'sync_queue_screen.dart';
import 'sync_conflicts_screen.dart';
import 'sync_analytics_screen.dart';
import 'sync_settings_screen.dart';

class SyncManagementScreen extends StatefulWidget {
  const SyncManagementScreen({super.key});

  @override
  State<SyncManagementScreen> createState() => _SyncManagementScreenState();
}

class _SyncManagementScreenState extends State<SyncManagementScreen>
    with TickerProviderStateMixin {
  late final SyncService _syncService;
  late final ConnectivityRepository _connectivityRepository;
  late final TabController _tabController;

  bool _isLoading = false;
  Map<String, dynamic>? _syncStatistics;
  bool _isConnected = false;

  @override
  void initState() {
    super.initState();
    _syncService = Provider.of<SyncService>(context, listen: false);
    _connectivityRepository = Provider.of<ConnectivityRepository>(context, listen: false);
    _tabController = TabController(length: 4, vsync: this);
    
    _initializeData();
    _listenToConnectivity();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  void _initializeData() async {
    setState(() => _isLoading = true);
    
    try {
      final statistics = await _syncService.getSyncStatistics();
      
      setState(() {
        _syncStatistics = statistics;
      });
    } catch (e) {
      _showErrorDialog('Failed to load sync data', e.toString());
    } finally {
      setState(() => _isLoading = false);
    }
  }

  void _listenToConnectivity() {
    _connectivityRepository.connectionStatusStream.listen((isConnected) {
      setState(() {
        _isConnected = isConnected;
      });
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Sync Management'),
        backgroundColor: Theme.of(context).primaryColor,
        foregroundColor: Colors.white,
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _refreshData,
          ),
          IconButton(
            icon: const Icon(Icons.settings),
            onPressed: _openSettings,
          ),
        ],
        bottom: TabBar(
          controller: _tabController,
          indicatorColor: Colors.white,
          labelColor: Colors.white,
          unselectedLabelColor: Colors.white70,
          tabs: const [
            Tab(icon: Icon(Icons.dashboard), text: 'Overview'),
            Tab(icon: Icon(Icons.queue), text: 'Queue'),
            Tab(icon: Icon(Icons.warning), text: 'Conflicts'),
            Tab(icon: Icon(Icons.analytics), text: 'Analytics'),
          ],
        ),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : TabBarView(
              controller: _tabController,
              children: [
                _buildOverviewTab(),
                const SyncQueueScreen(),
                const SyncConflictsScreen(),
                const SyncAnalyticsScreen(),
              ],
            ),
      floatingActionButton: _buildFloatingActionButton(),
    );
  }

  Widget _buildOverviewTab() {
    return RefreshIndicator(
      onRefresh: _refreshData,
      child: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildConnectivityCard(),
            const SizedBox(height: 16),
            _buildSyncStatusCard(),
            const SizedBox(height: 16),
            _buildQuickActionsCard(),
            const SizedBox(height: 16),
            _buildEntityStatsCard(),
            const SizedBox(height: 16),
            _buildRecentActivityCard(),
          ],
        ),
      ),
    );
  }

  Widget _buildConnectivityCard() {
  final isConnected = _isConnected;
    
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(
                  isConnected ? Icons.wifi : Icons.wifi_off,
                  color: isConnected ? Colors.green : Colors.red,
                ),
                const SizedBox(width: 8),
                Text(
                  'Network Status',
                  style: Theme.of(context).textTheme.titleMedium,
                ),
                const Spacer(),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: isConnected ? Colors.green : Colors.red,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Text(
                    isConnected ? 'Online' : 'Offline',
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 12,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Text(
              isConnected
                  ? 'Connected - Auto-sync enabled'
                  : 'Offline - Changes queued for sync',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: isConnected ? Colors.green : Colors.orange,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSyncStatusCard() {
    if (_syncStatistics == null) return const SizedBox.shrink();

    final queuedCount = _syncStatistics!['queuedCount'] ?? 0;
    final conflictsCount = _syncStatistics!['conflictsCount'] ?? 0;
    final lastSyncTime = _syncStatistics!['lastSyncTime'] as DateTime?;

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                const Icon(Icons.sync, color: Colors.blue),
                const SizedBox(width: 8),
                Text(
                  'Sync Status',
                  style: Theme.of(context).textTheme.titleMedium,
                ),
              ],
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: _buildStatusItem(
                    'Queued',
                    queuedCount.toString(),
                    queuedCount > 0 ? Colors.orange : Colors.green,
                    Icons.queue,
                  ),
                ),
                Expanded(
                  child: _buildStatusItem(
                    'Conflicts',
                    conflictsCount.toString(),
                    conflictsCount > 0 ? Colors.red : Colors.green,
                    Icons.warning,
                  ),
                ),
                Expanded(
                  child: _buildStatusItem(
                    'Last Sync',
                    lastSyncTime != null
                        ? _formatTimeAgo(lastSyncTime)
                        : 'Never',
                    lastSyncTime != null ? Colors.green : Colors.grey,
                    Icons.schedule,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildStatusItem(String label, String value, Color color, IconData icon) {
    return Column(
      children: [
        Icon(icon, color: color, size: 24),
        const SizedBox(height: 4),
        Text(
          value,
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
            color: color,
          ),
        ),
        Text(
          label,
          style: Theme.of(context).textTheme.bodySmall,
          textAlign: TextAlign.center,
        ),
      ],
    );
  }

  Widget _buildQuickActionsCard() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Quick Actions',
              style: Theme.of(context).textTheme.titleMedium,
            ),
            const SizedBox(height: 16),
            GridView.count(
              shrinkWrap: true,
              physics: const NeverScrollableScrollPhysics(),
              crossAxisCount: 2,
              childAspectRatio: 3,
              crossAxisSpacing: 8,
              mainAxisSpacing: 8,
              children: [
                _buildActionButton(
                  'Sync Now',
                  Icons.sync,
                  _performManualSync,
                  Colors.blue,
                ),
                _buildActionButton(
                  'Force Sync',
                  Icons.sync_problem,
                  _performForceSync,
                  Colors.orange,
                ),
                _buildActionButton(
                  'Clear Queue',
                  Icons.clear_all,
                  _clearSyncQueue,
                  Colors.red,
                ),
                _buildActionButton(
                  'Export Data',
                  Icons.download,
                  _exportData,
                  Colors.green,
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildActionButton(
    String label,
    IconData icon,
    VoidCallback onPressed,
    Color color,
  ) {
    return ElevatedButton.icon(
      onPressed: onPressed,
      icon: Icon(icon, size: 18),
      label: Text(
        label,
        style: const TextStyle(fontSize: 12),
      ),
      style: ElevatedButton.styleFrom(
        backgroundColor: color,
        foregroundColor: Colors.white,
        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      ),
    );
  }

  Widget _buildEntityStatsCard() {
    if (_syncStatistics == null) return const SizedBox.shrink();

    final entityStats = _syncStatistics!['entityStats'] as Map<String, dynamic>? ?? {};

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Entity Statistics',
              style: Theme.of(context).textTheme.titleMedium,
            ),
            const SizedBox(height: 16),
            ...SyncEntityType.values.map((entityType) {
              final stats = entityStats[entityType.name] as Map<String, dynamic>? ?? {};
              final totalCount = stats['total'] ?? 0;
              final unsyncedCount = stats['unsynced'] ?? 0;
              
              return Padding(
                padding: const EdgeInsets.symmetric(vertical: 4),
                child: Row(
                  children: [
                    Icon(
                      _getEntityIcon(entityType),
                      size: 20,
                      color: Theme.of(context).primaryColor,
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        _getEntityDisplayName(entityType),
                        style: Theme.of(context).textTheme.bodyMedium,
                      ),
                    ),
                    Text(
                      '$totalCount total',
                      style: Theme.of(context).textTheme.bodySmall,
                    ),
                    const SizedBox(width: 8),
                    if (unsyncedCount > 0)
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                        decoration: BoxDecoration(
                          color: Colors.orange,
                          borderRadius: BorderRadius.circular(10),
                        ),
                        child: Text(
                          '$unsyncedCount',
                          style: const TextStyle(
                            color: Colors.white,
                            fontSize: 10,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                  ],
                ),
              );
            }),
          ],
        ),
      ),
    );
  }

  Widget _buildRecentActivityCard() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Text(
                  'Recent Activity',
                  style: Theme.of(context).textTheme.titleMedium,
                ),
                const Spacer(),
                TextButton(
                  onPressed: () => _tabController.animateTo(3),
                  child: const Text('View All'),
                ),
              ],
            ),
            const SizedBox(height: 16),
            // This would be populated with actual recent sync activities
            const ListTile(
              leading: Icon(Icons.sync, color: Colors.green),
              title: Text('Products synchronized'),
              subtitle: Text('2 minutes ago'),
              trailing: Text('15 items'),
            ),
            const ListTile(
              leading: Icon(Icons.upload, color: Colors.blue),
              title: Text('Transactions uploaded'),
              subtitle: Text('5 minutes ago'),
              trailing: Text('3 items'),
            ),
            const ListTile(
              leading: Icon(Icons.warning, color: Colors.orange),
              title: Text('Conflict detected'),
              subtitle: Text('10 minutes ago'),
              trailing: Icon(Icons.arrow_forward_ios, size: 16),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildFloatingActionButton() {
    return FloatingActionButton(
      onPressed: _performManualSync,
      backgroundColor: Theme.of(context).primaryColor,
      child: const Icon(Icons.sync, color: Colors.white),
    );
  }

  Future<void> _refreshData() async {
    _initializeData();
  }

  Future<void> _performManualSync() async {
    if (!_isConnected) {
      _showSnackBar('No internet connection available', isError: true);
      return;
    }

    _showLoadingDialog('Synchronizing...');

    try {
      await _syncService.syncAll();
      Navigator.of(context).pop(); // Close loading dialog
      _showSnackBar('Synchronization completed successfully');
      await _refreshData();
    } catch (e) {
      Navigator.of(context).pop(); // Close loading dialog
      _showErrorDialog('Sync Failed', e.toString());
    }
  }

  Future<void> _performForceSync() async {
    final confirmed = await _showConfirmDialog(
      'Force Sync',
      'This will override local changes with server data. Continue?',
    );

    if (!confirmed) return;

    if (!_isConnected) {
      _showSnackBar('No internet connection available', isError: true);
      return;
    }

    _showLoadingDialog('Force synchronizing...');

    try {
      await _syncService.forceSync();
      Navigator.of(context).pop(); // Close loading dialog
      _showSnackBar('Force synchronization completed');
      await _refreshData();
    } catch (e) {
      Navigator.of(context).pop(); // Close loading dialog
      _showErrorDialog('Force Sync Failed', e.toString());
    }
  }

  Future<void> _clearSyncQueue() async {
    final confirmed = await _showConfirmDialog(
      'Clear Sync Queue',
      'This will remove all pending sync operations. Continue?',
    );

    if (!confirmed) return;

    try {
      await _syncService.clearQueue();
      _showSnackBar('Sync queue cleared');
      await _refreshData();
    } catch (e) {
      _showErrorDialog('Failed to Clear Queue', e.toString());
    }
  }

  Future<void> _exportData() async {
    _showLoadingDialog('Exporting data...');

    try {
  final backup = await _syncService.createBackup();
      Navigator.of(context).pop(); // Close loading dialog
      
      // Copy backup data to clipboard
      await Clipboard.setData(ClipboardData(text: backup.toString()));
      _showSnackBar('Data exported to clipboard');
    } catch (e) {
      Navigator.of(context).pop(); // Close loading dialog
      _showErrorDialog('Export Failed', e.toString());
    }
  }

  void _openSettings() {
    Navigator.of(context).push(
      MaterialPageRoute(
        builder: (context) => const SyncSettingsScreen(),
      ),
    ).then((_) => _refreshData());
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

  void _showSnackBar(String message, {bool isError = false}) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: isError ? Colors.red : Colors.green,
        behavior: SnackBarBehavior.floating,
      ),
    );
  }

  void _showErrorDialog(String title, String message) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(title),
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


  String _formatTimeAgo(DateTime dateTime) {
    final difference = DateTime.now().difference(dateTime);
    
    if (difference.inDays > 0) {
      return '${difference.inDays}d ago';
    } else if (difference.inHours > 0) {
      return '${difference.inHours}h ago';
    } else if (difference.inMinutes > 0) {
      return '${difference.inMinutes}m ago';
    } else {
      return 'Just now';
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
}
