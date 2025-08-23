---
description: Data model for HR & Payroll
---

# HR & Payroll Data Model
- Entities: Employee, Contract, Leave, Attendance, PayrollRun, Payslip, Benefit
- Relationships: One-to-many (Employee <-> Payslip), One-to-many (PayrollRun <-> Payslip)
- Key Fields: salary, deductions, net_pay, leave_balance, grade, department
- Audit: PII access logs, payroll approvals, corrections history
