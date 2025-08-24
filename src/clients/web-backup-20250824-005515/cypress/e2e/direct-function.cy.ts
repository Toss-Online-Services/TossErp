describe('Direct Function Test', () => {
  it('should test AI copilot using direct function call', () => {
    cy.visit('http://localhost:3001')
    
    // Wait for page to load
    cy.contains('TOSS ERP III').should('be.visible')
    
    // Check initial state - panel should not exist
    cy.get('[data-testid="ai-copilot-panel"]').should('not.exist')
    
    // Test using the direct function call
    cy.window().then((win) => {
      // Check if test function is available
      if ((win as any).testToggleCopilot) {
        cy.log('testToggleCopilot function is available')
        
        // Call the function directly
        const result = (win as any).testToggleCopilot()
        cy.log(`Direct function call result: ${result}`)
        
        // Wait for Vue reactivity
        cy.wait(1000)
        
        // Check if panel appeared
        cy.get('body').then(($body) => {
          const panelExists = $body.find('[data-testid="ai-copilot-panel"]').length > 0
          cy.log(`Panel exists after direct function call: ${panelExists}`)
          
          if (panelExists) {
            cy.get('[data-testid="ai-copilot-panel"]').should('be.visible')
            cy.log('SUCCESS: Direct function call worked!')
          } else {
            cy.log('PROBLEM: Direct function call did not show panel')
            
            // Check the state function
            if ((win as any).getCopilotState) {
              const state = (win as any).getCopilotState()
              cy.log(`Copilot state: ${JSON.stringify(state)}`)
            }
          }
        })
      } else {
        cy.log('testToggleCopilot function is NOT available')
      }
    })
    
    // Now test the actual button click
    cy.log('Testing actual button click...')
    cy.get('[data-testid="ai-copilot-trigger"]').should('be.visible')
    cy.get('[data-testid="ai-copilot-trigger"]').click({ force: true })
    
    cy.wait(1000)
    
    // Check if button click worked
    cy.get('body').then(($body) => {
      const panelExists = $body.find('[data-testid="ai-copilot-panel"]').length > 0
      cy.log(`Panel exists after button click: ${panelExists}`)
      
      if (panelExists) {
        cy.log('SUCCESS: Button click worked!')
      } else {
        cy.log('PROBLEM: Button click did not work')
      }
    })
  })
})
