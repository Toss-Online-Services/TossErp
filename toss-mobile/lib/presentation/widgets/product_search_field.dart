import 'dart:async';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../widgets/app_text_field.dart';
import '../providers/products/products_provider.dart';

class ProductSearchField extends StatefulWidget {
  final TextEditingController controller;
  final bool showScanButton;
  final bool showAddServiceButton;
  final VoidCallback? onScan;
  final VoidCallback? onAddService;

  const ProductSearchField({
    super.key,
    required this.controller,
    this.showScanButton = false,
    this.showAddServiceButton = false,
    this.onScan,
    this.onAddService,
  });

  @override
  State<ProductSearchField> createState() => _ProductSearchFieldState();
}

class _ProductSearchFieldState extends State<ProductSearchField> {
  Timer? _debounce;

  @override
  void dispose() {
    _debounce?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final productsProvider = context.read<ProductsProvider>();
    return Row(
      children: [
        Expanded(
          child: AppTextField(
            controller: widget.controller,
            hintText: 'Search Products...',
            type: AppTextFieldType.search,
            textInputAction: TextInputAction.search,
            onChanged: (val) {
              _debounce?.cancel();
              if (kDebugMode) {
                print('🔍 Search input changed: "$val"');
              }
              _debounce = Timer(const Duration(milliseconds: 300), () async {
                if (mounted) {
                  final searchTerm = val.trim().isEmpty ? null : val.trim();
                  if (kDebugMode) {
                    print('🔍 Executing search for: "$searchTerm"');
                  }
                  try {
                    await productsProvider.getAllProducts(contains: searchTerm);
                    if (kDebugMode) {
                      productsProvider.debugSearchState();
                    }
                  } catch (e) {
                    if (kDebugMode) {
                      print('🔍 Search error: $e');
                    }
                  }
                }
              });
            },
            onEditingComplete: () {
              FocusScope.of(context).unfocus();
              final searchText = widget.controller.text.trim();
              productsProvider.getAllProducts(contains: searchText.isEmpty ? null : searchText);
            },
            onTapClearButton: () {
              productsProvider.getAllProducts(); // Clear search when clear button is tapped
            },
          ),
        ),
        if (widget.showScanButton) const SizedBox(width: 8),
        if (widget.showScanButton)
          IconButton(
            tooltip: 'Scan',
            visualDensity: VisualDensity.compact,
            constraints: const BoxConstraints.tightFor(width: 36, height: 36),
            padding: EdgeInsets.zero,
            iconSize: 20,
            onPressed: widget.onScan,
            icon: Icon(Icons.qr_code_scanner, color: Theme.of(context).colorScheme.primary),
          ),
        if (widget.showAddServiceButton) const SizedBox(width: 4),
        if (widget.showAddServiceButton)
          IconButton(
            tooltip: 'Add service',
            visualDensity: VisualDensity.compact,
            constraints: const BoxConstraints.tightFor(width: 36, height: 36),
            padding: EdgeInsets.zero,
            iconSize: 20,
            onPressed: widget.onAddService,
            icon: Icon(Icons.design_services, color: Theme.of(context).colorScheme.primary),
          ),
      ],
    );
  }
}


