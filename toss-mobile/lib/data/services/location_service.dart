import 'dart:async';
import 'package:flutter/foundation.dart';

import '../../domain/entities/location_entity.dart';
import '../../domain/entities/inventory_movement_entity.dart';
import '../repositories/location_repository.dart';

class LocationService {
  static final LocationService _instance = LocationService._internal();
  factory LocationService() => _instance;
  LocationService._internal();

  final LocationRepository _locationRepository = LocationRepository();
  final StreamController<List<LocationEntity>> _locationsController =
      StreamController<List<LocationEntity>>.broadcast();
  final StreamController<String> _activeLocationController =
      StreamController<String>.broadcast();

  String? _activeLocationId;
  List<LocationEntity> _cachedLocations = [];

  // Streams
  Stream<List<LocationEntity>> get locationsStream => _locationsController.stream;
  Stream<String> get activeLocationStream => _activeLocationController.stream;

  String? get activeLocationId => _activeLocationId;
  LocationEntity? get activeLocation {
    if (_activeLocationId == null) return null;
    try {
      return _cachedLocations.firstWhere((loc) => loc.id == _activeLocationId);
    } catch (e) {
      return null;
    }
  }

  List<LocationEntity> get allLocations => List.unmodifiable(_cachedLocations);

  Future<void> initialize() async {
    try {
      await _loadLocations();
      await _loadActiveLocation();
    } catch (e) {
      debugPrint('LocationService initialization error: $e');
    }
  }

  Future<void> _loadLocations() async {
    try {
      final locations = await _locationRepository.getAllLocations();
      _cachedLocations = locations;
      _locationsController.add(_cachedLocations);
    } catch (e) {
      debugPrint('Error loading locations: $e');
      _locationsController.addError(e);
    }
  }

  Future<void> _loadActiveLocation() async {
    try {
      final activeId = await _locationRepository.getActiveLocationId();
      if (activeId != null) {
        await setActiveLocation(activeId);
      }
    } catch (e) {
      debugPrint('Error loading active location: $e');
    }
  }

  Future<void> setActiveLocation(String locationId) async {
    try {
      // Validate location exists
      final location = _cachedLocations.where((loc) => loc.id == locationId).firstOrNull;
      if (location == null) {
        throw Exception('Location not found: $locationId');
      }

      // Validate location is operational
      if (location.status != LocationStatus.active) {
        throw Exception('Location is not active: ${location.name}');
      }

      _activeLocationId = locationId;
      await _locationRepository.setActiveLocationId(locationId);
      _activeLocationController.add(locationId);

      debugPrint('Active location set to: ${location.name} (${location.code})');
    } catch (e) {
      debugPrint('Error setting active location: $e');
      rethrow;
    }
  }

  Future<LocationEntity> createLocation(LocationEntity location) async {
    try {
      // Validate location code is unique
      final existingLocation = _cachedLocations
          .where((loc) => loc.code.toLowerCase() == location.code.toLowerCase())
          .firstOrNull;
      
      if (existingLocation != null) {
        throw Exception('Location code already exists: ${location.code}');
      }

      final newLocation = await _locationRepository.createLocation(location);
      _cachedLocations.add(newLocation);
      _locationsController.add(_cachedLocations);

      return newLocation;
    } catch (e) {
      debugPrint('Error creating location: $e');
      rethrow;
    }
  }

  Future<LocationEntity> updateLocation(LocationEntity location) async {
    try {
      final updatedLocation = await _locationRepository.updateLocation(location);
      
      final index = _cachedLocations.indexWhere((loc) => loc.id == location.id);
      if (index != -1) {
        _cachedLocations[index] = updatedLocation;
        _locationsController.add(_cachedLocations);
      }

      return updatedLocation;
    } catch (e) {
      debugPrint('Error updating location: $e');
      rethrow;
    }
  }

  Future<void> deleteLocation(String locationId) async {
    try {
      // Check if location has active transfers or inventory
      final hasActiveData = await _locationRepository.hasActiveData(locationId);
      if (hasActiveData) {
        throw Exception('Cannot delete location with active inventory or transfers');
      }

      await _locationRepository.deleteLocation(locationId);
      _cachedLocations.removeWhere((loc) => loc.id == locationId);
      _locationsController.add(_cachedLocations);

      // If this was the active location, clear it
      if (_activeLocationId == locationId) {
        _activeLocationId = null;
        await _locationRepository.clearActiveLocationId();
      }
    } catch (e) {
      debugPrint('Error deleting location: $e');
      rethrow;
    }
  }

