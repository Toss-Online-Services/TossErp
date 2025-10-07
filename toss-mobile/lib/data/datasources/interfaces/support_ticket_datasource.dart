import '../../../domain/entities/support_ticket_entity.dart';

abstract class SupportTicketDatasource {
  Future<int> createSupportTicket(SupportTicketEntity ticket);
  Future<SupportTicketEntity?> getSupportTicketById(int id);
  Future<List<SupportTicketEntity>> getAllSupportTickets();
  Future<List<SupportTicketEntity>> getSupportTicketsByStatus(TicketStatus status);
  Future<List<SupportTicketEntity>> getSupportTicketsByPriority(TicketPriority priority);
  Future<List<SupportTicketEntity>> getSupportTicketsByCategory(TicketCategory category);
  Future<List<SupportTicketEntity>> getSupportTicketsByCustomer(int customerId);
  Future<List<SupportTicketEntity>> getSupportTicketsByAssignee(int assignedToId);
  Future<List<SupportTicketEntity>> getSupportTicketsByDateRange(DateTime startDate, DateTime endDate);
  Future<List<SupportTicketEntity>> getOverdueTickets();
  Future<List<SupportTicketEntity>> getMyTickets(int assignedToId);
  Future<void> updateSupportTicket(SupportTicketEntity ticket);
  Future<void> deleteSupportTicket(int id);
  Future<void> updateTicketStatus(int id, TicketStatus status);
  Future<void> assignTicket(int id, int assignedToId, String assignedToName);
  Future<void> resolveTicket(int id, String resolution);
  Future<void> closeTicket(int id);
  Future<void> seedSampleSupportTickets();
}
