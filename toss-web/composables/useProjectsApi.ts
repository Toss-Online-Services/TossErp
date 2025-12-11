import { ref } from 'vue'
import { useApi, useAuthApi } from './useApi'
import type { Project, ProjectTask, LabourEntry, ProjectStatus, TaskStatus } from '~/stores/projects'

export function useProjectsApi() {
  const { getHeaders } = useAuthApi()
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBase || 'http://localhost:5000/api'

  async function getProjects(status?: ProjectStatus) {
    const url = status ? `/projects?status=${status}` : '/projects'
    const { data, error, execute } = useApi<Project[]>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getProjectById(id: number) {
    const { data, error, execute } = useApi<Project>(`/projects/${id}`, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function createProject(project: Omit<Project, 'id' | 'createdAt' | 'updatedAt'>) {
    const { data, error, execute } = useApi<Project>('/projects', {
      method: 'POST',
      body: project,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function updateProject(id: number, updates: Partial<Project>) {
    const { data, error, execute } = useApi<Project>(`/projects/${id}`, {
      method: 'PUT',
      body: updates,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function updateProjectStatus(id: number, status: ProjectStatus) {
    const { data, error, execute } = useApi<Project>(`/projects/${id}/status`, {
      method: 'PUT',
      body: { status },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getTasks(projectId?: number) {
    const url = projectId ? `/projects/${projectId}/tasks` : '/projects/tasks'
    const { data, error, execute } = useApi<ProjectTask[]>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function createTask(task: Omit<ProjectTask, 'id' | 'createdAt' | 'updatedAt'>) {
    const { data, error, execute } = useApi<ProjectTask>(`/projects/${task.projectId}/tasks`, {
      method: 'POST',
      body: task,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function updateTask(id: number, projectId: number, updates: Partial<ProjectTask>) {
    const { data, error, execute } = useApi<ProjectTask>(`/projects/${projectId}/tasks/${id}`, {
      method: 'PUT',
      body: updates,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getLabourEntries(projectId?: number) {
    const url = projectId ? `/projects/${projectId}/labour` : '/projects/labour'
    const { data, error, execute } = useApi<LabourEntry[]>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function createLabourEntry(entry: Omit<LabourEntry, 'id' | 'createdAt'>) {
    const { data, error, execute } = useApi<LabourEntry>(`/projects/${entry.projectId}/labour`, {
      method: 'POST',
      body: entry,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  return {
    getProjects,
    getProjectById,
    createProject,
    updateProject,
    updateProjectStatus,
    getTasks,
    createTask,
    updateTask,
    getLabourEntries,
    createLabourEntry
  }
}



