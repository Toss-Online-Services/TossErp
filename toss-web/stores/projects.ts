import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export type ProjectStatus = 'New' | 'InProgress' | 'OnHold' | 'Completed' | 'Closed' | 'Cancelled'
export type TaskStatus = 'ToDo' | 'InProgress' | 'Done' | 'Cancelled'

export interface Project {
  id: number
  title: string
  description?: string
  status: ProjectStatus
  customerId?: number
  customerName?: string
  shopId?: number
  shopName?: string
  startDate?: Date
  expectedCompletionDate?: Date
  completedDate?: Date
  salesInvoiceId?: number
  totalCost: number
  totalRevenue?: number
  createdAt: Date
  updatedAt: Date
  tasks?: ProjectTask[]
  materials?: ProjectMaterial[]
  labourEntries?: LabourEntry[]
}

export interface ProjectTask {
  id: number
  projectId: number
  title: string
  description?: string
  status: TaskStatus
  assigneeId?: string
  assigneeName?: string
  dueDate?: Date
  completedDate?: Date
  estimatedHours?: number
  actualHours?: number
  createdAt: Date
  updatedAt: Date
}

export interface ProjectMaterial {
  id: number
  projectId: number
  itemId: number
  itemName: string
  quantity: number
  unit: string
  unitPrice: number
  totalCost: number
  notes?: string
  createdAt: Date
}

export interface LabourEntry {
  id: number
  projectId: number
  projectTaskId?: number
  userId?: string
  userName?: string
  workDate: Date
  hours: number
  rate: number
  totalCost: number
  description?: string
  createdAt: Date
}

