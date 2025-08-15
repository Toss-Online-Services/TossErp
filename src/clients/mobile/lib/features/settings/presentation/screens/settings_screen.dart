import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

class SettingsScreen extends ConsumerWidget {
  const SettingsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Settings'),
      ),
      body: ListView(
        padding: const EdgeInsets.all(16.0),
        children: [
          // User Profile Section
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Row(
                children: [
                  CircleAvatar(
                    radius: 30,
                    backgroundColor: Colors.blue[100],
                    child: Icon(
                      Icons.person,
                      size: 30,
                      color: Colors.blue[700],
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Admin User',
                          style: Theme.of(context).textTheme.titleMedium?.copyWith(
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'admin@toss-erp.com',
                          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                            color: Colors.grey[600],
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          'Store Manager',
                          style: Theme.of(context).textTheme.bodySmall?.copyWith(
                            color: Colors.blue[600],
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ],
                    ),
                  ),
                  IconButton(
                    icon: const Icon(Icons.edit),
                    onPressed: () {
                      // TODO: Edit profile
                    },
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: 24),
          
          // General Settings
          _buildSectionHeader(context, 'General'),
          _buildSettingTile(
            context,
            icon: Icons.store,
            title: 'Store Information',
            subtitle: 'Manage store details',
            onTap: () {
              // TODO: Store settings
            },
          ),
          _buildSettingTile(
            context,
            icon: Icons.currency_exchange,
            title: 'Currency & Units',
            subtitle: 'Set currency and measurement units',
            onTap: () {
              // TODO: Currency settings
            },
          ),
          _buildSettingTile(
            context,
            icon: Icons.language,
            title: 'Language',
            subtitle: 'English (US)',
            onTap: () {
              // TODO: Language settings
            },
          ),
          _buildSettingTile(
            context,
            icon: Icons.notifications,
            title: 'Notifications',
            subtitle: 'Configure alerts and notifications',
            onTap: () {
              // TODO: Notification settings
            },
          ),
          
          const SizedBox(height: 24),
          
          // Business Settings
          _buildSectionHeader(context, 'Business'),
          _buildSettingTile(
            context,
            icon: Icons.receipt,
            title: 'Receipt Settings',
            subtitle: 'Customize receipt format',
            onTap: () {
              // TODO: Receipt settings
            },
          ),
          _buildSettingTile(
            context,
            icon: Icons.receipt_long,
            title: 'Tax Settings',
            subtitle: 'Configure tax rates and rules',
            onTap: () {
              // TODO: Tax settings
            },
          ),
          _buildSettingTile(
            context,
            icon: Icons.discount,
            title: 'Discount Rules',
            subtitle: 'Set up discount policies',
            onTap: () {
              // TODO: Discount settings
            },
          ),
          _buildSettingTile(
            context,
            icon: Icons.inventory,
            title: 'Stock Alerts',
            subtitle: 'Configure low stock notifications',
            onTap: () {
              // TODO: Stock alert settings
            },
          ),
          
          const SizedBox(height: 24),
          
          // System Settings
          _buildSectionHeader(context, 'System'),
          _buildSettingTile(
            context,
            icon: Icons.sync,
            title: 'Data Synchronization',
            subtitle: 'Manage offline sync settings',
            onTap: () {
              // TODO: Sync settings
            },
          ),
          _buildSettingTile(
            context,
            icon: Icons.backup,
            title: 'Backup & Restore',
            subtitle: 'Manage data backups',
            onTap: () {
              // TODO: Backup settings
            },
          ),
          _buildSettingTile(
            context,
            icon: Icons.security,
            title: 'Security',
            subtitle: 'Password and access control',
            onTap: () {
              // TODO: Security settings
            },
          ),
          _buildSettingTile(
            context,
            icon: Icons.print,
            title: 'Printers',
            subtitle: 'Configure receipt printers',
            onTap: () {
              // TODO: Printer settings
            },
          ),
          
          const SizedBox(height: 24),
          
          // Support & About
          _buildSectionHeader(context, 'Support & About'),
          _buildSettingTile(
            context,
            icon: Icons.help,
            title: 'Help & Support',
            subtitle: 'Get help and contact support',
            onTap: () {
              // TODO: Help and support
            },
          ),
          _buildSettingTile(
            context,
            icon: Icons.info,
            title: 'About',
            subtitle: 'App version and information',
            onTap: () {
              // TODO: About page
            },
          ),
          _buildSettingTile(
            context,
            icon: Icons.logout,
            title: 'Logout',
            subtitle: 'Sign out of the application',
            onTap: () {
              _showLogoutDialog(context);
            },
            isDestructive: true,
          ),
        ],
      ),
    );
  }

  Widget _buildSectionHeader(BuildContext context, String title) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8.0),
      child: Text(
        title,
        style: Theme.of(context).textTheme.titleMedium?.copyWith(
          fontWeight: FontWeight.bold,
          color: Colors.grey[700],
        ),
      ),
    );
  }

  Widget _buildSettingTile(
    BuildContext context, {
    required IconData icon,
    required String title,
    required String subtitle,
    required VoidCallback onTap,
    bool isDestructive = false,
  }) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ListTile(
        leading: Icon(
          icon,
          color: isDestructive ? Colors.red : Colors.grey[600],
        ),
        title: Text(
          title,
          style: TextStyle(
            fontWeight: FontWeight.w600,
            color: isDestructive ? Colors.red : null,
          ),
        ),
        subtitle: Text(
          subtitle,
          style: TextStyle(
            color: isDestructive ? Colors.red[300] : Colors.grey[600],
          ),
        ),
        trailing: const Icon(Icons.chevron_right),
        onTap: onTap,
      ),
    );
  }

  void _showLogoutDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Logout'),
          content: const Text('Are you sure you want to logout?'),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('Cancel'),
            ),
            ElevatedButton(
              onPressed: () {
                Navigator.of(context).pop();
                // TODO: Implement logout
                context.go('/login');
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.red,
                foregroundColor: Colors.white,
              ),
              child: const Text('Logout'),
            ),
          ],
        );
      },
    );
  }
}
