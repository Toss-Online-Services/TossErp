import 'package:flutter/material.dart';
import '../../../../domain/entities/customer_entity.dart';

class POSCustomerSelector extends StatelessWidget {
  final CustomerEntity? selectedCustomer;
  final Function(CustomerEntity?) onCustomerChanged;

  const POSCustomerSelector({
    super.key,
    required this.selectedCustomer,
    required this.onCustomerChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      child: Row(
        children: [
          const Icon(Icons.person_outline),
          const SizedBox(width: 8),
          Expanded(
            child: selectedCustomer != null
                ? Text(selectedCustomer!.name ?? 'Unknown Customer')
                : const Text('Select Customer'),
          ),
          IconButton(
            icon: const Icon(Icons.search),
            onPressed: () => _showCustomerSelector(context),
          ),
          if (selectedCustomer != null)
            IconButton(
              icon: const Icon(Icons.clear),
              onPressed: () => onCustomerChanged(null),
            ),
        ],
      ),
    );
  }

  void _showCustomerSelector(BuildContext context) {
    // Implementation for customer selection dialog
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Select Customer'),
        content: const Text('Customer selection coming soon'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Close'),
          ),
        ],
      ),
    );
  }
}
