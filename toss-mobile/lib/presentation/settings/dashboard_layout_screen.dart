import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'package:toss_mobile/app/dashboard/dashboard_config.dart';
import 'package:toss_mobile/app/dashboard/dashboard_manager.dart';
import 'dashboard_layout_editor_screen.dart';

/// Screen for selecting and managing dashboard layouts
class DashboardLayoutScreen extends StatelessWidget {
  const DashboardLayoutScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Dashboard Layouts'),
        actions: [
          IconButton(
            icon: const Icon(Icons.add),
            onPressed: () => _navigateToLayoutEditor(context),
            tooltip: 'Create Custom Layout',
          ),
        ],
      ),
      body: Consumer<DashboardManager>(
        builder: (context, dashboardManager, child) {
          return SingleChildScrollView(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _buildQuickActions(context, dashboardManager),
                const SizedBox(height: 24),
                _buildPresetLayouts(context, dashboardManager),
                if (dashboardManager.customLayouts.isNotEmpty) ...[
                  const SizedBox(height: 24),
                  _buildCustomLayouts(context, dashboardManager),
                ],
                const SizedBox(height: 24),
                _buildLayoutSettings(context, dashboardManager),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildQuickActions(BuildContext context, DashboardManager dashboardManager) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Quick Actions',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: _ActionTile(
                    icon: Icons.refresh,
                    title: 'Reset to Default',
                    subtitle: 'Restore default layout',
                    onTap: () => _showResetDialog(context, dashboardManager),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: _ActionTile(
                    icon: Icons.copy,
                    title: 'Duplicate Current',
                    subtitle: 'Create copy',
                    onTap: () => _showDuplicateDialog(context, dashboardManager),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPresetLayouts(BuildContext context, DashboardManager dashboardManager) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'Preset Layouts',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 12),
        GridView.builder(
          shrinkWrap: true,
          physics: const NeverScrollableScrollPhysics(),
          gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
            crossAxisCount: 2,
            crossAxisSpacing: 12,
            mainAxisSpacing: 12,
            childAspectRatio: 1.1,
          ),
          itemCount: DashboardLayoutPresets.allPresets.length,
          itemBuilder: (context, index) {
            final layout = DashboardLayoutPresets.allPresets[index];
            final isSelected = dashboardManager.currentLayout.id == layout.id;
            
            return _LayoutCard(
              layout: layout,
              isSelected: isSelected,
              onTap: () => dashboardManager.applyLayout(layout),
              onEdit: () => _navigateToLayoutEditor(context, layout),
            );
          },
        ),
      ],
    );
  }

  Widget _buildCustomLayouts(BuildContext context, DashboardManager dashboardManager) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'Custom Layouts',
          style: TextStyle(
            fontSize: 18,
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 12),
        GridView.builder(
          shrinkWrap: true,
          physics: const NeverScrollableScrollPhysics(),
          gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
            crossAxisCount: 2,
            crossAxisSpacing: 12,
            mainAxisSpacing: 12,
            childAspectRatio: 1.1,
          ),
          itemCount: dashboardManager.customLayouts.length,
          itemBuilder: (context, index) {
            final layout = dashboardManager.customLayouts[index];
            final isSelected = dashboardManager.currentLayout.id == layout.id;
            
            return _LayoutCard(
              layout: layout,
              isSelected: isSelected,
              isCustom: true,
              onTap: () => dashboardManager.applyLayout(layout),
              onEdit: () => _navigateToLayoutEditor(context, layout),
              onDelete: () => _showDeleteDialog(context, dashboardManager, layout),
            );
          },
        ),
      ],
    );
  }

  Widget _buildLayoutSettings(BuildContext context, DashboardManager dashboardManager) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Current Layout Settings',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            _LayoutSettingsTile(
              icon: Icons.grid_view,
              title: 'Columns',
              value: '${dashboardManager.currentLayout.columns}',
              onTap: () => _showColumnsDialog(context, dashboardManager),
            ),
            const SizedBox(height: 8),
            _LayoutSettingsTile(
              icon: Icons.space_bar,
              title: 'Spacing',
              value: '${dashboardManager.currentLayout.spacing.toInt()}px',
              onTap: () => _showSpacingDialog(context, dashboardManager),
            ),
            const SizedBox(height: 8),
            _LayoutSettingsTile(
              icon: Icons.widgets,
              title: 'Widgets',
              value: '${dashboardManager.currentLayout.widgets.length} items',
              onTap: () => _navigateToLayoutEditor(context, dashboardManager.currentLayout),
            ),
          ],
        ),
      ),
    );
  }

  void _navigateToLayoutEditor(BuildContext context, [DashboardLayout? layout]) {
    Navigator.of(context).push(
      MaterialPageRoute(
        builder: (context) => DashboardLayoutEditorScreen(
          initialLayout: layout,
        ),
      ),
    );
  }

  void _showResetDialog(BuildContext context, DashboardManager dashboardManager) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Reset to Default'),
        content: const Text('This will reset your dashboard to the default layout. Any unsaved changes will be lost.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              dashboardManager.resetToDefault();
              Navigator.pop(context);
            },
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: const Text('Reset'),
          ),
        ],
      ),
    );
  }

  void _showDuplicateDialog(BuildContext context, DashboardManager dashboardManager) {
    final controller = TextEditingController(
      text: '${dashboardManager.currentLayout.name} (Copy)',
    );

    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Duplicate Layout'),
        content: TextField(
          controller: controller,
          decoration: const InputDecoration(
            labelText: 'Layout Name',
            hintText: 'Enter a name for the copy',
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              if (controller.text.trim().isNotEmpty) {
                dashboardManager.duplicateLayout(
                  dashboardManager.currentLayout,
                  controller.text.trim(),
                );
                Navigator.pop(context);
              }
            },
            child: const Text('Duplicate'),
          ),
        ],
      ),
    );
  }

  void _showDeleteDialog(
    BuildContext context,
    DashboardManager dashboardManager,
    DashboardLayout layout,
  ) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Delete Layout'),
        content: Text('Are you sure you want to delete "${layout.name}"?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              dashboardManager.deleteCustomLayout(layout.id);
              Navigator.pop(context);
            },
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: const Text('Delete'),
          ),
        ],
      ),
    );
  }

  void _showColumnsDialog(BuildContext context, DashboardManager dashboardManager) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Grid Columns'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [1, 2, 3, 4, 5].map((columns) {
            return RadioListTile<int>(
              title: Text('$columns Columns'),
              value: columns,
              groupValue: dashboardManager.currentLayout.columns,
              onChanged: (value) {
                if (value != null) {
                  dashboardManager.updateLayoutSettings(columns: value);
                  Navigator.pop(context);
                }
              },
            );
          }).toList(),
        ),
      ),
    );
  }

  void _showSpacingDialog(BuildContext context, DashboardManager dashboardManager) {
    double spacing = dashboardManager.currentLayout.spacing;

    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Widget Spacing'),
        content: StatefulBuilder(
          builder: (context, setState) {
            return Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text('${spacing.toInt()}px'),
                Slider(
                  value: spacing,
                  min: 8,
                  max: 32,
                  divisions: 6,
                  onChanged: (value) {
                    setState(() {
                      spacing = value;
                    });
                  },
                ),
              ],
            );
          },
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              dashboardManager.updateLayoutSettings(spacing: spacing);
              Navigator.pop(context);
            },
            child: const Text('Apply'),
          ),
        ],
      ),
    );
  }
}

