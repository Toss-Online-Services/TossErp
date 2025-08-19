// ***********************************************************
// This example support/e2e.ts is processed and
// loaded automatically before your test files.
//
// This is a great place to put global configuration and
// behavior that modifies Cypress.
//
// You can change the location of this file or turn off
// automatically serving support files with the
// 'supportFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/configuration
// ***********************************************************

// Import commands.js using ES2015 syntax:
import './commands'

// Alternatively you can use CommonJS syntax:
// require('./commands')

// Hide fetch/XHR requests from command log
const app = window.top;
if (!app.document.head.querySelector('[data-hide-command-log-request]')) {
  const style = app.document.createElement('style');
  style.innerHTML =
    '.command-name-request, .command-name-xhr { display: none }';
  style.setAttribute('data-hide-command-log-request', '');
  app.document.head.appendChild(style);
}

// Global configuration
Cypress.on('uncaught:exception', (err, runnable) => {
  // returning false here prevents Cypress from failing the test
  // on uncaught exceptions
  if (err.message.includes('ResizeObserver loop limit exceeded')) {
    return false;
  }
  return true;
});

// Custom viewport sizes for responsive testing
Cypress.Commands.add('viewportMobile', () => {
  cy.viewport('iphone-x');
});

Cypress.Commands.add('viewportTablet', () => {
  cy.viewport('ipad-2');
});

Cypress.Commands.add('viewportDesktop', () => {
  cy.viewport(1280, 720);
});

// Custom commands for common actions
Cypress.Commands.add('login', (email: string, password: string) => {
  cy.visit('/login');
  cy.get('input[name="email"]').type(email);
  cy.get('input[name="password"]').type(password);
  cy.get('button[type="submit"]').click();
  cy.url().should('not.include', '/login');
});

Cypress.Commands.add('logout', () => {
  cy.get('[data-testid="user-menu"]').click();
  cy.get('[data-testid="logout-button"]').click();
  cy.url().should('include', '/login');
});

Cypress.Commands.add('openChatbot', () => {
  cy.get('.fixed.bottom-4.right-4 button').click();
  cy.get('.w-96.h-96').should('be.visible');
});

Cypress.Commands.add('closeChatbot', () => {
  cy.get('.fixed.bottom-4.right-4 button').click();
  cy.get('.w-96.h-96').should('not.be.visible');
});

Cypress.Commands.add('sendChatbotMessage', (message: string) => {
  cy.openChatbot();
  cy.get('input[placeholder="Type your message..."]').type(message + '{enter}');
  cy.wait(2000); // Wait for bot response
});

Cypress.Commands.add('navigateToPage', (pageName: string) => {
  cy.get('nav').contains(pageName).click();
  cy.url().should('include', pageName.toLowerCase().replace(' ', '-'));
});

Cypress.Commands.add('refreshDashboard', () => {
  cy.get('button').contains('Refresh').click();
  cy.wait(2000); // Wait for refresh to complete
});

// Type definitions for custom commands
declare global {
  namespace Cypress {
    interface Chainable {
      viewportMobile(): Chainable<void>
      viewportTablet(): Chainable<void>
      viewportDesktop(): Chainable<void>
      login(email: string, password: string): Chainable<void>
      logout(): Chainable<void>
      openChatbot(): Chainable<void>
      closeChatbot(): Chainable<void>
      sendChatbotMessage(message: string): Chainable<void>
      navigateToPage(pageName: string): Chainable<void>
      refreshDashboard(): Chainable<void>
    }
  }
}
