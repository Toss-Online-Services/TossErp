import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../../../domain/entities/employee_entity.dart';
import '../../../core/services/employee_authentication_service_stub.dart';

class EmployeeManagementScreen extends StatefulWidget {
  const EmployeeManagementScreen({super.key});

  @override
  State<EmployeeManagementScreen> createState() => _EmployeeManagementScreenState();
}

class _EmployeeManagementScreenState extends State<EmployeeManagementScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  final _authService = EmployeeAuthenticationService();

  // Mock data - in real app, this would come from a repository
  List<EmployeeEntity> _employees = [];
  List<WorkShift> _shifts = [];
  EmployeeEntity? _selectedEmployee;

  // Filter states
  EmployeeStatus _statusFilter = EmployeeStatus.active;
  UserRole? _roleFilter;
  String _searchQuery = '';

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
    _loadMockData();
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  void _loadMockData() {
    // Mock employee data
    _employees = [
      EmployeeEntity(
        id: '1',
        employeeNumber: 'EMP001',
        name: 'John Doe',
        phone: '+1234567890',
        email: 'john@example.com',
        role: UserRole.manager,
        status: EmployeeStatus.active,
        permissions: PermissionSet.forRole(UserRole.manager),
        pin: 'hashed_pin',
        biometricType: BiometricType.fingerprint,
        locationIds: ['loc1', 'loc2'],
        hourlyRate: 25.0,
        hireDate: DateTime.now().subtract(const Duration(days: 365)),
        performanceMetrics: {
          'totalSales': 15000.0,
          'transactionsCount': 450,
          'averageTransactionValue': 33.33,
          'customerRating': 4.8,
        },
        createdAt: DateTime.now().subtract(const Duration(days: 365)),
        updatedAt: DateTime.now(),
      ),
      EmployeeEntity(
        id: '2',
        employeeNumber: 'EMP002',
        name: 'Jane Smith',
        phone: '+1234567891',
        email: 'jane@example.com',
        role: UserRole.cashier,
        status: EmployeeStatus.active,
        permissions: PermissionSet.forRole(UserRole.cashier),
        pin: 'hashed_pin2',
        biometricType: BiometricType.none,
        locationIds: ['loc1'],
        hourlyRate: 18.0,
        hireDate: DateTime.now().subtract(const Duration(days: 180)),
        performanceMetrics: {
          'totalSales': 8500.0,
          'transactionsCount': 320,
          'averageTransactionValue': 26.56,
          'customerRating': 4.6,
        },
        createdAt: DateTime.now().subtract(const Duration(days: 180)),
        updatedAt: DateTime.now(),
      ),
      EmployeeEntity(
        id: '3',
        employeeNumber: 'EMP003',
        name: 'Bob Johnson',
        phone: '+1234567892',
        role: UserRole.stockClerk,
        status: EmployeeStatus.active,
        permissions: PermissionSet.forRole(UserRole.stockClerk),
        locationIds: ['loc1'],
        hourlyRate: 16.0,
        hireDate: DateTime.now().subtract(const Duration(days: 90)),
        isTrainingMode: true,
        performanceMetrics: {
          'stockReceived': 150,
          'adjustmentsMade': 25,
          'accuracyRate': 0.96,
        },
        createdAt: DateTime.now().subtract(const Duration(days: 90)),
        updatedAt: DateTime.now(),
      ),
    ];

    // Mock shift data
    _shifts = [
      WorkShift(
        id: '1',
        employeeId: '1',
        startTime: DateTime.now().subtract(const Duration(hours: 8)),
        endTime: DateTime.now().subtract(const Duration(hours: 1)),
        locationId: 'loc1',
        breakDuration: 30,
        metrics: {
          'sales': 1250.0,
          'transactions': 15,
          'returns': 1,
        },
      ),
      WorkShift(
        id: '2',
        employeeId: '2',
        startTime: DateTime.now().subtract(const Duration(hours: 6)),
        locationId: 'loc1',
        metrics: {
          'sales': 890.0,
          'transactions': 12,
        },
      ),
    ];
  }

  List<EmployeeEntity> get _filteredEmployees {
    return _employees.where((employee) {
      if (employee.status != _statusFilter && _statusFilter != EmployeeStatus.active) {
        return false;
      }
      if (_roleFilter != null && employee.role != _roleFilter) {
        return false;
      }
      if (_searchQuery.isNotEmpty) {
        return employee.name.toLowerCase().contains(_searchQuery.toLowerCase()) ||
               employee.employeeNumber.toLowerCase().contains(_searchQuery.toLowerCase()) ||
               employee.phone.contains(_searchQuery);
      }
      return true;
    }).toList();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Employee Management'),
        actions: [
          IconButton(
            onPressed: _addNewEmployee,
            icon: const Icon(Icons.person_add),
            tooltip: 'Add Employee',
          ),
          IconButton(
            onPressed: () => _showExportOptions(),
            icon: const Icon(Icons.download),
            tooltip: 'Export Data',
          ),
        ],
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(icon: Icon(Icons.people), text: 'Employees'),
            Tab(icon: Icon(Icons.access_time), text: 'Time Tracking'),
            Tab(icon: Icon(Icons.analytics), text: 'Performance'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          _buildEmployeesTab(),
          _buildTimeTrackingTab(),
          _buildPerformanceTab(),
        ],
      ),
    );
  }

  Widget _buildEmployeesTab() {
    return Column(
      children: [
        _buildEmployeeFilters(),
        Expanded(
          child: _filteredEmployees.isEmpty
              ? const Center(
                  child: Text('No employees found'),
                )
              : ListView.builder(
                  itemCount: _filteredEmployees.length,
                  itemBuilder: (context, index) {
                    return _buildEmployeeCard(_filteredEmployees[index]);
                  },
                ),
        ),
      ],
    );
  }

  Widget _buildEmployeeFilters() {
    return Container(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          TextField(
            decoration: const InputDecoration(
              hintText: 'Search employees...',
              prefixIcon: Icon(Icons.search),
              border: OutlineInputBorder(),
            ),
            onChanged: (value) {
              setState(() {
                _searchQuery = value;
              });
            },
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              Expanded(
                child: DropdownButtonFormField<EmployeeStatus>(
                  value: _statusFilter,
                  decoration: const InputDecoration(
                    labelText: 'Status',
                    border: OutlineInputBorder(),
                  ),
                  items: EmployeeStatus.values.map((status) {
                    return DropdownMenuItem(
                      value: status,
                      child: Text(_getStatusName(status)),
                    );
                  }).toList(),
                  onChanged: (value) {
                    setState(() {
                      _statusFilter = value!;
                    });
                  },
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: DropdownButtonFormField<UserRole?>(
                  value: _roleFilter,
                  decoration: const InputDecoration(
                    labelText: 'Role',
                    border: OutlineInputBorder(),
                  ),
                  items: [
                    const DropdownMenuItem(value: null, child: Text('All Roles')),
                    ...UserRole.values.map((role) {
                      return DropdownMenuItem(
                        value: role,
                        child: Text(_getRoleName(role)),
                      );
                    }),
                  ],
                  onChanged: (value) {
                    setState(() {
                      _roleFilter = value;
                    });
                  },
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildEmployeeCard(EmployeeEntity employee) {
    final isCurrentlyWorking = _shifts.any((shift) => 
        shift.employeeId == employee.id && shift.isActive);
    final isDarkMode = Theme.of(context).brightness == Brightness.dark;
    final screenWidth = MediaQuery.of(context).size.width;
    final isTablet = screenWidth > 600;

    return Card(
      margin: EdgeInsets.symmetric(
        horizontal: isTablet ? 24 : 16, 
        vertical: isTablet ? 12 : 8,
      ),
      child: InkWell(
        onTap: () => _showEmployeeDetails(employee),
        child: Padding(
          padding: EdgeInsets.all(isTablet ? 20 : 16),
          child: Row(
            children: [
              CircleAvatar(
                radius: isTablet ? 28 : 24,
                backgroundColor: _getStatusColor(employee.status),
                child: Text(
                  employee.name.isNotEmpty 
                      ? employee.name[0].toUpperCase()
                      : employee.employeeNumber[0],
                  style: TextStyle(
                    color: Colors.white,
                    fontWeight: FontWeight.bold,
                    fontSize: isTablet ? 18 : 16,
                  ),
                ),
              ),
              SizedBox(width: isTablet ? 20 : 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      employee.name,
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.bold,
                        fontSize: isTablet ? 18 : 16,
                      ),
                    ),
                    Text(
                      '${employee.employeeNumber} • ${employee.roleDisplayName}',
                      style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                        color: isDarkMode ? Colors.grey[400] : Colors.grey[600],
                        fontSize: isTablet ? 14 : 12,
                      ),
                    ),
                    if (employee.phone.isNotEmpty)
                      Text(
                        employee.phone,
                        style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: isDarkMode ? Colors.grey[400] : Colors.grey[600],
                          fontSize: isTablet ? 12 : 11,
                        ),
                      ),
                    SizedBox(height: isTablet ? 8 : 4),
                    Wrap(
                      spacing: 8,
                      runSpacing: 4,
                      children: [
                        if (isCurrentlyWorking)
                          Container(
                            padding: EdgeInsets.symmetric(
                              horizontal: isTablet ? 8 : 6, 
                              vertical: isTablet ? 3 : 2,
                            ),
                            decoration: BoxDecoration(
                              color: Colors.green,
                              borderRadius: BorderRadius.circular(8),
                            ),
                            child: Text(
                              'Working',
                              style: TextStyle(
                                color: Colors.white,
                                fontSize: isTablet ? 11 : 10,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ),
                        if (employee.isTrainingMode)
                          Container(
                            padding: EdgeInsets.symmetric(
                              horizontal: isTablet ? 8 : 6, 
                              vertical: isTablet ? 3 : 2,
                            ),
                            decoration: BoxDecoration(
                              color: Colors.orange,
                              borderRadius: BorderRadius.circular(8),
                            ),
                            child: Text(
                              'Training',
                              style: TextStyle(
                                color: Colors.white,
                                fontSize: isTablet ? 11 : 10,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ),
                        if (employee.biometricType != BiometricType.none)
                          Icon(
                            Icons.fingerprint,
                            size: isTablet ? 18 : 16,
                            color: Colors.blue,
                          ),
                      ],
                    ),
                  ],
                ),
              ),
              Column(
                crossAxisAlignment: CrossAxisAlignment.end,
                children: [
                  Text(
                    '\$${employee.hourlyRate.toStringAsFixed(2)}/hr',
                    style: Theme.of(context).textTheme.titleSmall?.copyWith(
                      fontWeight: FontWeight.bold,
                      color: Theme.of(context).colorScheme.primary,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    _getStatusName(employee.status),
                    style: TextStyle(
                      color: _getStatusColor(employee.status),
                      fontWeight: FontWeight.w500,
                      fontSize: 12,
                    ),
                  ),
                ],
              ),
              const SizedBox(width: 8),
              PopupMenuButton<String>(
                onSelected: (value) => _handleEmployeeAction(value, employee),
                itemBuilder: (context) => [
                  const PopupMenuItem(
                    value: 'edit',
                    child: ListTile(
                      leading: Icon(Icons.edit),
                      title: Text('Edit'),
                      contentPadding: EdgeInsets.zero,
                    ),
                  ),
                  const PopupMenuItem(
                    value: 'permissions',
                    child: ListTile(
                      leading: Icon(Icons.security),
                      title: Text('Permissions'),
                      contentPadding: EdgeInsets.zero,
                    ),
                  ),
                  const PopupMenuItem(
                    value: 'biometric',
                    child: ListTile(
                      leading: Icon(Icons.fingerprint),
                      title: Text('Setup Biometric'),
                      contentPadding: EdgeInsets.zero,
                    ),
                  ),
                  const PopupMenuItem(
                    value: 'qr',
                    child: ListTile(
                      leading: Icon(Icons.qr_code),
                      title: Text('Generate QR'),
                      contentPadding: EdgeInsets.zero,
                    ),
                  ),
                  if (employee.status == EmployeeStatus.active)
                    const PopupMenuItem(
                      value: 'deactivate',
                      child: ListTile(
                        leading: Icon(Icons.person_off),
                        title: Text('Deactivate'),
                        contentPadding: EdgeInsets.zero,
                      ),
                    )
                  else
                    const PopupMenuItem(
                      value: 'activate',
                      child: ListTile(
                        leading: Icon(Icons.person),
                        title: Text('Activate'),
                        contentPadding: EdgeInsets.zero,
                      ),
                    ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildTimeTrackingTab() {
    final activeShifts = _shifts.where((shift) => shift.isActive).toList();
    final recentShifts = _shifts.where((shift) => !shift.isActive)
        .take(10).toList();

    return SingleChildScrollView(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          if (activeShifts.isNotEmpty) ...[
            Padding(
              padding: const EdgeInsets.all(16),
              child: Text(
                'Currently Working (${activeShifts.length})',
                style: Theme.of(context).textTheme.titleLarge?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            ...activeShifts.map((shift) => _buildShiftCard(shift, true)),
            const Divider(thickness: 2),
          ],
          Padding(
            padding: const EdgeInsets.all(16),
            child: Text(
              'Recent Shifts',
              style: Theme.of(context).textTheme.titleLarge?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          ...recentShifts.map((shift) => _buildShiftCard(shift, false)),
        ],
      ),
    );
  }

  Widget _buildShiftCard(WorkShift shift, bool isActive) {
    final employee = _employees.firstWhere((e) => e.id == shift.employeeId);
    final duration = isActive 
        ? DateTime.now().difference(shift.startTime)
        : shift.totalWorkTime;

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      color: isActive ? Colors.green.withOpacity(0.1) : null,
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                CircleAvatar(
                  backgroundColor: isActive ? Colors.green : Colors.grey,
                  child: Text(
                    employee.name[0].toUpperCase(),
                    style: const TextStyle(
                      color: Colors.white,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        employee.name,
                        style: Theme.of(context).textTheme.titleMedium?.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      Text(
                        '${employee.employeeNumber} • ${employee.roleDisplayName}',
                        style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                          color: Colors.grey[600],
                        ),
                      ),
                    ],
                  ),
                ),
                if (isActive)
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(
                      color: Colors.green,
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: const Text(
                      'ACTIVE',
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 10,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                Expanded(
                  child: _buildShiftInfo(
                    'Start Time',
                    '${shift.startTime.hour.toString().padLeft(2, '0')}:${shift.startTime.minute.toString().padLeft(2, '0')}',
                  ),
                ),
                Expanded(
                  child: _buildShiftInfo(
                    'Duration',
                    duration != null 
                        ? '${duration.inHours}h ${(duration.inMinutes % 60)}m'
                        : 'In progress',
                  ),
                ),
                if (shift.metrics['sales'] != null)
                  Expanded(
                    child: _buildShiftInfo(
                      'Sales',
                      '\$${shift.metrics['sales']?.toStringAsFixed(2) ?? '0.00'}',
                    ),
                  ),
              ],
            ),
            if (shift.metrics['transactions'] != null) ...[
              const SizedBox(height: 8),
              Row(
                children: [
                  Expanded(
                    child: _buildShiftInfo(
                      'Transactions',
                      '${shift.metrics['transactions'] ?? 0}',
                    ),
                  ),
                  if (shift.breakDuration != null)
                    Expanded(
                      child: _buildShiftInfo(
                        'Break Time',
                        '${shift.breakDuration}m',
                      ),
                    ),
                  if (shift.metrics['returns'] != null)
                    Expanded(
                      child: _buildShiftInfo(
                        'Returns',
                        '${shift.metrics['returns'] ?? 0}',
                      ),
                    ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildShiftInfo(String label, String value) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: Theme.of(context).textTheme.bodySmall?.copyWith(
            color: Colors.grey[600],
          ),
        ),
        Text(
          value,
          style: Theme.of(context).textTheme.titleSmall?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
      ],
    );
  }

  Widget _buildPerformanceTab() {
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text(
          'Performance Overview',
          style: Theme.of(context).textTheme.titleLarge?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 16),
        ..._employees.where((e) => e.performanceMetrics.isNotEmpty)
            .map((employee) => _buildPerformanceCard(employee)),
      ],
    );
  }

  Widget _buildPerformanceCard(EmployeeEntity employee) {
    return Card(
      margin: const EdgeInsets.only(bottom: 16),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                CircleAvatar(
                  child: Text(employee.name[0].toUpperCase()),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        employee.name,
                        style: Theme.of(context).textTheme.titleMedium?.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      Text(employee.roleDisplayName),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            if (employee.role == UserRole.cashier || 
                employee.role == UserRole.manager ||
                employee.role == UserRole.supervisor) ...[
              Row(
                children: [
                  Expanded(
                    child: _buildMetricTile(
                      'Total Sales',
                      '\$${employee.performanceMetrics['totalSales']?.toStringAsFixed(2) ?? '0.00'}',
                      Icons.attach_money,
                      Colors.green,
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: _buildMetricTile(
                      'Transactions',
                      '${employee.performanceMetrics['transactionsCount'] ?? 0}',
                      Icons.receipt,
                      Colors.blue,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              Row(
                children: [
                  Expanded(
                    child: _buildMetricTile(
                      'Avg Transaction',
                      '\$${employee.performanceMetrics['averageTransactionValue']?.toStringAsFixed(2) ?? '0.00'}',
                      Icons.trending_up,
                      Colors.orange,
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: _buildMetricTile(
                      'Rating',
                      '${employee.performanceMetrics['customerRating']?.toStringAsFixed(1) ?? '0.0'} ⭐',
                      Icons.star,
                      Colors.amber,
                    ),
                  ),
                ],
              ),
            ],
            if (employee.role == UserRole.stockClerk) ...[
              Row(
                children: [
                  Expanded(
                    child: _buildMetricTile(
                      'Stock Received',
                      '${employee.performanceMetrics['stockReceived'] ?? 0}',
                      Icons.inventory,
                      Colors.blue,
                    ),
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: _buildMetricTile(
                      'Adjustments',
                      '${employee.performanceMetrics['adjustmentsMade'] ?? 0}',
                      Icons.tune,
                      Colors.orange,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              _buildMetricTile(
                'Accuracy Rate',
                '${((employee.performanceMetrics['accuracyRate'] ?? 0.0) * 100).toStringAsFixed(1)}%',
                Icons.check_circle,
                Colors.green,
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildMetricTile(String title, String value, IconData icon, Color color) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(8),
        border: Border.all(color: color.withOpacity(0.3)),
      ),
      child: Row(
        children: [
          Icon(icon, color: color, size: 20),
          const SizedBox(width: 8),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: Colors.grey[600],
                  ),
                ),
                Text(
                  value,
                  style: Theme.of(context).textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.bold,
                    color: color,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  void _showEmployeeDetails(EmployeeEntity employee) {
    setState(() {
      _selectedEmployee = employee;
    });
    
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      builder: (context) => DraggableScrollableSheet(
        initialChildSize: 0.7,
        maxChildSize: 0.95,
        minChildSize: 0.5,
        builder: (context, scrollController) => _buildEmployeeDetailsSheet(
          employee,
          scrollController,
        ),
      ),
    );
  }

  Widget _buildEmployeeDetailsSheet(EmployeeEntity employee, ScrollController scrollController) {
    return Container(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          // Handle bar
          Container(
            width: 40,
            height: 4,
            decoration: BoxDecoration(
              color: Colors.grey[300],
              borderRadius: BorderRadius.circular(2),
            ),
          ),
          const SizedBox(height: 16),
          // Header
          Row(
            children: [
              CircleAvatar(
                radius: 30,
                backgroundColor: _getStatusColor(employee.status),
                child: Text(
                  employee.name[0].toUpperCase(),
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      employee.name,
                      style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    Text(
                      '${employee.employeeNumber} • ${employee.roleDisplayName}',
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(
                        color: Colors.grey[600],
                      ),
                    ),
                  ],
                ),
              ),
              IconButton(
                onPressed: () => Navigator.of(context).pop(),
                icon: const Icon(Icons.close),
              ),
            ],
          ),
          const SizedBox(height: 24),
          // Content
          Expanded(
            child: ListView(
              controller: scrollController,
              children: [
                _buildDetailSection('Contact Information', [
                  _buildDetailRow('Phone', employee.phone),
                  _buildDetailRow('Email', employee.email.isEmpty ? 'Not provided' : employee.email),
                ]),
                _buildDetailSection('Employment Details', [
                  _buildDetailRow('Status', _getStatusName(employee.status)),
                  _buildDetailRow('Role', employee.roleDisplayName),
                  _buildDetailRow('Hourly Rate', '\$${employee.hourlyRate.toStringAsFixed(2)}'),
                  _buildDetailRow('Hire Date', '${employee.hireDate.day}/${employee.hireDate.month}/${employee.hireDate.year}'),
                  if (employee.isTrainingMode)
                    _buildDetailRow('Training Mode', 'Active'),
                ]),
                _buildDetailSection('Security', [
                  _buildDetailRow('PIN Setup', employee.pin != null ? 'Configured' : 'Not set'),
                  _buildDetailRow('Biometric', employee.biometricType != BiometricType.none ? 'Enabled' : 'Disabled'),
                ]),
                _buildDetailSection('Permissions', [
                  _buildPermissionsList(employee.permissions),
                ]),
                if (employee.performanceMetrics.isNotEmpty)
                  _buildDetailSection('Performance Metrics', [
                    ...employee.performanceMetrics.entries.map((entry) =>
                      _buildDetailRow(
                        _formatMetricName(entry.key),
                        entry.value.toString(),
                      ),
                    ),
                  ]),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildDetailSection(String title, List<Widget> children) {
    return Container(
      margin: const EdgeInsets.only(bottom: 24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),
          Container(
            width: double.infinity,
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: Theme.of(context).colorScheme.surfaceVariant.withOpacity(0.3),
              borderRadius: BorderRadius.circular(12),
            ),
            child: Column(
              children: children,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildDetailRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 120,
            child: Text(
              label,
              style: TextStyle(
                color: Colors.grey[600],
                fontWeight: FontWeight.w500,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(fontWeight: FontWeight.w500),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildPermissionsList(PermissionSet permissions) {
    final permissionMap = permissions.toMap();
    final enabledPermissions = permissionMap.entries
        .where((entry) => entry.value == true)
        .map((entry) => _formatPermissionName(entry.key))
        .toList();

    if (enabledPermissions.isEmpty) {
      return const Text('No permissions granted');
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: enabledPermissions.map((permission) => 
        Padding(
          padding: const EdgeInsets.symmetric(vertical: 2),
          child: Row(
            children: [
              const Icon(Icons.check, size: 16, color: Colors.green),
              const SizedBox(width: 8),
              Expanded(child: Text(permission)),
            ],
          ),
        ),
      ).toList(),
    );
  }

  String _formatPermissionName(String key) {
    return key.replaceAll('can', '')
           .replaceAll(RegExp(r'([A-Z])'), ' \$1')
           .trim()
           .toLowerCase()
           .split(' ')
           .map((word) => word[0].toUpperCase() + word.substring(1))
           .join(' ');
  }

  String _formatMetricName(String key) {
    return key.replaceAll(RegExp(r'([A-Z])'), ' \$1')
           .toLowerCase()
           .split(' ')
           .map((word) => word[0].toUpperCase() + word.substring(1))
           .join(' ');
  }

  void _addNewEmployee() {
    // TODO: Navigate to add employee screen
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(content: Text('Add employee functionality coming soon')),
    );
  }

  void _handleEmployeeAction(String action, EmployeeEntity employee) {
    switch (action) {
      case 'edit':
        // TODO: Navigate to edit employee screen
        break;
      case 'permissions':
        _showPermissionsDialog(employee);
        break;
      case 'biometric':
        _setupBiometricForEmployee(employee);
        break;
      case 'qr':
        _showEmployeeQRCode(employee);
        break;
      case 'activate':
      case 'deactivate':
        _toggleEmployeeStatus(employee);
        break;
    }
  }

  void _showPermissionsDialog(EmployeeEntity employee) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text('Permissions - ${employee.name}'),
        content: SizedBox(
          width: double.maxFinite,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text('Current role: ${employee.roleDisplayName}'),
              const SizedBox(height: 16),
              const Text('Custom permissions can be configured here.'),
              // TODO: Add permission toggles
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Close'),
          ),
        ],
      ),
    );
  }

  void _setupBiometricForEmployee(EmployeeEntity employee) async {
    final success = await _authService.setupBiometricForEmployee(employee.id);
    
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(success 
            ? 'Biometric setup successful for ${employee.name}'
            : 'Failed to setup biometric authentication'),
        backgroundColor: success ? Colors.green : Colors.red,
      ),
    );
  }

  void _showEmployeeQRCode(EmployeeEntity employee) {
    final qrData = _authService.generateEmployeeQRCode(employee.id);
    
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text('QR Code - ${employee.name}'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // TODO: Add QR code widget
            Container(
              width: 200,
              height: 200,
              decoration: BoxDecoration(
                border: Border.all(color: Colors.grey),
              ),
              child: const Center(
                child: Text('QR Code\n(Widget not implemented)'),
              ),
            ),
            const SizedBox(height: 16),
            Text('QR Data: $qrData'),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('Close'),
          ),
        ],
      ),
    );
  }

  void _toggleEmployeeStatus(EmployeeEntity employee) {
    final newStatus = employee.status == EmployeeStatus.active 
        ? EmployeeStatus.inactive 
        : EmployeeStatus.active;
    
    // TODO: Update employee status in database
    setState(() {
      final index = _employees.indexWhere((e) => e.id == employee.id);
      if (index != -1) {
        _employees[index] = employee.copyWith(status: newStatus);
      }
    });

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('${employee.name} ${newStatus == EmployeeStatus.active ? 'activated' : 'deactivated'}'),
      ),
    );
  }

  void _showExportOptions() {
    showModalBottomSheet(
      context: context,
      builder: (context) => Container(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            ListTile(
              leading: const Icon(Icons.table_chart),
              title: const Text('Export Employee List'),
              onTap: () {
                Navigator.of(context).pop();
                // TODO: Export employee list
              },
            ),
            ListTile(
              leading: const Icon(Icons.access_time),
              title: const Text('Export Time Records'),
              onTap: () {
                Navigator.of(context).pop();
                // TODO: Export time records
              },
            ),
            ListTile(
              leading: const Icon(Icons.analytics),
              title: const Text('Export Performance Report'),
              onTap: () {
                Navigator.of(context).pop();
                // TODO: Export performance report
              },
            ),
          ],
        ),
      ),
    );
  }

  Color _getStatusColor(EmployeeStatus status) {
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
  }

  String _getStatusName(EmployeeStatus status) {
    switch (status) {
      case EmployeeStatus.active:
        return 'Active';
      case EmployeeStatus.inactive:
        return 'Inactive';
      case EmployeeStatus.terminated:
        return 'Terminated';
      case EmployeeStatus.onLeave:
        return 'On Leave';
    }
  }

  String _getRoleName(UserRole role) {
    switch (role) {
      case UserRole.owner:
        return 'Owner';
      case UserRole.manager:
        return 'Manager';
      case UserRole.supervisor:
        return 'Supervisor';
      case UserRole.cashier:
        return 'Cashier';
      case UserRole.stockClerk:
        return 'Stock Clerk';
      case UserRole.trainee:
        return 'Trainee';
    }
  }
}
