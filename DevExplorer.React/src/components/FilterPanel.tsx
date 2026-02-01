import React, { useState, useEffect } from 'react'
import { LogLevel } from '../api/projectApi'

type DateRange = 'today' | 'last-3-days' | 'last-week'

interface LogFilterProps {
  onFilter: (dateFrom?: string, dateTo?: string, logLevel?: LogLevel) => void
}

export const LogFilter: React.FC<LogFilterProps> = ({ onFilter }) => {
  const [dateRange, setDateRange] = useState<DateRange>('today')
  const [logLevel, setLogLevel] = useState<LogLevel>(LogLevel.Error)

  const getDateRange = (range: DateRange): [string, string] => {
    const today = new Date()
    today.setHours(0, 0, 0, 0)
    const todayStr = today.toISOString().split('T')[0]
    const endStr = todayStr

    const start = new Date(today)
    switch (range) {
      case 'today':
        return [todayStr, endStr]
      case 'last-3-days':
        start.setDate(start.getDate() - 3)
        return [start.toISOString().split('T')[0], endStr]
      case 'last-week':
        start.setDate(start.getDate() - 7)
        return [start.toISOString().split('T')[0], endStr]
    }
  }

  useEffect(() => {
    const [dateFrom, dateTo] = getDateRange(dateRange)
    onFilter(dateFrom, dateTo, logLevel)
  }, [dateRange, logLevel])

  return (
    <div className="filter-panel">
      <div className="filter-row">
        <div className="filter-field">
          <label className="filter-label">Date Range:</label>
          <select
            value={dateRange}
            onChange={(e) => setDateRange(e.target.value as DateRange)}
            className="form-select"
          >
            <option value="today">Today</option>
            <option value="last-3-days">Last 3 Days</option>
            <option value="last-week">Last Week</option>
          </select>
        </div>

        <div className="filter-field">
          <label className="filter-label">Log Level:</label>
          <select
            value={logLevel}
            onChange={(e) => setLogLevel(e.target.value as LogLevel)}
            className="form-select"
          >
            <option value={LogLevel.Critical}>Critical</option>
            <option value={LogLevel.Error}>Error</option>
            <option value={LogLevel.Warning}>Warning</option>
            <option value={LogLevel.Info}>Info</option>
            <option value={LogLevel.Debug}>Debug</option>
          </select>
        </div>
      </div>
    </div>
  )
}
