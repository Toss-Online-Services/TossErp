import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

class ReportsScreen extends ConsumerWidget {
  const ReportsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Reports & Analytics'),
        actions: [
          IconButton(
            icon: const Icon(Icons.download),
            onPressed: () {
              // TODO: Export reports
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(content: Text('Export coming soon!')),
              );
            },
          ),
          IconButton(
            icon: const Icon(Icons.settings),
            onPressed: () {
              // TODO: Report settings
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(content: Text('Settings coming soon!')),
              );
            },
          ),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          children: [
            // Date Range Selector
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Row(
                  children: [
                    const Icon(Icons.date_range),
                    const SizedBox(width: 8),
                    const Text('Date Range:'),
                    const SizedBox(width: 8),
                    Expanded(
                      child: DropdownButton<String>(
                        value: 'Last 30 Days',
                        isExpanded: true,
                        items: const [
                          DropdownMenuItem(value: 'Today', child: Text('Today')),
                          DropdownMenuItem(value: 'Last 7 Days', child: Text('Last 7 Days')),
                          DropdownMenuItem(value: 'Last 30 Days', child: Text('Last 30 Days')),
                          DropdownMenuItem(value: 'Last 90 Days', child: Text('Last 90 Days')),
                          DropdownMenuItem(value: 'This Year', child: Text('This Year')),
                        ],
                        onChanged: (value) {
                          // TODO: Update date range
                        },
                      ),
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 24),
            
            // Key Metrics
            Row(
              children: [
                Expanded(
                  child: _buildMetricCard(
                    context,
                    title: 'Total Sales',
                    value: '\$12,345',
                    change: '+15.2%',
                    isPositive: true,
                    icon: Icons.attach_money,
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: _buildMetricCard(
                    context,
                    title: 'Items Sold',
                    value: '234',
                    change: '+8.7%',
                    isPositive: true,
                    icon: Icons.shopping_cart,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            
            Row(
              children: [
                Expanded(
                  child: _buildMetricCard(
                    context,
                    title: 'Avg. Order',
                    value: '\$52.76',
                    change: '+2.1%',
                    isPositive: true,
                    icon: Icons.receipt,
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: _buildMetricCard(
                    context,
                    title: 'Customers',
                    value: '89',
                    change: '-3.2%',
                    isPositive: false,
                    icon: Icons.people,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 24),
            
            // Report Categories
            Expanded(
              child: GridView.count(
                crossAxisCount: 2,
                crossAxisSpacing: 16,
                mainAxisSpacing: 16,
                children: [
                  _buildReportCard(
                    context,
                    icon: Icons.trending_up,
                    title: 'Sales Report',
                    subtitle: 'Revenue and trends',
                    color: Colors.blue,
                    onTap: () {
                      // TODO: Show sales report
                    },
                  ),
                  _buildReportCard(
                    context,
                    icon: Icons.inventory,
                    title: 'Stock Report',
                    subtitle: 'Inventory levels',
                    color: Colors.green,
                    onTap: () {
                      // TODO: Show stock report
                    },
                  ),
                  _buildReportCard(
                    context,
                    icon: Icons.people,
                    title: 'Customer Report',
                    subtitle: 'Customer analytics',
                    color: Colors.purple,
                    onTap: () {
                      // TODO: Show customer report
                    },
                  ),
                  _buildReportCard(
                    context,
                    icon: Icons.analytics,
                    title: 'Performance',
                    subtitle: 'Business metrics',
                    color: Colors.orange,
                    onTap: () {
                      // TODO: Show performance report
                    },
                  ),
                  _buildReportCard(
                    context,
                    icon: Icons.warning,
                    title: 'Low Stock',
                    subtitle: 'Reorder alerts',
                    color: Colors.red,
                    onTap: () {
                      // TODO: Show low stock report
                    },
                  ),
                  _buildReportCard(
                    context,
                    icon: Icons.schedule,
                    title: 'Slow Moving',
                    subtitle: 'Inventory analysis',
                    color: Colors.teal,
                    onTap: () {
                      // TODO: Show slow moving report
                    },
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildMetricCard(
    BuildContext context, {
    required String title,
    required String value,
    required String change,
    required bool isPositive,
    required IconData icon,
  }) {
    return Card(
      elevation: 2,
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(
                  icon,
                  color: Colors.grey[600],
                ),
                const Spacer(),
                Icon(
                  isPositive ? Icons.trending_up : Icons.trending_down,
                  color: isPositive ? Colors.green : Colors.red,
                  size: 16,
                ),
                Text(
                  change,
                  style: TextStyle(
                    color: isPositive ? Colors.green : Colors.red,
                    fontSize: 12,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Text(
              value,
              style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              title,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: Colors.grey[600],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildReportCard(
    BuildContext context, {
    required IconData icon,
    required String title,
    required String subtitle,
    required Color color,
    required VoidCallback onTap,
  }) {
    return Card(
      elevation: 2,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(
                icon,
                size: 48,
                color: color,
              ),
              const SizedBox(height: 16),
              Text(
                title,
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.w600,
                ),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 8),
              Text(
                subtitle,
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                  color: Colors.grey[600],
                ),
                textAlign: TextAlign.center,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
