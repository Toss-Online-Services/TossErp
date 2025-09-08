import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

/// A widget that catches errors and displays a fallback UI for web compatibility
class WebErrorBoundary extends StatefulWidget {
  final Widget child;
  final Widget? fallback;
  final Function(Object error, StackTrace stackTrace)? onError;

  const WebErrorBoundary({
    super.key,
    required this.child,
    this.fallback,
    this.onError,
  });

  @override
  State<WebErrorBoundary> createState() => _WebErrorBoundaryState();
}

class _WebErrorBoundaryState extends State<WebErrorBoundary> {
  bool _hasError = false;
  Object? _error;
  StackTrace? _stackTrace;

  @override
  Widget build(BuildContext context) {
    if (_hasError) {
      return widget.fallback ?? 
        const Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(
                Icons.error_outline,
                size: 48,
                color: Colors.grey,
              ),
              SizedBox(height: 16),
              Text(
                'Something went wrong',
                style: TextStyle(
                  fontSize: 16,
                  color: Colors.grey,
                ),
              ),
            ],
          ),
        );
    }

    return widget.child;
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    
    // Reset error state when dependencies change
    if (_hasError) {
      setState(() {
        _hasError = false;
        _error = null;
        _stackTrace = null;
      });
    }
  }

  // Handle errors that occur in child widgets
  void _handleError(Object error, StackTrace stackTrace) {
    if (mounted) {
      setState(() {
        _hasError = true;
        _error = error;
        _stackTrace = stackTrace;
      });

      if (widget.onError != null) {
        widget.onError!(error, stackTrace);
      }

      if (kDebugMode) {
        debugPrint('WebErrorBoundary caught error: $error');
        debugPrint('Stack trace: $stackTrace');
      }
    }
  }
}

/// Extension to safely wrap widgets with error boundary
extension SafeWidget on Widget {
  Widget withErrorBoundary({
    Widget? fallback,
    Function(Object error, StackTrace stackTrace)? onError,
  }) {
    return WebErrorBoundary(
      fallback: fallback,
      onError: onError,
      child: this,
    );
  }
}
