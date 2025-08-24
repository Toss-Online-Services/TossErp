describe('Debug Test', () => {
  beforeEach(() => {
    cy.visit('http://localhost:3001')
  })

  it('should debug AI co-pilot button and state', () => {
    cy.contains('TOSS ERP III').should('be.visible')
    
    // Check if trigger button exists
    cy.get('[data-testid="ai-copilot-trigger"]').should('be.visible')
    
    // Check initial state
    cy.get('[data-testid="ai-copilot-panel"]').should('not.exist')
    
    // Debug: Check what happens when we click
    cy.get('[data-testid="ai-copilot-trigger"]').click({ force: true })
    
    // Wait and check DOM changes
    cy.wait(2000)
    
    // Check if anything changed in the DOM
    cy.get('body').then(($body) => {
      const panelExists = $body.find('[data-testid="ai-copilot-panel"]').length > 0
      cy.log(`Panel exists after click: ${panelExists}`)
      
      if (!panelExists) {
        cy.log('Panel did not appear - checking if element is in DOM but hidden')
        cy.get('body').then(($body) => {
          if ($body.html().includes('ai-copilot-panel')) {
            cy.log('Panel element found in DOM but might be hidden')
          } else {
            cy.log('Panel element not found in DOM at all')
          }
        })
      }
    })
    
    // Try to trigger the function directly via Vue devtools
    cy.window().then((win) => {
      // Check if Vue devtools global is available
      if ((win as any).__VUE__) {
        cy.log('Vue devtools available')
      } else {
        cy.log('Vue devtools not available')
      }
      
      // Try to access Vue app instance
      if ((win as any).__VUE_APP__) {
        cy.log('Vue app instance available')
      } else {
        cy.log('Vue app instance not available')
      }
      
      // Check if our test function is available
      if ((win as any).testToggleCopilot) {
        cy.log('Test toggle function available')
        const result = (win as any).testToggleCopilot()
        cy.log(`Manual toggle result: ${result}`)
      } else {
        cy.log('Test toggle function not available')
      }
    })
  })
})
