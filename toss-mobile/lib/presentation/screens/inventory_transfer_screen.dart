import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../domain/entities/location_entity.dart';
import '../../domain/entities/product_entity.dart';
import '../../data/services/location_service.dart';

class InventoryTransferScreen extends StatefulWidget {
  final String? fromLocationId;
  final String? toLocationId;

  const InventoryTransferScreen({
    Key? key,
    this.fromLocationId,
    this.toLocationId,
  }) : super(key: key);

  @override
  State<InventoryTransferScreen> createState() => _InventoryTransferScreenState();
}

class _InventoryTransferScreenState extends State<InventoryTransferScreen> {
  final LocationService _locationService = LocationService();
  final _formKey = GlobalKey<FormState>();
  
  LocationEntity? _fromLocation;
  LocationEntity? _toLocation;
  TransferType _transferType = TransferType.stock;
  final List<TransferItemEntity> _transferItems = [];
  final TextEditingController _notesController = TextEditingController();
  
  bool _isLoading = true;
  List<LocationEntity> _availableLocations = [];
  List<ProductEntity> _availableProducts = [];

  @override
  void initState() {
    super.initState();
    _initializeData();
  }

  Future<void> _initializeData() async {
    try {
      await _locationService.initialize();
      _availableLocations = _locationService.allLocations;
      
      // Set initial locations if provided
      if (widget.fromLocationId != null) {
        _fromLocation = await _locationService.getLocationById(widget.fromLocationId!);
      }
      if (widget.toLocationId != null) {
        _toLocation = await _locationService.getLocationById(widget.toLocationId!);
      }
      
      // Load mock product data
      _availableProducts = _getMockProducts();
      
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error loading data: $e')),
        );
      }
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  List<ProductEntity> _getMockProducts() {
    // Mock product data - in real app, load from inventory service
    return [
      ProductEntity(
        id: '1',
        name: 'Coca Cola 350ml',
        description: 'Coca Cola bottle 350ml',
        sku: 'CC350',
        category: 'Beverages',
        subcategory: 'Soft Drinks',
        unitOfMeasure: 'bottle',
        sellingPrice: 3.50,
        costPrice: 2.00,
        stockQuantity: 120,
        reorderLevel: 20,
        maxStockLevel: 500,
        isActive: true,
        taxable: true,
        createdAt: DateTime.now(),
      ),
      ProductEntity(
        id: '2',
        name: 'Rice - 5kg bag',
        description: 'Premium jasmine rice 5kg',
        sku: 'RICE5KG',
        category: 'Food',
        subcategory: 'Grains',
        unitOfMeasure: 'bag',
        sellingPrice: 45.00,
        costPrice: 35.00,
        stockQuantity: 80,
        reorderLevel: 10,
        maxStockLevel: 200,
        isActive: true,
        taxable: false,
        createdAt: DateTime.now(),
      ),
      ProductEntity(
        id: '3',
        name: 'Cooking Oil - 1L',
        description: 'Sunflower cooking oil 1 liter',
        sku: 'OIL1L',
        category: 'Food',
        subcategory: 'Cooking',
        unitOfMeasure: 'bottle',
        sellingPrice: 12.00,
        costPrice: 8.50,
        stockQuantity: 45,
        reorderLevel: 15,
        maxStockLevel: 150,
        isActive: true,
        taxable: false,
        createdAt: DateTime.now(),
      ),
    ];
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return const Scaffold(
        body: Center(child: CircularProgressIndicator()),
      );
    }

