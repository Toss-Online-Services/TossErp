import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../providers/supplier/supplier_provider.dart';
import '../../../domain/entities/supplier_entity.dart';

class SupplierFormScreen extends StatefulWidget {
  final SupplierEntity? supplier;

  const SupplierFormScreen({super.key, this.supplier});

  @override
  State<SupplierFormScreen> createState() => _SupplierFormScreenState();
}

class _SupplierFormScreenState extends State<SupplierFormScreen> {
  final _formKey = GlobalKey<FormState>();
  final _nameController = TextEditingController();
  final _companyNameController = TextEditingController();
  final _contactPersonController = TextEditingController();
  final _phoneController = TextEditingController();
  final _emailController = TextEditingController();
  final _addressController = TextEditingController();
  final _cityController = TextEditingController();
  final _countryController = TextEditingController();
  final _taxNumberController = TextEditingController();
  final _notesController = TextEditingController();
  final _ratingController = TextEditingController();

  bool _isActive = true;
  int _paymentDays = 30;
  String _paymentMethod = 'net';

  @override
  void initState() {
    super.initState();
    if (widget.supplier != null) {
      _populateForm();
    }
  }

  void _populateForm() {
    final supplier = widget.supplier!;
    _nameController.text = supplier.name;
    _companyNameController.text = supplier.companyName ?? '';
    _contactPersonController.text = supplier.contactPerson ?? '';
    _phoneController.text = supplier.phoneNumber ?? '';
    _emailController.text = supplier.email ?? '';
    _addressController.text = supplier.address ?? '';
    _cityController.text = supplier.city ?? '';
    _countryController.text = supplier.country ?? '';
    _taxNumberController.text = supplier.taxNumber ?? '';
    _notesController.text = supplier.notes ?? '';
    _ratingController.text = supplier.rating?.toString() ?? '';
    _isActive = supplier.isActive;
    
    if (supplier.paymentTerms != null) {
      _paymentDays = supplier.paymentTerms!['days'] ?? 30;
      _paymentMethod = supplier.paymentTerms!['method'] ?? 'net';
    }
  }

