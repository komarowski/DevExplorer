import React from 'react'
import { LogEvent } from '../../types'
import { convertLogLevel } from '../../utils/formatters'
import { formatTimestamp } from '../../utils/dateRange'

interface LogViewerProps {
  logs: LogEvent[]
  isLoading: boolean
  error: Error | null
}

export const LogViewer: React.FC<LogViewerProps> = ({ logs, isLoading, error }) => {
  if (isLoading) return <div className="loading-text">Loading logs...</div>
  if (error) return <div className="error-text">Failed to load logs: {error.message}</div>
  if (logs.length === 0) return <div className="loading-text">No logs found</div>
  
  return (
    <div className="log-viewer">
      <div className="log-container">
        {logs.map((log, index) => {
          console.log(log);
          const levelStr = convertLogLevel(log.level)
          return (
          <div key={index} className={`log-entry log-level-${levelStr}`}>
            <div className="log-header">
              <span className={`log-level`}>[{levelStr}]</span>
              <span className="log-timestamp">{formatTimestamp(log.timestamp)}</span>
            </div>
            <div className="log-message">{log.message}</div>
          </div>
          )
        })}
      </div>
      <div className="log-count">Found {logs.length} log entries</div>
    </div>
  )
}
