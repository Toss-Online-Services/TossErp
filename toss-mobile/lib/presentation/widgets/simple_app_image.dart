import 'package:flutter/material.dart';
import 'package:cached_network_image/cached_network_image.dart';

// Simplified, web-safe AppImage implementation
class SimpleAppImage extends StatelessWidget {
  final String? image;
  final double? width;
  final double? height;
  final BoxFit fit;
  final Widget? errorWidget;
  final Color? backgroundColor;
  final BoxBorder? border;
  final BorderRadius? borderRadius;

  const SimpleAppImage({
    super.key,
    required this.image,
    this.width,
    this.height,
    this.fit = BoxFit.cover,
    this.errorWidget,
    this.backgroundColor,
    this.border,
    this.borderRadius,
  });

  @override
  Widget build(BuildContext context) {
    final imageUrl = image?.toString() ?? '';
    
    return Container(
      width: width,
      height: height,
      decoration: BoxDecoration(
        color: backgroundColor,
        border: border,
        borderRadius: borderRadius,
      ),
      clipBehavior: borderRadius != null ? Clip.antiAlias : Clip.none,
      child: imageUrl.isNotEmpty && imageUrl.startsWith('http')
          ? CachedNetworkImage(
              imageUrl: imageUrl,
              fit: fit,
              width: width,
              height: height,
              placeholder: (context, url) => Container(
                color: backgroundColor ?? Colors.grey[100],
                child: const Center(
                  child: CircularProgressIndicator(),
                ),
              ),
              errorWidget: (context, url, error) => errorWidget ?? 
                const Center(
                  child: Icon(
                    Icons.broken_image,
                    size: 32,
                    color: Colors.grey,
                  ),
                ),
            )
          : imageUrl.isNotEmpty && imageUrl.startsWith('assets/')
              ? Image.asset(
                  imageUrl,
                  fit: fit,
                  width: width,
                  height: height,
                  errorBuilder: (context, error, stackTrace) => errorWidget ??
                    const Center(
                      child: Icon(
                        Icons.broken_image,
                        size: 32,
                        color: Colors.grey,
                      ),
                    ),
                )
              : errorWidget ??
                Center(
                  child: Icon(
                    Icons.image,
                    size: 32,
                    color: Colors.grey[600],
                  ),
                ),
    );
  }
}
