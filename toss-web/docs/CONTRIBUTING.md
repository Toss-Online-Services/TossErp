## Contributing to TOSS ERP III

Thank you for contributing! This guide helps you set up the project, run it locally, and follow standards.

### Prerequisites
- Node.js 22.x
- pnpm (recommended)
- .NET 9 SDK (for backend services)
- Docker Desktop (optional: for DB/services)

### Project Setup (Frontend)
1. Copy environment file:
	- Duplicate `.env.example` to `.env` inside `toss-web/` and adjust values.
2. Install deps:
	- From `toss-web/`, run `pnpm install`.
3. Dev server:
	- `pnpm dev` (Nuxt on http://localhost:3001)
4. Build/preview:
	- `pnpm build`; `pnpm preview`.

### Testing
- Unit tests: `pnpm test` or `pnpm test:ui`
- E2E tests: `pnpm test:e2e` (ensure server is running)

### Formatting (Prettier)
- Format all files: `pnpm format`
- Check formatting only: `pnpm format:check`

### Pre-commit Hooks (Husky + lint-staged)
On commit, staged files are auto-formatted and linted:
1. Husky hook runs `pnpm lint-staged` from repo root.
2. JavaScript/TypeScript/Vue files: Prettier write + ESLint fix.
3. JSON/CSS/Markdown/YAML: Prettier write.
If any step fails, the commit aborts; fix issues and re-stage.

To skip hooks temporarily (avoid unless urgent): `git commit -m "msg" --no-verify`.

### Linting & Types
- Type-check: `pnpm typecheck`
- Lint: currently disabled in scripts; enable ESLint/Prettier locally via VS Code.

### AI & MCP (VS Code)
We ship a `.vscode/mcp.json` configured for the shadcn MCP server:
- Start the server from VS Code MCP view; requires `npx`.
- Optional envs: `REGISTRY_TOKEN`, `SHADCN_API_KEY` (see repo `.env.example`).

Recommended extensions are in `.vscode/extensions.json` (Copilot, Vue, ESLint, Prettier, C#, PGSQL).

### Observability (Sentry)
- Set `NUXT_PUBLIC_SENTRY_DSN` in `toss-web/.env` to enable Sentry on the client.
- For server capture in production, build the app and run with the `--import` flag as per Sentry docs.
 - Source maps: provide `SENTRY_ORG`, `SENTRY_PROJECT`, and `SENTRY_AUTH_TOKEN` (CI secret) so the Sentry Nuxt module can upload source maps during `nuxt build`.
 - Hidden client sourcemaps enabled via `sourcemap: { client: 'hidden' }` in `nuxt.config.ts`.
 - Test page: visit `/sentry-example` and click the buttons to generate a client error and a server API error (`/api/sentry-example`).
 - Minimal required env vars in CI for source map upload:
	 - `SENTRY_ORG`, `SENTRY_PROJECT`, `SENTRY_AUTH_TOKEN`
 - Optional: define a release with `SENTRY_RELEASE` and add to `sentry` config when release pipeline is formalized.

### Backend
- See root `.env.example` for DB/AI keys.
- Use .NET 9 SDK and EF Core; run API locally (docs WIP).

### Commit Conventions
- Use conventional commits where possible: `feat:`, `fix:`, `docs:`, `chore:`, `test:`

### Code Style
- Follow existing patterns and module structure.
- Prefer composables and Pinia stores for shared logic.

### Pull Requests
- Include: description, screenshots (if UI), and test coverage for new logic.
- Ensure tests pass locally.