  Future<List<LocationEntity>> getLocationsByType(LocationType type) async {
    return _cachedLocations.where((loc) => loc.type == type).toList();
  }

  Future<List<LocationEntity>> getLocationsByStatus(LocationStatus status) async {
    return _cachedLocations.where((loc) => loc.status == status).toList();
  }

  Future<List<LocationEntity>> getLocationsByRegion(String region) async {
    return _cachedLocations.where((loc) => 
        loc.region.toLowerCase() == region.toLowerCase()).toList();
  }

  Future<List<LocationEntity>> searchLocations(String query) async {
    final searchQuery = query.toLowerCase();
    return _cachedLocations.where((loc) =>
        loc.name.toLowerCase().contains(searchQuery) ||
        loc.code.toLowerCase().contains(searchQuery) ||
        loc.address.toLowerCase().contains(searchQuery) ||
        loc.city.toLowerCase().contains(searchQuery)).toList();
  }

  // Transfer Management
  Future<InventoryTransferEntity> createTransfer({
    required String fromLocationId,
    required String toLocationId,
    required TransferType type,
    required List<TransferItemEntity> items,
    String? notes,
    required String requestedById,
  }) async {
    try {
      // Validate source and destination locations
      final fromLocation = await getLocationById(fromLocationId);
      final toLocation = await getLocationById(toLocationId);

      if (fromLocation == null || toLocation == null) {
        throw Exception('Invalid source or destination location');
      }

      if (!fromLocation.canTransferInventory) {
        throw Exception('Source location cannot transfer inventory');
      }

      if (!toLocation.canReceiveTransfers) {
        throw Exception('Destination location cannot receive transfers');
      }

      // Check if locations are connected
      if (!fromLocation.connectedLocations.contains(toLocationId)) {
        throw Exception('Locations are not connected for transfers');
      }

      // Calculate total value and items
      double totalValue = 0;
      int totalItems = 0;
      for (final item in items) {
        totalValue += item.unitCost * item.requestedQuantity;
        totalItems += item.requestedQuantity;
      }

      final transfer = InventoryTransferEntity(
        id: DateTime.now().millisecondsSinceEpoch.toString(),
        transferNumber: _generateTransferNumber(),
        fromLocationId: fromLocationId,
        toLocationId: toLocationId,
        type: type,
        items: items,
        notes: notes,
        requestedById: requestedById,
        requestedAt: DateTime.now(),
        totalItems: totalItems,
        totalValue: totalValue,
      );

      return await _locationRepository.createTransfer(transfer);
    } catch (e) {
      debugPrint('Error creating transfer: $e');
      rethrow;
    }
  }

  Future<List<InventoryTransferEntity>> getTransfersByLocation(String locationId) async {
    return await _locationRepository.getTransfersByLocation(locationId);
  }

  Future<List<InventoryTransferEntity>> getPendingTransfers() async {
    return await _locationRepository.getTransfersByStatus(TransferStatus.pending);
  }

  Future<InventoryTransferEntity> approveTransfer(String transferId, String approvedById) async {
    return await _locationRepository.updateTransferStatus(
      transferId, 
      TransferStatus.approved, 
      approvedById
    );
  }

  Future<InventoryTransferEntity> shipTransfer(String transferId, String shippedById) async {
    return await _locationRepository.updateTransferStatus(
      transferId, 
      TransferStatus.shipped, 
      shippedById
    );
  }

  Future<InventoryTransferEntity> receiveTransfer(
    String transferId, 
    String receivedById,
    Map<String, int> receivedQuantities
  ) async {
    try {
      // Update inventory in both locations
      await _locationRepository.processTransferReceiving(
        transferId, 
        receivedById, 
        receivedQuantities
      );

      return await _locationRepository.updateTransferStatus(
        transferId, 
        TransferStatus.received, 
        receivedById
      );
    } catch (e) {
      debugPrint('Error receiving transfer: $e');
      rethrow;
    }
  }

