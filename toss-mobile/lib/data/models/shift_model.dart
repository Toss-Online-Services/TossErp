class ShiftModel {
  int id;
  String userId;
  String? startedAt;
  int openingFloat;
  String? endedAt;
  int? closingCash;
  int? expectedCash;
  int? variance;
  String status;
  String? createdAt;
  String? updatedAt;

  ShiftModel({
    required this.id,
    required this.userId,
    this.startedAt,
    required this.openingFloat,
    this.endedAt,
    this.closingCash,
    this.expectedCash,
    this.variance,
    this.status = 'open',
    this.createdAt,
    this.updatedAt,
  });

  factory ShiftModel.fromJson(Map<String, dynamic> json) {
    return ShiftModel(
      id: json['id'],
      userId: json['userId'],
      startedAt: json['startedAt'],
      openingFloat: json['openingFloat'],
      endedAt: json['endedAt'],
      closingCash: json['closingCash'],
      expectedCash: json['expectedCash'],
      variance: json['variance'],
      status: json['status'] ?? 'open',
      createdAt: json['createdAt'],
      updatedAt: json['updatedAt'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'userId': userId,
      'startedAt': startedAt,
      'openingFloat': openingFloat,
      'endedAt': endedAt,
      'closingCash': closingCash,
      'expectedCash': expectedCash,
      'variance': variance,
      'status': status,
      'createdAt': createdAt,
      'updatedAt': updatedAt,
    };
  }
}


