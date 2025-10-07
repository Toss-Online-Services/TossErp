import '../../models/shift_model.dart';

abstract class ShiftDatasource {
  Future<int> createShift(ShiftModel shift);
  Future<void> updateShift(ShiftModel shift);
  Future<ShiftModel?> getOpenShift(String userId);
  Future<ShiftModel?> getShift(int id);
}


