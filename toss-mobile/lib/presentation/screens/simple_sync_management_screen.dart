import 'package:flutter/material.dart';

class SimpleSyncManagementScreen extends StatefulWidget {
  const SimpleSyncManagementScreen({super.key});

  @override
  State<SimpleSyncManagementScreen> createState() => _SimpleSyncManagementScreenState();
}

class _SimpleSyncManagementScreenState extends State<SimpleSyncManagementScreen> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Sync Management'),
        backgroundColor: Colors.green[600],
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.sync,
              size: 64,
              color: Colors.green[600],
            ),
            const SizedBox(height: 16),
            const Text(
              'Sync Management',
              style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 8),
            const Text(
              'Manage data synchronization between local and cloud storage',
              textAlign: TextAlign.center,
              style: TextStyle(fontSize: 16, color: Colors.grey),
            ),
            const SizedBox(height: 32),
            ElevatedButton.icon(
              onPressed: () {
                // TODO: Implement sync functionality
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(
                    content: Text('Sync functionality coming soon!'),
                  ),
                );
              },
              icon: const Icon(Icons.sync),
              label: const Text('Start Sync'),
            ),
            const SizedBox(height: 16),
            ElevatedButton.icon(
              onPressed: () {
                // TODO: Implement sync status functionality
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(
                    content: Text('Sync status features coming soon!'),
                  ),
                );
              },
              icon: const Icon(Icons.info),
              label: const Text('Sync Status'),
            ),
          ],
        ),
      ),
    );
  }
}
