---
description: Data model for Logistics & Supply Chain Management
---

# Logistics Data Model
- Entities: Shipment, Carrier, Route, Delivery, Warehouse, Inventory, Confirmation, Analytics
- Relationships: One-to-many (Carrier <-> Shipment), Many-to-many (Warehouse <-> Inventory), One-to-many (Shipment <-> Delivery)
- Key Fields: tracking_id, route_plan, delivery_status, warehouse_location
- Audit: Delivery logs, partner EDI, inventory ownership
