import axios from 'axios'
import { mockProjects, mockLogEvents } from './mockData'

const API_BASE = ''
const USE_MOCK_DATA = false;

export const apiClient = axios.create({
  baseURL: API_BASE,
  headers: {
    'Content-Type': 'application/json'
  }
})

export interface Project {
  name: string
}

export enum LogLevel {
  Debug = 'debug',
  Info = 'info',
  Warning = 'warning',
  Error = 'error',
  Critical = 'critical'
}

export interface LogEvent {
  message: string
  level: string
  filePath: string
  timestamp: string
  correlationId?: string
}

export interface LogFilter {
  dateFrom?: string
  dateTo?: string
  logLevel?: string
}

export const projectApi = {
  getProjects: () =>
    USE_MOCK_DATA
      ? Promise.resolve({ data: mockProjects })
      : apiClient.get<Project[]>('/projects'),
  
  getProjectLogs: (projectName: string, filter?: LogFilter) => {
    if (USE_MOCK_DATA) {
      let filtered = [...mockLogEvents]
      
      if (filter?.dateFrom) {
        const dateFrom = new Date(filter.dateFrom)
        filtered = filtered.filter(log => new Date(log.timestamp) >= dateFrom)
      }
      
      if (filter?.dateTo) {
        const dateTo = new Date(filter.dateTo)
        // Set time to end of day
        dateTo.setHours(23, 59, 59, 999)
        filtered = filtered.filter(log => new Date(log.timestamp) <= dateTo)
      }
      
      if (filter?.logLevel) {
        const logLevelHierarchy: Record<string, number> = {
          'debug': 0,
          'info': 1,
          'warning': 2,
          'error': 3,
          'critical': 4
        }
        
        const selectedLevelValue = logLevelHierarchy[filter.logLevel.toLowerCase()] ?? 4
        filtered = filtered.filter(log => {
          const logLevelValue = logLevelHierarchy[log.level.toLowerCase()] ?? 0
          return logLevelValue >= selectedLevelValue
        })
      }
      
      return Promise.resolve({ data: filtered })
    }
    return apiClient.post<LogEvent[]>(`/projects/${projectName}/logs`, filter)
  }
}
