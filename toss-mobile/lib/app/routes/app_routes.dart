import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../presentation/screens/account/about_screen.dart';
import '../../presentation/screens/account/account_screen.dart';
import '../../presentation/screens/account/profile_form_screen.dart';
import '../../presentation/screens/auth/sign_in/sign_in_screen.dart';
import '../../presentation/screens/error_handler_screen.dart';
import '../../presentation/screens/home/home_screen.dart';
import '../../presentation/screens/main/main_screen.dart';
import '../../presentation/screens/products/product_detail_screen.dart';
import '../../presentation/screens/products/product_form_screen.dart';
import '../../presentation/screens/products/products_screen.dart';
import '../../presentation/screens/transactions/transaction_detail_screen.dart';
import '../../presentation/screens/transactions/transactions_screen.dart';
import '../../presentation/screens/shifts/shift_screen.dart';
import '../../presentation/screens/home/barcode_scanner_screen.dart';
// Import advanced feature screens
import '../../presentation/screens/customer_management_screen.dart';
import '../../presentation/screens/employee/employee_management_screen.dart';
import '../../presentation/screens/simple_discount_management_screen.dart';
import '../../presentation/screens/simple_analytics_screen.dart';
import '../../presentation/screens/simple_receipt_settings_screen.dart';
import '../../presentation/screens/simple_sync_management_screen.dart';
import '../../presentation/screens/simple_location_management_screen.dart';
// Import theme screens
import '../../presentation/settings/theme_selection_screen.dart';
import '../../presentation/settings/custom_theme_editor_screen.dart';
// Import dashboard screens (temporarily disabled)
// import '../../presentation/settings/dashboard_layout_screen.dart';
// import '../../presentation/settings/dashboard_layout_editor_screen.dart';
import '../services/auth/auth_service.dart';

// App routes
class AppRoutes {
  // This class is not meant to be instatiated or extended; this constructor
  // prevents instantiation and extension.
  AppRoutes._();

  static final rootNavigatorKey = GlobalKey<NavigatorState>(debugLabel: 'root');
  static final navNavigatorKey = GlobalKey<NavigatorState>(debugLabel: 'nav');

  static final router = GoRouter(
    initialLocation: '/home',
    navigatorKey: rootNavigatorKey,
    errorBuilder: (context, state) => ErrorScreen(
      errorMessage: state.error?.message,
    ),
    redirect: (context, state) async {
      // Temporarily disable auth check to test navigation
      return null;
      // if isAuthenticated = false, go to sign-in screen
      // else continue to current intended route screen
      // if (!await AuthService().isAuthenticated()) {
      //   return '/auth/sign-in';
      // } else {
      //   return null;
      // }
    },
    routes: [
      _main,
      _auth,
      _error,
    ],
  );

  static final _error = GoRoute(
    path: '/error',
    builder: (context, state) {
      return ErrorScreen(
        errorDetails: state.extra as FlutterErrorDetails?,
      );
    },
  );

  static final _auth = GoRoute(
    path: '/auth',
    redirect: (context, state) async {
      // Temporarily disable auth check to test navigation
      return null;
      // if isAuthenticated = false, go to intended route screen
      // else back to main screen
      // if (!await AuthService().isAuthenticated()) {
      //   return '/auth/sign-in';
      // } else {
      //   return '/home';
      // }
    },
    routes: [
      _signIn,
    ],
  );

  static final _signIn = GoRoute(
    path: 'sign-in',
    builder: (context, state) {
      return const SignInScreen();
    },
  );

  static final _main = ShellRoute(
    navigatorKey: navNavigatorKey,
    builder: (BuildContext context, GoRouterState state, Widget child) {
      return MainScreen(child: child);
    },
    redirect: (context, state) async {
      // Temporarily disable auth check to test navigation
      return null;
      // if isAuthenticated = true, go to intended route screen
      // else return to auth screen
      // if (!await AuthService().isAuthenticated()) {
      //   return '/auth';
      // } else {
      //   return null;
      // }
    },
    routes: [
      _home,
      _products,
      _transactions,
      _shifts,
      _scan,
      _account,
      // Advanced feature routes
      _customers,
      _staff,
      _discounts,
      _reports,
      _receipts,
      _sync,
      _locations,
      // Theme routes
      _themes,
      // Dashboard customization routes (temporarily disabled)
      // _dashboardLayout,
    ],
  );

  static final _home = GoRoute(
    path: '/home',
    pageBuilder: (context, state) {
      return const NoTransitionPage<void>(
        child: HomeScreen(),
      );
    },
  );

  static final _products = GoRoute(
    path: '/products',
    pageBuilder: (context, state) {
      return const NoTransitionPage<void>(
        child: ProductsScreen(),
      );
    },
    routes: [
      _productCreate,
      _productEdit,
      _productDetail,
    ],
  );

