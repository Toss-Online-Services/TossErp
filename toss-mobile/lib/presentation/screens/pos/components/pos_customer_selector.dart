import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../../../../domain/entities/customer_entity.dart';

class POSCustomerSelector extends StatelessWidget {
  final CustomerEntity? selectedCustomer;
  final Function(CustomerEntity?) onCustomerChanged;
  final Function(String)? onCustomerScanned;

  const POSCustomerSelector({
    super.key,
    required this.selectedCustomer,
    required this.onCustomerChanged,
    this.onCustomerScanned,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceVariant,
        border: Border(
          bottom: BorderSide(
            color: Theme.of(context).colorScheme.outline.withOpacity(0.2),
          ),
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(
                Icons.person_outline,
                color: Theme.of(context).colorScheme.primary,
              ),
              const SizedBox(width: 8),
              Text(
                'Customer',
                style: Theme.of(context).textTheme.titleSmall?.copyWith(
                  fontWeight: FontWeight.w600,
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          if (selectedCustomer != null) ...[
            _buildSelectedCustomerCard(context),
          ] else ...[
            _buildCustomerSelector(context),
          ],
        ],
      ),
    );
  }

  Widget _buildSelectedCustomerCard(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                CircleAvatar(
                  radius: 20,
                  backgroundColor: Theme.of(context).colorScheme.primary,
                  child: Text(
                    selectedCustomer!.name?.substring(0, 1).toUpperCase() ?? 'C',
                    style: const TextStyle(
                      color: Colors.white,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        selectedCustomer!.name ?? 'Walk-in Customer',
                        style: Theme.of(context).textTheme.titleSmall?.copyWith(
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      if (selectedCustomer!.phone != null) ...[
                        const SizedBox(height: 2),
                        Text(
                          selectedCustomer!.phone!,
                          style: Theme.of(context).textTheme.bodySmall?.copyWith(
                            color: Colors.grey[600],
                          ),
                        ),
                      ],
                      if (selectedCustomer!.membershipNumber != null) ...[
                        const SizedBox(height: 2),
                        Text(
                          'Member: ${selectedCustomer!.membershipNumber}',
                          style: Theme.of(context).textTheme.bodySmall?.copyWith(
                            color: Colors.grey[600],
                          ),
                        ),
                      ],
                    ],
                  ),
                ),
                // Customer tier badge
                if (selectedCustomer!.tier != CustomerTier.bronze)
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(
                      color: _getTierColor(selectedCustomer!.tier),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Text(
                      selectedCustomer!.tier.name.toUpperCase(),
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 10,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                const SizedBox(width: 8),
                IconButton(
                  icon: const Icon(Icons.clear),
                  onPressed: () => onCustomerChanged(null),
                  visualDensity: VisualDensity.compact,
                ),
              ],
            ),
            if (selectedCustomer!.loyaltyPoints > 0) ...[
              const SizedBox(height: 8),
              Container(
                padding: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  color: Colors.orange.withOpacity(0.1),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: Colors.orange.withOpacity(0.3)),
                ),
                child: Row(
                  children: [
                    const Icon(
                      Icons.stars,
                      color: Colors.orange,
                      size: 16,
                    ),
                    const SizedBox(width: 4),
                    Text(
                      '${selectedCustomer!.loyaltyPoints} Loyalty Points',
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: Colors.orange[800],
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                    const Spacer(),
                    Text(
                      'Value: \$${(selectedCustomer!.loyaltyPoints * 0.01).toStringAsFixed(2)}',
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: Colors.orange[800],
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildCustomerSelector(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: OutlinedButton.icon(
            onPressed: () => _showCustomerSearchDialog(context),
            icon: const Icon(Icons.search),
            label: const Text('Search Customer'),
            style: OutlinedButton.styleFrom(
              padding: const EdgeInsets.symmetric(vertical: 12),
            ),
          ),
        ),
        const SizedBox(width: 8),
        OutlinedButton.icon(
          onPressed: () => _showWalkInCustomerDialog(context),
          icon: const Icon(Icons.person_add),
          label: const Text('Walk-in'),
          style: OutlinedButton.styleFrom(
            padding: const EdgeInsets.symmetric(vertical: 12),
          ),
        ),
        const SizedBox(width: 8),
        IconButton(
          onPressed: () => _showQRScannerDialog(context),
          icon: const Icon(Icons.qr_code_scanner),
          style: IconButton.styleFrom(
            backgroundColor: Theme.of(context).colorScheme.primary,
            foregroundColor: Theme.of(context).colorScheme.onPrimary,
          ),
        ),
      ],
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

  void _showCustomerSearchDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => _CustomerSearchDialog(
        onCustomerSelected: onCustomerChanged,
      ),
    );
  }

  void _showWalkInCustomerDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => _WalkInCustomerDialog(
        onCustomerCreated: onCustomerChanged,
      ),
    );
  }

  void _showQRScannerDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Scan Customer QR Code'),
        content: const Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              Icons.qr_code_scanner,
              size: 80,
              color: Colors.grey,
            ),
            SizedBox(height: 16),
            Text('QR Scanner functionality will be integrated here'),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () {
              Navigator.of(context).pop();
              // Simulate QR scan
              if (onCustomerScanned != null) {
                onCustomerScanned!('CUSTOMER_QR_12345');
              }
            },
            child: const Text('Simulate Scan'),
          ),
        ],
      ),
    );
  }
}

