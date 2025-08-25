export default defineEventHandler(async (event) => {
  const query = getQuery(event)
  const { status = 'all', priority = '', limit = 20, search = '' } = query

  // Demo projects data
  const demoProjects = [
    {
      id: 'PROJ-001',
      title: 'Kitchen Renovation - Corner Café',
      description: 'Complete kitchen equipment upgrade and renovation project for Corner Café including new appliances, layout redesign, and equipment installation.',
      client: {
        id: 'CUST-001',
        name: 'Corner Café',
        contact: 'Sarah Johnson',
        email: 'sarah@cornercafe.co.za',
        phone: '+27 82 555 0123'
      },
      status: 'in_progress',
      priority: 'high',
      budget: {
        estimated: 125000.00,
        actual: 87500.00,
        remaining: 37500.00,
        currency: 'ZAR'
      },
      timeline: {
        startDate: '2024-08-15T08:00:00Z',
        estimatedEndDate: '2024-09-15T17:00:00Z',
        actualEndDate: null,
        daysRemaining: 21
      },
      progress: {
        percentage: 70,
        milestonesCompleted: 7,
        totalMilestones: 10,
        tasksCompleted: 28,
        totalTasks: 40
      },
      team: [
        {
          id: 'USER_001',
          name: 'Project Manager',
          role: 'Project Manager',
          allocation: 100,
          contact: '+27 11 123 4567'
        },
        {
          id: 'USER_002',
          name: 'Kitchen Designer',
          role: 'Design Lead',
          allocation: 75,
          contact: '+27 82 111 2222'
        },
        {
          id: 'USER_003',
          name: 'Installation Team',
          role: 'Installation',
          allocation: 80,
          contact: '+27 83 333 4444'
        }
      ],
      milestones: [
        {
          id: 'MILE-001',
          title: 'Design Approval',
          status: 'completed',
          dueDate: '2024-08-20T17:00:00Z',
          completedDate: '2024-08-19T15:30:00Z'
        },
        {
          id: 'MILE-002',
          title: 'Equipment Procurement',
          status: 'completed',
          dueDate: '2024-08-25T17:00:00Z',
          completedDate: '2024-08-24T14:00:00Z'
        },
        {
          id: 'MILE-003',
          title: 'Installation Phase 1',
          status: 'in_progress',
          dueDate: '2024-08-30T17:00:00Z',
          completedDate: null
        },
        {
          id: 'MILE-004',
          title: 'Final Testing & Handover',
          status: 'pending',
          dueDate: '2024-09-15T17:00:00Z',
          completedDate: null
        }
      ],
      resources: [
        {
          type: 'equipment',
          name: 'Commercial Oven',
          quantity: 1,
          cost: 25000.00,
          status: 'delivered'
        },
        {
          type: 'equipment',
          name: 'Industrial Refrigerator',
          quantity: 2,
          cost: 35000.00,
          status: 'installed'
        },
        {
          type: 'material',
          name: 'Stainless Steel Counters',
          quantity: 4,
          cost: 15000.00,
          status: 'pending'
        }
      ],
      risks: [
        {
          id: 'RISK-001',
          description: 'Potential delay in custom counter delivery',
          impact: 'medium',
          probability: 'low',
          mitigation: 'Alternative supplier identified as backup'
        }
      ],
      documents: [
        {
          name: 'Kitchen Design Plans.pdf',
          type: 'design',
          uploadedAt: '2024-08-18T10:00:00Z',
          size: '2.5MB'
        },
        {
          name: 'Equipment Specifications.xlsx',
          type: 'specifications',
          uploadedAt: '2024-08-20T14:30:00Z',
          size: '1.2MB'
        }
      ]
    },
    {
      id: 'PROJ-002',
      title: 'Inventory Management System Setup',
      description: 'Implementation of new inventory tracking system for Williams Construction including barcode scanning, automated reordering, and reporting dashboards.',
      client: {
        id: 'CUST-003',
        name: 'Williams Construction',
        contact: 'John Williams',
        email: 'john@williams-construction.co.za',
        phone: '+27 82 777 8888'
      },
      status: 'planning',
      priority: 'medium',
      budget: {
        estimated: 45000.00,
        actual: 5000.00,
        remaining: 40000.00,
        currency: 'ZAR'
      },
      timeline: {
        startDate: '2024-09-01T08:00:00Z',
        estimatedEndDate: '2024-10-15T17:00:00Z',
        actualEndDate: null,
        daysRemaining: 51
      },
      progress: {
        percentage: 15,
        milestonesCompleted: 1,
        totalMilestones: 6,
        tasksCompleted: 3,
        totalTasks: 20
      },
      team: [
        {
          id: 'USER_004',
          name: 'System Analyst',
          role: 'System Design',
          allocation: 60,
          contact: '+27 11 987 6543'
        },
        {
          id: 'USER_005',
          name: 'Implementation Specialist',
          role: 'Technical Lead',
          allocation: 80,
          contact: '+27 84 555 6666'
        }
      ],
      milestones: [
        {
          id: 'MILE-005',
          title: 'Requirements Gathering',
          status: 'completed',
          dueDate: '2024-08-30T17:00:00Z',
          completedDate: '2024-08-28T16:00:00Z'
        },
        {
          id: 'MILE-006',
          title: 'System Configuration',
          status: 'pending',
          dueDate: '2024-09-15T17:00:00Z',
          completedDate: null
        }
      ],
      resources: [
        {
          type: 'software',
          name: 'Inventory Management License',
          quantity: 1,
          cost: 15000.00,
          status: 'ordered'
        },
        {
          type: 'hardware',
          name: 'Barcode Scanners',
          quantity: 5,
          cost: 8000.00,
          status: 'pending'
        }
      ],
      risks: [
        {
          id: 'RISK-002',
          description: 'Integration complexity with existing systems',
          impact: 'high',
          probability: 'medium',
          mitigation: 'Detailed technical assessment planned'
        }
      ],
      documents: [
        {
          name: 'Requirements Document.docx',
          type: 'requirements',
          uploadedAt: '2024-08-25T11:00:00Z',
          size: '856KB'
        }
      ]
    },
    {
      id: 'PROJ-003',
      title: 'Group Buying Platform Integration',
      description: 'Development and integration of collaborative purchasing platform for local business network including vendor management and logistics coordination.',
      client: {
        id: 'CUST-INTERNAL',
        name: 'Internal Development',
        contact: 'Development Team',
        email: 'dev@tosserp.co.za',
        phone: '+27 11 000 0000'
      },
      status: 'completed',
      priority: 'high',
      budget: {
        estimated: 80000.00,
        actual: 75000.00,
        remaining: 5000.00,
        currency: 'ZAR'
      },
      timeline: {
        startDate: '2024-07-01T08:00:00Z',
        estimatedEndDate: '2024-08-15T17:00:00Z',
        actualEndDate: '2024-08-12T15:30:00Z',
        daysRemaining: 0
      },
      progress: {
        percentage: 100,
        milestonesCompleted: 8,
        totalMilestones: 8,
        tasksCompleted: 35,
        totalTasks: 35
      },
      team: [
        {
          id: 'USER_006',
          name: 'Lead Developer',
          role: 'Development Lead',
          allocation: 100,
          contact: '+27 82 444 5555'
        },
        {
          id: 'USER_007',
          name: 'UI/UX Designer',
          role: 'Design',
          allocation: 50,
          contact: '+27 83 666 7777'
        }
      ],
      milestones: [
        {
          id: 'MILE-007',
          title: 'Platform Architecture',
          status: 'completed',
          dueDate: '2024-07-15T17:00:00Z',
          completedDate: '2024-07-14T12:00:00Z'
        },
        {
          id: 'MILE-008',
          title: 'Core Features Development',
          status: 'completed',
          dueDate: '2024-08-01T17:00:00Z',
          completedDate: '2024-07-30T16:45:00Z'
        },
        {
          id: 'MILE-009',
          title: 'Testing & Deployment',
          status: 'completed',
          dueDate: '2024-08-15T17:00:00Z',
          completedDate: '2024-08-12T15:30:00Z'
        }
      ],
      resources: [
        {
          type: 'development',
          name: 'Cloud Infrastructure',
          quantity: 1,
          cost: 12000.00,
          status: 'active'
        },
        {
          type: 'software',
          name: 'Development Tools',
          quantity: 1,
          cost: 8000.00,
          status: 'active'
        }
      ],
      risks: [],
      documents: [
        {
          name: 'Technical Specification.pdf',
          type: 'technical',
          uploadedAt: '2024-07-10T09:00:00Z',
          size: '3.2MB'
        },
        {
          name: 'Final Report.pdf',
          type: 'report',
          uploadedAt: '2024-08-12T16:00:00Z',
          size: '1.8MB'
        }
      ]
    }
  ]

  // Filter by status
  let filteredProjects = demoProjects
  if (status !== 'all') {
    filteredProjects = filteredProjects.filter(proj => proj.status === status)
  }

  // Filter by priority
  if (priority) {
    filteredProjects = filteredProjects.filter(proj => proj.priority === priority)
  }

  // Search filter
  if (search) {
    const searchLower = search.toLowerCase()
    filteredProjects = filteredProjects.filter(proj => 
      proj.title.toLowerCase().includes(searchLower) ||
      proj.description.toLowerCase().includes(searchLower) ||
      proj.client.name.toLowerCase().includes(searchLower)
    )
  }

  // Apply limit
  const limitedProjects = filteredProjects.slice(0, Number(limit))

  // Calculate summary statistics
  const stats = {
    total: filteredProjects.length,
    byStatus: {
      planning: demoProjects.filter(p => p.status === 'planning').length,
      in_progress: demoProjects.filter(p => p.status === 'in_progress').length,
      completed: demoProjects.filter(p => p.status === 'completed').length,
      on_hold: demoProjects.filter(p => p.status === 'on_hold').length
    },
    byPriority: {
      high: demoProjects.filter(p => p.priority === 'high').length,
      medium: demoProjects.filter(p => p.priority === 'medium').length,
      low: demoProjects.filter(p => p.priority === 'low').length
    },
    budget: {
      totalEstimated: demoProjects.reduce((sum, proj) => sum + proj.budget.estimated, 0),
      totalActual: demoProjects.reduce((sum, proj) => sum + proj.budget.actual, 0),
      totalRemaining: demoProjects.reduce((sum, proj) => sum + proj.budget.remaining, 0)
    },
    averageProgress: Math.round(demoProjects.reduce((sum, proj) => sum + proj.progress.percentage, 0) / demoProjects.length)
  }

  return {
    success: true,
    data: {
      projects: limitedProjects,
      stats,
      pagination: {
        total: filteredProjects.length,
        limit: Number(limit),
        page: 1
      }
    }
  }
})
