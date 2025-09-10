class AppointmentModel {
  int id;
  String? customerName;
  String? customerPhone;
  String serviceName;
  String? staffName;
  String scheduledAt;
  String status; // scheduled, checked_in, completed, cancelled
  String? note;
  int? linkedTransactionId;
  String? createdAt;
  String? updatedAt;

  AppointmentModel({
    required this.id,
    this.customerName,
    this.customerPhone,
    required this.serviceName,
    this.staffName,
    required this.scheduledAt,
    this.status = 'scheduled',
    this.note,
    this.linkedTransactionId,
    this.createdAt,
    this.updatedAt,
  });

  factory AppointmentModel.fromJson(Map<String, dynamic> json) {
    return AppointmentModel(
      id: json['id'],
      customerName: json['customerName'],
      customerPhone: json['customerPhone'],
      serviceName: json['serviceName'],
      staffName: json['staffName'],
      scheduledAt: json['scheduledAt'],
      status: json['status'] ?? 'scheduled',
      note: json['note'],
      linkedTransactionId: json['linkedTransactionId'],
      createdAt: json['createdAt'],
      updatedAt: json['updatedAt'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'customerName': customerName,
      'customerPhone': customerPhone,
      'serviceName': serviceName,
      'staffName': staffName,
      'scheduledAt': scheduledAt,
      'status': status,
      'note': note,
      'linkedTransactionId': linkedTransactionId,
      'createdAt': createdAt,
      'updatedAt': updatedAt,
    };
  }
}


