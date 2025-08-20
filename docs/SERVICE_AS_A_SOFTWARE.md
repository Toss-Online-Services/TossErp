## Service-as-a-Software (SaaS 2.0) — TOSS ERP III

Purpose
- Explain the architecture, API contract, agent model, and essential implementation decisions to enable "Service as a Software" in TOSS ERP III.

Scope
- Conversational interface and AI Co-Pilots
- Orchestration and workflow definition
- Domain services (inventory, sales, procurement, marketing, reporting)
- Background jobs and event-driven automation
- Security, observability, and deployment notes

High-level architecture
- Conversational UI (WhatsApp, web chat, mobile, voice)
- AI Co-Pilot Agents (domain-specialized agents + orchestrator)
- Orchestration Layer (workflow engine / semantic kernel)
- Execution Layer (model selection, prompt pipelines, grounding)
- Domain Services (Inventory, Sales, Ordering, Marketing, Reporting)
- Background Jobs & Scheduler (nightly reconciliations, proactive checks)
- Data stores & Integrations (Postgres/SQL, RabbitMQ/EventBus, external APIs)

Platform fit / repo notes
- This repository is .NET-first (solution `TossErp.sln`, folders: `src/Services`, `EventBus`). Keep implementation in .NET 7+ microservices to match the existing codebase.
- Use existing EventBus pieces (RabbitMQ/MessageBus) rather than introducing new brokers unless needed.

Technology recommendations (practical choices)
- Language & runtime: .NET 7/8 microservices (C#) for domain services and orchestrator.
- Orchestration engine: Temporal (recommended) or Durable Functions / Camunda (alternative). Temporal has strong .NET SDKs and supports long-running workflows, retries, and visibility.
- Messaging: RabbitMQ + MassTransit or existing EventBus components in the repo.
- Background jobs: Temporal activities or Hangfire for simpler jobs; prefer Temporal if chosen for orchestration.
- Conversational UI: Integrations via connectors: Twilio (WhatsApp/SMS), WhatsApp Business API, and SignalR for web sockets.
- LLM / Model layer: Azure OpenAI or OpenAI for cloud models; support OpenRouter or Ollama for privacy/local deployment. Use a policy that combines cloud LLM + local SLM for latency and residency requirements.
- Semantic grounding: Microsoft Semantic Kernel (if using Azure) or custom grounding service that performs retrieval-augmented generation (RAG) using your DB, vector store (e.g., Milvus, Pinecone, or on-prem FAISS), and domain knowledge.
- Observability: Prometheus + Grafana (already present), OpenTelemetry for traces, and structured logs.
- Security: OAuth2 / OpenID Connect (IdentityServer or Azure AD), role-based access control, strong API auth for agent clients, audit trail for actions.

API & Orchestrator contract (minimal runnable surface)
These endpoints are a minimal contract between agents, UI, and the orchestration layer. Implement as OpenAPI-first.

- POST /orchestrator/execute
  - Purpose: Start an orchestrated service action (synchronous start; may return workflow id)
  - Request: { "workflow": "reorder_stock", "context": { ...business context... }, "userId": "owner-123", "priority": "normal" }
  - Response: { "workflowId": "wf_abc123", "status": "started" }

- GET /orchestrator/workflow/{id}
  - Purpose: Get status, last step, and audit trail
  - Response: { "workflowId": "...", "status": "running|completed|failed", "steps": [...], "result": {...} }

- POST /agents/intent
  - Purpose: Agent or UI sends parsed intent to agent manager; returns suggested actions or direct execution, depending on authorization
  - Request: { "intent": "reorder", "entity": "item-456", "qty": 20, "confirmRequired": true }
  - Response: { "suggestedActions": [...], "confidence": 0.92 }

- POST /agents/authorize-action
  - Purpose: Approve a pending action (used when workflows require human confirmation)
  - Request: { "workflowId":"...", "actionId":"...", "userId":"...", "decision":"approve|reject" }

Agent behavior contract (2–3 bullet contract)
- Inputs: conversation context, business object references (IDs), relevant recent events, user preferences/authorization scope.
- Outputs: suggested action(s) with exact API calls and confidence/uncertainty; if authorized, the agent can call the orchestrator to execute.
- Errors: agent must detect and escalate uncertainty or system errors (e.g., supplier API down) and create a human-review task.

Edge cases & safety
- Data freshness: always retrieve latest inventory from authoritative DB before placing orders.
- Large-value orders: must require explicit user confirmation or multi-signature approval.
- Supplier failures: orchestrator must implement retry policy + fallback suppliers + user notification.
- Partial failures: implement compensating transactions or human-notify-and-rollback steps.

Testing & governance
- Unit tests for agent prompt pipelines and domain services.
- Integration tests for workflows (Temporal test harness or durable function test harness).
- Simulation tests that run agents against historical data to validate decisions.
- Policy review for privacy and data residency; log all agent actions and provide human audit UI.

Deliverables in repo (suggested initial changes)
1) `src/Orchestrator` — new microservice scaffold (OpenAPI, Temporal client, workflow definitions)
2) `src/AgentManager` — agent orchestration / intent endpoint and model selector
3) `src/Connectors/WhatsAppConnector` — message ingress/egress
4) `docs/SERVICE_AS_A_SOFTWARE.md` — this document

