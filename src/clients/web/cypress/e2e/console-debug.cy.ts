describe('Console Debug Test', () => {
  it('should check for console errors and hydration issues', () => {
    let consoleErrors: any[] = []
    let consoleWarns: any[] = []
    
    cy.visit('http://localhost:3001', {
      onBeforeLoad(win) {
        // Capture console errors and warnings
        const originalError = win.console.error
        const originalWarn = win.console.warn
        
        win.console.error = (...args) => {
          consoleErrors.push(args)
          originalError.apply(win.console, args)
        }
        
        win.console.warn = (...args) => {
          consoleWarns.push(args)
          originalWarn.apply(win.console, args)
        }
      }
    })
    
    // Wait for page to fully load
    cy.wait(3000)
    
    // Check for title to ensure basic load
    cy.contains('TOSS ERP III').should('be.visible')
    
    // Log console errors
    cy.then(() => {
      if (consoleErrors.length > 0) {
        cy.log('Console errors found:')
        consoleErrors.forEach((args, index) => {
          cy.log(`Error ${index + 1}: ${args.join(' ')}`)
        })
      } else {
        cy.log('No console errors found')
      }
      
      if (consoleWarns.length > 0) {
        cy.log('Console warnings found:')
        consoleWarns.forEach((args, index) => {
          cy.log(`Warning ${index + 1}: ${args.join(' ')}`)
        })
      } else {
        cy.log('No console warnings found')
      }
    })
    
    // Check the full page HTML to see if there are error overlays
    cy.get('body').then(($body) => {
      const bodyHtml = $body.html()
      
      // Look for Nuxt error overlays
      if (bodyHtml.includes('nuxt-error-page') || bodyHtml.includes('__nuxt_error')) {
        cy.log('Nuxt error page detected in body')
      }
      
      // Look for Vue dev overlay
      if (bodyHtml.includes('nuxt-dev-error') || bodyHtml.includes('vue-dev-error')) {
        cy.log('Vue dev error overlay detected')
      }
      
      // Look for frame elements
      if (bodyHtml.includes('<pre class="frame">')) {
        cy.log('Frame element found - likely error stack trace')
        // Extract and log the frame content
        const frameMatch = bodyHtml.match(/<pre class="frame"[^>]*>(.*?)<\/pre>/s)
        if (frameMatch) {
          cy.log('Frame content:', frameMatch[1])
        }
      }
      
      // Check if the AI copilot trigger button exists in DOM
      if (bodyHtml.includes('data-testid="ai-copilot-trigger"')) {
        cy.log('AI copilot trigger button found in DOM')
      } else {
        cy.log('AI copilot trigger button NOT found in DOM')
      }
    })
  })
})
