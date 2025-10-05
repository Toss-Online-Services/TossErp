import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../providers/purchase_order/purchase_order_provider.dart';
import '../../../domain/entities/purchase_order_entity.dart';
import 'purchase_order_form_screen.dart';

class PurchaseOrderDetailScreen extends StatelessWidget {
  final PurchaseOrderEntity purchaseOrder;

  const PurchaseOrderDetailScreen({super.key, required this.purchaseOrder});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(purchaseOrder.orderNumber),
        actions: [
          IconButton(
            icon: const Icon(Icons.edit),
            onPressed: () {
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => PurchaseOrderFormScreen(purchaseOrder: purchaseOrder),
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
                          backgroundColor: _getStatusColor(purchaseOrder.status),
                          child: Icon(
                            _getStatusIcon(purchaseOrder.status),
                            color: Colors.white,
                            size: 30,
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                purchaseOrder.orderNumber,
                                style: const TextStyle(
                                  fontSize: 24,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              Text(
                                'Supplier ID: ${purchaseOrder.supplierId}',
                                style: const TextStyle(
                                  fontSize: 16,
                                  color: Colors.grey,
                                ),
                              ),
                              const SizedBox(height: 8),
                              Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 8,
                                  vertical: 4,
                                ),
                                decoration: BoxDecoration(
                                  color: _getStatusColor(purchaseOrder.status),
                                  borderRadius: BorderRadius.circular(12),
                                ),
                                child: Text(
                                  _getStatusDisplayName(purchaseOrder.status),
                                  style: const TextStyle(
                                    color: Colors.white,
                                    fontSize: 12,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                              ),
                              if (purchaseOrder.isOverdue)
                                const Padding(
                                  padding: EdgeInsets.only(top: 4.0),
                                  child: Text(
                                    'OVERDUE',
                                    style: TextStyle(
                                      color: Colors.red,
                                      fontWeight: FontWeight.bold,
                                    ),
                                  ),
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

            // Financial Information
            _buildSectionCard(
              'Financial Information',
              [
                _buildInfoRow('Total Amount', '\$${(purchaseOrder.totalAmount / 100).toStringAsFixed(2)}'),
                if (purchaseOrder.taxAmount != null)
                  _buildInfoRow('Tax Amount', '\$${(purchaseOrder.taxAmount! / 100).toStringAsFixed(2)}'),
                if (purchaseOrder.discountAmount != null)
                  _buildInfoRow('Discount Amount', '\$${(purchaseOrder.discountAmount! / 100).toStringAsFixed(2)}'),
                _buildInfoRow('Net Amount', '\$${(purchaseOrder.netAmount / 100).toStringAsFixed(2)}'),
              ],
            ),

            const SizedBox(height: 16),

            // Date Information
            _buildSectionCard(
              'Date Information',
              [
                _buildInfoRow('Order Date', _formatDate(purchaseOrder.orderDate)),
                if (purchaseOrder.expectedDate != null)
                  _buildInfoRow('Expected Date', _formatDate(purchaseOrder.expectedDate!)),
                if (purchaseOrder.receivedDate != null)
                  _buildInfoRow('Received Date', _formatDate(purchaseOrder.receivedDate!)),
              ],
            ),

            const SizedBox(height: 16),

            // Items Information
            if (purchaseOrder.items.isNotEmpty)
              _buildSectionCard(
                'Items',
                purchaseOrder.items.map((item) => 
                  _buildItemRow(item)
                ).toList(),
              ),

            if (purchaseOrder.items.isNotEmpty)
              const SizedBox(height: 16),

            // Additional Information
            if (purchaseOrder.notes != null)
              _buildSectionCard(
                'Additional Information',
                [
                  _buildInfoRow('Notes', purchaseOrder.notes!),
                ],
              ),

            if (purchaseOrder.notes != null)
              const SizedBox(height: 16),

            // Metadata
            _buildSectionCard(
              'Metadata',
              [
                if (purchaseOrder.createdAt != null)
                  _buildInfoRow('Created', _formatDateString(purchaseOrder.createdAt!)),
                if (purchaseOrder.updatedAt != null)
                  _buildInfoRow('Last Updated', _formatDateString(purchaseOrder.updatedAt!)),
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
                          builder: (context) => PurchaseOrderFormScreen(purchaseOrder: purchaseOrder),
                        ),
                      );
                    },
                    icon: const Icon(Icons.edit),
                    label: const Text('Edit Order'),
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

            const SizedBox(height: 16),

            // Status Update Buttons
            if (purchaseOrder.status != PurchaseOrderStatus.received && 
                purchaseOrder.status != PurchaseOrderStatus.cancelled)
              Column(
                children: [
                  const Divider(),
                  const SizedBox(height: 16),
                  Text(
                    'Update Status',
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 16),
                  Wrap(
                    spacing: 8,
                    children: [
                      if (purchaseOrder.status == PurchaseOrderStatus.draft)
                        ElevatedButton(
                          onPressed: () => _updateStatus(context, PurchaseOrderStatus.pending),
                          child: const Text('Mark Pending'),
                        ),
                      if (purchaseOrder.status == PurchaseOrderStatus.pending)
                        ElevatedButton(
                          onPressed: () => _updateStatus(context, PurchaseOrderStatus.ordered),
                          child: const Text('Mark Ordered'),
                        ),
                      if (purchaseOrder.status == PurchaseOrderStatus.ordered)
                        ElevatedButton(
                          onPressed: () => _updateStatus(context, PurchaseOrderStatus.partiallyReceived),
                          child: const Text('Partially Received'),
                        ),
                      if (purchaseOrder.status == PurchaseOrderStatus.partiallyReceived)
                        ElevatedButton(
                          onPressed: () => _updateStatus(context, PurchaseOrderStatus.received),
                          child: const Text('Mark Received'),
                        ),
                      if (purchaseOrder.status != PurchaseOrderStatus.cancelled)
                        OutlinedButton(
                          onPressed: () => _updateStatus(context, PurchaseOrderStatus.cancelled),
                          style: OutlinedButton.styleFrom(foregroundColor: Colors.red),
                          child: const Text('Cancel'),
                        ),
                    ],
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

  Widget _buildItemRow(item) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8.0),
      child: Padding(
        padding: const EdgeInsets.all(12.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'Product ID: ${item.productId}',
                  style: const TextStyle(fontWeight: FontWeight.bold),
                ),
                Text(
                  '\$${(item.totalPrice / 100).toStringAsFixed(2)}',
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    color: Colors.green,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                Text('Quantity: ${item.quantity}'),
                const SizedBox(width: 16),
                Text('Unit Price: \$${(item.unitPrice / 100).toStringAsFixed(2)}'),
              ],
            ),
            if (item.quantityReceived > 0) ...[
              const SizedBox(height: 4),
              Text('Received: ${item.quantityReceived}'),
            ],
            if (item.notes != null) ...[
              const SizedBox(height: 4),
              Text('Notes: ${item.notes}'),
            ],
          ],
        ),
      ),
    );
  }

  Color _getStatusColor(PurchaseOrderStatus status) {
    switch (status) {
      case PurchaseOrderStatus.draft:
        return Colors.grey;
      case PurchaseOrderStatus.pending:
        return Colors.orange;
      case PurchaseOrderStatus.ordered:
        return Colors.blue;
      case PurchaseOrderStatus.partiallyReceived:
        return Colors.purple;
      case PurchaseOrderStatus.received:
        return Colors.green;
      case PurchaseOrderStatus.cancelled:
        return Colors.red;
    }
  }

  IconData _getStatusIcon(PurchaseOrderStatus status) {
    switch (status) {
      case PurchaseOrderStatus.draft:
        return Icons.edit;
      case PurchaseOrderStatus.pending:
        return Icons.pending;
      case PurchaseOrderStatus.ordered:
        return Icons.shopping_cart;
      case PurchaseOrderStatus.partiallyReceived:
        return Icons.inventory_2;
      case PurchaseOrderStatus.received:
        return Icons.check_circle;
      case PurchaseOrderStatus.cancelled:
        return Icons.cancel;
    }
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

  String _formatDate(DateTime date) {
    return '${date.day}/${date.month}/${date.year}';
  }

  String _formatDateString(String dateString) {
    try {
      final date = DateTime.parse(dateString);
      return _formatDate(date);
    } catch (e) {
      return dateString;
    }
  }

  void _updateStatus(BuildContext context, PurchaseOrderStatus status) {
    final provider = context.read<PurchaseOrderProvider>();
    provider.updatePurchaseOrderStatus(purchaseOrder.id!, status).then((success) {
      if (success) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Status updated to ${_getStatusDisplayName(status)}'),
          ),
        );
      }
    });
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
              _deletePurchaseOrder(context);
            },
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: const Text('Delete'),
          ),
        ],
      ),
    );
  }

  void _deletePurchaseOrder(BuildContext context) {
    final provider = context.read<PurchaseOrderProvider>();
    provider.deletePurchaseOrder(purchaseOrder.id!).then((success) {
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
