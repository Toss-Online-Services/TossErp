import 'dart:convert';
import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'dashboard_config.dart';

/// Manager for dashboard layout configuration and persistence
class DashboardManager extends ChangeNotifier {
  static const String _currentLayoutKey = 'current_dashboard_layout';
  static const String _customLayoutsKey = 'custom_dashboard_layouts';
  
  DashboardLayout _currentLayout = DashboardLayoutPresets.defaultLayout;
  List<DashboardLayout> _customLayouts = [];
  bool _isInitialized = false;

  /// Current active dashboard layout
  DashboardLayout get currentLayout => _currentLayout;
  
  /// List of custom layouts created by user
  List<DashboardLayout> get customLayouts => List.unmodifiable(_customLayouts);
  
  /// List of all available preset layouts
  List<DashboardLayout> get presetLayouts => DashboardLayoutPresets.allPresets;
  
  /// All layouts (presets + custom)
  List<DashboardLayout> get allLayouts => [...presetLayouts, ..._customLayouts];
  
  /// Check if manager is initialized
  bool get isInitialized => _isInitialized;

  /// Initialize the dashboard manager
  Future<void> initialize() async {
    if (_isInitialized) return;
    
    try {
      final prefs = await SharedPreferences.getInstance();
      
      // Load current layout
      final currentLayoutJson = prefs.getString(_currentLayoutKey);
      if (currentLayoutJson != null) {
        final layoutData = json.decode(currentLayoutJson);
        _currentLayout = DashboardLayout.fromJson(layoutData);
      }
      
      // Load custom layouts
      final customLayoutsJson = prefs.getString(_customLayoutsKey);
      if (customLayoutsJson != null) {
        final layoutsData = json.decode(customLayoutsJson) as List;
        _customLayouts = layoutsData
            .map((data) => DashboardLayout.fromJson(data))
            .toList();
      }
      
      _isInitialized = true;
      notifyListeners();
    } catch (e) {
      debugPrint('Error initializing dashboard manager: $e');
      _currentLayout = DashboardLayoutPresets.defaultLayout;
      _customLayouts = [];
      _isInitialized = true;
      notifyListeners();
    }
  }

  /// Apply a dashboard layout
  Future<void> applyLayout(DashboardLayout layout) async {
    _currentLayout = layout.copyWith(
      updatedAt: DateTime.now(),
    );
    await _saveCurrentLayout();
    notifyListeners();
  }

  /// Create a new custom layout
  void createCustomLayout(String name, String description) {
    final newLayout = DashboardLayout(
      id: DateTime.now().millisecondsSinceEpoch.toString(),
      name: name,
      description: description,
      widgets: DashboardLayoutPresets.defaultLayout.widgets,
      createdAt: DateTime.now(),
      updatedAt: DateTime.now(),
    );
    _customLayouts.add(newLayout);
    _saveCustomLayouts();
    notifyListeners();
  }

  /// Update an existing custom layout
  void updateCustomLayout(DashboardLayout updatedLayout) {
    final index = _customLayouts.indexWhere((l) => l.id == updatedLayout.id);
    if (index != -1) {
      _customLayouts[index] = updatedLayout.copyWith(
        updatedAt: DateTime.now(),
      );
      _saveCustomLayouts();
      
      // If this is the current layout, update it
      if (_currentLayout.id == updatedLayout.id) {
        _currentLayout = _customLayouts[index];
        _saveCurrentLayout();
      }
      notifyListeners();
    }
  }

  /// Delete a custom layout
  void deleteCustomLayout(String layoutId) {
    _customLayouts.removeWhere((l) => l.id == layoutId);
    _saveCustomLayouts();
    
    // If this was the current layout, switch to default
    if (_currentLayout.id == layoutId) {
      applyLayout(DashboardLayoutPresets.defaultLayout);
    }
    notifyListeners();
  }

  /// Duplicate a layout
  void duplicateLayout(DashboardLayout sourceLayout, String newName) {
    final duplicatedLayout = sourceLayout.copyWith(
      id: DateTime.now().millisecondsSinceEpoch.toString(),
      name: newName,
      createdAt: DateTime.now(),
      updatedAt: DateTime.now(),
    );
    
    _customLayouts.add(duplicatedLayout);
    _saveCustomLayouts();
    notifyListeners();
  }

  /// Update widget in current layout
  void updateWidget(DashboardWidgetConfig updatedWidget) {
    final widgets = List<DashboardWidgetConfig>.from(_currentLayout.widgets);
    final index = widgets.indexWhere((w) => w.id == updatedWidget.id);
    
    if (index != -1) {
      widgets[index] = updatedWidget;
      _currentLayout = _currentLayout.copyWith(
        widgets: widgets,
        updatedAt: DateTime.now(),
      );
      
      _saveCurrentLayout();
      
      // If it's a custom layout, update it in the custom layouts list
      final customIndex = _customLayouts.indexWhere((l) => l.id == _currentLayout.id);
      if (customIndex != -1) {
        updateCustomLayout(_currentLayout);
      }
      
      notifyListeners();
    }
  }

