import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:mobile_scanner/mobile_scanner.dart';

import '../../../core/services/employee_authentication_service.dart';
import '../../../domain/entities/employee_entity.dart';

class EmployeeLoginScreen extends StatefulWidget {
  final String? locationId;
  final VoidCallback? onLoginSuccess;

  const EmployeeLoginScreen({
    super.key,
    this.locationId,
    this.onLoginSuccess,
  });

  @override
  State<EmployeeLoginScreen> createState() => _EmployeeLoginScreenState();
}

class _EmployeeLoginScreenState extends State<EmployeeLoginScreen>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  final _authService = EmployeeAuthenticationService();
  
  // Controllers
  final _employeeNumberController = TextEditingController();
  final _pinController = TextEditingController();
  
  // State
  bool _isLoading = false;
  List<AuthenticationMethod> _availableMethods = [];

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 4, vsync: this);
    _loadAvailableMethods();
  }

  @override
  void dispose() {
    _tabController.dispose();
    _employeeNumberController.dispose();
    _pinController.dispose();
    super.dispose();
  }

  Future<void> _loadAvailableMethods() async {
    final methods = await _authService.getAvailableAuthMethods();
    setState(() {
      _availableMethods = methods;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        decoration: BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
            colors: [
              Theme.of(context).colorScheme.primary,
              Theme.of(context).colorScheme.primaryContainer,
            ],
          ),
        ),
        child: SafeArea(
          child: Column(
            children: [
              _buildHeader(),
              Expanded(
                child: Container(
                  margin: const EdgeInsets.all(16),
                  padding: const EdgeInsets.all(24),
                  decoration: BoxDecoration(
                    color: Theme.of(context).colorScheme.surface,
                    borderRadius: BorderRadius.circular(16),
                    boxShadow: [
                      BoxShadow(
                        color: Colors.black.withOpacity(0.1),
                        blurRadius: 10,
                        offset: const Offset(0, 5),
                      ),
                    ],
                  ),
                  child: _buildAuthenticationTabs(),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Container(
      padding: const EdgeInsets.all(24),
      child: Column(
        children: [
          Icon(
            Icons.store,
            size: 64,
            color: Colors.white,
          ),
          const SizedBox(height: 16),
          Text(
            'TOSS POS',
            style: Theme.of(context).textTheme.headlineMedium?.copyWith(
              color: Colors.white,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            'Employee Login',
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
              color: Colors.white.withOpacity(0.9),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildAuthenticationTabs() {
    return Column(
      children: [
        TabBar(
          controller: _tabController,
          isScrollable: true,
          tabs: [
            if (_availableMethods.contains(AuthenticationMethod.biometric))
              const Tab(icon: Icon(Icons.fingerprint), text: 'Biometric'),
            const Tab(icon: Icon(Icons.pin), text: 'PIN'),
            const Tab(icon: Icon(Icons.qr_code_scanner), text: 'QR Code'),
            const Tab(icon: Icon(Icons.badge), text: 'Employee #'),
          ],
        ),
        const SizedBox(height: 24),
        Expanded(
          child: TabBarView(
            controller: _tabController,
            children: [
              if (_availableMethods.contains(AuthenticationMethod.biometric))
                _buildBiometricTab(),
              _buildPinTab(),
              _buildQRTab(),
              _buildEmployeeNumberTab(),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildBiometricTab() {
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Icon(
          Icons.fingerprint,
          size: 100,
          color: Theme.of(context).colorScheme.primary.withOpacity(0.5),
        ),
        const SizedBox(height: 32),
        Text(
          'Use your fingerprint or face to login',
          style: Theme.of(context).textTheme.titleMedium,
          textAlign: TextAlign.center,
        ),
        const SizedBox(height: 32),
        SizedBox(
          width: double.infinity,
          child: ElevatedButton.icon(
            onPressed: _isLoading ? null : _authenticateWithBiometric,
            icon: _isLoading 
                ? const SizedBox(
                    width: 20,
                    height: 20,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.fingerprint),
            label: Text(_isLoading ? 'Authenticating...' : 'Authenticate'),
            style: ElevatedButton.styleFrom(
              padding: const EdgeInsets.all(16),
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildPinTab() {
    return Column(
      children: [
        TextField(
          controller: _employeeNumberController,
          decoration: const InputDecoration(
            labelText: 'Employee Number',
            prefixIcon: Icon(Icons.badge),
            border: OutlineInputBorder(),
          ),
          textInputAction: TextInputAction.next,
        ),
        const SizedBox(height: 16),
        TextField(
          controller: _pinController,
          obscureText: true,
          keyboardType: TextInputType.number,
          inputFormatters: [
            FilteringTextInputFormatter.digitsOnly,
            LengthLimitingTextInputFormatter(6),
          ],
          decoration: const InputDecoration(
            labelText: 'PIN',
            prefixIcon: Icon(Icons.lock),
            border: OutlineInputBorder(),
          ),
          textInputAction: TextInputAction.done,
          onSubmitted: (_) => _authenticateWithPin(),
        ),
        const SizedBox(height: 24),
        SizedBox(
          width: double.infinity,
          child: ElevatedButton.icon(
            onPressed: _isLoading ? null : _authenticateWithPin,
            icon: _isLoading 
                ? const SizedBox(
                    width: 20,
                    height: 20,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.login),
            label: Text(_isLoading ? 'Authenticating...' : 'Login'),
            style: ElevatedButton.styleFrom(
              padding: const EdgeInsets.all(16),
            ),
          ),
        ),
        const SizedBox(height: 16),
        _buildPinPad(),
      ],
    );
  }

  Widget _buildPinPad() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceVariant.withOpacity(0.5),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Column(
        children: [
          Text(
            'Quick PIN Entry',
            style: Theme.of(context).textTheme.titleSmall?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          GridView.builder(
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
              crossAxisCount: 3,
              childAspectRatio: 1.5,
              crossAxisSpacing: 8,
              mainAxisSpacing: 8,
            ),
            itemCount: 12,
            itemBuilder: (context, index) {
              if (index == 9) {
                return OutlinedButton(
                  onPressed: () {
                    HapticFeedback.lightImpact();
                    _pinController.clear();
                  },
                  child: const Icon(Icons.clear),
                );
              } else if (index == 10) {
                return OutlinedButton(
                  onPressed: () {
                    HapticFeedback.lightImpact();
                    _pinController.text += '0';
                  },
                  child: const Text('0'),
                );
              } else if (index == 11) {
                return OutlinedButton(
                  onPressed: () {
                    HapticFeedback.lightImpact();
                    if (_pinController.text.isNotEmpty) {
                      _pinController.text = _pinController.text.substring(0, _pinController.text.length - 1);
                    }
                  },
                  child: const Icon(Icons.backspace),
                );
              } else {
                final number = index + 1;
                return OutlinedButton(
                  onPressed: () {
                    HapticFeedback.lightImpact();
                    if (_pinController.text.length < 6) {
                      _pinController.text += number.toString();
                    }
                  },
                  child: Text(number.toString()),
                );
              }
            },
          ),
        ],
      ),
    );
  }

  Widget _buildQRTab() {
    return Column(
      children: [
        Text(
          'Scan your employee QR code',
          style: Theme.of(context).textTheme.titleMedium,
          textAlign: TextAlign.center,
        ),
        const SizedBox(height: 24),
        Expanded(
          child: Container(
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(12),
              border: Border.all(
                color: Theme.of(context).colorScheme.outline,
                width: 2,
              ),
            ),
            child: ClipRRect(
              borderRadius: BorderRadius.circular(10),
              child: MobileScanner(
                onDetect: (capture) {
                  final barcodes = capture.barcodes;
                  if (barcodes.isNotEmpty && !_isLoading) {
                    final qrData = barcodes.first.rawValue;
                    if (qrData != null) {
                      _authenticateWithQR(qrData);
                    }
                  }
                },
              ),
            ),
          ),
        ),
        const SizedBox(height: 16),
        Text(
          'Point your camera at the QR code',
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
            color: Colors.grey[600],
          ),
          textAlign: TextAlign.center,
        ),
      ],
    );
  }

  Widget _buildEmployeeNumberTab() {
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Icon(
          Icons.admin_panel_settings,
          size: 80,
          color: Colors.orange,
        ),
        const SizedBox(height: 24),
        Text(
          'Manager Override',
          style: Theme.of(context).textTheme.titleLarge?.copyWith(
            fontWeight: FontWeight.bold,
          ),
          textAlign: TextAlign.center,
        ),
        const SizedBox(height: 8),
        Text(
          'Login with employee number only\n(Manager/Owner access required)',
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
            color: Colors.grey[600],
          ),
          textAlign: TextAlign.center,
        ),
        const SizedBox(height: 32),
        TextField(
          decoration: const InputDecoration(
            labelText: 'Employee Number',
            prefixIcon: Icon(Icons.badge),
            border: OutlineInputBorder(),
          ),
          textInputAction: TextInputAction.done,
          onSubmitted: (value) => _authenticateWithEmployeeNumber(value),
        ),
        const SizedBox(height: 24),
        SizedBox(
          width: double.infinity,
          child: ElevatedButton.icon(
            onPressed: _isLoading ? null : () {
              // Get value from the text field above
              final employeeNumber = _employeeNumberController.text;
              _authenticateWithEmployeeNumber(employeeNumber);
            },
            icon: _isLoading 
                ? const SizedBox(
                    width: 20,
                    height: 20,
                    child: CircularProgressIndicator(strokeWidth: 2),
                  )
                : const Icon(Icons.login),
            label: Text(_isLoading ? 'Authenticating...' : 'Manager Login'),
            style: ElevatedButton.styleFrom(
              padding: const EdgeInsets.all(16),
              backgroundColor: Colors.orange,
            ),
          ),
        ),
      ],
    );
  }

  Future<void> _authenticateWithBiometric() async {
    setState(() {
      _isLoading = true;
    });

    final result = await _authService.authenticateWithBiometric();
    await _handleAuthResult(result);

    setState(() {
      _isLoading = false;
    });
  }

  Future<void> _authenticateWithPin() async {
    if (_employeeNumberController.text.isEmpty || _pinController.text.isEmpty) {
      _showError('Please enter both employee number and PIN');
      return;
    }

    setState(() {
      _isLoading = true;
    });

    final result = await _authService.authenticateWithPin(
      _employeeNumberController.text,
      _pinController.text,
    );
    await _handleAuthResult(result);

    setState(() {
      _isLoading = false;
    });
  }

  Future<void> _authenticateWithQR(String qrData) async {
    setState(() {
      _isLoading = true;
    });

    final result = await _authService.authenticateWithQRCode(qrData);
    await _handleAuthResult(result);

    setState(() {
      _isLoading = false;
    });
  }

  Future<void> _authenticateWithEmployeeNumber(String employeeNumber) async {
    if (employeeNumber.isEmpty) {
      _showError('Please enter employee number');
      return;
    }

    setState(() {
      _isLoading = true;
    });

    final result = await _authService.authenticateWithEmployeeNumber(employeeNumber);
    await _handleAuthResult(result);

    setState(() {
      _isLoading = false;
    });
  }

  Future<void> _handleAuthResult(AuthenticationResult result) async {
    if (result.success && result.employee != null) {
      // Start shift if location provided
      if (widget.locationId != null) {
        await _authService.startShift(widget.locationId!);
      }

      // Show success message
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Welcome, ${result.employee!.name}!'),
          backgroundColor: Colors.green,
        ),
      );

      // Navigate to main app
      widget.onLoginSuccess?.call();
      Navigator.of(context).pushReplacementNamed('/pos');
      
    } else {
      _showError(result.error ?? 'Authentication failed');
    }
  }

  void _showError(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: Colors.red,
      ),
    );

    // Clear sensitive fields
    _pinController.clear();
  }
}
