import { useQuery } from 'react-query'
import { projectApi } from '../api/projectApi'

export const useProjects = () => {
  return useQuery('projects', () => projectApi.getProjects().then(res => res.data), {
    staleTime: 1000 * 60 * 5 // 5 minutes
  })
}

export const useProjectLogs = (projectName: string | null, filter?: { dateFrom?: string; dateTo?: string; logLevel?: string }) => {
  return useQuery(
    ['logs', projectName, filter],
    () => projectApi.getProjectLogs(projectName!, filter).then(res => res.data),
    {
      enabled: !!projectName,
      staleTime: 0 // Always fresh
    }
  )
}
