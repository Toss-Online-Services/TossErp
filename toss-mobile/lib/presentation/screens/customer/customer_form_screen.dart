import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../domain/entities/customer_entity.dart';
import '../../providers/customer/customer_provider.dart';

class CustomerFormScreen extends StatefulWidget {
  final CustomerEntity? customer;

  const CustomerFormScreen({super.key, this.customer});

  @override
  State<CustomerFormScreen> createState() => _CustomerFormScreenState();
}

class _CustomerFormScreenState extends State<CustomerFormScreen> {
  final _formKey = GlobalKey<FormState>();
  final _nameController = TextEditingController();
  final _phoneController = TextEditingController();
  final _alternatePhoneController = TextEditingController();
  final _membershipNumberController = TextEditingController();
  final _addressController = TextEditingController();
  final _cityController = TextEditingController();
  final _countryController = TextEditingController();
  final _notesController = TextEditingController();

  CustomerTier _selectedTier = CustomerTier.bronze;
  LoyaltyTier _selectedLoyaltyTier = LoyaltyTier.none;
  CustomerIdType _selectedIdType = CustomerIdType.phone;
  PreferredCommunication _selectedCommunication = PreferredCommunication.none;
  String? _selectedGender;
  DateTime? _selectedBirthDate;
  DateTime? _selectedAnniversaryDate;

  bool get isEditing => widget.customer != null;

  @override
  void initState() {
    super.initState();
    if (isEditing) {
      _populateFields();
    }
  }

  void _populateFields() {
    final customer = widget.customer!;
    _nameController.text = customer.name ?? '';
    _phoneController.text = customer.phone ?? '';
    _alternatePhoneController.text = customer.alternatePhone ?? '';
    _membershipNumberController.text = customer.membershipNumber ?? '';
    _addressController.text = customer.address ?? '';
    _cityController.text = customer.city ?? '';
    _countryController.text = customer.country ?? '';
    _notesController.text = customer.notes ?? '';
    
    _selectedTier = customer.tier;
    _selectedLoyaltyTier = customer.loyaltyTier;
    _selectedIdType = customer.primaryIdType;
    _selectedCommunication = customer.preferredCommunication;
    _selectedGender = customer.gender;
    _selectedBirthDate = customer.dateOfBirth;
    _selectedAnniversaryDate = customer.anniversaryDate;
  }

