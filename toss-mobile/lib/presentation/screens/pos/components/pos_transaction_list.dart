import 'package:flutter/material.dart';
import '../../../../domain/entities/sales_transaction_entity.dart';

class POSTransactionList extends StatelessWidget {
  final List<SalesTransactionEntity> transactions;
  final Function(SalesTransactionEntity)? onTransactionTap;

  const POSTransactionList({
    super.key,
    this.transactions = const [],
    this.onTransactionTap,
  });

  @override
  Widget build(BuildContext context) {
    if (transactions.isEmpty) {
      return const Card(
        child: Padding(
          padding: EdgeInsets.all(16.0),
          child: Center(
            child: Text('No recent transactions'),
          ),
        ),
      );
    }

    return Card(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Padding(
            padding: EdgeInsets.all(16.0),
            child: Text(
              'Recent Transactions',
              style: TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          Expanded(
            child: ListView.builder(
              itemCount: transactions.length,
              itemBuilder: (context, index) {
                final transaction = transactions[index];
                return ListTile(
                  leading: CircleAvatar(
                    child: Text('#${index + 1}'),
                  ),
                  title: Text('Transaction ${transaction.id}'),
                  subtitle: Text('\$${transaction.totalAmount.toStringAsFixed(2)}'),
                  trailing: Text(
                    transaction.status.toString().split('.').last.toUpperCase(),
                    style: TextStyle(
                      color: _getStatusColor(transaction.status),
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  onTap: () => onTransactionTap?.call(transaction),
                );
              },
            ),
          ),
        ],
      ),
    );
  }

  Color _getStatusColor(TransactionStatus status) {
    switch (status) {
      case TransactionStatus.completed:
        return Colors.green;
      case TransactionStatus.pending:
        return Colors.orange;
      case TransactionStatus.cancelled:
        return Colors.red;
      default:
        return Colors.grey;
    }
  }
}
