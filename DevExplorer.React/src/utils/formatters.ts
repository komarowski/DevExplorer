/**
 * Data formatting utilities for different content types
 */
import formatXml from 'xml-formatter'
import { ResultFormat } from '../types'

/**
 * Map log level numeric codes to string representations
 */
const LOG_LEVEL_MAP: Record<number, string> = {
  0: 'trace',
  1: 'debug',
  2: 'info',
  3: 'warning',
  4: 'error',
  5: 'critical'
}

/**
 * Convert numeric log level to string representation
 */
export const convertLogLevel = (level: number): string => {
  return LOG_LEVEL_MAP[level] ?? 'info'
}

/**
 * Detect content format from MIME type
 */
export const detectFormat = (contentType?: string): ResultFormat => {
  if (!contentType) return 'text'
  
  const type = contentType.toLowerCase()
  if (type.includes('json')) return 'json'
  if (type.includes('xml')) return 'xml'
  if (type.includes('html')) return 'html'
  return 'text'
}

/**
 * Format data for display based on content type
 * Handles JSON pretty-printing and XML formatting
 */
export const formatDataForDisplay = (data: string, format: ResultFormat): string => {
  try {
    if (format === 'json') {
      return JSON.stringify(JSON.parse(data), null, 2)
    } else if (format === 'xml') {
      return formatXml(data)
    }
  } catch {
    // If formatting fails, return data as-is
  }
  return data
}

/**
 * Format action response data for display
 * Combines format detection and data formatting
 */
export const formatActionResponse = (data: string, contentType?: string): [ResultFormat, string] => {
  const format = detectFormat(contentType)
  const formatted = formatDataForDisplay(data, format)
  return [format, formatted]
}