  @override
  void dispose() {
    _nameController.dispose();
    _phoneController.dispose();
    _alternatePhoneController.dispose();
    _membershipNumberController.dispose();
    _addressController.dispose();
    _cityController.dispose();
    _countryController.dispose();
    _notesController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(isEditing ? 'Edit Customer' : 'Add Customer'),
        actions: [
          if (isEditing)
            IconButton(
              icon: const Icon(Icons.delete),
              onPressed: _showDeleteDialog,
            ),
        ],
      ),
      body: Consumer<CustomerProvider>(
        builder: (context, provider, child) {
          return Form(
            key: _formKey,
            child: ListView(
              padding: const EdgeInsets.all(16),
              children: [
                _buildBasicInfoSection(),
                const SizedBox(height: 24),
                _buildTierSection(),
                const SizedBox(height: 24),
                _buildContactInfoSection(),
                const SizedBox(height: 24),
                _buildPersonalInfoSection(),
                const SizedBox(height: 24),
                _buildNotesSection(),
                const SizedBox(height: 32),
                _buildActionButtons(provider),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildBasicInfoSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Basic Information',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _nameController,
              decoration: const InputDecoration(
                labelText: 'Full Name *',
                border: OutlineInputBorder(),
              ),
              validator: (value) {
                if (value == null || value.trim().isEmpty) {
                  return 'Name is required';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _phoneController,
              decoration: const InputDecoration(
                labelText: 'Phone Number *',
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.phone,
              validator: (value) {
                if (value == null || value.trim().isEmpty) {
                  return 'Phone number is required';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _membershipNumberController,
              decoration: const InputDecoration(
                labelText: 'Membership Number',
                border: OutlineInputBorder(),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildTierSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Customer Tiers',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            DropdownButtonFormField<CustomerTier>(
              value: _selectedTier,
              decoration: const InputDecoration(
                labelText: 'Customer Tier',
                border: OutlineInputBorder(),
              ),
              items: CustomerTier.values.map((tier) {
                return DropdownMenuItem(
                  value: tier,
                  child: Text(tier.name.toUpperCase()),
                );
              }).toList(),
              onChanged: (value) {
                setState(() {
                  _selectedTier = value!;
                });
              },
            ),
            const SizedBox(height: 16),
            DropdownButtonFormField<LoyaltyTier>(
              value: _selectedLoyaltyTier,
              decoration: const InputDecoration(
                labelText: 'Loyalty Tier',
                border: OutlineInputBorder(),
              ),
              items: LoyaltyTier.values.map((tier) {
                return DropdownMenuItem(
                  value: tier,
                  child: Text(tier.name.toUpperCase()),
                );
              }).toList(),
              onChanged: (value) {
                setState(() {
                  _selectedLoyaltyTier = value!;
                });
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildContactInfoSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Contact Information',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _alternatePhoneController,
              decoration: const InputDecoration(
                labelText: 'Alternate Phone',
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.phone,
            ),
            const SizedBox(height: 16),
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
            const SizedBox(height: 16),
            DropdownButtonFormField<PreferredCommunication>(
              value: _selectedCommunication,
              decoration: const InputDecoration(
                labelText: 'Preferred Communication',
                border: OutlineInputBorder(),
              ),
              items: PreferredCommunication.values.map((comm) {
                return DropdownMenuItem(
                  value: comm,
                  child: Text(comm.name.toUpperCase()),
                );
              }).toList(),
              onChanged: (value) {
                setState(() {
                  _selectedCommunication = value!;
                });
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPersonalInfoSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Personal Information',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            DropdownButtonFormField<String>(
              value: _selectedGender,
              decoration: const InputDecoration(
                labelText: 'Gender',
                border: OutlineInputBorder(),
              ),
              items: const [
                DropdownMenuItem(value: 'Male', child: Text('Male')),
                DropdownMenuItem(value: 'Female', child: Text('Female')),
                DropdownMenuItem(value: 'Other', child: Text('Other')),
              ],
              onChanged: (value) {
                setState(() {
                  _selectedGender = value;
                });
              },
            ),
            const SizedBox(height: 16),
            ListTile(
              title: Text(
                _selectedBirthDate != null
                    ? 'Birth Date: ${_selectedBirthDate!.day}/${_selectedBirthDate!.month}/${_selectedBirthDate!.year}'
                    : 'Birth Date',
              ),
              trailing: const Icon(Icons.calendar_today),
              onTap: _selectBirthDate,
            ),
            const SizedBox(height: 16),
            ListTile(
              title: Text(
                _selectedAnniversaryDate != null
                    ? 'Anniversary: ${_selectedAnniversaryDate!.day}/${_selectedAnniversaryDate!.month}/${_selectedAnniversaryDate!.year}'
                    : 'Anniversary Date',
              ),
              trailing: const Icon(Icons.calendar_today),
              onTap: _selectAnniversaryDate,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildNotesSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Notes',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _notesController,
              decoration: const InputDecoration(
                labelText: 'Additional Notes',
                border: OutlineInputBorder(),
              ),
              maxLines: 4,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildActionButtons(CustomerProvider provider) {
    return Row(
      children: [
        Expanded(
          child: OutlinedButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
        ),
        const SizedBox(width: 16),
        Expanded(
          child: ElevatedButton(
            onPressed: provider.isLoading ? null : _saveCustomer,
            child: provider.isLoading
                ? const SizedBox(
                    height: 20,
                    width: 20,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : Text(isEditing ? 'Update' : 'Save'),
          ),
        ),
      ],
    );
  }

  Future<void> _selectBirthDate() async {
    final date = await showDatePicker(
      context: context,
      initialDate: _selectedBirthDate ?? DateTime.now(),
      firstDate: DateTime(1900),
      lastDate: DateTime.now(),
    );
    if (date != null) {
      setState(() {
        _selectedBirthDate = date;
      });
    }
  }

  Future<void> _selectAnniversaryDate() async {
    final date = await showDatePicker(
      context: context,
      initialDate: _selectedAnniversaryDate ?? DateTime.now(),
      firstDate: DateTime(1900),
      lastDate: DateTime.now().add(const Duration(days: 365 * 10)),
    );
    if (date != null) {
      setState(() {
        _selectedAnniversaryDate = date;
      });
    }
  }

  Future<void> _saveCustomer() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    final provider = context.read<CustomerProvider>();
    
    final customer = isEditing
        ? widget.customer!.copyWith(
            name: _nameController.text.trim(),
            phone: _phoneController.text.trim(),
            alternatePhone: _alternatePhoneController.text.trim().isEmpty
                ? null
                : _alternatePhoneController.text.trim(),
            membershipNumber: _membershipNumberController.text.trim().isEmpty
                ? null
                : _membershipNumberController.text.trim(),
            tier: _selectedTier,
            loyaltyTier: _selectedLoyaltyTier,
            primaryIdType: _selectedIdType,
            address: _addressController.text.trim().isEmpty
                ? null
                : _addressController.text.trim(),
            city: _cityController.text.trim().isEmpty
                ? null
                : _cityController.text.trim(),
            country: _countryController.text.trim().isEmpty
                ? null
                : _countryController.text.trim(),
            gender: _selectedGender,
            dateOfBirth: _selectedBirthDate,
            anniversaryDate: _selectedAnniversaryDate,
            preferredCommunication: _selectedCommunication,
            notes: _notesController.text.trim().isEmpty
                ? null
                : _notesController.text.trim(),
            updatedAt: DateTime.now().toIso8601String(),
          )
        : CustomerEntity(
            id: DateTime.now().millisecondsSinceEpoch.toString(),
            name: _nameController.text.trim(),
            phone: _phoneController.text.trim(),
            alternatePhone: _alternatePhoneController.text.trim().isEmpty
                ? null
                : _alternatePhoneController.text.trim(),
            membershipNumber: _membershipNumberController.text.trim().isEmpty
                ? null
                : _membershipNumberController.text.trim(),
            primaryIdType: _selectedIdType,
            tier: _selectedTier,
            loyaltyTier: _selectedLoyaltyTier,
            pointsBalance: 0,
            loyaltyPoints: 0,
            totalSpent: 0.0,
            visitCount: 0,
            address: _addressController.text.trim().isEmpty
                ? null
                : _addressController.text.trim(),
            city: _cityController.text.trim().isEmpty
                ? null
                : _cityController.text.trim(),
            country: _countryController.text.trim().isEmpty
                ? null
                : _countryController.text.trim(),
            gender: _selectedGender,
            dateOfBirth: _selectedBirthDate,
            anniversaryDate: _selectedAnniversaryDate,
            preferredCommunication: _selectedCommunication,
            notes: _notesController.text.trim().isEmpty
                ? null
                : _notesController.text.trim(),
            isActive: true,
            createdAt: DateTime.now().toIso8601String(),
            updatedAt: DateTime.now().toIso8601String(),
          );

    final success = isEditing
        ? await provider.updateCustomer(customer)
        : await provider.createCustomer(customer);

    if (success && mounted) {
      Navigator.pop(context);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            isEditing
                ? 'Customer updated successfully'
                : 'Customer created successfully',
          ),
          backgroundColor: Colors.green,
        ),
      );
    }
  }

  void _showDeleteDialog() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Delete Customer'),
        content: Text('Are you sure you want to delete ${widget.customer?.name}?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () async {
              Navigator.pop(context);
              final provider = context.read<CustomerProvider>();
              final success = await provider.deleteCustomer(widget.customer!.id);
              if (success && mounted) {
                Navigator.pop(context);
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(
                    content: Text('${widget.customer!.name} deleted successfully'),
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
