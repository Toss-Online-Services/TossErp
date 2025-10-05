import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../providers/inventory/inventory_provider.dart';

class InventoryTransferScreen extends StatefulWidget {
  const InventoryTransferScreen({super.key});

  @override
  State<InventoryTransferScreen> createState() => _InventoryTransferScreenState();
}

class _InventoryTransferScreenState extends State<InventoryTransferScreen> {
  final _formKey = GlobalKey<FormState>();
  final _productIdController = TextEditingController();
  final _quantityController = TextEditingController();
  final _notesController = TextEditingController();

  String? _selectedFromLocation;
  String? _selectedToLocation;

  final List<Map<String, String>> _locations = [
    {'id': 'LOC001', 'name': 'Main Store'},
    {'id': 'LOC002', 'name': 'Warehouse'},
    {'id': 'LOC003', 'name': 'Back Room'},
    {'id': 'LOC004', 'name': 'Display Area'},
  ];

  @override
  void dispose() {
    _productIdController.dispose();
    _quantityController.dispose();
    _notesController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Inventory Transfer'),
        actions: [
          IconButton(
            icon: const Icon(Icons.history),
            onPressed: () {
              // TODO: Navigate to transfer history
            },
          ),
        ],
      ),
      body: Consumer<InventoryProvider>(
        builder: (context, provider, child) {
          return Form(
            key: _formKey,
            child: ListView(
              padding: const EdgeInsets.all(16),
              children: [
                _buildProductSection(),
                const SizedBox(height: 24),
                _buildLocationSection(),
                const SizedBox(height: 24),
                _buildQuantitySection(),
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

  Widget _buildProductSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Product Information',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _productIdController,
              decoration: const InputDecoration(
                labelText: 'Product ID *',
                border: OutlineInputBorder(),
                hintText: 'Enter product ID or scan barcode',
              ),
              validator: (value) {
                if (value == null || value.trim().isEmpty) {
                  return 'Product ID is required';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: ElevatedButton.icon(
                    onPressed: () {
                      // TODO: Implement barcode scanner
                      ScaffoldMessenger.of(context).showSnackBar(
                        const SnackBar(
                          content: Text('Barcode scanner not implemented yet'),
                        ),
                      );
                    },
                    icon: const Icon(Icons.qr_code_scanner),
                    label: const Text('Scan Barcode'),
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: OutlinedButton.icon(
                    onPressed: () {
                      // TODO: Show product picker
                      ScaffoldMessenger.of(context).showSnackBar(
                        const SnackBar(
                          content: Text('Product picker not implemented yet'),
                        ),
                      );
                    },
                    icon: const Icon(Icons.search),
                    label: const Text('Browse Products'),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildLocationSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Transfer Locations',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            DropdownButtonFormField<String>(
              value: _selectedFromLocation,
              decoration: const InputDecoration(
                labelText: 'From Location *',
                border: OutlineInputBorder(),
              ),
              items: _locations.map((location) {
                return DropdownMenuItem(
                  value: location['id'],
                  child: Text(location['name']!),
                );
              }).toList(),
              onChanged: (value) {
                setState(() {
                  _selectedFromLocation = value;
                  // Reset to location if it's the same as from location
                  if (_selectedToLocation == value) {
                    _selectedToLocation = null;
                  }
                });
              },
              validator: (value) {
                if (value == null) {
                  return 'Please select a from location';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
            DropdownButtonFormField<String>(
              value: _selectedToLocation,
              decoration: const InputDecoration(
                labelText: 'To Location *',
                border: OutlineInputBorder(),
              ),
              items: _locations
                  .where((location) => location['id'] != _selectedFromLocation)
                  .map((location) {
                return DropdownMenuItem(
                  value: location['id'],
                  child: Text(location['name']!),
                );
              }).toList(),
              onChanged: (value) {
                setState(() {
                  _selectedToLocation = value;
                });
              },
              validator: (value) {
                if (value == null) {
                  return 'Please select a to location';
                }
                if (value == _selectedFromLocation) {
                  return 'To location must be different from from location';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
            if (_selectedFromLocation != null && _selectedToLocation != null)
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.blue[50],
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: Colors.blue[200]!),
                ),
                child: Row(
                  children: [
                    Icon(Icons.swap_horiz, color: Colors.blue[700], size: 20),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        'Transferring from ${_getLocationName(_selectedFromLocation!)} to ${_getLocationName(_selectedToLocation!)}',
                        style: TextStyle(
                          color: Colors.blue[700],
                          fontSize: 12,
                        ),
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

  Widget _buildQuantitySection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Transfer Quantity',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _quantityController,
              decoration: const InputDecoration(
                labelText: 'Quantity *',
                border: OutlineInputBorder(),
                hintText: 'Enter quantity to transfer',
              ),
              keyboardType: TextInputType.numberWithOptions(decimal: true),
              validator: (value) {
                if (value == null || value.trim().isEmpty) {
                  return 'Quantity is required';
                }
                final quantity = double.tryParse(value);
                if (quantity == null) {
                  return 'Please enter a valid number';
                }
                if (quantity <= 0) {
                  return 'Quantity must be greater than zero';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: Colors.orange[50],
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: Colors.orange[200]!),
              ),
              child: Row(
                children: [
                  Icon(Icons.warning, color: Colors.orange[700], size: 20),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      'Make sure the source location has sufficient stock before transferring',
                      style: TextStyle(
                        color: Colors.orange[700],
                        fontSize: 12,
                      ),
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

  Widget _buildNotesSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Transfer Notes',
              style: Theme.of(context).textTheme.titleLarge,
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _notesController,
              decoration: const InputDecoration(
                labelText: 'Notes (Optional)',
                border: OutlineInputBorder(),
                hintText: 'Add any additional information about this transfer',
              ),
              maxLines: 3,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildActionButtons(InventoryProvider provider) {
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
            onPressed: provider.isLoading ? null : _createTransfer,
            child: provider.isLoading
                ? const SizedBox(
                    height: 20,
                    width: 20,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Text('Create Transfer'),
          ),
        ),
      ],
    );
  }

  String _getLocationName(String locationId) {
    return _locations.firstWhere(
      (location) => location['id'] == locationId,
      orElse: () => {'name': 'Unknown'},
    )['name']!;
  }

  Future<void> _createTransfer() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    final provider = context.read<InventoryProvider>();
    final quantity = double.parse(_quantityController.text);

    final success = await provider.createInventoryTransfer(
      _productIdController.text.trim(),
      quantity,
      _selectedFromLocation!,
      _selectedToLocation!,
      _notesController.text.trim().isEmpty ? null : _notesController.text.trim(),
    );

    if (success && mounted) {
      Navigator.pop(context);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            'Inventory transfer created successfully for ${_productIdController.text}',
          ),
          backgroundColor: Colors.green,
        ),
      );
    }
  }
}
