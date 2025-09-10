import 'dart:async';

import '../../../app/services/connectivity/connectivity_service.dart';
import '../../../service_locator.dart';
import '../../../domain/entities/queued_action_entity.dart';
import '../../../domain/repositories/queued_action_repository.dart';
import '../../utilities/console_log.dart';

class SyncService {
  static final SyncService _instance = SyncService._();
  factory SyncService() => _instance;
  SyncService._();

  Timer? _timer;
  bool _running = false;

  void start() {
    // Fire on connectivity change
    ConnectivityService.initNetworkChecker(onHasInternet: (connected) {
      if (connected) {
        _scheduleImmediate();
      }
    });
    // Periodic backoff timer
    _timer ??= Timer.periodic(const Duration(minutes: 2), (_) => _tick());
  }

  void stop() {
    _timer?.cancel();
    _timer = null;
  }

  Future<void> _scheduleImmediate() async {
    // schedule a short delayed tick to allow network to settle
    Future.delayed(const Duration(seconds: 2), _tick);
  }

  Future<void> _tick() async {
    if (_running || !ConnectivityService.isConnected) return;
    _running = true;
    try {
      final repo = sl<QueuedActionRepository>();
      final res = await repo.getAllQueuedAction();
      if (!res.isSuccess) return;
      final List<QueuedActionEntity> queues = res.data ?? [];
      if (queues.isEmpty) return;
      await repo.executeAllQueuedActions(queues);
    } catch (e) {
      cl('[SyncService.tick].error $e');
    } finally {
      _running = false;
    }
  }
}


