# 📊 TOSS ERP III - Accounting Module User Guide

**Version**: 1.0  
**Last Updated**: October 13, 2025  
**For**: TOSS ERP III Users

---

## 📖 Table of Contents

1. [Getting Started](#getting-started)
2. [Dashboard Overview](#dashboard-overview)
3. [Chart of Accounts](#chart-of-accounts)
4. [Journal Entries](#journal-entries)
5. [Financial Reports](#financial-reports)
6. [Configuration](#configuration)
7. [Common Tasks](#common-tasks)
8. [Best Practices](#best-practices)
9. [Troubleshooting](#troubleshooting)
10. [FAQs](#faqs)

---

## 🚀 Getting Started

### Accessing Accounting

**Desktop**:
1. Click "📊 Accounting" in navigation menu
2. Or navigate to `/accounting`

**Mobile**:
1. Tap Accounting icon in bottom navigation
2. Or tap burger menu → Accounting

### First-Time Setup

**Before you start**:
1. ✅ Company created (Setup → Company)
2. ✅ Fiscal year configured (Setup → Fiscal Year)
3. ✅ Base currency set (Setup → Currency)
4. ✅ Chart of accounts initialized

**Quick Setup Wizard** (Coming Soon):
- Pre-configured accounts for SA businesses
- Township business templates
- SMME quick start

---

## 📊 Dashboard Overview

### Main Dashboard (`/accounting`)

**Top Section - Stats Cards**:
- **Total Assets**: All asset account balances
- **Total Liabilities**: All liability balances
- **Monthly Revenue**: Current month income
- **Net Profit**: Revenue - Expenses

**Core Accounting Section**:
- **Chart of Accounts**: Manage account structure
- **Journal Entries**: Record transactions
- **Company Setup**: Configure companies

**Financial Reports Section**:
Six report cards linking to:
1. Balance Sheet (Blue)
2. Profit & Loss (Green)
3. Cash Flow (Purple)
4. VAT Report (Yellow)
5. Trial Balance (Red)
6. General Ledger (Indigo)

**Configuration Section**:
Links to all setup pages (Fiscal Year, Periods, Currency, etc.)

**Content Panels**:
- Recent Accounts: Quick view of main accounts
- Recent Transactions: Latest journal entries
- P&L Summary: Current period profitability
- Balance Sheet Summary: Financial position

---

## 💼 Chart of Accounts

**Access**: Dashboard → Chart of Accounts

### Understanding Account Structure

**Account Hierarchy**:
```
1000 - Assets (Group)
  ├── 1100 - Current Assets (Group)
  │     ├── 1110 - Cash and Bank (Ledger)
  │     ├── 1120 - Accounts Receivable (Ledger)
  │     └── 1130 - Inventory (Ledger)
  └── 1200 - Fixed Assets (Group)
        ├── 1210 - Equipment (Ledger)
        └── 1220 - Vehicles (Ledger)
```

**Account Types**:
- **Asset**: Things you own (Cash, Inventory, Equipment)
- **Liability**: What you owe (Loans, Payables)
- **Equity**: Owner's investment
- **Revenue**: Income from sales/services
- **Expense**: Costs of doing business

### Creating a New Account

**Steps**:
1. Click "Add Account" button
2. Fill in account details:
   - **Account Code**: e.g., "1150"
   - **Account Name**: e.g., "Petty Cash"
   - **Account Type**: Select from dropdown
   - **Parent Account**: Select (optional)
   - **Opening Balance**: Enter if applicable
   - **Is Group**: Check if this is a parent account
   - **Active**: Check to enable
3. Click "Create Account"

**Tips**:
- Use consistent numbering (1000s = Assets, 2000s = Liabilities, etc.)
- Group accounts cannot have transactions directly
- Child accounts inherit type from parent
- Opening balance = balance at fiscal year start

### Editing an Account

**Steps**:
1. Find account in list
2. Click "Edit" button
3. Modify details
4. Click "Update Account"

**Note**: Cannot change account type if transactions exist

### Deleting an Account

**Steps**:
1. Click "Delete" button
2. Confirm deletion

**Restrictions**:
- Cannot delete account with child accounts
- Cannot delete account with transactions
- Delete child accounts first, or reassign

### Searching & Filtering

**Search**: Type account code or name
**Filter**: Select account type from dropdown
**Export**: Click "Export CSV" for full list

---

## 📝 Journal Entries

**Access**: Dashboard → Journal Entries

### What is a Journal Entry?

A journal entry records a financial transaction using double-entry bookkeeping:
- Every transaction has **equal debits and credits**
- Maintains the accounting equation: **Assets = Liabilities + Equity**

### Creating a Journal Entry

**Steps**:
1. Click "New Entry" button
2. Fill in entry header:
   - **Date**: Transaction date
   - **Reference**: Invoice #, PO #, etc.
   - **Description**: What this entry is for
3. Add line items:
   - Click "+ Add Line" for each account affected
   - Select **Account** from dropdown
   - Enter **Debit** OR **Credit** (not both)
   - Add line **Description** (optional)
4. Verify balance:
   - Total Debit must equal Total Credit
   - Green checkmark = balanced
   - Red warning = out of balance
5. Click "Create Entry (Draft)"

**Example - Cash Sale**:
```
Debit:  1110 - Cash and Bank       R 1,150.00
Credit: 4100 - Sales Revenue       R 1,000.00
Credit: 2130 - VAT Payable         R   150.00
        (R 1,000 × 15% = R 150 VAT)
---
Total Debit:  R 1,150.00
Total Credit: R 1,150.00 ✓ Balanced
```

### Journal Entry Workflow

**Statuses**:
- **Draft**: Being created/edited
- **Posted**: Finalized, affects ledger
- **Cancelled**: Voided entry

**Actions by Status**:

| Status | Available Actions |
|--------|-------------------|
| Draft | View, Edit, Post, Delete |
| Posted | View, Reverse |
| Cancelled | View only |

### Posting an Entry

**Steps**:
1. Find draft entry
2. Click "Post" button
3. Confirm posting

**Effect**:
- Updates general ledger
- Updates account balances
- Entry becomes permanent
- Cannot edit (can only reverse)

### Reversing an Entry

**Use Case**: Undo a posted entry

**Steps**:
1. Find posted entry
2. Click "Reverse" button
3. Enter reversal date
4. Confirm

**Effect**:
- Creates opposite entry
- Original entry remains (for audit)
- Balances adjusted
- New entry is posted immediately

### Viewing Entry Details

**Steps**:
1. Click "View" button
2. See full entry details:
   - Entry number
   - Date & reference
   - Status & posting info
   - All line items
   - Debit/credit totals

### Filtering & Searching

**Filters**:
- **Status**: Draft, Posted, Cancelled
- **Date**: Specific date
- **Search**: Entry number, reference, description

**Totals Summary**:
- Shows total debits/credits for filtered entries
- Shows difference (should be 0)

### Exporting

Click "Export CSV" to download all filtered entries

---

## 📈 Financial Reports

### 1. Balance Sheet

**Purpose**: Snapshot of financial position

**Access**: Dashboard → Balance Sheet

**Shows**:
- **Assets**: What you own
  - Current Assets (cash, receivables)
  - Fixed Assets (equipment, buildings)
- **Liabilities**: What you owe
  - Current Liabilities (payables, short-term)
  - Long-term Liabilities (loans, mortgages)
- **Equity**: Owner's stake

**Key Equation**: Assets = Liabilities + Equity

**How to Use**:
1. Select "As of Date"
2. Click "Refresh" or it auto-loads
3. Review sections
4. Export to CSV/PDF if needed

**Interpretation**:
- Higher assets = stronger position
- Assets should exceed liabilities
- Equity growth = business growth

---

### 2. Profit & Loss Statement

**Purpose**: How profitable are you?

**Access**: Dashboard → Profit & Loss

**Shows**:
- **Revenue**: All income
- **Expenses**: All costs
- **Gross Profit**: Revenue - COGS
- **Operating Profit**: Gross - Operating Expenses
- **Net Profit**: Final profit after all

**How to Use**:
1. Select date range (Start Date, End Date)
2. Click "Generate Report"
3. Review revenue and expenses
4. Check profitability

**Interpretation**:
- Positive net profit = profitable
- Compare month-to-month
- Analyze expense categories
- Track profit margin %

---

### 3. Cash Flow Statement

**Purpose**: Where does cash come from/go to?

**Access**: Dashboard → Cash Flow

**Shows**:
- **Operating Activities**: Daily business (customers, suppliers)
- **Investing Activities**: Asset purchases/sales
- **Financing Activities**: Loans, equity, dividends

**Key Insight**: You can be profitable but cash-poor!

**How to Use**:
1. Select date range
2. Click "Generate Report"
3. Review each activity section
4. Check net cash flow
5. Verify closing cash balance

**Interpretation**:
- Positive operating cash = healthy business
- Negative investing = growing (buying assets)
- Watch closing balance trend

---

### 4. Trial Balance

**Purpose**: Verify books are balanced

**Access**: Dashboard → Trial Balance

**Shows**:
- All accounts
- Debit balances
- Credit balances
- Balance status (✓ or ✗)

**What it Checks**:
```
Total Debits = Total Credits
```

**How to Use**:
1. Select date
2. Click "Refresh"
3. Check balance status
4. If unbalanced, investigate

**Troubleshooting**:
- ✓ Balanced = books are correct
- ✗ Out of balance = error exists
  - Review recent journal entries
  - Check for partial entries
  - Verify all posts completed

---

### 5. General Ledger

**Purpose**: Complete history of an account

**Access**: Dashboard → General Ledger

**Shows**:
- All transactions for selected account
- Date, reference, description
- Debit/credit amounts
- Running balance

**How to Use**:
1. Select account from dropdown
2. Select date range
3. Click "Generate Report"
4. Review all transactions
5. Export if needed

**Use Cases**:
- Investigate account activity
- Verify specific transactions
- Reconcile accounts
- Audit trail

---

### 6. VAT Report

**Purpose**: South African VAT compliance

**Access**: Dashboard → VAT Report

**Shows**:
- **Output VAT**: VAT collected from customers (15%)
- **Input VAT**: VAT paid to suppliers (15%)
- **Net VAT**: Output - Input
- **Amount Payable/Refundable**: To/from SARS

**How to Use**:
1. Select period (usually monthly)
2. Click "Generate Report"
3. Review calculations
4. Export for SARS submission

**VAT Calculation Example**:
```
Sale: R 1,000 + 15% VAT = R 1,150 (Output VAT: R 150)
Purchase: R 500 + 15% VAT = R 575 (Input VAT: R 75)
Net VAT Payable: R 150 - R 75 = R 75 to SARS
```

**SARS Submission**:
1. Export to CSV
2. Use data for eFiling
3. Pay net VAT to SARS
4. Keep copy for records

---

## ⚙️ Configuration

### Company Setup

**Purpose**: Define your business entities

**Fields**:
- Company Name
- Abbreviation
- Domain (Manufacturing, Services, Retail, etc.)
- Default Currency
- Country
- Tax ID
- Parent Company (for groups)

**Multi-Company**:
- Create parent company
- Create subsidiaries
- Link via Parent Company field
- Consolidated reporting (planned)

---

### Fiscal Year

**Purpose**: Define your financial year

**Setup**:
1. Navigate to Fiscal Year
2. Click "Create Fiscal Year"
3. Enter year, start date, end date
4. System creates 12 periods automatically

**Examples**:
- Calendar Year: Jan 1 - Dec 31
- Financial Year: Apr 1 - Mar 31

---

### Accounting Periods

**Purpose**: Monthly/quarterly periods for control

**Features**:
- Auto-created from fiscal year
- Open/Closed status
- Close period = lock transactions
- Reopen if needed (with permission)

**Best Practice**: Close periods monthly after review

---

### Currency Management

**Purpose**: Handle multiple currencies

**Features**:
- Add currencies (USD, EUR, GBP, etc.)
- Set exchange rates
- Update rates regularly
- Auto-conversion in reports

**Note**: Base currency = ZAR for SA businesses

---

### Payment Terms

**Purpose**: Define credit conditions

**Examples**:
- **Net 30**: Pay in 30 days
- **2/10 Net 30**: 2% discount if paid in 10 days, else net 30
- **Immediate**: Pay on receipt
- **Net 60**: Pay in 60 days

**Usage**: Assign to customers/suppliers

---

### Payment Methods

**Purpose**: Track how payments are made

**Methods Included**:
- Cash
- Credit Card
- Bank Transfer
- Mobile Money (e.g., M-Pesa)
- Online Payment

**Features**:
- Processing fee %
- Transaction count
- Total volume
- Active/inactive

---

## 🔧 Common Tasks

### Task 1: Record a Cash Sale

**Scenario**: Customer pays R 1,150 cash for goods worth R 1,000 + VAT

**Steps**:
1. Go to Journal Entries
2. Click "New Entry"
3. Fill in:
   - Date: Today
   - Reference: "SALE-001"
   - Description: "Cash sale to customer"
4. Add line items:
   ```
   Debit:  1110 - Cash and Bank       R 1,150.00
   Credit: 4100 - Sales Revenue       R 1,000.00
   Credit: 2130 - VAT Payable         R   150.00
   ```
5. Verify balanced (✓)
6. Click "Create Entry (Draft)"
7. Review and click "Post"

**Result**: 
- Cash increased by R 1,150
- Revenue increased by R 1,000
- VAT payable increased by R 150

---

### Task 2: Pay a Supplier

**Scenario**: Pay Shoprite R 2,000 from bank account

**Steps**:
1. New Journal Entry
2. Fill in:
   - Date: Today
   - Reference: "PAY-001"
   - Description: "Payment to Shoprite"
3. Add line items:
   ```
   Debit:  2110 - Accounts Payable    R 2,000.00
   Credit: 1110 - Cash and Bank       R 2,000.00
   ```
4. Post entry

**Result**:
- Cash decreased by R 2,000
- Payable decreased by R 2,000

---

### Task 3: Record Monthly Salary

**Scenario**: Pay staff salaries of R 15,000

**Steps**:
1. New Journal Entry
2. Fill in:
   - Date: End of month
   - Reference: "SAL-001"
   - Description: "Monthly salaries"
3. Add line items:
   ```
   Debit:  5200 - Salaries Expense    R 15,000.00
   Credit: 1110 - Cash and Bank       R 15,000.00
   ```
4. Post entry

**Result**:
- Expense increased by R 15,000
- Cash decreased by R 15,000
- Reduces profit by R 15,000

---

### Task 4: Month-End Closing

**Steps**:
1. Review Trial Balance
   - Go to Reports → Trial Balance
   - Verify balanced (✓)
2. Post all draft entries
   - Go to Journal Entries
   - Filter by "Draft"
   - Post each one
3. Generate financial statements
   - Balance Sheet
   - Profit & Loss
   - Cash Flow
4. Export reports for records
5. Close the period
   - Go to Accounting Periods
   - Find current period
   - Click "Close Period"

**Monthly Checklist**:
- [ ] All transactions recorded
- [ ] All entries posted
- [ ] Trial balance balanced
- [ ] Reports generated
- [ ] Reports reviewed
- [ ] Period closed

---

### Task 5: Prepare VAT Return

**Steps** (Monthly/Bi-monthly):
1. Go to VAT Report
2. Select period dates
3. Click "Generate Report"
4. Review calculations:
   - Output VAT (sales)
   - Input VAT (purchases)
   - Net VAT
5. Export to CSV
6. Submit to SARS eFiling
7. Record VAT payment in journal entry

**VAT Payment Entry**:
```
Debit:  2130 - VAT Payable         R [Net VAT]
Credit: 1110 - Cash and Bank       R [Net VAT]
```

---

### Task 6: Add a New Account Mid-Year

**Scenario**: Need to track a new expense type

**Steps**:
1. Go to Chart of Accounts
2. Click "Add Account"
3. Fill in:
   - Code: "5150"
   - Name: "Marketing Expense"
   - Type: Expense
   - Parent: "5000 - Expenses"
   - Opening Balance: 0 (mid-year)
   - Is Group: No
   - Active: Yes
4. Click "Create Account"
5. Account is now available in journal entries

---

## 💡 Best Practices

### 1. Account Numbering

**Standard Ranges**:
- **1000-1999**: Assets
  - 1000-1199: Current Assets
  - 1200-1999: Fixed Assets
- **2000-2999**: Liabilities
  - 2000-2199: Current Liabilities
  - 2200-2999: Long-term Liabilities
- **3000-3999**: Equity
- **4000-4999**: Revenue/Income
- **5000-5999**: Expenses

**Benefits**:
- Easy to find accounts
- Clear categorization
- Professional structure
- Matches SA standards

### 2. Journal Entry Best Practices

**Always**:
- ✓ Use clear descriptions
- ✓ Add references (invoice numbers)
- ✓ Verify balance before posting
- ✓ Review before posting
- ✓ Keep backups of imports

**Never**:
- ✗ Post unbalanced entries
- ✗ Delete posted entries (reverse instead)
- ✗ Skip descriptions
- ✗ Post to closed periods

### 3. Monthly Routine

**Week 1**:
- Record all sales/income
- Record all purchases/expenses
- Post entries daily

**Week 2-3**:
- Continue daily entries
- Review accounts
- Check for missing transactions

**Week 4**:
- Post all remaining entries
- Run trial balance
- Generate reports
- Review for accuracy

**Week 5** (Month-end):
- Accruals and adjustments
- Depreciation entry
- Close period
- Archive reports

### 4. Backup & Security

**Regular Backups**:
- Export chart of accounts monthly
- Export journal entries monthly
- Export all reports quarterly
- Keep offline copies

**Data Security**:
- Limit who can post entries
- Limit who can close periods
- Audit log reviews
- Regular reconciliation

---

## 🔍 Troubleshooting

### Issue: Trial Balance Not Balanced

**Symptoms**: Debits ≠ Credits

**Causes & Fixes**:
1. **Unposted entries**
   - Filter journal entries by "Draft"
   - Post or delete drafts
2. **Partial entry**
   - Review recent entries
   - Check all have full line items
3. **Data error**
   - Run account balance check
   - Compare to general ledger

---

### Issue: Cannot Delete Account

**Error**: "Cannot delete an account that has child accounts"

**Fix**:
1. Go to Chart of Accounts
2. Find child accounts
3. Delete children first
4. Or reassign children to different parent
5. Then delete parent

---

### Issue: Cannot Edit Posted Entry

**Error**: "Only draft entries can be edited"

**Explanation**: Posted entries are locked for audit trail

**Fix**:
1. **If error in entry**: Reverse it, create new correct entry
2. **If need to modify**: Reverse old, post new
3. **Never**: Manually edit database (breaks audit)

---

### Issue: VAT Calculation Seems Wrong

**Check**:
1. Verify VAT rate is 15%
2. Check transaction categorization (Standard/Zero/Exempt)
3. Ensure amounts exclude VAT (VAT is additional)
4. Review VAT payable account

**Common Mistake**:
- Recording R 1,150 as revenue (should be R 1,000 + R 150 VAT)

**Correct Entry**:
```
Debit:  Cash           R 1,150
Credit: Revenue        R 1,000
Credit: VAT Payable    R   150
```

---

### Issue: Reports Show Old Data

**Fix**:
1. Click "Refresh" or "Generate Report"
2. Check date range
3. Ensure all entries are posted
4. Clear browser cache if needed

---

## ❓ FAQs

### Q: Can I change my fiscal year after starting?

**A**: Not recommended. Fiscal year should be set before transactions. If you must, consult support.

---

### Q: What if I make a mistake in a posted entry?

**A**: Use the "Reverse" function to create an offsetting entry, then create the correct entry. This maintains audit trail.

---

### Q: How do I handle VAT on exports?

**A**: Exports are zero-rated. Record revenue with 0% VAT.
```
Debit:  Cash/Bank           R 1,000
Credit: Export Revenue      R 1,000
(No VAT on exports)
```

---

### Q: Can I have multiple companies in one system?

**A**: Yes! Create companies under Company Setup. Each can have separate books.

---

### Q: How often should I close periods?

**A**: Monthly is recommended. Allows monthly reporting while keeping data secure.

---

### Q: What's the difference between Draft and Posted?

**A**:
- **Draft**: Work in progress, can edit/delete
- **Posted**: Finalized, affects ledger, cannot edit (only reverse)

---

### Q: How do I record depreciation?

**A**: Monthly depreciation entry:
```
Debit:  Depreciation Expense           R 1,000
Credit: Accumulated Depreciation       R 1,000
```

---

### Q: Can I import opening balances?

**A**: Currently manual entry. Import feature is planned for next release.

**Workaround**:
1. Create accounts
2. Create journal entry dated first day of fiscal year
3. Enter all opening balances
4. Post entry

---

### Q: How do I export for my accountant?

**A**:
1. Generate Trial Balance → Export CSV
2. Generate Balance Sheet → Export PDF
3. Generate P&L → Export PDF
4. Export Chart of Accounts → Export CSV
5. Export Journal Entries → Export CSV

Send all 5 files to accountant.

---

## 📚 Additional Resources

### Related Modules

- **Sales**: Sales invoices auto-create journal entries
- **Purchasing**: Purchase invoices auto-create entries
- **Stock**: Inventory valuation affects balance sheet
- **HR**: Payroll creates salary expense entries

### Support

- **In-App Help**: Click (?) icons throughout
- **Documentation**: This guide
- **Support Email**: support@toss-erp.com (fictional)
- **Community**: forums.toss-erp.com (fictional)

### Training

- **Video Tutorials**: Coming soon
- **Webinars**: Monthly accounting webinars
- **Certification**: TOSS ERP Accounting Certification

---

## 🎓 Accounting Basics (for Non-Accountants)

### The Accounting Equation

```
Assets = Liabilities + Equity
```

**In English**:
"What you own = What you owe + What's truly yours"

### Debit vs. Credit (Simplified)

**Assets & Expenses** (Debit to increase):
- Debit = Add money/value
- Credit = Remove money/value

**Liabilities, Equity & Revenue** (Credit to increase):
- Debit = Remove money/value
- Credit = Add money/value

**Easy Rule**:
- Cash coming in = Debit Cash
- Cash going out = Credit Cash

### Double-Entry Bookkeeping

**Every transaction affects at least 2 accounts**

**Example**: Buy inventory for R 500 cash
```
Debit:  Inventory (Asset ↑)    R 500
Credit: Cash (Asset ↓)          R 500
```

**Why**: Keeps equation balanced!

---

## ✅ Quick Reference

### Keyboard Shortcuts (Coming Soon)

- `Ctrl + N`: New journal entry
- `Ctrl + E`: Export
- `Ctrl + F`: Focus search
- `/`: Open command palette

### Status Colors

- 🟢 **Green**: Positive (profit, balanced, active)
- 🔴 **Red**: Negative (loss, unbalanced, inactive)
- 🟡 **Yellow**: Warning (draft, pending)
- 🔵 **Blue**: Info (totals, summaries)

### Account Type Icons

- 📈 **Asset**: Trending up
- 📉 **Liability**: Trending down
- 🏦 **Equity**: Building
- 💰 **Revenue**: Dollar sign
- 💸 **Expense**: Money with wings

---

## 🎉 Congratulations!

You now know how to use the TOSS ERP III Accounting Module!

**Remember**:
- Start with chart of accounts setup
- Record transactions daily
- Review monthly
- Close periods regularly
- Keep good records

**Need Help?**: Check FAQs or contact support

---

**End of User Guide** 📚