Quick MVP feature suggestions (start here)
- Inventory autopilot: nightly reconcile + automatic reorder proposal
- Invoice automation: generate and send invoice on sale event
- Daily summary: proactive daily sales and KPI message to owner

Governance & pricing notes
- Track outcomes using immutable event logs; combine with metering service for outcome-based billing (per-order, per-invoice)
- Provide toggles per tenant: automation level (autonomy: off/notify/propose/auto) and approval thresholds.

References & sources
- globalorange.nl — Service-as-a-Software primer
- blog.goldfoot.com — product/design implications
- aicadata.com, ibbaka.com — pricing & BPO analysis
- lanternstudios.com — Copilot stack guidance

Status: Architectural doc added. Below are the concrete API contracts, data shapes, a minimal OpenAPI example, quick-start developer commands, and quality gates to make this doc directly actionable.

Contract: inputs / outputs / error modes (small "contract")
- Inputs: conversation + intent payload or an explicit orchestrator request. Must include: source system (UI/connector), userId, tenantId, timestamp, and business object references (IDs).
- Outputs: workflowId / action suggestions / execution result. Every action must include an audit record (who/what/when) and a confidence/uncertainty field.
- Error modes: validation error (400), auth error (401/403), transient dependency failure (retryable 5xx), permanent failure (5xx + requires human review). Agents must surface uncertainty and never silently perform high-risk operations.

Minimal data shapes (JSON examples)
- Orchestrator Execute request:

  {
    "workflow": "reorder_stock",
    "tenantId": "tenant-abc",
    "userId": "owner-123",
    "context": {
      "itemId": "item-456",
      "currentQty": 5,
      "threshold": 10,
      "preferredSupplierId": "supplier-99"
    },
    "priority": "normal"
  }

- Orchestrator Execute response:

  {
    "workflowId": "wf_abc123",
    "status": "started",
    "startedAt": "2025-08-20T10:15:30Z"
  }

- Agent Intent request:

  {
    "source": "whatsapp",
    "tenantId": "tenant-abc",
    "userId": "owner-123",
    "intent": "reorder",
    "entity": "item-456",
    "qty": 20,
    "confirmRequired": true
  }

- Agent Intent response (suggested action):

  {
    "suggestedActions": [
      {
        "actionId": "a-1",
        "type": "create_purchase_order",
        "payload": { "supplierId": "supplier-99", "items": [{ "id":"item-456", "qty":20 }] },
        "confidence": 0.92,
        "explanation": "Autosuggest based on sales trend and current stock"
      }
    ],
    "requiresApproval": true
  }

Minimal OpenAPI fragment (developer copy-paste)

```yaml
openapi: 3.0.3
info:
  title: TOSS Orchestrator API (minimal)
  version: 0.1.0
paths:
  /orchestrator/execute:
    post:
      summary: Start a workflow
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/OrchestratorExecuteRequest'
      responses:
        '200':
          description: started
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/OrchestratorExecuteResponse'
components:
  schemas:
    OrchestratorExecuteRequest:
      type: object
      required: [workflow, tenantId, userId, context]
      properties:
        workflow:
          type: string
        tenantId:
          type: string
        userId:
          type: string
        context:
          type: object
    OrchestratorExecuteResponse:
      type: object
      properties:
        workflowId:
          type: string
        status:
          type: string
        startedAt:
          type: string
          format: date-time
```

Developer quick-start (local)
1) Scaffold services (in repo root):

```powershell
cd src
dotnet new webapi -o Orchestrator --no-https --framework net7.0
dotnet new webapi -o AgentManager --no-https --framework net7.0
```

2) Add a minimal controller in `Orchestrator` for `/orchestrator/execute` that stores a workflow in-memory (for MVP).

3) Run both services locally (use separate ports); test with curl/PowerShell Invoke-RestMethod.

Quality gates (what to run before merging)
- Build: `dotnet build` for the solution (verify no errors).
- Lint / static analysis: enable Roslyn analyzers and fix high-severity issues.
- Unit tests: add tests covering agent parsing and orchestrator request validation.
- Integration smoke: start Orchestrator and AgentManager locally and run a scripted intent -> orchestrator flow.

Files suggested to add in the next commit
- `src/Orchestrator/Controllers/OrchestratorController.cs` (basic execute + status endpoints)
- `src/AgentManager/Controllers/AgentController.cs` (intent + authorize endpoints)
- `docs/openapi/orchestrator.yaml` (full OpenAPI spec)
- `tests/Orchestrator.IntegrationTests/` (integration harness using TestServer)

Done: this doc now contains concrete contracts and developer steps to get an MVP running. Next I can scaffold the WebAPI projects and add the basic controllers and tests — say `yes` and I'll create them in `src/` and `tests/`.
