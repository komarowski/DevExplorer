import React from 'react'
import { LogEvent } from '../api/projectApi'

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
        {logs.map((log, index) => (
          <div key={index} className={`log-entry log-level-${log.level.toLowerCase()}`}>
            <div className="log-header">
              <span className={`log-level`}>[{log.level}]</span>
              <span className="log-timestamp">{new Date(log.timestamp).toLocaleString()}</span>
            </div>
            {log.correlationId && (
              <div className="log-correlation">
                CorrelationId: <span>{log.correlationId}</span>
              </div>
            )}
            <div className="log-message">{log.message}</div>
          </div>
        ))}
      </div>
      <div className="log-count">Found {logs.length} log entries</div>
    </div>
  )
}
