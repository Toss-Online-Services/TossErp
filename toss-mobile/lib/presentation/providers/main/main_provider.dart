import 'package:flutter/material.dart';

import '../../../app/const/const.dart';
import '../../../app/services/auth/auth_service.dart';
import '../../../app/services/connectivity/connectivity_service.dart';
import '../../../domain/entities/queued_action_entity.dart';
import '../../../domain/entities/user_entity.dart';
import '../../../domain/repositories/product_repository.dart';
import '../../../domain/repositories/queued_action_repository.dart';
import '../../../domain/repositories/transaction_repository.dart';
import '../../../domain/repositories/user_repository.dart';
import '../../../domain/usecases/params/no_params.dart';
import '../../../domain/usecases/product_usecases.dart';
import '../../../domain/usecases/ququed_action_usecases.dart';
import '../../../domain/usecases/transaction_usecases.dart';
import '../../../domain/usecases/user_usecases.dart';
import '../../../service_locator.dart';
import '../products/products_provider.dart';
import '../../../data/repositories/appointment_repository_impl.dart';
import '../../../app/services/notifications/notification_service.dart';
import '../../../app/services/notifications/notification_service.dart';

class MainProvider extends ChangeNotifier {
  final UserRepository userRepository;
  final ProductRepository productRepository;
  final TransactionRepository transactionRepository;
  final QueuedActionRepository queuedActionRepository;

  MainProvider({
    required this.transactionRepository,
    required this.userRepository,
    required this.productRepository,
    required this.queuedActionRepository,
  });

  bool isLoaded = false;
  bool isHasInternet = true;
  bool isHasQueuedActions = false;
  bool isSyncronizing = false;

  UserEntity? user;

  void resetStates() {
    isHasInternet = false;
    isHasInternet = true;
    isHasQueuedActions = false;
    isSyncronizing = false;
    user = null;
  }

  Future<void> initMainProvider(BuildContext context) async {
    ConnectivityService.initNetworkChecker(onHasInternet: (value) => _onHasInternet(context, value));
    await getAndSyncAllUserData();
  }

  @override
  void dispose() {
    // Cancel connectivity subscription to prevent callbacks on disposed widgets
    ConnectivityService.cancelSubs();
    super.dispose();
  }

  Future<void> checkAndSyncAllData(BuildContext context) async {
    final theme = Theme.of(context);
    final messenger = ScaffoldMessenger.of(context);

    // Prevent sync during first time app open
    if (!isLoaded) return;

    if (!ConnectivityService.isConnected) {
      messenger.hideCurrentSnackBar();
      messenger.showSnackBar(const SnackBar(content: Text(SYNC_PENDING_MESSAGE)));
      return;
    }

    try {
      messenger.hideCurrentSnackBar();
      messenger.showSnackBar(const SnackBar(content: Text(SYNCRONIZING_MESSAGE)));

      isSyncronizing = true;
      notifyListeners();

      // Execute all queued actions
      int queueExecutedCount = await executeAllQueuedActions();

      // Sync all data
      await getAndSyncAllUserData();

      messenger.hideCurrentSnackBar();
      messenger.showSnackBar(
        SnackBar(
          content: Text("$SYNCED_MESSAGE! ${queueExecutedCount > 0 ? "$queueExecutedCount queues executed" : ""}"),
        ),
      );

      // Re-check queued actions
      checkIsHasQueuedActions();

      isSyncronizing = false;
      notifyListeners();
    } catch (e) {
      isSyncronizing = false;
      notifyListeners();

      messenger.hideCurrentSnackBar();
      messenger.showSnackBar(
        SnackBar(
          content: Text('Failed to sync data\n\n${e.toString()}'),
          backgroundColor: theme.colorScheme.error,
        ),
      );
    }
  }

  Future<void> getAndSyncAllUserData() async {
    var auth = AuthService().getAuthData();
    if (auth == null) throw 'Unauthenticated';

    // Run multiple futures simultaneusly
    // Because each repository has beed added data checker method
    // The local db will automatically sync with cloud db or vice versa
    var res = await Future.wait([
      GetUserUsecase(userRepository).call(auth.uid),
      SyncAllUserProductsUsecase(productRepository).call(auth.uid),
      SyncAllUserTransactionsUsecase(transactionRepository).call(auth.uid),
    ]);

    // Set and notify user state
    if (res.isNotEmpty && res.first.isSuccess) {
      user = res.first.data as UserEntity?;
      notifyListeners();
    }

    // Refresh products list
    await sl<ProductsProvider>().getAllProducts();
    // After products are loaded, check for low stock and notify once
    await notifyLowStockIfAny();

    // Check queued actions
    checkIsHasQueuedActions();

    // Schedule today's appointment reminders
    await _scheduleTodayAppointments();

    // Notify to MainScreen
    isLoaded = true;
    notifyListeners();
  }

  Future<int> executeAllQueuedActions() async {
    var queuedActions = await getQueuedActions();

    if (queuedActions.isNotEmpty) {
      var res = await ExecuteAllQueuedActionUsecase(queuedActionRepository).call(queuedActions);

      int executedCount = res.data?.where((e) => e).length ?? 0;
      return executedCount;
    }

    return 0;
  }

  Future<List<QueuedActionEntity>> getQueuedActions() async {
    var res = await GetAllQueuedActionUsecase(queuedActionRepository).call(NoParams());
    return res.data ?? [];
  }

  Future<void> _onHasInternet(BuildContext context, bool value) async {
    isHasInternet = value;
    notifyListeners();

    // Only proceed if the context is still mounted and valid
    if (isHasInternet && context.mounted) {
      try {
        await checkAndSyncAllData(context);
      } catch (e) {
        // Handle any errors silently to prevent crashes
        debugPrint('Error in _onHasInternet: $e');
      }
    }
  }

  Future<void> checkIsHasQueuedActions() async {
    isHasQueuedActions = (await getQueuedActions()).isEmpty;
    notifyListeners();
  }

  // After any sync cycle, surface low stock notifications once per app start
  bool _lowStockNotified = false;
  Future<void> notifyLowStockIfAny() async {
    if (_lowStockNotified) return;
    try {
      final productsProvider = sl<ProductsProvider>();
      await productsProvider.getAllProducts();
      final low = productsProvider.lowStockProducts;
      if (low.isNotEmpty) {
        final first = low.first.name;
        final more = low.length - 1;
        final body = more > 0 ? '$first and $more other item(s) are low on stock' : '$first is low on stock';
        await NotificationService().show(id: 1001, title: 'Low stock alert', body: body);
        _lowStockNotified = true;
      }
    } catch (_) {}
  }

  Future<void> _scheduleTodayAppointments() async {
    try {
      final repo = sl<AppointmentRepositoryImpl>();
      final todayPrefix = DateTime.now().toIso8601String().substring(0, 10);
      final res = await repo.getAppointmentsByDate(todayPrefix);
      if (!res.isSuccess || (res.data?.isEmpty ?? true)) return;
      int idBase = 3000;
      for (final a in res.data!) {
        if (a.scheduledAt == null) continue;
        final at = DateTime.tryParse(a.scheduledAt!);
        if (at == null) continue;
        // Only schedule future times today
        if (at.isBefore(DateTime.now())) continue;
        final title = 'Appointment: ${a.serviceName ?? 'Service'}';
        final body = '${a.customerName ?? 'Customer'} at ${at.hour.toString().padLeft(2, '0')}:${at.minute.toString().padLeft(2, '0')}';
        await NotificationService().scheduleAt(
          id: idBase++,
          title: title,
          body: body,
          whenLocal: at,
        );
      }
    } catch (_) {}
  }
}
