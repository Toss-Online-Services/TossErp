// ***********************************************
// This example commands.ts shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************

// Custom command to wait for page load
Cypress.Commands.add('waitForPageLoad', () => {
  cy.get('body').should('not.have.class', 'loading');
  cy.get('[data-testid="loading-spinner"]').should('not.exist');
});

// Custom command to check if element is visible and clickable
Cypress.Commands.add('clickIfVisible', (selector: string) => {
  cy.get('body').then(($body) => {
    if ($body.find(selector).length > 0) {
      cy.get(selector).should('be.visible').click();
    }
  });
});

// Custom command to type with delay (useful for forms)
Cypress.Commands.add('typeWithDelay', (selector: string, text: string, delay: number = 100) => {
  cy.get(selector).clear();
  text.split('').forEach((char) => {
    cy.get(selector).type(char, { delay });
  });
});

// Custom command to check toast notifications
Cypress.Commands.add('checkToast', (message: string, type: 'success' | 'error' | 'warning' = 'success') => {
  cy.get(`[data-testid="toast-${type}"]`).should('contain', message);
});

// Custom command to wait for API response
Cypress.Commands.add('waitForApi', (method: string, url: string) => {
  cy.intercept(method, url).as('apiCall');
  cy.wait('@apiCall');
});

// Custom command to mock API response
Cypress.Commands.add('mockApi', (method: string, url: string, response: any) => {
  cy.intercept(method as any, url, response).as('mockedApi');
});

// Custom command to check if element exists
Cypress.Commands.add('elementExists', (selector: string) => {
  cy.get('body').then(($body) => {
    return $body.find(selector).length > 0;
  });
});

// Custom command to scroll to element
Cypress.Commands.add('scrollToElement', (selector: string) => {
  cy.get(selector).scrollIntoView();
});

// Custom command to check responsive behavior
Cypress.Commands.add('checkResponsive', (viewport: string, callback: () => void) => {
  cy.viewport(viewport as any);
  callback();
});

// Custom command to take screenshot with custom name
Cypress.Commands.add('takeScreenshot', (name: string) => {
  cy.screenshot(name);
});

// Custom command to check accessibility (mocked - requires cypress-axe plugin)
Cypress.Commands.add('checkAccessibility', () => {
  // Mock implementation - would require installing cypress-axe
  // cy.injectAxe();
  // cy.checkA11y();
  cy.log('Accessibility check skipped - cypress-axe not installed');
});

// Custom command to wait for animation to complete
Cypress.Commands.add('waitForAnimation', (selector: string) => {
  cy.get(selector).should('not.have.class', 'animate');
});

// Custom command to check if data is loaded
Cypress.Commands.add('waitForData', (selector: string) => {
  cy.get(selector).should('not.contain', 'Loading...');
  cy.get(selector).should('not.contain', 'No data available');
});

// Custom command to handle file upload (mocked - requires cypress-file-upload plugin)
Cypress.Commands.add('uploadFile', (selector: string, filePath: string) => {
  // Mock implementation - would require installing cypress-file-upload
  // cy.get(selector).attachFile(filePath);
  cy.get(selector).selectFile(filePath, { force: true });
});

// Custom command to check if modal is open
Cypress.Commands.add('checkModalOpen', (modalSelector: string) => {
  cy.get(modalSelector).should('be.visible');
  cy.get('body').should('have.class', 'modal-open');
});

// Custom command to close modal
Cypress.Commands.add('closeModal', (modalSelector: string) => {
  cy.get(`${modalSelector} [data-testid="close-button"]`).click();
  cy.get(modalSelector).should('not.be.visible');
});

// Custom command to check form validation
Cypress.Commands.add('checkFormValidation', (formSelector: string) => {
  cy.get(`${formSelector} [data-testid="submit-button"]`).click();
  cy.get(`${formSelector} .error-message`).should('be.visible');
});

// Custom command to fill form fields
Cypress.Commands.add('fillForm', (formData: Record<string, string>) => {
  Object.entries(formData).forEach(([field, value]) => {
    cy.get(`[name="${field}"]`).type(value);
  });
});

