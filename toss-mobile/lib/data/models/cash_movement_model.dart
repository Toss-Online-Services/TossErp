class CashMovementModel {
  int id;
  int shiftId;
  String type; // 'sale','payout','withdrawal','adjustment'
  int amount;
  String? note;
  String? createdAt;

  CashMovementModel({
    required this.id,
    required this.shiftId,
    required this.type,
    required this.amount,
    this.note,
    this.createdAt,
  });

  factory CashMovementModel.fromJson(Map<String, dynamic> json) {
    return CashMovementModel(
      id: json['id'],
      shiftId: json['shiftId'],
      type: json['type'],
      amount: json['amount'],
      note: json['note'],
      createdAt: json['createdAt'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'shiftId': shiftId,
      'type': type,
      'amount': amount,
      'note': note,
      'createdAt': createdAt,
    };
  }
}


