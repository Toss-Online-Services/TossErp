describe('Chatbot E2E Tests', () => {
  beforeEach(() => {
    cy.visit('/dashboard')
  })

  it('should display chatbot toggle button', () => {
    cy.get('.fixed.bottom-4.right-4 button').should('be.visible')
    cy.get('.fixed.bottom-4.right-4 button svg').should('be.visible')
  })

  it('should open chatbot when toggle button is clicked', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('.w-96.h-96').should('be.visible')
    cy.get('h3').should('contain', 'TOSS ERP Assistant')
  })

  it('should display welcome message when chatbot is opened', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('p').should('contain', "Hello! I'm your TOSS ERP assistant")
    cy.get('p').should('contain', 'Ask me about inventory, sales, or any business questions')
  })

  it('should display quick action buttons', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('button').should('contain', 'Check inventory')
    cy.get('button').should('contain', 'Sales report')
    cy.get('button').should('contain', 'Help')
    cy.get('button').should('contain', 'Settings')
  })

  it('should send message when user types and presses enter', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('input[placeholder="Type your message..."]').type('Hello{enter}')
    cy.get('.bg-orange-600').should('contain', 'Hello')
  })

  it('should send message when user clicks send button', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('input[placeholder="Type your message..."]').type('Hello')
    cy.get('button svg').last().click()
    cy.get('.bg-orange-600').should('contain', 'Hello')
  })

  it('should display typing indicator when bot is responding', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('input[placeholder="Type your message..."]').type('Hello{enter}')
    cy.get('.animate-bounce').should('be.visible')
  })

  it('should generate bot response for inventory query', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('input[placeholder="Type your message..."]').type('inventory{enter}')
    cy.wait(2000) // Wait for bot response
    cy.get('.bg-gray-100').should('contain', 'inventory management')
  })

  it('should generate bot response for sales query', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('input[placeholder="Type your message..."]').type('sales report{enter}')
    cy.wait(2000) // Wait for bot response
    cy.get('.bg-gray-100').should('contain', 'sales overview')
  })

  it('should generate bot response for help query', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('input[placeholder="Type your message..."]').type('help{enter}')
    cy.wait(2000) // Wait for bot response
    cy.get('.bg-gray-100').should('contain', "I'm here to help")
  })

  it('should send quick message when quick action is clicked', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('button').contains('Check inventory').click()
    cy.get('.bg-orange-600').should('contain', 'Check inventory')
  })

  it('should close chatbot when close button is clicked', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('.w-96.h-96').should('be.visible')
    cy.get('button svg').eq(1).click() // Close button
    cy.get('.w-96.h-96').should('not.be.visible')
  })

  it('should close chatbot when toggle button is clicked again', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('.w-96.h-96').should('be.visible')
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('.w-96.h-96').should('not.be.visible')
  })

  it('should display message timestamps', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('input[placeholder="Type your message..."]').type('Hello{enter}')
    cy.wait(2000) // Wait for bot response
    cy.get('.opacity-70').should('be.visible')
  })

  it('should scroll to bottom when new messages are added', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('input[placeholder="Type your message..."]').type('Hello{enter}')
    cy.wait(2000) // Wait for bot response
    // Verify that the latest message is visible (scrolled to bottom)
    cy.get('.bg-gray-100').last().should('be.visible')
  })

  it('should disable input when bot is typing', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('input[placeholder="Type your message..."]').type('Hello{enter}')
    cy.get('input[placeholder="Type your message..."]').should('be.disabled')
    cy.wait(2000) // Wait for bot response
    cy.get('input[placeholder="Type your message..."]').should('not.be.disabled')
  })

  it('should disable send button when input is empty', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('button svg').last().should('have.class', 'disabled:opacity-50')
  })

  it('should enable send button when input has content', () => {
    cy.get('.fixed.bottom-4.right-4 button').click()
    cy.get('input[placeholder="Type your message..."]').type('Hello')
    cy.get('button svg').last().should('not.have.class', 'disabled:opacity-50')
  })
})
