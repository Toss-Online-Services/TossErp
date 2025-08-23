---
description: Data model for Shared Asset & Tool Sharing Network
---

# Asset Sharing Data Model
- Entities: Asset, Owner, Booking, Reservation, Usage, Maintenance, CostShare, Notification
- Relationships: One-to-many (Owner <-> Asset), Many-to-many (Asset <-> Booking), One-to-many (Asset <-> Maintenance)
- Key Fields: asset_status, booking_time, cost_share, maintenance_due
- Audit: Usage logs, condition reports, ratings
