import 'package:flutter/material.dart';
import 'app/dashboard/dashboard_manager.dart';

void testDashboardManager() {
  // Test if DashboardManager works now
  final manager = DashboardManager();
  print('DashboardManager created successfully');
  print('Current layout: ${manager.currentLayout.name}');
  print('Is initialized: ${manager.isInitialized}');
}
