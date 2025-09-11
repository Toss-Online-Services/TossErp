import 'package:equatable/equatable.dart';
import 'product_category_entity.dart';
import 'product_variant_entity.dart';
import 'inventory_batch_entity.dart';

enum ProductType { physical, service, digital }
enum TrackingMethod { none, simple, batch, serial }

class ProductEntity extends Equatable {
  final int? id;
  final String createdById;
  final String name;
  final String? sku;
  final String? barcode;
  final String imageUrl;
  final List<String>? additionalImages;
  final ProductType type;
  final TrackingMethod trackingMethod;
  final int stock;
  final int? sold;
  final int price;
  final int? costPrice;
  final String? description;
  final String? unit; // e.g., 'pieces', 'kg', 'liters'
  final bool isActive;
  final bool isFavorite;
  final int? categoryId;
  final ProductCategoryEntity? category;
  final List<ProductVariantEntity>? variants;
  final List<InventoryBatchEntity>? batches;
  final int? lowStockThreshold;
  final int? reorderPoint;
  final int? reorderQuantity;
  final bool enableLowStockAlert;
  final bool enableExpiryAlert;
  final Map<String, dynamic>? customAttributes;
  final String? createdAt;
  final String? updatedAt;

  const ProductEntity({
    this.id,
    required this.createdById,
    required this.name,
    this.sku,
    this.barcode,
    required this.imageUrl,
    this.additionalImages,
    this.type = ProductType.physical,
    this.trackingMethod = TrackingMethod.simple,
    required this.stock,
    this.sold,
    required this.price,
    this.costPrice,
    this.description,
    this.unit,
    this.isActive = true,
    this.isFavorite = false,
    this.categoryId,
    this.category,
    this.variants,
    this.batches,
    this.lowStockThreshold,
    this.reorderPoint,
    this.reorderQuantity,
    this.enableLowStockAlert = true,
    this.enableExpiryAlert = true,
    this.customAttributes,
    this.createdAt,
    this.updatedAt,
  });

  ProductEntity copyWith({
    int? id,
    String? createdById,
    String? name,
    String? sku,
    String? barcode,
    String? imageUrl,
    List<String>? additionalImages,
    ProductType? type,
    TrackingMethod? trackingMethod,
    int? stock,
    int? sold,
    int? price,
    int? costPrice,
    String? description,
    String? unit,
    bool? isActive,
    bool? isFavorite,
    int? categoryId,
    ProductCategoryEntity? category,
    List<ProductVariantEntity>? variants,
    List<InventoryBatchEntity>? batches,
    int? lowStockThreshold,
    int? reorderPoint,
    int? reorderQuantity,
    bool? enableLowStockAlert,
    bool? enableExpiryAlert,
    Map<String, dynamic>? customAttributes,
    String? createdAt,
    String? updatedAt,
  }) {
    return ProductEntity(
      id: id ?? this.id,
      createdById: createdById ?? this.createdById,
      name: name ?? this.name,
      sku: sku ?? this.sku,
      barcode: barcode ?? this.barcode,
      imageUrl: imageUrl ?? this.imageUrl,
      additionalImages: additionalImages ?? this.additionalImages,
      type: type ?? this.type,
      trackingMethod: trackingMethod ?? this.trackingMethod,
      stock: stock ?? this.stock,
      sold: sold ?? this.sold,
      price: price ?? this.price,
      costPrice: costPrice ?? this.costPrice,
      description: description ?? this.description,
      unit: unit ?? this.unit,
      isActive: isActive ?? this.isActive,
      isFavorite: isFavorite ?? this.isFavorite,
      categoryId: categoryId ?? this.categoryId,
      category: category ?? this.category,
      variants: variants ?? this.variants,
      batches: batches ?? this.batches,
      lowStockThreshold: lowStockThreshold ?? this.lowStockThreshold,
      reorderPoint: reorderPoint ?? this.reorderPoint,
      reorderQuantity: reorderQuantity ?? this.reorderQuantity,
      enableLowStockAlert: enableLowStockAlert ?? this.enableLowStockAlert,
      enableExpiryAlert: enableExpiryAlert ?? this.enableExpiryAlert,
      customAttributes: customAttributes ?? this.customAttributes,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  // Helper methods
  bool get isLowStock => lowStockThreshold != null && stock <= lowStockThreshold!;
  bool get isOutOfStock => stock == 0;
  bool get hasVariants => variants != null && variants!.isNotEmpty;
  bool get hasBatches => batches != null && batches!.isNotEmpty;
  
  int get totalStock {
    if (hasVariants) {
      return variants!.fold(0, (sum, variant) => sum + variant.stock);
    }
    return stock;
  }
  
  double get profitMargin {
    if (costPrice == null || costPrice == 0) return 0.0;
    return ((price - costPrice!) / price) * 100;
  }
  
  List<InventoryBatchEntity> get expiredBatches {
    if (!hasBatches) return [];
    return batches!.where((batch) => batch.isExpired).toList();
  }
  
  List<InventoryBatchEntity> get expiringSoonBatches {
    if (!hasBatches) return [];
    return batches!.where((batch) => batch.isExpiringSoon).toList();
  }

  @override
  List<Object?> get props => [
    id,
    createdById,
    name,
    sku,
    barcode,
    imageUrl,
    additionalImages,
    type,
    trackingMethod,
    stock,
    sold,
    price,
    costPrice,
    description,
    unit,
    isActive,
    isFavorite,
    categoryId,
    category,
    variants,
    batches,
    lowStockThreshold,
    reorderPoint,
    reorderQuantity,
    enableLowStockAlert,
    enableExpiryAlert,
    customAttributes,
    createdAt,
    updatedAt,
  ];
}
