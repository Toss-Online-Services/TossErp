<!-- # Nuxt 4 Testing Guidelines

## Testing Stack
- **Unit Testing**: Vitest for fast, modern unit testing
- **Component Testing**: Vue Test Utils with Vitest for Vue component testing
- **E2E Testing**: Playwright for end-to-end testing
- **API Testing**: Supertest or Playwright for API endpoint testing

## Vitest Configuration
- Use `vitest.config.ts` for configuration
- Enable Vue plugin for component testing
- Configure test environment and coverage
- Use proper test file naming conventions

## Component Testing
- Test Vue components in isolation
- Mock external dependencies properly
- Test component props, events, and slots
- Use proper test utilities and helpers
- Test component lifecycle and reactivity

## Unit Testing
- Test composables and utilities
- Mock API calls and external services
- Test error handling and edge cases
- Use proper test data and fixtures
- Maintain high test coverage (>80%)

## E2E Testing with Playwright
- Test complete user workflows
- Test responsive design across devices
- Test accessibility and keyboard navigation
- Use proper page object models
- Test error scenarios and edge cases

## Testing Best Practices
- Follow AAA pattern (Arrange, Act, Assert)
- Use descriptive test names
- Keep tests focused and independent
- Use proper mocking strategies
- Test both success and failure scenarios

## Test Organization
- Group tests by feature or component
- Use proper test suites and describe blocks
- Keep test files organized and maintainable
- Use shared test utilities and helpers
- Implement proper test data management

## Performance Testing
- Test component rendering performance
- Test API response times
- Test bundle size and loading performance
- Use performance monitoring tools
- Test under different network conditions

## Accessibility Testing
- Test with screen readers
- Test keyboard navigation
- Test color contrast and readability
- Test ARIA attributes and labels
- Use accessibility testing tools
- Always test in @browser and capture screenshots, to ensure the UI is accessible and readable and working as expected.
 
## Continuous Integration
- Run tests on every commit
- Enforce minimum test coverage
- Use proper CI/CD pipelines
- Test across different environments
- Monitor test performance and reliability
description: Comprehensive testing guidelines for Nuxt 4 applications including Vitest, component testing, and E2E testing
globs: **/*.test.ts, **/*.spec.ts, **/*.test.js, **/*.spec.js, **/*.test.vue, **/*.spec.vue, vitest.config.*, playwright.config.*
alwaysApply: true
---
 -->