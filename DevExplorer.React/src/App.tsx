import { useState } from 'react'
import { Sidebar } from './components/Sidebar'
import { ProjectTabs } from './components/ProjectTabs'
import { LogFilter } from './components/FilterPanel'
import { LogViewer } from './components/LogViewer'
import { useProjectLogs } from './hooks/useProjectData'

function App() {
  const [selectedProject, setSelectedProject] = useState<string | null>(null)
  const [activeTab, setActiveTab] = useState<'logs' | 'search'>('logs')
  const [filter, setFilter] = useState<{ dateFrom?: string; dateTo?: string; logLevel?: string }>({})

  const { data: logs = [], isLoading, error } = useProjectLogs(selectedProject, filter)

  const handleFilter = (dateFrom?: string, dateTo?: string, logLevel?: string) => {
    setFilter({
      dateFrom: dateFrom ? new Date(dateFrom).toISOString() : undefined,
      dateTo: dateTo ? new Date(dateTo).toISOString() : undefined,
      logLevel
    })
  }

  return (
    <div className="app-container">
      <header className="app-header">
        <div className="app-header-content">
          <h1 className="app-title">DevExplorer</h1>
        </div>
      </header>

      <div className="app-layout">
        <Sidebar selectedProject={selectedProject} onSelect={setSelectedProject} />

        <main className="app-main">
          {selectedProject ? (
            <ProjectTabs activeTab={activeTab} onTabChange={setActiveTab}>
              {activeTab === 'logs' && (
                <div className="tab-panel">
                  <LogFilter onFilter={handleFilter} />
                  <LogViewer logs={logs} isLoading={isLoading} error={error as Error | null} />
                </div>
              )}
              {activeTab === 'search' && (
                <div className="tab-panel">
                  <div className="placeholder-content">Search functionality coming soon</div>
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
