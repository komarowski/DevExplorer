import { Project, LogEvent } from '../types'

export const mockProjects: Project[] = [
  { name: 'Test1' },
  { name: 'Test2' }
]

export const mockLogEvents: LogEvent[] = [
  {
    message: 'Application started successfully',
    level: 0,
    filePath: 'Program.cs',
    timestamp: '2026-01-20T08:00:00Z'
  },
  {
    message: 'Database connection established',
    level: 2,
    filePath: 'DatabaseService.cs',
    timestamp: '2026-01-20T08:00:05Z',
  },
  {
    message: 'Configuration loaded from appsettings.json',
    level: 1,
    filePath: 'Startup.cs',
    timestamp: '2026-01-20T08:00:10Z',
  },
  {
    message: 'Dependency injection container configured',
    level: 2,
    filePath: 'Startup.cs',
    timestamp: '2026-01-20T08:00:15Z',
  },
  {
    message: 'GET /api/projects request received',
    level: 2,
    filePath: 'ProjectController.cs',
    timestamp: '2026-01-20T08:15:00Z',
  },
  {
    message: 'Query executed: SELECT * FROM Projects',
    level: 1,
    filePath: 'ProjectRepository.cs',
    timestamp: '2026-01-20T08:15:05Z',
  },
  {
    message: 'Warning: Slow query detected. Execution time: 245ms',
    level: 2,
    filePath: 'ProjectRepository.cs',
    timestamp: '2026-01-20T08:15:06Z',
  },
  {
    message: 'Failed to connect to external logging service',
    level: 3,
    filePath: 'LoggingService.cs',
    timestamp: '2026-01-20T08:30:00Z',
  },
  {
    message: 'Retrying connection attempt 1 of 3',
    level: 2,
    filePath: 'LoggingService.cs',
    timestamp: '2026-01-20T08:30:05Z',
  },
  {
    message: 'CRITICAL: System failure - All services down',
    level: 4,
    filePath: 'SystemMonitor.cs',
    timestamp: '2026-01-24T10:30:00Z',
  }
]