import 'package:flutter/material.dart';
import '../../../../domain/entities/sales_transaction_entity.dart';

class POSTransactionList extends StatelessWidget {
  final List<SalesTransactionEntity> transactions;
  final Function(SalesTransactionEntity)? onTransactionTap;
  final Function(SalesTransactionEntity)? onReturnTransaction;
  final Function(SalesTransactionEntity)? onReprintReceipt;

  const POSTransactionList({
    super.key,
    this.transactions = const [],
    this.onTransactionTap,
    this.onReturnTransaction,
    this.onReprintReceipt,
  });

  @override
  Widget build(BuildContext context) {
    if (transactions.isEmpty) {
      return const Card(
        child: Padding(
          padding: EdgeInsets.all(16.0),
          child: Center(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(
                  Icons.history,
                  size: 64,
                  color: Colors.grey,
                ),
                SizedBox(height: 16),
                Text(
                  'No recent transactions',
                  style: TextStyle(
                    fontSize: 18,
                    color: Colors.grey,
                  ),
                ),
                SizedBox(height: 8),
                Text(
                  'Completed transactions will appear here',
                  style: TextStyle(
                    color: Colors.grey,
                  ),
                ),
              ],
            ),
          ),
        ),
      );
    }

    return Column(
      children: [
        Container(
          padding: const EdgeInsets.all(16.0),
          child: Row(
            children: [
              const Icon(Icons.history),
              const SizedBox(width: 8),
              Text(
                'Recent Transactions (${transactions.length})',
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const Spacer(),
              DropdownButton<TransactionFilterType>(
                value: TransactionFilterType.all,
                items: TransactionFilterType.values.map((filter) {
                  return DropdownMenuItem(
                    value: filter,
                    child: Text(filter.displayName),
                  );
                }).toList(),
                onChanged: (value) {
                  // TODO: Implement filtering
                },
              ),
            ],
          ),
        ),
        Expanded(
          child: ListView.builder(
            itemCount: transactions.length,
            itemBuilder: (context, index) {
              return _buildTransactionItem(context, transactions[index], index);
            },
          ),
        ),
      ],
    );
  }

  Widget _buildTransactionItem(BuildContext context, SalesTransactionEntity transaction, int index) {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      child: ExpansionTile(
        leading: CircleAvatar(
          backgroundColor: _getStatusColor(transaction.status),
          child: Text(
            '#${index + 1}',
            style: const TextStyle(
              color: Colors.white,
              fontWeight: FontWeight.bold,
              fontSize: 12,
            ),
          ),
        ),
        title: Row(
          children: [
            Expanded(
              child: Text(
                'Transaction ${transaction.transactionNumber}',
                style: const TextStyle(fontWeight: FontWeight.w600),
              ),
            ),
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
              decoration: BoxDecoration(
                color: _getStatusColor(transaction.status),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Text(
                transaction.status.toString().split('.').last.toUpperCase(),
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 10,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ],
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const SizedBox(height: 4),
            Row(
              children: [
                Text(
                  '\$${(transaction.total / 100).toStringAsFixed(2)}',
                  style: TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 16,
                    color: Theme.of(context).colorScheme.primary,
                  ),
                ),
                const SizedBox(width: 12),
                Text(
                  '${transaction.itemCount} items',
                  style: TextStyle(color: Colors.grey[600]),
                ),
                const Spacer(),
                Text(
                  _formatDateTime(transaction.createdAt),
                  style: TextStyle(
                    color: Colors.grey[600],
                    fontSize: 12,
                  ),
                ),
              ],
            ),
          ],
        ),
        children: [
          _buildTransactionDetails(context, transaction),
        ],
        onExpansionChanged: (expanded) {
          if (expanded && onTransactionTap != null) {
            onTransactionTap!(transaction);
          }
        },
      ),
    );
  }

  Widget _buildTransactionDetails(BuildContext context, SalesTransactionEntity transaction) {
    return Container(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Transaction items
          const Text(
            'Items:',
            style: TextStyle(fontWeight: FontWeight.w600),
          ),
          const SizedBox(height: 8),
          ...transaction.items.map((item) => Padding(
            padding: const EdgeInsets.only(bottom: 4),
            child: Row(
              children: [
                Text('${item.quantity}x '),
                Expanded(child: Text(item.displayName)),
                Text('\$${(item.netPrice / 100).toStringAsFixed(2)}'),
              ],
            ),
          )),
          const Divider(),
          // Transaction summary
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Subtotal:'),
              Text('\$${(transaction.subtotal / 100).toStringAsFixed(2)}'),
            ],
          ),
          if (transaction.discountAmount > 0) ...[
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text('Discount:', style: TextStyle(color: Colors.red)),
                Text(
                  '-\$${(transaction.discountAmount / 100).toStringAsFixed(2)}',
                  style: const TextStyle(color: Colors.red),
                ),
              ],
            ),
          ],
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Total:', style: TextStyle(fontWeight: FontWeight.bold)),
              Text(
                '\$${(transaction.total / 100).toStringAsFixed(2)}',
                style: const TextStyle(fontWeight: FontWeight.bold),
              ),
            ],
          ),
          const SizedBox(height: 16),
          // Action buttons
          Row(
            children: [
              if (transaction.isCompleted && onReturnTransaction != null) ...[
                Expanded(
                  child: OutlinedButton.icon(
                    onPressed: () => onReturnTransaction!(transaction),
                    icon: const Icon(Icons.undo),
                    label: const Text('Return'),
                    style: OutlinedButton.styleFrom(
                      foregroundColor: Colors.orange,
                    ),
                  ),
                ),
                const SizedBox(width: 8),
              ],
              if (onReprintReceipt != null) ...[
                Expanded(
                  child: OutlinedButton.icon(
                    onPressed: () => onReprintReceipt!(transaction),
                    icon: const Icon(Icons.print),
                    label: const Text('Reprint'),
                  ),
                ),
                const SizedBox(width: 8),
              ],
              Expanded(
                child: ElevatedButton.icon(
                  onPressed: () => _showTransactionDetails(context, transaction),
                  icon: const Icon(Icons.visibility),
                  label: const Text('View'),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Color _getStatusColor(SalesTransactionStatus status) {
    switch (status) {
      case SalesTransactionStatus.completed:
        return Colors.green;
      case SalesTransactionStatus.hold:
        return Colors.orange;
      case SalesTransactionStatus.active:
        return Colors.blue;
      case SalesTransactionStatus.cancelled:
        return Colors.red;
      case SalesTransactionStatus.refunded:
        return Colors.purple;
      case SalesTransactionStatus.partiallyRefunded:
        return Colors.deepPurple;
      case SalesTransactionStatus.draft:
      default:
        return Colors.grey;
    }
  }

  String _formatDateTime(DateTime dateTime) {
    final now = DateTime.now();
    final difference = now.difference(dateTime);

    if (difference.inDays > 0) {
      return '${difference.inDays}d ago';
    } else if (difference.inHours > 0) {
      return '${difference.inHours}h ago';
    } else if (difference.inMinutes > 0) {
      return '${difference.inMinutes}m ago';
    } else {
      return 'Just now';
    }
  }

  void _showTransactionDetails(BuildContext context, SalesTransactionEntity transaction) {
    showDialog(
      context: context,
      builder: (context) => TransactionDetailDialog(transaction: transaction),
    );
  }
}

enum TransactionFilterType {
  all('All'),
  paid('Paid'),
  hold('On Hold'),
  refunded('Refunded'),
  cancelled('Cancelled');

  const TransactionFilterType(this.displayName);
  final String displayName;
}

class TransactionDetailDialog extends StatelessWidget {
  final SalesTransactionEntity transaction;

  const TransactionDetailDialog({
    super.key,
    required this.transaction,
  });

  @override
  Widget build(BuildContext context) {
    return Dialog(
      child: Container(
        width: MediaQuery.of(context).size.width * 0.9,
        height: MediaQuery.of(context).size.height * 0.8,
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                const Icon(Icons.receipt_long),
                const SizedBox(width: 8),
                Text(
                  'Transaction Details',
                  style: Theme.of(context).textTheme.titleLarge?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const Spacer(),
                IconButton(
                  onPressed: () => Navigator.of(context).pop(),
                  icon: const Icon(Icons.close),
                ),
              ],
            ),
            const SizedBox(height: 20),
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: Theme.of(context).colorScheme.surfaceVariant,
                borderRadius: BorderRadius.circular(12),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Text(
                        'Transaction: ${transaction.transactionNumber}',
                        style: const TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const Spacer(),
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
                        decoration: BoxDecoration(
                          color: _getStatusColor(transaction.status),
                          borderRadius: BorderRadius.circular(16),
                        ),
                        child: Text(
                          transaction.status.toString().split('.').last.toUpperCase(),
                          style: const TextStyle(
                            color: Colors.white,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 12),
                  Text('Date: ${transaction.createdAt.toString().substring(0, 19)}'),
                  if (transaction.customerId != null)
                    Text('Customer ID: ${transaction.customerId}'),
                  Text('Type: ${transaction.type.toString().split('.').last}'),
                ],
              ),
            ),
            const SizedBox(height: 20),
            Text(
              'Items (${transaction.items.length})',
              style: Theme.of(context).textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 12),
            Expanded(
              child: ListView.builder(
                itemCount: transaction.items.length,
                itemBuilder: (context, index) {
                  final item = transaction.items[index];
                  return Card(
                    child: ListTile(
                      title: Text(item.displayName),
                      subtitle: Text('Unit Price: \$${(item.unitPrice / 100).toStringAsFixed(2)}'),
                      trailing: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        crossAxisAlignment: CrossAxisAlignment.end,
                        children: [
                          Text('Qty: ${item.quantity}'),
                          Text(
                            '\$${(item.netPrice / 100).toStringAsFixed(2)}',
                            style: const TextStyle(
                              fontWeight: FontWeight.bold,
                              fontSize: 16,
                            ),
                          ),
                        ],
                      ),
                    ),
                  );
                },
              ),
            ),
            const SizedBox(height: 20),
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: Theme.of(context).colorScheme.primaryContainer,
                borderRadius: BorderRadius.circular(12),
              ),
              child: Column(
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text('Subtotal:'),
                      Text('\$${(transaction.subtotal / 100).toStringAsFixed(2)}'),
                    ],
                  ),
                  if (transaction.discountAmount > 0) ...[
                    const SizedBox(height: 4),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        const Text('Discount:', style: TextStyle(color: Colors.red)),
                        Text(
                          '-\$${(transaction.discountAmount / 100).toStringAsFixed(2)}',
                          style: const TextStyle(color: Colors.red),
                        ),
                      ],
                    ),
                  ],
                  if (transaction.taxAmount > 0) ...[
                    const SizedBox(height: 4),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        const Text('Tax:'),
                        Text('\$${(transaction.taxAmount / 100).toStringAsFixed(2)}'),
                      ],
                    ),
                  ],
                  const Divider(),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        'Total:',
                        style: Theme.of(context).textTheme.titleMedium?.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      Text(
                        '\$${(transaction.total / 100).toStringAsFixed(2)}',
                        style: Theme.of(context).textTheme.titleMedium?.copyWith(
                          fontWeight: FontWeight.bold,
                          color: Theme.of(context).colorScheme.primary,
                        ),
                      ),
                    ],
                  ),
                  if (transaction.changeAmount > 0) ...[
                    const SizedBox(height: 4),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        const Text('Change Given:', style: TextStyle(color: Colors.green)),
                        Text(
                          '\$${(transaction.changeAmount / 100).toStringAsFixed(2)}',
                          style: const TextStyle(
                            color: Colors.green,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ],
                    ),
                  ],
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Color _getStatusColor(SalesTransactionStatus status) {
    switch (status) {
      case SalesTransactionStatus.completed:
        return Colors.green;
      case SalesTransactionStatus.hold:
        return Colors.orange;
      case SalesTransactionStatus.active:
        return Colors.blue;
      case SalesTransactionStatus.cancelled:
        return Colors.red;
      case SalesTransactionStatus.refunded:
        return Colors.purple;
      case SalesTransactionStatus.partiallyRefunded:
        return Colors.deepPurple;
      case SalesTransactionStatus.draft:
      default:
        return Colors.grey;
    }
  }
}
