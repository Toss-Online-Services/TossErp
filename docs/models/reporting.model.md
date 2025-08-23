---
description: Data model for Reporting & Analytics
---

# Reporting Data Model
- Entities: Report, Dashboard, Widget, Dataset, Schedule, Export
- Relationships: One-to-many (Dashboard <-> Widget), One-to-many (Dataset <-> Report)
- Key Fields: filters, refresh_interval, owner, permissions, format
- Audit: Run history, data lineage, change logs
