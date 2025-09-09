import 'package:flutter/material.dart';

import '../../../app/themes/app_sizes.dart';
import '../../../service_locator.dart';
import '../../providers/transactions/transactions_provider.dart';
import '../../widgets/app_empty_state.dart';
import '../../../app/utilities/currency_formatter.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import '../../widgets/app_loading_more_indicator.dart';
import '../../widgets/app_progress_indicator.dart';
import '../../widgets/app_text_field.dart';
import 'components/transaction_card.dart';

class TransactionsScreen extends StatefulWidget {
  const TransactionsScreen({super.key});

  @override
  State<TransactionsScreen> createState() => _TransactionsScreenState();
}

class _TransactionsScreenState extends State<TransactionsScreen> {
  final transactionProvider = sl<TransactionsProvider>();

  final scrollController = ScrollController();

  final searchFieldController = TextEditingController();

  @override
  void initState() {
    scrollController.addListener(scrollListener);
    WidgetsBinding.instance.addPostFrameCallback((_) {
      transactionProvider.getAllTransactions();
    });
    super.initState();
  }

  @override
  void dispose() {
    scrollController.removeListener(scrollListener);
    scrollController.dispose();
    super.dispose();
  }

  void scrollListener() async {
    // Automatically load more data on end of scroll position
    if (scrollController.offset == scrollController.position.maxScrollExtent) {
      await transactionProvider.getAllTransactions(offset: transactionProvider.allTransactions?.length);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Transactions'),
        elevation: 0,
        shadowColor: Colors.transparent,
        actions: [
          IconButton(
            tooltip: 'Cash-up',
            icon: const Icon(Icons.calculate_outlined),
            onPressed: () => _showCashUpDialog(context),
          ),
        ],
      ),
      body: Consumer<TransactionsProvider>(
        builder: (context, provider, _) {
          return RefreshIndicator(
            onRefresh: () => provider.getAllTransactions(),
            displacement: 60,
            child: Scrollbar(
              child: CustomScrollView(
                controller: scrollController,
                // Disable scroll when data is null or empty
                physics: (provider.allTransactions?.isEmpty ?? true) ? const NeverScrollableScrollPhysics() : null,
                slivers: [
                  SliverAppBar(
                    floating: true,
                    snap: true,
                    automaticallyImplyLeading: false,
                    collapsedHeight: 70,
                    titleSpacing: 0,
                    title: Padding(
                      padding: const EdgeInsets.symmetric(horizontal: AppSizes.padding),
                      child: searchField(),
                    ),
                  ),
                  SliverToBoxAdapter(
                    child: Padding(
                      padding: const EdgeInsets.symmetric(horizontal: AppSizes.padding, vertical: 8),
                      child: _DailyTotalsSummary(),
                    ),
                  ),
                  SliverLayoutBuilder(
                    builder: (context, constraint) {
                      if (provider.allTransactions == null) {
                        return const SliverFillRemaining(
                          hasScrollBody: false,
                          fillOverscroll: true,
                          child: AppProgressIndicator(),
                        );
                      }

                      if (provider.allTransactions!.isEmpty) {
                        return const SliverFillRemaining(
                          hasScrollBody: false,
                          fillOverscroll: true,
                          child: AppEmptyState(
                            subtitle: 'No transaction available',
                          ),
                        );
                      }

                      return SliverPadding(
                        padding: const EdgeInsets.fromLTRB(AppSizes.padding, 2, AppSizes.padding, AppSizes.padding),
                        sliver: SliverList.builder(
                          itemCount: provider.allTransactions!.length,
                          itemBuilder: (context, i) {
                            return TransactionCard(transaction: provider.allTransactions![i]);
                          },
                        ),
                      );
                    },
                  ),
                  SliverToBoxAdapter(
                    child: AppLoadingMoreIndicator(isLoading: provider.isLoadingMore),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }

  Widget searchField() {
    return AppTextField(
      controller: searchFieldController,
      hintText: 'Search Transaction ID...',
      type: AppTextFieldType.search,
      textInputAction: TextInputAction.search,
      onEditingComplete: () {
        FocusScope.of(context).unfocus();
        transactionProvider.allTransactions = null;
        transactionProvider.getAllTransactions(contains: searchFieldController.text);
      },
      onTapClearButton: () {
        transactionProvider.getAllTransactions(contains: searchFieldController.text);
      },
    );
  }
}

class _DailyTotalsSummary extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Consumer<TransactionsProvider>(
      builder: (context, provider, _) {
        final today = DateTime.now();
        final data = provider.allTransactions ?? [];
        int totalSales = 0;
        int cash = 0;
        int bank = 0;
        int txCount = 0;
        for (final t in data) {
          final createdAt = DateTime.tryParse(t.createdAt ?? '');
          if (createdAt != null && createdAt.year == today.year && createdAt.month == today.month && createdAt.day == today.day) {
            totalSales += t.totalAmount;
            txCount += 1;
            if (t.paymentMethod.toLowerCase() == 'cash') cash += t.totalAmount;
            if (t.paymentMethod.toLowerCase() == 'bank') bank += t.totalAmount;
            if (t.paymentMethod.toLowerCase() == 'split') {
              // When split, we cannot parse exact split here; count all to total only
            }
          }
        }
        final avg = txCount == 0 ? 0 : (totalSales / txCount).round();
        return Container(
          padding: const EdgeInsets.all(AppSizes.padding),
          decoration: BoxDecoration(
            color: Theme.of(context).colorScheme.surface,
            borderRadius: BorderRadius.circular(8),
          ),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text('Today ($txCount tx, avg ${CurrencyFormatter.format(avg)})', style: Theme.of(context).textTheme.labelLarge),
              Row(
                children: [
                  Text('Cash: '),
                  Text(CurrencyFormatter.format(cash), style: Theme.of(context).textTheme.labelLarge),
                  const SizedBox(width: 12),
                  Text('Bank: '),
                  Text(CurrencyFormatter.format(bank), style: Theme.of(context).textTheme.labelLarge),
                  const SizedBox(width: 12),
                  Text('Total: '),
                  Text(CurrencyFormatter.format(totalSales), style: Theme.of(context).textTheme.labelLarge),
                ],
              ),
            ],
          ),
        );
      },
    );
  }
}

void _showCashUpDialog(BuildContext context) {
  final provider = Provider.of<TransactionsProvider>(context, listen: false);
  final today = DateTime.now();
  final data = provider.allTransactions ?? [];
  int cash = 0;
  int bank = 0;
  int totalSales = 0;
  int txCount = 0;
  for (final t in data) {
    final createdAt = DateTime.tryParse(t.createdAt ?? '');
    if (createdAt != null && createdAt.year == today.year && createdAt.month == today.month && createdAt.day == today.day) {
      totalSales += t.totalAmount;
      txCount += 1;
      if (t.paymentMethod.toLowerCase() == 'cash') cash += t.totalAmount;
      if (t.paymentMethod.toLowerCase() == 'bank') bank += t.totalAmount;
    }
  }
  final avg = txCount == 0 ? 0 : (totalSales / txCount).round();
  final note = 'Cash-up ${today.toIso8601String()}\n'
      'Transactions: $txCount\n'
      'Average: ${CurrencyFormatter.format(avg)}\n'
      'Cash: ${CurrencyFormatter.format(cash)}\n'
      'Bank: ${CurrencyFormatter.format(bank)}\n'
      'Total: ${CurrencyFormatter.format(totalSales)}';

  showDialog(
    context: context,
    builder: (ctx) {
      return AlertDialog(
        title: const Text('Cash-up Summary'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(note),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(),
            child: const Text('Close'),
          ),
          TextButton(
            onPressed: () async {
              await Clipboard.setData(ClipboardData(text: note));
              Navigator.of(ctx).pop();
              ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Cash-up copied')));
            },
            child: const Text('Copy'),
          ),
        ],
      );
    },
  );
}
