import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter_svg/flutter_svg.dart';

// Enum to match the original app_image package
enum ImgProvider {
  networkImage,
  assetImage,
  fileImage,
  memoryImage,
  svgImageNetwork,
  svgImageFile,
  svgImageAsset,
}

class AppImage extends StatefulWidget {
  final dynamic image;
  final List? allImages;
  final ImgProvider? imgProvider;
  final BoxFit fit;
  final Duration fadeInDuration;
  final Curve fadeInCurve;
  final Widget? errorWidget;
  final Widget? placeHolderWidget;
  final bool enableFullScreenView;
  final double? width;
  final double? height;
  final EdgeInsets? margin;
  final Color? backgroundColor;
  final BoxBorder? border;
  final BorderRadius? borderRadius;
  final BuildContext? context;

  const AppImage({
    super.key,
    required this.image,
    this.allImages,
    this.imgProvider,
    this.fit = BoxFit.cover,
    this.fadeInDuration = const Duration(milliseconds: 200),
    this.fadeInCurve = Curves.easeInOut,
    this.errorWidget,
    this.placeHolderWidget,
    this.enableFullScreenView = false,
    this.width,
    this.height,
    this.margin,
    this.backgroundColor,
    this.border,
    this.borderRadius,
    this.context,
  });

  @override
  State<AppImage> createState() => _AppImageState();
}

