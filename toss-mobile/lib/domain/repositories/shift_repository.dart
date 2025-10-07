import '../../core/usecase/usecase.dart';
import '../entities/shift_entity.dart';
import '../entities/cash_movement_entity.dart';
import '../entities/z_report_entity.dart';

abstract class ShiftRepository {
  Future<Result<int>> openShift(ShiftEntity shift);
  Future<Result<void>> closeShift(ShiftEntity shift, {ZReportEntity? zReport});
  Future<Result<ShiftEntity>> getOpenShift(String userId);
  Future<Result<int>> addCashMovement(CashMovementEntity movement);
  Future<Result<List<CashMovementEntity>>> getShiftMovements(int shiftId);
  Future<Result<ZReportEntity>> getShiftZReport(int shiftId);
}


