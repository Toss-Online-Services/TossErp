import '../../core/usecase/usecase.dart';
import '../entities/appointment_entity.dart';

abstract class AppointmentRepository {
  Future<Result<int>> createAppointment(AppointmentEntity appointment);
  Future<Result<void>> updateAppointment(AppointmentEntity appointment);
  Future<Result<List<AppointmentEntity>>> getAppointmentsByDate(String dateIsoPrefix);
  Future<Result<void>> linkAppointmentToTransaction(int appointmentId, int transactionId);
}


