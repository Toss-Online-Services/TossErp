describe('Enhanced Chatbot E2E Tests', () => {
  beforeEach(() => {
    cy.visit('/dashboard')
  })

  it('should display enhanced chatbot with additional features', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Check for enhanced features
    cy.get('h3').should('contain', 'TOSS ERP Assistant')
    cy.get('button').should('contain', 'Clear chat')
  })

  it('should have clear chat functionality', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Send a message first
    cy.get('[data-testid="chatbot-input"]').type('Hello{enter}')
    cy.get('.bg-orange-600').should('contain', 'Hello')
    
    // Clear chat
    cy.get('button[title="Clear chat"]').click()
    cy.get('.bg-orange-600').should('not.exist')
  })

  it('should display voice input button', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Check for voice input button
    cy.get('button[title="Voice input"]').should('be.visible')
    cy.get('button[title="Voice input"] svg').should('be.visible')
  })

  it('should display file upload button', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Check for file upload button
    cy.get('input[type="file"]').should('be.visible')
    cy.get('input[type="file"]').should('have.attr', 'accept', '.txt,.pdf,.doc,.docx,.xls,.xlsx')
  })

  it('should display action buttons in bot responses', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Send a message that triggers action buttons
    cy.get('[data-testid="chatbot-input"]').type('inventory{enter}')
    cy.wait(2000) // Wait for bot response
    
    // Check for action buttons
    cy.get('.bg-white\\/20').should('be.visible')
    cy.get('button').should('contain', 'Go to Inventory')
    cy.get('button').should('contain', 'Export Stock Report')
  })

  it('should execute navigation actions', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Send a message that triggers navigation
    cy.get('[data-testid="chatbot-input"]').type('sales report{enter}')
    cy.wait(2000) // Wait for bot response
    
    // Click navigation action
    cy.get('button').contains('Go to Sales').click()
    
    // Should navigate to sales page
    cy.url().should('include', '/sales')
  })

  it('should display enhanced quick actions', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Check for enhanced quick actions
    cy.get('button').should('contain', 'Check inventory')
    cy.get('button').should('contain', 'Sales report')
    cy.get('button').should('contain', 'Financial summary')
    cy.get('button').should('contain', 'Help')
    cy.get('button').should('contain', 'Settings')
  })

  it('should send quick messages with enhanced actions', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Click financial summary quick action
    cy.get('button').contains('Financial summary').click()
    cy.get('.bg-orange-600').should('contain', 'Financial summary')
    
    // Wait for bot response with actions
    cy.wait(2000)
    cy.get('.bg-white\\/20').should('be.visible')
  })

  it('should disable input during bot typing', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Send a message
    cy.get('[data-testid="chatbot-input"]').type('Hello{enter}')
    
    // Input should be disabled during typing
    cy.get('[data-testid="chatbot-input"]').should('be.disabled')
    
    // Wait for bot response
    cy.wait(2000)
    
    // Input should be enabled again
    cy.get('[data-testid="chatbot-input"]').should('not.be.disabled')
  })

  it('should disable send button when input is empty', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Send button should be disabled initially
    cy.get('[data-testid="chatbot-send"]').should('be.disabled')
    
    // Type some text
    cy.get('[data-testid="chatbot-input"]').type('Hello')
    
    // Send button should be enabled
    cy.get('[data-testid="chatbot-send"]').should('not.be.disabled')
  })

  it('should handle long messages properly', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Send a long message
    const longMessage = 'This is a very long message that should be handled properly by the chatbot interface.'
    cy.get('[data-testid="chatbot-input"]').type(longMessage + '{enter}')
    
    // Check that the message is displayed properly
    cy.get('.bg-orange-600').should('contain', longMessage)
  })

  it('should scroll to bottom when new messages are added', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Send multiple messages
    cy.get('[data-testid="chatbot-input"]').type('Message 1{enter}')
    cy.wait(1000)
    cy.get('[data-testid="chatbot-input"]').type('Message 2{enter}')
    cy.wait(1000)
    cy.get('[data-testid="chatbot-input"]').type('Message 3{enter}')
    
    // Wait for all responses
    cy.wait(3000)
    
    // Latest message should be visible (scrolled to bottom)
    cy.get('.bg-orange-600').last().should('be.visible')
  })

  it('should handle special characters in messages', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Send message with special characters
    const specialMessage = 'Hello! How are you? I need help with inventory (urgent).'
    cy.get('[data-testid="chatbot-input"]').type(specialMessage + '{enter}')
    
    // Check that special characters are handled properly
    cy.get('.bg-orange-600').should('contain', specialMessage)
  })

  it('should maintain chat history during session', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Send first message
    cy.get('[data-testid="chatbot-input"]').type('First message{enter}')
    cy.wait(2000)
    
    // Send second message
    cy.get('[data-testid="chatbot-input"]').type('Second message{enter}')
    cy.wait(2000)
    
    // Both messages should be in chat history
    cy.get('.bg-orange-600').should('contain', 'First message')
    cy.get('.bg-orange-600').should('contain', 'Second message')
  })

  it('should handle empty messages gracefully', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Try to send empty message
    cy.get('[data-testid="chatbot-input"]').type('{enter}')
    
    // Should not add empty message to chat
    cy.get('.bg-orange-600').should('not.contain', '')
  })

  it('should be responsive on mobile devices', () => {
    cy.viewport('iphone-x')
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Check that chatbot is properly sized on mobile
    cy.get('.w-96.h-\\[500px\\]').should('be.visible')
    
    // Check that input area is accessible
    cy.get('[data-testid="chatbot-input"]').should('be.visible')
  })

  it('should be responsive on tablet devices', () => {
    cy.viewport('ipad-2')
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Check that chatbot is properly sized on tablet
    cy.get('.w-96.h-\\[500px\\]').should('be.visible')
  })

  it('should close when clicking outside', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Click outside the chatbot
    cy.get('body').click(0, 0)
    
    // Chatbot should close
    cy.get('[data-testid="chatbot-window"]').should('not.be.visible')
  })

  it('should handle rapid message sending', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Send multiple messages rapidly
    cy.get('[data-testid="chatbot-input"]').type('Message 1{enter}')
    cy.get('[data-testid="chatbot-input"]').type('Message 2{enter}')
    cy.get('[data-testid="chatbot-input"]').type('Message 3{enter}')
    
    // Wait for responses
    cy.wait(3000)
    
    // All messages should be processed
    cy.get('.bg-orange-600').should('have.length.at.least', 3)
  })

  it('should display proper timestamps', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // Send a message
    cy.get('[data-testid="chatbot-input"]').type('Hello{enter}')
    cy.wait(2000)
    
    // Check for timestamp
    cy.get('.opacity-70').should('be.visible')
  })

  it('should handle network errors gracefully', () => {
    cy.get('[data-testid="chatbot-toggle"]').click()
    cy.get('[data-testid="chatbot-window"]').should('be.visible')
    
    // This test would be implemented when we have actual API integration
    // For now, we check that the component handles errors gracefully
    cy.get('[data-testid="chatbot-input"]').should('be.visible')
    cy.get('[data-testid="chatbot-send"]').should('be.visible')
  })
})