    return Scaffold(
      appBar: AppBar(
        title: const Text('Create Inventory Transfer'),
        actions: [
          TextButton(
            onPressed: _canCreateTransfer() ? _createTransfer : null,
            child: const Text(
              'CREATE',
              style: TextStyle(
                fontWeight: FontWeight.bold,
                color: Colors.white,
              ),
            ),
          ),
        ],
      ),
      body: Form(
        key: _formKey,
        child: Column(
          children: [
            Expanded(
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    _buildTransferHeaderSection(),
                    const SizedBox(height: 24),
                    _buildLocationSelectionSection(),
                    const SizedBox(height: 24),
                    _buildItemsSection(),
                    const SizedBox(height: 24),
                    _buildNotesSection(),
                  ],
                ),
              ),
            ),
            _buildSummarySection(),
          ],
        ),
      ),
    );
  }

  Widget _buildTransferHeaderSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Transfer Information',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 16),
            DropdownButtonFormField<TransferType>(
              value: _transferType,
              decoration: const InputDecoration(
                labelText: 'Transfer Type',
                border: OutlineInputBorder(),
              ),
              items: TransferType.values.map((type) {
                return DropdownMenuItem(
                  value: type,
                  child: Row(
                    children: [
                      Icon(_getTransferTypeIcon(type)),
                      const SizedBox(width: 8),
                      Text(_getTransferTypeName(type)),
                    ],
                  ),
                );
              }).toList(),
              onChanged: (value) {
                setState(() {
                  _transferType = value!;
                });
              },
              validator: (value) {
                if (value == null) return 'Please select transfer type';
                return null;
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildLocationSelectionSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Locations',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 16),
            
            // From Location
            DropdownButtonFormField<LocationEntity>(
              value: _fromLocation,
              decoration: const InputDecoration(
                labelText: 'From Location',
                border: OutlineInputBorder(),
                prefixIcon: Icon(Icons.store_mall_directory),
              ),
              items: _availableLocations.where((loc) => 
                loc.canTransferInventory && loc.status == LocationStatus.active
              ).map((location) {
                return DropdownMenuItem(
                  value: location,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(location.name),
                      Text(
                        '${location.code} • ${location.city}',
                        style: TextStyle(
                          fontSize: 12,
                          color: Colors.grey[600],
                        ),
                      ),
                    ],
                  ),
                );
              }).toList(),
              onChanged: (value) {
                setState(() {
                  _fromLocation = value;
                  // Clear items when source location changes
                  _transferItems.clear();
                });
              },
              validator: (value) {
                if (value == null) return 'Please select source location';
                return null;
              },
            ),
            
            const SizedBox(height: 16),
            
            // Transfer direction indicator
            Center(
              child: Container(
                padding: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  color: Theme.of(context).primaryColor.withOpacity(0.1),
                  borderRadius: BorderRadius.circular(20),
                ),
                child: Icon(
                  Icons.arrow_downward,
                  color: Theme.of(context).primaryColor,
                ),
              ),
            ),
            
            const SizedBox(height: 16),
            
            // To Location
            DropdownButtonFormField<LocationEntity>(
              value: _toLocation,
              decoration: const InputDecoration(
                labelText: 'To Location',
                border: OutlineInputBorder(),
                prefixIcon: Icon(Icons.store),
              ),
              items: _availableLocations.where((loc) => 
                loc.canReceiveTransfers && 
                loc.status == LocationStatus.active &&
                loc.id != _fromLocation?.id
              ).map((location) {
                return DropdownMenuItem(
                  value: location,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(location.name),
                      Text(
                        '${location.code} • ${location.city}',
                        style: TextStyle(
                          fontSize: 12,
                          color: Colors.grey[600],
                        ),
                      ),
                    ],
                  ),
                );
              }).toList(),
              onChanged: (value) {
                setState(() {
                  _toLocation = value;
                });
              },
              validator: (value) {
                if (value == null) return 'Please select destination location';
                if (value.id == _fromLocation?.id) {
                  return 'Destination must be different from source';
                }
                return null;
              },
            ),
            
            // Connection status
            if (_fromLocation != null && _toLocation != null)
              Container(
                margin: const EdgeInsets.only(top: 12),
                padding: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  color: _areLocationsConnected() ? Colors.green.withOpacity(0.1) : Colors.red.withOpacity(0.1),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(
                    color: _areLocationsConnected() ? Colors.green : Colors.red,
                  ),
                ),
                child: Row(
                  children: [
                    Icon(
                      _areLocationsConnected() ? Icons.check_circle : Icons.error,
                      color: _areLocationsConnected() ? Colors.green : Colors.red,
                      size: 16,
                    ),
                    const SizedBox(width: 8),
                    Text(
                      _areLocationsConnected() 
                          ? 'Locations are connected for transfers'
                          : 'Locations are not connected - transfer requires approval',
                      style: TextStyle(
                        color: _areLocationsConnected() ? Colors.green : Colors.red,
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

  Widget _buildItemsSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text(
                  'Items to Transfer',
                  style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                ),
                ElevatedButton.icon(
                  onPressed: _fromLocation != null ? _showAddItemDialog : null,
                  icon: const Icon(Icons.add),
                  label: const Text('Add Item'),
                ),
              ],
            ),
            const SizedBox(height: 16),
            
            if (_transferItems.isEmpty)
              Container(
                width: double.infinity,
                padding: const EdgeInsets.all(32),
                decoration: BoxDecoration(
                  border: Border.all(color: Colors.grey.withOpacity(0.3)),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Column(
                  children: [
                    Icon(
                      Icons.inventory_outlined,
                      size: 48,
                      color: Colors.grey[400],
                    ),
                    const SizedBox(height: 16),
                    Text(
                      'No items added yet',
                      style: TextStyle(
                        fontSize: 16,
                        color: Colors.grey[600],
                      ),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'Add items to transfer from the source location',
                      style: TextStyle(
                        fontSize: 14,
                        color: Colors.grey[500],
                      ),
                    ),
                  ],
                ),
              )
            else
              ...._transferItems.asMap().entries.map((entry) {
                final index = entry.key;
                final item = entry.value;
                final product = _availableProducts.where((p) => p.id == item.productId).firstOrNull;
                
                return Container(
                  margin: const EdgeInsets.only(bottom: 8),
                  decoration: BoxDecoration(
                    border: Border.all(color: Colors.grey.withOpacity(0.3)),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: ListTile(
                    leading: CircleAvatar(
                      backgroundColor: Theme.of(context).primaryColor.withOpacity(0.1),
                      child: Text(
                        '${index + 1}',
                        style: TextStyle(
                          color: Theme.of(context).primaryColor,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                    title: Text(
                      product?.name ?? 'Unknown Product',
                      style: const TextStyle(fontWeight: FontWeight.bold),
                    ),
                    subtitle: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text('SKU: ${product?.sku ?? 'N/A'}'),
                        Text('Quantity: ${item.requestedQuantity}'),
                        Text('Unit Cost: GHS ${item.unitCost.toStringAsFixed(2)}'),
                        Text('Total: GHS ${(item.unitCost * item.requestedQuantity).toStringAsFixed(2)}'),
                      ],
                    ),
                    trailing: PopupMenuButton<String>(
                      onSelected: (value) {
                        if (value == 'edit') {
                          _showEditItemDialog(index, item);
                        } else if (value == 'remove') {
                          setState(() {
                            _transferItems.removeAt(index);
                          });
                        }
                      },
                      itemBuilder: (context) => [
                        const PopupMenuItem(
                          value: 'edit',
                          child: ListTile(
                            leading: Icon(Icons.edit),
                            title: Text('Edit'),
                          ),
                        ),
                        const PopupMenuItem(
                          value: 'remove',
                          child: ListTile(
                            leading: Icon(Icons.delete, color: Colors.red),
                            title: Text('Remove', style: TextStyle(color: Colors.red)),
                          ),
                        ),
                      ],
                    ),
                  ),
                );
              }).toList(),
          ],
        ),
      ),
    );
  }

  Widget _buildNotesSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Additional Notes',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _notesController,
              decoration: const InputDecoration(
                hintText: 'Add any additional notes about this transfer...',
                border: OutlineInputBorder(),
              ),
              maxLines: 3,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSummarySection() {
    final totalItems = _transferItems.fold<int>(0, (sum, item) => sum + item.requestedQuantity);
    final totalValue = _transferItems.fold<double>(0, (sum, item) => sum + (item.unitCost * item.requestedQuantity));

    return Container(
      padding: const EdgeInsets.all(16.0),
      decoration: BoxDecoration(
        color: Theme.of(context).cardColor,
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.1),
            blurRadius: 4,
            offset: const Offset(0, -2),
          ),
        ],
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                'Total Items: $totalItems',
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
              Text(
                'Total Value: GHS ${totalValue.toStringAsFixed(2)}',
                style: TextStyle(
                  fontSize: 14,
                  color: Colors.grey[600],
                ),
              ),
            ],
          ),
          ElevatedButton.icon(
            onPressed: _canCreateTransfer() ? _createTransfer : null,
            icon: const Icon(Icons.send),
            label: const Text('Create Transfer'),
            style: ElevatedButton.styleFrom(
              padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
            ),
          ),
        ],
      ),
    );
  }

  // Helper methods
  bool _areLocationsConnected() {
    if (_fromLocation == null || _toLocation == null) return false;
    return _fromLocation!.connectedLocations.contains(_toLocation!.id);
  }

  bool _canCreateTransfer() {
    return _fromLocation != null && 
           _toLocation != null && 
           _transferItems.isNotEmpty &&
           _formKey.currentState?.validate() == true;
  }

  IconData _getTransferTypeIcon(TransferType type) {
    switch (type) {
      case TransferType.stock:
        return Icons.inventory;
      case TransferType.emergency:
        return Icons.priority_high;
      case TransferType.rebalance:
        return Icons.balance;
      case TransferType.returned:
        return Icons.keyboard_return;
      case TransferType.damaged:
        return Icons.warning;
    }
  }

  String _getTransferTypeName(TransferType type) {
    switch (type) {
      case TransferType.stock:
        return 'Stock Transfer';
      case TransferType.emergency:
        return 'Emergency Transfer';
      case TransferType.rebalance:
        return 'Rebalance Transfer';
      case TransferType.returned:
        return 'Return Transfer';
      case TransferType.damaged:
        return 'Damaged Goods Transfer';
    }
  }

  // Dialog methods
  void _showAddItemDialog() {
    showDialog(
      context: context,
      builder: (context) => _ItemSelectionDialog(
        availableProducts: _availableProducts,
        onItemAdded: (item) {
          setState(() {
            _transferItems.add(item);
          });
        },
      ),
    );
  }

  void _showEditItemDialog(int index, TransferItemEntity item) {
    showDialog(
      context: context,
      builder: (context) => _ItemEditDialog(
        item: item,
        availableProducts: _availableProducts,
        onItemUpdated: (updatedItem) {
          setState(() {
            _transferItems[index] = updatedItem;
          });
        },
      ),
    );
  }

  Future<void> _createTransfer() async {
    if (!_formKey.currentState!.validate() || !_canCreateTransfer()) {
      return;
    }

    try {
      showDialog(
        context: context,
        barrierDismissible: false,
        builder: (context) => const AlertDialog(
          content: Row(
            children: [
              CircularProgressIndicator(),
              SizedBox(width: 16),
              Text('Creating transfer...'),
            ],
          ),
        ),
      );

      await _locationService.createTransfer(
        fromLocationId: _fromLocation!.id,
        toLocationId: _toLocation!.id,
        type: _transferType,
        items: _transferItems,
        notes: _notesController.text.trim().isNotEmpty ? _notesController.text.trim() : null,
        requestedById: 'current_user_id', // Replace with actual user ID
      );

      if (mounted) {
        Navigator.pop(context); // Close loading dialog
        Navigator.pop(context, true); // Return to previous screen with success result
        
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Transfer created successfully'),
            backgroundColor: Colors.green,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        Navigator.pop(context); // Close loading dialog
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Error creating transfer: $e'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  @override
  void dispose() {
    _notesController.dispose();
    super.dispose();
  }
}

// Item Selection Dialog
class _ItemSelectionDialog extends StatefulWidget {
  final List<ProductEntity> availableProducts;
  final Function(TransferItemEntity) onItemAdded;

  const _ItemSelectionDialog({
    required this.availableProducts,
    required this.onItemAdded,
  });

  @override
  State<_ItemSelectionDialog> createState() => _ItemSelectionDialogState();
}

class _ItemSelectionDialogState extends State<_ItemSelectionDialog> {
  ProductEntity? _selectedProduct;
  final TextEditingController _quantityController = TextEditingController();
  final TextEditingController _unitCostController = TextEditingController();
  final _formKey = GlobalKey<FormState>();

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Add Transfer Item'),
      content: SizedBox(
        width: double.maxFinite,
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              DropdownButtonFormField<ProductEntity>(
                value: _selectedProduct,
                decoration: const InputDecoration(
                  labelText: 'Product',
                  border: OutlineInputBorder(),
                ),
                items: widget.availableProducts.map((product) {
                  return DropdownMenuItem(
                    value: product,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(product.name),
                        Text(
                          'SKU: ${product.sku} • Stock: ${product.stockQuantity}',
                          style: TextStyle(
                            fontSize: 12,
                            color: Colors.grey[600],
                          ),
                        ),
                      ],
                    ),
                  );
                }).toList(),
                onChanged: (value) {
                  setState(() {
                    _selectedProduct = value;
                    _unitCostController.text = value?.costPrice.toStringAsFixed(2) ?? '';
                  });
                },
                validator: (value) {
                  if (value == null) return 'Please select a product';
                  return null;
                },
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _quantityController,
                decoration: const InputDecoration(
                  labelText: 'Quantity to Transfer',
                  border: OutlineInputBorder(),
                ),
                keyboardType: TextInputType.number,
                inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter quantity';
                  }
                  final quantity = int.tryParse(value);
                  if (quantity == null || quantity <= 0) {
                    return 'Please enter a valid quantity';
                  }
                  if (_selectedProduct != null && quantity > _selectedProduct!.stockQuantity) {
                    return 'Quantity exceeds available stock (${_selectedProduct!.stockQuantity})';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _unitCostController,
                decoration: const InputDecoration(
                  labelText: 'Unit Cost (GHS)',
                  border: OutlineInputBorder(),
                ),
                keyboardType: const TextInputType.numberWithOptions(decimal: true),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter unit cost';
                  }
                  final cost = double.tryParse(value);
                  if (cost == null || cost <= 0) {
                    return 'Please enter a valid cost';
                  }
                  return null;
                },
              ),
            ],
          ),
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: const Text('Cancel'),
        ),
        ElevatedButton(
          onPressed: () {
            if (_formKey.currentState!.validate()) {
              final item = TransferItemEntity(
                id: DateTime.now().millisecondsSinceEpoch.toString(),
                productId: _selectedProduct!.id,
                requestedQuantity: int.parse(_quantityController.text),
                unitCost: double.parse(_unitCostController.text),
              );
              widget.onItemAdded(item);
              Navigator.pop(context);
            }
          },
          child: const Text('Add Item'),
        ),
      ],
    );
  }

  @override
  void dispose() {
    _quantityController.dispose();
    _unitCostController.dispose();
    super.dispose();
  }
}

