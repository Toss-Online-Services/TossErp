class CustomerModel {
  String id; // could be phone or UUID
  String? name;
  String? phone;
  int pointsBalance;
  String? createdAt;
  String? updatedAt;

  CustomerModel({
    required this.id,
    this.name,
    this.phone,
    this.pointsBalance = 0,
    this.createdAt,
    this.updatedAt,
  });

  factory CustomerModel.fromJson(Map<String, dynamic> json) {
    return CustomerModel(
      id: json['id'],
      name: json['name'],
      phone: json['phone'],
      pointsBalance: json['pointsBalance'] ?? 0,
      createdAt: json['createdAt'],
      updatedAt: json['updatedAt'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'phone': phone,
      'pointsBalance': pointsBalance,
      'createdAt': createdAt,
      'updatedAt': updatedAt,
    };
  }
}



