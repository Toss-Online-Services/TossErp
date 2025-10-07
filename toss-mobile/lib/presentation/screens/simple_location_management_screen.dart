import 'package:flutter/material.dart';

class SimpleLocationManagementScreen extends StatefulWidget {
  const SimpleLocationManagementScreen({super.key});

  @override
  State<SimpleLocationManagementScreen> createState() => _SimpleLocationManagementScreenState();
}

class _SimpleLocationManagementScreenState extends State<SimpleLocationManagementScreen> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Location Management'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.location_on,
              size: 80,
              color: Colors.red,
            ),
            SizedBox(height: 16),
            Text(
              'Location Management',
              style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
              ),
            ),
            SizedBox(height: 8),
            Text(
              'Manage multiple store locations,\ninventory distribution, and location settings.',
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
                    Icon(Icons.add_location, size: 40, color: Colors.green),
                    SizedBox(height: 8),
                    Text('Add Location'),
                  ],
                ),
                Column(
                  children: [
                    Icon(Icons.store, size: 40, color: Colors.blue),
                    SizedBox(height: 8),
                    Text('Store Settings'),
                  ],
                ),
                Column(
                  children: [
                    Icon(Icons.inventory, size: 40, color: Colors.orange),
                    SizedBox(height: 8),
                    Text('Inventory Transfer'),
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
