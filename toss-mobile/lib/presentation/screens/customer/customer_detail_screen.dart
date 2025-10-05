import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../domain/entities/customer_entity.dart';
import '../../providers/customer/customer_provider.dart';
import 'customer_form_screen.dart';

class CustomerDetailScreen extends StatelessWidget {
  final CustomerEntity customer;

  const CustomerDetailScreen({super.key, required this.customer});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(customer.name ?? 'Customer Details'),
        actions: [
          IconButton(
            icon: const Icon(Icons.edit),
            onPressed: () {
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => CustomerFormScreen(customer: customer),
                ),
              );
            },
          ),
          PopupMenuButton<String>(
            onSelected: (value) {
              switch (value) {
                case 'delete':
                  _showDeleteDialog(context);
                  break;
                case 'add_points':
                  _showAddPointsDialog(context);
                  break;
                case 'add_loyalty':
                  _showAddLoyaltyDialog(context);
                  break;
              }
            },
            itemBuilder: (context) => [
              const PopupMenuItem(
                value: 'add_points',
                child: ListTile(
                  leading: Icon(Icons.stars),
                  title: Text('Add Points'),
                  contentPadding: EdgeInsets.zero,
                ),
              ),
              const PopupMenuItem(
                value: 'add_loyalty',
                child: ListTile(
                  leading: Icon(Icons.card_giftcard),
                  title: Text('Add Loyalty Points'),
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
        ],
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildHeaderCard(),
            const SizedBox(height: 16),
            _buildStatsCards(),
            const SizedBox(height: 16),
            _buildPersonalInfoCard(),
            const SizedBox(height: 16),
            _buildContactInfoCard(),
            const SizedBox(height: 16),
            _buildTierInfoCard(),
            const SizedBox(height: 16),
            _buildNotesCard(),
          ],
        ),
      ),
    );
  }

  Widget _buildHeaderCard() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Row(
          children: [
            CircleAvatar(
              radius: 40,
              backgroundColor: _getTierColor(customer.tier),
              child: Text(
                customer.name?.substring(0, 1).toUpperCase() ?? '?',
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 32,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    customer.name ?? 'Unknown',
                    style: const TextStyle(
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  if (customer.phone != null)
                    Text(
                      customer.phone!,
                      style: TextStyle(
                        fontSize: 16,
                        color: Colors.grey[600],
                      ),
                    ),
                  if (customer.membershipNumber != null)
                    Text(
                      'Member #${customer.membershipNumber}',
                      style: TextStyle(
                        fontSize: 14,
                        color: Colors.grey[500],
                      ),
                    ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildStatsCards() {
    return Row(
      children: [
        Expanded(
          child: _buildStatCard(
            'Visits',
            customer.visitCount.toString(),
            Icons.shopping_cart,
            Colors.blue,
          ),
        ),
        const SizedBox(width: 8),
        Expanded(
          child: _buildStatCard(
            'Total Spent',
            '\$${customer.totalSpent.toStringAsFixed(2)}',
            Icons.attach_money,
            Colors.green,
          ),
        ),
        const SizedBox(width: 8),
        Expanded(
          child: _buildStatCard(
            'Points',
            customer.pointsBalance.toString(),
            Icons.stars,
            Colors.orange,
          ),
        ),
        const SizedBox(width: 8),
        Expanded(
          child: _buildStatCard(
            'Loyalty',
            customer.loyaltyPoints.toString(),
            Icons.card_giftcard,
            Colors.purple,
          ),
        ),
      ],
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
              style: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
                color: Colors.black87,
              ),
            ),
            Text(
              title,
              style: TextStyle(
                fontSize: 12,
                color: Colors.grey[600],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPersonalInfoCard() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Personal Information',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            _buildInfoRow('Gender', customer.gender ?? 'Not specified'),
            _buildInfoRow('Birth Date', customer.dateOfBirth != null
                ? '${customer.dateOfBirth!.day}/${customer.dateOfBirth!.month}/${customer.dateOfBirth!.year}'
                : 'Not specified'),
            _buildInfoRow('Anniversary', customer.anniversaryDate != null
                ? '${customer.anniversaryDate!.day}/${customer.anniversaryDate!.month}/${customer.anniversaryDate!.year}'
                : 'Not specified'),
            _buildInfoRow('Last Visit', customer.lastVisit != null
                ? '${customer.lastVisit!.day}/${customer.lastVisit!.month}/${customer.lastVisit!.year}'
                : 'Never'),
            _buildInfoRow('Average per Visit', '\$${customer.averageSpentPerVisit.toStringAsFixed(2)}'),
            _buildInfoRow('Status', customer.isActive ? 'Active' : 'Inactive'),
            if (customer.isInactive)
              Container(
                margin: const EdgeInsets.only(top: 8),
                padding: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  color: Colors.orange[100],
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  children: [
                    Icon(Icons.warning, color: Colors.orange[700], size: 16),
                    const SizedBox(width: 8),
                    Text(
                      'Customer is inactive (${customer.daysSinceLastVisit} days since last visit)',
                      style: TextStyle(
                        color: Colors.orange[700],
                        fontSize: 12,
                      ),
                    ),
                  ],
                ),
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildContactInfoCard() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Contact Information',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            _buildInfoRow('Phone', customer.phone ?? 'Not specified'),
            _buildInfoRow('Alternate Phone', customer.alternatePhone ?? 'Not specified'),
            _buildInfoRow('Address', customer.address ?? 'Not specified'),
            _buildInfoRow('City', customer.city ?? 'Not specified'),
            _buildInfoRow('Country', customer.country ?? 'Not specified'),
            _buildInfoRow('Preferred Communication', customer.preferredCommunication.name.toUpperCase()),
          ],
        ),
      ),
    );
  }

  Widget _buildTierInfoCard() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Tier Information',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: _buildTierChip(
                    'Customer Tier',
                    customer.tier.name.toUpperCase(),
                    _getTierColor(customer.tier),
                  ),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: _buildTierChip(
                    'Loyalty Tier',
                    customer.loyaltyTier.name.toUpperCase(),
                    _getLoyaltyTierColor(customer.loyaltyTier),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            _buildInfoRow('Primary ID Type', customer.primaryIdType.name.toUpperCase()),
            if (customer.qrCode != null)
              _buildInfoRow('QR Code', customer.qrCode!),
            if (customer.nfcId != null)
              _buildInfoRow('NFC ID', customer.nfcId!),
            if (customer.biometricId != null)
              _buildInfoRow('Biometric ID', customer.biometricId!),
          ],
        ),
      ),
    );
  }

  Widget _buildTierChip(String label, String value, Color color) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(8),
        border: Border.all(color: color.withOpacity(0.3)),
      ),
      child: Column(
        children: [
          Text(
            label,
            style: TextStyle(
              fontSize: 12,
              color: Colors.grey[600],
            ),
          ),
          const SizedBox(height: 4),
          Text(
            value,
            style: TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.bold,
              color: color,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildNotesCard() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Notes',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            Text(
              customer.notes ?? 'No notes available',
              style: TextStyle(
                fontSize: 14,
                color: Colors.grey[700],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildInfoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 120,
            child: Text(
              '$label:',
              style: TextStyle(
                fontWeight: FontWeight.w500,
                color: Colors.grey[700],
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(fontWeight: FontWeight.w400),
            ),
          ),
        ],
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

  void _showDeleteDialog(BuildContext context) {
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
              final provider = context.read<CustomerProvider>();
              final success = await provider.deleteCustomer(customer.id);
              if (success && context.mounted) {
                Navigator.pop(context);
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

  void _showAddPointsDialog(BuildContext context) {
    final pointsController = TextEditingController();
    
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Add Points'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text('Current points: ${customer.pointsBalance}'),
            const SizedBox(height: 16),
            TextField(
              controller: pointsController,
              decoration: const InputDecoration(
                labelText: 'Points to add',
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.number,
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () async {
              final points = int.tryParse(pointsController.text);
              if (points != null && points > 0) {
                Navigator.pop(context);
                final provider = context.read<CustomerProvider>();
                final success = await provider.updateCustomerPoints(
                  customer.id,
                  customer.pointsBalance + points,
                );
                if (success && context.mounted) {
                  ScaffoldMessenger.of(context).showSnackBar(
                    SnackBar(
                      content: Text('Added $points points to ${customer.name}'),
                      backgroundColor: Colors.green,
                    ),
                  );
                }
              }
            },
            child: const Text('Add'),
          ),
        ],
      ),
    );
  }

  void _showAddLoyaltyDialog(BuildContext context) {
    final loyaltyController = TextEditingController();
    
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Add Loyalty Points'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text('Current loyalty points: ${customer.loyaltyPoints}'),
            const SizedBox(height: 16),
            TextField(
              controller: loyaltyController,
              decoration: const InputDecoration(
                labelText: 'Loyalty points to add',
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.number,
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () async {
              final points = int.tryParse(loyaltyController.text);
              if (points != null && points > 0) {
                Navigator.pop(context);
                final provider = context.read<CustomerProvider>();
                final success = await provider.updateCustomerLoyaltyPoints(
                  customer.id,
                  customer.loyaltyPoints + points,
                );
                if (success && context.mounted) {
                  ScaffoldMessenger.of(context).showSnackBar(
                    SnackBar(
                      content: Text('Added $points loyalty points to ${customer.name}'),
                      backgroundColor: Colors.green,
                    ),
                  );
                }
              }
            },
            child: const Text('Add'),
          ),
        ],
      ),
    );
  }
}
