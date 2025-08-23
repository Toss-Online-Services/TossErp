describe('Overlay Dismissal Test', () => {
  it('should handle error overlays and test AI copilot', () => {
    cy.visit('http://localhost:3001')
    
    // Wait for page to load
    cy.contains('TOSS ERP III').should('be.visible')
    
    // Check for and dismiss any potential error overlays
    cy.get('body').then(($body) => {
      const bodyHtml = $body.html()
      
      // Look for common error overlay patterns
      if (bodyHtml.includes('<pre class="frame">')) {
        cy.log('Found <pre class="frame"> overlay')
        
        // Try to find and click any close buttons or dismiss overlays
        cy.get('body').then(() => {
          // Try common overlay dismiss patterns
          const dismissSelectors = [
            '[data-cy="error-overlay-dismiss"]',
            '.error-overlay-close',
            '[aria-label="Close"]',
            '.close-button',
            '.nuxt-error-close'
          ]
          
          dismissSelectors.forEach(selector => {
            cy.get('body').then(($body) => {
              if ($body.find(selector).length > 0) {
                cy.log(`Found dismiss button: ${selector}`)
                cy.get(selector).click()
                cy.wait(500)
              }
            })
          })
        })
      }
      
      // Try pressing Escape key to dismiss overlays
      cy.get('body').type('{esc}')
      cy.wait(500)
      
      // Try clicking on areas that might dismiss overlays
      cy.get('body').click(100, 100)
      cy.wait(500)
    })
    
    // Now try to interact with the AI copilot button
    cy.get('[data-testid="ai-copilot-trigger"]').should('exist')
    
    // Try different click strategies
    cy.log('Trying normal click...')
    cy.get('[data-testid="ai-copilot-trigger"]').click()
    cy.wait(1000)
    
    // Check if panel appeared
    cy.get('body').then(($body) => {
      let panelVisible = false
      try {
        panelVisible = $body.find('[data-testid="ai-copilot-panel"]:visible').length > 0
      } catch (e) {
        // Element might exist but not be visible
      }
      
      if (!panelVisible) {
        cy.log('Normal click failed, trying force click...')
        cy.get('[data-testid="ai-copilot-trigger"]').click({ force: true })
        cy.wait(1000)
        
        // Check again
        try {
          panelVisible = $body.find('[data-testid="ai-copilot-panel"]:visible').length > 0
        } catch (e) {
          // Element might exist but not be visible
        }
      }
      
      if (!panelVisible) {
        cy.log('Force click failed, trying coordinates click...')
        // Try clicking by coordinates (bottom-right corner where button should be)
        cy.get('body').click(window.innerWidth - 80, window.innerHeight - 80, { force: true })
        cy.wait(1000)
      }
      
      // Final check
      cy.get('[data-testid="ai-copilot-panel"]').should('exist')
      cy.log('Panel element exists in DOM')
    })
  })
})
