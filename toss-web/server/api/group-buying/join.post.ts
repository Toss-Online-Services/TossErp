export default defineEventHandler(async (event) => {
  const method = getMethod(event)
  
  if (method !== 'POST') {
    throw createError({
      statusCode: 405,
      statusMessage: 'Method not allowed'
    })
  }

  const body = await readBody(event)
  const { groupPurchaseId, quantity, paymentMethod = 'bank_transfer', notes = '' } = body

  // Validate required fields
  if (!groupPurchaseId || !quantity) {
    throw createError({
      statusCode: 400,
      statusMessage: 'groupPurchaseId and quantity are required'
    })
  }

  if (quantity <= 0) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Quantity must be greater than 0'
    })
  }

  // Demo validation - check if group purchase exists and is active
  const validGroupPurchases = ['gp_flour_001', 'gp_sugar_002', 'gp_oil_003', 'gp_packaging_004']
  if (!validGroupPurchases.includes(groupPurchaseId)) {
    throw createError({
      statusCode: 404,
      statusMessage: 'Group purchase not found'
    })
  }

  // Demo business logic - simulate joining
  const participantId = `participant_${Date.now()}`
  const commitment = {
    id: participantId,
    groupPurchaseId,
    participantName: 'Your Business', // Would be from auth context
    quantity,
    paymentMethod,
    notes,
    status: 'committed',
    commitmentDate: new Date().toISOString(),
    totalCost: quantity * 370, // Demo calculation based on flour example
    paymentStatus: 'pending',
    deliveryPreference: 'pickup' // Default
  }

  // Calculate updated group progress (demo)
  const updatedProgress = {
    currentQuantity: 76 + quantity, // Previous + new commitment
    currentParticipants: 13, // Previous + 1
    progressPercentage: Math.min(100, ((76 + quantity) / 100) * 100) // Based on minimum of 100
  }

  return {
    success: true,
    data: {
      commitment,
      message: `Successfully joined group purchase with ${quantity} units`,
      updatedProgress,
      nextSteps: [
        'You will receive payment instructions within 24 hours',
        'Payment deadline is shown in the group purchase details',
        'Delivery/pickup details will be shared once the minimum is reached'
      ],
      estimatedSavings: quantity * (450 - 370), // Demo savings calculation
      totalCommitment: commitment.totalCost
    }
  }
})
