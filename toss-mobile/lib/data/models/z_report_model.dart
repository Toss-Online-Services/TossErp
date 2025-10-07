class ZReportModel {
  int id;
  int shiftId;
  String summaryJson;
  String? createdAt;

  ZReportModel({
    required this.id,
    required this.shiftId,
    required this.summaryJson,
    this.createdAt,
  });

  factory ZReportModel.fromJson(Map<String, dynamic> json) {
    return ZReportModel(
      id: json['id'],
      shiftId: json['shiftId'],
      summaryJson: json['summaryJson'],
      createdAt: json['createdAt'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'shiftId': shiftId,
      'summaryJson': summaryJson,
      'createdAt': createdAt,
    };
  }
}


