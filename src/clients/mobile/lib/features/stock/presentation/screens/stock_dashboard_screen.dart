import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

class StockDashboardScreen extends ConsumerWidget {
  const StockDashboardScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Stock Management'),
        actions: [
          IconButton(
            icon: const Icon(Icons.add),
            onPressed: () {
              // TODO: Add new item
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(content: Text('Add item coming soon!')),
              );
            },
          ),
          IconButton(
            icon: const Icon(Icons.filter_list),
            onPressed: () {
              // TODO: Filter items
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(content: Text('Filter coming soon!')),
              );
            },
          ),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          children: [
            // Search Bar
            TextField(
              decoration: InputDecoration(
                hintText: 'Search items by name or SKU...',
                prefixIcon: const Icon(Icons.search),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
              ),
            ),
            const SizedBox(height: 24),
            
            // Stock Overview Cards
            Row(
              children: [
                Expanded(
                  child: _buildOverviewCard(
                    context,
                    title: 'Total Items',
                    value: '1,234',
                    icon: Icons.inventory,
                    color: Colors.blue,
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: _buildOverviewCard(
                    context,
                    title: 'Low Stock',
                    value: '23',
                    icon: Icons.warning,
                    color: Colors.orange,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            
            Row(
              children: [
                Expanded(
                  child: _buildOverviewCard(
                    context,
                    title: 'Out of Stock',
                    value: '5',
                    icon: Icons.error,
                    color: Colors.red,
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: _buildOverviewCard(
                    context,
                    title: 'Total Value',
                    value: '\$45,678',
                    icon: Icons.attach_money,
                    color: Colors.green,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 24),
            
            // Quick Actions
            Row(
              children: [
                Expanded(
                  child: _buildActionButton(
                    context,
                    icon: Icons.add_box,
                    label: 'Add Item',
                    onTap: () {
                      // TODO: Add new item
                    },
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: _buildActionButton(
                    context,
                    icon: Icons.input,
                    label: 'Stock In',
                    onTap: () {
                      // TODO: Stock in
                    },
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: _buildActionButton(
                    context,
                    icon: Icons.output,
                    label: 'Stock Out',
                    onTap: () {
                      // TODO: Stock out
                    },
                  ),
                ),
              ],
            ),
            const SizedBox(height: 24),
            
            // Recent Stock Movements
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Recent Stock Movements',
                    style: Theme.of(context).textTheme.titleLarge?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 16),
                  Expanded(
                    child: ListView.builder(
                      itemCount: 10,
                      itemBuilder: (context, index) {
                        return _buildMovementCard(context, index);
                      },
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

  Widget _buildOverviewCard(
    BuildContext context, {
    required String title,
    required String value,
    required IconData icon,
    required Color color,
  }) {
    return Card(
      elevation: 2,
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          children: [
            Icon(
              icon,
              size: 32,
              color: color,
            ),
            const SizedBox(height: 8),
            Text(
              value,
              style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                fontWeight: FontWeight.bold,
                color: color,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              title,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: Colors.grey[600],
              ),
              textAlign: TextAlign.center,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildActionButton(
    BuildContext context, {
    required IconData icon,
    required String label,
    required VoidCallback onTap,
  }) {
    return ElevatedButton.icon(
      onPressed: onTap,
      icon: Icon(icon),
      label: Text(label),
      style: ElevatedButton.styleFrom(
        padding: const EdgeInsets.symmetric(vertical: 12),
      ),
    );
  }

  Widget _buildMovementCard(BuildContext context, int index) {
    final movements = [
      {'item': 'Laptop', 'type': 'IN', 'quantity': '5', 'date': '2024-01-15'},
      {'item': 'Smartphone', 'type': 'OUT', 'quantity': '2', 'date': '2024-01-15'},
      {'item': 'Headphones', 'type': 'IN', 'quantity': '10', 'date': '2024-01-14'},
      {'item': 'Tablet', 'type': 'OUT', 'quantity': '1', 'date': '2024-01-14'},
      {'item': 'Keyboard', 'type': 'IN', 'quantity': '15', 'date': '2024-01-13'},
      {'item': 'Mouse', 'type': 'OUT', 'quantity': '3', 'date': '2024-01-13'},
      {'item': 'Monitor', 'type': 'IN', 'quantity': '8', 'date': '2024-01-12'},
      {'item': 'Speaker', 'type': 'OUT', 'quantity': '2', 'date': '2024-01-12'},
      {'item': 'Webcam', 'type': 'IN', 'quantity': '12', 'date': '2024-01-11'},
      {'item': 'Microphone', 'type': 'OUT', 'quantity': '1', 'date': '2024-01-11'},
    ];
    
    final movement = movements[index];
    final isIn = movement['type'] == 'IN';
    
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: isIn ? Colors.green[100] : Colors.red[100],
          child: Icon(
            isIn ? Icons.input : Icons.output,
            color: isIn ? Colors.green[700] : Colors.red[700],
          ),
        ),
        title: Text(
          movement['item']!,
          style: const TextStyle(fontWeight: FontWeight.w600),
        ),
        subtitle: Text('Date: ${movement['date']}'),
        trailing: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          crossAxisAlignment: CrossAxisAlignment.end,
          children: [
            Text(
              '${movement['type']} ${movement['quantity']}',
              style: TextStyle(
                fontWeight: FontWeight.bold,
                color: isIn ? Colors.green[700] : Colors.red[700],
              ),
            ),
            Text(
              isIn ? 'Stock In' : 'Stock Out',
              style: TextStyle(
                fontSize: 12,
                color: isIn ? Colors.green[600] : Colors.red[600],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
