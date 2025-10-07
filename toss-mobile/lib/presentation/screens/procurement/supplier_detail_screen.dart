import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../providers/supplier/supplier_provider.dart';
import '../../../domain/entities/supplier_entity.dart';
import 'supplier_form_screen.dart';

class SupplierDetailScreen extends StatelessWidget {
  final SupplierEntity supplier;

  const SupplierDetailScreen({super.key, required this.supplier});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(supplier.name),
        actions: [
          IconButton(
            icon: const Icon(Icons.edit),
            onPressed: () {
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => SupplierFormScreen(supplier: supplier),
                ),
              );
            },
          ),
        ],
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header Card
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  children: [
                    Row(
                      children: [
                        CircleAvatar(
                          radius: 30,
                          backgroundColor: supplier.isActive ? Colors.green : Colors.grey,
                          child: Text(
                            supplier.name.isNotEmpty ? supplier.name[0].toUpperCase() : '?',
                            style: const TextStyle(
                              color: Colors.white,
                              fontSize: 24,
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
                                supplier.name,
                                style: const TextStyle(
                                  fontSize: 24,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              if (supplier.companyName != null)
                                Text(
                                  supplier.companyName!,
                                  style: const TextStyle(
                                    fontSize: 16,
                                    color: Colors.grey,
                                  ),
                                ),
                              const SizedBox(height: 8),
                              Row(
                                children: [
                                  Container(
                                    padding: const EdgeInsets.symmetric(
                                      horizontal: 8,
                                      vertical: 4,
                                    ),
                                    decoration: BoxDecoration(
                                      color: supplier.isActive ? Colors.green : Colors.red,
                                      borderRadius: BorderRadius.circular(12),
                                    ),
                                    child: Text(
                                      supplier.isActive ? 'Active' : 'Inactive',
                                      style: const TextStyle(
                                        color: Colors.white,
                                        fontSize: 12,
                                        fontWeight: FontWeight.bold,
                                      ),
                                    ),
                                  ),
                                  if (supplier.rating != null) ...[
                                    const SizedBox(width: 8),
                                    Row(
                                      children: [
                                        const Icon(Icons.star, size: 16, color: Colors.orange),
                                        const SizedBox(width: 4),
                                        Text(
                                          supplier.rating!.toStringAsFixed(1),
                                          style: const TextStyle(
                                            fontWeight: FontWeight.bold,
                                          ),
                                        ),
                                      ],
                                    ),
                                  ],
                                ],
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),

            const SizedBox(height: 16),

            // Contact Information
            _buildSectionCard(
              'Contact Information',
              [
                if (supplier.contactPerson != null)
                  _buildInfoRow('Contact Person', supplier.contactPerson!),
                if (supplier.phoneNumber != null)
                  _buildInfoRow('Phone', supplier.phoneNumber!),
                if (supplier.email != null)
                  _buildInfoRow('Email', supplier.email!),
              ],
            ),

            const SizedBox(height: 16),

            // Address Information
            if (supplier.address != null || supplier.city != null || supplier.country != null)
              _buildSectionCard(
                'Address Information',
                [
                  if (supplier.address != null)
                    _buildInfoRow('Address', supplier.address!),
                  if (supplier.city != null)
                    _buildInfoRow('City', supplier.city!),
                  if (supplier.country != null)
                    _buildInfoRow('Country', supplier.country!),
                ],
              ),

            if (supplier.address != null || supplier.city != null || supplier.country != null)
              const SizedBox(height: 16),

            // Business Information
            _buildSectionCard(
              'Business Information',
              [
                if (supplier.taxNumber != null)
                  _buildInfoRow('Tax Number', supplier.taxNumber!),
                if (supplier.paymentTerms != null)
                  _buildInfoRow(
                    'Payment Terms',
                    '${supplier.paymentTerms!['days']} days ${supplier.paymentTerms!['method']}',
                  ),
              ],
            ),

            const SizedBox(height: 16),

            // Additional Information
            if (supplier.notes != null)
              _buildSectionCard(
                'Additional Information',
                [
                  _buildInfoRow('Notes', supplier.notes!),
                ],
              ),

            if (supplier.notes != null)
              const SizedBox(height: 16),

            // Metadata
            _buildSectionCard(
              'Metadata',
              [
                if (supplier.createdAt != null)
                  _buildInfoRow(
                    'Created',
                    _formatDate(supplier.createdAt!),
                  ),
                if (supplier.updatedAt != null)
                  _buildInfoRow(
                    'Last Updated',
                    _formatDate(supplier.updatedAt!),
                  ),
              ],
            ),

            const SizedBox(height: 32),

            // Action Buttons
            Row(
              children: [
                Expanded(
                  child: ElevatedButton.icon(
                    onPressed: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => SupplierFormScreen(supplier: supplier),
                        ),
                      );
                    },
                    icon: const Icon(Icons.edit),
                    label: const Text('Edit Supplier'),
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: OutlinedButton.icon(
                    onPressed: () {
                      _showDeleteDialog(context);
                    },
                    icon: const Icon(Icons.delete),
                    label: const Text('Delete'),
                    style: OutlinedButton.styleFrom(
                      foregroundColor: Colors.red,
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSectionCard(String title, List<Widget> children) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
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
            const SizedBox(height: 16),
            ...children,
          ],
        ),
      ),
    );
  }

  Widget _buildInfoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8.0),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 120,
            child: Text(
              label,
              style: const TextStyle(
                fontWeight: FontWeight.w500,
                color: Colors.grey,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(
                fontWeight: FontWeight.w500,
              ),
            ),
          ),
        ],
      ),
    );
  }

  String _formatDate(String dateString) {
    try {
      final date = DateTime.parse(dateString);
      return '${date.day}/${date.month}/${date.year}';
    } catch (e) {
      return dateString;
    }
  }

  void _showDeleteDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Delete Supplier'),
        content: const Text('Are you sure you want to delete this supplier? This action cannot be undone.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              Navigator.pop(context);
              _deleteSupplier(context);
            },
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: const Text('Delete'),
          ),
        ],
      ),
    );
  }

  void _deleteSupplier(BuildContext context) {
    final provider = context.read<SupplierProvider>();
    provider.deleteSupplier(supplier.id!).then((success) {
      if (success) {
        Navigator.pop(context);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Supplier deleted successfully'),
          ),
        );
      }
    });
  }
}