  Future<InventoryTransferEntity> cancelTransfer(
    String transferId, 
    String reason,
    String cancelledById
  ) async {
    return await _locationRepository.cancelTransfer(transferId, reason, cancelledById);
  }

  // Location Analytics
  Future<Map<String, dynamic>> getLocationAnalytics(String locationId) async {
    try {
      final location = await getLocationById(locationId);
      if (location == null) {
        throw Exception('Location not found');
      }

      // Get sales data for the location
      final salesData = await _locationRepository.getLocationSalesData(locationId);
      
      // Get inventory data
      final inventoryData = await _locationRepository.getLocationInventoryData(locationId);
      
      // Get transfer data
      final transferData = await _locationRepository.getLocationTransferData(locationId);

      return {
        'location': location.toJson(),
        'sales': salesData,
        'inventory': inventoryData,
        'transfers': transferData,
        'performance': {
          'isOpen': location.isOpen,
          'status': location.status.name,
          'salesTarget': location.dailySalesLimit,
          'utilizationRate': _calculateUtilizationRate(salesData, location.dailySalesLimit),
        }
      };
    } catch (e) {
      debugPrint('Error getting location analytics: $e');
      rethrow;
    }
  }

  Future<Map<String, dynamic>> getConsolidatedReport() async {
    try {
      final allLocationData = <String, dynamic>{};
      
      for (final location in _cachedLocations) {
        final analytics = await getLocationAnalytics(location.id);
        allLocationData[location.id] = analytics;
      }

      // Calculate consolidated metrics
      final consolidatedMetrics = _calculateConsolidatedMetrics(allLocationData);

      return {
        'locations': allLocationData,
        'consolidated': consolidatedMetrics,
        'summary': {
          'totalLocations': _cachedLocations.length,
          'activeLocations': _cachedLocations.where((l) => l.status == LocationStatus.active).length,
          'totalSales': consolidatedMetrics['totalSales'],
          'averageSalesPerLocation': consolidatedMetrics['averageSalesPerLocation'],
          'topPerformingLocation': consolidatedMetrics['topPerformingLocation'],
          'generatedAt': DateTime.now().toIso8601String(),
        }
      };
    } catch (e) {
      debugPrint('Error generating consolidated report: $e');
      rethrow;
    }
  }

  // Helper methods
  Future<LocationEntity?> getLocationById(String locationId) async {
    try {
      return _cachedLocations.where((loc) => loc.id == locationId).firstOrNull;
    } catch (e) {
      return null;
    }
  }

  String _generateTransferNumber() {
    final timestamp = DateTime.now().millisecondsSinceEpoch;
    return 'TXF${timestamp.toString().substring(8)}';
  }

  double _calculateUtilizationRate(Map<String, dynamic> salesData, double? salesLimit) {
    if (salesLimit == null || salesLimit == 0) return 0.0;
    final currentSales = salesData['todaySales'] ?? 0.0;
    return (currentSales / salesLimit).clamp(0.0, 1.0);
  }

  Map<String, dynamic> _calculateConsolidatedMetrics(Map<String, dynamic> allLocationData) {
    double totalSales = 0.0;
    double totalInventoryValue = 0.0;
    String topPerformingLocation = '';
    double topSales = 0.0;

    for (final entry in allLocationData.entries) {
      final locationData = entry.value;
      final sales = locationData['sales']['todaySales'] ?? 0.0;
      final inventoryValue = locationData['inventory']['totalValue'] ?? 0.0;

      totalSales += sales;
      totalInventoryValue += inventoryValue;

      if (sales > topSales) {
        topSales = sales;
        topPerformingLocation = locationData['location']['name'];
      }
    }

    final locationCount = allLocationData.length;
    final averageSalesPerLocation = locationCount > 0 ? totalSales / locationCount : 0.0;

    return {
      'totalSales': totalSales,
      'totalInventoryValue': totalInventoryValue,
      'averageSalesPerLocation': averageSalesPerLocation,
      'topPerformingLocation': topPerformingLocation,
      'topPerformingSales': topSales,
    };
  }

  void dispose() {
    _locationsController.close();
    _activeLocationController.close();
  }
}

// Mock Location Repository for demonstration
class LocationRepository {
  final List<LocationEntity> _locations = [];
  final List<InventoryTransferEntity> _transfers = [];
  String? _activeLocationId;

