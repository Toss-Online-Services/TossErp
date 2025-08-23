---
description: Data model for Group Buying & Collective Procurement
---

# Group Buying Data Model
- Entities: GroupBuy, Member, Vendor, Product, Commitment, RFQ, Contract, Allocation, Settlement
- Relationships: Many-to-many (GroupBuy <-> Member), One-to-many (GroupBuy <-> Product), One-to-many (GroupBuy <-> Vendor)
- Key Fields: status, total_commitment, savings, allocation, settlement_date
- Audit: All changes tracked with timestamps and user IDs
