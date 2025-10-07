import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../providers/purchase_order/purchase_order_provider.dart';
import '../../../domain/entities/purchase_order_entity.dart';

class PurchaseOrderFormScreen extends StatefulWidget {
  final PurchaseOrderEntity? purchaseOrder;

  const PurchaseOrderFormScreen({super.key, this.purchaseOrder});

  @override
  State<PurchaseOrderFormScreen> createState() => _PurchaseOrderFormScreenState();
}

class _PurchaseOrderFormScreenState extends State<PurchaseOrderFormScreen> {
  final _formKey = GlobalKey<FormState>();
  final _orderNumberController = TextEditingController();
  final _notesController = TextEditingController();

  int _supplierId = 1;
  PurchaseOrderStatus _status = PurchaseOrderStatus.draft;
  DateTime? _orderDate;
  DateTime? _expectedDate;
  int _totalAmount = 0;
  int _taxAmount = 0;
  int _discountAmount = 0;

  @override
  void initState() {
    super.initState();
    if (widget.purchaseOrder != null) {
      _populateForm();
    } else {
      _orderDate = DateTime.now();
      _expectedDate = DateTime.now().add(const Duration(days: 7));
    }
  }

  void _populateForm() {
    final order = widget.purchaseOrder!;
    _orderNumberController.text = order.orderNumber;
    _supplierId = order.supplierId;
    _status = order.status;
    _orderDate = order.orderDate;
    _expectedDate = order.expectedDate;
    _totalAmount = order.totalAmount;
    _taxAmount = order.taxAmount ?? 0;
    _discountAmount = order.discountAmount ?? 0;
    _notesController.text = order.notes ?? '';
  }

