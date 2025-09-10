import 'package:flutter/foundation.dart';

import '../../../core/errors/errors.dart';
import '../../../core/usecase/usecase.dart';
import '../../../domain/entities/cash_movement_entity.dart';
import '../../../domain/entities/shift_entity.dart';
import '../../../domain/entities/z_report_entity.dart';
import '../../../domain/repositories/shift_repository.dart';

class ShiftProvider extends ChangeNotifier {
  final ShiftRepository shiftRepository;

  ShiftProvider({required this.shiftRepository});

  ShiftEntity? openShift;
  List<CashMovementEntity> movements = [];
  bool isLoading = false;

  Future<void> loadOpenShift(String userId) async {
    isLoading = true;
    notifyListeners();
    final res = await shiftRepository.getOpenShift(userId);
    if (res.isSuccess) {
      openShift = res.data;
      await loadMovements();
    } else {
      openShift = null;
      movements = [];
    }
    isLoading = false;
    notifyListeners();
  }

  Future<Result<int>> startShift({required String userId, required int openingFloat}) async {
    final res = await shiftRepository.openShift(ShiftEntity(
      userId: userId,
      openingFloat: openingFloat,
      startedAt: DateTime.now().toIso8601String(),
      status: 'open',
    ));
    if (res.isSuccess) {
      await loadOpenShift(userId);
    }
    return res;
  }

  Future<Result<void>> addCashMovement({required String type, required int amount, String? note}) async {
    if (openShift == null) return Result.error(UnknownError(message: 'No open shift'));
    final res = await shiftRepository.addCashMovement(CashMovementEntity(
      shiftId: openShift!.id!,
      type: type,
      amount: amount,
      note: note,
      createdAt: DateTime.now().toIso8601String(),
    ));
    if (res.isSuccess) {
      await loadMovements();
    }
    return Result.success(null);
  }

  Future<void> loadMovements() async {
    if (openShift == null) return;
    final res = await shiftRepository.getShiftMovements(openShift!.id!);
    if (res.isSuccess) movements = res.data ?? [];
    notifyListeners();
  }

  int get totalSalesCash => movements.where((m) => m.type == 'sale').fold(0, (sum, m) => sum + m.amount);
  int get totalPayouts => movements.where((m) => m.type == 'payout').fold(0, (sum, m) => sum + m.amount);
  int get totalWithdrawals => movements.where((m) => m.type == 'withdrawal').fold(0, (sum, m) => sum + m.amount);

  int computeExpectedCash() {
    if (openShift == null) return 0;
    return (openShift!.openingFloat) + totalSalesCash - totalPayouts - totalWithdrawals;
  }

  Future<Result<void>> endShift({required int countedCash}) async {
    if (openShift == null) return Result.error(UnknownError(message: 'No open shift'));
    final expected = computeExpectedCash();
    final variance = countedCash - expected;
    final zSummary = {
      'openingFloat': openShift!.openingFloat,
      'totalSalesCash': totalSalesCash,
      'totalPayouts': totalPayouts,
      'totalWithdrawals': totalWithdrawals,
      'expectedCash': expected,
      'countedCash': countedCash,
      'variance': variance,
      'movements': movements
          .map((m) => {
                'type': m.type,
                'amount': m.amount,
                'note': m.note,
                'createdAt': m.createdAt,
              })
          .toList(),
    };
    final res = await shiftRepository.closeShift(
      openShift!.copyWith(
        endedAt: DateTime.now().toIso8601String(),
        closingCash: countedCash,
        expectedCash: expected,
        variance: variance,
        status: 'closed',
      ),
      zReport: ZReportEntity(shiftId: openShift!.id!, summaryJson: zSummary.toString()),
    );
    return res;
  }
}


