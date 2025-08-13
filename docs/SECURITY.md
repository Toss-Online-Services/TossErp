# Security Notes (POPIA-friendly defaults)

- Env-driven secrets only, never commit real secrets.
- JWT validation at gateway (authority/audience via env).
- Tenant scoping to be enforced via claims and per-service filters.
- Do not log PII; prefer correlation IDs.
- Use HTTPS/TLS in production; dev may use HTTP locally.