class _AppImageState extends State<AppImage> with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _fadeAnimation;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: widget.fadeInDuration,
      vsync: this,
    );
    _fadeAnimation = CurvedAnimation(
      parent: _animationController,
      curve: widget.fadeInCurve,
    );
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  ImgProvider _getImageProvider() {
    if (widget.imgProvider != null) {
      return widget.imgProvider!;
    }

    // Auto-detect image provider based on image source
    if (widget.image is String) {
      final imageStr = widget.image as String;
      if (imageStr.startsWith('http://') || imageStr.startsWith('https://')) {
        if (imageStr.toLowerCase().endsWith('.svg')) {
          return ImgProvider.svgImageNetwork;
        }
        return ImgProvider.networkImage;
      } else if (imageStr.startsWith('assets/')) {
        if (imageStr.toLowerCase().endsWith('.svg')) {
          return ImgProvider.svgImageAsset;
        }
        return ImgProvider.assetImage;
      } else {
        // For web, treat non-network URLs as assets by default
        if (kIsWeb) {
          if (imageStr.toLowerCase().endsWith('.svg')) {
            return ImgProvider.svgImageAsset;
          }
          return ImgProvider.assetImage;
        }
        // For mobile, assume it's a file path
        if (imageStr.toLowerCase().endsWith('.svg')) {
          return ImgProvider.svgImageFile;
        }
        return ImgProvider.fileImage;
      }
    } else if (widget.image is Uint8List) {
      return ImgProvider.memoryImage;
    } else if (!kIsWeb && widget.image.toString().contains('File:')) {
      // Only check for File type on non-web platforms
      final filePath = widget.image.toString();
      if (filePath.toLowerCase().endsWith('.svg')) {
        return ImgProvider.svgImageFile;
      }
      return ImgProvider.fileImage;
    }

    return ImgProvider.networkImage; // default
  }

  Widget _buildImage() {
    try {
      final provider = _getImageProvider();
      final imageStr = widget.image is String ? widget.image as String : widget.image.toString();
      
      // Additional null/empty check
      if (imageStr.isEmpty) {
        return widget.errorWidget ?? 
          const Center(
            child: Icon(
              Icons.image,
              size: 32,
              color: Colors.grey,
            ),
          );
      }

      Widget imageWidget;

      switch (provider) {
        case ImgProvider.networkImage:
          imageWidget = CachedNetworkImage(
            imageUrl: imageStr,
            fit: widget.fit,
            placeholder: widget.placeHolderWidget != null ? (context, url) => widget.placeHolderWidget! : null,
            errorWidget: widget.errorWidget != null ? (context, url, error) => widget.errorWidget! : 
              (context, url, error) => const Center(
                child: Icon(
                  Icons.broken_image,
                  size: 32,
                  color: Colors.grey,
                ),
              ),
            fadeInDuration: Duration.zero, // We handle fade ourselves
          );
          break;
        
        case ImgProvider.assetImage:
          imageWidget = Image.asset(
            imageStr,
            fit: widget.fit,
            errorBuilder: widget.errorWidget != null ? (context, error, stackTrace) => widget.errorWidget! :
              (context, error, stackTrace) => const Center(
                child: Icon(
                  Icons.broken_image,
                  size: 32,
                  color: Colors.grey,
                ),
              ),
          );
          break;
          
        case ImgProvider.fileImage:
          if (kIsWeb) {
            // On web, fallback to network image for file paths
            imageWidget = CachedNetworkImage(
              imageUrl: imageStr,
              fit: widget.fit,
              placeholder: widget.placeHolderWidget != null ? (context, url) => widget.placeHolderWidget! : null,
              errorWidget: widget.errorWidget != null ? (context, url, error) => widget.errorWidget! :
                (context, url, error) => const Center(
                  child: Icon(
                    Icons.broken_image,
                    size: 32,
                    color: Colors.grey,
                  ),
                ),
              fadeInDuration: Duration.zero,
            );
          } else {
            // On mobile platforms, handle File objects
            try {
              imageWidget = Image.network(
                imageStr,
                fit: widget.fit,
                errorBuilder: widget.errorWidget != null ? (context, error, stackTrace) => widget.errorWidget! :
                  (context, error, stackTrace) => const Center(
                    child: Icon(
                      Icons.broken_image,
                      size: 32,
                      color: Colors.grey,
                    ),
                  ),
              );
            } catch (e) {
              imageWidget = widget.errorWidget ?? const Center(
                child: Icon(
                  Icons.error,
                  size: 32,
                  color: Colors.red,
                ),
              );
            }
          }
          break;
          
        case ImgProvider.memoryImage:
          imageWidget = Image.memory(
            widget.image as Uint8List,
            fit: widget.fit,
            errorBuilder: widget.errorWidget != null ? (context, error, stackTrace) => widget.errorWidget! :
              (context, error, stackTrace) => const Center(
                child: Icon(
                  Icons.broken_image,
                  size: 32,
                  color: Colors.grey,
                ),
              ),
          );
          break;
          
        case ImgProvider.svgImageNetwork:
          imageWidget = SvgPicture.network(
            imageStr,
            fit: widget.fit,
            placeholderBuilder: widget.placeHolderWidget != null ? (context) => widget.placeHolderWidget! : null,
          );
          break;
          
        case ImgProvider.svgImageAsset:
          imageWidget = SvgPicture.asset(
            imageStr,
            fit: widget.fit,
            placeholderBuilder: widget.placeHolderWidget != null ? (context) => widget.placeHolderWidget! : null,
          );
          break;
          
        case ImgProvider.svgImageFile:
          if (kIsWeb) {
            // On web, fallback to asset or network
            if (imageStr.startsWith('assets/')) {
              imageWidget = SvgPicture.asset(
                imageStr,
                fit: widget.fit,
                placeholderBuilder: widget.placeHolderWidget != null ? (context) => widget.placeHolderWidget! : null,
              );
            } else {
              imageWidget = SvgPicture.network(
                imageStr,
                fit: widget.fit,
                placeholderBuilder: widget.placeHolderWidget != null ? (context) => widget.placeHolderWidget! : null,
              );
            }
          } else {
            // On mobile platforms, this would use SvgPicture.file()
            // For now, fallback to network
            imageWidget = SvgPicture.network(
              imageStr,
              fit: widget.fit,
              placeholderBuilder: widget.placeHolderWidget != null ? (context) => widget.placeHolderWidget! : null,
            );
          }
          break;
      }

      // Start fade animation when image loads
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (mounted && !_animationController.isCompleted) {
          _animationController.forward();
        }
      });

      return FadeTransition(
        opacity: _fadeAnimation,
        child: imageWidget,
      );
    } catch (e) {
      // Return error widget on any exception
      return widget.errorWidget ?? 
        const Center(
          child: Icon(
            Icons.error,
            size: 32,
            color: Colors.red,
          ),
        );
    }
  }

  Widget _buildContainer(Widget child) {
    Widget container = child;

    if (widget.backgroundColor != null || widget.border != null || widget.borderRadius != null) {
      container = Container(
        width: widget.width,
        height: widget.height,
        margin: widget.margin,
        decoration: BoxDecoration(
          color: widget.backgroundColor,
          border: widget.border,
          borderRadius: widget.borderRadius,
        ),
        child: ClipRRect(
          borderRadius: widget.borderRadius ?? BorderRadius.zero,
          child: child,
        ),
      );
    } else {
      container = Container(
        width: widget.width,
        height: widget.height,
        margin: widget.margin,
        child: child,
      );
    }

    return container;
  }

  void _openFullScreen() {
    if (!widget.enableFullScreenView) return;

    Navigator.of(context).push(
      MaterialPageRoute(
        builder: (context) => _FullScreenImageViewer(
          images: widget.allImages ?? [widget.image],
          initialIndex: 0,
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    // Early null check
    if (widget.image == null) {
      return Container(
        width: widget.width,
        height: widget.height,
        margin: widget.margin,
        decoration: BoxDecoration(
          color: widget.backgroundColor ?? Colors.grey[100],
          border: widget.border,
          borderRadius: widget.borderRadius,
        ),
        child: widget.errorWidget ?? 
          const Center(
            child: Icon(
              Icons.image,
              size: 32,
              color: Colors.grey,
            ),
          ),
      );
    }

    try {
      Widget imageWidget = _buildImage();
      Widget container = _buildContainer(imageWidget);

      if (widget.enableFullScreenView) {
        return GestureDetector(
          onTap: _openFullScreen,
          child: container,
        );
      }

      return container;
    } catch (e) {
      // Fallback error widget
      return Container(
        width: widget.width,
        height: widget.height,
        margin: widget.margin,
        decoration: BoxDecoration(
          color: widget.backgroundColor ?? Colors.grey[100],
          border: widget.border,
          borderRadius: widget.borderRadius,
        ),
        child: widget.errorWidget ?? 
          const Center(
            child: Icon(
              Icons.error,
              size: 32,
              color: Colors.red,
            ),
          ),
      );
    }
  }
}

class _FullScreenImageViewer extends StatefulWidget {
  final List images;
  final int initialIndex;

  const _FullScreenImageViewer({
    required this.images,
    required this.initialIndex,
  });

  @override
  State<_FullScreenImageViewer> createState() => _FullScreenImageViewerState();
}

class _FullScreenImageViewerState extends State<_FullScreenImageViewer> {
  late PageController _pageController;
  late int _currentIndex;

  @override
  void initState() {
    super.initState();
    _currentIndex = widget.initialIndex;
    _pageController = PageController(initialPage: widget.initialIndex);
  }

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.black,
      appBar: AppBar(
        backgroundColor: Colors.black,
        iconTheme: const IconThemeData(color: Colors.white),
        title: Text(
          '${_currentIndex + 1} / ${widget.images.length}',
          style: const TextStyle(color: Colors.white),
        ),
      ),
      body: PageView.builder(
        controller: _pageController,
        itemCount: widget.images.length,
        onPageChanged: (index) {
          setState(() {
            _currentIndex = index;
          });
        },
        itemBuilder: (context, index) {
          return Center(
            child: InteractiveViewer(
              child: AppImage(
                image: widget.images[index],
                fit: BoxFit.contain,
                enableFullScreenView: false,
              ),
            ),
          );
        },
      ),
    );
  }
}
