import 'dart:io';

import 'package:flutter/foundation.dart';
import 'package:flutter_local_notifications/flutter_local_notifications.dart';
import 'package:timezone/data/latest.dart' as tz;
import 'package:timezone/timezone.dart' as tz;

class NotificationService {
  static final NotificationService _instance = NotificationService._();
  factory NotificationService() => _instance;
  NotificationService._();

  final FlutterLocalNotificationsPlugin _fln = FlutterLocalNotificationsPlugin();
  bool _initialized = false;

  Future<void> init() async {
    if (kIsWeb) return; // skip on web
    if (_initialized) return;

    final androidInit = const AndroidInitializationSettings('@mipmap/ic_launcher');
    final initSettings = InitializationSettings(android: androidInit);
    await _fln.initialize(initSettings);

    // Android 13+ explicit permission
    if (Platform.isAndroid) {
      final androidPlugin = _fln.resolvePlatformSpecificImplementation<AndroidFlutterLocalNotificationsPlugin>();
      await androidPlugin?.requestNotificationsPermission();
    }

    // Default channel
    const AndroidNotificationChannel channel = AndroidNotificationChannel(
      'toss_default',
      'General Notifications',
      description: 'General alerts such as low stock and system notices',
      importance: Importance.defaultImportance,
    );
    final android = _fln.resolvePlatformSpecificImplementation<AndroidFlutterLocalNotificationsPlugin>();
    await android?.createNotificationChannel(channel);

    _initialized = true;

    // Initialize timezone database for scheduled notifications
    tz.initializeTimeZones();
  }

  Future<void> show({
    required int id,
    required String title,
    required String body,
    String channelId = 'toss_default',
  }) async {
    if (kIsWeb) return;
    final details = NotificationDetails(
      android: AndroidNotificationDetails(
        channelId,
        'General Notifications',
        channelDescription: 'General alerts such as low stock and system notices',
        importance: Importance.defaultImportance,
        priority: Priority.defaultPriority,
      ),
    );
    await _fln.show(id, title, body, details);
  }

  Future<void> scheduleAt({
    required int id,
    required String title,
    required String body,
    required DateTime whenLocal,
    String channelId = 'toss_default',
  }) async {
    if (kIsWeb) return;
    final details = NotificationDetails(
      android: AndroidNotificationDetails(
        channelId,
        'General Notifications',
        channelDescription: 'General alerts such as low stock and system notices',
        importance: Importance.defaultImportance,
        priority: Priority.defaultPriority,
      ),
    );
    final tzTime = tz.TZDateTime.from(whenLocal, tz.local);
    await _fln.zonedSchedule(
      id,
      title,
      body,
      tzTime,
      details,
      androidScheduleMode: AndroidScheduleMode.exactAllowWhileIdle,
      uiLocalNotificationDateInterpretation: UILocalNotificationDateInterpretation.absoluteTime,
      matchDateTimeComponents: DateTimeComponents.time,
    );
  }
}


