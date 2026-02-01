import axios from 'axios'
import { mockProjects, mockLogEvents } from './mockData'
import { Project, LogEvent, LogFilter } from '../types'
import { convertLogLevel } from '../utils/formatters'

const API_BASE = '/api'
const USE_MOCK_DATA = false;

export const apiClient = axios.create({
  baseURL: API_BASE,
  headers: {
    'Content-Type': 'application/json'
  }
})

export { convertLogLevel }

export const projectApi = {
  getProjects: () =>
    USE_MOCK_DATA
      ? Promise.resolve({ data: mockProjects })
      : apiClient.get<Project[]>('/projects'),
  
  getProjectLogs: (projectName: string, filter?: LogFilter) => {
    console.log(filter);
    if (USE_MOCK_DATA) {
      return Promise.resolve({ data: mockLogEvents })
    }
    return apiClient.post<LogEvent[]>(`/projects/${projectName}/logs`, filter)
  },

  executeProjectAction: (projectName: string, actionName: string) => {
    return apiClient.get(`/projects/${projectName}/actions/${actionName}`)
  }
}
