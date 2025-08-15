import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:pos_store/features/auth/presentation/screens/login_screen.dart';
import 'package:pos_store/features/pos/presentation/screens/pos_screen.dart';
import 'package:pos_store/features/reports/presentation/screens/reports_screen.dart';
import 'package:pos_store/features/settings/presentation/screens/settings_screen.dart';
import 'package:pos_store/features/stock/presentation/screens/stock_dashboard_screen.dart';
import 'package:pos_store/shared/widgets/main_layout.dart';

final appRouterProvider = Provider<GoRouter>((ref) {
  return GoRouter(
    initialLocation: '/login',
    routes: [
      // Login route
      GoRoute(
        path: '/login',
        name: 'login',
        builder: (context, state) => const LoginScreen(),
      ),
      
      // Main app shell route
      ShellRoute(
        builder: (context, state, child) => MainLayout(child: child),
        routes: [
          // POS route
          GoRoute(
            path: '/pos',
            name: 'pos',
            builder: (context, state) => const PosScreen(),
          ),
          
          // Stock route
          GoRoute(
            path: '/stock',
            name: 'stock',
            builder: (context, state) => const StockDashboardScreen(),
          ),
          
          // Reports route
          GoRoute(
            path: '/reports',
            name: 'reports',
            builder: (context, state) => const ReportsScreen(),
          ),
          
          // Settings route
          GoRoute(
            path: '/settings',
            name: 'settings',
            builder: (context, state) => const SettingsScreen(),
          ),
        ],
      ),
    ],
  );
});
