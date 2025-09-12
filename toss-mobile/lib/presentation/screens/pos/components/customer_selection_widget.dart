import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:local_auth/local_auth.dart';
import 'package:mobile_scanner/mobile_scanner.dart';

import '../../../../domain/entities/customer_entity.dart';

class CustomerSelectionWidget extends StatefulWidget {
  final CustomerEntity? selectedCustomer;
  final Function(CustomerEntity) onCustomerSelected;
  final VoidCallback onClearSelection;

  const CustomerSelectionWidget({
    super.key,
    this.selectedCustomer,
    required this.onCustomerSelected,
    required this.onClearSelection,
  });

  @override
  State<CustomerSelectionWidget> createState() => _CustomerSelectionWidgetState();
}

class _CustomerSelectionWidgetState extends State<CustomerSelectionWidget> {
  final TextEditingController _searchController = TextEditingController();
  final LocalAuthentication _localAuth = LocalAuthentication();
  List<CustomerEntity> _searchResults = [];
  bool _isSearching = false;

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        border: Border(
          bottom: BorderSide(
            color: Theme.of(context).colorScheme.outline.withOpacity(0.2),
          ),
        ),
      ),
      child: Column(
        children: [
          _buildSelectedCustomerBar(),
          if (widget.selectedCustomer == null) _buildCustomerSearchSection(),
        ],
      ),
    );
  }

  Widget _buildSelectedCustomerBar() {
    if (widget.selectedCustomer == null) {
      return Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
        child: Row(
          children: [
            Icon(
              Icons.person_add,
              color: Theme.of(context).colorScheme.primary,
            ),
            const SizedBox(width: 8),
            const Text(
              'Walk-in Customer',
              style: TextStyle(
                fontWeight: FontWeight.w500,
              ),
            ),
            const Spacer(),
            OutlinedButton.icon(
              onPressed: _showCustomerSelectionOptions,
              icon: const Icon(Icons.person_search, size: 18),
              label: const Text('Select Customer'),
              style: OutlinedButton.styleFrom(
                padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
              ),
            ),
          ],
        ),
      );
    }

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.primaryContainer.withOpacity(0.3),
      ),
      child: Row(
        children: [
          CircleAvatar(
            backgroundColor: Theme.of(context).colorScheme.primary,
            child: Text(
              widget.selectedCustomer!.name?.isNotEmpty == true 
                  ? widget.selectedCustomer!.name![0].toUpperCase()
                  : '?',
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
                  widget.selectedCustomer!.name ?? 'Unknown Customer',
                  style: Theme.of(context).textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
                if (widget.selectedCustomer!.phone?.isNotEmpty == true)
                  Text(
                    widget.selectedCustomer!.phone!,
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      color: Colors.grey[600],
                    ),
                  ),
                if (widget.selectedCustomer!.loyaltyTier != LoyaltyTier.none) ...[
                  const SizedBox(height: 4),
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                    decoration: BoxDecoration(
                      color: _getLoyaltyTierColor(widget.selectedCustomer!.loyaltyTier),
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Text(
                      _getLoyaltyTierName(widget.selectedCustomer!.loyaltyTier),
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 10,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ),
                ],
              ],
            ),
          ),
          Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              if (widget.selectedCustomer!.loyaltyPoints > 0)
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: Colors.amber,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Text(
                    '${widget.selectedCustomer!.loyaltyPoints} pts',
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 11,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              const SizedBox(width: 8),
              IconButton(
                onPressed: widget.onClearSelection,
                icon: const Icon(Icons.close, size: 20),
                visualDensity: VisualDensity.compact,
                tooltip: 'Remove customer',
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildCustomerSearchSection() {
    return Container(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          TextField(
            controller: _searchController,
            decoration: InputDecoration(
              hintText: 'Search by phone, name, or membership',
              prefixIcon: const Icon(Icons.search),
              suffixIcon: _isSearching
                  ? const SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : null,
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
              ),
            ),
            onChanged: _performSearch,
          ),
          if (_searchResults.isNotEmpty) ...[
            const SizedBox(height: 12),
            Container(
              constraints: const BoxConstraints(maxHeight: 200),
              decoration: BoxDecoration(
                border: Border.all(color: Colors.grey[300]!),
                borderRadius: BorderRadius.circular(8),
              ),
              child: ListView.builder(
                shrinkWrap: true,
                itemCount: _searchResults.length,
                itemBuilder: (context, index) {
                  return _buildCustomerSearchResult(_searchResults[index]);
                },
              ),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildCustomerSearchResult(CustomerEntity customer) {
    return ListTile(
      leading: CircleAvatar(
        backgroundColor: Theme.of(context).colorScheme.primary.withOpacity(0.2),
        child: Text(
          customer.name?.isNotEmpty == true 
              ? customer.name![0].toUpperCase()
              : '?',
          style: TextStyle(
            color: Theme.of(context).colorScheme.primary,
            fontWeight: FontWeight.bold,
          ),
        ),
      ),
      title: Text(customer.name ?? 'Unknown Customer'),
      subtitle: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          if (customer.phone?.isNotEmpty == true) Text(customer.phone!),
          if (customer.loyaltyTier != LoyaltyTier.none)
            Text(
              '${_getLoyaltyTierName(customer.loyaltyTier)} • ${customer.loyaltyPoints} points',
              style: TextStyle(
                color: _getLoyaltyTierColor(customer.loyaltyTier),
                fontWeight: FontWeight.w500,
              ),
            ),
        ],
      ),
      trailing: customer.loyaltyPoints > 0
          ? Container(
              padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
              decoration: BoxDecoration(
                color: Colors.amber,
                borderRadius: BorderRadius.circular(8),
              ),
              child: Text(
                '${customer.loyaltyPoints}',
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 10,
                  fontWeight: FontWeight.bold,
                ),
              ),
            )
          : null,
      onTap: () {
        widget.onCustomerSelected(customer);
        _searchController.clear();
        _searchResults.clear();
      },
    );
  }

  void _performSearch(String query) {
    if (query.length < 2) {
      setState(() {
        _searchResults.clear();
        _isSearching = false;
      });
      return;
    }

    setState(() {
      _isSearching = true;
    });

    // TODO: Implement actual customer search
    // This would typically call a repository or service
    Future.delayed(const Duration(milliseconds: 500), () {
      if (mounted) {
        setState(() {
          // Mock search results
          _searchResults = [
            CustomerEntity(
              id: '1',
              name: 'John Doe',
              phone: '+1234567890',
              loyaltyTier: LoyaltyTier.silver,
              loyaltyPoints: 250,
              createdAt: DateTime.now().toIso8601String(),
              updatedAt: DateTime.now().toIso8601String(),
            ),
            CustomerEntity(
              id: '2',
              name: 'Jane Smith',
              phone: '+0987654321',
              loyaltyTier: LoyaltyTier.gold,
              loyaltyPoints: 500,
              createdAt: DateTime.now().toIso8601String(),
              updatedAt: DateTime.now().toIso8601String(),
            ),
          ].where((customer) =>
              (customer.name?.toLowerCase().contains(query.toLowerCase()) ?? false) ||
              (customer.phone?.contains(query) ?? false)
          ).toList();
          _isSearching = false;
        });
      }
    });
  }

  void _showCustomerSelectionOptions() {
    showModalBottomSheet(
      context: context,
      builder: (context) => Container(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Select Customer',
              style: Theme.of(context).textTheme.titleLarge?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            _buildSelectionOption(
              icon: Icons.search,
              title: 'Search by Name/Phone',
              subtitle: 'Find existing customer',
              onTap: () {
                Navigator.of(context).pop();
                // Focus search field
                Future.delayed(const Duration(milliseconds: 100), () {
                  FocusScope.of(context).requestFocus(FocusNode());
                });
              },
            ),
            _buildSelectionOption(
              icon: Icons.qr_code_scanner,
              title: 'Scan QR Code',
              subtitle: 'Scan customer membership QR code',
              onTap: () {
                Navigator.of(context).pop();
                _scanCustomerQR();
              },
            ),
            _buildSelectionOption(
              icon: Icons.nfc,
              title: 'NFC Tag',
              subtitle: 'Tap customer membership card',
              onTap: () {
                Navigator.of(context).pop();
                _readNFCCustomer();
              },
            ),
            _buildSelectionOption(
              icon: Icons.fingerprint,
              title: 'Biometric ID',
              subtitle: 'Fingerprint or face recognition',
              onTap: () {
                Navigator.of(context).pop();
                _authenticateCustomerBiometric();
              },
            ),
            _buildSelectionOption(
              icon: Icons.person_add,
              title: 'New Customer',
              subtitle: 'Register a new customer',
              onTap: () {
                Navigator.of(context).pop();
                _showNewCustomerDialog();
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSelectionOption({
    required IconData icon,
    required String title,
    required String subtitle,
    required VoidCallback onTap,
  }) {
    return ListTile(
      leading: CircleAvatar(
        backgroundColor: Theme.of(context).colorScheme.primary.withOpacity(0.1),
        child: Icon(
          icon,
          color: Theme.of(context).colorScheme.primary,
        ),
      ),
      title: Text(title),
      subtitle: Text(subtitle),
      trailing: const Icon(Icons.chevron_right),
      onTap: onTap,
    );
  }

  void _scanCustomerQR() {
    Navigator.of(context).push(
      MaterialPageRoute(
        builder: (context) => Scaffold(
          appBar: AppBar(
            title: const Text('Scan Customer QR'),
            backgroundColor: Colors.black,
          ),
          body: MobileScanner(
            onDetect: (capture) {
              final barcodes = capture.barcodes;
              if (barcodes.isNotEmpty) {
                final qrData = barcodes.first.rawValue;
                if (qrData != null) {
                  Navigator.of(context).pop();
                  _processCustomerQR(qrData);
                }
              }
            },
          ),
        ),
      ),
    );
  }

  void _processCustomerQR(String qrData) {
    // TODO: Implement QR code processing to find customer
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('QR Code scanned: $qrData')),
    );
  }

  void _readNFCCustomer() {
    // TODO: Implement NFC reading for customer identification
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('NFC functionality not implemented yet')),
    );
  }

  void _authenticateCustomerBiometric() async {
    try {
      final isAvailable = await _localAuth.isDeviceSupported();
      if (!isAvailable) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Biometric authentication not available')),
        );
        return;
      }

      final isAuthenticated = await _localAuth.authenticate(
        localizedReason: 'Authenticate to identify customer',
        options: const AuthenticationOptions(
          biometricOnly: false,
          stickyAuth: true,
        ),
      );

      if (isAuthenticated) {
        // TODO: Match biometric data with customer database
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Biometric authentication successful')),
        );
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Authentication error: $e')),
      );
    }
  }

  void _showNewCustomerDialog() {
    final nameController = TextEditingController();
    final phoneController = TextEditingController();

    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('New Customer'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(
              controller: nameController,
              decoration: const InputDecoration(
                labelText: 'Customer Name',
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 16),
            TextField(
              controller: phoneController,
              keyboardType: TextInputType.phone,
              decoration: const InputDecoration(
                labelText: 'Phone Number (Optional)',
                border: OutlineInputBorder(),
              ),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () {
              if (nameController.text.trim().isNotEmpty) {
                final newCustomer = CustomerEntity(
                  id: DateTime.now().millisecondsSinceEpoch.toString(),
                  name: nameController.text.trim(),
                  phone: phoneController.text.trim(),
                  loyaltyTier: LoyaltyTier.bronze,
                  loyaltyPoints: 0,
                  createdAt: DateTime.now().toIso8601String(),
                  updatedAt: DateTime.now().toIso8601String(),
                );
                
                Navigator.of(context).pop();
                widget.onCustomerSelected(newCustomer);
              }
            },
            child: const Text('Create'),
          ),
        ],
      ),
    );
  }

  Color _getLoyaltyTierColor(LoyaltyTier tier) {
    switch (tier) {
      case LoyaltyTier.bronze:
        return Colors.brown;
      case LoyaltyTier.silver:
        return Colors.grey;
      case LoyaltyTier.gold:
        return Colors.amber;
      case LoyaltyTier.platinum:
        return Colors.purple;
      case LoyaltyTier.vip:
        return Colors.blue;
      case LoyaltyTier.none:
        return Colors.grey;
    }
  }

  String _getLoyaltyTierName(LoyaltyTier tier) {
    switch (tier) {
      case LoyaltyTier.bronze:
        return 'Bronze';
      case LoyaltyTier.silver:
        return 'Silver';
      case LoyaltyTier.gold:
        return 'Gold';
      case LoyaltyTier.platinum:
        return 'Platinum';
      case LoyaltyTier.vip:
        return 'VIP';
      case LoyaltyTier.none:
        return 'Member';
    }
  }
}
