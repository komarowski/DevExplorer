import { useQuery, useMutation, useQueryClient } from 'react-query'
import { projectApi } from '../api/projectApi'

export const useProjects = () => {
  return useQuery('projects', () => projectApi.getProjects().then(res => res.data), {
    staleTime: 1000 * 60 * 5 // 5 minutes
  })
}

export const useEnvironment = () => {
  return useQuery('environment', () => projectApi.getEnvironment().then(res => res.data), {
    staleTime: 1000 * 60 * 5 // 5 minutes
  })
}

export const useSetEnvironment = () => {
  const queryClient = useQueryClient()
  return useMutation(
    (environment: string) => projectApi.setEnvironment(environment),
    {
      onSuccess: () => {
        queryClient.invalidateQueries('environment')
        queryClient.invalidateQueries('projects')
        queryClient.invalidateQueries('logs')
      }
    }
  )
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
