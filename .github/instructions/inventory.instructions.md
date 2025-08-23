description: Rules and best practices for Inventory & Procurement Management module in TOSS ERP III
applyTo: "**/*"
globs: "**/*"
alwaysApply: true
---

# Inventory Module Instructions
- Track stock levels, locations, and movements in real time.
- Support multi-warehouse, batch/serial tracking, and expiry management.
- Automate procurement workflows: requisition, PO, receipt, invoice matching.
- Integrate with vendor management and group buying features.
- Provide APIs for barcode/RFID, logistics, and supply chain partners.
- Enforce audit trails for all inventory transactions.

## Security & Compliance
- OAuth2 scopes: inventory:read, inventory:write, procurement:write; enforce warehouse and company scoping.
- Immutable audit of stock moves; conflict handling on over-issuance with 409.

## Acceptance Criteria
- API: RFC9457 errors, cursor pagination, idempotency on stock-moves and receipts.
- Cross-links: [Model](mdc:docs/models/inventory.model.md) · [API](mdc:docs/api-specs/inventory.openapi.md) · [Workflow](mdc:docs/architecture/inventory.workflow.md)
- Validate batch/serial tracking, expiry, and 3-way match (PO→Receipt→Invoice).
