import 'package:flutter/material.dart';

import '../../domain/entities/discount_entity.dart';
import '../../data/services/discount_service.dart';
import '../widgets/common/custom_app_bar.dart';
import '../widgets/common/loading_widget.dart';
import 'add_discount_screen.dart';
import 'discount_details_screen.dart';

class DiscountManagementScreen extends StatefulWidget {
  const DiscountManagementScreen({super.key});

  @override
  State<DiscountManagementScreen> createState() => _DiscountManagementScreenState();
}

class _DiscountManagementScreenState extends State<DiscountManagementScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(
        title: 'Discount Management',
        actions: [
          IconButton(
            icon: const Icon(Icons.add),
            onPressed: () => _navigateToAddDiscount(),
            tooltip: 'Add Discount',
          ),
        ],
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(text: 'Active'),
            Tab(text: 'All Discounts'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: const [
          ActiveDiscountsTab(),
          AllDiscountsTab(),
        ],
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: _navigateToAddDiscount,
        tooltip: 'Add Discount',
        child: const Icon(Icons.add),
      ),
    );
  }

  void _navigateToAddDiscount() {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => const AddDiscountScreen(),
      ),
    );
  }
}

class ActiveDiscountsTab extends StatefulWidget {
  const ActiveDiscountsTab({super.key});

  @override
  State<ActiveDiscountsTab> createState() => _ActiveDiscountsTabState();
}

class _ActiveDiscountsTabState extends State<ActiveDiscountsTab> {
  final DiscountService _discountService = DiscountService();
  final _searchController = TextEditingController();
  
  List<DiscountEntity> _discounts = [];
  List<DiscountEntity> _filteredDiscounts = [];
  bool _isLoading = true;
  DiscountType? _selectedType;

