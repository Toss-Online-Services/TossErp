import 'package:sqflite/sqflite.dart';
import '../../../app/database/app_database.dart';
import '../../../domain/entities/support_ticket_entity.dart';
import '../interfaces/support_ticket_datasource.dart';

class SupportTicketLocalDatasourceImpl implements SupportTicketDatasource {
  final AppDatabase _appDatabase;

  SupportTicketLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createSupportTicket(SupportTicketEntity ticket) async {
    final db = await _appDatabase.database;
    final id = await db.insert(
      AppDatabaseConfig.supportTicketTableName,
      _ticketToMap(ticket),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
    return id;
  }

  @override
  Future<SupportTicketEntity?> getSupportTicketById(int id) async {
    final db = await _appDatabase.database;
    final res = await db.query(
      AppDatabaseConfig.supportTicketTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
    if (res.isNotEmpty) {
      return _mapToTicket(res.first);
    }
    return null;
  }

  @override
  Future<List<SupportTicketEntity>> getAllSupportTickets() async {
    final db = await _appDatabase.database;
    final res = await db.query(
      AppDatabaseConfig.supportTicketTableName,
      orderBy: 'createdAt DESC',
    );
    return res.map((row) => _mapToTicket(row)).toList();
  }

  @override
  Future<List<SupportTicketEntity>> getSupportTicketsByStatus(TicketStatus status) async {
    final db = await _appDatabase.database;
    final res = await db.query(
      AppDatabaseConfig.supportTicketTableName,
      where: 'status = ?',
      whereArgs: [status.name],
      orderBy: 'createdAt DESC',
    );
    return res.map((row) => _mapToTicket(row)).toList();
  }

  @override
  Future<List<SupportTicketEntity>> getSupportTicketsByPriority(TicketPriority priority) async {
    final db = await _appDatabase.database;
    final res = await db.query(
      AppDatabaseConfig.supportTicketTableName,
      where: 'priority = ?',
      whereArgs: [priority.name],
      orderBy: 'createdAt DESC',
    );
    return res.map((row) => _mapToTicket(row)).toList();
  }

  @override
  Future<List<SupportTicketEntity>> getSupportTicketsByCategory(TicketCategory category) async {
    final db = await _appDatabase.database;
    final res = await db.query(
      AppDatabaseConfig.supportTicketTableName,
      where: 'category = ?',
      whereArgs: [category.name],
      orderBy: 'createdAt DESC',
    );
    return res.map((row) => _mapToTicket(row)).toList();
  }

  @override
  Future<List<SupportTicketEntity>> getSupportTicketsByCustomer(int customerId) async {
    final db = await _appDatabase.database;
    final res = await db.query(
      AppDatabaseConfig.supportTicketTableName,
      where: 'customerId = ?',
      whereArgs: [customerId],
      orderBy: 'createdAt DESC',
    );
    return res.map((row) => _mapToTicket(row)).toList();
  }

  @override
  Future<List<SupportTicketEntity>> getSupportTicketsByAssignee(int assignedToId) async {
    final db = await _appDatabase.database;
    final res = await db.query(
      AppDatabaseConfig.supportTicketTableName,
      where: 'assignedToId = ?',
      whereArgs: [assignedToId],
      orderBy: 'createdAt DESC',
    );
    return res.map((row) => _mapToTicket(row)).toList();
  }

  @override
  Future<List<SupportTicketEntity>> getSupportTicketsByDateRange(DateTime startDate, DateTime endDate) async {
    final db = await _appDatabase.database;
    final res = await db.query(
      AppDatabaseConfig.supportTicketTableName,
      where: 'createdAt BETWEEN ? AND ?',
      whereArgs: [startDate.toIso8601String(), endDate.toIso8601String()],
      orderBy: 'createdAt DESC',
    );
    return res.map((row) => _mapToTicket(row)).toList();
  }

  @override
  Future<List<SupportTicketEntity>> getOverdueTickets() async {
    final db = await _appDatabase.database;
    final res = await db.rawQuery('''
      SELECT * FROM ${AppDatabaseConfig.supportTicketTableName}
      WHERE status IN ('open', 'inProgress', 'pendingCustomer')
      ORDER BY 
        CASE priority
          WHEN 'urgent' THEN 1
          WHEN 'high' THEN 2
          WHEN 'medium' THEN 3
          WHEN 'low' THEN 4
        END,
        createdAt ASC
    ''');
    
    return res.map((row) => _mapToTicket(row)).toList();
  }

  @override
  Future<List<SupportTicketEntity>> getMyTickets(int assignedToId) async {
    final db = await _appDatabase.database;
    final res = await db.query(
      AppDatabaseConfig.supportTicketTableName,
      where: 'assignedToId = ? AND status IN (?, ?, ?)',
      whereArgs: [
        assignedToId,
        TicketStatus.open.name,
        TicketStatus.inProgress.name,
        TicketStatus.pendingCustomer.name,
      ],
      orderBy: 'priority ASC, createdAt ASC',
    );
    return res.map((row) => _mapToTicket(row)).toList();
  }

  @override
  Future<void> updateSupportTicket(SupportTicketEntity ticket) async {
    final db = await _appDatabase.database;
    await db.update(
      AppDatabaseConfig.supportTicketTableName,
      _ticketToMap(ticket),
      where: 'id = ?',
      whereArgs: [ticket.id],
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
  }

  @override
  Future<void> deleteSupportTicket(int id) async {
    final db = await _appDatabase.database;
    await db.delete(
      AppDatabaseConfig.supportTicketTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<void> updateTicketStatus(int id, TicketStatus status) async {
    final db = await _appDatabase.database;
    final updateData = {
      'status': status.name,
      'updatedAt': DateTime.now().toIso8601String(),
    };

    if (status == TicketStatus.resolved) {
      updateData['resolvedAt'] = DateTime.now().toIso8601String();
    } else if (status == TicketStatus.closed) {
      updateData['closedAt'] = DateTime.now().toIso8601String();
    }

    await db.update(
      AppDatabaseConfig.supportTicketTableName,
      updateData,
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<void> assignTicket(int id, int assignedToId, String assignedToName) async {
    final db = await _appDatabase.database;
    await db.update(
      AppDatabaseConfig.supportTicketTableName,
      {
        'assignedToId': assignedToId,
        'assignedToName': assignedToName,
        'status': TicketStatus.inProgress.name,
        'updatedAt': DateTime.now().toIso8601String(),
      },
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<void> resolveTicket(int id, String resolution) async {
    final db = await _appDatabase.database;
    await db.update(
      AppDatabaseConfig.supportTicketTableName,
      {
        'status': TicketStatus.resolved.name,
        'resolution': resolution,
        'resolvedAt': DateTime.now().toIso8601String(),
        'updatedAt': DateTime.now().toIso8601String(),
      },
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<void> closeTicket(int id) async {
    final db = await _appDatabase.database;
    await db.update(
      AppDatabaseConfig.supportTicketTableName,
      {
        'status': TicketStatus.closed.name,
        'closedAt': DateTime.now().toIso8601String(),
        'updatedAt': DateTime.now().toIso8601String(),
      },
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<void> seedSampleSupportTickets() async {
    final sampleTickets = [
      SupportTicketEntity(
        id: DateTime.now().millisecondsSinceEpoch,
        ticketNumber: 'TKT-2024-001',
        title: 'Product quality issue',
        description: 'Customer reported that the product received was damaged during shipping. Need to arrange replacement.',
        status: TicketStatus.open,
        priority: TicketPriority.high,
        category: TicketCategory.product,
        customerId: 1,
        customerName: 'John Doe',
        customerPhone: '+1234567890',
        customerEmail: 'john.doe@email.com',
        relatedTransactionId: 1,
        relatedTransactionType: 'sale',
        createdAt: DateTime.now().subtract(const Duration(days: 2)),
        createdById: 'user1',
        createdByName: 'Store Manager',
        locationId: 1,
        locationName: 'Main Store',
      ),
      SupportTicketEntity(
        id: DateTime.now().millisecondsSinceEpoch + 1,
        ticketNumber: 'TKT-2024-002',
        title: 'Billing inquiry',
        description: 'Customer has questions about their recent invoice and payment terms.',
        status: TicketStatus.inProgress,
        priority: TicketPriority.medium,
        category: TicketCategory.billing,
        customerId: 2,
        customerName: 'Jane Smith',
        customerPhone: '+1234567891',
        customerEmail: 'jane.smith@email.com',
        assignedToId: 1,
        assignedToName: 'Support Agent 1',
        createdAt: DateTime.now().subtract(const Duration(days: 1)),
        createdById: 'user1',
        createdByName: 'Store Manager',
        locationId: 1,
        locationName: 'Main Store',
      ),
      SupportTicketEntity(
        id: DateTime.now().millisecondsSinceEpoch + 2,
        ticketNumber: 'TKT-2024-003',
        title: 'Service complaint',
        description: 'Customer is not satisfied with the service received during their last visit.',
        status: TicketStatus.pendingCustomer,
        priority: TicketPriority.urgent,
        category: TicketCategory.complaint,
        customerId: 3,
        customerName: 'Bob Johnson',
        customerPhone: '+1234567892',
        customerEmail: 'bob.johnson@email.com',
        assignedToId: 2,
        assignedToName: 'Support Agent 2',
        createdAt: DateTime.now().subtract(const Duration(hours: 6)),
        createdById: 'user1',
        createdByName: 'Store Manager',
        locationId: 1,
        locationName: 'Main Store',
      ),
      SupportTicketEntity(
        id: DateTime.now().millisecondsSinceEpoch + 3,
        ticketNumber: 'TKT-2024-004',
        title: 'Return request',
        description: 'Customer wants to return a product within the return policy period.',
        status: TicketStatus.resolved,
        priority: TicketPriority.medium,
        category: TicketCategory.return,
        customerId: 4,
        customerName: 'Alice Brown',
        customerPhone: '+1234567893',
        customerEmail: 'alice.brown@email.com',
        assignedToId: 1,
        assignedToName: 'Support Agent 1',
        relatedTransactionId: 2,
        relatedTransactionType: 'sale',
        createdAt: DateTime.now().subtract(const Duration(days: 3)),
        resolvedAt: DateTime.now().subtract(const Duration(days: 1)),
        resolution: 'Return processed successfully. Refund issued to original payment method.',
        createdById: 'user1',
        createdByName: 'Store Manager',
        locationId: 1,
        locationName: 'Main Store',
      ),
      SupportTicketEntity(
        id: DateTime.now().millisecondsSinceEpoch + 4,
        ticketNumber: 'TKT-2024-005',
        title: 'Technical support',
        description: 'Customer needs help with product setup and configuration.',
        status: TicketStatus.closed,
        priority: TicketPriority.low,
        category: TicketCategory.technical,
        customerId: 5,
        customerName: 'Charlie Wilson',
        customerPhone: '+1234567894',
        customerEmail: 'charlie.wilson@email.com',
        assignedToId: 2,
        assignedToName: 'Support Agent 2',
        createdAt: DateTime.now().subtract(const Duration(days: 5)),
        resolvedAt: DateTime.now().subtract(const Duration(days: 2)),
        closedAt: DateTime.now().subtract(const Duration(days: 1)),
        resolution: 'Provided step-by-step setup instructions via phone call. Customer confirmed successful setup.',
        createdById: 'user1',
        createdByName: 'Store Manager',
        locationId: 1,
        locationName: 'Main Store',
      ),
    ];

    for (final ticket in sampleTickets) {
      await createSupportTicket(ticket);
    }
  }

  Map<String, dynamic> _ticketToMap(SupportTicketEntity ticket) {
    return {
      'id': ticket.id,
      'ticketNumber': ticket.ticketNumber,
      'title': ticket.title,
      'description': ticket.description,
      'status': ticket.status.name,
      'priority': ticket.priority.name,
      'category': ticket.category.name,
      'customerId': ticket.customerId,
      'customerName': ticket.customerName,
      'customerPhone': ticket.customerPhone,
      'customerEmail': ticket.customerEmail,
      'assignedToId': ticket.assignedToId,
      'assignedToName': ticket.assignedToName,
      'relatedTransactionId': ticket.relatedTransactionId,
      'relatedTransactionType': ticket.relatedTransactionType,
      'createdAt': ticket.createdAt.toIso8601String(),
      'updatedAt': ticket.updatedAt?.toIso8601String(),
      'resolvedAt': ticket.resolvedAt?.toIso8601String(),
      'closedAt': ticket.closedAt?.toIso8601String(),
      'resolution': ticket.resolution,
      'notes': ticket.notes,
      'attachments': ticket.attachments.join(','),
      'createdById': ticket.createdById,
      'createdByName': ticket.createdByName,
      'locationId': ticket.locationId,
      'locationName': ticket.locationName,
      'metadata': ticket.metadata?.toString(),
    };
  }

  SupportTicketEntity _mapToTicket(Map<String, dynamic> map) {
    return SupportTicketEntity(
      id: map['id'] as int,
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
