import 'dart:async';
import 'dart:io';
import 'package:connectivity_plus/connectivity_plus.dart';

class ConnectivityRepository {
  final Connectivity _connectivity = Connectivity();
  
  StreamController<bool>? _connectionStatusController;
  StreamSubscription<List<ConnectivityResult>>? _connectivitySubscription;
  
  bool _isConnected = false;
  bool _isInitialized = false;

  Stream<bool> get connectionStatusStream {
    _connectionStatusController ??= StreamController<bool>.broadcast();
    if (!_isInitialized) {
      _initialize();
    }
    return _connectionStatusController!.stream;
  }

  bool get isConnected => _isConnected;

  Future<void> _initialize() async {
    _isInitialized = true;
    
    // Check initial connectivity
    await _updateConnectionStatus();
    
    // Listen for connectivity changes
    _connectivitySubscription = _connectivity.onConnectivityChanged.listen(
      _onConnectivityChanged,
    );
  }

  Future<void> _onConnectivityChanged(List<ConnectivityResult> results) async {
    await _updateConnectionStatus();
  }

  Future<void> _updateConnectionStatus() async {
    final connectivityResults = await _connectivity.checkConnectivity();
    
    // Check if any connection type is available
    final hasConnection = connectivityResults.any((result) => 
        result != ConnectivityResult.none);
    
    if (hasConnection) {
      // Additional check with actual network request
      final actuallyConnected = await _performConnectivityTest();
      _setConnectionStatus(actuallyConnected);
    } else {
      _setConnectionStatus(false);
    }
  }

  Future<bool> _performConnectivityTest() async {
    try {
      // Try to connect to a reliable server
      final result = await InternetAddress.lookup('google.com')
          .timeout(const Duration(seconds: 5));
      
      return result.isNotEmpty && result[0].rawAddress.isNotEmpty;
    } catch (e) {
      return false;
    }
  }

  void _setConnectionStatus(bool isConnected) {
    if (_isConnected != isConnected) {
      _isConnected = isConnected;
      _connectionStatusController?.add(_isConnected);
    }
  }

  Future<bool> isWifiConnected() async {
    final connectivityResults = await _connectivity.checkConnectivity();
    return connectivityResults.contains(ConnectivityResult.wifi);
  }

  Future<bool> isMobileConnected() async {
    final connectivityResults = await _connectivity.checkConnectivity();
    return connectivityResults.contains(ConnectivityResult.mobile);
  }

  Future<List<ConnectivityResult>> getConnectivityResults() async {
    return await _connectivity.checkConnectivity();
  }

  Future<String> getConnectionType() async {
    final results = await _connectivity.checkConnectivity();
    
    if (results.contains(ConnectivityResult.wifi)) {
      return 'WiFi';
    } else if (results.contains(ConnectivityResult.mobile)) {
      return 'Mobile Data';
    } else if (results.contains(ConnectivityResult.ethernet)) {
      return 'Ethernet';
    } else {
      return 'None';
    }
  }

  Future<Map<String, dynamic>> getDetailedConnectivityInfo() async {
    final results = await _connectivity.checkConnectivity();
    final isConnected = await this.isConnected;
    final connectionType = await getConnectionType();
    
    return {
      'isConnected': isConnected,
      'connectionType': connectionType,
      'availableConnections': results.map((r) => r.name).toList(),
      'isWifi': results.contains(ConnectivityResult.wifi),
      'isMobile': results.contains(ConnectivityResult.mobile),
      'timestamp': DateTime.now().toIso8601String(),
    };
  }

  Future<bool> waitForConnection({Duration? timeout}) async {
    if (_isConnected) return true;
    
    final completer = Completer<bool>();
    StreamSubscription<bool>? subscription;
    Timer? timeoutTimer;
    
    subscription = connectionStatusStream.listen((isConnected) {
      if (isConnected) {
        subscription?.cancel();
        timeoutTimer?.cancel();
        if (!completer.isCompleted) {
          completer.complete(true);
        }
      }
    });
    
    if (timeout != null) {
      timeoutTimer = Timer(timeout, () {
        subscription?.cancel();
        if (!completer.isCompleted) {
          completer.complete(false);
        }
      });
    }
    
    return completer.future;
  }

  void dispose() {
    _connectivitySubscription?.cancel();
    _connectionStatusController?.close();
    _connectionStatusController = null;
    _isInitialized = false;
  }
}