  Future<List<LocationEntity>> getAllLocations() async {
    await Future.delayed(const Duration(milliseconds: 300));
    return List.from(_locations);
  }

  Future<LocationEntity> createLocation(LocationEntity location) async {
    await Future.delayed(const Duration(milliseconds: 300));
    _locations.add(location);
    return location;
  }

  Future<LocationEntity> updateLocation(LocationEntity location) async {
    await Future.delayed(const Duration(milliseconds: 300));
    final index = _locations.indexWhere((l) => l.id == location.id);
    if (index != -1) {
      _locations[index] = location;
    }
    return location;
  }

  Future<void> deleteLocation(String locationId) async {
    await Future.delayed(const Duration(milliseconds: 300));
    _locations.removeWhere((l) => l.id == locationId);
  }

  Future<bool> hasActiveData(String locationId) async {
    await Future.delayed(const Duration(milliseconds: 200));
    // Mock check - in real implementation, check database
    return false;
  }

  Future<String?> getActiveLocationId() async {
    await Future.delayed(const Duration(milliseconds: 100));
    return _activeLocationId;
  }

  Future<void> setActiveLocationId(String locationId) async {
    await Future.delayed(const Duration(milliseconds: 100));
    _activeLocationId = locationId;
  }

  Future<void> clearActiveLocationId() async {
    await Future.delayed(const Duration(milliseconds: 100));
    _activeLocationId = null;
  }

  Future<InventoryTransferEntity> createTransfer(InventoryTransferEntity transfer) async {
    await Future.delayed(const Duration(milliseconds: 300));
    _transfers.add(transfer);
    return transfer;
  }

  Future<List<InventoryTransferEntity>> getTransfersByLocation(String locationId) async {
    await Future.delayed(const Duration(milliseconds: 300));
    return _transfers.where((t) => 
        t.fromLocationId == locationId || t.toLocationId == locationId).toList();
  }

  Future<List<InventoryTransferEntity>> getTransfersByStatus(TransferStatus status) async {
    await Future.delayed(const Duration(milliseconds: 300));
    return _transfers.where((t) => t.status == status).toList();
  }

  Future<InventoryTransferEntity> updateTransferStatus(
    String transferId, 
    TransferStatus status, 
    String userId
  ) async {
    await Future.delayed(const Duration(milliseconds: 300));
    final index = _transfers.indexWhere((t) => t.id == transferId);
    if (index != -1) {
      final transfer = _transfers[index];
      // Update transfer with new status and appropriate timestamps
      // This is simplified - in real implementation, update properly
      _transfers[index] = transfer;
    }
    return _transfers[index];
  }

  Future<void> processTransferReceiving(
    String transferId, 
    String receivedById, 
    Map<String, int> receivedQuantities
  ) async {
    await Future.delayed(const Duration(milliseconds: 500));
    // Mock implementation - in real app, update inventory levels
  }

  Future<InventoryTransferEntity> cancelTransfer(
    String transferId, 
    String reason, 
    String cancelledById
  ) async {
    await Future.delayed(const Duration(milliseconds: 300));
    final index = _transfers.indexWhere((t) => t.id == transferId);
    if (index != -1) {
      // Update transfer status to cancelled
      // This is simplified
    }
    return _transfers[index];
  }

  Future<Map<String, dynamic>> getLocationSalesData(String locationId) async {
    await Future.delayed(const Duration(milliseconds: 300));
    // Mock sales data
    return {
      'todaySales': 2500.0,
      'weekSales': 15000.0,
      'monthSales': 65000.0,
      'transactions': 45,
    };
  }

  Future<Map<String, dynamic>> getLocationInventoryData(String locationId) async {
    await Future.delayed(const Duration(milliseconds: 300));
    // Mock inventory data
    return {
      'totalItems': 234,
      'totalValue': 45000.0,
      'lowStockItems': 12,
      'outOfStockItems': 3,
    };
  }

  Future<Map<String, dynamic>> getLocationTransferData(String locationId) async {
    await Future.delayed(const Duration(milliseconds: 300));
    // Mock transfer data
    return {
      'pendingIncoming': 3,
      'pendingOutgoing': 2,
      'completedThisWeek': 8,
      'totalValue': 12000.0,
    };
  }
}
