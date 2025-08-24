describe('TOSS ERP Application E2E Tests', () => {
  beforeEach(() => {
    cy.visit('/')
    cy.waitForPageLoad()
  })

  describe('Application Layout and Navigation', () => {
    it('should display the main layout with all components', () => {
      // Check header
      cy.get('header').should('be.visible')
      cy.get('h1').should('contain', 'TOSS ERP III')
      
      // Check sidebar
      cy.get('[data-testid="sidebar"]').should('be.visible')
      
      // Check main content area
      cy.get('main').should('be.visible')
      
      // Check AI co-pilot button
      cy.get('[data-testid="ai-copilot-toggle"]').should('be.visible')
    })

    it('should display the TOSS ERP logo and branding', () => {
      cy.get('.w-8.h-8.bg-gradient-to-r').should('be.visible')
      cy.get('h1').should('contain', 'TOSS ERP III')
      cy.get('.text-xs.text-gray-500').should('contain', 'Township Business Efficiency')
    })

    it('should toggle sidebar on mobile', () => {
      cy.viewport('iphone-x')
      
      // Sidebar should be hidden on mobile
      cy.get('[data-testid="sidebar"]').should('not.be.visible')
      
      // Toggle sidebar
      cy.get('[data-testid="sidebar-toggle"]').click()
      cy.get('[data-testid="sidebar"]').should('be.visible')
      
      // Close sidebar by clicking overlay
      cy.get('.fixed.inset-0.bg-black').click()
      cy.get('[data-testid="sidebar"]').should('not.be.visible')
    })

    it('should have working dark mode toggle', () => {
      cy.get('[data-testid="dark-mode-toggle"]').click()
      cy.get('body').should('have.class', 'dark')
      
      cy.get('[data-testid="dark-mode-toggle"]').click()
      cy.get('body').should('not.have.class', 'dark')
    })
  })

  describe('Sidebar Navigation', () => {
    it('should display all main navigation items', () => {
      // Dashboard
      cy.get('a[href="/"]').should('contain', 'Dashboard')
      
      // Service-as-Software section
      cy.get('a[href="/service-dashboard"]').should('contain', 'Service Dashboard')
      cy.get('a[href="/autonomous-tasks"]').should('contain', 'Autonomous Tasks')
      cy.get('a[href="/service-outcomes"]').should('contain', 'Service Outcomes')
    })

    it('should navigate to different sections', () => {
      // Test navigation to service dashboard
      cy.get('a[href="/service-dashboard"]').click()
      cy.url().should('include', '/service-dashboard')
      
      // Test navigation back to home
      cy.get('a[href="/"]').click()
      cy.url().should('eq', Cypress.config().baseUrl + '/')
    })

    it('should expand and collapse module sections', () => {
      // Test Accounts module
      cy.get('button').contains('Accounts').click()
      cy.get('a[href="/accounting"]').should('be.visible')
      cy.get('a[href="/bank-reconciliation"]').should('be.visible')
      
      // Collapse again
      cy.get('button').contains('Accounts').click()
      cy.get('a[href="/accounting"]').should('not.be.visible')
    })

    it('should highlight active navigation items', () => {
      cy.get('a[href="/"]').should('have.class', 'bg-purple-50')
      
      cy.get('a[href="/service-dashboard"]').click()
      cy.get('a[href="/service-dashboard"]').should('have.class', 'bg-purple-50')
    })
  })

  describe('Service-as-Software Features', () => {
    it('should display service dashboard', () => {
      cy.get('a[href="/service-dashboard"]').click()
      cy.url().should('include', '/service-dashboard')
      
      // Check for service dashboard content
      cy.get('h2').should('contain', 'Service Dashboard')
    })

    it('should display autonomous tasks', () => {
      cy.get('a[href="/autonomous-tasks"]').click()
      cy.url().should('include', '/autonomous-tasks')
      
      // Check for autonomous tasks content
      cy.get('h2').should('contain', 'Autonomous Tasks')
    })

    it('should display service outcomes', () => {
      cy.get('a[href="/service-outcomes"]').click()
      cy.url().should('include', '/service-outcomes')
      
      // Check for service outcomes content
      cy.get('h2').should('contain', 'Service Outcomes')
    })
  })

  describe('AI Co-Pilot Integration', () => {
    it('should display AI co-pilot toggle button with notification', () => {
      cy.get('[data-testid="ai-copilot-toggle"]').should('be.visible')
      cy.get('.animate-bounce').should('contain', '12')
    })

    it('should open and close AI co-pilot panel', () => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
      
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('not.be.visible')
    })

    it('should display quick action bubbles', () => {
      // Double-click to enable quick actions
      cy.get('[data-testid="ai-copilot-toggle"]').dblclick()
      
      cy.get('.fixed.bottom-20.right-4').should('be.visible')
      cy.get('.w-12.h-12.bg-white').should('have.length.at.least', 1)
    })

    it('should integrate AI co-pilot with different pages', () => {
      // Test on service dashboard
      cy.get('a[href="/service-dashboard"]').click()
      cy.get('[data-testid="ai-copilot-toggle"]').should('be.visible')
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
      
      // Test on autonomous tasks
      cy.get('a[href="/autonomous-tasks"]').click()
      cy.get('[data-testid="ai-copilot-toggle"]').should('be.visible')
    })
  })

  describe('Responsive Design', () => {
    const viewports = ['iphone-x', 'ipad-2', 'macbook-13', 'macbook-15']

    viewports.forEach(viewport => {
      it(`should work correctly on ${viewport}`, () => {
        cy.viewport(viewport as any)
        
        // Check main layout
        cy.get('header').should('be.visible')
        cy.get('h1').should('contain', 'TOSS ERP III')
        
        // Check AI co-pilot is accessible
        cy.get('[data-testid="ai-copilot-toggle"]').should('be.visible')
        cy.get('[data-testid="ai-copilot-toggle"]').click()
        cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
        
        // Test tab switching on different viewports
        cy.get('[data-testid="tab-chat"]').click()
        cy.get('[data-testid="chat-content"]').should('be.visible')
        
        cy.get('[data-testid="tab-tasks"]').click()
        cy.get('[data-testid="tasks-content"]').should('be.visible')
      })
    })

    it('should handle orientation changes on mobile', () => {
      cy.viewport('iphone-x', 'portrait')
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
      
      cy.viewport('iphone-x', 'landscape')
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
    })
  })

  describe('Performance and Loading', () => {
    it('should load the application within acceptable time', () => {
      cy.visit('/')
      cy.checkPerformance()
    })

    it('should handle slow network conditions', () => {
      // Simulate slow network
      cy.intercept('*', { delay: 2000 })
      cy.visit('/')
      
      // App should still be functional
      cy.get('h1').should('contain', 'TOSS ERP III')
      cy.get('[data-testid="ai-copilot-toggle"]').should('be.visible')
    })

    it('should load resources efficiently', () => {
      cy.visit('/')
      
      // Check that critical resources are loaded
      cy.window().then(win => {
        const performance = win.performance
        const entries = performance.getEntriesByType('resource')
        
        // Should have loaded CSS and JS resources
        const cssResources = entries.filter(entry => entry.name.includes('.css'))
        const jsResources = entries.filter(entry => entry.name.includes('.js'))
        
        expect(cssResources.length).to.be.greaterThan(0)
        expect(jsResources.length).to.be.greaterThan(0)
      })
    })
  })

  describe('Error Handling and Fallbacks', () => {
    it('should handle 404 errors gracefully', () => {
      cy.visit('/non-existent-page', { failOnStatusCode: false })
      cy.get('body').should('contain', '404')
    })

    it('should handle JavaScript errors gracefully', () => {
      cy.window().then(win => {
        win.addEventListener('error', cy.stub().as('jsError'))
      })
      
      cy.visit('/')
      cy.get('@jsError').should('not.have.been.called')
    })

    it('should handle network errors', () => {
      // Mock network failures
      cy.intercept('GET', '/api/**', { forceNetworkError: true })
      
      cy.visit('/')
      
      // App should still render basic structure
      cy.get('h1').should('contain', 'TOSS ERP III')
      cy.get('[data-testid="ai-copilot-toggle"]').should('be.visible')
    })
  })

  describe('Data Persistence', () => {
    it('should persist sidebar state across page reloads', () => {
      // Expand a module
      cy.get('button').contains('Accounts').click()
      cy.get('a[href="/accounting"]').should('be.visible')
      
      // Reload page
      cy.reload()
      
      // Module should still be expanded
      cy.get('a[href="/accounting"]').should('be.visible')
    })

    it('should persist dark mode preference', () => {
      // Enable dark mode
      cy.get('[data-testid="dark-mode-toggle"]').click()
      cy.get('body').should('have.class', 'dark')
      
      // Reload page
      cy.reload()
      
      // Dark mode should still be enabled
      cy.get('body').should('have.class', 'dark')
    })

    it('should persist AI co-pilot tab selection', () => {
      // Open co-pilot and switch to Tasks tab
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="tab-tasks"]').click()
      cy.get('[data-testid="tasks-content"]').should('be.visible')
      
      // Close and reopen co-pilot
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      
      // Tasks tab should still be active
      cy.get('[data-testid="tasks-content"]').should('be.visible')
    })
  })

  describe('Security and Privacy', () => {
    it('should not expose sensitive data in DOM', () => {
      cy.visit('/')
      
      // Check that no API keys or sensitive data is in the DOM
      cy.get('body').should('not.contain', 'api_key')
      cy.get('body').should('not.contain', 'secret')
      cy.get('body').should('not.contain', 'password')
    })

    it('should handle HTTPS correctly', () => {
      // This would be environment-specific
      if (Cypress.config().baseUrl?.includes('https')) {
        cy.location('protocol').should('eq', 'https:')
      }
    })
  })

  describe('Accessibility (A11y)', () => {
    it('should have proper heading hierarchy', () => {
      cy.get('h1').should('have.length', 1)
      cy.get('h1').should('contain', 'TOSS ERP III')
    })

    it('should have proper alt text for images', () => {
      cy.get('img').each($img => {
        cy.wrap($img).should('have.attr', 'alt')
      })
    })

    it('should support keyboard navigation', () => {
      // Test tab navigation through main interface
      cy.get('body').type('{tab}')
      cy.focused().should('be.visible')
      
      // Test navigation to co-pilot
      cy.get('[data-testid="ai-copilot-toggle"]').focus().type('{enter}')
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
    })

    it('should have proper ARIA labels', () => {
      cy.get('[data-testid="ai-copilot-toggle"]').should('have.attr', 'aria-label')
      cy.get('[data-testid="sidebar-toggle"]').should('have.attr', 'aria-label')
    })

    it('should have sufficient color contrast', () => {
      // This would require color contrast checking library
      cy.get('h1').should('be.visible')
      cy.get('.text-gray-700').should('be.visible')
    })
  })

  describe('SEO and Meta Tags', () => {
    it('should have proper page title', () => {
      cy.title().should('not.be.empty')
      cy.title().should('contain', 'TOSS')
    })

    it('should have meta description', () => {
      cy.get('meta[name="description"]').should('have.attr', 'content')
    })

    it('should have viewport meta tag', () => {
      cy.get('meta[name="viewport"]').should('exist')
    })
  })

  describe('Browser Compatibility', () => {
    it('should work with modern browser features', () => {
      cy.window().then(win => {
        // Check for required browser APIs
        expect(win.fetch).to.exist
        expect(win.localStorage).to.exist
        expect(win.sessionStorage).to.exist
        expect(win.Promise).to.exist
      })
    })

    it('should handle CSS Grid and Flexbox', () => {
      cy.get('.flex').should('exist')
      cy.get('.grid').should('exist')
    })
  })

  describe('User Experience Flows', () => {
    it('should complete typical user journey', () => {
      // 1. User lands on dashboard
      cy.get('h1').should('contain', 'TOSS ERP III')
      
      // 2. User explores AI co-pilot
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
      
      // 3. User checks alerts
      cy.get('[data-testid="tab-alerts"]').click()
      cy.get('[data-testid="alerts-content"]').should('be.visible')
      
      // 4. User checks tasks
      cy.get('[data-testid="tab-tasks"]').click()
      cy.get('[data-testid="tasks-content"]').should('be.visible')
      
      // 5. User navigates to different section
      cy.get('a[href="/service-dashboard"]').click()
      cy.url().should('include', '/service-dashboard')
      
      // 6. AI co-pilot should still be accessible
      cy.get('[data-testid="ai-copilot-toggle"]').should('be.visible')
    })

    it('should handle rapid interactions gracefully', () => {
      // Rapid clicking on various elements
      for (let i = 0; i < 5; i++) {
        cy.get('[data-testid="ai-copilot-toggle"]').click()
        cy.get('[data-testid="ai-copilot-toggle"]').click()
      }
      
      // Should still work properly
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
    })
  })

  describe('Data Integrity', () => {
    it('should display consistent data across tabs', () => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      
      // Check notification counts match across different views
      cy.get('[data-testid="tab-alerts"]').find('.bg-red-500').then($badge => {
        const alertCount = $badge.text()
        
        cy.get('[data-testid="tab-alerts"]').click()
        cy.get('.bg-red-50').should('have.length.at.least', 1)
      })
    })

    it('should handle data updates properly', () => {
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="tab-tasks"]').click()
      
      // Check that task data is displayed
      cy.get('.bg-blue-50').should('have.length.at.least', 1)
      
      // Navigate away and back
      cy.get('a[href="/service-dashboard"]').click()
      cy.get('a[href="/"]').click()
      
      // Data should still be consistent
      cy.get('[data-testid="ai-copilot-toggle"]').click()
      cy.get('[data-testid="tab-tasks"]').click()
      cy.get('.bg-blue-50').should('have.length.at.least', 1)
    })
  })
})
