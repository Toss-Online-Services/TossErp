import '../../models/appointment_model.dart';

abstract class AppointmentDatasource {
  Future<int> createAppointment(AppointmentModel appointment);
  Future<void> updateAppointment(AppointmentModel appointment);
  Future<List<AppointmentModel>> getAppointmentsByDate(String dateIsoPrefix);
  Future<void> linkAppointmentToTransaction(int appointmentId, int transactionId);
}


