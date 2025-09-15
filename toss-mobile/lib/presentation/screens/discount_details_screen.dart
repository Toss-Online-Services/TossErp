import 'package:flutter/material.dart';

import '../../domain/entities/discount_entity.dart';
import '../../data/services/discount_service.dart';
import '../widgets/common/custom_app_bar.dart';
import '../widgets/common/loading_widget.dart';
import 'add_discount_screen.dart';
import 'discount_analytics_screen.dart';

class DiscountDetailsScreen extends StatefulWidget {
  final DiscountEntity discount;

  const DiscountDetailsScreen({super.key, required this.discount});

  @override
  State<DiscountDetailsScreen> createState() => _DiscountDetailsScreenState();
}

class _DiscountDetailsScreenState extends State<DiscountDetailsScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  final DiscountService _discountService = DiscountService();
  
  late DiscountEntity _discount;
  List<DiscountUsage> _usageHistory = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
    _discount = widget.discount;
    _loadData();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Future<void> _loadData() async {
    setState(() {
      _isLoading = true;
    });

    try {
      final usageHistory = await _discountService.getDiscountUsageHistory(_discount.id);
      setState(() {
        _usageHistory = usageHistory;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
      });
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to load data: $e')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(
        title: _discount.name,
        actions: [
          IconButton(
            icon: const Icon(Icons.analytics),
            onPressed: () => _viewAnalytics(),
            tooltip: 'View Analytics',
          ),
          IconButton(
            icon: const Icon(Icons.edit),
            onPressed: () => _editDiscount(),
            tooltip: 'Edit Discount',
          ),
          PopupMenuButton<String>(
            onSelected: _handleMenuAction,
            itemBuilder: (context) => [
              PopupMenuItem(
                value: 'toggle_status',
                child: Row(
                  children: [
                    Icon(_discount.isActive ? Icons.pause : Icons.play_arrow),
                    const SizedBox(width: 8),
                    Text(_discount.isActive ? 'Deactivate' : 'Activate'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'duplicate',
                child: Row(
                  children: [
                    Icon(Icons.copy),
                    SizedBox(width: 8),
                    Text('Duplicate'),
                  ],
                ),
              ),
              const PopupMenuItem(
                value: 'delete',
                child: Row(
                  children: [
                    Icon(Icons.delete, color: Colors.red),
                    SizedBox(width: 8),
                    Text('Delete', style: TextStyle(color: Colors.red)),
                  ],
                ),
              ),
            ],
          ),
        ],
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(text: 'Details'),
            Tab(text: 'Usage'),
            Tab(text: 'Settings'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          _buildDetailsTab(),
          _buildUsageTab(),
          _buildSettingsTab(),
        ],
      ),
    );
  }

  Widget _buildDetailsTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Status Card
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Row(
                children: [
                  CircleAvatar(
                    backgroundColor: _discount.isActive 
                        ? _getDiscountTypeColor(_discount.type)
                        : Colors.grey,
                    child: Icon(
                      _getDiscountTypeIcon(_discount.type),
                      color: Colors.white,
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          _discount.name,
                          style: const TextStyle(
                            fontSize: 20,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        Text(
                          _discount.description,
                          style: TextStyle(
                            color: Colors.grey[600],
                          ),
                        ),
                        const SizedBox(height: 8),
                        Row(
                          children: [
                            Container(
                              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                              decoration: BoxDecoration(
                                color: _discount.isActive 
                                    ? Colors.green.withOpacity(0.1)
                                    : Colors.grey.withOpacity(0.1),
                                borderRadius: BorderRadius.circular(12),
                              ),
                              child: Text(
                                _discount.isActive ? 'ACTIVE' : 'INACTIVE',
                                style: TextStyle(
                                  color: _discount.isActive ? Colors.green : Colors.grey,
                                  fontSize: 12,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ),
                            const SizedBox(width: 8),
                            Container(
                              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                              decoration: BoxDecoration(
                                color: _getDiscountTypeColor(_discount.type).withOpacity(0.1),
                                borderRadius: BorderRadius.circular(12),
                              ),
                              child: Text(
                                _getDiscountTypeDisplayName(_discount.type),
                                style: TextStyle(
                                  color: _getDiscountTypeColor(_discount.type),
                                  fontSize: 12,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.end,
                    children: [
                      Text(
                        _getDiscountValueText(_discount),
                        style: const TextStyle(
                          fontSize: 24,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      Text(
                        '${_discount.currentUses} uses',
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
          ),
          
          const SizedBox(height: 16),
          
          // Basic Information
          _buildInfoSection('Basic Information', [
            _buildInfoRow('Type', _getDiscountTypeDisplayName(_discount.type)),
            _buildInfoRow('Scope', _getDiscountScopeDisplayName(_discount.scope)),
            _buildInfoRow('Application', _getDiscountApplicationDisplayName(_discount.application)),
            if (_discount.couponCode != null)
              _buildInfoRow('Coupon Code', _discount.couponCode!),
            _buildInfoRow('Priority', _discount.priority.toString()),
            _buildInfoRow('Stackable', _discount.isStackable ? 'Yes' : 'No'),
          ]),
          
          const SizedBox(height: 16),
          
          // Dates and Duration
          _buildInfoSection('Duration', [
            _buildInfoRow('Start Date', _formatDate(_discount.startDate)),
            _buildInfoRow('End Date', _formatDate(_discount.endDate)),
            _buildInfoRow('Duration', _getDurationText()),
            _buildInfoRow('Status', _getDateStatus()),
          ]),
          
          const SizedBox(height: 16),
          
          // Amount Restrictions
          if (_discount.minimumAmount != null || _discount.maxDiscountAmount != null)
            _buildInfoSection('Amount Restrictions', [
              if (_discount.minimumAmount != null)
                _buildInfoRow('Minimum Amount', 'GHS ${_discount.minimumAmount!.toStringAsFixed(2)}'),
              if (_discount.maxDiscountAmount != null)
                _buildInfoRow('Max Discount', 'GHS ${_discount.maxDiscountAmount!.toStringAsFixed(2)}'),
            ]),
          
          const SizedBox(height: 16),
          
          // Usage Limits
          _buildInfoSection('Usage Information', [
            _buildInfoRow('Current Uses', _discount.currentUses.toString()),
            if (_discount.usageLimit != null)
              _buildInfoRow('Total Limit', _discount.usageLimit.toString()),
            if (_discount.usageLimitPerCustomer != null)
              _buildInfoRow('Per Customer Limit', _discount.usageLimitPerCustomer.toString()),
            _buildInfoRow('Remaining Uses', _getRemainingUsesText()),
          ]),
          
          const SizedBox(height: 16),
          
          // BOGO Configuration
          if (_discount.bogoConfig != null)
            _buildInfoSection('BOGO Configuration', [
              _buildInfoRow('Buy Quantity', _discount.bogoConfig!.buyQuantity.toString()),
              _buildInfoRow('Get Quantity', _discount.bogoConfig!.getQuantity.toString()),
              _buildInfoRow('Get Discount', '${_discount.bogoConfig!.getDiscountPercentage}%'),
            ]),
          
          const SizedBox(height: 16),
          
          // Time Restrictions
          if (_discount.timeBasedConfig != null)
            _buildTimeRestrictionsSection(),
          
          const SizedBox(height: 16),
          
          // Location Restrictions
          if (_discount.locationConfig != null && _discount.locationConfig!.applicableLocations.isNotEmpty)
            _buildInfoSection('Location Restrictions', [
              _buildInfoRow('Applicable Locations', _discount.locationConfig!.applicableLocations.join(', ')),
            ]),
          
          const SizedBox(height: 16),
          
          // Target Information
          if (_discount.targetProductIds.isNotEmpty ||
              _discount.targetCategoryIds.isNotEmpty ||
              _discount.targetCustomerIds.isNotEmpty)
            _buildTargetInformationSection(),
        ],
      ),
    );
  }

  Widget _buildUsageTab() {
    if (_isLoading) {
      return const LoadingWidget();
    }
    
    return RefreshIndicator(
      onRefresh: _loadData,
      child: _usageHistory.isEmpty
          ? const Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.history, size: 64, color: Colors.grey),
                  SizedBox(height: 16),
                  Text('No usage history found'),
                  SizedBox(height: 8),
                  Text(
                    'This discount has not been used yet',
                    style: TextStyle(color: Colors.grey),
                  ),
                ],
              ),
            )
          : ListView.builder(
              padding: const EdgeInsets.all(16),
              itemCount: _usageHistory.length,
              itemBuilder: (context, index) {
                final usage = _usageHistory[index];
                return _buildUsageCard(usage);
              },
            ),
    );
  }

  Widget _buildUsageCard(DiscountUsage usage) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: Colors.green.withOpacity(0.1),
          child: const Icon(Icons.receipt, color: Colors.green),
        ),
        title: Text('Transaction #${usage.transactionId}'),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (usage.customerId != null)
              Text('Customer: ${usage.customerId}'),
            Text('Amount Saved: GHS ${usage.discountAmount.toStringAsFixed(2)}'),
            Text('Used at: ${_formatDateTime(usage.usedAt)}'),
          ],
        ),
        trailing: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            if (usage.quantity > 1)
              Text(
                '${usage.quantity}x',
                style: const TextStyle(
                  fontWeight: FontWeight.bold,
                ),
              ),
            Text(
              _formatDateTime(usage.usedAt).substring(11, 16),
              style: const TextStyle(
                fontSize: 12,
                color: Colors.grey,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSettingsTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Quick Actions
          Card(
            child: Column(
              children: [
                ListTile(
                  leading: Icon(_discount.isActive ? Icons.pause : Icons.play_arrow),
                  title: Text(_discount.isActive ? 'Deactivate Discount' : 'Activate Discount'),
                  subtitle: Text(_discount.isActive 
                      ? 'Temporarily stop this discount' 
                      : 'Make this discount available'),
                  trailing: Switch(
                    value: _discount.isActive,
                    onChanged: (value) => _toggleDiscountStatus(),
                  ),
                ),
                const Divider(),
                ListTile(
                  leading: const Icon(Icons.edit),
                  title: const Text('Edit Discount'),
                  subtitle: const Text('Modify discount settings'),
                  trailing: const Icon(Icons.chevron_right),
                  onTap: _editDiscount,
                ),
                const Divider(),
                ListTile(
                  leading: const Icon(Icons.copy),
                  title: const Text('Duplicate Discount'),
                  subtitle: const Text('Create a copy of this discount'),
                  trailing: const Icon(Icons.chevron_right),
                  onTap: _duplicateDiscount,
                ),
              ],
            ),
          ),
          
          const SizedBox(height: 16),
          
          // Danger Zone
          Card(
            color: Colors.red.withOpacity(0.05),
            child: Column(
              children: [
                ListTile(
                  leading: const Icon(Icons.warning, color: Colors.orange),
                  title: const Text('Danger Zone'),
                  subtitle: const Text('Irreversible actions'),
                ),
                const Divider(),
                ListTile(
                  leading: const Icon(Icons.delete, color: Colors.red),
                  title: const Text('Delete Discount', style: TextStyle(color: Colors.red)),
                  subtitle: const Text('Permanently remove this discount'),
                  trailing: const Icon(Icons.chevron_right, color: Colors.red),
                  onTap: _deleteDiscount,
                ),
              ],
            ),
          ),
          
          const SizedBox(height: 24),
          
          // Metadata
          _buildInfoSection('Metadata', [
            _buildInfoRow('Created', _formatDateTime(_discount.createdAt)),
            if (_discount.updatedAt != null)
              _buildInfoRow('Last Updated', _formatDateTime(_discount.updatedAt!)),
            _buildInfoRow('Discount ID', _discount.id),
          ]),
        ],
      ),
    );
  }

  Widget _buildInfoSection(String title, List<Widget> children) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              title,
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 12),
            ...children,
          ],
        ),
      ),
    );
  }

  Widget _buildInfoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 120,
            child: Text(
              label,
              style: TextStyle(
                color: Colors.grey[600],
                fontWeight: FontWeight.w500,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(fontWeight: FontWeight.w500),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTimeRestrictionsSection() {
    final config = _discount.timeBasedConfig!;
    final restrictions = <String>[];
    
    if (config.daysOfWeek.isNotEmpty) {
      final dayNames = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
      final days = config.daysOfWeek.map((day) => dayNames[day - 1]).join(', ');
      restrictions.add('Days: $days');
    }
    
    if (config.startTime != null && config.endTime != null) {
      restrictions.add('Hours: ${config.startTime!.format(context)} - ${config.endTime!.format(context)}');
    }
    
    if (config.isSeasonalDiscount && config.seasonStartDate != null && config.seasonEndDate != null) {
      restrictions.add('Season: ${_formatDate(config.seasonStartDate!)} - ${_formatDate(config.seasonEndDate!)}');
    }
    
    return _buildInfoSection('Time Restrictions', [
      ...restrictions.map((restriction) => _buildInfoRow('', restriction)),
    ]);
  }

  Widget _buildTargetInformationSection() {
    final targets = <String>[];
    
    if (_discount.targetProductIds.isNotEmpty) {
      targets.add('${_discount.targetProductIds.length} specific products');
    }
    
    if (_discount.targetCategoryIds.isNotEmpty) {
      targets.add('${_discount.targetCategoryIds.length} categories');
    }
    
    if (_discount.targetCustomerIds.isNotEmpty) {
      targets.add('${_discount.targetCustomerIds.length} specific customers');
    }
    
    return _buildInfoSection('Target Information', [
      _buildInfoRow('Applies to', targets.join(', ')),
    ]);
  }

  void _handleMenuAction(String action) {
    switch (action) {
      case 'toggle_status':
        _toggleDiscountStatus();
        break;
      case 'duplicate':
        _duplicateDiscount();
        break;
      case 'delete':
        _deleteDiscount();
        break;
    }
  }

  void _viewAnalytics() {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => DiscountAnalyticsScreen(discount: _discount),
      ),
    );
  }

  void _editDiscount() {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => AddDiscountScreen(discount: _discount),
      ),
    ).then((result) {
      if (result != null) {
        setState(() {
          _discount = result;
        });
      }
    });
  }

  Future<void> _toggleDiscountStatus() async {
    try {
      await _discountService.toggleDiscountStatus(_discount.id, !_discount.isActive);
      setState(() {
        _discount = _discount.copyWith(isActive: !_discount.isActive);
      });
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              _discount.isActive 
                  ? 'Discount activated' 
                  : 'Discount deactivated',
            ),
            backgroundColor: Colors.green,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Failed to update discount: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  void _duplicateDiscount() {
    final duplicatedDiscount = _discount.copyWith(
      id: DateTime.now().millisecondsSinceEpoch.toString(),
      name: '${_discount.name} (Copy)',
      createdAt: DateTime.now(),
      updatedAt: null,
      currentUses: 0,
    );

    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => AddDiscountScreen(discount: duplicatedDiscount),
      ),
    );
  }

  Future<void> _deleteDiscount() async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Delete Discount'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text('Are you sure you want to delete "${_discount.name}"?'),
            const SizedBox(height: 16),
            const Text(
              'This action cannot be undone. All usage history will be lost.',
              style: TextStyle(color: Colors.red),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: const Text('Delete'),
          ),
        ],
      ),
    );

    if (confirmed == true) {
      try {
        await _discountService.deleteDiscount(_discount.id);
        
        if (mounted) {
          Navigator.pop(context);
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Discount deleted'),
              backgroundColor: Colors.green,
            ),
          );
        }
      } catch (e) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text('Failed to delete discount: $e'),
              backgroundColor: Colors.red,
            ),
          );
        }
      }
    }
  }

  String _formatDate(DateTime date) {
    return '${date.day}/${date.month}/${date.year}';
  }

  String _formatDateTime(DateTime dateTime) {
    return '${dateTime.day}/${dateTime.month}/${dateTime.year} ${dateTime.hour.toString().padLeft(2, '0')}:${dateTime.minute.toString().padLeft(2, '0')}';
  }

  String _getDiscountValueText(DiscountEntity discount) {
    switch (discount.type) {
      case DiscountType.percentage:
        return '${discount.value.toStringAsFixed(0)}%';
      case DiscountType.fixedAmount:
        return 'GHS ${discount.value.toStringAsFixed(2)}';
      case DiscountType.bogo:
        return 'BOGO';
      case DiscountType.buyXGetY:
        return 'Buy X Get Y';
      default:
        return discount.value.toStringAsFixed(0);
    }
  }

  String _getDiscountTypeDisplayName(DiscountType type) {
    switch (type) {
      case DiscountType.percentage:
        return 'Percentage';
      case DiscountType.fixedAmount:
        return 'Fixed Amount';
      case DiscountType.bogo:
        return 'Buy One Get One';
      case DiscountType.buyXGetY:
        return 'Buy X Get Y';
      case DiscountType.freeShipping:
        return 'Free Shipping';
      case DiscountType.customerSpecific:
        return 'Customer Specific';
      case DiscountType.loyaltyPoints:
        return 'Loyalty Points';
    }
  }

  String _getDiscountScopeDisplayName(DiscountScope scope) {
    switch (scope) {
      case DiscountScope.item:
        return 'Specific Items';
      case DiscountScope.category:
        return 'Product Categories';
      case DiscountScope.brand:
        return 'Product Brands';
      case DiscountScope.cart:
        return 'Entire Cart';
      case DiscountScope.shipping:
        return 'Shipping';
      case DiscountScope.total:
        return 'Order Total';
    }
  }

  String _getDiscountApplicationDisplayName(DiscountApplication application) {
    switch (application) {
      case DiscountApplication.automatic:
        return 'Automatic';
      case DiscountApplication.couponCode:
        return 'Coupon Code';
      case DiscountApplication.loyaltyTier:
        return 'Loyalty Tier';
      case DiscountApplication.customerGroup:
        return 'Customer Group';
      case DiscountApplication.manual:
        return 'Manual Application';
    }
  }

  Color _getDiscountTypeColor(DiscountType type) {
    switch (type) {
      case DiscountType.percentage:
        return Colors.blue;
      case DiscountType.fixedAmount:
        return Colors.green;
      case DiscountType.bogo:
        return Colors.orange;
      case DiscountType.buyXGetY:
        return Colors.purple;
      case DiscountType.freeShipping:
        return Colors.teal;
      case DiscountType.customerSpecific:
        return Colors.indigo;
      case DiscountType.loyaltyPoints:
        return Colors.amber;
    }
  }

  IconData _getDiscountTypeIcon(DiscountType type) {
    switch (type) {
      case DiscountType.percentage:
        return Icons.percent;
      case DiscountType.fixedAmount:
        return Icons.attach_money;
      case DiscountType.bogo:
        return Icons.redeem;
      case DiscountType.buyXGetY:
        return Icons.card_giftcard;
      case DiscountType.freeShipping:
        return Icons.local_shipping;
      case DiscountType.customerSpecific:
        return Icons.person;
      case DiscountType.loyaltyPoints:
        return Icons.stars;
    }
  }

  String _getDurationText() {
    final duration = _discount.endDate.difference(_discount.startDate);
    if (duration.inDays > 365) {
      return '${(duration.inDays / 365).round()} year(s)';
    } else if (duration.inDays > 30) {
      return '${(duration.inDays / 30).round()} month(s)';
    } else {
      return '${duration.inDays} day(s)';
    }
  }

  String _getDateStatus() {
    final now = DateTime.now();
    if (now.isBefore(_discount.startDate)) {
      return 'Not started';
    } else if (now.isAfter(_discount.endDate)) {
      return 'Expired';
    } else {
      return 'Active period';
    }
  }

  String _getRemainingUsesText() {
    if (_discount.usageLimit == null) {
      return 'Unlimited';
    }
    
    final remaining = _discount.usageLimit! - _discount.currentUses;
    return remaining > 0 ? remaining.toString() : '0 (Limit reached)';
  }
}