  @override
  void initState() {
    super.initState();
    _loadDiscounts();
    _searchController.addListener(_filterDiscounts);
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _loadDiscounts() async {
    setState(() {
      _isLoading = true;
    });

    try {
      final discounts = await _discountService.getActiveDiscounts();
      setState(() {
        _discounts = discounts;
        _filteredDiscounts = discounts;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
      });
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to load discounts: $e')),
        );
      }
    }
  }

  void _filterDiscounts() {
    final query = _searchController.text.toLowerCase();
    setState(() {
      _filteredDiscounts = _discounts.where((discount) {
        final matchesSearch = discount.name.toLowerCase().contains(query) ||
                            discount.description.toLowerCase().contains(query);
        final matchesType = _selectedType == null || discount.type == _selectedType;
        return matchesSearch && matchesType;
      }).toList();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        // Search and Filter
        Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            children: [
              TextField(
                controller: _searchController,
                decoration: const InputDecoration(
                  hintText: 'Search discounts...',
                  prefixIcon: Icon(Icons.search),
                ),
              ),
              const SizedBox(height: 16),
              DropdownButtonFormField<DiscountType?>(
                value: _selectedType,
                decoration: const InputDecoration(
                  labelText: 'Filter by Type',
                  contentPadding: EdgeInsets.symmetric(horizontal: 12),
                ),
                items: [
                  const DropdownMenuItem(
                    value: null,
                    child: Text('All Types'),
                  ),
                  ...DiscountType.values.map((type) => DropdownMenuItem(
                    value: type,
                    child: Text(_getDiscountTypeDisplayName(type)),
                  )),
                ],
                onChanged: (value) {
                  setState(() {
                    _selectedType = value;
                  });
                  _filterDiscounts();
                },
              ),
            ],
          ),
        ),

        // Discounts List
        Expanded(
          child: RefreshIndicator(
            onRefresh: _loadDiscounts,
            child: _isLoading
                ? const LoadingWidget()
                : _filteredDiscounts.isEmpty
                    ? const Center(
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Icon(Icons.local_offer, size: 64, color: Colors.grey),
                            SizedBox(height: 16),
                            Text('No active discounts found'),
                            SizedBox(height: 8),
                            Text(
                              'Create a discount to get started',
                              style: TextStyle(color: Colors.grey),
                            ),
                          ],
                        ),
                      )
                    : ListView.builder(
                        padding: const EdgeInsets.symmetric(horizontal: 16),
                        itemCount: _filteredDiscounts.length,
                        itemBuilder: (context, index) {
                          final discount = _filteredDiscounts[index];
                          return _buildDiscountCard(discount);
                        },
                      ),
          ),
        ),
      ],
    );
  }

  Widget _buildDiscountCard(DiscountEntity discount) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: _getDiscountTypeColor(discount.type),
          child: Icon(
            _getDiscountTypeIcon(discount.type),
            color: Colors.white,
            size: 20,
          ),
        ),
        title: Text(
          discount.name,
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(discount.description),
            const SizedBox(height: 4),
            Row(
              children: [
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                  decoration: BoxDecoration(
                    color: _getDiscountTypeColor(discount.type).withOpacity(0.1),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Text(
                    _getDiscountTypeDisplayName(discount.type),
                    style: TextStyle(
                      color: _getDiscountTypeColor(discount.type),
                      fontSize: 12,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                const SizedBox(width: 8),
                Text(
                  'Expires: ${_formatDate(discount.endDate)}',
                  style: const TextStyle(fontSize: 12, color: Colors.grey),
                ),
              ],
            ),
          ],
        ),
        trailing: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Text(
              _getDiscountValueText(discount),
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                fontSize: 16,
              ),
            ),
            Text(
              '${discount.currentUses} uses',
              style: const TextStyle(fontSize: 12, color: Colors.grey),
            ),
          ],
        ),
        onTap: () => _viewDiscountDetails(discount),
        onLongPress: () => _showDiscountOptions(discount),
      ),
    );
  }

  void _viewDiscountDetails(DiscountEntity discount) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => DiscountDetailsScreen(discount: discount),
      ),
    ).then((_) => _loadDiscounts());
  }

  void _showDiscountOptions(DiscountEntity discount) {
    showModalBottomSheet(
      context: context,
      builder: (context) => Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          ListTile(
            leading: const Icon(Icons.visibility),
            title: const Text('View Details'),
            onTap: () {
              Navigator.pop(context);
              _viewDiscountDetails(discount);
            },
          ),
          ListTile(
            leading: const Icon(Icons.edit),
            title: const Text('Edit'),
            onTap: () {
              Navigator.pop(context);
              _editDiscount(discount);
            },
          ),
          ListTile(
            leading: Icon(discount.isActive ? Icons.pause : Icons.play_arrow),
            title: Text(discount.isActive ? 'Deactivate' : 'Activate'),
            onTap: () {
              Navigator.pop(context);
              _toggleDiscountStatus(discount);
            },
          ),
          ListTile(
            leading: const Icon(Icons.copy),
            title: const Text('Duplicate'),
            onTap: () {
              Navigator.pop(context);
              _duplicateDiscount(discount);
            },
          ),
          ListTile(
            leading: const Icon(Icons.delete, color: Colors.red),
            title: const Text('Delete', style: TextStyle(color: Colors.red)),
            onTap: () {
              Navigator.pop(context);
              _deleteDiscount(discount);
            },
          ),
        ],
      ),
    );
  }

  void _editDiscount(DiscountEntity discount) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => AddDiscountScreen(discount: discount),
      ),
    ).then((_) => _loadDiscounts());
  }

  Future<void> _toggleDiscountStatus(DiscountEntity discount) async {
    try {
      await _discountService.toggleDiscountStatus(discount.id, !discount.isActive);
      await _loadDiscounts();
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              discount.isActive 
                  ? 'Discount deactivated' 
                  : 'Discount activated',
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

  void _duplicateDiscount(DiscountEntity discount) {
    final duplicatedDiscount = discount.copyWith(
      id: DateTime.now().millisecondsSinceEpoch.toString(),
      name: '${discount.name} (Copy)',
      createdAt: DateTime.now(),
      updatedAt: null,
      currentUses: 0,
    );

    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => AddDiscountScreen(discount: duplicatedDiscount),
      ),
    ).then((_) => _loadDiscounts());
  }

  Future<void> _deleteDiscount(DiscountEntity discount) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Delete Discount'),
        content: Text('Are you sure you want to delete "${discount.name}"?'),
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
        await _discountService.deleteDiscount(discount.id);
        await _loadDiscounts();
        
        if (mounted) {
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
}

class AllDiscountsTab extends StatefulWidget {
  const AllDiscountsTab({super.key});

  @override
  State<AllDiscountsTab> createState() => _AllDiscountsTabState();
}

class _AllDiscountsTabState extends State<AllDiscountsTab> {
  final DiscountService _discountService = DiscountService();
  final _searchController = TextEditingController();
  
  List<DiscountEntity> _discounts = [];
  List<DiscountEntity> _filteredDiscounts = [];
  bool _isLoading = true;
  DiscountType? _selectedType;
  bool? _activeFilter;

