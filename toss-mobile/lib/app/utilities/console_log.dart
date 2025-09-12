import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:logger/logger.dart';

// Log something into console log on debug mode
void cl(dynamic text, {String? title, dynamic json}) {
  // Skip logging during Flutter tests (if available) or always log in debug mode
  if (kDebugMode) {
    String jsonPrettier(jsonObject) {
      var encoder = const JsonEncoder.withIndent("     ");
      return encoder.convert(jsonObject);
    }

    var logger = Logger(
      printer: PrettyPrinter(),
    );

    logger.d(
      '${title != null ? ('$title :') : ''}: $text ${json != null ? jsonPrettier(json) : ''}',
    );
  }
}
