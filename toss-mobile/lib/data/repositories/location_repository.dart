import '../../domain/entities/location_entity.dart';

abstract class LocationRepository {
  Future<List<LocationEntity>> getAllLocations();
  Future<LocationEntity?> getLocationById(String id);
  Future<String> createLocation(LocationEntity location);
  Future<void> updateLocation(LocationEntity location);
  Future<void> deleteLocation(String id);
  Future<List<LocationEntity>> getActiveLocations();
  Future<LocationEntity?> getCurrentLocation();
}
