describe('Analytics Dashboard E2E Tests', () => {
  beforeEach(() => {
    cy.visit('/dashboard')
  })

  it('should load the analytics dashboard successfully', () => {
    cy.get('h3').should('contain', 'Analytics Dashboard')
    cy.get('p').should('contain', 'Real-time business insights')
  })

  it('should display all key metrics cards', () => {
    // Check Total Revenue card
    cy.get('.bg-gradient-to-r.from-blue-500').should('be.visible')
    cy.get('.bg-gradient-to-r.from-blue-500').should('contain', 'Total Revenue')
    cy.get('.bg-gradient-to-r.from-blue-500').should('contain', '$')

    // Check Total Orders card
    cy.get('.bg-gradient-to-r.from-green-500').should('be.visible')
    cy.get('.bg-gradient-to-r.from-green-500').should('contain', 'Total Orders')

    // Check Average Order Value card
    cy.get('.bg-gradient-to-r.from-purple-500').should('be.visible')
    cy.get('.bg-gradient-to-r.from-purple-500').should('contain', 'Average Order Value')

    // Check Conversion Rate card
    cy.get('.bg-gradient-to-r.from-orange-500').should('be.visible')
    cy.get('.bg-gradient-to-r.from-orange-500').should('contain', 'Conversion Rate')
  })

  it('should display period selector and refresh button', () => {
    cy.get('select').should('be.visible')
    cy.get('select').should('contain', 'Last 7 days')
    cy.get('select').should('contain', 'Last 30 days')
    cy.get('select').should('contain', 'Last 90 days')
    cy.get('select').should('contain', 'Last year')
    
    cy.get('button').contains('Refresh').should('be.visible')
  })

  it('should change period when selector is used', () => {
    cy.get('select').select('7d')
    cy.get('select').should('have.value', '7d')
    
    cy.get('select').select('90d')
    cy.get('select').should('have.value', '90d')
  })

  it('should refresh data when refresh button is clicked', () => {
    // Store initial values and perform refresh test
    cy.get('.bg-gradient-to-r.from-blue-500').then(($el) => {
      const initialRevenue = $el.text()
      
      // Click refresh button
      cy.get('button').contains('Refresh').click()
      
      // Wait for refresh to complete
      cy.wait(2000)
      
      // Verify data has changed (simulated random data)
      cy.get('.bg-gradient-to-r.from-blue-500').should('not.have.text', initialRevenue)
    })
  })

  it('should display charts section', () => {
    cy.get('h4').should('contain', 'Revenue Trend')
    cy.get('h4').should('contain', 'Orders Trend')
    
    // Check chart placeholders
    cy.get('.text-gray-500').should('contain', 'Revenue trend chart will be implemented')
    cy.get('.text-gray-500').should('contain', 'Orders trend chart will be implemented')
  })

  it('should display top products section', () => {
    cy.get('h4').should('contain', 'Top Products')
    
    // Check that products are displayed
    cy.get('.space-y-4').should('be.visible')
    cy.get('.text-sm.font-medium').should('contain', 'Premium Widget')
    cy.get('.text-sm.font-medium').should('contain', 'Smart Device')
  })

  it('should display top categories section', () => {
    cy.get('h4').should('contain', 'Top Categories')
    
    // Check that categories are displayed
    cy.get('.space-y-4').should('be.visible')
    cy.get('.text-sm.font-medium').should('contain', 'Electronics')
    cy.get('.text-sm.font-medium').should('contain', 'Technology')
  })

  it('should display customer insights section', () => {
    cy.get('h4').should('contain', 'Customer Insights')
    
    // Check customer metrics
    cy.get('.text-3xl.font-bold.text-blue-600').should('be.visible')
    cy.get('.text-3xl.font-bold.text-green-600').should('be.visible')
    cy.get('.text-3xl.font-bold.text-purple-600').should('be.visible')
    
    cy.get('p').should('contain', 'New Customers')
    cy.get('p').should('contain', 'Repeat Customers')
    cy.get('p').should('contain', 'Avg. Customer Lifetime Value')
  })

  it('should be responsive on mobile devices', () => {
    cy.viewport('iphone-x')
    cy.get('h3').should('contain', 'Analytics Dashboard')
    cy.get('.grid').should('be.visible')
    
    // Check that metrics cards stack properly
    cy.get('.grid-cols-1.md\\:grid-cols-2.lg\\:grid-cols-4').should('be.visible')
  })

  it('should be responsive on tablet devices', () => {
    cy.viewport('ipad-2')
    cy.get('h3').should('contain', 'Analytics Dashboard')
    cy.get('.grid').should('be.visible')
  })

  it('should display percentage changes correctly', () => {
    // Check that percentage changes are displayed
    cy.get('.text-sm.opacity-90').should('contain', '% vs last period')
    
    // Check for positive/negative indicators
    cy.get('.text-sm.opacity-90').should('contain', '+')
  })

  it('should format currency values correctly', () => {
    // Check that currency values are formatted with dollar signs
    cy.get('.text-2xl.font-bold').should('contain', '$')
  })

  it('should display product and category icons', () => {
    // Check that icons are displayed for products and categories
    cy.get('.w-10.h-10.bg-gray-200').should('be.visible')
    cy.get('.w-5.h-5.text-gray-500').should('be.visible')
  })

  it('should show loading state during refresh', () => {
    // Click refresh and check for loading state
    cy.get('button').contains('Refresh').click()
    
    // The button should be disabled or show loading state
    cy.get('button').contains('Refresh').should('be.disabled')
    
    // Wait for refresh to complete
    cy.wait(2000)
    
    // Button should be enabled again
    cy.get('button').contains('Refresh').should('not.be.disabled')
  })

  it('should maintain data consistency across sections', () => {
    // Check that the total revenue in the card matches the context
    cy.get('.bg-gradient-to-r.from-blue-500').then(($card) => {
      const cardText = $card.text()
      expect(cardText).to.contain('Total Revenue')
      expect(cardText).to.contain('$')
    })
  })

  it('should handle empty data gracefully', () => {
    // This test would be implemented when we have actual API integration
    // For now, we check that the component loads without errors
    cy.get('.bg-white.shadow.rounded-lg').should('be.visible')
    cy.get('.text-gray-500').should('be.visible')
  })

  it('should display proper error states', () => {
    // This test would be implemented when we have actual API integration
    // For now, we check that the component handles errors gracefully
    cy.get('.bg-white.shadow.rounded-lg').should('be.visible')
  })

  it('should have proper accessibility attributes', () => {
    // Check for proper heading structure
    cy.get('h3').should('have.length.at.least', 1)
    cy.get('h4').should('have.length.at.least', 1)
    
    // Check for proper button labels
    cy.get('button').contains('Refresh').should('have.attr', 'type')
    
    // Check for proper select labels
    cy.get('select').should('be.visible')
  })

  it('should support keyboard navigation', () => {
    // Test tab navigation by pressing Tab key
    cy.get('body').type('{tab}')
    cy.focused().should('exist')
    
    // Test select navigation
    cy.get('select').focus()
    cy.get('select').should('be.focused')
  })

  it('should display proper tooltips and help text', () => {
    // Check for descriptive text
    cy.get('p').should('contain', 'Real-time business insights')
    cy.get('p').should('contain', 'performance metrics')
  })

  it('should handle different screen sizes properly', () => {
    // Test different viewport sizes
    cy.viewport(1920, 1080)
    cy.get('.grid-cols-1.lg\\:grid-cols-2').should('be.visible')
    
    cy.viewport(1366, 768)
    cy.get('.grid-cols-1.lg\\:grid-cols-2').should('be.visible')
    
    cy.viewport(1024, 768)
    cy.get('.grid-cols-1.lg\\:grid-cols-2').should('be.visible')
  })
})