  @override
  void dispose() {
    _nameController.dispose();
    _companyNameController.dispose();
    _contactPersonController.dispose();
    _phoneController.dispose();
    _emailController.dispose();
    _addressController.dispose();
    _cityController.dispose();
    _countryController.dispose();
    _taxNumberController.dispose();
    _notesController.dispose();
    _ratingController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final isEditing = widget.supplier != null;
    
    return Scaffold(
      appBar: AppBar(
        title: Text(isEditing ? 'Edit Supplier' : 'Add Supplier'),
        actions: [
          if (isEditing)
            IconButton(
              icon: const Icon(Icons.delete),
              onPressed: () => _showDeleteDialog(context),
            ),
        ],
      ),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16.0),
          children: [
            // Basic Information
            _buildSectionHeader('Basic Information'),
            TextFormField(
              controller: _nameController,
              decoration: const InputDecoration(
                labelText: 'Supplier Name *',
                border: OutlineInputBorder(),
              ),
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return 'Please enter supplier name';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _companyNameController,
              decoration: const InputDecoration(
                labelText: 'Company Name',
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _contactPersonController,
              decoration: const InputDecoration(
                labelText: 'Contact Person',
                border: OutlineInputBorder(),
              ),
            ),

            const SizedBox(height: 24),

            // Contact Information
            _buildSectionHeader('Contact Information'),
            TextFormField(
              controller: _phoneController,
              decoration: const InputDecoration(
                labelText: 'Phone Number',
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.phone,
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _emailController,
              decoration: const InputDecoration(
                labelText: 'Email',
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.emailAddress,
              validator: (value) {
                if (value != null && value.isNotEmpty) {
                  if (!RegExp(r'^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$').hasMatch(value)) {
                    return 'Please enter a valid email';
                  }
                }
                return null;
              },
            ),

            const SizedBox(height: 24),

            // Address Information
            _buildSectionHeader('Address Information'),
            TextFormField(
              controller: _addressController,
              decoration: const InputDecoration(
                labelText: 'Address',
                border: OutlineInputBorder(),
              ),
              maxLines: 2,
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: TextFormField(
                    controller: _cityController,
                    decoration: const InputDecoration(
                      labelText: 'City',
                      border: OutlineInputBorder(),
                    ),
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: TextFormField(
                    controller: _countryController,
                    decoration: const InputDecoration(
                      labelText: 'Country',
                      border: OutlineInputBorder(),
                    ),
                  ),
                ),
              ],
            ),

            const SizedBox(height: 24),

            // Business Information
            _buildSectionHeader('Business Information'),
            TextFormField(
              controller: _taxNumberController,
              decoration: const InputDecoration(
                labelText: 'Tax Number',
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: TextFormField(
                    decoration: const InputDecoration(
                      labelText: 'Payment Days',
                      border: OutlineInputBorder(),
                    ),
                    keyboardType: TextInputType.number,
                    initialValue: _paymentDays.toString(),
                    onChanged: (value) {
                      _paymentDays = int.tryParse(value) ?? 30;
                    },
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: DropdownButtonFormField<String>(
                    decoration: const InputDecoration(
                      labelText: 'Payment Method',
                      border: OutlineInputBorder(),
                    ),
                    value: _paymentMethod,
                    items: const [
                      DropdownMenuItem(value: 'net', child: Text('Net')),
                      DropdownMenuItem(value: 'cash', child: Text('Cash')),
                      DropdownMenuItem(value: 'credit', child: Text('Credit')),
                    ],
                    onChanged: (value) {
                      setState(() {
                        _paymentMethod = value ?? 'net';
                      });
                    },
                  ),
                ),
              ],
            ),

            const SizedBox(height: 24),

            // Additional Information
            _buildSectionHeader('Additional Information'),
            TextFormField(
              controller: _ratingController,
              decoration: const InputDecoration(
                labelText: 'Rating (1-5)',
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.number,
              validator: (value) {
                if (value != null && value.isNotEmpty) {
                  final rating = double.tryParse(value);
                  if (rating == null || rating < 1 || rating > 5) {
                    return 'Rating must be between 1 and 5';
                  }
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _notesController,
              decoration: const InputDecoration(
                labelText: 'Notes',
                border: OutlineInputBorder(),
              ),
              maxLines: 3,
            ),
            const SizedBox(height: 16),
            SwitchListTile(
              title: const Text('Active'),
              subtitle: const Text('Supplier is active and can receive orders'),
              value: _isActive,
              onChanged: (value) {
                setState(() {
                  _isActive = value;
                });
              },
            ),

            const SizedBox(height: 32),

            // Save Button
            ElevatedButton(
              onPressed: _saveSupplier,
              style: ElevatedButton.styleFrom(
                padding: const EdgeInsets.symmetric(vertical: 16),
              ),
              child: Text(
                isEditing ? 'Update Supplier' : 'Create Supplier',
                style: const TextStyle(fontSize: 16),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSectionHeader(String title) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 16.0),
      child: Text(
        title,
        style: Theme.of(context).textTheme.titleMedium?.copyWith(
          fontWeight: FontWeight.bold,
          color: Theme.of(context).primaryColor,
        ),
      ),
    );
  }

  void _saveSupplier() {
    if (_formKey.currentState!.validate()) {
      final provider = context.read<SupplierProvider>();
      
      final supplier = widget.supplier?.copyWith(
        name: _nameController.text,
        companyName: _companyNameController.text.isEmpty ? null : _companyNameController.text,
        contactPerson: _contactPersonController.text.isEmpty ? null : _contactPersonController.text,
        phoneNumber: _phoneController.text.isEmpty ? null : _phoneController.text,
        email: _emailController.text.isEmpty ? null : _emailController.text,
        address: _addressController.text.isEmpty ? null : _addressController.text,
        city: _cityController.text.isEmpty ? null : _cityController.text,
        country: _countryController.text.isEmpty ? null : _countryController.text,
        taxNumber: _taxNumberController.text.isEmpty ? null : _taxNumberController.text,
        paymentTerms: {
          'days': _paymentDays,
          'method': _paymentMethod,
        },
        isActive: _isActive,
        rating: _ratingController.text.isEmpty ? null : double.tryParse(_ratingController.text),
        notes: _notesController.text.isEmpty ? null : _notesController.text,
        updatedAt: DateTime.now().toIso8601String(),
      ) ?? provider.createNewSupplier(
        name: _nameController.text,
        companyName: _companyNameController.text.isEmpty ? null : _companyNameController.text,
        contactPerson: _contactPersonController.text.isEmpty ? null : _contactPersonController.text,
        phoneNumber: _phoneController.text.isEmpty ? null : _phoneController.text,
        email: _emailController.text.isEmpty ? null : _emailController.text,
        address: _addressController.text.isEmpty ? null : _addressController.text,
        city: _cityController.text.isEmpty ? null : _cityController.text,
        country: _countryController.text.isEmpty ? null : _countryController.text,
        taxNumber: _taxNumberController.text.isEmpty ? null : _taxNumberController.text,
        paymentTerms: {
          'days': _paymentDays,
          'method': _paymentMethod,
        },
        isActive: _isActive,
        rating: _ratingController.text.isEmpty ? null : double.tryParse(_ratingController.text),
        notes: _notesController.text.isEmpty ? null : _notesController.text,
      );

      final isEditing = widget.supplier != null;
      final future = isEditing 
        ? provider.updateSupplier(supplier)
        : provider.createSupplier(supplier);

      future.then((success) {
        if (success) {
          Navigator.pop(context);
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text(
                isEditing ? 'Supplier updated successfully' : 'Supplier created successfully',
              ),
            ),
          );
        }
      });
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
              _deleteSupplier();
            },
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: const Text('Delete'),
          ),
        ],
      ),
    );
  }

  void _deleteSupplier() {
    final provider = context.read<SupplierProvider>();
    provider.deleteSupplier(widget.supplier!.id!).then((success) {
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
