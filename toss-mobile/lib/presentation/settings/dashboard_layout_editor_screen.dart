import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'package:toss_mobile/app/dashboard/dashboard_config.dart';

/// Screen for editing and customizing dashboard layouts
class DashboardLayoutEditorScreen extends StatefulWidget {
  final DashboardLayout? initialLayout;

  const DashboardLayoutEditorScreen({
    super.key,
    this.initialLayout,
  });

  @override
  State<DashboardLayoutEditorScreen> createState() => _DashboardLayoutEditorScreenState();
}

class _DashboardLayoutEditorScreenState extends State<DashboardLayoutEditorScreen> {
  late DashboardLayout _currentLayout;
  final _nameController = TextEditingController();
  final _descriptionController = TextEditingController();
  bool _hasChanges = false;

  @override
  void initState() {
    super.initState();
    _currentLayout = widget.initialLayout?.copyWith() ?? _createNewLayout();
    _nameController.text = _currentLayout.name;
    _descriptionController.text = _currentLayout.description;
  }

  @override
  void dispose() {
    _nameController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  DashboardLayout _createNewLayout() {
    return DashboardLayout(
      id: 'custom_${DateTime.now().millisecondsSinceEpoch}',
      name: 'Custom Layout',
      description: 'My custom dashboard layout',
      widgets: [
        DashboardWidgetConfig(
          id: 'sales_summary_${DateTime.now().millisecondsSinceEpoch}',
          type: DashboardWidget.salesSummary,
          position: 0,
          size: DashboardWidgetSize.medium,
        ),
      ],
      columns: 2,
      spacing: 16,
    );
  }

  void _markAsChanged() {
    if (!_hasChanges) {
      setState(() {
        _hasChanges = true;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !_hasChanges,
      onPopInvoked: (didPop) {
        if (!didPop && _hasChanges) {
          _showDiscardDialog();
        }
      },
      child: Scaffold(
        appBar: AppBar(
          title: Text(widget.initialLayout == null ? 'Create Layout' : 'Edit Layout'),
          actions: [
            TextButton(
              onPressed: _hasChanges ? _saveLayout : null,
              child: Text(
                'Save',
                style: TextStyle(
                  color: _hasChanges 
                      ? Theme.of(context).primaryColor 
                      : Colors.grey,
                ),
              ),
            ),
          ],
        ),
        body: Column(
          children: [
            Expanded(
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    _buildLayoutInfo(),
                    const SizedBox(height: 24),
                    _buildLayoutSettings(),
                    const SizedBox(height: 24),
                    _buildWidgetsList(),
                    const SizedBox(height: 24),
                    _buildAddWidgetSection(),
                  ],
                ),
              ),
            ),
            _buildPreviewSection(),
          ],
        ),
      ),
    );
  }

