export default defineEventHandler(async (event) => {
  const method = getMethod(event)
  
  if (method !== 'POST') {
    throw createError({
      statusCode: 405,
      statusMessage: 'Method not allowed'
    })
  }

  const body = await readBody(event)
  const { 
    title, 
    description, 
    clientId, 
    priority = 'medium', 
    estimatedBudget, 
    startDate, 
    estimatedEndDate,
    teamMembers = [],
    milestones = []
  } = body

  // Validate required fields
  if (!title || !description || !clientId || !estimatedBudget || !startDate || !estimatedEndDate) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Title, description, clientId, estimatedBudget, startDate, and estimatedEndDate are required'
    })
  }

  // Validate budget
  if (estimatedBudget <= 0) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Estimated budget must be greater than 0'
    })
  }

  // Validate dates
  const start = new Date(startDate)
  const end = new Date(estimatedEndDate)
  if (start >= end) {
    throw createError({
      statusCode: 400,
      statusMessage: 'End date must be after start date'
    })
  }

  // Generate project ID
  const projectId = `PROJ-${Date.now()}`

  // Demo client lookup (in real app, validate client exists)
  const demoClients = {
    'CUST-001': { name: 'Corner Café', contact: 'Sarah Johnson', email: 'sarah@cornercafe.co.za', phone: '+27 82 555 0123' },
    'CUST-002': { name: 'Local Restaurant Co.', contact: 'Restaurant Manager', email: 'manager@localrestaurant.co.za', phone: '+27 82 999 1111' },
    'CUST-003': { name: 'Williams Construction', contact: 'John Williams', email: 'john@williams-construction.co.za', phone: '+27 82 777 8888' }
  }

  const client = demoClients[clientId]
  if (!client) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Invalid client ID'
    })
  }

  // Calculate timeline
  const timelineStart = new Date(startDate)
  const timelineEnd = new Date(estimatedEndDate)
  const daysTotal = Math.ceil((timelineEnd.getTime() - timelineStart.getTime()) / (1000 * 60 * 60 * 24))
  const daysRemaining = Math.max(0, Math.ceil((timelineEnd.getTime() - new Date().getTime()) / (1000 * 60 * 60 * 24)))

  // Create project structure
  const newProject = {
    id: projectId,
    title,
    description,
    client: {
      id: clientId,
      ...client
    },
    status: 'planning',
    priority,
    budget: {
      estimated: estimatedBudget,
      actual: 0,
      remaining: estimatedBudget,
      currency: 'ZAR'
    },
    timeline: {
      startDate,
      estimatedEndDate,
      actualEndDate: null,
      daysTotal,
      daysRemaining
    },
    progress: {
      percentage: 0,
      milestonesCompleted: 0,
      totalMilestones: milestones.length,
      tasksCompleted: 0,
      totalTasks: 0
    },
    team: teamMembers.map(member => ({
      id: member.id || `USER_${Date.now()}_${Math.random()}`,
      name: member.name,
      role: member.role,
      allocation: member.allocation || 100,
      contact: member.contact || ''
    })),
    milestones: milestones.map((milestone, index) => ({
      id: `MILE_${projectId}_${index + 1}`,
      title: milestone.title,
      description: milestone.description || '',
      status: 'pending',
      dueDate: milestone.dueDate,
      completedDate: null,
      dependencies: milestone.dependencies || []
    })),
    resources: [],
    risks: [],
    documents: [],
    createdAt: new Date().toISOString(),
    createdBy: 'Current User', // Would be from auth context
    lastUpdated: new Date().toISOString()
  }

  return {
    success: true,
    data: {
      project: newProject,
      message: `Project "${title}" created successfully`,
      nextSteps: [
        'Add team members if not already specified',
        'Define detailed milestones and tasks',
        'Upload relevant documents',
        'Set up project tracking and reporting'
      ]
    }
  }
})
