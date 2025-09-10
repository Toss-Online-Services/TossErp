import 'package:blue_thermal_printer/blue_thermal_printer.dart';

class PrintingService {
  static final PrintingService _instance = PrintingService._();
  factory PrintingService() => _instance;
  PrintingService._();

  final BlueThermalPrinter _printer = BlueThermalPrinter.instance;

  Future<List<BluetoothDevice>> getBondedDevices() async {
    try {
      return await _printer.getBondedDevices();
    } catch (_) {
      return [];
    }
  }

  Future<bool> connect(BluetoothDevice device) async {
    try {
      final isConnected = await _printer.isConnected ?? false;
      if (isConnected) return true;
      await _printer.connect(device);
      return await _printer.isConnected ?? false;
    } catch (_) {
      return false;
    }
  }

  Future<void> disconnect() async {
    try {
      await _printer.disconnect();
    } catch (_) {}
  }

  Future<void> printReceipt(String content) async {
    try {
      final isConnected = await _printer.isConnected ?? false;
      if (!isConnected) return;
      // Simple text print; formatting can be enhanced later
      _printer.printCustom(content, 0, 0);
      _printer.paperCut();
    } catch (_) {}
  }
}


