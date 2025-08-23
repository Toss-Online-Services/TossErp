---
description: Data model for Inventory & Procurement
---

# Inventory Data Model
- Entities: Item, Warehouse, StockBin, Batch, Serial, Requisition, PurchaseOrder, Receipt, Invoice
- Relationships: Many-to-many (Item <-> Warehouse via StockBin), One-to-many (PO <-> Receipt)
- Key Fields: qty, uom, lot_no, expiry, reorder_level, landed_cost
- Audit: Stock moves with source doc ref, user/time, reason codes
