import 'package:flutter/material.dart';

class POSPaymentDialog extends StatefulWidget {
  final double total;
  final Function(Map<String, dynamic> paymentData) onPaymentComplete;

  const POSPaymentDialog({
    super.key,
    required this.total,
    required this.onPaymentComplete,
  });

  @override
  State<POSPaymentDialog> createState() => _POSPaymentDialogState();
}

class _POSPaymentDialogState extends State<POSPaymentDialog> {
  String selectedPaymentMethod = 'cash';

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Payment'),
      content: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Text('Total: \$${widget.total.toStringAsFixed(2)}'),
          const SizedBox(height: 16),
          DropdownButton<String>(
            value: selectedPaymentMethod,
            items: const [
              DropdownMenuItem(value: 'cash', child: Text('Cash')),
              DropdownMenuItem(value: 'card', child: Text('Card')),
              DropdownMenuItem(value: 'digital', child: Text('Digital')),
            ],
            onChanged: (value) {
              setState(() {
                selectedPaymentMethod = value ?? 'cash';
              });
            },
          ),
        ],
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: const Text('Cancel'),
        ),
        ElevatedButton(
          onPressed: () {
            widget.onPaymentComplete({
              'method': selectedPaymentMethod,
              'amount': widget.total,
            });
            Navigator.pop(context);
          },
          child: const Text('Complete Payment'),
        ),
      ],
    );
  }
}
