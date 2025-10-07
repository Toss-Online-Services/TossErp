import '../../core/usecase/usecase.dart';
import '../entities/support_ticket_entity.dart';

abstract class SupportTicketRepository {
  Future<Result<List<SupportTicketEntity>>> getAllSupportTickets();
  Future<Result<SupportTicketEntity?>> getSupportTicketById(int id);
  Future<Result<List<SupportTicketEntity>>> getSupportTicketsByStatus(TicketStatus status);
  Future<Result<List<SupportTicketEntity>>> getSupportTicketsByPriority(TicketPriority priority);
  Future<Result<List<SupportTicketEntity>>> getSupportTicketsByCategory(TicketCategory category);
  Future<Result<List<SupportTicketEntity>>> getSupportTicketsByCustomer(int customerId);
  Future<Result<List<SupportTicketEntity>>> getSupportTicketsByAssignee(int assignedToId);
  Future<Result<List<SupportTicketEntity>>> getSupportTicketsByDateRange(DateTime startDate, DateTime endDate);
  Future<Result<List<SupportTicketEntity>>> getOverdueTickets();
  Future<Result<List<SupportTicketEntity>>> getMyTickets(int assignedToId);
  Future<Result<int>> createSupportTicket(SupportTicketEntity ticket);
  Future<Result<void>> updateSupportTicket(SupportTicketEntity ticket);
  Future<Result<void>> deleteSupportTicket(int id);
  Future<Result<void>> updateTicketStatus(int id, TicketStatus status);
  Future<Result<void>> assignTicket(int id, int assignedToId, String assignedToName);
  Future<Result<void>> resolveTicket(int id, String resolution);
  Future<Result<void>> closeTicket(int id);
  Future<Result<void>> seedSampleSupportTickets();
}
