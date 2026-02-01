import React from 'react'

interface ProjectTabsProps {
  activeTab: 'logs' | 'actions'
  onTabChange: (tab: 'logs' | 'actions') => void
  hasLogs: boolean
  hasActions: boolean
  children: React.ReactNode
}

export const ProjectTabs: React.FC<ProjectTabsProps> = ({
  activeTab,
  onTabChange,
  hasLogs,
  hasActions,
  children
}) => {
  return (
    <>
      <div className="tabs-header">
        {hasLogs && (
          <button
            onClick={() => onTabChange('logs')}
            className={`tab-button ${activeTab === 'logs' ? 'active' : ''}`}
          >
            Logs
          </button>
        )}
        {hasActions && (
          <button
            onClick={() => onTabChange('actions')}
            className={`tab-button ${activeTab === 'actions' ? 'active' : ''}`}
          >
            Actions
          </button>
        )}
      </div>
      <div className="tabs-content">{children}</div>
    </>
  )
}
