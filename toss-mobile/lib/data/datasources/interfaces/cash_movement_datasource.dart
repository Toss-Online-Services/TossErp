import '../../models/cash_movement_model.dart';

abstract class CashMovementDatasource {
  Future<int> createCashMovement(CashMovementModel movement);
  Future<List<CashMovementModel>> getShiftMovements(int shiftId);
}


