class PaymentModel {
  int id;
  int transactionId;
  String method;
  int amount;
  String? reference;
  String? createdAt;
  String? updatedAt;

  PaymentModel({
    required this.id,
    required this.transactionId,
    required this.method,
    required this.amount,
    this.reference,
    this.createdAt,
    this.updatedAt,
  });

  factory PaymentModel.fromJson(Map<String, dynamic> json) {
    return PaymentModel(
      id: json['id'],
      transactionId: json['transactionId'],
      method: json['method'],
      amount: json['amount'],
      reference: json['reference'],
      createdAt: json['createdAt'],
      updatedAt: json['updatedAt'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'transactionId': transactionId,
      'method': method,
      'amount': amount,
      'reference': reference,
      'createdAt': createdAt,
      'updatedAt': updatedAt,
    };
  }
}


