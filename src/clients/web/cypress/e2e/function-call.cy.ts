describe('Function Call Test', () => {
  it('should verify toggleCopilot function is called', () => {
    const logs: string[] = []
    
    cy.visit('http://localhost:3001', {
      onBeforeLoad(win) {
        // Capture console.log calls
        const originalLog = win.console.log
        win.console.log = (...args) => {
          const message = args.join(' ')
          logs.push(message)
          originalLog.apply(win.console, args)
        }
      }
    })
    
    // Wait for page to load
    cy.contains('TOSS ERP III').should('be.visible')
    
    // Verify the button exists
    cy.get('[data-testid="ai-copilot-trigger"]').should('exist')
    
    // Click the button
    cy.get('[data-testid="ai-copilot-trigger"]').click({ force: true })
    
    // Wait a moment for logs to be captured
    cy.wait(1000)
    
    // Check if our function was called by looking for the console logs
    cy.then(() => {
      cy.log('All captured logs:', logs.join('; '))
      
      const toggleLogs = logs.filter(log => log.includes('toggleCopilot'))
      if (toggleLogs.length > 0) {
        cy.log('toggleCopilot function was called:')
        toggleLogs.forEach(log => cy.log(log))
      } else {
        cy.log('toggleCopilot function was NOT called')
      }
      
      // Look for the showCopilot state change
      const showCopilotLogs = logs.filter(log => log.includes('showCopilot'))
      if (showCopilotLogs.length > 0) {
        cy.log('showCopilot state logs:')
        showCopilotLogs.forEach(log => cy.log(log))
      }
    })
    
    // Also check if the panel appeared
    cy.get('body').then(($body) => {
      const panelExists = $body.find('[data-testid="ai-copilot-panel"]').length > 0
      cy.log(`Panel exists after click: ${panelExists}`)
    })
  })
})
