import { Project, LogEvent } from './projectApi'

export const mockProjects: Project[] = [
  { name: 'Test1' },
  { name: 'Test2' }
]

export const mockLogEvents: LogEvent[] = [
  {
    message: 'Application started successfully',
    level: 'info',
    filePath: 'Program.cs',
    timestamp: '2026-01-20T08:00:00Z',
    correlationId: 'req-001'
  },
  {
    message: 'Database connection established',
    level: 'info',
    filePath: 'DatabaseService.cs',
    timestamp: '2026-01-20T08:00:05Z',
    correlationId: 'req-001'
  },
  {
    message: 'Configuration loaded from appsettings.json',
    level: 'debug',
    filePath: 'Startup.cs',
    timestamp: '2026-01-20T08:00:10Z',
    correlationId: 'req-001'
  },
  {
    message: 'Dependency injection container configured',
    level: 'debug',
    filePath: 'Startup.cs',
    timestamp: '2026-01-20T08:00:15Z',
    correlationId: 'req-001'
  },
  {
    message: 'GET /api/projects request received',
    level: 'info',
    filePath: 'ProjectController.cs',
    timestamp: '2026-01-20T08:15:00Z',
    correlationId: 'req-002'
  },
  {
    message: 'Query executed: SELECT * FROM Projects',
    level: 'debug',
    filePath: 'ProjectRepository.cs',
    timestamp: '2026-01-20T08:15:05Z',
    correlationId: 'req-002'
  },
  {
    message: 'Warning: Slow query detected. Execution time: 245ms',
    level: 'warning',
    filePath: 'ProjectRepository.cs',
    timestamp: '2026-01-20T08:15:06Z',
    correlationId: 'req-002'
  },
  {
    message: 'Failed to connect to external logging service',
    level: 'error',
    filePath: 'LoggingService.cs',
    timestamp: '2026-01-20T08:30:00Z',
    correlationId: 'req-003'
  },
  {
    message: 'Retrying connection attempt 1 of 3',
    level: 'warning',
    filePath: 'LoggingService.cs',
    timestamp: '2026-01-20T08:30:05Z',
    correlationId: 'req-003'
  },
  {
    message: 'CRITICAL: System failure - All services down',
    level: 'critical',
    filePath: 'SystemMonitor.cs',
    timestamp: '2026-01-24T10:30:00Z',
    correlationId: 'req-006'
  }
]