class _CustomerSearchDialog extends StatefulWidget {
  final Function(CustomerEntity?) onCustomerSelected;

  const _CustomerSearchDialog({
    required this.onCustomerSelected,
  });

  @override
  State<_CustomerSearchDialog> createState() => _CustomerSearchDialogState();
}

class _CustomerSearchDialogState extends State<_CustomerSearchDialog> {
  final TextEditingController _searchController = TextEditingController();
  List<CustomerEntity> _searchResults = [];
  bool _isLoading = false;
  String _searchQuery = '';

  // Mock customers for demonstration
  final List<CustomerEntity> _mockCustomers = [
    CustomerEntity(
      id: '1',
      name: 'John Smith',
      phone: '+1234567890',
      membershipNumber: 'MEM001',
      tier: CustomerTier.gold,
      loyaltyPoints: 1250,
      totalSpent: 89999,
      visitCount: 25,
    ),
    CustomerEntity(
      id: '2',
      name: 'Sarah Johnson',
      phone: '+1234567891',
      membershipNumber: 'MEM002',
      tier: CustomerTier.silver,
      loyaltyPoints: 750,
      totalSpent: 45000,
      visitCount: 15,
    ),
    CustomerEntity(
      id: '3',
      name: 'Mike Wilson',
      phone: '+1234567892',
      tier: CustomerTier.bronze,
      loyaltyPoints: 150,
      totalSpent: 12000,
      visitCount: 5,
    ),
  ];

