---
description: Data model for Finance (GL, AP, AR, Tax)
---

# Finance Data Model
- Entities: Account, JournalEntry, Ledger, TaxCode, Invoice, Payment, Vendor, Customer
- Relationships: One-to-many (Account <-> JournalEntry), One-to-many (Invoice <-> Payment)
- Key Fields: debit, credit, balance, currency, exchange_rate, tax_amount, posting_date
- Audit: Immutable journal entries with user/time, source module reference
