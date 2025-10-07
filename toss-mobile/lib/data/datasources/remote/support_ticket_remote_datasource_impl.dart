import 'package:cloud_firestore/cloud_firestore.dart';
import '../../../domain/entities/support_ticket_entity.dart';
import '../interfaces/support_ticket_datasource.dart';

class SupportTicketRemoteDatasourceImpl implements SupportTicketDatasource {
  final FirebaseFirestore _firestore;

  SupportTicketRemoteDatasourceImpl(this._firestore);

  @override
  Future<int> createSupportTicket(SupportTicketEntity ticket) async {
    final docRef = await _firestore
        .collection('support_tickets')
        .add(ticket.toMap());
    return docRef.id.hashCode;
  }

  @override
  Future<SupportTicketEntity?> getSupportTicketById(int id) async {
    final snapshot = await _firestore
        .collection('support_tickets')
        .where('id', isEqualTo: id)
        .limit(1)
        .get();
    
    if (snapshot.docs.isNotEmpty) {
      final doc = snapshot.docs.first;
      return _mapToTicket(doc.data(), doc.id);
    }
    return null;
  }

  @override
  Future<List<SupportTicketEntity>> getAllSupportTickets() async {
    final snapshot = await _firestore
        .collection('support_tickets')
        .orderBy('createdAt', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToTicket(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<SupportTicketEntity>> getSupportTicketsByStatus(TicketStatus status) async {
    final snapshot = await _firestore
        .collection('support_tickets')
        .where('status', isEqualTo: status.name)
        .orderBy('createdAt', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToTicket(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<SupportTicketEntity>> getSupportTicketsByPriority(TicketPriority priority) async {
    final snapshot = await _firestore
        .collection('support_tickets')
        .where('priority', isEqualTo: priority.name)
        .orderBy('createdAt', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToTicket(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<SupportTicketEntity>> getSupportTicketsByCategory(TicketCategory category) async {
    final snapshot = await _firestore
        .collection('support_tickets')
        .where('category', isEqualTo: category.name)
        .orderBy('createdAt', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToTicket(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<SupportTicketEntity>> getSupportTicketsByCustomer(int customerId) async {
    final snapshot = await _firestore
        .collection('support_tickets')
        .where('customerId', isEqualTo: customerId)
        .orderBy('createdAt', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToTicket(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<SupportTicketEntity>> getSupportTicketsByAssignee(int assignedToId) async {
    final snapshot = await _firestore
        .collection('support_tickets')
        .where('assignedToId', isEqualTo: assignedToId)
        .orderBy('createdAt', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToTicket(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<SupportTicketEntity>> getSupportTicketsByDateRange(DateTime startDate, DateTime endDate) async {
    final snapshot = await _firestore
        .collection('support_tickets')
        .where('createdAt', isGreaterThanOrEqualTo: startDate.toIso8601String())
        .where('createdAt', isLessThanOrEqualTo: endDate.toIso8601String())
        .orderBy('createdAt', descending: true)
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToTicket(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<SupportTicketEntity>> getOverdueTickets() async {
    final snapshot = await _firestore
        .collection('support_tickets')
        .where('status', whereIn: [
          TicketStatus.open.name,
          TicketStatus.inProgress.name,
          TicketStatus.pendingCustomer.name,
        ])
        .orderBy('priority')
        .orderBy('createdAt')
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToTicket(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<List<SupportTicketEntity>> getMyTickets(int assignedToId) async {
    final snapshot = await _firestore
        .collection('support_tickets')
        .where('assignedToId', isEqualTo: assignedToId)
        .where('status', whereIn: [
          TicketStatus.open.name,
          TicketStatus.inProgress.name,
          TicketStatus.pendingCustomer.name,
        ])
        .orderBy('priority')
        .orderBy('createdAt')
        .get();
    
    return snapshot.docs
        .map((doc) => _mapToTicket(doc.data(), doc.id))
        .toList();
  }

  @override
  Future<void> updateSupportTicket(SupportTicketEntity ticket) async {
    await _firestore
        .collection('support_tickets')
        .doc(ticket.id.toString())
        .update(ticket.toMap());
  }

  @override
  Future<void> deleteSupportTicket(int id) async {
    await _firestore
        .collection('support_tickets')
        .doc(id.toString())
        .delete();
  }

  @override
  Future<void> updateTicketStatus(int id, TicketStatus status) async {
    final updateData = {
      'status': status.name,
      'updatedAt': DateTime.now().toIso8601String(),
    };

    if (status == TicketStatus.resolved) {
      updateData['resolvedAt'] = DateTime.now().toIso8601String();
    } else if (status == TicketStatus.closed) {
      updateData['closedAt'] = DateTime.now().toIso8601String();
    }

    await _firestore
        .collection('support_tickets')
        .doc(id.toString())
        .update(updateData);
  }

  @override
  Future<void> assignTicket(int id, int assignedToId, String assignedToName) async {
    await _firestore
        .collection('support_tickets')
        .doc(id.toString())
        .update({
      'assignedToId': assignedToId,
      'assignedToName': assignedToName,
      'status': TicketStatus.inProgress.name,
      'updatedAt': DateTime.now().toIso8601String(),
    });
  }

  @override
  Future<void> resolveTicket(int id, String resolution) async {
    await _firestore
        .collection('support_tickets')
        .doc(id.toString())
        .update({
      'status': TicketStatus.resolved.name,
      'resolution': resolution,
      'resolvedAt': DateTime.now().toIso8601String(),
      'updatedAt': DateTime.now().toIso8601String(),
    });
  }

  @override
  Future<void> closeTicket(int id) async {
    await _firestore
        .collection('support_tickets')
        .doc(id.toString())
        .update({
      'status': TicketStatus.closed.name,
      'closedAt': DateTime.now().toIso8601String(),
      'updatedAt': DateTime.now().toIso8601String(),
    });
  }

  @override
  Future<void> seedSampleSupportTickets() async {
    // Remote seeding is typically not done in production
    // This method is kept for consistency with the interface
  }

  SupportTicketEntity _mapToTicket(Map<String, dynamic> map, String documentId) {
    return SupportTicketEntity(
      id: map['id'] as int? ?? documentId.hashCode,
      ticketNumber: map['ticketNumber'] as String,
      title: map['title'] as String,
      description: map['description'] as String,
      status: TicketStatus.values.firstWhere(
        (e) => e.name == map['status'],
        orElse: () => TicketStatus.open,
      ),
      priority: TicketPriority.values.firstWhere(
        (e) => e.name == map['priority'],
        orElse: () => TicketPriority.medium,
      ),
      category: TicketCategory.values.firstWhere(
        (e) => e.name == map['category'],
        orElse: () => TicketCategory.general,
      ),
      customerId: map['customerId'] as int?,
      customerName: map['customerName'] as String?,
      customerPhone: map['customerPhone'] as String?,
      customerEmail: map['customerEmail'] as String?,
      assignedToId: map['assignedToId'] as int?,
      assignedToName: map['assignedToName'] as String?,
      relatedTransactionId: map['relatedTransactionId'] as int?,
      relatedTransactionType: map['relatedTransactionType'] as String?,
      createdAt: DateTime.parse(map['createdAt'] as String),
      updatedAt: map['updatedAt'] != null ? DateTime.parse(map['updatedAt'] as String) : null,
      resolvedAt: map['resolvedAt'] != null ? DateTime.parse(map['resolvedAt'] as String) : null,
      closedAt: map['closedAt'] != null ? DateTime.parse(map['closedAt'] as String) : null,
      resolution: map['resolution'] as String?,
      notes: map['notes'] as String?,
      attachments: (map['attachments'] as String?)?.split(',') ?? [],
      createdById: map['createdById'] as String?,
      createdByName: map['createdByName'] as String?,
      locationId: map['locationId'] as int?,
      locationName: map['locationName'] as String?,
      metadata: map['metadata'] != null ? {'data': map['metadata']} : null,
    );
  }
}
