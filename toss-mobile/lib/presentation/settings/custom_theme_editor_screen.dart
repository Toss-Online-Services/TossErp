import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';

import '../../app/themes/theme_manager.dart';
import '../../app/themes/theme_config.dart';

/// Screen for creating and editing custom themes
class CustomThemeEditorScreen extends StatefulWidget {
  final ThemeConfig? initialTheme;

  const CustomThemeEditorScreen({
    super.key,
    this.initialTheme,
  });

  @override
  State<CustomThemeEditorScreen> createState() => _CustomThemeEditorScreenState();
}

class _CustomThemeEditorScreenState extends State<CustomThemeEditorScreen> {
  late TextEditingController _nameController;
  late TextEditingController _descriptionController;
  
  late ThemeConfig _workingTheme;
  bool _isEditing = false;

  @override
  void initState() {
    super.initState();
    _isEditing = widget.initialTheme != null;
    _workingTheme = widget.initialTheme?.copyWith() ?? ThemePresets.defaultTheme.copyWith(
      id: 'temp_theme',
      name: 'My Custom Theme',
      description: 'Custom theme created by user',
      isCustom: true,
    );
    
    _nameController = TextEditingController(text: _workingTheme.name);
    _descriptionController = TextEditingController(text: _workingTheme.description);
  }

