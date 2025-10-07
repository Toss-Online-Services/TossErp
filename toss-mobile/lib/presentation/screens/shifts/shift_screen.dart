import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../app/themes/app_sizes.dart';
import '../../../service_locator.dart';
import '../../providers/shifts/shift_provider.dart';
import '../../../app/services/auth/auth_service.dart';

class ShiftScreen extends StatefulWidget {
  const ShiftScreen({super.key});

  @override
  State<ShiftScreen> createState() => _ShiftScreenState();
}

class _ShiftScreenState extends State<ShiftScreen> {
  final provider = sl<ShiftProvider>();
  final openingFloatController = TextEditingController();
  final countedCashController = TextEditingController();
  final movementAmountController = TextEditingController();
  String movementType = 'payout';

  @override
  void initState() {
    super.initState();
    final userId = AuthService().getAuthData()!.uid;
    provider.loadOpenShift(userId);
  }

  @override
  void dispose() {
    openingFloatController.dispose();
    countedCashController.dispose();
    movementAmountController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider.value(
      value: provider,
      child: Consumer<ShiftProvider>(
        builder: (context, p, _) {
          return Scaffold(
            appBar: AppBar(title: const Text('Shift Management')),
            body: Padding(
              padding: const EdgeInsets.all(AppSizes.padding),
              child: ListView(
                children: [
                  if (p.openShift == null) ...[
                    const Text('No open shift'),
                    const SizedBox(height: AppSizes.padding),
                    TextField(
                      controller: openingFloatController,
                      keyboardType: TextInputType.number,
                      decoration: const InputDecoration(labelText: 'Opening Float (cash)')
                    ),
                    const SizedBox(height: AppSizes.padding),
                    ElevatedButton(
                      onPressed: () async {
                        final userId = AuthService().getAuthData()!.uid;
                        final opening = int.tryParse(openingFloatController.text) ?? 0;
                        await p.startShift(userId: userId, openingFloat: opening);
                      },
                      child: const Text('Start Shift'),
                    ),
                  ] else ...[
                    Text('Shift started: ${p.openShift!.startedAt}'),
                    const SizedBox(height: AppSizes.padding),
                    Row(
                      children: [
                        Expanded(
                          child: DropdownButtonFormField<String>(
                            value: movementType,
                            items: const [
                              DropdownMenuItem(value: 'payout', child: Text('Payout')),
                              DropdownMenuItem(value: 'withdrawal', child: Text('Withdrawal')),
                              DropdownMenuItem(value: 'adjustment', child: Text('Adjustment')),
                            ],
                            onChanged: (val) => setState(() => movementType = val ?? 'payout'),
                            decoration: const InputDecoration(labelText: 'Movement Type'),
                          ),
                        ),
                        const SizedBox(width: AppSizes.padding/2),
                        Expanded(
                          child: TextField(
                            controller: movementAmountController,
                            keyboardType: TextInputType.number,
                            decoration: const InputDecoration(labelText: 'Amount'),
                          ),
                        ),
                        const SizedBox(width: AppSizes.padding/2),
                        ElevatedButton(
                          onPressed: () async {
                            final amt = int.tryParse(movementAmountController.text) ?? 0;
                            if (amt > 0) {
                              await p.addCashMovement(type: movementType, amount: amt);
                              movementAmountController.clear();
                            }
                          },
                          child: const Text('Add'),
                        ),
                      ],
                    ),
                    const SizedBox(height: AppSizes.padding),
                    const Text('Cash Movements'),
                    const SizedBox(height: 8),
                    ...p.movements.map((m) => ListTile(
                          title: Text('${m.type}  ${m.amount}'),
                          subtitle: Text(m.createdAt ?? ''),
                        )),
                    const Divider(height: 32),
                    Text('Expected cash: ${p.computeExpectedCash()}'),
                    const SizedBox(height: AppSizes.padding),
                    TextField(
                      controller: countedCashController,
                      keyboardType: TextInputType.number,
                      decoration: const InputDecoration(labelText: 'Counted Cash (closing)'),
                    ),
                    const SizedBox(height: AppSizes.padding),
                    ElevatedButton(
                      onPressed: () async {
                        final counted = int.tryParse(countedCashController.text) ?? 0;
                        final res = await p.endShift(countedCash: counted);
                        if (res.isSuccess) {
                          if (mounted) Navigator.of(context).pop();
                        }
                      },
                      child: const Text('End Shift & Generate Z-Report'),
                    ),
                  ]
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}


