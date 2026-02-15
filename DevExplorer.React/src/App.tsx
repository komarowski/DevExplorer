import { useState } from 'react'
import { Sidebar } from './components/Sidebar'
import { ProjectTabs } from './components/ProjectTabs'
import { LogFilter, LogViewer } from './components/logs'
import { ActionViewer } from './components/actions'
import { useProjectLogs, useProjects, useEnvironment, useSetEnvironment } from './hooks/useProjectData'
import { Project, LogLevel } from './types'

function App() {
  const [selectedProject, setSelectedProject] = useState<string | null>(null)
  const [activeTab, setActiveTab] = useState<'logs' | 'actions'>('logs')
  const [selectedAction, setSelectedAction] = useState<string | null>(null)
  const [filter, setFilter] = useState<{ dateFrom?: string; dateTo?: string }>({})
  const [logLevel, setLogLevel] = useState<LogLevel>(LogLevel.Error)

  const { data: projects = [] } = useProjects()
  const { data: logs = [], isLoading, error } = useProjectLogs(selectedProject, filter)
  const { data: environment } = useEnvironment()
  const setEnvironmentMutation = useSetEnvironment()

  const handleEnvironmentChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setEnvironmentMutation.mutate(e.target.value)
    setSelectedProject(null)
  }

  const currentProject: Project | undefined = projects.find(p => p.name === selectedProject)
  const hasLogs = currentProject?.hasLogs ?? false
  const projectActions = currentProject?.actions ?? []

  // Filter logs by level on the frontend
  const filteredLogs = logs.filter(log => {
    const logLevelValue = Object.values(LogLevel).indexOf(logLevel)
    return log.level >= logLevelValue
  })

  const handleFilter = (dateFrom?: string, dateTo?: string, selectedLogLevel?: LogLevel) => {
    // Only send date filters to backend
    setFilter({
      dateFrom: dateFrom ? new Date(dateFrom).toISOString() : undefined,
      dateTo: dateTo ? new Date(dateTo).toISOString() : undefined
    })
    // Keep log level for frontend filtering
    if (selectedLogLevel) {
      setLogLevel(selectedLogLevel)
    }
  }

  return (
    <div className="app-container">
      <header className="app-header">
        <div className="app-header-content">
          <h1 className="app-title">DevExplorer</h1>
          <div className="environment-selector">
            <select
              id="environment-select"
              value={environment?.current ?? ''}
              onChange={handleEnvironmentChange}
              disabled={setEnvironmentMutation.isLoading}
              title='Select the environment to view projects and logs for'
            >
              {environment?.available.map((env) => (
                <option key={env} value={env}>
                  {env}
                </option>
              ))}
            </select>
          </div>
        </div>
      </header>

      <div className="app-layout">
        <Sidebar selectedProject={selectedProject} onSelect={setSelectedProject} />

        <main className="app-main">
          {selectedProject ? (
            <ProjectTabs
              activeTab={activeTab}
              onTabChange={setActiveTab}
              hasLogs={hasLogs}
              hasActions={projectActions.length > 0}
            >
              {activeTab === 'logs' && (
                <div className="tab-panel">
                  <LogFilter onFilter={handleFilter} />
                  <LogViewer logs={filteredLogs} isLoading={isLoading} error={error as Error | null} />
                </div>
              )}
              {activeTab === 'actions' && (
                <div className="tab-panel">
                  <ActionViewer
                    projectName={selectedProject}
                    actions={projectActions}
                    selectedAction={selectedAction}
                    onActionSelect={setSelectedAction}
                  />
                </div>
              )}
            </ProjectTabs>
          ) : (
            <div className="placeholder">
              <p>Select a project from the sidebar</p>
            </div>
          )}
        </main>
      </div>
    </div>
  )
}

export default App