  static final _transactions = GoRoute(
    path: '/transactions',
    pageBuilder: (context, state) {
      return const NoTransitionPage<void>(
        child: TransactionsScreen(),
      );
    },
    routes: [
      _transactionDetail,
    ],
  );

  static final _account = GoRoute(
    path: '/account',
    pageBuilder: (context, state) {
      return const NoTransitionPage<void>(
        child: AccountScreen(),
      );
    },
    routes: [
      _profileEdit,
      _about,
    ],
  );

  static final _productCreate = GoRoute(
    path: 'product-create',
    parentNavigatorKey: navNavigatorKey,
    builder: (context, state) {
      return const ProductFormScreen();
    },
  );

  static final _productEdit = GoRoute(
    path: 'product-edit/:id',
    builder: (context, state) {
      int? id = int.tryParse(state.pathParameters["id"] ?? '');

      if (id == null) {
        throw 'Required productId is not provided!';
      }

      return ProductFormScreen(id: id);
    },
  );

  static final _productDetail = GoRoute(
    path: 'product-detail/:id',
    builder: (context, state) {
      int? id = int.tryParse(state.pathParameters["id"] ?? '');

      if (id == null) {
        throw 'Required productId is not provided!';
      }

      return ProductDetailScreen(id: id);
    },
  );

  static final _transactionDetail = GoRoute(
    path: 'transaction-detail/:id',
    builder: (context, state) {
      int? id = int.tryParse(state.pathParameters["id"] ?? '');

      if (id == null) {
        throw 'Required productId is not provided!';
      }

      return TransactionDetailScreen(id: id);
    },
  );

  static final _scan = GoRoute(
    path: '/scan',
    pageBuilder: (context, state) {
      return const NoTransitionPage<void>(
        child: BarcodeScannerScreen(),
      );
    },
  );

  static final _shifts = GoRoute(
    path: '/shifts',
    pageBuilder: (context, state) {
      return const NoTransitionPage<void>(
        child: ShiftScreen(),
      );
    },
  );

  static final _profileEdit = GoRoute(
    path: 'profile',
    builder: (context, state) {
      return const ProfileFormScreen();
    },
  );

  static final _about = GoRoute(
    path: 'about',
    builder: (context, state) {
      return const AboutScreen();
    },
  );

  // Advanced feature routes
  static final _customers = GoRoute(
    path: '/customers',
    pageBuilder: (context, state) {
      return const NoTransitionPage<void>(
        child: CustomerManagementScreen(),
      );
    },
  );

  static final _staff = GoRoute(
    path: '/staff',
    pageBuilder: (context, state) {
      return const NoTransitionPage<void>(
        child: EmployeeManagementScreen(),
      );
    },
  );

  static final _discounts = GoRoute(
    path: '/discounts',
    pageBuilder: (context, state) {
      return const NoTransitionPage<void>(
        child: SimpleDiscountManagementScreen(),
      );
    },
  );

  static final _reports = GoRoute(
    path: '/reports',
    pageBuilder: (context, state) {
      return const NoTransitionPage<void>(
        child: SimpleAnalyticsScreen(),
      );
    },
  );

  static final _receipts = GoRoute(
    path: '/receipts',
    pageBuilder: (context, state) {
      return const NoTransitionPage<void>(
        child: SimpleReceiptSettingsScreen(),
      );
    },
  );

  static final _sync = GoRoute(
    path: '/sync',
    pageBuilder: (context, state) {
      return const NoTransitionPage<void>(
        child: SimpleSyncManagementScreen(),
      );
    },
  );

  static final _locations = GoRoute(
    path: '/locations',
    pageBuilder: (context, state) {
      return const NoTransitionPage<void>(
        child: SimpleLocationManagementScreen(),
      );
    },
  );

  // Theme management routes
  static final _themes = GoRoute(
    path: '/themes',
    pageBuilder: (context, state) {
      return const NoTransitionPage<void>(
        child: ThemeSelectionScreen(),
      );
    },
    routes: [
      _themeEditor,
    ],
  );

  static final _themeEditor = GoRoute(
    path: 'editor',
    builder: (context, state) {
      return const CustomThemeEditorScreen();
    },
  );

  // Dashboard customization routes (temporarily disabled)
  // static final _dashboardLayout = GoRoute(
  //   path: '/dashboard-layout',
  //   pageBuilder: (context, state) {
  //     return const NoTransitionPage<void>(
  //       child: DashboardLayoutScreen(),
  //     );
  //   },
  //   routes: [
  //     _dashboardLayoutEditor,
  //   ],
  // );

  // static final _dashboardLayoutEditor = GoRoute(
  //   path: 'editor',
  //   builder: (context, state) {
  //     return const DashboardLayoutEditorScreen();
  //   },
  // );
}
