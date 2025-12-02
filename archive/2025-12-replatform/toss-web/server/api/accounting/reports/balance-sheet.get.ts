import type { BalanceSheet } from '~/types/accounting'

export default defineEventHandler(async (event): Promise<BalanceSheet> => {
  try {
    const query = getQuery(event)
    const date = query.date ? new Date(query.date as string) : new Date()

    // In a real application, this would fetch from database
    // For now, return mock data
    const balanceSheet: BalanceSheet = {
      date,
      currency: 'ZAR',
      assets: {
        currentAssets: [
          {
            accountId: '1',
            accountCode: '1100',
            accountName: 'Cash and Cash Equivalents',
            balance: 250000,
          },
          {
            accountId: '2',
            accountCode: '1200',
            accountName: 'Accounts Receivable',
            balance: 180000,
          },
          {
            accountId: '3',
            accountCode: '1300',
            accountName: 'Inventory',
            balance: 320000,
          },
          {
            accountId: '4',
            accountCode: '1400',
            accountName: 'Prepaid Expenses',
            balance: 25000,
          },
        ],
        fixedAssets: [
          {
            accountId: '5',
            accountCode: '1500',
            accountName: 'Property, Plant & Equipment',
            balance: 850000,
          },
          {
            accountId: '6',
            accountCode: '1600',
            accountName: 'Accumulated Depreciation',
            balance: -150000,
          },
          {
            accountId: '7',
            accountCode: '1700',
            accountName: 'Intangible Assets',
            balance: 120000,
          },
        ],
        totalAssets: 1595000,
      },
      liabilities: {
        currentLiabilities: [
          {
            accountId: '8',
            accountCode: '2100',
            accountName: 'Accounts Payable',
            balance: 145000,
          },
          {
            accountId: '9',
            accountCode: '2200',
            accountName: 'Short-term Loans',
            balance: 80000,
          },
          {
            accountId: '10',
            accountCode: '2300',
            accountName: 'Accrued Expenses',
            balance: 35000,
          },
        ],
        longTermLiabilities: [
          {
            accountId: '11',
            accountCode: '2400',
            accountName: 'Long-term Debt',
            balance: 400000,
          },
          {
            accountId: '12',
            accountCode: '2500',
            accountName: 'Deferred Tax Liabilities',
            balance: 55000,
          },
        ],
        totalLiabilities: 715000,
      },
      equity: {
        accounts: [
          {
            accountId: '13',
            accountCode: '3100',
            accountName: 'Share Capital',
            balance: 500000,
          },
          {
            accountId: '14',
            accountCode: '3200',
            accountName: 'Retained Earnings',
            balance: 380000,
          },
        ],
        totalEquity: 880000,
      },
      totalLiabilitiesAndEquity: 1595000,
    }

    return balanceSheet
  } catch (error: any) {
    console.error('Error generating balance sheet:', error)
    throw createError({
      statusCode: 500,
      statusMessage: 'Internal Server Error',
      message: error.message || 'Failed to generate balance sheet',
    })
  }
})

