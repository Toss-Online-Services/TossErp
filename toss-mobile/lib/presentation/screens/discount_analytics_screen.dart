import 'package:flutter/material.dart';
import 'package:fl_chart/fl_chart.dart';

import '../../domain/entities/discount_entity.dart';
import '../../data/services/discount_service.dart';
import '../widgets/common/custom_app_bar.dart';
import '../widgets/common/loading_widget.dart';

class DiscountAnalyticsScreen extends StatefulWidget {
  final DiscountEntity discount;

  const DiscountAnalyticsScreen({super.key, required this.discount});

  @override
  State<DiscountAnalyticsScreen> createState() => _DiscountAnalyticsScreenState();
}

class _DiscountAnalyticsScreenState extends State<DiscountAnalyticsScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  final DiscountService _discountService = DiscountService();

  // Analytics data
  List<DiscountUsage> _usageHistory = [];
  Map<String, dynamic> _analyticsData = {};
  bool _isLoading = true;
  
  String _selectedTimeframe = '30d';
  final List<String> _timeframes = ['7d', '30d', '90d', '1y'];

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
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
      // Load usage history
      final usageHistory = await _discountService.getDiscountUsageHistory(widget.discount.id);
      
      // Calculate analytics
      final analytics = _calculateAnalytics(usageHistory);
      
      setState(() {
        _usageHistory = usageHistory;
        _analyticsData = analytics;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
      });
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to load analytics: $e')),
        );
      }
    }
  }

  Map<String, dynamic> _calculateAnalytics(List<DiscountUsage> usageHistory) {
    if (usageHistory.isEmpty) {
      return {
        'totalSavings': 0.0,
        'totalTransactions': 0,
        'averageSavings': 0.0,
        'usageByDay': <String, int>{},
        'usageByHour': <int, int>{},
        'topCustomers': <Map<String, dynamic>>[],
        'conversionRate': 0.0,
        'revenueImpact': 0.0,
      };
    }

    // Filter by timeframe
    final now = DateTime.now();
    final timeframeDays = _getTimeframeDays(_selectedTimeframe);
    final startDate = now.subtract(Duration(days: timeframeDays));
    
    final filteredUsage = usageHistory
        .where((usage) => usage.usedAt.isAfter(startDate))
        .toList();

    // Calculate metrics
    final totalSavings = filteredUsage.fold<double>(
      0.0,
      (sum, usage) => sum + usage.discountAmount,
    );

    final totalTransactions = filteredUsage.length;
    final averageSavings = totalTransactions > 0 ? totalSavings / totalTransactions : 0.0;

    // Usage by day
    final usageByDay = <String, int>{};
    for (final usage in filteredUsage) {
      final day = _formatDate(usage.usedAt);
      usageByDay[day] = (usageByDay[day] ?? 0) + 1;
    }

    // Usage by hour
    final usageByHour = <int, int>{};
    for (final usage in filteredUsage) {
      final hour = usage.usedAt.hour;
      usageByHour[hour] = (usageByHour[hour] ?? 0) + 1;
    }

    // Top customers
    final customerUsage = <String, Map<String, dynamic>>{};
    for (final usage in filteredUsage) {
      if (usage.customerId != null) {
        final customerId = usage.customerId!;
        if (customerUsage.containsKey(customerId)) {
          customerUsage[customerId]!['count'] += 1;
          customerUsage[customerId]!['savings'] += usage.discountAmount;
        } else {
          customerUsage[customerId] = {
            'customerId': customerId,
            'count': 1,
            'savings': usage.discountAmount,
          };
        }
      }
    }

    final topCustomers = (customerUsage.values.toList()
          ..sort((a, b) => (b['count'] as int).compareTo(a['count'] as int)))
        .take(10)
        .toList();

    // Estimated conversion rate and revenue impact
    final conversionRate = _calculateConversionRate(filteredUsage);
    final revenueImpact = _calculateRevenueImpact(filteredUsage);

    return {
      'totalSavings': totalSavings,
      'totalTransactions': totalTransactions,
      'averageSavings': averageSavings,
      'usageByDay': usageByDay,
      'usageByHour': usageByHour,
      'topCustomers': topCustomers,
      'conversionRate': conversionRate,
      'revenueImpact': revenueImpact,
    };
  }

  int _getTimeframeDays(String timeframe) {
    switch (timeframe) {
      case '7d':
        return 7;
      case '30d':
        return 30;
      case '90d':
        return 90;
      case '1y':
        return 365;
      default:
        return 30;
    }
  }

  double _calculateConversionRate(List<DiscountUsage> usageHistory) {
    // This is a simplified calculation
    // In a real implementation, you'd compare with total transactions
    return usageHistory.length * 0.15; // Placeholder calculation
  }

  double _calculateRevenueImpact(List<DiscountUsage> usageHistory) {
    // Estimate revenue generated from discount usage
    return usageHistory.fold<double>(
      0.0,
      (sum, usage) => sum + (usage.discountAmount * 3), // Assuming 3x multiplier
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(
        title: 'Discount Analytics',
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(text: 'Overview'),
            Tab(text: 'Usage Trends'),
            Tab(text: 'Performance'),
          ],
        ),
      ),
      body: _isLoading
          ? const LoadingWidget()
          : Column(
              children: [
                // Timeframe Selector
                Container(
                  padding: const EdgeInsets.all(16),
                  child: Row(
                    children: [
                      const Text('Timeframe: '),
                      const SizedBox(width: 8),
                      Expanded(
                        child: SingleChildScrollView(
                          scrollDirection: Axis.horizontal,
                          child: Row(
                            children: _timeframes.map((timeframe) {
                              final isSelected = _selectedTimeframe == timeframe;
                              return Padding(
                                padding: const EdgeInsets.only(right: 8),
                                child: FilterChip(
                                  label: Text(_getTimeframeLabel(timeframe)),
                                  selected: isSelected,
                                  onSelected: (selected) {
                                    if (selected) {
                                      setState(() {
                                        _selectedTimeframe = timeframe;
                                      });
                                      _loadAnalytics();
                                    }
                                  },
                                ),
                              );
                            }).toList(),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
                
                // Tab Content
                Expanded(
                  child: TabBarView(
                    controller: _tabController,
                    children: [
                      _buildOverviewTab(),
                      _buildTrendsTab(),
                      _buildPerformanceTab(),
                    ],
                  ),
                ),
              ],
            ),
    );
  }

  Widget _buildOverviewTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Key Metrics Cards
          Row(
            children: [
              Expanded(
                child: _buildMetricCard(
                  'Total Savings',
                  'GHS ${_analyticsData['totalSavings'].toStringAsFixed(2)}',
                  Icons.savings,
                  Colors.green,
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: _buildMetricCard(
                  'Transactions',
                  _analyticsData['totalTransactions'].toString(),
                  Icons.receipt,
                  Colors.blue,
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Row(
            children: [
              Expanded(
                child: _buildMetricCard(
                  'Avg. Savings',
                  'GHS ${_analyticsData['averageSavings'].toStringAsFixed(2)}',
                  Icons.trending_up,
                  Colors.orange,
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: _buildMetricCard(
                  'Usage Rate',
                  '${(widget.discount.currentUses / (widget.discount.totalMaxUses ?? 100) * 100).toStringAsFixed(1)}%',
                  Icons.donut_large,
                  Colors.purple,
                ),
              ),
            ],
          ),
          
          const SizedBox(height: 24),
          
          // Discount Information
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Discount Information',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 16),
                  _buildInfoRow('Type', _getDiscountTypeDisplayName(widget.discount.type)),
                  _buildInfoRow('Value', _getDiscountValueText(widget.discount)),
                  _buildInfoRow('Status', widget.discount.isActive ? 'Active' : 'Inactive'),
                  _buildInfoRow('Start Date', _formatDate(widget.discount.startDate)),
                  _buildInfoRow('End Date', _formatDate(widget.discount.endDate)),
                  if (widget.discount.totalMaxUses != null)
                    _buildInfoRow('Usage Limit', widget.discount.totalMaxUses.toString()),
                ],
              ),
            ),
          ),
          
          const SizedBox(height: 16),
          
          // Recent Usage
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Recent Usage',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 16),
                  if (_usageHistory.isEmpty)
                    const Center(
                      child: Padding(
                        padding: EdgeInsets.all(32),
                        child: Text('No usage data available'),
                      ),
                    )
                  else
                    ...(_usageHistory.take(5).map((usage) => ListTile(
                      leading: const CircleAvatar(
                        backgroundColor: Colors.green,
                        child: Icon(Icons.receipt, color: Colors.white),
                      ),
                      title: Text('Transaction #${usage.transactionId}'),
                      subtitle: Text(_formatDateTime(usage.usedAt)),
                      trailing: Text(
                        'GHS ${usage.discountAmount.toStringAsFixed(2)}',
                        style: const TextStyle(fontWeight: FontWeight.bold),
                      ),
                    ))),
                  if (_usageHistory.length > 5)
                    TextButton(
                      onPressed: () {
                        _tabController.animateTo(1);
                      },
                      child: const Text('View All Usage'),
                    ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTrendsTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Usage by Day Chart
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Daily Usage Trend',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 16),
                  SizedBox(
                    height: 200,
                    child: _buildDailyUsageChart(),
                  ),
                ],
              ),
            ),
          ),
          
          const SizedBox(height: 16),
          
          // Usage by Hour Chart
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Hourly Usage Pattern',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 16),
                  SizedBox(
                    height: 200,
                    child: _buildHourlyUsageChart(),
                  ),
                ],
              ),
            ),
          ),
          
          const SizedBox(height: 16),
          
          // Usage Summary
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Usage Summary',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 16),
                  _buildInfoRow('Peak Usage Day', _getPeakUsageDay()),
                  _buildInfoRow('Peak Usage Hour', _getPeakUsageHour()),
                  _buildInfoRow('Average Daily Usage', _getAverageDailyUsage()),
                  _buildInfoRow('Usage Trend', _getUsageTrend()),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildPerformanceTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Performance Metrics
          Row(
            children: [
              Expanded(
                child: _buildMetricCard(
                  'Conversion Rate',
                  '${_analyticsData['conversionRate'].toStringAsFixed(1)}%',
                  Icons.trending_up,
                  Colors.green,
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: _buildMetricCard(
                  'Revenue Impact',
                  'GHS ${_analyticsData['revenueImpact'].toStringAsFixed(2)}',
                  Icons.attach_money,
                  Colors.blue,
                ),
              ),
            ],
          ),
          
          const SizedBox(height: 16),
          
          // Top Customers
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Top Customers',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 16),
                  if (_analyticsData['topCustomers'].isEmpty)
                    const Center(
                      child: Padding(
                        padding: EdgeInsets.all(32),
                        child: Text('No customer data available'),
                      ),
                    )
                  else
                    ...(_analyticsData['topCustomers'].map<Widget>((customer) => ListTile(
                      leading: CircleAvatar(
                        backgroundColor: Colors.blue,
                        child: Text(
                          customer['count'].toString(),
                          style: const TextStyle(color: Colors.white),
                        ),
                      ),
                      title: Text('Customer ${customer['customerId']}'),
                      subtitle: Text('${customer['count']} uses'),
                      trailing: Text(
                        'GHS ${customer['savings'].toStringAsFixed(2)}',
                        style: const TextStyle(fontWeight: FontWeight.bold),
                      ),
                    ))),
                ],
              ),
            ),
          ),
          
          const SizedBox(height: 16),
          
          // Performance Insights
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Performance Insights',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 16),
                  ...(_getPerformanceInsights().map((insight) => Padding(
                    padding: const EdgeInsets.symmetric(vertical: 4),
                    child: Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Icon(Icons.lightbulb_outline, 
                            color: Colors.orange, size: 20),
                        const SizedBox(width: 8),
                        Expanded(child: Text(insight)),
                      ],
                    ),
                  ))),
                ],
              ),
            ),
          ),
          
          const SizedBox(height: 16),
          
          // Recommendations
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Recommendations',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 16),
                  ...(_getRecommendations().map((recommendation) => Padding(
                    padding: const EdgeInsets.symmetric(vertical: 4),
                    child: Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Icon(Icons.recommend, 
                            color: Colors.green, size: 20),
                        const SizedBox(width: 8),
                        Expanded(child: Text(recommendation)),
                      ],
                    ),
                  ))),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildMetricCard(String title, String value, IconData icon, Color color) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(icon, color: color, size: 20),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    title,
                    style: TextStyle(
                      color: Colors.grey[600],
                      fontSize: 12,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Text(
              value,
              style: const TextStyle(
                fontSize: 20,
                fontWeight: FontWeight.bold,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildInfoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 120,
            child: Text(
              label,
              style: TextStyle(
                color: Colors.grey[600],
                fontWeight: FontWeight.w500,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(fontWeight: FontWeight.w500),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildDailyUsageChart() {
    final usageByDay = _analyticsData['usageByDay'] as Map<String, int>;
    
    if (usageByDay.isEmpty) {
      return const Center(child: Text('No data available'));
    }

    final spots = usageByDay.entries
        .map((entry) {
          final date = DateTime.parse(entry.key);
          final daysSinceEpoch = date.difference(DateTime(1970)).inDays.toDouble();
          return FlSpot(daysSinceEpoch, entry.value.toDouble());
        })
        .toList()
      ..sort((a, b) => a.x.compareTo(b.x));

    return LineChart(
      LineChartData(
        gridData: const FlGridData(show: true),
        titlesData: FlTitlesData(
          leftTitles: AxisTitles(
            sideTitles: SideTitles(
              showTitles: true,
              reservedSize: 40,
              getTitlesWidget: (value, meta) {
                return Text(
                  value.toInt().toString(),
                  style: const TextStyle(fontSize: 12),
                );
              },
            ),
          ),
          bottomTitles: AxisTitles(
            sideTitles: SideTitles(
              showTitles: true,
              reservedSize: 30,
              getTitlesWidget: (value, meta) {
                final date = DateTime(1970).add(Duration(days: value.toInt()));
                return Text(
                  '${date.day}/${date.month}',
                  style: const TextStyle(fontSize: 10),
                );
              },
            ),
          ),
          topTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
          rightTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
        ),
        borderData: FlBorderData(show: true),
        lineBarsData: [
          LineChartBarData(
            spots: spots,
            isCurved: true,
            color: Colors.blue,
            barWidth: 3,
            dotData: const FlDotData(show: true),
          ),
        ],
      ),
    );
  }

  Widget _buildHourlyUsageChart() {
    final usageByHour = _analyticsData['usageByHour'] as Map<int, int>;
    
    if (usageByHour.isEmpty) {
      return const Center(child: Text('No data available'));
    }

    final barGroups = List.generate(24, (index) {
      final count = usageByHour[index] ?? 0;
      return BarChartGroupData(
        x: index,
        barRods: [
          BarChartRodData(
            toY: count.toDouble(),
            color: Colors.orange,
            width: 12,
          ),
        ],
      );
    });

    return BarChart(
      BarChartData(
        gridData: const FlGridData(show: true),
        titlesData: FlTitlesData(
          leftTitles: AxisTitles(
            sideTitles: SideTitles(
              showTitles: true,
              reservedSize: 40,
              getTitlesWidget: (value, meta) {
                return Text(
                  value.toInt().toString(),
                  style: const TextStyle(fontSize: 12),
                );
              },
            ),
          ),
          bottomTitles: AxisTitles(
            sideTitles: SideTitles(
              showTitles: true,
              reservedSize: 30,
              getTitlesWidget: (value, meta) {
                return Text(
                  '${value.toInt()}h',
                  style: const TextStyle(fontSize: 10),
                );
              },
            ),
          ),
          topTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
          rightTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
        ),
        borderData: FlBorderData(show: true),
        barGroups: barGroups,
      ),
    );
  }

  String _getTimeframeLabel(String timeframe) {
    switch (timeframe) {
      case '7d':
        return 'Last 7 days';
      case '30d':
        return 'Last 30 days';
      case '90d':
        return 'Last 90 days';
      case '1y':
        return 'Last year';
      default:
        return timeframe;
    }
  }

  String _getPeakUsageDay() {
    final usageByDay = _analyticsData['usageByDay'] as Map<String, int>;
    if (usageByDay.isEmpty) return 'No data';
    
    final peakEntry = usageByDay.entries
        .reduce((a, b) => a.value > b.value ? a : b);
    
    return '${peakEntry.key} (${peakEntry.value} uses)';
  }

  String _getPeakUsageHour() {
    final usageByHour = _analyticsData['usageByHour'] as Map<int, int>;
    if (usageByHour.isEmpty) return 'No data';
    
    final peakEntry = usageByHour.entries
        .reduce((a, b) => a.value > b.value ? a : b);
    
    return '${peakEntry.key}:00 (${peakEntry.value} uses)';
  }

  String _getAverageDailyUsage() {
    final usageByDay = _analyticsData['usageByDay'] as Map<String, int>;
    if (usageByDay.isEmpty) return '0';
    
    final totalUsage = usageByDay.values.fold<int>(0, (sum, usage) => sum + usage);
    final average = totalUsage / usageByDay.length;
    
    return average.toStringAsFixed(1);
  }

  String _getUsageTrend() {
    final usageByDay = _analyticsData['usageByDay'] as Map<String, int>;
    if (usageByDay.length < 2) return 'Insufficient data';
    
    final entries = usageByDay.entries.toList()
      ..sort((a, b) => a.key.compareTo(b.key));
    
    final firstHalf = entries.take(entries.length ~/ 2);
    final secondHalf = entries.skip(entries.length ~/ 2);
    
    final firstAvg = firstHalf.fold<int>(0, (sum, e) => sum + e.value) / firstHalf.length;
    final secondAvg = secondHalf.fold<int>(0, (sum, e) => sum + e.value) / secondHalf.length;
    
    if (secondAvg > firstAvg * 1.1) {
      return 'Increasing';
    } else if (secondAvg < firstAvg * 0.9) {
      return 'Decreasing';
    } else {
      return 'Stable';
    }
  }

  List<String> _getPerformanceInsights() {
    final insights = <String>[];
    final totalTransactions = _analyticsData['totalTransactions'] as int;
    final totalSavings = _analyticsData['totalSavings'] as double;
    
    if (totalTransactions > 100) {
      insights.add('This discount is performing well with high usage.');
    } else if (totalTransactions < 10) {
      insights.add('Low usage detected. Consider promoting this discount more.');
    }
    
    if (totalSavings > 1000) {
      insights.add('Significant customer savings generated.');
    }
    
    final usageByHour = _analyticsData['usageByHour'] as Map<int, int>;
    if (usageByHour.isNotEmpty) {
      final peakHour = usageByHour.entries
          .reduce((a, b) => a.value > b.value ? a : b);
      if (peakHour.key >= 9 && peakHour.key <= 17) {
        insights.add('Peak usage during business hours suggests office worker audience.');
      } else if (peakHour.key >= 18 && peakHour.key <= 22) {
        insights.add('Evening peak usage suggests leisure shoppers.');
      }
    }
    
    return insights.isNotEmpty ? insights : ['No specific insights available.'];
  }

  List<String> _getRecommendations() {
    final recommendations = <String>[];
    final totalTransactions = _analyticsData['totalTransactions'] as int;
    final averageSavings = _analyticsData['averageSavings'] as double;
    
    if (totalTransactions < 50) {
      recommendations.add('Consider increasing promotion of this discount through email or social media.');
    }
    
    if (averageSavings < 10) {
      recommendations.add('Consider increasing the discount value to attract more customers.');
    }
    
    final usageByDay = _analyticsData['usageByDay'] as Map<String, int>;
    if (usageByDay.isNotEmpty) {
      final entries = usageByDay.entries.toList()
        ..sort((a, b) => a.key.compareTo(b.key));
      
      if (entries.length >= 7) {
        final recentUsage = entries.takeLast(3).fold<int>(0, (sum, e) => sum + e.value);
        final earlierUsage = entries.take(3).fold<int>(0, (sum, e) => sum + e.value);
        
        if (recentUsage < earlierUsage * 0.5) {
          recommendations.add('Usage is declining. Consider refreshing the discount or extending the deadline.');
        }
      }
    }
    
    if (widget.discount.endDate.difference(DateTime.now()).inDays < 7) {
      recommendations.add('Discount expires soon. Consider extending or creating a follow-up promotion.');
    }
    
    return recommendations.isNotEmpty ? recommendations : ['No specific recommendations available.'];
  }

  String _formatDate(DateTime date) {
    return '${date.year}-${date.month.toString().padLeft(2, '0')}-${date.day.toString().padLeft(2, '0')}';
  }

  String _formatDateTime(DateTime dateTime) {
    return '${dateTime.day}/${dateTime.month}/${dateTime.year} ${dateTime.hour.toString().padLeft(2, '0')}:${dateTime.minute.toString().padLeft(2, '0')}';
  }

  String _getDiscountValueText(DiscountEntity discount) {
    switch (discount.type) {
      case DiscountType.percentage:
        return '${discount.value.toStringAsFixed(0)}%';
      case DiscountType.fixedAmount:
        return 'GHS ${discount.value.toStringAsFixed(2)}';
      case DiscountType.bogo:
        return 'BOGO';
      case DiscountType.buyXGetY:
        return 'Buy X Get Y';
      default:
        return discount.value.toStringAsFixed(0);
    }
  }

  String _getDiscountTypeDisplayName(DiscountType type) {
    switch (type) {
      case DiscountType.percentage:
        return 'Percentage';
      case DiscountType.fixedAmount:
        return 'Fixed Amount';
      case DiscountType.bogo:
        return 'Buy One Get One';
      case DiscountType.buyXGetY:
        return 'Buy X Get Y';
      case DiscountType.freeShipping:
        return 'Free Shipping';
      case DiscountType.customerSpecific:
        return 'Customer Specific';
      case DiscountType.loyaltyPoints:
        return 'Loyalty Points';
    }
  }
}

extension<T> on Iterable<T> {
  Iterable<T> takeLast(int count) {
    return skip(length - count);
  }
}