  @override
  void dispose() {
    _nameController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(_isEditing ? 'Edit Theme' : 'Create Theme'),
        actions: [
          TextButton(
            onPressed: _saveTheme,
            child: const Text('Save'),
          ),
        ],
      ),
      body: Theme(
        data: context.read<ThemeManager>().generateThemeData(_workingTheme),
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              _buildPreview(),
              const SizedBox(height: 24),
              _buildBasicInfo(),
              const SizedBox(height: 24),
              _buildColorSettings(),
              const SizedBox(height: 24),
              _buildTypographySettings(),
              const SizedBox(height: 24),
              _buildLayoutSettings(),
              const SizedBox(height: 24),
              _buildBrightnessSettings(),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildPreview() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Preview',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            Container(
              height: 200,
              decoration: BoxDecoration(
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: Colors.grey.withOpacity(0.3)),
              ),
              child: Theme(
                data: context.read<ThemeManager>().generateThemeData(_workingTheme),
                child: Builder(
                  builder: (context) => Material(
                    borderRadius: BorderRadius.circular(8),
                    child: Padding(
                      padding: const EdgeInsets.all(16),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            _workingTheme.name,
                            style: Theme.of(context).textTheme.headlineSmall,
                          ),
                          const SizedBox(height: 8),
                          Text(
                            'This is how your theme will look',
                            style: Theme.of(context).textTheme.bodyMedium,
                          ),
                          const SizedBox(height: 16),
                          Row(
                            children: [
                              Expanded(
                                child: ElevatedButton(
                                  onPressed: () {},
                                  child: const Text('Primary'),
                                ),
                              ),
                              const SizedBox(width: 8),
                              Expanded(
                                child: OutlinedButton(
                                  onPressed: () {},
                                  child: const Text('Secondary'),
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 12),
                          TextField(
                            decoration: const InputDecoration(
                              labelText: 'Sample Input',
                              hintText: 'Type something...',
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildBasicInfo() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Basic Information',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            TextField(
              controller: _nameController,
              decoration: const InputDecoration(
                labelText: 'Theme Name',
                hintText: 'Enter theme name',
              ),
              onChanged: (value) {
                setState(() {
                  _workingTheme = _workingTheme.copyWith(name: value);
                });
              },
            ),
            const SizedBox(height: 16),
            TextField(
              controller: _descriptionController,
              decoration: const InputDecoration(
                labelText: 'Description (Optional)',
                hintText: 'Describe your theme',
              ),
              maxLines: 2,
              onChanged: (value) {
                setState(() {
                  _workingTheme = _workingTheme.copyWith(description: value);
                });
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildColorSettings() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Colors',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            _ColorPicker(
              label: 'Primary Color',
              color: _workingTheme.primaryColor,
              onColorChanged: (color) {
                setState(() {
                  _workingTheme = _workingTheme.copyWith(primaryColor: color);
                });
              },
            ),
            const SizedBox(height: 16),
            _ColorPicker(
              label: 'Secondary Color',
              color: _workingTheme.secondaryColor,
              onColorChanged: (color) {
                setState(() {
                  _workingTheme = _workingTheme.copyWith(secondaryColor: color);
                });
              },
            ),
            const SizedBox(height: 16),
            _ColorPicker(
              label: 'Accent Color',
              color: _workingTheme.accentColor,
              onColorChanged: (color) {
                setState(() {
                  _workingTheme = _workingTheme.copyWith(accentColor: color);
                });
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildTypographySettings() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Typography',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            DropdownButtonFormField<String>(
              value: _workingTheme.fontFamily,
              decoration: const InputDecoration(
                labelText: 'Font Family',
              ),
              items: ThemeManager.availableFonts.map((font) {
                return DropdownMenuItem(
                  value: font,
                  child: Text(font),
                );
              }).toList(),
              onChanged: (value) {
                if (value != null) {
                  setState(() {
                    _workingTheme = _workingTheme.copyWith(fontFamily: value);
                  });
                }
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
              'Layout',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            _SliderSetting(
              label: 'Border Radius',
              value: _workingTheme.borderRadius,
              min: 0,
              max: 20,
              divisions: 20,
              onChanged: (value) {
                setState(() {
                  _workingTheme = _workingTheme.copyWith(borderRadius: value);
                });
              },
            ),
            const SizedBox(height: 16),
            _SliderSetting(
              label: 'Spacing',
              value: _workingTheme.spacing,
              min: 8,
              max: 24,
              divisions: 16,
              onChanged: (value) {
                setState(() {
                  _workingTheme = _workingTheme.copyWith(spacing: value);
                });
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildBrightnessSettings() {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Appearance',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            SwitchListTile(
              title: const Text('Dark Mode'),
              subtitle: Text(_workingTheme.isDarkMode ? 'Dark theme' : 'Light theme'),
              value: _workingTheme.isDarkMode,
              onChanged: (value) {
                setState(() {
                  _workingTheme = _workingTheme.copyWith(
                    isDarkMode: value,
                    brightness: value ? Brightness.dark : Brightness.light,
                  );
                });
              },
            ),
          ],
        ),
      ),
    );
  }

  void _saveTheme() async {
    if (_nameController.text.trim().isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please enter a theme name')),
      );
      return;
    }

    final themeManager = context.read<ThemeManager>();
    final finalTheme = _workingTheme.copyWith(
      name: _nameController.text.trim(),
      description: _descriptionController.text.trim(),
    );

    try {
      if (_isEditing) {
        await themeManager.updateCustomTheme(finalTheme);
      } else {
        await themeManager.createCustomTheme(finalTheme);
      }

      if (mounted) {
        Navigator.of(context).pop();
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(_isEditing ? 'Theme updated successfully' : 'Theme created successfully'),
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error saving theme: $e')),
        );
      }
    }
  }
}

/// Color picker widget
class _ColorPicker extends StatelessWidget {
  final String label;
  final Color color;
  final ValueChanged<Color> onColorChanged;

  const _ColorPicker({
    required this.label,
    required this.color,
    required this.onColorChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: Text(
            label,
            style: const TextStyle(fontWeight: FontWeight.w500),
          ),
        ),
        GestureDetector(
          onTap: () => _showColorPicker(context),
          child: Container(
            width: 48,
            height: 48,
            decoration: BoxDecoration(
              color: color,
              borderRadius: BorderRadius.circular(8),
              border: Border.all(color: Colors.grey.withOpacity(0.3)),
            ),
          ),
        ),
      ],
    );
  }

  void _showColorPicker(BuildContext context) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text('Select $label'),
        content: SizedBox(
          width: 300,
          height: 400,
          child: _ColorPickerDialog(
            currentColor: color,
            onColorChanged: onColorChanged,
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Done'),
          ),
        ],
      ),
    );
  }
}

/// Color picker dialog content
class _ColorPickerDialog extends StatefulWidget {
  final Color currentColor;
  final ValueChanged<Color> onColorChanged;

