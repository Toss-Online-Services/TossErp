---
description: Data model for Sales & CRM
---

# Sales Data Model
- Entities: Lead, Opportunity, Quote, SalesOrder, Customer, Contact, Activity
- Relationships: One-to-many (Customer <-> SalesOrder), One-to-many (Lead <-> Opportunity)
- Key Fields: stage, probability, total, discount, currency, status
- Audit: Stage changes, approval logs, channel source attribution