  @override
  void initState() {
    super.initState();
    _loadDiscounts();
    _searchController.addListener(_filterDiscounts);
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  Future<void> _loadDiscounts() async {
    setState(() {
      _isLoading = true;
    });

    try {
      final discounts = await _discountService.getAllDiscounts();
      setState(() {
        _discounts = discounts;
        _filteredDiscounts = discounts;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
      });
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to load discounts: $e')),
        );
      }
    }
  }

  void _filterDiscounts() {
    final query = _searchController.text.toLowerCase();
    setState(() {
      _filteredDiscounts = _discounts.where((discount) {
        final matchesSearch = discount.name.toLowerCase().contains(query) ||
                            discount.description.toLowerCase().contains(query);
        final matchesType = _selectedType == null || discount.type == _selectedType;
        final matchesActive = _activeFilter == null || discount.isActive == _activeFilter;
        return matchesSearch && matchesType && matchesActive;
      }).toList();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        // Search and Filters
        Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            children: [
              TextField(
                controller: _searchController,
                decoration: const InputDecoration(
                  hintText: 'Search discounts...',
                  prefixIcon: Icon(Icons.search),
                ),
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  Expanded(
                    child: DropdownButtonFormField<DiscountType?>(
                      value: _selectedType,
                      decoration: const InputDecoration(
                        labelText: 'Type',
                        contentPadding: EdgeInsets.symmetric(horizontal: 12),
                      ),
                      items: [
                        const DropdownMenuItem(
                          value: null,
                          child: Text('All Types'),
                        ),
                        ...DiscountType.values.map((type) => DropdownMenuItem(
                          value: type,
                          child: Text(_getDiscountTypeDisplayName(type)),
                        )),
                      ],
                      onChanged: (value) {
                        setState(() {
                          _selectedType = value;
                        });
                        _filterDiscounts();
                      },
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: DropdownButtonFormField<bool?>(
                      value: _activeFilter,
                      decoration: const InputDecoration(
                        labelText: 'Status',
                        contentPadding: EdgeInsets.symmetric(horizontal: 12),
                      ),
                      items: const [
                        DropdownMenuItem(
                          value: null,
                          child: Text('All Status'),
                        ),
                        DropdownMenuItem(
                          value: true,
                          child: Text('Active'),
                        ),
                        DropdownMenuItem(
                          value: false,
                          child: Text('Inactive'),
                        ),
                      ],
                      onChanged: (value) {
                        setState(() {
                          _activeFilter = value;
                        });
                        _filterDiscounts();
                      },
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),

        // Discounts List
        Expanded(
          child: RefreshIndicator(
            onRefresh: _loadDiscounts,
            child: _isLoading
                ? const LoadingWidget()
                : _filteredDiscounts.isEmpty
                    ? const Center(
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Icon(Icons.local_offer, size: 64, color: Colors.grey),
                            SizedBox(height: 16),
                            Text('No discounts found'),
                          ],
                        ),
                      )
                    : ListView.builder(
                        padding: const EdgeInsets.symmetric(horizontal: 16),
                        itemCount: _filteredDiscounts.length,
                        itemBuilder: (context, index) {
                          final discount = _filteredDiscounts[index];
                          return _buildDiscountCard(discount);
                        },
                      ),
          ),
        ),
      ],
    );
  }

  Widget _buildDiscountCard(DiscountEntity discount) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: discount.isActive 
              ? _getDiscountTypeColor(discount.type) 
              : Colors.grey,
          child: Icon(
            _getDiscountTypeIcon(discount.type),
            color: Colors.white,
            size: 20,
          ),
        ),
        title: Text(
          discount.name,
          style: TextStyle(
            fontWeight: FontWeight.bold,
            color: discount.isActive ? null : Colors.grey,
          ),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              discount.description,
              style: TextStyle(
                color: discount.isActive ? null : Colors.grey,
              ),
            ),
            const SizedBox(height: 4),
            Row(
              children: [
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                  decoration: BoxDecoration(
                    color: discount.isActive 
                        ? _getDiscountTypeColor(discount.type).withOpacity(0.1)
                        : Colors.grey.withOpacity(0.1),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Text(
                    _getDiscountTypeDisplayName(discount.type),
                    style: TextStyle(
                      color: discount.isActive 
                          ? _getDiscountTypeColor(discount.type)
                          : Colors.grey,
                      fontSize: 12,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                const SizedBox(width: 8),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                  decoration: BoxDecoration(
                    color: discount.isActive 
                        ? Colors.green.withOpacity(0.1)
                        : Colors.grey.withOpacity(0.1),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Text(
                    discount.isActive ? 'ACTIVE' : 'INACTIVE',
                    style: TextStyle(
                      color: discount.isActive ? Colors.green : Colors.grey,
                      fontSize: 10,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
        trailing: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Text(
              _getDiscountValueText(discount),
              style: TextStyle(
                fontWeight: FontWeight.bold,
                fontSize: 16,
                color: discount.isActive ? null : Colors.grey,
              ),
            ),
            Text(
              '${discount.currentUses} uses',
              style: const TextStyle(fontSize: 12, color: Colors.grey),
            ),
          ],
        ),
        onTap: () => _viewDiscountDetails(discount),
      ),
    );
  }

  void _viewDiscountDetails(DiscountEntity discount) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => DiscountDetailsScreen(discount: discount),
      ),
    ).then((_) => _loadDiscounts());
  }

  // Helper methods (same as ActiveDiscountsTab)
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
}