// Custom command to check table data
Cypress.Commands.add('checkTableData', (tableSelector: string, expectedData: any[]) => {
  cy.get(`${tableSelector} tbody tr`).should('have.length', expectedData.length);
  expectedData.forEach((row, index) => {
    Object.entries(row).forEach(([key, value]) => {
      cy.get(`${tableSelector} tbody tr`).eq(index).should('contain', value);
    });
  });
});

// Custom command to sort table column
Cypress.Commands.add('sortTableColumn', (tableSelector: string, columnName: string) => {
  cy.get(`${tableSelector} th`).contains(columnName).click();
});

// Custom command to filter table
Cypress.Commands.add('filterTable', (filterSelector: string, filterValue: string) => {
  cy.get(filterSelector).type(filterValue);
  cy.wait(500); // Wait for filter to apply
});

// Custom command to paginate table
Cypress.Commands.add('paginateTable', (pageNumber: number) => {
  cy.get('[data-testid="pagination"]').contains(pageNumber.toString()).click();
});

// Custom command to export data
Cypress.Commands.add('exportData', (exportType: 'csv' | 'excel' | 'pdf') => {
  cy.get(`[data-testid="export-${exportType}"]`).click();
  cy.wait(2000); // Wait for export to complete
});

// Custom command to check chart data
Cypress.Commands.add('checkChartData', (chartSelector: string, expectedData: any) => {
  cy.get(chartSelector).should('be.visible');
  // Add specific chart data validation based on chart library
});

// Custom command to toggle sidebar
Cypress.Commands.add('toggleSidebar', () => {
  cy.get('[data-testid="sidebar-toggle"]').click();
});

// Custom command to check notification count
Cypress.Commands.add('checkNotificationCount', (expectedCount: number) => {
  cy.get('[data-testid="notification-badge"]').should('contain', expectedCount.toString());
});

// Custom command to mark notification as read
Cypress.Commands.add('markNotificationAsRead', (notificationId: string) => {
  cy.get(`[data-testid="notification-${notificationId}"]`).click();
  cy.get(`[data-testid="notification-${notificationId}"]`).should('have.class', 'read');
});

// Custom command to search global
Cypress.Commands.add('globalSearch', (searchTerm: string) => {
  cy.get('[data-testid="global-search"]').type(searchTerm);
  cy.get('[data-testid="search-results"]').should('be.visible');
});

// Custom command to check user permissions
Cypress.Commands.add('checkUserPermissions', (permissions: string[]) => {
  permissions.forEach(permission => {
    cy.get(`[data-testid="permission-${permission}"]`).should('be.visible');
  });
});

// Custom command to switch user role
Cypress.Commands.add('switchUserRole', (role: string) => {
  cy.get('[data-testid="user-role-selector"]').select(role);
  cy.get('[data-testid="role-confirmation"]').click();
});

// Custom command to check system health
Cypress.Commands.add('checkSystemHealth', () => {
  cy.request('/api/health').then((response) => {
    expect(response.status).to.eq(200);
    expect(response.body.status).to.eq('healthy');
  });
});

// Custom command to clear browser data
Cypress.Commands.add('clearBrowserData', () => {
  cy.clearCookies();
  cy.clearLocalStorage();
  cy.clearAllSessionStorage();
});

// Custom command to check performance metrics
Cypress.Commands.add('checkPerformance', () => {
  cy.window().then((win) => {
    const performance = win.performance;
    const navigation = performance.getEntriesByType('navigation')[0] as PerformanceNavigationTiming;
    
    // Check page load time
    expect(navigation.loadEventEnd - navigation.loadEventStart).to.be.lessThan(3000);
    
    // Check DOM content loaded time
    expect(navigation.domContentLoadedEventEnd - navigation.domContentLoadedEventStart).to.be.lessThan(2000);
  });
});

// Custom command for keyboard navigation (tab key)
Cypress.Commands.add('tabKey', () => {
  cy.focused().type('{tab}');
});

// Export commands for use in other files
export {};
