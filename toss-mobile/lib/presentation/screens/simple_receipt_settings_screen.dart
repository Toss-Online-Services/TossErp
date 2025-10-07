import 'package:flutter/material.dart';

class SimpleReceiptSettingsScreen extends StatefulWidget {
  const SimpleReceiptSettingsScreen({super.key});

  @override
  State<SimpleReceiptSettingsScreen> createState() => _SimpleReceiptSettingsScreenState();
}

class _SimpleReceiptSettingsScreenState extends State<SimpleReceiptSettingsScreen> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Receipt Settings'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.receipt,
              size: 80,
              color: Colors.orange,
            ),
            SizedBox(height: 16),
            Text(
              'Receipt Settings',
              style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
              ),
            ),
            SizedBox(height: 8),
            Text(
              'Configure receipt templates, printing\noptions, and customer receipt preferences.',
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
                    Icon(Icons.print, size: 40, color: Colors.blue),
                    SizedBox(height: 8),
                    Text('Print Settings'),
                  ],
                ),
                Column(
                  children: [
                    Icon(Icons.design_services, size: 40, color: Colors.purple),
                    SizedBox(height: 8),
                    Text('Template Design'),
                  ],
                ),
                Column(
                  children: [
                    Icon(Icons.email, size: 40, color: Colors.green),
                    SizedBox(height: 8),
                    Text('Email Options'),
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