  /// Add widget to current layout
  void addWidget(DashboardWidgetConfig widget) {
    final widgets = List<DashboardWidgetConfig>.from(_currentLayout.widgets);
    final newWidget = widget.copyWith(
      id: DateTime.now().millisecondsSinceEpoch.toString(),
      position: widgets.length,
    );
    widgets.add(newWidget);
    
    _currentLayout = _currentLayout.copyWith(
      widgets: widgets,
      updatedAt: DateTime.now(),
    );
    
    _saveCurrentLayout();
    
    // If it's a custom layout, update it in the custom layouts list
    final customIndex = _customLayouts.indexWhere((l) => l.id == _currentLayout.id);
    if (customIndex != -1) {
      updateCustomLayout(_currentLayout);
    }
    
    notifyListeners();
  }

  /// Remove widget from current layout
  void removeWidget(String widgetId) {
    final widgets = _currentLayout.widgets.where((w) => w.id != widgetId).toList();
    
    // Reposition remaining widgets
    final reorderedWidgets = widgets.asMap().entries.map((entry) {
      return entry.value.copyWith(position: entry.key);
    }).toList();
    
    _currentLayout = _currentLayout.copyWith(
      widgets: reorderedWidgets,
      updatedAt: DateTime.now(),
    );
    
    _saveCurrentLayout();
    
    // If it's a custom layout, update it in the custom layouts list
    final customIndex = _customLayouts.indexWhere((l) => l.id == _currentLayout.id);
    if (customIndex != -1) {
      updateCustomLayout(_currentLayout);
    }
    
    notifyListeners();
  }

  /// Reorder widgets in current layout
  void reorderWidgets(List<DashboardWidgetConfig> reorderedWidgets) {
    // Update positions based on new order
    final widgets = reorderedWidgets.asMap().entries.map((entry) {
      return entry.value.copyWith(position: entry.key);
    }).toList();
    
    _currentLayout = _currentLayout.copyWith(
      widgets: widgets,
      updatedAt: DateTime.now(),
    );
    
    _saveCurrentLayout();
    
    // If it's a custom layout, update it in the custom layouts list
    final customIndex = _customLayouts.indexWhere((l) => l.id == _currentLayout.id);
    if (customIndex != -1) {
      updateCustomLayout(_currentLayout);
    }
    
    notifyListeners();
  }

  /// Update layout settings (columns, spacing)
  void updateLayoutSettings({
    int? columns,
    double? spacing,
  }) {
    _currentLayout = _currentLayout.copyWith(
      columns: columns ?? _currentLayout.columns,
      spacing: spacing ?? _currentLayout.spacing,
      updatedAt: DateTime.now(),
    );
    
    _saveCurrentLayout();
    
    // If it's a custom layout, update it in the custom layouts list
    final customIndex = _customLayouts.indexWhere((l) => l.id == _currentLayout.id);
    if (customIndex != -1) {
      updateCustomLayout(_currentLayout);
    }
    
    notifyListeners();
  }

  /// Get available widget types not in current layout
  List<DashboardWidget> getAvailableWidgetTypes() {
    final currentTypes = _currentLayout.widgets.map((w) => w.type).toSet();
    return DashboardWidget.values.where((type) => !currentTypes.contains(type)).toList();
  }

  /// Check if widget type is in current layout
  bool isWidgetTypeInLayout(DashboardWidget type) {
    return _currentLayout.widgets.any((w) => w.type == type);
  }

  /// Get widget by ID
  DashboardWidgetConfig? getWidgetById(String widgetId) {
    try {
      return _currentLayout.widgets.firstWhere((w) => w.id == widgetId);
    } catch (e) {
      return null;
    }
  }

  /// Reset to default layout
  Future<void> resetToDefault() async {
    await applyLayout(DashboardLayoutPresets.defaultLayout);
  }

  /// Import layout from JSON
  Future<bool> importLayout(String jsonString) async {
    try {
      final layoutData = json.decode(jsonString);
      final layout = DashboardLayout.fromJson(layoutData);
      
      // Create as custom layout with new ID
      final importedLayout = layout.copyWith(
        id: DateTime.now().millisecondsSinceEpoch.toString(),
        name: '${layout.name} (Imported)',
        createdAt: DateTime.now(),
        updatedAt: DateTime.now(),
      );
      
      _customLayouts.add(importedLayout);
      await _saveCustomLayouts();
      notifyListeners();
      
      return true;
    } catch (e) {
      debugPrint('Error importing layout: $e');
      return false;
    }
  }

  /// Export layout to JSON
  String exportLayout(DashboardLayout layout) {
    return json.encode(layout.toJson());
  }

  /// Get layout statistics
  Map<String, dynamic> getLayoutStatistics(DashboardLayout layout) {
    return {
      'totalWidgets': layout.widgets.length,
      'widgetTypes': layout.widgets.map((w) => w.type.name).toSet().length,
      'createdAt': layout.createdAt?.toIso8601String(),
      'updatedAt': layout.updatedAt?.toIso8601String(),
      'columns': layout.columns,
      'spacing': layout.spacing,
    };
  }

  /// Save current layout to preferences
  Future<void> _saveCurrentLayout() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final layoutJson = json.encode(_currentLayout.toJson());
      await prefs.setString(_currentLayoutKey, layoutJson);
    } catch (e) {
      debugPrint('Error saving current layout: $e');
    }
  }

  /// Save custom layouts to preferences
  Future<void> _saveCustomLayouts() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final layoutsJson = json.encode(_customLayouts.map((l) => l.toJson()).toList());
      await prefs.setString(_customLayoutsKey, layoutsJson);
    } catch (e) {
      debugPrint('Error saving custom layouts: $e');
    }
  }
}
