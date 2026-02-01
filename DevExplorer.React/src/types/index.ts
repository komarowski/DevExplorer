/**
 * Shared type definitions for the DevExplorer application
 */

export interface Project {
  name: string
  hasLogs?: boolean
  actions?: ProjectAction[]
}

export interface ProjectAction {
  name: string
  actionType: number
  description?: string
}

export enum LogLevel {
  Trace = 'Trace',
  Debug = 'Debug',
  Info = 'Info',
  Warning = 'Warning',
  Error = 'Error',
  Critical = 'Critical'
}

export interface LogEvent {
  message: string
  level: number
  filePath: string
  timestamp: string
}

export interface LogFilter {
  dateFrom?: string
  dateTo?: string
}

export interface LogFilterState {
  dateFrom?: string
  dateTo?: string
  logLevel?: LogLevel
}

export interface ActionResponse {
  success: boolean
  data: string
  contentType: string
  message: string | null
}

export type ResultFormat = 'json' | 'text' | 'xml' | 'html'
export type DateRange = 'today' | 'last-3-days' | 'last-week'
