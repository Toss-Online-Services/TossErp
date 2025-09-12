import 'package:flutter/material.dart';

class SimpleAnalyticsScreen extends StatefulWidget {
  const SimpleAnalyticsScreen({super.key});

  @override
  State<SimpleAnalyticsScreen> createState() => _SimpleAnalyticsScreenState();
}

class _SimpleAnalyticsScreenState extends State<SimpleAnalyticsScreen> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Analytics & Reports'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.analytics,
              size: 80,
              color: Colors.purple,
            ),
            SizedBox(height: 16),
            Text(
              'Analytics & Reports',
              style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
              ),
            ),
            SizedBox(height: 8),
            Text(
              'Comprehensive business analytics,\nreports, and insights available here.',
              textAlign: TextAlign.center,
              style: TextStyle(
                fontSize: 16,
                color: Colors.grey,
              ),
            ),
            SizedBox(height: 32),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                Column(
                  children: [
                    Icon(Icons.trending_up, size: 40, color: Colors.green),
                    SizedBox(height: 8),
                    Text('Sales Reports'),
                  ],
                ),
                Column(
                  children: [
                    Icon(Icons.pie_chart, size: 40, color: Colors.blue),
                    SizedBox(height: 8),
                    Text('Product Analytics'),
                  ],
                ),
                Column(
                  children: [
                    Icon(Icons.bar_chart, size: 40, color: Colors.orange),
                    SizedBox(height: 8),
                    Text('Performance'),
                  ],
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
