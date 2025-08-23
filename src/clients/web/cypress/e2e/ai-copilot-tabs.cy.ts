describe('AI Co-Pilot Tabs E2E Tests', () => {
  beforeEach(() => {
    cy.visit('/')
    cy.waitForPageLoad()
  })

  describe('Co-Pilot Initial State', () => {
    it('should display AI co-pilot toggle button', () => {
      cy.get('[data-testid="ai-copilot-toggle"]').should('be.visible')
      cy.get('[data-testid="ai-copilot-toggle"] svg').should('be.visible')
    })

    it('should show notification badge on co-pilot button', () => {
      cy.get('[data-testid="ai-copilot-toggle"]')
        .parent()
        .find('.animate-bounce')
        .should('be.visible')
        .should('contain.text', '12') // Default notification count
    })

    it('should display quick action bubbles when enabled', () => {
      // Enable quick actions first
      cy.get('[data-testid="ai-copilot-toggle"]').dblclick()
      
      // Check for quick action bubbles
      cy.get('.fixed.bottom-20.right-4 .space-y-3').should('be.visible')
      cy.get('.w-12.h-12.bg-white').should('have.length.at.least', 1)
    })
  })

  describe('Co-Pilot Panel Opening and Closing', () => {
    it('should open AI co-pilot panel when toggle button is clicked', () => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
      cy.get('[data-testid="ai-copilot-panel"]').should('have.class', 'fixed')
      cy.get('[data-testid="ai-copilot-panel"]').should('have.class', 'w-96')
    })

    it('should close AI co-pilot panel when backdrop is clicked', () => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
      
      // Click backdrop
      cy.get('.fixed.inset-0.bg-black.bg-opacity-25').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('not.be.visible')
    })

    it('should close AI co-pilot panel when toggle button is clicked again', () => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
      
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('not.be.visible')
    })

    it('should display co-pilot header with correct title and status', () => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
      
      cy.get('h4').should('contain', 'AI Co-Pilot')
      cy.get('.text-green-500').should('contain', 'Actively Monitoring')
    })
  })

  describe('Tab Navigation', () => {
    beforeEach(() => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
    })

    it('should display all three tabs (Chat, Alerts, Tasks)', () => {
      cy.get('[data-testid="tab-chat"]').should('be.visible').should('contain', 'Chat')
      cy.get('[data-testid="tab-alerts"]').should('be.visible').should('contain', 'Alerts')
      cy.get('[data-testid="tab-tasks"]').should('be.visible').should('contain', 'Tasks')
    })

    it('should have Alerts tab active by default', () => {
      cy.get('[data-testid="tab-alerts"]')
        .should('have.class', 'border-purple-500')
        .should('have.class', 'text-purple-600')
    })

    it('should switch to Chat tab when clicked', () => {
      cy.get('[data-testid="tab-chat"]').click()
      
      // Check tab is active
      cy.get('[data-testid="tab-chat"]')
        .should('have.class', 'border-purple-500')
        .should('have.class', 'text-purple-600')
      
      // Check content is visible
      cy.get('[data-testid="chat-content"]').should('be.visible')
    })

    it('should switch to Tasks tab when clicked', () => {
      cy.get('[data-testid="tab-tasks"]').click()
      
      // Check tab is active
      cy.get('[data-testid="tab-tasks"]')
        .should('have.class', 'border-purple-500')
        .should('have.class', 'text-purple-600')
      
      // Check content is visible
      cy.get('[data-testid="tasks-content"]').should('be.visible')
    })

    it('should display correct notification badges on tabs', () => {
      // Alerts tab should show notification count
      cy.get('[data-testid="tab-alerts"]')
        .find('.bg-red-500')
        .should('contain', '12')
      
      // Tasks tab should show notification count
      cy.get('[data-testid="tab-tasks"]')
        .find('.bg-blue-500')
        .should('contain', '5')
    })

    it('should persist tab state when panel is reopened', () => {
      // Switch to Tasks tab
      cy.get('[data-testid="tab-tasks"]').click()
      cy.get('[data-testid="tasks-content"]').should('be.visible')
      
      // Close and reopen panel
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('not.be.visible')
      
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
      
      // Tasks tab should still be active
      cy.get('[data-testid="tab-tasks"]')
        .should('have.class', 'border-purple-500')
      cy.get('[data-testid="tasks-content"]').should('be.visible')
    })
  })

  describe('Chat Tab Functionality', () => {
    beforeEach(() => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="tab-chat"]').click()
    })

    it('should display welcome message', () => {
      cy.get('[data-testid="chat-content"]').should('contain', 'Welcome!')
      cy.get('[data-testid="chat-content"]').should('contain', "I'm actively monitoring your business")
    })

    it('should display Show Opportunities button', () => {
      cy.get('button').contains('📊 Show Opportunities').should('be.visible')
    })

    it('should display Daily Summary button', () => {
      cy.get('button').contains('📋 Daily Summary').should('be.visible')
    })

    it('should handle Show Opportunities button click', () => {
      cy.get('button').contains('📊 Show Opportunities').click()
      // Add assertions for what should happen when clicked
    })

    it('should handle Daily Summary button click', () => {
      cy.get('button').contains('📋 Daily Summary').click()
      // Add assertions for what should happen when clicked
    })

    it('should be scrollable when content exceeds height', () => {
      cy.get('[data-testid="chat-content"]')
        .should('have.css', 'overflow-y', 'auto')
        .should('have.css', 'max-height', '400px')
    })
  })

  describe('Alerts Tab Functionality', () => {
    beforeEach(() => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="tab-alerts"]').click()
    })

    it('should display debug information', () => {
      cy.get('.bg-purple-100').should('contain', '12 alerts found')
      cy.get('.bg-purple-100').should('contain', 'VISIBLE')
    })

    it('should display inventory alert', () => {
      cy.get('.bg-red-50').should('contain', 'INVENTORY - HIGH')
      cy.get('.bg-red-50').should('contain', 'Stock Alert: Cooking Oil below threshold')
    })

    it('should display alert actions', () => {
      cy.get('button').contains('Take Action').should('be.visible')
      cy.get('button').contains('View Details').should('be.visible')
    })

    it('should handle Take Action button click', () => {
      cy.get('button').contains('Take Action').first().click()
      // Add assertions for what should happen when clicked
    })

    it('should display alert timestamp', () => {
      cy.get('.text-xs.text-red-600').should('contain', 'ago')
    })

    it('should be scrollable when content exceeds height', () => {
      cy.get('[data-testid="alerts-content"]')
        .should('have.css', 'overflow-y', 'auto')
        .should('have.css', 'max-height', '400px')
    })

    it('should close individual alerts', () => {
      cy.get('button[title="Close alert"]').first().click()
      // Verify alert is removed or marked as closed
    })
  })

  describe('Tasks Tab Functionality', () => {
    beforeEach(() => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="tab-tasks"]').click()
    })

    it('should display debug information', () => {
      cy.get('.bg-purple-100').should('contain', 'Tasks: 5')
      cy.get('.bg-purple-100').should('contain', 'VISIBLE')
    })

    it('should display task items', () => {
      cy.get('.bg-blue-50').should('have.length.at.least', 1)
    })

    it('should display task details', () => {
      cy.get('.bg-blue-50').first().within(() => {
        cy.get('.text-sm.font-medium').should('be.visible')
        cy.get('.text-xs').should('contain', '85%') // Progress percentage
        cy.get('.text-xs').should('contain', 'minutes') // Estimated completion
      })
    })

    it('should display progress bars for tasks', () => {
      cy.get('.bg-blue-50').first().within(() => {
        cy.get('.bg-blue-500').should('be.visible') // Progress bar
        cy.get('.text-xs').should('contain', 'Progress')
      })
    })

    it('should display task types', () => {
      cy.get('.bg-blue-50').should('contain', 'Inventory Monitoring')
      cy.get('.bg-blue-50').should('contain', 'Invoice Processing')
    })

    it('should be scrollable when content exceeds height', () => {
      cy.get('[data-testid="tasks-content"]')
        .should('have.css', 'overflow-y', 'auto')
        .should('have.css', 'max-height', '400px')
    })

    it('should display empty state when no tasks', () => {
      // This would require mocking empty tasks state
      // cy.mockApi('GET', '/api/tasks', [])
      // cy.reload()
      // cy.get('[data-testid="tab-tasks"]').click()
      // cy.get('.text-center').should('contain', 'No active automations')
    })

    it('should handle task completion updates', () => {
      // Check that progress updates are reflected
      cy.get('.bg-blue-50').first().within(() => {
        cy.get('.text-xs').should('contain', '%')
      })
    })
  })

  describe('Scrollbar Styling', () => {
    beforeEach(() => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
    })

    it('should have custom scrollbar styling on all tabs', () => {
      // Test Chat tab scrollbar
      cy.get('[data-testid="tab-chat"]').click()
      cy.get('.ai-copilot-content').should('have.css', 'overflow-y', 'auto')

      // Test Alerts tab scrollbar
      cy.get('[data-testid="tab-alerts"]').click()
      cy.get('.ai-copilot-content').should('have.css', 'overflow-y', 'auto')

      // Test Tasks tab scrollbar
      cy.get('[data-testid="tab-tasks"]').click()
      cy.get('.ai-copilot-content').should('have.css', 'overflow-y', 'auto')
    })
  })

  describe('Responsive Behavior', () => {
    it('should work properly on mobile devices', () => {
      cy.viewport('iphone-x')
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
      
      // Test all tabs on mobile
      cy.get('[data-testid="tab-chat"]').click()
      cy.get('[data-testid="chat-content"]').should('be.visible')
      
      cy.get('[data-testid="tab-alerts"]').click()
      cy.get('[data-testid="alerts-content"]').should('be.visible')
      
      cy.get('[data-testid="tab-tasks"]').click()
      cy.get('[data-testid="tasks-content"]').should('be.visible')
    })

    it('should work properly on tablet devices', () => {
      cy.viewport('ipad-2')
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
      
      // Test tab functionality on tablet
      cy.get('[data-testid="tab-tasks"]').click()
      cy.get('[data-testid="tasks-content"]').should('be.visible')
    })
  })

  describe('Performance and Animations', () => {
    it('should have smooth tab transitions', () => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      
      // Test rapid tab switching
      cy.get('[data-testid="tab-chat"]').click()
      cy.get('[data-testid="tab-alerts"]').click()
      cy.get('[data-testid="tab-tasks"]').click()
      cy.get('[data-testid="tab-chat"]').click()
      
      // Should end up on Chat tab
      cy.get('[data-testid="chat-content"]').should('be.visible')
    })

    it('should handle rapid panel opening and closing', () => {
      for (let i = 0; i < 3; i++) {
        cy.get('[data-testid="ai-copilot-toggle"]').click()
        cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
        cy.get('[data-testid="ai-copilot-toggle"]').click()
        cy.get('[data-testid="ai-copilot-panel"]').should('not.be.visible')
      }
    })
  })

  describe('Accessibility', () => {
    beforeEach(() => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
    })

    it('should support keyboard navigation between tabs', () => {
      cy.get('[data-testid="tab-chat"]').focus().type('{enter}')
      cy.get('[data-testid="chat-content"]').should('be.visible')
      
      cy.get('[data-testid="tab-alerts"]').focus().type('{enter}')
      cy.get('[data-testid="alerts-content"]').should('be.visible')
      
      cy.get('[data-testid="tab-tasks"]').focus().type('{enter}')
      cy.get('[data-testid="tasks-content"]').should('be.visible')
    })

    it('should have proper ARIA labels', () => {
      cy.get('[data-testid="tab-chat"]').should('have.attr', 'role', 'tab')
      cy.get('[data-testid="tab-alerts"]').should('have.attr', 'role', 'tab')
      cy.get('[data-testid="tab-tasks"]').should('have.attr', 'role', 'tab')
    })

    it('should announce tab changes to screen readers', () => {
      cy.get('[data-testid="tab-tasks"]').should('have.attr', 'aria-selected')
    })
  })

  describe('Error Handling', () => {
    it('should handle missing data gracefully', () => {
      // Mock empty responses
      cy.intercept('GET', '/api/alerts', []).as('emptyAlerts')
      cy.intercept('GET', '/api/tasks', []).as('emptyTasks')
      
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      
      // Should still display tabs even with no data
      cy.get('[data-testid="tab-alerts"]').should('be.visible')
      cy.get('[data-testid="tab-tasks"]').should('be.visible')
    })

    it('should handle network errors gracefully', () => {
      // Mock network errors
      cy.intercept('GET', '/api/alerts', { forceNetworkError: true }).as('alertsError')
      cy.intercept('GET', '/api/tasks', { forceNetworkError: true }).as('tasksError')
      
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      
      // Should still display interface
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
    })
  })

  describe('Data Updates', () => {
    it('should update notification badges when data changes', () => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      
      // Initial counts
      cy.get('[data-testid="tab-alerts"]').find('.bg-red-500').should('contain', '12')
      cy.get('[data-testid="tab-tasks"]').find('.bg-blue-500').should('contain', '5')
      
      // Mock data updates
      // This would require real-time updates or polling implementation
    })

    it('should refresh data when tab is switched', () => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      
      cy.get('[data-testid="tab-alerts"]').click()
      cy.get('[data-testid="tab-tasks"]').click()
      cy.get('[data-testid="tab-alerts"]').click()
      
      // Should maintain data integrity
      cy.get('[data-testid="alerts-content"]').should('be.visible')
    })
  })
})
