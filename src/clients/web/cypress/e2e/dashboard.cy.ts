describe('Dashboard E2E Tests', () => {
  beforeEach(() => {
    cy.visit('/dashboard')
  })

  it('should load the dashboard page successfully', () => {
    cy.get('h2').should('contain', 'General Dashboard')
    cy.get('p').should('contain', 'Real-time business analytics')
  })

  it('should display all summary cards with correct data', () => {
    // Check Today's Money card
    cy.get('[data-testid="today-money"]').should('be.visible')
    cy.get('[data-testid="today-money"]').should('contain', '$')

    // Check Today's Users card
    cy.get('[data-testid="today-users"]').should('be.visible')
    cy.get('[data-testid="today-users"]').should('contain', '2,300')

    // Check New Clients card
    cy.get('[data-testid="new-clients"]').should('be.visible')
    cy.get('[data-testid="new-clients"]').should('contain', '+3,462')

    // Check Sales card
    cy.get('[data-testid="sales"]').should('be.visible')
    cy.get('[data-testid="sales"]').should('contain', '$103,430')
  })

  it('should display sales by country table', () => {
    cy.get('h3').should('contain', 'Sales by Country')
    cy.get('table').should('be.visible')
    cy.get('table tbody tr').should('have.length', 4)
    
    // Check specific countries
    cy.get('table tbody tr').first().should('contain', 'United States')
    cy.get('table tbody tr').eq(1).should('contain', 'Germany')
    cy.get('table tbody tr').eq(2).should('contain', 'Great Britain')
    cy.get('table tbody tr').eq(3).should('contain', 'Brasil')
  })

  it('should display global sales card', () => {
    cy.get('h3').should('contain', 'Global Sales')
    cy.get('h3').should('contain', 'Check the global stats')
    
    // Check key metrics
    cy.get('h3').should('contain', '$103,430')
    cy.get('h3').should('contain', '24,500')
    cy.get('h3').should('contain', 'Active Users')
  })

  it('should refresh data when refresh button is clicked', () => {
    // Store initial values
    let initialMoney: string
    cy.get('[data-testid="today-money"]').then(($el) => {
      initialMoney = $el.text()
    })

    // Click refresh button
    cy.get('button').contains('Refresh').click()
    
    // Wait for refresh to complete
    cy.wait(2000)
    
    // Verify data has changed (simulated random data)
    cy.get('[data-testid="today-money"]').should('not.have.text', initialMoney)
  })

  it('should display bottom stats row', () => {
    cy.get('[data-testid="users-stat"]').should('be.visible')
    cy.get('[data-testid="clicks-stat"]').should('be.visible')
    cy.get('[data-testid="sales-stat"]').should('be.visible')
    cy.get('[data-testid="items-stat"]').should('be.visible')
  })

  it('should display sales overview section', () => {
    cy.get('h3').should('contain', 'Sales overview')
    cy.get('p').should('contain', '% more in')
  })

  it('should be responsive on mobile devices', () => {
    cy.viewport('iphone-x')
    cy.get('h2').should('contain', 'General Dashboard')
    cy.get('.grid').should('be.visible')
  })

  it('should be responsive on tablet devices', () => {
    cy.viewport('ipad-2')
    cy.get('h2').should('contain', 'General Dashboard')
    cy.get('.grid').should('be.visible')
  })
})
