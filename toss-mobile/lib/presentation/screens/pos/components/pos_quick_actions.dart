import 'package:flutter/material.dart';

class POSQuickActions extends StatelessWidget {
  final VoidCallback? onBarcodeScan;
  final VoidCallback? onHoldTransaction;
  final VoidCallback? onVoidTransaction;

  const POSQuickActions({
    super.key,
    this.onBarcodeScan,
    this.onHoldTransaction,
    this.onVoidTransaction,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Quick Actions',
              style: TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                _QuickActionButton(
                  icon: Icons.qr_code_scanner,
                  label: 'Scan',
                  onTap: onBarcodeScan,
                ),
                _QuickActionButton(
                  icon: Icons.pause,
                  label: 'Hold',
                  onTap: onHoldTransaction,
                ),
                _QuickActionButton(
                  icon: Icons.cancel_outlined,
                  label: 'Void',
                  onTap: onVoidTransaction,
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}

class _QuickActionButton extends StatelessWidget {
  final IconData icon;
  final String label;
  final VoidCallback? onTap;

  const _QuickActionButton({
    required this.icon,
    required this.label,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(8),
      child: Container(
        padding: const EdgeInsets.all(8),
        child: Column(
          children: [
            Icon(icon, size: 24),
            const SizedBox(height: 4),
            Text(
              label,
              style: const TextStyle(fontSize: 10),
            ),
          ],
        ),
      ),
    );
  }
}
