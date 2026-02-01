import React, { useState } from 'react'
import { ProjectAction, ActionResponse, ResultFormat } from '../../types'
import { projectApi } from '../../api/projectApi'
import { formatActionResponse } from '../../utils/formatters'

interface ActionViewerProps {
  projectName: string | null
  actions: ProjectAction[]
  selectedAction: string | null
  onActionSelect: (action: string) => void
}

export const ActionViewer: React.FC<ActionViewerProps> = ({
  projectName,
  actions,
  selectedAction,
  onActionSelect
}) => {
  const [result, setResult] = useState<string | null>(null)
  const [resultFormat, setResultFormat] = useState<ResultFormat>('json')
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const selected = actions.find(a => a.name === selectedAction)

  const handleActionSelect = async (actionName: string) => {
    onActionSelect(actionName)

    if (!projectName) return

    await executeAction(actionName)
  }

  const executeAction = async (actionName: string) => {
    setIsLoading(true)
    setError(null)
    setResult(null)

    try {
      const response = await projectApi.executeProjectAction(projectName!, actionName)
      const actionResponse: ActionResponse = response.data

      if (!actionResponse.success) {
        setError(actionResponse.message || 'Action failed')
        setResult(null)
        return
      }

      const [format, displayData] = formatActionResponse(
        actionResponse.data,
        actionResponse.contentType
      )
      setResultFormat(format)
      setResult(displayData)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to execute action')
      setResult(null)
    } finally {
      setIsLoading(false)
    }
  }

  const renderResult = () => {
    if (!result) return null

    switch (resultFormat) {
      case 'json':
        return (
          <pre className="result-content result-json">
            <code>{result}</code>
          </pre>
        )
      case 'xml':
        return (
          <pre className="result-content result-xml">
            <code>{result}</code>
          </pre>
        )
      case 'html':
        return (
          <iframe
            srcDoc={result}
            className="result-content result-html"
            title="HTML Result"
          />
        )
      case 'text':
      default:
        return (
          <pre className="result-content result-text">
            <code>{result}</code>
          </pre>
        )
    }
  }

  return (
    <div className="action-viewer">
      <div className="action-selector">
        <label className="filter-label">Select Action:</label>
        <div className="selector-controls">
          <select
            onChange={(e) => handleActionSelect(e.target.value)}
            value={selectedAction || ''}
            className="form-select"
          >
            <option value="">Choose an action...</option>
            {actions.map((action) => {
              const typeLabel = action.actionType === 0 ? 'sql' : 'api'
              return (
                <option key={action.name} value={action.name} title={action.description}>
                  {typeLabel}: {action.name}
                </option>
              )
            })}
          </select>
          {selectedAction && (result || error) && (
            <button
              onClick={() => selectedAction && executeAction(selectedAction)}
              disabled={isLoading}
              className="replay-btn"
            >
              {isLoading ? 'Executing...' : 'Replay'}
            </button>
          )}
        </div>
      </div>

      {isLoading && <div className="loading-text">Executing action...</div>}

      {error && <div className="error-text">Error: {error}</div>}

      {result && (
        <div className="action-result">
          <div className="result-header">
            <span className="result-format">{resultFormat.toUpperCase()}</span>
          </div>
          {renderResult()}
        </div>
      )}

      {!selected && !result && (
        <div className="placeholder-content">Select an action to view details and execute</div>
      )}
    </div>
  )
}