  Widget _buildLayoutInfo() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Layout Information',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            TextField(
              controller: _nameController,
              decoration: const InputDecoration(
                labelText: 'Layout Name',
                border: OutlineInputBorder(),
              ),
              onChanged: (value) {
                _markAsChanged();
                _currentLayout = _currentLayout.copyWith(name: value);
              },
            ),
            const SizedBox(height: 16),
            TextField(
              controller: _descriptionController,
              decoration: const InputDecoration(
                labelText: 'Description (Optional)',
                border: OutlineInputBorder(),
              ),
              maxLines: 2,
              onChanged: (value) {
                _markAsChanged();
                _currentLayout = _currentLayout.copyWith(description: value);
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildLayoutSettings() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Layout Settings',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text('Grid Columns'),
                      const SizedBox(height: 8),
                      DropdownButtonFormField<int>(
                        value: _currentLayout.columns,
                        decoration: const InputDecoration(
                          border: OutlineInputBorder(),
                        ),
                        items: [1, 2, 3, 4, 5].map((columns) {
                          return DropdownMenuItem(
                            value: columns,
                            child: Text('$columns Columns'),
                          );
                        }).toList(),
                        onChanged: (value) {
                          if (value != null) {
                            _markAsChanged();
                            setState(() {
                              _currentLayout = _currentLayout.copyWith(columns: value);
                            });
                          }
                        },
                      ),
                    ],
                  ),
                ),
                const SizedBox(width: 16),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text('Spacing: ${_currentLayout.spacing.toInt()}px'),
                      const SizedBox(height: 8),
                      Slider(
                        value: _currentLayout.spacing,
                        min: 8,
                        max: 32,
                        divisions: 6,
                        onChanged: (value) {
                          _markAsChanged();
                          setState(() {
                            _currentLayout = _currentLayout.copyWith(spacing: value);
                          });
                        },
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildWidgetsList() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                const Text(
                  'Dashboard Widgets',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const Spacer(),
                Text(
                  '${_currentLayout.widgets.length} widgets',
                  style: TextStyle(
                    color: Colors.grey[600],
                    fontSize: 14,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            if (_currentLayout.widgets.isEmpty)
              const Center(
                child: Padding(
                  padding: EdgeInsets.all(32),
                  child: Text(
                    'No widgets added yet.\nTap "Add Widget" to get started.',
                    textAlign: TextAlign.center,
                    style: TextStyle(color: Colors.grey),
                  ),
                ),
              )
            else
              ReorderableListView.builder(
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                itemCount: _currentLayout.widgets.length,
                onReorder: _reorderWidgets,
                itemBuilder: (context, index) {
                  final widget = _currentLayout.widgets[index];
                  return _WidgetConfigTile(
                    key: ValueKey(widget.id),
                    widget: widget,
                    onSizeChanged: (size) => _updateWidgetSize(index, size),
                    onColorChanged: (color) => _updateWidgetColor(index, color),
                    onRemove: () => _removeWidget(index),
                  );
                },
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildAddWidgetSection() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Available Widgets',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: AvailableWidgets.allWidgets.entries.map((entry) {
                final widgetType = entry.key;
                final widgetInfo = entry.value;
                final isAdded = _currentLayout.widgets.any((w) => w.type == widgetType);
                
                return FilterChip(
                  label: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Icon(
                        widgetInfo.icon,
                        size: 16,
                      ),
                      const SizedBox(width: 4),
                      Text(widgetInfo.name),
                    ],
                  ),
                  selected: isAdded,
                  onSelected: isAdded ? null : (_) => _addWidget(widgetType),
                  backgroundColor: isAdded ? Colors.grey[200] : null,
                );
              }).toList(),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPreviewSection() {
    return Container(
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        border: Border(
          top: BorderSide(
            color: Theme.of(context).dividerColor,
          ),
        ),
      ),
      child: ExpansionTile(
        title: const Text('Layout Preview'),
        subtitle: Text('${_currentLayout.widgets.length} widgets in ${_currentLayout.columns} columns'),
        children: [
          Container(
            height: 200,
            margin: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              border: Border.all(color: Colors.grey.withOpacity(0.3)),
              borderRadius: BorderRadius.circular(8),
            ),
            child: _buildLayoutPreview(),
          ),
        ],
      ),
    );
  }

  Widget _buildLayoutPreview() {
    return Padding(
      padding: EdgeInsets.all(_currentLayout.spacing),
      child: GridView.builder(
        gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
          crossAxisCount: _currentLayout.columns,
          crossAxisSpacing: _currentLayout.spacing,
          mainAxisSpacing: _currentLayout.spacing,
          childAspectRatio: 2,
        ),
        itemCount: _currentLayout.widgets.length,
        itemBuilder: (context, index) {
          final widget = _currentLayout.widgets[index];
          final widgetInfo = AvailableWidgets.allWidgets[widget.type]!;
          
          return Container(
            decoration: BoxDecoration(
              color: widget.color?.withOpacity(0.8) ?? widgetInfo.defaultColor.withOpacity(0.8),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                Icon(
                  widgetInfo.icon,
                  color: Colors.white,
                  size: 24,
                ),
                const SizedBox(height: 4),
                Text(
                  widgetInfo.name,
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 12,
                    fontWeight: FontWeight.w500,
                  ),
                  textAlign: TextAlign.center,
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  void _addWidget(DashboardWidget widgetType) {
    _markAsChanged();
    setState(() {
      final newWidget = DashboardWidgetConfig(
        id: '${widgetType.name}_${DateTime.now().millisecondsSinceEpoch}',
        type: widgetType,
        position: _currentLayout.widgets.length,
        size: DashboardWidgetSize.medium,
      );
      _currentLayout = _currentLayout.copyWith(
        widgets: [..._currentLayout.widgets, newWidget],
      );
    });
  }

  void _removeWidget(int index) {
    _markAsChanged();
    setState(() {
      final widgets = List<DashboardWidgetConfig>.from(_currentLayout.widgets);
      widgets.removeAt(index);
      
      // Update positions
      for (int i = 0; i < widgets.length; i++) {
        widgets[i] = widgets[i].copyWith(position: i);
      }
      
      _currentLayout = _currentLayout.copyWith(widgets: widgets);
    });
  }

  void _reorderWidgets(int oldIndex, int newIndex) {
    _markAsChanged();
    setState(() {
      final widgets = List<DashboardWidgetConfig>.from(_currentLayout.widgets);
      
      if (newIndex > oldIndex) {
        newIndex -= 1;
      }
      
      final widget = widgets.removeAt(oldIndex);
      widgets.insert(newIndex, widget);
      
      // Update positions
      for (int i = 0; i < widgets.length; i++) {
        widgets[i] = widgets[i].copyWith(position: i);
      }
      
      _currentLayout = _currentLayout.copyWith(widgets: widgets);
    });
  }

  void _updateWidgetSize(int index, DashboardWidgetSize size) {
    _markAsChanged();
    setState(() {
      final widgets = List<DashboardWidgetConfig>.from(_currentLayout.widgets);
      widgets[index] = widgets[index].copyWith(size: size);
      _currentLayout = _currentLayout.copyWith(widgets: widgets);
    });
  }

  void _updateWidgetColor(int index, Color color) {
    _markAsChanged();
    setState(() {
      final widgets = List<DashboardWidgetConfig>.from(_currentLayout.widgets);
      widgets[index] = widgets[index].copyWith(color: color);
      _currentLayout = _currentLayout.copyWith(widgets: widgets);
    });
  }

  void _saveLayout() {
    final dashboardManager = context.read<DashboardManager>();
    
    final finalLayout = _currentLayout.copyWith(
      name: _nameController.text.trim(),
      description: _descriptionController.text.trim(),
    );

    if (widget.initialLayout == null) {
      // Create new layout
      dashboardManager.createCustomLayout(finalLayout);
    } else {
      // Update existing layout
      dashboardManager.updateCustomLayout(finalLayout);
    }

    Navigator.pop(context);
  }

  void _showDiscardDialog() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Discard Changes?'),
        content: const Text('You have unsaved changes. Are you sure you want to discard them?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              Navigator.pop(context);
              Navigator.pop(context);
            },
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: const Text('Discard'),
          ),
        ],
      ),
    );
  }
}

