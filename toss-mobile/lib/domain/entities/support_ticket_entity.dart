import 'package:equatable/equatable.dart';

enum TicketStatus { open, inProgress, pendingCustomer, resolved, closed, cancelled }
enum TicketPriority { low, medium, high, urgent }
enum TicketCategory { 
  general, 
  product, 
  service, 
  billing, 
  technical, 
  complaint, 
  suggestion, 
  return, 
  refund,
  other 
}

class SupportTicketEntity extends Equatable {
  final int? id;
  final String ticketNumber;
  final String title;
  final String description;
  final TicketStatus status;
  final TicketPriority priority;
  final TicketCategory category;
  final int? customerId;
  final String? customerName;
  final String? customerPhone;
  final String? customerEmail;
  final int? assignedToId;
  final String? assignedToName;
  final int? relatedTransactionId;
  final String? relatedTransactionType; // 'sale', 'purchase', 'return', etc.
  final DateTime createdAt;
  final DateTime? updatedAt;
  final DateTime? resolvedAt;
  final DateTime? closedAt;
  final String? resolution;
  final String? notes;
  final List<String> attachments; // URLs or file paths
  final String? createdById;
  final String? createdByName;
  final int? locationId;
  final String? locationName;
  final Map<String, dynamic>? metadata; // Additional custom fields

  const SupportTicketEntity({
    this.id,
    required this.ticketNumber,
    required this.title,
    required this.description,
    this.status = TicketStatus.open,
    this.priority = TicketPriority.medium,
    this.category = TicketCategory.general,
    this.customerId,
    this.customerName,
    this.customerPhone,
    this.customerEmail,
    this.assignedToId,
    this.assignedToName,
    this.relatedTransactionId,
    this.relatedTransactionType,
    required this.createdAt,
    this.updatedAt,
    this.resolvedAt,
    this.closedAt,
    this.resolution,
    this.notes,
    this.attachments = const [],
    this.createdById,
    this.createdByName,
    this.locationId,
    this.locationName,
    this.metadata,
  });

  SupportTicketEntity copyWith({
    int? id,
    String? ticketNumber,
    String? title,
    String? description,
    TicketStatus? status,
    TicketPriority? priority,
    TicketCategory? category,
    int? customerId,
    String? customerName,
    String? customerPhone,
    String? customerEmail,
    int? assignedToId,
    String? assignedToName,
    int? relatedTransactionId,
    String? relatedTransactionType,
    DateTime? createdAt,
    DateTime? updatedAt,
    DateTime? resolvedAt,
    DateTime? closedAt,
    String? resolution,
    String? notes,
    List<String>? attachments,
    String? createdById,
    String? createdByName,
    int? locationId,
    String? locationName,
    Map<String, dynamic>? metadata,
  }) {
    return SupportTicketEntity(
      id: id ?? this.id,
      ticketNumber: ticketNumber ?? this.ticketNumber,
      title: title ?? this.title,
      description: description ?? this.description,
      status: status ?? this.status,
      priority: priority ?? this.priority,
      category: category ?? this.category,
      customerId: customerId ?? this.customerId,
      customerName: customerName ?? this.customerName,
      customerPhone: customerPhone ?? this.customerPhone,
      customerEmail: customerEmail ?? this.customerEmail,
      assignedToId: assignedToId ?? this.assignedToId,
      assignedToName: assignedToName ?? this.assignedToName,
      relatedTransactionId: relatedTransactionId ?? this.relatedTransactionId,
      relatedTransactionType: relatedTransactionType ?? this.relatedTransactionType,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      resolvedAt: resolvedAt ?? this.resolvedAt,
      closedAt: closedAt ?? this.closedAt,
      resolution: resolution ?? this.resolution,
      notes: notes ?? this.notes,
      attachments: attachments ?? this.attachments,
      createdById: createdById ?? this.createdById,
      createdByName: createdByName ?? this.createdByName,
      locationId: locationId ?? this.locationId,
      locationName: locationName ?? this.locationName,
      metadata: metadata ?? this.metadata,
    );
  }

  bool get isOpen => status == TicketStatus.open;
  bool get isInProgress => status == TicketStatus.inProgress;
  bool get isPendingCustomer => status == TicketStatus.pendingCustomer;
  bool get isResolved => status == TicketStatus.resolved;
  bool get isClosed => status == TicketStatus.closed;
  bool get isCancelled => status == TicketStatus.cancelled;
  bool get isActive => [TicketStatus.open, TicketStatus.inProgress, TicketStatus.pendingCustomer].contains(status);

  bool get isHighPriority => priority == TicketPriority.high || priority == TicketPriority.urgent;
  bool get isUrgent => priority == TicketPriority.urgent;

  Duration? get resolutionTime {
    if (resolvedAt != null) {
      return resolvedAt!.difference(createdAt);
    }
    return null;
  }

  Duration get age {
    return DateTime.now().difference(createdAt);
  }

  bool get isOverdue {
    if (isClosed || isCancelled) return false;
    
    final ageInHours = age.inHours;
    switch (priority) {
      case TicketPriority.urgent:
        return ageInHours > 4; // 4 hours
      case TicketPriority.high:
        return ageInHours > 24; // 1 day
      case TicketPriority.medium:
        return ageInHours > 72; // 3 days
      case TicketPriority.low:
        return ageInHours > 168; // 1 week
    }
  }

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'ticketNumber': ticketNumber,
      'title': title,
      'description': description,
      'status': status.name,
      'priority': priority.name,
      'category': category.name,
      'customerId': customerId,
      'customerName': customerName,
      'customerPhone': customerPhone,
      'customerEmail': customerEmail,
      'assignedToId': assignedToId,
      'assignedToName': assignedToName,
      'relatedTransactionId': relatedTransactionId,
      'relatedTransactionType': relatedTransactionType,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt?.toIso8601String(),
      'resolvedAt': resolvedAt?.toIso8601String(),
      'closedAt': closedAt?.toIso8601String(),
      'resolution': resolution,
      'notes': notes,
      'attachments': attachments.join(','),
      'createdById': createdById,
      'createdByName': createdByName,
      'locationId': locationId,
      'locationName': locationName,
      'metadata': metadata?.toString(),
    };
  }

  @override
  List<Object?> get props => [
        id,
        ticketNumber,
        title,
        description,
        status,
        priority,
        category,
        customerId,
        customerName,
        customerPhone,
        customerEmail,
        assignedToId,
        assignedToName,
        relatedTransactionId,
        relatedTransactionType,
        createdAt,
        updatedAt,
        resolvedAt,
        closedAt,
        resolution,
        notes,
        attachments,
        createdById,
        createdByName,
        locationId,
        locationName,
        metadata,
      ];
}
