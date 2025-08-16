# Cursor Rules - TossErp Project

This directory contains comprehensive rules for the TossErp project, ensuring consistent code quality and best practices across all technologies used.

## Updated Rules Structure

### 🚀 Nuxt 4 Integration
- **`nuxt.mdc`** - Comprehensive Nuxt 4 best practices and patterns
- **`vue.mdc`** - Updated Vue.js 3 rules compatible with Nuxt 4
- Both rules are set to `alwaysApply: true` for consistent enforcement

### 🎯 Beast Mode 4.1
- **`beast-mode.mdc`** - Autonomous problem-solving and thorough implementation
- **`autonomous-workflow.mdc`** - Comprehensive workflow integrating all MCP servers and tools
- Always applied (`alwaysApply: true`) across all files
- Enforces complete problem resolution before yielding control

### 🔧 Core Development Rules
- **`typescript.mdc`** - TypeScript best practices
- **`tailwind.mdc`** - Tailwind CSS and UI patterns
- **`clean-code.mdc`** - Clean code principles
- **`code-quality.mdc`** - Code quality guidelines

### 🏗️ Architecture & Design
- **`architecture.mdc`** - .NET microservices and Clean Architecture
- **`dotnet-ddd.mdc`** - .NET 9 DDD and Clean Architecture patterns
- **`microservices.mdc`** - Microservices architecture best practices
- **`system-design.mdc`** - System design, API design, and scalability patterns
- **`deployment.mdc`** - Deployment and CI/CD guidelines
- **`pinia.mdc`** - Pinia state management guidelines

### 🧪 Testing & Quality
- **`testing.mdc`** - Testing best practices
- **`nuxt-testing.mdc`** - Comprehensive Nuxt 4 testing guidelines
- **`performance.mdc`** - Performance optimization patterns
- **`security.mdc`** - Security best practices
- **`error-handling.mdc`** - Error handling and logging guidelines
- **`accessibility.mdc`** - Accessibility guidelines and WCAG compliance

### 🚀 Mobile & AI
- **`flutter.mdc`** - Flutter 3.x development with Riverpod and offline support
- **`langchain-ai.mdc`** - LangChain workflows, AI agents, and RAG systems

## Key Features

### Nuxt 4 Best Practices
- App Router patterns and directory structure
- Auto-imports and component organization
- Server-side features and API routes
- Performance optimization with @nuxt/image
- TypeScript integration and type safety
- Testing with Vitest and component testing

### Vue 3 + Composition API
- `<script setup>` syntax enforcement
- Proper composable patterns
- State management with Pinia
- Component composition over inheritance
- Proper TypeScript integration

### Beast Mode 4.1
- Autonomous problem-solving workflow
- Thorough implementation and testing
- Iterative improvement until perfection
- Context-aware development
- Quality assurance and edge case handling

## Usage

These rules are automatically applied by Cursor based on file patterns and `alwaysApply` settings. The Beast Mode rule ensures that all problems are solved completely before control is returned to the user.

## Maintenance

Rules are updated based on:
- New code patterns in the codebase
- Emerging best practices
- Technology updates (e.g., Nuxt 4)
- Code review feedback
- Performance and security improvements

## File Patterns

- **Vue/Nuxt**: `**/*.vue`, `**/*.ts`, `nuxt.config.ts`
- **Components**: `components/**/*`
- **Pages**: `pages/**/*`, `app/**/*`
- **Server**: `server/**/*`, `middleware/**/*`
- **All Files**: `**/*` (Beast Mode rule)

## 🚀 MCP Server Integration

### Available MCP Servers
- **task-master-ai**: Task management and project planning
- **filesystem**: File system operations and management
- **git**: Version control operations
- **web-search**: Web search capabilities for research
- **brave-search**: Advanced search with Brave API
- **sqlite**: SQLite database operations
- **postgres**: PostgreSQL database operations
- **docker**: Docker container operations
- **kubernetes**: Kubernetes cluster operations
- **http**: HTTP request testing and API calls
- **puppeteer**: Web automation and testing

### Setup Scripts
- **`scripts/install-mcp-servers.bat`** - Windows batch file for MCP server installation
- **`scripts/verify-setup.bat`** - Windows batch file for complete setup verification
- **`scripts/verify-setup.ps1`** - PowerShell script for setup verification
- **`scripts/verify-setup.sh`** - Linux/macOS shell script for setup verification

### Environment Configuration
- **`.env.template`** - Template for environment variables
- **`.env`** - Your actual API keys and configuration (not committed to git)
- **`.cursor/mcp.json`** - MCP server configuration

## 🎯 Autonomous Development Ready

Your TossErp project is now fully configured for autonomous development with:
- ✅ Comprehensive rules covering all technologies
- ✅ Beast Mode 4.1 for complete problem-solving
- ✅ All necessary MCP servers for autonomous operations
- ✅ Automated setup and verification scripts
- ✅ Cross-platform compatibility (Windows, macOS, Linux)
- ✅ Latest Nuxt 4 and Vue 3 best practices
- ✅ Complete testing, deployment, and accessibility guidelines
