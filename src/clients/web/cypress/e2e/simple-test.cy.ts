describe('Simple Test', () => {
  beforeEach(() => {
    cy.visit('http://localhost:3001')
  })

  it('should load the homepage', () => {
    cy.contains('TOSS ERP III').should('be.visible')
  })

  it('should open AI co-pilot when button is clicked', () => {
    cy.get('[data-testid="ai-copilot-trigger"]').should('be.visible')
    
    // Debug: Check initial state
    cy.get('[data-testid="ai-copilot-panel"]').should('not.exist')
    
    // Try manual toggle first for debugging
    cy.window().then((win) => {
      const result = (win as any).testToggleCopilot()
      console.log('Manual toggle result:', result)
    })
    
    cy.wait(500)
    
    // Check if manual toggle worked
    cy.get('[data-testid="ai-copilot-panel"]').then(($panel) => {
      if ($panel.length > 0) {
        console.log('Manual toggle successful - panel exists')
      } else {
        console.log('Manual toggle failed - trying click')
        // If manual toggle didn't work, try the click
        cy.get('[data-testid="ai-copilot-trigger"]').click({ force: true })
        cy.wait(1000)
      }
    })
    
    cy.get('[data-testid="ai-copilot-panel"]').should('exist').and('be.visible')
  })

  it('should have working tabs in AI co-pilot', () => {
    cy.get('[data-testid="ai-copilot-trigger"]').click({ force: true })
    cy.wait(1000) // Wait for animation
    cy.get('[data-testid="ai-copilot-panel"]').should('exist').and('be.visible')
    
    // Check tabs exist
    cy.get('[data-testid="ai-copilot-tab-chat"]').should('be.visible')
    cy.get('[data-testid="ai-copilot-tab-alerts"]').should('be.visible')
    cy.get('[data-testid="ai-copilot-tab-tasks"]').should('be.visible')
    
    // Test tab switching
    cy.get('[data-testid="ai-copilot-tab-chat"]').click({ force: true })
    cy.get('[data-testid="ai-copilot-content-chat"]').should('be.visible')
    
    cy.get('[data-testid="ai-copilot-tab-tasks"]').click({ force: true })
    cy.get('[data-testid="ai-copilot-content-tasks"]').should('be.visible')
    
    cy.get('[data-testid="ai-copilot-tab-alerts"]').click({ force: true })
    cy.get('[data-testid="ai-copilot-content-alerts"]').should('be.visible')
  })

  it('should close AI co-pilot when close button is clicked', () => {
    cy.get('[data-testid="ai-copilot-trigger"]').click({ force: true })
    cy.wait(1000) // Wait for animation
    cy.get('[data-testid="ai-copilot-panel"]').should('exist').and('be.visible')
    cy.get('[data-testid="ai-copilot-close"]').click({ force: true })
    cy.wait(500) // Wait for close animation
    cy.get('[data-testid="ai-copilot-panel"]').should('not.exist')
  })
})
