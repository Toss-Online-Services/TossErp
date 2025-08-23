/// <reference types="cypress" />

declare namespace Cypress {
  interface Chainable {
    /**
     * Custom command to wait for page load
     */
    waitForPageLoad(): Chainable<void>

    /**
     * Custom command to check if element is visible and clickable
     */
    clickIfVisible(selector: string): Chainable<void>

    /**
     * Custom command to type with delay (useful for forms)
     */
    typeWithDelay(selector: string, text: string, delay?: number): Chainable<void>

    /**
     * Custom command to check toast notifications
     */
    checkToast(message: string, type?: 'success' | 'error' | 'warning'): Chainable<void>

    /**
     * Custom command to wait for API response
     */
    waitForApi(method: string, url: string): Chainable<void>

    /**
     * Custom command to mock API response
     */
    mockApi(method: string, url: string, response: any): Chainable<void>

    /**
     * Custom command to check if element exists
     */
    elementExists(selector: string): Chainable<boolean>

    /**
     * Custom command to scroll to element
     */
    scrollToElement(selector: string): Chainable<void>

    /**
     * Custom command to check responsive behavior
     */
    checkResponsive(viewport: string, callback: () => void): Chainable<void>

    /**
     * Custom command to take screenshot with custom name
     */
    takeScreenshot(name: string): Chainable<void>

    /**
     * Custom command to check accessibility (mocked)
     */
    checkAccessibility(): Chainable<void>

    /**
     * Custom command to wait for animation to complete
     */
    waitForAnimation(selector: string): Chainable<void>

    /**
     * Custom command to check if data is loaded
     */
    waitForData(selector: string): Chainable<void>

    /**
     * Custom command to handle file upload (mocked)
     */
    uploadFile(selector: string, filePath: string): Chainable<void>

    /**
     * Custom command to check if modal is open
     */
    checkModalOpen(modalSelector: string): Chainable<void>

    /**
     * Custom command to close modal
     */
    closeModal(modalSelector: string): Chainable<void>

    /**
     * Custom command to check form validation
     */
    checkFormValidation(formSelector: string): Chainable<void>

    /**
     * Custom command to fill form fields
     */
    fillForm(formData: Record<string, string>): Chainable<void>

    /**
     * Custom command to check table data
     */
    checkTableData(tableSelector: string, expectedData: any[]): Chainable<void>

    /**
     * Custom command to sort table column
     */
    sortTableColumn(tableSelector: string, columnName: string): Chainable<void>

    /**
     * Custom command to filter table
     */
    filterTable(filterSelector: string, filterValue: string): Chainable<void>

    /**
     * Custom command to paginate table
     */
    paginateTable(pageNumber: number): Chainable<void>

    /**
     * Custom command to export data
     */
    exportData(exportType: 'csv' | 'excel' | 'pdf'): Chainable<void>

    /**
     * Custom command to check chart data
     */
    checkChartData(chartSelector: string, expectedData: any): Chainable<void>

    /**
     * Custom command to toggle sidebar
     */
    toggleSidebar(): Chainable<void>

    /**
     * Custom command to check notification count
     */
    checkNotificationCount(expectedCount: number): Chainable<void>

    /**
     * Custom command to mark notification as read
     */
    markNotificationAsRead(notificationId: string): Chainable<void>

    /**
     * Custom command to search global
     */
    globalSearch(searchTerm: string): Chainable<void>

    /**
     * Custom command to check user permissions
     */
    checkUserPermissions(permissions: string[]): Chainable<void>

    /**
     * Custom command to switch user role
     */
    switchUserRole(role: string): Chainable<void>

    /**
     * Custom command to check system health
     */
    checkSystemHealth(): Chainable<void>

    /**
     * Custom command to clear browser data
     */
    clearBrowserData(): Chainable<void>

    /**
     * Custom command to check performance metrics
     */
    checkPerformance(): Chainable<void>

    /**
     * Custom command for keyboard navigation (tab key)
     */
    tabKey(): Chainable<void>
  }
}
