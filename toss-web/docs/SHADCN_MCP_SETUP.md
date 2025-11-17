## shadcn MCP + Creative Tim Registry Setup

### Why we added this

- The shadcn MCP server lets Cursor (and other MCP-aware clients) browse and install components via natural language prompts.  
- Creative Tim publishes a shadcn-compatible registry, so we can fetch their blocks the same way we fetch stock shadcn/ui components.

### 1. Configure MCP in Cursor

1. Ensure `.cursor/mcp.json` includes the server entry:

   ```json
   "shadcn": {
     "command": "npx",
     "args": ["shadcn@latest", "mcp"],
     "env": {}
   }
   ```

2. Restart Cursor and enable the `shadcn` MCP server (Settings → MCP Servers). You should see a green dot once the server is running.  
3. Use `/mcp` → `shadcn` → `list tools` to verify Cursor can talk to the server.

### 2. Point shadcn to the Creative Tim registry

Add the registry block to `components.json` (already committed):

```json
"registries": {
  "@creative-tim": "https://www.creative-tim.com/ui/{name}.json"
}
```

This keeps the default shadcn/ui catalog **and** exposes a namespaced registry called `@creative-tim`.

### 3. Test the workflow

1. In Cursor, run `/mcp run shadcn browse --registry @creative-tim` (or use the MCP tool picker) to list available Creative Tim items.  
2. Install a component by prompting Cursor, for example:  
   > “Add the `@creative-tim/order-summary` component to my project.”  
3. Confirm that the CLI drops the component into `components/ui` with the usual shadcn project structure.

### 4. Optional CLI bootstrap

If you also use other MCP clients, you can initialize their presets with:

```bash
pnpm dlx shadcn@latest mcp init --client <claude|vs-code|codex>
```

This command just scaffolds the per-client config; Cursor already uses `.cursor/mcp.json`.

### 5. Troubleshooting checklist

- **Server not visible** → Re-run Cursor after saving `.cursor/mcp.json`; check terminal for MCP errors.  
- **Creative Tim items missing** → Verify the registry stanza is present and spelled exactly `@creative-tim`.  
- **Private registries** → If Creative Tim ever locks endpoints, place the required token in `.env.local` and reference it inside `components.json` as documented by shadcn.

Keep this doc in sync if we add more registries or change MCP clients.

