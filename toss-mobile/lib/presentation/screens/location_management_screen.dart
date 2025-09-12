import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../domain/entities/location_entity.dart';
import '../../data/services/location_service.dart';

class LocationManagementScreen extends StatefulWidget {
  const LocationManagementScreen({Key? key}) : super(key: key);

  @override
  State<LocationManagementScreen> createState() => _LocationManagementScreenState();
}

class _LocationManagementScreenState extends State<LocationManagementScreen>
    with TickerProviderStateMixin {
  final LocationService _locationService = LocationService();
  late TabController _tabController;
  
  List<LocationEntity> _locations = [];
  List<InventoryTransferEntity> _transfers = [];
  LocationEntity? _selectedLocation;
  bool _isLoading = true;
  
  final TextEditingController _searchController = TextEditingController();
  String _searchQuery = '';

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 4, vsync: this);
    _initializeData();
  }

  Future<void> _initializeData() async {
    try {
      await _locationService.initialize();
      await _loadLocations();
      await _loadTransfers();
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error loading data: $e')),
        );
      }
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  Future<void> _loadLocations() async {
    try {
      final locations = _locationService.allLocations;
      setState(() {
        _locations = locations;
      });
    } catch (e) {
      debugPrint('Error loading locations: $e');
    }
  }

  Future<void> _loadTransfers() async {
    try {
      final transfers = await _locationService.getPendingTransfers();
      setState(() {
        _transfers = transfers;
      });
    } catch (e) {
      debugPrint('Error loading transfers: $e');
    }
  }

  List<LocationEntity> get _filteredLocations {
    if (_searchQuery.isEmpty) return _locations;
    return _locations.where((location) =>
        location.name.toLowerCase().contains(_searchQuery.toLowerCase()) ||
        location.code.toLowerCase().contains(_searchQuery.toLowerCase()) ||
        location.city.toLowerCase().contains(_searchQuery.toLowerCase())).toList();
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return const Scaffold(
        body: Center(child: CircularProgressIndicator()),
      );
    }

    return Scaffold(
      appBar: AppBar(
        title: const Text('Location Management'),
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(icon: Icon(Icons.store), text: 'Locations'),
            Tab(icon: Icon(Icons.swap_horiz), text: 'Transfers'),
            Tab(icon: Icon(Icons.analytics), text: 'Analytics'),
            Tab(icon: Icon(Icons.settings), text: 'Settings'),
          ],
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.add),
            onPressed: () => _showCreateLocationDialog(),
          ),
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _refreshData,
          ),
        ],
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          _buildLocationsTab(),
          _buildTransfersTab(),
          _buildAnalyticsTab(),
          _buildSettingsTab(),
        ],
      ),
    );
  }

  Widget _buildLocationsTab() {
    return Column(
      children: [
        // Search bar
        Padding(
          padding: const EdgeInsets.all(16.0),
          child: TextField(
            controller: _searchController,
            decoration: InputDecoration(
              hintText: 'Search locations...',
              prefixIcon: const Icon(Icons.search),
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(8),
              ),
              suffixIcon: _searchQuery.isNotEmpty
                  ? IconButton(
                      icon: const Icon(Icons.clear),
                      onPressed: () {
                        _searchController.clear();
                        setState(() => _searchQuery = '');
                      },
                    )
                  : null,
            ),
            onChanged: (value) {
              setState(() => _searchQuery = value);
            },
          ),
        ),
        
        // Active location selector
        if (_locationService.activeLocation != null)
          Container(
            margin: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 8.0),
            padding: const EdgeInsets.all(12.0),
            decoration: BoxDecoration(
              color: Colors.green.withOpacity(0.1),
              borderRadius: BorderRadius.circular(8),
              border: Border.all(color: Colors.green),
            ),
            child: Row(
              children: [
                const Icon(Icons.check_circle, color: Colors.green),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text(
                        'Active Location',
                        style: TextStyle(
                          fontWeight: FontWeight.bold,
                          color: Colors.green,
                        ),
                      ),
                      Text(
                        '${_locationService.activeLocation!.name} (${_locationService.activeLocation!.code})',
                        style: const TextStyle(fontSize: 16),
                      ),
                    ],
                  ),
                ),
                TextButton(
                  onPressed: () => _showLocationSelector(),
                  child: const Text('Change'),
                ),
              ],
            ),
          ),

        // Locations list
        Expanded(
          child: _filteredLocations.isEmpty
              ? const Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Icon(Icons.store_outlined, size: 64, color: Colors.grey),
                      SizedBox(height: 16),
                      Text(
                        'No locations found',
                        style: TextStyle(fontSize: 18, color: Colors.grey),
                      ),
                    ],
                  ),
                )
              : ListView.builder(
                  itemCount: _filteredLocations.length,
                  itemBuilder: (context, index) {
                    final location = _filteredLocations[index];
                    return _buildLocationCard(location);
                  },
                ),
        ),
      ],
    );
  }

  Widget _buildLocationCard(LocationEntity location) {
    final isActive = _locationService.activeLocationId == location.id;
    
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 4.0),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: _getLocationStatusColor(location.status),
          child: Icon(
            _getLocationTypeIcon(location.type),
            color: Colors.white,
          ),
        ),
        title: Row(
          children: [
            Expanded(
              child: Text(
                location.name,
                style: const TextStyle(fontWeight: FontWeight.bold),
              ),
            ),
            if (isActive)
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                decoration: BoxDecoration(
                  color: Colors.green,
                  borderRadius: BorderRadius.circular(12),
                ),
                child: const Text(
                  'ACTIVE',
                  style: TextStyle(
                    color: Colors.white,
                    fontSize: 10,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
          ],
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Code: ${location.code} • ${location.type.name.toUpperCase()}'),
            Text(location.displayAddress),
            Row(
              children: [
                Icon(
                  location.isOpen ? Icons.access_time : Icons.access_time_filled,
                  size: 16,
                  color: location.isOpen ? Colors.green : Colors.red,
                ),
                const SizedBox(width: 4),
                Text(
                  location.isOpen ? 'Open' : 'Closed',
                  style: TextStyle(
                    color: location.isOpen ? Colors.green : Colors.red,
                    fontWeight: FontWeight.w500,
                  ),
                ),
                const SizedBox(width: 16),
                Icon(
                  Icons.circle,
                  size: 12,
                  color: _getLocationStatusColor(location.status),
                ),
                const SizedBox(width: 4),
                Text(location.status.name.toUpperCase()),
              ],
            ),
          ],
        ),
        trailing: PopupMenuButton<String>(
          onSelected: (value) => _handleLocationAction(value, location),
          itemBuilder: (context) => [
            if (!isActive)
              const PopupMenuItem(
                value: 'activate',
                child: ListTile(
                  leading: Icon(Icons.check_circle),
                  title: Text('Set as Active'),
                ),
              ),
            const PopupMenuItem(
              value: 'edit',
              child: ListTile(
                leading: Icon(Icons.edit),
                title: Text('Edit'),
              ),
            ),
            const PopupMenuItem(
              value: 'transfer',
              child: ListTile(
                leading: Icon(Icons.swap_horiz),
                title: Text('Create Transfer'),
              ),
            ),
            const PopupMenuItem(
              value: 'analytics',
              child: ListTile(
                leading: Icon(Icons.analytics),
                title: Text('View Analytics'),
              ),
            ),
            if (!isActive)
              const PopupMenuItem(
                value: 'delete',
                child: ListTile(
                  leading: Icon(Icons.delete, color: Colors.red),
                  title: Text('Delete', style: TextStyle(color: Colors.red)),
                ),
              ),
          ],
        ),
        onTap: () => _showLocationDetails(location),
      ),
    );
  }

  Widget _buildTransfersTab() {
    return Column(
      children: [
        // Transfer status filter
        Container(
          height: 50,
          margin: const EdgeInsets.all(16.0),
          child: ListView(
            scrollDirection: Axis.horizontal,
            children: TransferStatus.values.map((status) {
              return Container(
                margin: const EdgeInsets.only(right: 8),
                child: FilterChip(
                  label: Text(status.name.toUpperCase()),
                  selected: false, // Add state management for filter
                  onSelected: (selected) {
                    // Implement filter logic
                  },
                ),
              );
            }).toList(),
          ),
        ),

        // Transfers list
        Expanded(
          child: _transfers.isEmpty
              ? const Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Icon(Icons.swap_horiz, size: 64, color: Colors.grey),
                      SizedBox(height: 16),
                      Text(
                        'No transfers found',
                        style: TextStyle(fontSize: 18, color: Colors.grey),
                      ),
                    ],
                  ),
                )
              : ListView.builder(
                  itemCount: _transfers.length,
                  itemBuilder: (context, index) {
                    final transfer = _transfers[index];
                    return _buildTransferCard(transfer);
                  },
                ),
        ),
      ],
    );
  }

  Widget _buildTransferCard(InventoryTransferEntity transfer) {
    final fromLocation = _locations.where((l) => l.id == transfer.fromLocationId).firstOrNull;
    final toLocation = _locations.where((l) => l.id == transfer.toLocationId).firstOrNull;

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 4.0),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: _getTransferStatusColor(transfer.status),
          child: Icon(
            _getTransferTypeIcon(transfer.type),
            color: Colors.white,
          ),
        ),
        title: Text(
          transfer.transferNumber,
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('${fromLocation?.name ?? 'Unknown'} → ${toLocation?.name ?? 'Unknown'}'),
            Text('${transfer.totalItems} items • GHS ${transfer.totalValue.toStringAsFixed(2)}'),
            Text('${transfer.type.name.toUpperCase()} • ${transfer.status.name.toUpperCase()}'),
          ],
        ),
        trailing: PopupMenuButton<String>(
          onSelected: (value) => _handleTransferAction(value, transfer),
          itemBuilder: (context) => [
            if (transfer.status == TransferStatus.pending)
              const PopupMenuItem(
                value: 'approve',
                child: ListTile(
                  leading: Icon(Icons.check),
                  title: Text('Approve'),
                ),
              ),
            if (transfer.status == TransferStatus.approved)
              const PopupMenuItem(
                value: 'ship',
                child: ListTile(
                  leading: Icon(Icons.local_shipping),
                  title: Text('Mark as Shipped'),
                ),
              ),
            if (transfer.status == TransferStatus.shipped)
              const PopupMenuItem(
                value: 'receive',
                child: ListTile(
                  leading: Icon(Icons.inventory),
                  title: Text('Receive'),
                ),
              ),
            const PopupMenuItem(
              value: 'details',
              child: ListTile(
                leading: Icon(Icons.info),
                title: Text('View Details'),
              ),
            ),
            if (transfer.status == TransferStatus.pending)
              const PopupMenuItem(
                value: 'cancel',
                child: ListTile(
                  leading: Icon(Icons.cancel, color: Colors.red),
                  title: Text('Cancel', style: TextStyle(color: Colors.red)),
                ),
              ),
          ],
        ),
        onTap: () => _showTransferDetails(transfer),
      ),
    );
  }

  Widget _buildAnalyticsTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Location Performance Overview',
            style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 16),
          
          // Summary cards
          Row(
            children: [
              Expanded(
                child: _buildAnalyticsCard(
                  title: 'Total Locations',
                  value: '${_locations.length}',
                  icon: Icons.store,
                  color: Colors.blue,
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: _buildAnalyticsCard(
                  title: 'Active Locations',
                  value: '${_locations.where((l) => l.status == LocationStatus.active).length}',
                  icon: Icons.check_circle,
                  color: Colors.green,
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          
          Row(
            children: [
              Expanded(
                child: _buildAnalyticsCard(
                  title: 'Pending Transfers',
                  value: '${_transfers.length}',
                  icon: Icons.swap_horiz,
                  color: Colors.orange,
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: _buildAnalyticsCard(
                  title: 'Total Sales Today',
                  value: 'GHS 15,750',
                  icon: Icons.trending_up,
                  color: Colors.purple,
                ),
              ),
            ],
          ),
          const SizedBox(height: 24),

          // Location performance list
          const Text(
            'Location Performance',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 12),
          
          ..._locations.map((location) => _buildLocationPerformanceCard(location)),
          
          const SizedBox(height: 24),
          
          // Generate consolidated report button
          SizedBox(
            width: double.infinity,
            child: ElevatedButton.icon(
              onPressed: _generateConsolidatedReport,
              icon: const Icon(Icons.assessment),
              label: const Text('Generate Consolidated Report'),
              style: ElevatedButton.styleFrom(
                padding: const EdgeInsets.all(16),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildAnalyticsCard({
    required String title,
    required String value,
    required IconData icon,
    required Color color,
  }) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: color.withOpacity(0.3)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(icon, color: color),
              const Spacer(),
              Text(
                value,
                style: TextStyle(
                  fontSize: 24,
                  fontWeight: FontWeight.bold,
                  color: color,
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Text(
            title,
            style: const TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.w500,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildLocationPerformanceCard(LocationEntity location) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Row(
          children: [
            CircleAvatar(
              backgroundColor: _getLocationStatusColor(location.status),
              radius: 20,
              child: Icon(
                _getLocationTypeIcon(location.type),
                color: Colors.white,
                size: 20,
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    location.name,
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                  Text('${location.code} • ${location.city}'),
                ],
              ),
            ),
            Column(
              crossAxisAlignment: CrossAxisAlignment.end,
              children: [
                const Text(
                  'GHS 2,450',
                  style: TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 16,
                  ),
                ),
                Text(
                  '15 transactions',
                  style: TextStyle(
                    color: Colors.grey[600],
                    fontSize: 12,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSettingsTab() {
    return ListView(
      padding: const EdgeInsets.all(16.0),
      children: [
        const Text(
          'Location Settings',
          style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
        ),
        const SizedBox(height: 16),
        
        ListTile(
          leading: const Icon(Icons.sync),
          title: const Text('Auto-sync Inventory'),
          subtitle: const Text('Automatically sync inventory across locations'),
          trailing: Switch(
            value: true,
            onChanged: (value) {
              // Implement setting toggle
            },
          ),
        ),
        
        ListTile(
          leading: const Icon(Icons.notifications),
          title: const Text('Transfer Notifications'),
          subtitle: const Text('Receive notifications for transfer updates'),
          trailing: Switch(
            value: true,
            onChanged: (value) {
              // Implement setting toggle
            },
          ),
        ),
        
        ListTile(
          leading: const Icon(Icons.security),
          title: const Text('Require Approval for Transfers'),
          subtitle: const Text('All transfers need manager approval'),
          trailing: Switch(
            value: false,
            onChanged: (value) {
              // Implement setting toggle
            },
          ),
        ),
        
        const Divider(),
        
        ListTile(
          leading: const Icon(Icons.backup),
          title: const Text('Backup Location Data'),
          subtitle: const Text('Export location and transfer data'),
          onTap: () {
            // Implement data backup
          },
        ),
        
        ListTile(
          leading: const Icon(Icons.restore),
          title: const Text('Restore from Backup'),
          subtitle: const Text('Import location data from backup file'),
          onTap: () {
            // Implement data restore
          },
        ),
        
        const Divider(),
        
        ListTile(
          leading: const Icon(Icons.help),
          title: const Text('Help & Documentation'),
          subtitle: const Text('Learn about multi-location features'),
          onTap: () {
            // Show help documentation
          },
        ),
      ],
    );
  }

  // Helper methods for UI
  Color _getLocationStatusColor(LocationStatus status) {
    switch (status) {
      case LocationStatus.active:
        return Colors.green;
      case LocationStatus.inactive:
        return Colors.orange;
      case LocationStatus.maintenance:
        return Colors.blue;
      case LocationStatus.closed:
        return Colors.red;
      case LocationStatus.suspended:
        return Colors.purple;
    }
  }

  IconData _getLocationTypeIcon(LocationType type) {
    switch (type) {
      case LocationType.store:
        return Icons.store;
      case LocationType.warehouse:
        return Icons.warehouse;
      case LocationType.kiosk:
        return Icons.storefront;
      case LocationType.popup:
        return Icons.location_on;
      case LocationType.outlet:
        return Icons.shopping_basket;
      case LocationType.franchise:
        return Icons.business;
    }
  }

  Color _getTransferStatusColor(TransferStatus status) {
    switch (status) {
      case TransferStatus.pending:
        return Colors.orange;
      case TransferStatus.approved:
        return Colors.blue;
      case TransferStatus.shipped:
        return Colors.purple;
      case TransferStatus.received:
        return Colors.green;
      case TransferStatus.cancelled:
        return Colors.red;
      case TransferStatus.rejected:
        return Colors.red;
    }
  }

  IconData _getTransferTypeIcon(TransferType type) {
    switch (type) {
      case TransferType.stock:
        return Icons.inventory;
      case TransferType.emergency:
        return Icons.priority_high;
      case TransferType.rebalance:
        return Icons.balance;
      case TransferType.returned:
        return Icons.keyboard_return;
      case TransferType.damaged:
        return Icons.warning;
    }
  }

  // Action handlers
  void _handleLocationAction(String action, LocationEntity location) async {
    switch (action) {
      case 'activate':
        try {
          await _locationService.setActiveLocation(location.id);
          setState(() {});
          if (mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text('${location.name} set as active location')),
            );
          }
        } catch (e) {
          if (mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text('Error: $e')),
            );
          }
        }
        break;
      case 'edit':
        _showEditLocationDialog(location);
        break;
      case 'transfer':
        _showCreateTransferDialog(location);
        break;
      case 'analytics':
        _showLocationAnalytics(location);
        break;
      case 'delete':
        _showDeleteLocationDialog(location);
        break;
    }
  }

  void _handleTransferAction(String action, InventoryTransferEntity transfer) async {
    switch (action) {
      case 'approve':
        try {
          await _locationService.approveTransfer(transfer.id, 'current_user_id');
          await _loadTransfers();
          if (mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(content: Text('Transfer approved')),
            );
          }
        } catch (e) {
          if (mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text('Error: $e')),
            );
          }
        }
        break;
      case 'ship':
        try {
          await _locationService.shipTransfer(transfer.id, 'current_user_id');
          await _loadTransfers();
          if (mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(content: Text('Transfer marked as shipped')),
            );
          }
        } catch (e) {
          if (mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text('Error: $e')),
            );
          }
        }
        break;
      case 'receive':
        _showReceiveTransferDialog(transfer);
        break;
      case 'details':
        _showTransferDetails(transfer);
        break;
      case 'cancel':
        _showCancelTransferDialog(transfer);
        break;
    }
  }

  // Dialog methods (simplified implementations)
  void _showCreateLocationDialog() {
    // Implement create location dialog
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Create New Location'),
        content: const Text('Location creation form would go here'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Create'),
          ),
        ],
      ),
    );
  }

  void _showEditLocationDialog(LocationEntity location) {
    // Implement edit location dialog
  }

  void _showDeleteLocationDialog(LocationEntity location) {
    // Implement delete confirmation dialog
  }

  void _showLocationSelector() {
    // Implement location selector dialog
  }

  void _showCreateTransferDialog(LocationEntity location) {
    // Implement create transfer dialog
  }

  void _showReceiveTransferDialog(InventoryTransferEntity transfer) {
    // Implement receive transfer dialog
  }

  void _showCancelTransferDialog(InventoryTransferEntity transfer) {
    // Implement cancel transfer dialog
  }

  void _showLocationDetails(LocationEntity location) {
    // Implement location details view
  }

  void _showTransferDetails(InventoryTransferEntity transfer) {
    // Implement transfer details view
  }

  void _showLocationAnalytics(LocationEntity location) {
    // Implement location analytics view
  }

  void _generateConsolidatedReport() async {
    try {
      final report = await _locationService.getConsolidatedReport();
      // Show report or export options
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Consolidated report generated')),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error generating report: $e')),
        );
      }
    }
  }

  Future<void> _refreshData() async {
    setState(() => _isLoading = true);
    await _initializeData();
  }

  @override
  void dispose() {
    _tabController.dispose();
    _searchController.dispose();
    super.dispose();
  }
}
