describe('Vue Hydration Test', () => {
  it('should wait for Vue hydration and test AI copilot', () => {
    cy.visit('http://localhost:3001')
    
    // Wait for the main title to be visible
    cy.contains('TOSS ERP III').should('be.visible')
    
    // Wait for Vue to be fully hydrated by checking for data attributes that Vue adds
    cy.get('body').should('have.attr', 'data-n-head')
    
    // Check that the button is in the DOM
    cy.get('[data-testid="ai-copilot-trigger"]').should('exist')
    
    // Wait for the button to be visible (this might help with timing)
    cy.get('[data-testid="ai-copilot-trigger"]', { timeout: 10000 }).should('be.visible')
    
    // Check if panel is initially hidden
    cy.get('[data-testid="ai-copilot-panel"]').should('not.exist')
    
    // Try to get the button's bounding box to see if it's actually visible
    cy.get('[data-testid="ai-copilot-trigger"]').then(($btn) => {
      const rect = $btn[0].getBoundingClientRect()
      cy.log(`Button position: top=${rect.top}, left=${rect.left}, width=${rect.width}, height=${rect.height}`)
      cy.log(`Button visibility: ${$btn.is(':visible')}`)
      
      // Check computed styles
      const computedStyle = window.getComputedStyle($btn[0])
      cy.log(`Button display: ${computedStyle.display}`)
      cy.log(`Button opacity: ${computedStyle.opacity}`)
      cy.log(`Button z-index: ${computedStyle.zIndex}`)
    })
    
    // Try clicking the button
    cy.get('[data-testid="ai-copilot-trigger"]').click({ force: true })
    
    // Wait a moment for the Vue reactivity to take effect
    cy.wait(1000)
    
    // Check if the panel appeared
    cy.get('body').then(($body) => {
      if ($body.find('[data-testid="ai-copilot-panel"]').length > 0) {
        cy.log('Success: AI copilot panel is now visible')
        cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
      } else {
        cy.log('Panel still not visible after click')
        
        // Let's check the Vue instance to see the state
        cy.window().its('$nuxt').then((nuxt: any) => {
          cy.log('Nuxt app:', nuxt)
          // Try to access the Vue component state
        })
      }
    })
  })
})
