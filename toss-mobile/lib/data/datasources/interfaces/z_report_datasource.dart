import '../../models/z_report_model.dart';

abstract class ZReportDatasource {
  Future<int> createZReport(ZReportModel report);
  Future<ZReportModel?> getZReport(int id);
  Future<ZReportModel?> getShiftZReport(int shiftId);
}