/// Widget configuration tile for editing individual widgets
class _WidgetConfigTile extends StatelessWidget {
  final DashboardWidgetConfig widget;
  final Function(DashboardWidgetSize) onSizeChanged;
  final Function(Color) onColorChanged;
  final VoidCallback onRemove;

  const _WidgetConfigTile({
    super.key,
    required this.widget,
    required this.onSizeChanged,
    required this.onColorChanged,
    required this.onRemove,
  });

  @override
  Widget build(BuildContext context) {
    final widgetInfo = AvailableWidgets.allWidgets[widget.type]!;
    
    return Card(
      margin: const EdgeInsets.symmetric(vertical: 4),
      child: ExpansionTile(
        leading: Icon(
          widgetInfo.icon,
          color: widget.color ?? widgetInfo.defaultColor,
        ),
        title: Text(widgetInfo.name),
        subtitle: Text('Size: ${widget.size.name.toUpperCase()}'),
        trailing: IconButton(
          icon: const Icon(Icons.delete, color: Colors.red),
          onPressed: onRemove,
        ),
        children: [
          Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              children: [
                _buildSizeSelector(),
                const SizedBox(height: 16),
                _buildColorSelector(),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSizeSelector() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'Widget Size',
          style: TextStyle(fontWeight: FontWeight.w500),
        ),
        const SizedBox(height: 8),
        SegmentedButton<DashboardWidgetSize>(
          segments: const [
            ButtonSegment(
              value: DashboardWidgetSize.small,
              label: Text('Small'),
            ),
            ButtonSegment(
              value: DashboardWidgetSize.medium,
              label: Text('Medium'),
            ),
            ButtonSegment(
              value: DashboardWidgetSize.large,
              label: Text('Large'),
            ),
          ],
          selected: {widget.size},
          onSelectionChanged: (Set<DashboardWidgetSize> selection) {
            onSizeChanged(selection.first);
          },
        ),
      ],
    );
  }

  Widget _buildColorSelector() {
    final colors = [
      Colors.blue,
      Colors.green,
      Colors.orange,
      Colors.red,
      Colors.purple,
      Colors.teal,
      Colors.amber,
      Colors.indigo,
    ];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'Widget Color',
          style: TextStyle(fontWeight: FontWeight.w500),
        ),
        const SizedBox(height: 8),
        Wrap(
          spacing: 8,
          children: colors.map((color) {
            final isSelected = widget.color == color ||
                (widget.color == null && color == AvailableWidgets.allWidgets[widget.type]!.defaultColor);
            
            return GestureDetector(
              onTap: () => onColorChanged(color),
              child: Container(
                width: 40,
                height: 40,
                decoration: BoxDecoration(
                  color: color,
                  borderRadius: BorderRadius.circular(8),
                  border: isSelected
                      ? Border.all(color: Colors.black, width: 3)
                      : null,
                ),
                child: isSelected
                    ? const Icon(
                        Icons.check,
                        color: Colors.white,
                        size: 20,
                      )
                    : null,
              ),
            );
          }).toList(),
        ),
      ],
    );
  }
}
