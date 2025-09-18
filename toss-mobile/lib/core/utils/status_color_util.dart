import 'package:flutter/material.dart';

/// Utility function to get status color based on status type.
Color getStatusColor(dynamic status) {
  if (status is EmployeeStatus) {
    switch (status) {
      case EmployeeStatus.active:
        return Colors.green;
      case EmployeeStatus.inactive:
        return Colors.orange;
      case EmployeeStatus.terminated:
        return Colors.red;
      case EmployeeStatus.onLeave:
        return Colors.blue;
    }
  } else if (status is SyncStatus) {
    switch (status) {
      case SyncStatus.pending:
        return Colors.orange;
      case SyncStatus.inProgress:
        return Colors.blue;
      case SyncStatus.completed:
        return Colors.green;
      case SyncStatus.failed:
        return Colors.red;
      case SyncStatus.conflict:
        return Colors.purple;
      case SyncStatus.retrying:
        return Colors.teal;
    }
  }
  return Colors.grey; // Default color for unknown status
}