export const useProjectsStore = defineStore('projects', () => {
  // State
  const projects = ref<Project[]>([])
  const tasks = ref<ProjectTask[]>([])
  const labourEntries = ref<LabourEntry[]>([])
  const loading = ref(false)
  const selectedProject = ref<Project | null>(null)

  // Computed
  const projectsByStatus = computed(() => {
    const grouped: Record<ProjectStatus, Project[]> = {
      New: [],
      InProgress: [],
      OnHold: [],
      Completed: [],
      Closed: [],
      Cancelled: []
    }
    projects.value.forEach(project => {
      if (grouped[project.status]) {
        grouped[project.status].push(project)
      }
    })
    return grouped
  })

  const activeProjects = computed(() => {
    return projects.value.filter(p => 
      p.status === 'New' || p.status === 'InProgress' || p.status === 'OnHold'
    )
  })

  const completedProjects = computed(() => {
    return projects.value.filter(p => p.status === 'Completed' || p.status === 'Closed')
  })

  const overdueProjects = computed(() => {
    const now = new Date()
    return projects.value.filter(p => {
      if (!p.expectedCompletionDate || p.status === 'Completed' || p.status === 'Closed') {
        return false
      }
      return new Date(p.expectedCompletionDate) < now
    })
  })

  const tasksByStatus = computed(() => {
    const grouped: Record<TaskStatus, ProjectTask[]> = {
      ToDo: [],
      InProgress: [],
      Done: [],
      Cancelled: []
    }
    tasks.value.forEach(task => {
      if (grouped[task.status]) {
        grouped[task.status].push(task)
      }
    })
    return grouped
  })

  const tasksByProject = computed(() => {
    const grouped: Record<number, ProjectTask[]> = {}
    tasks.value.forEach(task => {
      if (!grouped[task.projectId]) {
        grouped[task.projectId] = []
      }
      grouped[task.projectId].push(task)
    })
    return grouped
  })

  const totalProjectValue = computed(() => {
    return projects.value.reduce((total, project) => total + (project.totalCost || 0), 0)
  })

  const totalLabourHours = computed(() => {
    return labourEntries.value.reduce((total, entry) => total + entry.hours, 0)
  })

  const totalLabourCost = computed(() => {
    return labourEntries.value.reduce((total, entry) => total + entry.totalCost, 0)
  })

  // Actions
  async function fetchProjects(status?: ProjectStatus) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      projects.value = [
        {
          id: 1,
          title: 'Kitchen Renovation - Mkhize Residence',
          description: 'Complete kitchen renovation including cabinets, tiling, and plumbing',
          status: 'InProgress',
          customerId: 1,
          customerName: 'John Mkhize',
          shopId: 1,
          shopName: 'Main Shop',
          startDate: new Date(Date.now() - 15 * 24 * 60 * 60 * 1000),
          expectedCompletionDate: new Date(Date.now() + 10 * 24 * 60 * 60 * 1000),
          totalCost: 45000,
          totalRevenue: 55000,
          createdAt: new Date(Date.now() - 15 * 24 * 60 * 60 * 1000),
          updatedAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000)
        },
        {
          id: 2,
          title: 'Bathroom Tiling - Dlamini Shop',
          description: 'Install ceramic tiles in bathroom',
          status: 'New',
          customerId: 2,
          customerName: 'Sarah Dlamini',
          shopId: 1,
          shopName: 'Main Shop',
          expectedCompletionDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000),
          totalCost: 0,
          totalRevenue: 8500,
          createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
          updatedAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000)
        },
        {
          id: 3,
          title: 'Electrical Wiring - New Building',
          description: 'Complete electrical installation for new building',
          status: 'Completed',
          customerId: 3,
          customerName: 'Mike Ndlovu',
          shopId: 1,
          shopName: 'Main Shop',
          startDate: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000),
          expectedCompletionDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000),
          completedDate: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000),
          totalCost: 28000,
          totalRevenue: 35000,
          salesInvoiceId: 101,
          createdAt: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000),
          updatedAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000)
        }
      ]

      if (status) {
        projects.value = projects.value.filter(p => p.status === status)
      }
    } catch (error) {
      console.error('Failed to fetch projects:', error)
    } finally {
      loading.value = false
    }
  }

  async function fetchProjectById(id: number) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 300))
      
      const project = projects.value.find(p => p.id === id)
      if (project) {
        selectedProject.value = project
        // Fetch related tasks and labour entries
        await fetchTasksByProject(id)
        await fetchLabourEntriesByProject(id)
      }
      return project
    } catch (error) {
      console.error('Failed to fetch project:', error)
      return null
    } finally {
      loading.value = false
    }
  }

  async function fetchTasksByProject(projectId: number) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 300))
      
      // Mock tasks for project
      const projectTasks: ProjectTask[] = [
        {
          id: 1,
          projectId,
          title: 'Remove old cabinets',
          description: 'Remove existing kitchen cabinets',
          status: 'Done',
          assigneeId: 'user1',
          assigneeName: 'Thabo Mthembu',
          dueDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
          completedDate: new Date(Date.now() - 8 * 24 * 60 * 60 * 1000),
          estimatedHours: 4,
          actualHours: 3.5,
          createdAt: new Date(Date.now() - 15 * 24 * 60 * 60 * 1000),
          updatedAt: new Date(Date.now() - 8 * 24 * 60 * 60 * 1000)
        },
        {
          id: 2,
          projectId,
          title: 'Install new cabinets',
          description: 'Install new kitchen cabinets',
          status: 'InProgress',
          assigneeId: 'user1',
          assigneeName: 'Thabo Mthembu',
          dueDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000),
          estimatedHours: 16,
          actualHours: 8,
          createdAt: new Date(Date.now() - 12 * 24 * 60 * 60 * 1000),
          updatedAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000)
        },
        {
          id: 3,
          projectId,
          title: 'Install tiles',
          description: 'Install ceramic floor tiles',
          status: 'ToDo',
          assigneeId: 'user2',
          assigneeName: 'Sipho Zulu',
          dueDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000),
          estimatedHours: 12,
          createdAt: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
          updatedAt: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000)
        }
      ]

      // Filter existing tasks and add new ones
      tasks.value = tasks.value.filter(t => t.projectId !== projectId)
      tasks.value.push(...projectTasks)
    } catch (error) {
      console.error('Failed to fetch tasks:', error)
    } finally {
      loading.value = false
    }
  }

  async function fetchLabourEntriesByProject(projectId: number) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 300))
      
      // Mock labour entries
      const entries: LabourEntry[] = [
        {
          id: 1,
          projectId,
          projectTaskId: 1,
          userId: 'user1',
          userName: 'Thabo Mthembu',
          workDate: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000),
          hours: 3.5,
          rate: 150,
          totalCost: 525,
          description: 'Removed old cabinets',
          createdAt: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000)
        },
        {
          id: 2,
          projectId,
          projectTaskId: 2,
          userId: 'user1',
          userName: 'Thabo Mthembu',
          workDate: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
          hours: 8,
          rate: 150,
          totalCost: 1200,
          description: 'Installed new cabinets',
          createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000)
        }
      ]

      // Filter existing entries and add new ones
      labourEntries.value = labourEntries.value.filter(e => e.projectId !== projectId)
      labourEntries.value.push(...entries)
    } catch (error) {
      console.error('Failed to fetch labour entries:', error)
    } finally {
      loading.value = false
    }
  }

  async function createProject(project: Omit<Project, 'id' | 'createdAt' | 'updatedAt'>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const newProject: Project = {
        ...project,
        id: Date.now(),
        createdAt: new Date(),
        updatedAt: new Date()
      }
      
      projects.value.push(newProject)
      return newProject
    } catch (error) {
      console.error('Failed to create project:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function updateProject(id: number, updates: Partial<Project>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const index = projects.value.findIndex(p => p.id === id)
      if (index !== -1) {
        projects.value[index] = {
          ...projects.value[index],
          ...updates,
          updatedAt: new Date()
        }
      }
    } catch (error) {
      console.error('Failed to update project:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function updateProjectStatus(id: number, status: ProjectStatus) {
    return updateProject(id, { status })
  }

  async function createTask(task: Omit<ProjectTask, 'id' | 'createdAt' | 'updatedAt'>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const newTask: ProjectTask = {
        ...task,
        id: Date.now(),
        createdAt: new Date(),
        updatedAt: new Date()
      }
      
      tasks.value.push(newTask)
      return newTask
    } catch (error) {
      console.error('Failed to create task:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function updateTask(id: number, updates: Partial<ProjectTask>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const index = tasks.value.findIndex(t => t.id === id)
      if (index !== -1) {
        tasks.value[index] = {
          ...tasks.value[index],
          ...updates,
          updatedAt: new Date()
        }
      }
    } catch (error) {
      console.error('Failed to update task:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function createLabourEntry(entry: Omit<LabourEntry, 'id' | 'createdAt'>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const newEntry: LabourEntry = {
        ...entry,
        id: Date.now(),
        createdAt: new Date()
      }
      
      labourEntries.value.push(newEntry)
      return newEntry
    } catch (error) {
      console.error('Failed to create labour entry:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  function getProjectById(id: number): Project | undefined {
    return projects.value.find(p => p.id === id)
  }

  function getTasksByProject(projectId: number): ProjectTask[] {
    return tasks.value.filter(t => t.projectId === projectId)
  }

  function getLabourEntriesByProject(projectId: number): LabourEntry[] {
    return labourEntries.value.filter(e => e.projectId === projectId)
  }

  return {
    // State
    projects,
    tasks,
    labourEntries,
    loading,
    selectedProject,
    // Computed
    projectsByStatus,
    activeProjects,
    completedProjects,
    overdueProjects,
    tasksByStatus,
    tasksByProject,
    totalProjectValue,
    totalLabourHours,
    totalLabourCost,
    // Actions
    fetchProjects,
    fetchProjectById,
    fetchTasksByProject,
    fetchLabourEntriesByProject,
    createProject,
    updateProject,
    updateProjectStatus,
    createTask,
    updateTask,
    createLabourEntry,
    getProjectById,
    getTasksByProject,
    getLabourEntriesByProject
  }
})



