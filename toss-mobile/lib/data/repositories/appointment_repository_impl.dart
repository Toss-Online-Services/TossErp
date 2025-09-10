import '../../core/errors/errors.dart';
import '../../core/usecase/usecase.dart';
import '../../domain/entities/appointment_entity.dart';
import '../../domain/repositories/appointment_repository.dart';
import '../datasources/local/appointment_local_datasource_impl.dart';
import '../models/appointment_model.dart';

class AppointmentRepositoryImpl extends AppointmentRepository {
  final AppointmentLocalDatasourceImpl appointmentLocalDatasource;

  AppointmentRepositoryImpl({required this.appointmentLocalDatasource});

  @override
  Future<Result<int>> createAppointment(AppointmentEntity appointment) async {
    try {
      final id = await appointmentLocalDatasource.createAppointment(AppointmentModel(
        id: appointment.id ?? DateTime.now().millisecondsSinceEpoch,
        customerName: appointment.customerName,
        customerPhone: appointment.customerPhone,
        serviceName: appointment.serviceName,
        staffName: appointment.staffName,
        scheduledAt: appointment.scheduledAt,
        status: appointment.status,
        note: appointment.note,
        linkedTransactionId: appointment.linkedTransactionId,
        createdAt: appointment.createdAt ?? DateTime.now().toIso8601String(),
        updatedAt: appointment.updatedAt ?? DateTime.now().toIso8601String(),
      ));
      return Result.success(id);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> updateAppointment(AppointmentEntity appointment) async {
    try {
      await appointmentLocalDatasource.updateAppointment(AppointmentModel(
        id: appointment.id!,
        customerName: appointment.customerName,
        customerPhone: appointment.customerPhone,
        serviceName: appointment.serviceName,
        staffName: appointment.staffName,
        scheduledAt: appointment.scheduledAt,
        status: appointment.status,
        note: appointment.note,
        linkedTransactionId: appointment.linkedTransactionId,
        createdAt: appointment.createdAt,
        updatedAt: DateTime.now().toIso8601String(),
      ));
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<AppointmentEntity>>> getAppointmentsByDate(String dateIsoPrefix) async {
    try {
      final rows = await appointmentLocalDatasource.getAppointmentsByDate(dateIsoPrefix);
      return Result.success(rows
          .map((e) => AppointmentEntity(
                id: e.id,
                customerName: e.customerName,
                customerPhone: e.customerPhone,
                serviceName: e.serviceName,
                staffName: e.staffName,
                scheduledAt: e.scheduledAt,
                status: e.status,
                note: e.note,
                linkedTransactionId: e.linkedTransactionId,
                createdAt: e.createdAt,
                updatedAt: e.updatedAt,
              ))
          .toList());
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> linkAppointmentToTransaction(int appointmentId, int transactionId) async {
    try {
      await appointmentLocalDatasource.linkAppointmentToTransaction(appointmentId, transactionId);
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}


