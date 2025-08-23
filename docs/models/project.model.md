---
description: Data model for Project & Service Management
---

# Project Data Model
- Entities: Project, Task, Milestone, Resource, Timesheet, Expense, ServiceOrder
- Relationships: One-to-many (Project <-> Task), One-to-many (Task <-> Timesheet)
- Key Fields: start/end_date, progress, assigned_to, billable, cost_code
- Audit: Status transitions, approvals, SLA timestamps