  const _ColorPickerDialog({
    required this.currentColor,
    required this.onColorChanged,
  });

  @override
  State<_ColorPickerDialog> createState() => _ColorPickerDialogState();
}

class _ColorPickerDialogState extends State<_ColorPickerDialog> {
  late Color _selectedColor;

  @override
  void initState() {
    super.initState();
    _selectedColor = widget.currentColor;
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        // Color preview
        Container(
          width: double.infinity,
          height: 80,
          decoration: BoxDecoration(
            color: _selectedColor,
            borderRadius: BorderRadius.circular(8),
          ),
          child: Center(
            child: Text(
              '#${_selectedColor.value.toRadixString(16).substring(2).toUpperCase()}',
              style: TextStyle(
                color: _selectedColor.computeLuminance() > 0.5 ? Colors.black : Colors.white,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ),
        const SizedBox(height: 16),
        // Predefined colors
        Expanded(
          child: GridView.builder(
            gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
              crossAxisCount: 6,
              crossAxisSpacing: 8,
              mainAxisSpacing: 8,
            ),
            itemCount: _predefinedColors.length,
            itemBuilder: (context, index) {
              final color = _predefinedColors[index];
              return GestureDetector(
                onTap: () {
                  setState(() {
                    _selectedColor = color;
                  });
                  widget.onColorChanged(color);
                },
                child: Container(
                  decoration: BoxDecoration(
                    color: color,
                    borderRadius: BorderRadius.circular(8),
                    border: _selectedColor == color
                        ? Border.all(color: Colors.black, width: 2)
                        : Border.all(color: Colors.grey.withOpacity(0.3)),
                  ),
                ),
              );
            },
          ),
        ),
      ],
    );
  }

  static const List<Color> _predefinedColors = [
    // Material Design Colors
    Colors.red,
    Colors.pink,
    Colors.purple,
    Colors.deepPurple,
    Colors.indigo,
    Colors.blue,
    Colors.lightBlue,
    Colors.cyan,
    Colors.teal,
    Colors.green,
    Colors.lightGreen,
    Colors.lime,
    Colors.yellow,
    Colors.amber,
    Colors.orange,
    Colors.deepOrange,
    Colors.brown,
    Colors.grey,
    Colors.blueGrey,
    Colors.black,
    // Custom colors
    Color(0xFFFF6B35), // Orange
    Color(0xFF2E86AB), // Blue
    Color(0xFF0F4C5C), // Dark Blue
    Color(0xFF5F9DF7), // Light Blue
    Color(0xFF9B59B6), // Purple
    Color(0xFFE74C3C), // Red
    Color(0xFF27AE60), // Green
    Color(0xFFF39C12), // Yellow
    Color(0xFF34495E), // Dark Gray
    Color(0xFF95A5A6), // Light Gray
    Color(0xFF1ABC9C), // Turquoise
    Color(0xFFE67E22), // Carrot Orange
  ];
}

/// Slider setting widget
class _SliderSetting extends StatelessWidget {
  final String label;
  final double value;
  final double min;
  final double max;
  final int? divisions;
  final ValueChanged<double> onChanged;

  const _SliderSetting({
    required this.label,
    required this.value,
    required this.min,
    required this.max,
    this.divisions,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              label,
              style: const TextStyle(fontWeight: FontWeight.w500),
            ),
            Text(
              value.toStringAsFixed(0),
              style: const TextStyle(
                fontWeight: FontWeight.w500,
                fontSize: 16,
              ),
            ),
          ],
        ),
        Slider(
          value: value,
          min: min,
          max: max,
          divisions: divisions,
          onChanged: onChanged,
        ),
      ],
    );
  }
}