  @override
  void initState() {
    super.initState();
    _searchResults = _mockCustomers;
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void _performSearch(String query) {
    setState(() {
      _isLoading = true;
      _searchQuery = query;
    });

    // Simulate search delay
    Future.delayed(const Duration(milliseconds: 500), () {
      if (mounted) {
        setState(() {
          _isLoading = false;
          if (query.isEmpty) {
            _searchResults = _mockCustomers;
          } else {
            _searchResults = _mockCustomers.where((customer) {
              return (customer.name?.toLowerCase().contains(query.toLowerCase()) ?? false) ||
                     (customer.phone?.contains(query) ?? false) ||
                     (customer.membershipNumber?.toLowerCase().contains(query.toLowerCase()) ?? false);
            }).toList();
          }
        });
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Dialog(
      child: Container(
        width: MediaQuery.of(context).size.width * 0.8,
        height: MediaQuery.of(context).size.height * 0.7,
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                const Icon(Icons.search),
                const SizedBox(width: 8),
                Text(
                  'Search Customer',
                  style: Theme.of(context).textTheme.titleLarge?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const Spacer(),
                IconButton(
                  onPressed: () => Navigator.of(context).pop(),
                  icon: const Icon(Icons.close),
                ),
              ],
            ),
            const SizedBox(height: 16),
            TextField(
              controller: _searchController,
              decoration: const InputDecoration(
                hintText: 'Search by name, phone, or membership number...',
                prefixIcon: Icon(Icons.search),
                border: OutlineInputBorder(),
              ),
              onChanged: _performSearch,
              autofocus: true,
            ),
            const SizedBox(height: 16),
            Text(
              _searchQuery.isEmpty 
                  ? 'Recent Customers'
                  : 'Search Results (${_searchResults.length})',
              style: Theme.of(context).textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.w600,
              ),
            ),
            const SizedBox(height: 12),
            Expanded(
              child: _isLoading
                  ? const Center(child: CircularProgressIndicator())
                  : _searchResults.isEmpty
                      ? Center(
                          child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              const Icon(
                                Icons.search_off,
                                size: 64,
                                color: Colors.grey,
                              ),
                              const SizedBox(height: 16),
                              Text(
                                'No customers found',
                                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                                  color: Colors.grey,
                                ),
                              ),
                              const SizedBox(height: 8),
                              OutlinedButton.icon(
                                onPressed: () {
                                  Navigator.of(context).pop();
                                  _showCreateCustomerDialog(context);
                                },
                                icon: const Icon(Icons.person_add),
                                label: const Text('Create New Customer'),
                              ),
                            ],
                          ),
                        )
                      : ListView.builder(
                          itemCount: _searchResults.length,
                          itemBuilder: (context, index) {
                            return _buildCustomerListItem(_searchResults[index]);
                          },
                        ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildCustomerListItem(CustomerEntity customer) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: _getTierColor(customer.tier),
          child: Text(
            customer.name?.substring(0, 1).toUpperCase() ?? 'C',
            style: const TextStyle(
              color: Colors.white,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        title: Text(
          customer.name ?? 'Unknown Customer',
          style: const TextStyle(fontWeight: FontWeight.w600),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (customer.phone != null)
              Text(customer.phone!),
            Row(
              children: [
                if (customer.membershipNumber != null) ...[
                  Icon(Icons.card_membership, size: 14, color: Colors.grey[600]),
                  const SizedBox(width: 4),
                  Text(
                    customer.membershipNumber!,
                    style: TextStyle(color: Colors.grey[600]),
                  ),
                  const SizedBox(width: 12),
                ],
                Icon(Icons.stars, size: 14, color: Colors.orange),
                const SizedBox(width: 4),
                Text(
                  '${customer.loyaltyPoints} pts',
                  style: const TextStyle(color: Colors.orange),
                ),
              ],
            ),
          ],
        ),
        trailing: Container(
          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
          decoration: BoxDecoration(
            color: _getTierColor(customer.tier),
            borderRadius: BorderRadius.circular(12),
          ),
          child: Text(
            customer.tier.name.toUpperCase(),
            style: const TextStyle(
              color: Colors.white,
              fontSize: 10,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        onTap: () {
          widget.onCustomerSelected(customer);
          Navigator.of(context).pop();
        },
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

  void _showCreateCustomerDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => _WalkInCustomerDialog(
        onCustomerCreated: widget.onCustomerSelected,
        searchQuery: _searchQuery,
      ),
    );
  }
}

class _WalkInCustomerDialog extends StatefulWidget {
  final Function(CustomerEntity?) onCustomerCreated;
  final String? searchQuery;

  const _WalkInCustomerDialog({
    required this.onCustomerCreated,
    this.searchQuery,
  });

  @override
  State<_WalkInCustomerDialog> createState() => _WalkInCustomerDialogState();
}

class _WalkInCustomerDialogState extends State<_WalkInCustomerDialog> {
  final _formKey = GlobalKey<FormState>();
  final _nameController = TextEditingController();
  final _phoneController = TextEditingController();
  bool _saveCustomer = true;

  @override
  void initState() {
    super.initState();
    if (widget.searchQuery != null) {
      _nameController.text = widget.searchQuery!;
    }
  }

  @override
  void dispose() {
    _nameController.dispose();
    _phoneController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Row(
        children: [
          Icon(Icons.person_add),
          SizedBox(width: 8),
          Text('Walk-in Customer'),
        ],
      ),
      content: Form(
        key: _formKey,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextFormField(
              controller: _nameController,
              decoration: const InputDecoration(
                labelText: 'Customer Name',
                border: OutlineInputBorder(),
                prefixIcon: Icon(Icons.person),
              ),
              validator: (value) {
                if (value == null || value.trim().isEmpty) {
                  return 'Please enter customer name';
                }
                return null;
              },
              autofocus: true,
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _phoneController,
              decoration: const InputDecoration(
                labelText: 'Phone Number (Optional)',
                border: OutlineInputBorder(),
                prefixIcon: Icon(Icons.phone),
              ),
              keyboardType: TextInputType.phone,
              inputFormatters: [
                FilteringTextInputFormatter.digitsOnly,
              ],
            ),
            const SizedBox(height: 16),
            CheckboxListTile(
              value: _saveCustomer,
              onChanged: (value) {
                setState(() {
                  _saveCustomer = value ?? true;
                });
              },
              title: const Text('Save customer for future transactions'),
              subtitle: const Text('Customer will be added to your database'),
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.of(context).pop(),
          child: const Text('Cancel'),
        ),
        OutlinedButton(
          onPressed: () {
            // Create anonymous walk-in customer
            final customer = CustomerEntity(
              id: 'walkin_${DateTime.now().millisecondsSinceEpoch}',
              name: 'Walk-in Customer',
              primaryIdType: CustomerIdType.phone,
            );
            widget.onCustomerCreated(customer);
            Navigator.of(context).pop();
          },
          child: const Text('Anonymous'),
        ),
        ElevatedButton(
          onPressed: _createCustomer,
          child: const Text('Create'),
        ),
      ],
    );
  }

  void _createCustomer() {
    if (_formKey.currentState!.validate()) {
      final customer = CustomerEntity(
        id: _saveCustomer 
            ? 'customer_${DateTime.now().millisecondsSinceEpoch}'
            : 'walkin_${DateTime.now().millisecondsSinceEpoch}',
        name: _nameController.text.trim(),
        phone: _phoneController.text.trim().isEmpty ? null : _phoneController.text.trim(),
        primaryIdType: CustomerIdType.phone,
        tier: CustomerTier.bronze,
        loyaltyPoints: 0,
        totalSpent: 0,
        visitCount: 0,
        createdAt: DateTime.now().toIso8601String(),
      );

      if (_saveCustomer) {
        // TODO: Save customer to database
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Customer created successfully!'),
            backgroundColor: Colors.green,
          ),
        );
      }

      widget.onCustomerCreated(customer);
      Navigator.of(context).pop();
    }
  }
}