/// Action tile for quick actions
class _ActionTile extends StatelessWidget {
  final IconData icon;
  final String title;
  final String subtitle;
  final VoidCallback onTap;

  const _ActionTile({
    required this.icon,
    required this.title,
    required this.subtitle,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(8),
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(8),
          border: Border.all(
            color: Theme.of(context).dividerColor,
          ),
        ),
        child: Column(
          children: [
            Icon(
              icon,
              size: 32,
              color: Theme.of(context).primaryColor,
            ),
            const SizedBox(height: 8),
            Text(
              title,
              style: const TextStyle(
                fontWeight: FontWeight.w600,
              ),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 4),
            Text(
              subtitle,
              style: TextStyle(
                fontSize: 12,
                color: Colors.grey[600],
              ),
              textAlign: TextAlign.center,
            ),
          ],
        ),
      ),
    );
  }
}

/// Layout preview card
class _LayoutCard extends StatelessWidget {
  final DashboardLayout layout;
  final bool isSelected;
  final bool isCustom;
  final VoidCallback onTap;
  final VoidCallback? onEdit;
  final VoidCallback? onDelete;

  const _LayoutCard({
    required this.layout,
    required this.isSelected,
    this.isCustom = false,
    required this.onTap,
    this.onEdit,
    this.onDelete,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(12),
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: isSelected 
                ? Theme.of(context).primaryColor 
                : Colors.grey.withOpacity(0.3),
            width: isSelected ? 2 : 1,
          ),
        ),
        child: Column(
          children: [
            Expanded(
              child: Container(
                margin: const EdgeInsets.all(8),
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(8),
                  color: Colors.grey[100],
                ),
                child: Stack(
                  children: [
                    // Layout preview
                    _buildLayoutPreview(),
                    if (isSelected)
                      const Positioned(
                        top: 8,
                        right: 8,
                        child: Icon(
                          Icons.check_circle,
                          color: Colors.green,
                          size: 20,
                        ),
                      ),
                    if (isCustom)
                      Positioned(
                        top: 8,
                        right: isSelected ? 32 : 8,
                        child: PopupMenuButton<String>(
                          icon: const Icon(
                            Icons.more_vert,
                            size: 18,
                          ),
                          onSelected: (value) {
                            if (value == 'edit' && onEdit != null) {
                              onEdit!();
                            } else if (value == 'delete' && onDelete != null) {
                              onDelete!();
                            }
                          },
                          itemBuilder: (context) => [
                            const PopupMenuItem(
                              value: 'edit',
                              child: Row(
                                children: [
                                  Icon(Icons.edit, size: 16),
                                  SizedBox(width: 8),
                                  Text('Edit'),
                                ],
                              ),
                            ),
                            const PopupMenuItem(
                              value: 'delete',
                              child: Row(
                                children: [
                                  Icon(Icons.delete, size: 16, color: Colors.red),
                                  SizedBox(width: 8),
                                  Text('Delete', style: TextStyle(color: Colors.red)),
                                ],
                              ),
                            ),
                          ],
                        ),
                      ),
                  ],
                ),
              ),
            ),
            Padding(
              padding: const EdgeInsets.all(8),
              child: Column(
                children: [
                  Text(
                    layout.name,
                    style: const TextStyle(
                      fontWeight: FontWeight.w600,
                      fontSize: 14,
                    ),
                    textAlign: TextAlign.center,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  if (layout.description.isNotEmpty) ...[
                    const SizedBox(height: 2),
                    Text(
                      layout.description,
                      style: TextStyle(
                        fontSize: 10,
                        color: Colors.grey[600],
                      ),
                      textAlign: TextAlign.center,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ],
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildLayoutPreview() {
    // Create a simple grid representation of the layout
    return Padding(
      padding: const EdgeInsets.all(12),
      child: GridView.builder(
        gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
          crossAxisCount: layout.columns,
          crossAxisSpacing: 2,
          mainAxisSpacing: 2,
          childAspectRatio: 1.5,
        ),
        itemCount: layout.widgets.take(6).length, // Show max 6 widgets in preview
        itemBuilder: (context, index) {
          final widget = layout.widgets[index];
          return Container(
            decoration: BoxDecoration(
              color: widget.color?.withOpacity(0.6) ?? Colors.blue.withOpacity(0.6),
              borderRadius: BorderRadius.circular(2),
            ),
          );
        },
      ),
    );
  }
}

/// Settings tile for layout settings
class _LayoutSettingsTile extends StatelessWidget {
  final IconData icon;
  final String title;
  final String value;
  final VoidCallback onTap;

  const _LayoutSettingsTile({
    required this.icon,
    required this.title,
    required this.value,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(8),
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 4),
        child: Row(
          children: [
            Icon(
              icon,
              size: 20,
              color: Theme.of(context).primaryColor,
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Text(
                title,
                style: const TextStyle(
                  fontWeight: FontWeight.w500,
                ),
              ),
            ),
            Text(
              value,
              style: TextStyle(
                color: Colors.grey[600],
                fontSize: 14,
              ),
            ),
            const SizedBox(width: 8),
            const Icon(
              Icons.arrow_forward_ios,
              size: 16,
            ),
          ],
        ),
      ),
    );
  }
}
