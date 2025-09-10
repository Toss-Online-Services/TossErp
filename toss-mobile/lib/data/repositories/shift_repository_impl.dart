import '../../core/errors/errors.dart';
import '../../core/usecase/usecase.dart';
import '../../domain/entities/cash_movement_entity.dart';
import '../../domain/entities/shift_entity.dart';
import '../../domain/entities/z_report_entity.dart';
import '../../domain/repositories/shift_repository.dart';
import '../datasources/local/cash_movement_local_datasource_impl.dart';
import '../datasources/local/shift_local_datasource_impl.dart';
import '../datasources/local/z_report_local_datasource_impl.dart';
import '../models/cash_movement_model.dart';
import '../models/shift_model.dart';
import '../models/z_report_model.dart';

class ShiftRepositoryImpl extends ShiftRepository {
  final ShiftLocalDatasourceImpl shiftLocalDatasource;
  final CashMovementLocalDatasourceImpl cashMovementLocalDatasource;
  final ZReportLocalDatasourceImpl zReportLocalDatasource;

  ShiftRepositoryImpl({
    required this.shiftLocalDatasource,
    required this.cashMovementLocalDatasource,
    required this.zReportLocalDatasource,
  });

  @override
  Future<Result<int>> openShift(ShiftEntity shift) async {
    try {
      final id = await shiftLocalDatasource.createShift(ShiftModel(
        id: shift.id ?? DateTime.now().millisecondsSinceEpoch,
        userId: shift.userId,
        startedAt: shift.startedAt ?? DateTime.now().toIso8601String(),
        openingFloat: shift.openingFloat,
        status: 'open',
        createdAt: shift.createdAt ?? DateTime.now().toIso8601String(),
        updatedAt: shift.updatedAt ?? DateTime.now().toIso8601String(),
      ));
      return Result.success(id);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> closeShift(ShiftEntity shift, {ZReportEntity? zReport}) async {
    try {
      await shiftLocalDatasource.updateShift(ShiftModel(
        id: shift.id!,
        userId: shift.userId,
        startedAt: shift.startedAt,
        openingFloat: shift.openingFloat,
        endedAt: shift.endedAt ?? DateTime.now().toIso8601String(),
        closingCash: shift.closingCash,
        expectedCash: shift.expectedCash,
        variance: shift.variance,
        status: 'closed',
        createdAt: shift.createdAt,
        updatedAt: DateTime.now().toIso8601String(),
      ));
      if (zReport != null) {
        await zReportLocalDatasource.createZReport(ZReportModel(
          id: zReport.id ?? DateTime.now().millisecondsSinceEpoch,
          shiftId: shift.id!,
          summaryJson: zReport.summaryJson,
          createdAt: zReport.createdAt ?? DateTime.now().toIso8601String(),
        ));
      }
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<ShiftEntity>> getOpenShift(String userId) async {
    try {
      final row = await shiftLocalDatasource.getOpenShift(userId);
      if (row == null) return Result.error(APIError(message: 'No open shift'));
      return Result.success(ShiftEntity(
        id: row.id,
        userId: row.userId,
        startedAt: row.startedAt,
        openingFloat: row.openingFloat,
        endedAt: row.endedAt,
        closingCash: row.closingCash,
        expectedCash: row.expectedCash,
        variance: row.variance,
        status: row.status,
        createdAt: row.createdAt,
        updatedAt: row.updatedAt,
      ));
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<int>> addCashMovement(CashMovementEntity movement) async {
    try {
      final id = await cashMovementLocalDatasource.createCashMovement(CashMovementModel(
        id: movement.id ?? DateTime.now().millisecondsSinceEpoch,
        shiftId: movement.shiftId,
        type: movement.type,
        amount: movement.amount,
        note: movement.note,
        createdAt: movement.createdAt ?? DateTime.now().toIso8601String(),
      ));
      return Result.success(id);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<CashMovementEntity>>> getShiftMovements(int shiftId) async {
    try {
      final rows = await cashMovementLocalDatasource.getShiftMovements(shiftId);
      return Result.success(rows
          .map((e) => CashMovementEntity(
                id: e.id,
                shiftId: e.shiftId,
                type: e.type,
                amount: e.amount,
                note: e.note,
                createdAt: e.createdAt,
              ))
          .toList());
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<ZReportEntity>> getShiftZReport(int shiftId) async {
    try {
      final row = await zReportLocalDatasource.getShiftZReport(shiftId);
      if (row == null) return Result.error(APIError(message: 'No Z-report for shift'));
      return Result.success(ZReportEntity(id: row.id, shiftId: row.shiftId, summaryJson: row.summaryJson, createdAt: row.createdAt));
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}


