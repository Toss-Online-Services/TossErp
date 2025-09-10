class DiscountModel {
  int id;
  int? transactionId;
  int? orderedProductId;
  String scope; // 'line' or 'cart'
  String type; // 'percentage' or 'fixed'
  int value;
  String? code;
  String? reason;
  String? createdAt;
  String? updatedAt;

  DiscountModel({
    required this.id,
    this.transactionId,
    this.orderedProductId,
    required this.scope,
    required this.type,
    required this.value,
    this.code,
    this.reason,
    this.createdAt,
    this.updatedAt,
  });

  factory DiscountModel.fromJson(Map<String, dynamic> json) {
    return DiscountModel(
      id: json['id'],
      transactionId: json['transactionId'],
      orderedProductId: json['orderedProductId'],
      scope: json['scope'],
      type: json['type'],
      value: json['value'],
      code: json['code'],
      reason: json['reason'],
      createdAt: json['createdAt'],
      updatedAt: json['updatedAt'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'transactionId': transactionId,
      'orderedProductId': orderedProductId,
      'scope': scope,
      'type': type,
      'value': value,
      'code': code,
      'reason': reason,
      'createdAt': createdAt,
      'updatedAt': updatedAt,
    };
  }
}