// Item Edit Dialog
class _ItemEditDialog extends StatefulWidget {
  final TransferItemEntity item;
  final List<ProductEntity> availableProducts;
  final Function(TransferItemEntity) onItemUpdated;

  const _ItemEditDialog({
    required this.item,
    required this.availableProducts,
    required this.onItemUpdated,
  });

  @override
  State<_ItemEditDialog> createState() => _ItemEditDialogState();
}

class _ItemEditDialogState extends State<_ItemEditDialog> {
  late ProductEntity _selectedProduct;
  late TextEditingController _quantityController;
  late TextEditingController _unitCostController;
  final _formKey = GlobalKey<FormState>();

  @override
  void initState() {
    super.initState();
    _selectedProduct = widget.availableProducts.firstWhere((p) => p.id == widget.item.productId);
    _quantityController = TextEditingController(text: widget.item.requestedQuantity.toString());
    _unitCostController = TextEditingController(text: widget.item.unitCost.toStringAsFixed(2));
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Edit Transfer Item'),
      content: SizedBox(
        width: double.maxFinite,
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              // Product display (read-only)
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.grey.withOpacity(0.1),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: Colors.grey.withOpacity(0.3)),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.inventory_2),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            _selectedProduct.name,
                            style: const TextStyle(fontWeight: FontWeight.bold),
                          ),
                          Text(
                            'SKU: ${_selectedProduct.sku} • Stock: ${_selectedProduct.stockQuantity}',
                            style: TextStyle(
                              fontSize: 12,
                              color: Colors.grey[600],
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _quantityController,
                decoration: const InputDecoration(
                  labelText: 'Quantity to Transfer',
                  border: OutlineInputBorder(),
                ),
                keyboardType: TextInputType.number,
                inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter quantity';
                  }
                  final quantity = int.tryParse(value);
                  if (quantity == null || quantity <= 0) {
                    return 'Please enter a valid quantity';
                  }
                  if (quantity > _selectedProduct.stockQuantity) {
                    return 'Quantity exceeds available stock (${_selectedProduct.stockQuantity})';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _unitCostController,
                decoration: const InputDecoration(
                  labelText: 'Unit Cost (GHS)',
                  border: OutlineInputBorder(),
                ),
                keyboardType: const TextInputType.numberWithOptions(decimal: true),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter unit cost';
                  }
                  final cost = double.tryParse(value);
                  if (cost == null || cost <= 0) {
                    return 'Please enter a valid cost';
                  }
                  return null;
                },
              ),
            ],
          ),
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: const Text('Cancel'),
        ),
        ElevatedButton(
          onPressed: () {
            if (_formKey.currentState!.validate()) {
              final updatedItem = TransferItemEntity(
                id: widget.item.id,
                productId: widget.item.productId,
                requestedQuantity: int.parse(_quantityController.text),
                unitCost: double.parse(_unitCostController.text),
                batchId: widget.item.batchId,
                shippedQuantity: widget.item.shippedQuantity,
                receivedQuantity: widget.item.receivedQuantity,
                expiryDate: widget.item.expiryDate,
                serialNumbers: widget.item.serialNumbers,
                notes: widget.item.notes,
              );
              widget.onItemUpdated(updatedItem);
              Navigator.pop(context);
            }
          },
          child: const Text('Update Item'),
        ),
      ],
    );
  }

  @override
  void dispose() {
    _quantityController.dispose();
    _unitCostController.dispose();
    super.dispose();
  }
}
