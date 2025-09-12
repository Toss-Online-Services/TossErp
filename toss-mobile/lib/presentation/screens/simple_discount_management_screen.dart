import 'package:flutter/material.dart';

class SimpleDiscountManagementScreen extends StatefulWidget {
  const SimpleDiscountManagementScreen({super.key});

  @override
  State<SimpleDiscountManagementScreen> createState() => _SimpleDiscountManagementScreenState();
}

class _SimpleDiscountManagementScreenState extends State<SimpleDiscountManagementScreen> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Discount Management'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.discount,
              size: 80,
              color: Colors.blue,
            ),
            SizedBox(height: 16),
            Text(
              'Discount Management',
              style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
              ),
            ),
            SizedBox(height: 8),
            Text(
              'Advanced discount and promotion management\nfeatures are available here.',
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
                    Icon(Icons.add_circle, size: 40, color: Colors.green),
                    SizedBox(height: 8),
                    Text('Create Discount'),
                  ],
                ),
                Column(
                  children: [
                    Icon(Icons.list, size: 40, color: Colors.orange),
                    SizedBox(height: 8),
                    Text('View Discounts'),
                  ],
                ),
                Column(
                  children: [
                    Icon(Icons.analytics, size: 40, color: Colors.purple),
                    SizedBox(height: 8),
                    Text('Discount Analytics'),
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