  @override
  void dispose() {
    _orderNumberController.dispose();
    _notesController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final isEditing = widget.purchaseOrder != null;
    
    return Scaffold(
      appBar: AppBar(
        title: Text(isEditing ? 'Edit Purchase Order' : 'Create Purchase Order'),
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
              controller: _orderNumberController,
              decoration: const InputDecoration(
                labelText: 'Order Number *',
                border: OutlineInputBorder(),
              ),
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return 'Please enter order number';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
            TextFormField(
              decoration: const InputDecoration(
                labelText: 'Supplier ID',
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.number,
              initialValue: _supplierId.toString(),
              onChanged: (value) {
                _supplierId = int.tryParse(value) ?? 1;
              },
            ),
            const SizedBox(height: 16),
            DropdownButtonFormField<PurchaseOrderStatus>(
              decoration: const InputDecoration(
                labelText: 'Status',
                border: OutlineInputBorder(),
              ),
              value: _status,
              items: PurchaseOrderStatus.values.map((status) {
                return DropdownMenuItem(
                  value: status,
                  child: Text(_getStatusDisplayName(status)),
                );
              }).toList(),
              onChanged: (value) {
                setState(() {
                  _status = value ?? PurchaseOrderStatus.draft;
                });
              },
            ),

            const SizedBox(height: 24),

            // Date Information
            _buildSectionHeader('Date Information'),
            ListTile(
              title: const Text('Order Date'),
              subtitle: Text(_orderDate != null 
                ? '${_orderDate!.day}/${_orderDate!.month}/${_orderDate!.year}'
                : 'Select date'),
              trailing: const Icon(Icons.calendar_today),
              onTap: () async {
                final date = await showDatePicker(
                  context: context,
                  initialDate: _orderDate ?? DateTime.now(),
                  firstDate: DateTime(2020),
                  lastDate: DateTime(2030),
                );
                if (date != null) {
                  setState(() {
                    _orderDate = date;
                  });
                }
              },
            ),
            ListTile(
              title: const Text('Expected Date'),
              subtitle: Text(_expectedDate != null 
                ? '${_expectedDate!.day}/${_expectedDate!.month}/${_expectedDate!.year}'
                : 'Select date'),
              trailing: const Icon(Icons.calendar_today),
              onTap: () async {
                final date = await showDatePicker(
                  context: context,
                  initialDate: _expectedDate ?? DateTime.now().add(const Duration(days: 7)),
                  firstDate: DateTime.now(),
                  lastDate: DateTime(2030),
                );
                if (date != null) {
                  setState(() {
                    _expectedDate = date;
                  });
                }
              },
            ),

            const SizedBox(height: 24),

            // Financial Information
            _buildSectionHeader('Financial Information'),
            TextFormField(
              decoration: const InputDecoration(
                labelText: 'Total Amount (in cents)',
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.number,
              initialValue: _totalAmount.toString(),
              onChanged: (value) {
                _totalAmount = int.tryParse(value) ?? 0;
              },
            ),
            const SizedBox(height: 16),
            TextFormField(
              decoration: const InputDecoration(
                labelText: 'Tax Amount (in cents)',
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.number,
              initialValue: _taxAmount.toString(),
              onChanged: (value) {
                _taxAmount = int.tryParse(value) ?? 0;
              },
            ),
            const SizedBox(height: 16),
            TextFormField(
              decoration: const InputDecoration(
                labelText: 'Discount Amount (in cents)',
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.number,
              initialValue: _discountAmount.toString(),
              onChanged: (value) {
                _discountAmount = int.tryParse(value) ?? 0;
              },
            ),

            const SizedBox(height: 24),

            // Additional Information
            _buildSectionHeader('Additional Information'),
            TextFormField(
              controller: _notesController,
              decoration: const InputDecoration(
                labelText: 'Notes',
                border: OutlineInputBorder(),
              ),
              maxLines: 3,
            ),

            const SizedBox(height: 32),

            // Save Button
            ElevatedButton(
              onPressed: _savePurchaseOrder,
              style: ElevatedButton.styleFrom(
                padding: const EdgeInsets.symmetric(vertical: 16),
              ),
              child: Text(
                isEditing ? 'Update Purchase Order' : 'Create Purchase Order',
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

  String _getStatusDisplayName(PurchaseOrderStatus status) {
    switch (status) {
      case PurchaseOrderStatus.draft:
        return 'Draft';
      case PurchaseOrderStatus.pending:
        return 'Pending';
      case PurchaseOrderStatus.ordered:
        return 'Ordered';
      case PurchaseOrderStatus.partiallyReceived:
        return 'Partially Received';
      case PurchaseOrderStatus.received:
        return 'Received';
      case PurchaseOrderStatus.cancelled:
        return 'Cancelled';
    }
  }

  void _savePurchaseOrder() {
    if (_formKey.currentState!.validate()) {
      final provider = context.read<PurchaseOrderProvider>();
      
      final purchaseOrder = widget.purchaseOrder?.copyWith(
        orderNumber: _orderNumberController.text,
        supplierId: _supplierId,
        status: _status,
        orderDate: _orderDate!,
        expectedDate: _expectedDate,
        totalAmount: _totalAmount,
        taxAmount: _taxAmount,
        discountAmount: _discountAmount,
        notes: _notesController.text.isEmpty ? null : _notesController.text,
        updatedAt: DateTime.now().toIso8601String(),
      ) ?? provider.createNewPurchaseOrder(
        orderNumber: _orderNumberController.text,
        supplierId: _supplierId,
        status: _status,
        orderDate: _orderDate,
        expectedDate: _expectedDate,
        totalAmount: _totalAmount,
        taxAmount: _taxAmount,
        discountAmount: _discountAmount,
        notes: _notesController.text.isEmpty ? null : _notesController.text,
      );

      final isEditing = widget.purchaseOrder != null;
      final future = isEditing 
        ? provider.updatePurchaseOrder(purchaseOrder)
        : provider.createPurchaseOrder(purchaseOrder);

      future.then((success) {
        if (success) {
          Navigator.pop(context);
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text(
                isEditing ? 'Purchase order updated successfully' : 'Purchase order created successfully',
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
        title: const Text('Delete Purchase Order'),
        content: const Text('Are you sure you want to delete this purchase order? This action cannot be undone.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              Navigator.pop(context);
              _deletePurchaseOrder();
            },
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: const Text('Delete'),
          ),
        ],
      ),
    );
  }

  void _deletePurchaseOrder() {
    final provider = context.read<PurchaseOrderProvider>();
    provider.deletePurchaseOrder(widget.purchaseOrder!.id!).then((success) {
      if (success) {
        Navigator.pop(context);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Purchase order deleted successfully'),
          ),
        );
      }
    });
  }
}
