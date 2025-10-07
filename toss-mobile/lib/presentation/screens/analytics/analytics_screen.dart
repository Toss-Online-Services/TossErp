import 'package:flutter/material.dart';
import 'package:fl_chart/fl_chart.dart';

import '../../../domain/entities/analytics_entity.dart';
import '../../../core/services/analytics_service.dart';

class AnalyticsScreen extends StatefulWidget {
  const AnalyticsScreen({super.key});

  @override
  State<AnalyticsScreen> createState() => _AnalyticsScreenState();
}

class _AnalyticsScreenState extends State<AnalyticsScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  final _analyticsService = AnalyticsService();

  // State
  ReportPeriod _selectedPeriod = ReportPeriod.thisWeek;
  DateRange _dateRange = DateRange.forPeriod(ReportPeriod.thisWeek);
  bool _isLoading = false;

  // Analytics data
  SalesAnalytics? _salesAnalytics;
  List<ProductAnalytics> _productAnalytics = [];
  InventoryAnalytics? _inventoryAnalytics;
  CustomerAnalytics? _customerAnalytics;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 4, vsync: this);
    _loadAnalytics();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  Future<void> _loadAnalytics() async {
    setState(() {
      _isLoading = true;
    });

    try {
      final futures = await Future.wait([
        _analyticsService.generateSalesAnalytics(_dateRange),
        _analyticsService.generateProductAnalytics(_dateRange),
        _analyticsService.generateInventoryAnalytics(_dateRange),
        _analyticsService.generateCustomerAnalytics(_dateRange),
      ]);

      setState(() {
        _salesAnalytics = futures[0] as SalesAnalytics;
        _productAnalytics = futures[1] as List<ProductAnalytics>;
        _inventoryAnalytics = futures[2] as InventoryAnalytics;
        _customerAnalytics = futures[3] as CustomerAnalytics;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
      });
      _showError('Failed to load analytics: $e');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Analytics & Reports'),
        actions: [
          IconButton(
            onPressed: _showDateRangePicker,
            icon: const Icon(Icons.date_range),
            tooltip: 'Select Date Range',
          ),
          IconButton(
            onPressed: _showExportDialog,
            icon: const Icon(Icons.download),
            tooltip: 'Export Report',
          ),
          PopupMenuButton<String>(
            onSelected: _handleMenuAction,
            itemBuilder: (context) => [
              const PopupMenuItem(
                value: 'refresh',
                child: ListTile(
                  leading: Icon(Icons.refresh),
                  title: Text('Refresh Data'),
                  contentPadding: EdgeInsets.zero,
                ),
              ),
              const PopupMenuItem(
                value: 'schedule',
                child: ListTile(
                  leading: Icon(Icons.schedule),
                  title: Text('Schedule Report'),
                  contentPadding: EdgeInsets.zero,
                ),
              ),
              const PopupMenuItem(
                value: 'compare',
                child: ListTile(
                  leading: Icon(Icons.compare),
                  title: Text('Compare Periods'),
                  contentPadding: EdgeInsets.zero,
                ),
              ),
            ],
          ),
        ],
        bottom: PreferredSize(
          preferredSize: const Size.fromHeight(100),
          child: Column(
            children: [
              _buildPeriodSelector(),
              TabBar(
                controller: _tabController,
                isScrollable: true,
                tabs: const [
                  Tab(icon: Icon(Icons.trending_up), text: 'Sales'),
                  Tab(icon: Icon(Icons.inventory), text: 'Products'),
                  Tab(icon: Icon(Icons.warehouse), text: 'Inventory'),
                  Tab(icon: Icon(Icons.people), text: 'Customers'),
                ],
              ),
            ],
          ),
        ),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : TabBarView(
              controller: _tabController,
              children: [
                _buildSalesTab(),
                _buildProductsTab(),
                _buildInventoryTab(),
                _buildCustomersTab(),
              ],
            ),
    );
  }

  Widget _buildPeriodSelector() {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: SingleChildScrollView(
        scrollDirection: Axis.horizontal,
        child: Row(
          children: ReportPeriod.values
              .where((period) => period != ReportPeriod.custom)
              .map((period) => Padding(
                    padding: const EdgeInsets.only(right: 8),
                    child: ChoiceChip(
                      label: Text(_getPeriodName(period)),
                      selected: _selectedPeriod == period,
                      onSelected: (selected) {
                        if (selected) {
                          setState(() {
                            _selectedPeriod = period;
                            _dateRange = DateRange.forPeriod(period);
                          });
                          _loadAnalytics();
                        }
                      },
                    ),
                  ))
              .toList(),
        ),
      ),
    );
  }

  Widget _buildSalesTab() {
    if (_salesAnalytics == null) {
      return const Center(child: Text('No sales data available'));
    }

    final analytics = _salesAnalytics!;

    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildSalesKPIs(analytics),
          const SizedBox(height: 24),
          _buildSalesChart(analytics),
          const SizedBox(height: 24),
          _buildSalesBreakdown(analytics),
          const SizedBox(height: 24),
          _buildPaymentMethodChart(analytics),
        ],
      ),
    );
  }

  Widget _buildSalesKPIs(SalesAnalytics analytics) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Sales Overview',
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 16),
        Row(
          children: [
            Expanded(
              child: _buildKPICard(
                'Total Sales',
                '\$${analytics.totalSales.toStringAsFixed(2)}',
                Icons.attach_money,
                Colors.green,
                subtitle: '${analytics.transactionCount} transactions',
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: _buildKPICard(
                'Net Sales',
                '\$${analytics.netSales.toStringAsFixed(2)}',
                Icons.trending_up,
                Colors.blue,
                subtitle: 'After discounts',
              ),
            ),
          ],
        ),
        const SizedBox(height: 16),
        Row(
          children: [
            Expanded(
              child: _buildKPICard(
                'Avg Transaction',
                '\$${analytics.averageTransactionValue.toStringAsFixed(2)}',
                Icons.receipt,
                Colors.orange,
                subtitle: '${analytics.itemCount} items sold',
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: _buildKPICard(
                'Returns',
                '\$${analytics.returnsAmount.toStringAsFixed(2)}',
                Icons.keyboard_return,
                Colors.red,
                subtitle: '${analytics.returnsCount} returns',
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildKPICard(
    String title,
    String value,
    IconData icon,
    Color color, {
    String? subtitle,
  }) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: color.withOpacity(0.3)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(icon, color: color, size: 24),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  title,
                  style: Theme.of(context).textTheme.titleSmall?.copyWith(
                    color: color,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Text(
            value,
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
              fontWeight: FontWeight.bold,
              color: color,
            ),
          ),
          if (subtitle != null) ...[
            const SizedBox(height: 4),
            Text(
              subtitle,
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                color: Colors.grey[600],
              ),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildSalesChart(SalesAnalytics analytics) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Sales by Hour',
          style: Theme.of(context).textTheme.titleLarge?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 16),
        Container(
          height: 250,
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: Theme.of(context).colorScheme.surface,
            borderRadius: BorderRadius.circular(12),
            border: Border.all(
              color: Theme.of(context).colorScheme.outline.withOpacity(0.2),
            ),
          ),
          child: LineChart(
            LineChartData(
              gridData: const FlGridData(show: true),
              titlesData: FlTitlesData(
                leftTitles: AxisTitles(
                  sideTitles: SideTitles(
                    showTitles: true,
                    reservedSize: 40,
                    getTitlesWidget: (value, meta) {
                      return Text(
                        '\$${value.toInt()}',
                        style: const TextStyle(fontSize: 10),
                      );
                    },
                  ),
                ),
                bottomTitles: AxisTitles(
                  sideTitles: SideTitles(
                    showTitles: true,
                    reservedSize: 20,
                    getTitlesWidget: (value, meta) {
                      return Text(
                        '${value.toInt()}h',
                        style: const TextStyle(fontSize: 10),
                      );
                    },
                  ),
                ),
                rightTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
                topTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
              ),
              borderData: FlBorderData(show: true),
              lineBarsData: [
                LineChartBarData(
                  spots: analytics.hourlySales
                      .map((data) => FlSpot(data.hour.toDouble(), data.sales))
                      .toList(),
                  isCurved: true,
                  color: Theme.of(context).colorScheme.primary,
                  barWidth: 3,
                  dotData: const FlDotData(show: true),
                  belowBarData: BarAreaData(
                    show: true,
                    color: Theme.of(context).colorScheme.primary.withOpacity(0.1),
                  ),
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildSalesBreakdown(SalesAnalytics analytics) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Sales Breakdown',
          style: Theme.of(context).textTheme.titleLarge?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 16),
        if (analytics.salesByEmployee.isNotEmpty)
          _buildBreakdownSection('By Employee', analytics.salesByEmployee),
        const SizedBox(height: 16),
        if (analytics.salesByCategory.isNotEmpty)
          _buildBreakdownSection('By Category', analytics.salesByCategory),
      ],
    );
  }

  Widget _buildBreakdownSection(String title, Map<String, double> data) {
    final sortedEntries = data.entries.toList()
      ..sort((a, b) => b.value.compareTo(a.value));

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceVariant.withOpacity(0.3),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),
          ...sortedEntries.take(5).map((entry) => Padding(
                padding: const EdgeInsets.symmetric(vertical: 4),
                child: Row(
                  children: [
                    Expanded(
                      child: Text(entry.key),
                    ),
                    Text(
                      '\$${entry.value.toStringAsFixed(2)}',
                      style: const TextStyle(fontWeight: FontWeight.bold),
                    ),
                  ],
                ),
              )),
        ],
      ),
    );
  }

  Widget _buildPaymentMethodChart(SalesAnalytics analytics) {
    if (analytics.salesByPaymentMethod.isEmpty) return const SizedBox.shrink();

    final sections = analytics.salesByPaymentMethod.entries
        .map((entry) => PieChartSectionData(
              value: entry.value,
              title: '${((entry.value / analytics.totalSales) * 100).toStringAsFixed(1)}%',
              radius: 50,
              titleStyle: const TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.bold,
                color: Colors.white,
              ),
            ))
        .toList();

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Payment Methods',
          style: Theme.of(context).textTheme.titleLarge?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 16),
        Container(
          height: 200,
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: Theme.of(context).colorScheme.surface,
            borderRadius: BorderRadius.circular(12),
            border: Border.all(
              color: Theme.of(context).colorScheme.outline.withOpacity(0.2),
            ),
          ),
          child: Row(
            children: [
              Expanded(
                flex: 2,
                child: PieChart(
                  PieChartData(
                    sections: sections,
                    centerSpaceRadius: 40,
                    sectionsSpace: 2,
                  ),
                ),
              ),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: analytics.salesByPaymentMethod.entries
                      .map((entry) => Padding(
                            padding: const EdgeInsets.symmetric(vertical: 4),
                            child: Row(
                              children: [
                                Container(
                                  width: 12,
                                  height: 12,
                                  decoration: const BoxDecoration(
                                    color: Colors.blue,
                                    shape: BoxShape.circle,
                                  ),
                                ),
                                const SizedBox(width: 8),
                                Expanded(
                                  child: Text(
                                    entry.key,
                                    style: const TextStyle(fontSize: 12),
                                  ),
                                ),
                              ],
                            ),
                          ))
                      .toList(),
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildProductsTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Top Products',
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          ListView.builder(
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            itemCount: _productAnalytics.take(20).length,
            itemBuilder: (context, index) {
              return _buildProductCard(_productAnalytics[index]);
            },
          ),
        ],
      ),
    );
  }

  Widget _buildProductCard(ProductAnalytics product) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        product.productName,
                        style: Theme.of(context).textTheme.titleMedium?.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      Text(
                        product.categoryName,
                        style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                          color: Colors.grey[600],
                        ),
                      ),
                    ],
                  ),
                ),
                Column(
                  crossAxisAlignment: CrossAxisAlignment.end,
                  children: [
                    Text(
                      '\$${product.totalSales.toStringAsFixed(2)}',
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.bold,
                        color: Theme.of(context).colorScheme.primary,
                      ),
                    ),
                    Text(
                      '${product.quantitySold} sold',
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: Colors.grey[600],
                      ),
                    ),
                  ],
                ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                Expanded(
                  child: _buildProductMetric(
                    'Profit Margin',
                    '${(product.profitMargin * 100).toStringAsFixed(1)}%',
                    product.profitMargin > 0.3 ? Colors.green : Colors.orange,
                  ),
                ),
                Expanded(
                  child: _buildProductMetric(
                    'Avg Price',
                    '\$${product.averageSellingPrice.toStringAsFixed(2)}',
                    Colors.blue,
                  ),
                ),
                Expanded(
                  child: _buildProductMetric(
                    'Stock',
                    '${product.currentStock}',
                    product.currentStock > 10 ? Colors.green : Colors.red,
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildProductMetric(String label, String value, Color color) {
    return Column(
      children: [
        Text(
          label,
          style: TextStyle(
            color: Colors.grey[600],
            fontSize: 12,
          ),
        ),
        Text(
          value,
          style: TextStyle(
            color: color,
            fontWeight: FontWeight.bold,
          ),
        ),
      ],
    );
  }

  Widget _buildInventoryTab() {
    if (_inventoryAnalytics == null) {
      return const Center(child: Text('No inventory data available'));
    }

    final analytics = _inventoryAnalytics!;

    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildInventoryKPIs(analytics),
          const SizedBox(height: 24),
          _buildStockHealthChart(analytics),
          const SizedBox(height: 24),
          _buildCategoryBreakdown(analytics),
        ],
      ),
    );
  }

  Widget _buildInventoryKPIs(InventoryAnalytics analytics) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Inventory Overview',
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 16),
        Row(
          children: [
            Expanded(
              child: _buildKPICard(
                'Total Products',
                '${analytics.totalProducts}',
                Icons.inventory,
                Colors.blue,
                subtitle: '${analytics.totalStock} units',
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: _buildKPICard(
                'Total Value',
                '\$${analytics.totalValue.toStringAsFixed(2)}',
                Icons.attach_money,
                Colors.green,
                subtitle: 'Inventory worth',
              ),
            ),
          ],
        ),
        const SizedBox(height: 16),
        Row(
          children: [
            Expanded(
              child: _buildKPICard(
                'Low Stock',
                '${analytics.lowStockItems}',
                Icons.warning,
                Colors.orange,
                subtitle: 'Need attention',
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: _buildKPICard(
                'Out of Stock',
                '${analytics.outOfStockItems}',
                Icons.error,
                Colors.red,
                subtitle: 'Require restock',
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildStockHealthChart(InventoryAnalytics analytics) {
    final healthScore = analytics.stockHealthScore * 100;
    
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Stock Health Score',
          style: Theme.of(context).textTheme.titleLarge?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 16),
        Container(
          padding: const EdgeInsets.all(24),
          decoration: BoxDecoration(
            color: Theme.of(context).colorScheme.surface,
            borderRadius: BorderRadius.circular(12),
            border: Border.all(
              color: Theme.of(context).colorScheme.outline.withOpacity(0.2),
            ),
          ),
          child: Column(
            children: [
              SizedBox(
                width: 150,
                height: 150,
                child: CircularProgressIndicator(
                  value: analytics.stockHealthScore,
                  strokeWidth: 12,
                  backgroundColor: Colors.grey[300],
                  valueColor: AlwaysStoppedAnimation<Color>(
                    healthScore > 80 ? Colors.green :
                    healthScore > 60 ? Colors.orange : Colors.red,
                  ),
                ),
              ),
              const SizedBox(height: 16),
              Text(
                '${healthScore.toStringAsFixed(1)}%',
                style: Theme.of(context).textTheme.headlineLarge?.copyWith(
                  fontWeight: FontWeight.bold,
                  color: healthScore > 80 ? Colors.green :
                         healthScore > 60 ? Colors.orange : Colors.red,
                ),
              ),
              Text(
                'Stock Health Score',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  color: Colors.grey[600],
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildCategoryBreakdown(InventoryAnalytics analytics) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Inventory by Category',
          style: Theme.of(context).textTheme.titleLarge?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 16),
        _buildBreakdownSection('Stock Distribution', 
            analytics.stockByCategory.map((key, value) => MapEntry(key, value.toDouble()))),
        const SizedBox(height: 16),
        _buildBreakdownSection('Value Distribution', analytics.valueByCategory),
      ],
    );
  }

  Widget _buildCustomersTab() {
    if (_customerAnalytics == null) {
      return const Center(child: Text('No customer data available'));
    }

    final analytics = _customerAnalytics!;

    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildCustomerKPIs(analytics),
          const SizedBox(height: 24),
          _buildLoyaltyTierChart(analytics),
          const SizedBox(height: 24),
          _buildTopCustomers(analytics),
        ],
      ),
    );
  }

  Widget _buildCustomerKPIs(CustomerAnalytics analytics) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Customer Overview',
          style: Theme.of(context).textTheme.headlineSmall?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 16),
        Row(
          children: [
            Expanded(
              child: _buildKPICard(
                'Total Customers',
                '${analytics.totalCustomers}',
                Icons.people,
                Colors.blue,
                subtitle: '${analytics.newCustomers} new',
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: _buildKPICard(
                'Average Spend',
                '\$${analytics.averageSpend.toStringAsFixed(2)}',
                Icons.attach_money,
                Colors.green,
                subtitle: 'Per customer',
              ),
            ),
          ],
        ),
        const SizedBox(height: 16),
        Row(
          children: [
            Expanded(
              child: _buildKPICard(
                'Retention Rate',
                '${(analytics.customerRetentionRate * 100).toStringAsFixed(1)}%',
                Icons.repeat,
                Colors.purple,
                subtitle: 'Customer retention',
              ),
            ),
            const SizedBox(width: 16),
            Expanded(
              child: _buildKPICard(
                'Avg Transactions',
                '${analytics.averageTransactionsPerCustomer}',
                Icons.receipt_long,
                Colors.orange,
                subtitle: 'Per customer',
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildLoyaltyTierChart(CustomerAnalytics analytics) {
    if (analytics.customersByTier.isEmpty) return const SizedBox.shrink();

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Customer Loyalty Tiers',
          style: Theme.of(context).textTheme.titleLarge?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 16),
        ...analytics.customersByTier.entries.map((entry) => Container(
              margin: const EdgeInsets.only(bottom: 8),
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: Theme.of(context).colorScheme.surfaceVariant.withOpacity(0.3),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Row(
                children: [
                  Expanded(
                    child: Text(
                      entry.key,
                      style: const TextStyle(fontWeight: FontWeight.w500),
                    ),
                  ),
                  Text(
                    '${entry.value} customers',
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                ],
              ),
            )),
      ],
    );
  }

  Widget _buildTopCustomers(CustomerAnalytics analytics) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Top Customers',
          style: Theme.of(context).textTheme.titleLarge?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 16),
        ListView.builder(
          shrinkWrap: true,
          physics: const NeverScrollableScrollPhysics(),
          itemCount: analytics.topSpenders.length,
          itemBuilder: (context, index) {
            final customer = analytics.topSpenders[index];
            return Card(
              margin: const EdgeInsets.only(bottom: 8),
              child: ListTile(
                leading: CircleAvatar(
                  child: Text('${index + 1}'),
                ),
                title: Text(customer.customerName),
                subtitle: Text('${customer.transactionCount} transactions'),
                trailing: Text(
                  '\$${customer.totalSpend.toStringAsFixed(2)}',
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    fontSize: 16,
                  ),
                ),
              ),
            );
          },
        ),
      ],
    );
  }

  void _showDateRangePicker() async {
    final DateTimeRange? picked = await showDateRangePicker(
      context: context,
      firstDate: DateTime.now().subtract(const Duration(days: 365)),
      lastDate: DateTime.now(),
      initialDateRange: DateTimeRange(
        start: _dateRange.startDate,
        end: _dateRange.endDate,
      ),
    );

    if (picked != null) {
      setState(() {
        _selectedPeriod = ReportPeriod.custom;
        _dateRange = DateRange(
          startDate: picked.start,
          endDate: picked.end,
        );
      });
      _loadAnalytics();
    }
  }

  void _showExportDialog() {
    showModalBottomSheet(
      context: context,
      builder: (context) => Container(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Export Report',
              style: Theme.of(context).textTheme.titleLarge?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            ListTile(
              leading: const Icon(Icons.picture_as_pdf),
              title: const Text('Export as PDF'),
              onTap: () {
                Navigator.of(context).pop();
                _exportReport(ExportFormat.pdf);
              },
            ),
            ListTile(
              leading: const Icon(Icons.table_chart),
              title: const Text('Export as CSV'),
              onTap: () {
                Navigator.of(context).pop();
                _exportReport(ExportFormat.csv);
              },
            ),
            ListTile(
              leading: const Icon(Icons.grid_on),
              title: const Text('Export as Excel'),
              onTap: () {
                Navigator.of(context).pop();
                _exportReport(ExportFormat.xlsx);
              },
            ),
          ],
        ),
      ),
    );
  }

  void _handleMenuAction(String action) {
    switch (action) {
      case 'refresh':
        _loadAnalytics();
        break;
      case 'schedule':
        _showScheduleDialog();
        break;
      case 'compare':
        _showCompareDialog();
        break;
    }
  }

  void _showScheduleDialog() {
    // TODO: Implement scheduled reports
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Scheduled reports feature coming soon')),
    );
  }

  void _showCompareDialog() {
    // TODO: Implement period comparison
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Period comparison feature coming soon')),
    );
  }

  void _exportReport(ExportFormat format) {
    // TODO: Implement actual export functionality
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Exporting report as ${format.name.toUpperCase()}...')),
    );
  }

  void _showError(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: Colors.red,
      ),
    );
  }

  String _getPeriodName(ReportPeriod period) {
    switch (period) {
      case ReportPeriod.today:
        return 'Today';
      case ReportPeriod.yesterday:
        return 'Yesterday';
      case ReportPeriod.thisWeek:
        return 'This Week';
      case ReportPeriod.lastWeek:
        return 'Last Week';
      case ReportPeriod.thisMonth:
        return 'This Month';
      case ReportPeriod.lastMonth:
        return 'Last Month';
      case ReportPeriod.thisQuarter:
        return 'This Quarter';
      case ReportPeriod.lastQuarter:
        return 'Last Quarter';
      case ReportPeriod.thisYear:
        return 'This Year';
      case ReportPeriod.lastYear:
        return 'Last Year';
      case ReportPeriod.custom:
        return 'Custom';
    }
  }
}
