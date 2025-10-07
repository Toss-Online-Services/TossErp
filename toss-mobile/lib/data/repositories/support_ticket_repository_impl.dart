import '../../core/usecase/usecase.dart';
import '../../core/errors/errors.dart';
import '../../domain/entities/support_ticket_entity.dart';
import '../../domain/repositories/support_ticket_repository.dart';
import '../datasources/interfaces/support_ticket_datasource.dart';
import '../datasources/local/queued_action_local_datasource_impl.dart';
import '../models/queued_action_model.dart';

class SupportTicketRepositoryImpl implements SupportTicketRepository {
  final SupportTicketDatasource _localDatasource;
  final SupportTicketDatasource _remoteDatasource;
  final QueuedActionLocalDatasourceImpl _queuedActionDatasource;

  SupportTicketRepositoryImpl(
    this._localDatasource,
    this._remoteDatasource,
    this._queuedActionDatasource,
  );

  @override
  Future<Result<List<SupportTicketEntity>>> getAllSupportTickets() async {
    try {
      final tickets = await _localDatasource.getAllSupportTickets();
      return Result.success(tickets);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<SupportTicketEntity?>> getSupportTicketById(int id) async {
    try {
      final ticket = await _localDatasource.getSupportTicketById(id);
      return Result.success(ticket);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<SupportTicketEntity>>> getSupportTicketsByStatus(TicketStatus status) async {
    try {
      final tickets = await _localDatasource.getSupportTicketsByStatus(status);
      return Result.success(tickets);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<SupportTicketEntity>>> getSupportTicketsByPriority(TicketPriority priority) async {
    try {
      final tickets = await _localDatasource.getSupportTicketsByPriority(priority);
      return Result.success(tickets);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<SupportTicketEntity>>> getSupportTicketsByCategory(TicketCategory category) async {
    try {
      final tickets = await _localDatasource.getSupportTicketsByCategory(category);
      return Result.success(tickets);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<SupportTicketEntity>>> getSupportTicketsByCustomer(int customerId) async {
    try {
      final tickets = await _localDatasource.getSupportTicketsByCustomer(customerId);
      return Result.success(tickets);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<SupportTicketEntity>>> getSupportTicketsByAssignee(int assignedToId) async {
    try {
      final tickets = await _localDatasource.getSupportTicketsByAssignee(assignedToId);
      return Result.success(tickets);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<SupportTicketEntity>>> getSupportTicketsByDateRange(DateTime startDate, DateTime endDate) async {
    try {
      final tickets = await _localDatasource.getSupportTicketsByDateRange(startDate, endDate);
      return Result.success(tickets);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<SupportTicketEntity>>> getOverdueTickets() async {
    try {
      final tickets = await _localDatasource.getOverdueTickets();
      return Result.success(tickets);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<SupportTicketEntity>>> getMyTickets(int assignedToId) async {
    try {
      final tickets = await _localDatasource.getMyTickets(assignedToId);
      return Result.success(tickets);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<int>> createSupportTicket(SupportTicketEntity ticket) async {
    try {
      // Create locally first
      final id = await _localDatasource.createSupportTicket(ticket);
      
      // Queue for remote sync
      final queuedAction = QueuedActionModel(
        id: DateTime.now().millisecondsSinceEpoch,
        repository: 'support_ticket',
        method: 'create',
        param: ticket.toMap().toString(),
        isCritical: true,
        createdAt: DateTime.now().toIso8601String(),
      );
      await _queuedActionDatasource.createQueuedAction(queuedAction);
      
      return Result.success(id);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> updateSupportTicket(SupportTicketEntity ticket) async {
    try {
      // Update locally first
      await _localDatasource.updateSupportTicket(ticket);
      
      // Queue for remote sync
      final queuedAction = QueuedActionModel(
        id: DateTime.now().millisecondsSinceEpoch,
        repository: 'support_ticket',
        method: 'update',
        param: ticket.toMap().toString(),
        isCritical: true,
        createdAt: DateTime.now().toIso8601String(),
      );
      await _queuedActionDatasource.createQueuedAction(queuedAction);
      
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> deleteSupportTicket(int id) async {
    try {
      // Delete locally first
      await _localDatasource.deleteSupportTicket(id);
      
      // Queue for remote sync
      final queuedAction = QueuedActionModel(
        id: DateTime.now().millisecondsSinceEpoch,
        repository: 'support_ticket',
        method: 'delete',
        param: id.toString(),
        isCritical: true,
        createdAt: DateTime.now().toIso8601String(),
      );
      await _queuedActionDatasource.createQueuedAction(queuedAction);
      
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> updateTicketStatus(int id, TicketStatus status) async {
    try {
      // Update locally first
      await _localDatasource.updateTicketStatus(id, status);
      
      // Queue for remote sync
      final queuedAction = QueuedActionModel(
        id: DateTime.now().millisecondsSinceEpoch,
        repository: 'support_ticket',
        method: 'updateStatus',
        param: '{"id": $id, "status": "${status.name}"}',
        isCritical: true,
        createdAt: DateTime.now().toIso8601String(),
      );
      await _queuedActionDatasource.createQueuedAction(queuedAction);
      
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> assignTicket(int id, int assignedToId, String assignedToName) async {
    try {
      // Update locally first
      await _localDatasource.assignTicket(id, assignedToId, assignedToName);
      
      // Queue for remote sync
      final queuedAction = QueuedActionModel(
        id: DateTime.now().millisecondsSinceEpoch,
        repository: 'support_ticket',
        method: 'assign',
        param: '{"id": $id, "assignedToId": $assignedToId, "assignedToName": "$assignedToName"}',
        isCritical: true,
        createdAt: DateTime.now().toIso8601String(),
      );
      await _queuedActionDatasource.createQueuedAction(queuedAction);
      
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> resolveTicket(int id, String resolution) async {
    try {
      // Update locally first
      await _localDatasource.resolveTicket(id, resolution);
      
      // Queue for remote sync
      final queuedAction = QueuedActionModel(
        id: DateTime.now().millisecondsSinceEpoch,
        repository: 'support_ticket',
        method: 'resolve',
        param: '{"id": $id, "resolution": "$resolution"}',
        isCritical: true,
        createdAt: DateTime.now().toIso8601String(),
      );
      await _queuedActionDatasource.createQueuedAction(queuedAction);
      
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> closeTicket(int id) async {
    try {
      // Update locally first
      await _localDatasource.closeTicket(id);
      
      // Queue for remote sync
      final queuedAction = QueuedActionModel(
        id: DateTime.now().millisecondsSinceEpoch,
        repository: 'support_ticket',
        method: 'close',
        param: id.toString(),
        isCritical: true,
        createdAt: DateTime.now().toIso8601String(),
      );
      await _queuedActionDatasource.createQueuedAction(queuedAction);
      
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> seedSampleSupportTickets() async {
    try {
      await _localDatasource.seedSampleSupportTickets();
      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }
}
