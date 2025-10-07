import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../domain/entities/customer_entity.dart';
import '../../providers/customer/customer_provider.dart';
import 'customer_form_screen.dart';
import 'customer_detail_screen.dart';

class CustomerManagementScreen extends StatefulWidget {
  const CustomerManagementScreen({super.key});

  @override
  State<CustomerManagementScreen> createState() => _CustomerManagementScreenState();
}

class _CustomerManagementScreenState extends State<CustomerManagementScreen> {
  final TextEditingController _searchController = TextEditingController();
  CustomerTier? _selectedTier;
  LoyaltyTier? _selectedLoyaltyTier;
  String? _selectedTag;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<CustomerProvider>().loadAllCustomers();
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
        title: const Text('Customer Management'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: () {
              context.read<CustomerProvider>().loadAllCustomers();
            },
          ),
          IconButton(
            icon: const Icon(Icons.add),
            onPressed: () {
              _navigateToCustomerForm();
            },
          ),
        ],
      ),
      body: Consumer<CustomerProvider>(
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
                      provider.loadAllCustomers();
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
                child: _buildCustomerList(provider),
              ),
            ],
          );
        },
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: _navigateToCustomerForm,
        child: const Icon(Icons.add),
      ),
    );
  }

  Widget _buildSearchAndFilters(CustomerProvider provider) {
    return Container(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          // Search bar
          TextField(
            controller: _searchController,
            decoration: InputDecoration(
              hintText: 'Search customers...',
              prefixIcon: const Icon(Icons.search),
              suffixIcon: _searchController.text.isNotEmpty
                  ? IconButton(
                      icon: const Icon(Icons.clear),
                      onPressed: () {
                        _searchController.clear();
                        provider.searchCustomers('');
                      },
                    )
                  : null,
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
              ),
            ),
            onChanged: (value) {
              provider.searchCustomers(value);
            },
          ),
          const SizedBox(height: 16),
          
          // Filter chips
          SingleChildScrollView(
            scrollDirection: Axis.horizontal,
            child: Row(
              children: [
                _buildFilterChip(
                  'All Tiers',
                  _selectedTier == null,
                  () {
                    setState(() {
                      _selectedTier = null;
                    });
                    provider.filterByTier(null);
                  },
                ),
                ...CustomerTier.values.map((tier) => _buildFilterChip(
                  tier.name.toUpperCase(),
                  _selectedTier == tier,
                  () {
                    setState(() {
                      _selectedTier = tier;
                    });
                    provider.filterByTier(tier);
                  },
                )),
                const SizedBox(width: 8),
                _buildFilterChip(
                  'All Loyalty',
                  _selectedLoyaltyTier == null,
                  () {
                    setState(() {
                      _selectedLoyaltyTier = null;
                    });
                    provider.filterByLoyaltyTier(null);
                  },
                ),
                ...LoyaltyTier.values.map((tier) => _buildFilterChip(
                  tier.name.toUpperCase(),
                  _selectedLoyaltyTier == tier,
                  () {
                    setState(() {
                      _selectedLoyaltyTier = tier;
                    });
                    provider.filterByLoyaltyTier(tier);
                  },
                )),
              ],
            ),
          ),
          const SizedBox(height: 8),
          
          // Action buttons
          Row(
            children: [
              Expanded(
                child: OutlinedButton.icon(
                  onPressed: () {
                    provider.loadBirthdayCustomers();
                  },
                  icon: const Icon(Icons.cake),
                  label: const Text('Birthdays'),
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: OutlinedButton.icon(
                  onPressed: () {
                    provider.loadInactiveCustomers();
                  },
                  icon: const Icon(Icons.person_off),
                  label: const Text('Inactive'),
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: OutlinedButton.icon(
                  onPressed: () {
                    provider.seedSampleCustomers();
                  },
                  icon: const Icon(Icons.data_usage),
                  label: const Text('Seed Data'),
                ),
              ),
            ],
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

  Widget _buildStatsCards(CustomerProvider provider) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Row(
        children: [
          Expanded(
            child: _buildStatCard(
              'Total',
              provider.totalCustomers.toString(),
              Icons.people,
              Colors.blue,
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: _buildStatCard(
              'Active',
              provider.activeCustomers.toString(),
              Icons.person,
              Colors.green,
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: _buildStatCard(
              'Birthdays',
              provider.birthdayCustomers.toString(),
              Icons.cake,
              Colors.orange,
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: _buildStatCard(
              'Value',
              '\$${provider.totalCustomerValue.toStringAsFixed(0)}',
              Icons.attach_money,
              Colors.purple,
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

  Widget _buildCustomerList(CustomerProvider provider) {
    final customers = provider.customers;

    if (customers.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.people_outline,
              size: 64,
              color: Colors.grey[400],
            ),
            const SizedBox(height: 16),
            Text(
              'No customers found',
              style: Theme.of(context).textTheme.titleLarge?.copyWith(
                color: Colors.grey[600],
              ),
            ),
            const SizedBox(height: 8),
            Text(
              'Add your first customer to get started',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: Colors.grey[500],
              ),
            ),
            const SizedBox(height: 16),
            ElevatedButton.icon(
              onPressed: _navigateToCustomerForm,
              icon: const Icon(Icons.add),
              label: const Text('Add Customer'),
            ),
          ],
        ),
      );
    }

    return ListView.builder(
      padding: const EdgeInsets.all(16),
      itemCount: customers.length,
      itemBuilder: (context, index) {
        final customer = customers[index];
        return _buildCustomerCard(customer, provider);
      },
    );
  }

  Widget _buildCustomerCard(CustomerEntity customer, CustomerProvider provider) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: _getTierColor(customer.tier),
          child: Text(
            customer.name?.substring(0, 1).toUpperCase() ?? '?',
            style: const TextStyle(
              color: Colors.white,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        title: Text(
          customer.name ?? 'Unknown',
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (customer.phone != null) Text('Phone: ${customer.phone}'),
            Row(
              children: [
                Chip(
                  label: Text(
                    customer.tier.name.toUpperCase(),
                    style: const TextStyle(fontSize: 10),
                  ),
                  backgroundColor: _getTierColor(customer.tier).withOpacity(0.2),
                ),
                const SizedBox(width: 4),
                Chip(
                  label: Text(
                    customer.loyaltyTier.name.toUpperCase(),
                    style: const TextStyle(fontSize: 10),
                  ),
                  backgroundColor: _getLoyaltyTierColor(customer.loyaltyTier).withOpacity(0.2),
                ),
              ],
            ),
            Text(
              'Visits: ${customer.visitCount} | Spent: \$${customer.totalSpent.toStringAsFixed(2)}',
              style: Theme.of(context).textTheme.bodySmall,
            ),
          ],
        ),
        trailing: PopupMenuButton<String>(
          onSelected: (value) {
            switch (value) {
              case 'view':
                _navigateToCustomerDetail(customer);
                break;
              case 'edit':
                _navigateToCustomerForm(customer: customer);
                break;
              case 'delete':
                _showDeleteDialog(customer, provider);
                break;
            }
          },
          itemBuilder: (context) => [
            const PopupMenuItem(
              value: 'view',
              child: ListTile(
                leading: Icon(Icons.visibility),
                title: Text('View Details'),
                contentPadding: EdgeInsets.zero,
              ),
            ),
            const PopupMenuItem(
              value: 'edit',
              child: ListTile(
                leading: Icon(Icons.edit),
                title: Text('Edit'),
                contentPadding: EdgeInsets.zero,
              ),
            ),
            const PopupMenuItem(
              value: 'delete',
              child: ListTile(
                leading: Icon(Icons.delete, color: Colors.red),
                title: Text('Delete', style: TextStyle(color: Colors.red)),
                contentPadding: EdgeInsets.zero,
              ),
            ),
          ],
        ),
        onTap: () => _navigateToCustomerDetail(customer),
      ),
    );
  }

  Color _getTierColor(CustomerTier tier) {
    switch (tier) {
      case CustomerTier.bronze:
        return Colors.brown;
      case CustomerTier.silver:
        return Colors.grey;
      case CustomerTier.gold:
        return Colors.amber;
      case CustomerTier.platinum:
        return Colors.blue;
      case CustomerTier.vip:
        return Colors.purple;
    }
  }

  Color _getLoyaltyTierColor(LoyaltyTier tier) {
    switch (tier) {
      case LoyaltyTier.none:
        return Colors.grey;
      case LoyaltyTier.bronze:
        return Colors.brown;
      case LoyaltyTier.silver:
        return Colors.grey;
      case LoyaltyTier.gold:
        return Colors.amber;
      case LoyaltyTier.platinum:
        return Colors.blue;
      case LoyaltyTier.vip:
        return Colors.purple;
    }
  }

  void _navigateToCustomerForm({CustomerEntity? customer}) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => CustomerFormScreen(customer: customer),
      ),
    );
  }

  void _navigateToCustomerDetail(CustomerEntity customer) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => CustomerDetailScreen(customer: customer),
      ),
    );
  }

  void _showDeleteDialog(CustomerEntity customer, CustomerProvider provider) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Delete Customer'),
        content: Text('Are you sure you want to delete ${customer.name}?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () async {
              Navigator.pop(context);
              final success = await provider.deleteCustomer(customer.id);
              if (success && mounted) {
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(
                    content: Text('${customer.name} deleted successfully'),
                    backgroundColor: Colors.green,
                  ),
                );
              }
            },
            child: const Text('Delete', style: TextStyle(color: Colors.red)),
          ),
        ],
      ),
    );
  }
}
