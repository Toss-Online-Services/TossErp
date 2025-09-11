import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../app/themes/app_sizes.dart';
import '../../domain/entities/product_entity.dart';
import '../providers/products/products_provider.dart';

class MostUsedProductChips extends StatelessWidget {
  final void Function(ProductEntity product) onSelect;
  const MostUsedProductChips({super.key, required this.onSelect});

  @override
  Widget build(BuildContext context) {
    final provider = context.watch<ProductsProvider>();
    final products = provider.allProducts ?? [];
    if (products.isEmpty) return const SizedBox.shrink();

    final width = MediaQuery.of(context).size.width;
    final topCount = width >= 1280
        ? 18
        : width >= 1024
            ? 14
            : width >= 800
                ? 12
                : width >= 600
                    ? 10
                    : 6;
    final mostUsed = [...products]..sort((a, b) => (b.sold ?? 0).compareTo(a.sold ?? 0));
    final chips = mostUsed.take(topCount).toList();

    return Padding(
      padding: const EdgeInsets.fromLTRB(AppSizes.padding, 8, AppSizes.padding, 4),
      child: width < 600
          ? SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: Row(
                children: [
                  for (final p in chips)
                    Padding(
                      padding: const EdgeInsets.only(right: 8),
                      child: ActionChip(
                        label: Text(p.name, overflow: TextOverflow.ellipsis),
                        onPressed: () => onSelect(p),
                      ),
                    ),
                ],
              ),
            )
          : Wrap(
              spacing: 8,
              runSpacing: 8,
              children: [
                for (final p in chips)
                  ActionChip(
                    label: Text(p.name, overflow: TextOverflow.ellipsis),
                    onPressed: () => onSelect(p),
                  ),
              ],
            ),
    );
  }
}


