/**
 * Date range and time formatting utilities
 */
import { DateRange } from '../types'

/**
 * Convert a JavaScript Date to ISO 8601 string in local timezone
 * Format: YYYY-MM-DDTHH:mm:ss
 */
export const getLocalISOString = (date: Date): string => {
  return date.toLocaleString('sv').replace(' ', 'T')
}

/**
 * Get date range boundaries based on the selected range type
 * Returns [startDate, endDate] in ISO 8601 format
 */
export const getDateRangeBoundaries = (range: DateRange): [string, string] => {
  const today = new Date()
  today.setHours(23, 59, 59, 999)
  const endStr = getLocalISOString(today)
  today.setHours(0, 0, 0, 0)

  switch (range) {
    case 'today':
      return [getLocalISOString(today), endStr]
    case 'last-3-days':
      today.setDate(today.getDate() - 3)
      return [getLocalISOString(today), endStr]
    case 'last-week':
      today.setDate(today.getDate() - 7)
      return [getLocalISOString(today), endStr]
  }
}

/**
 * Format a timestamp string to a readable local date/time string
 */
export const formatTimestamp = (timestamp: string): string => {
  return new Date(timestamp).toLocaleString("en-GB")
}
