import React from 'react'

interface ProjectTabsProps {
  activeTab: 'logs' | 'search'
  onTabChange: (tab: 'logs' | 'search') => void
  children: React.ReactNode
}

export const ProjectTabs: React.FC<ProjectTabsProps> = ({ activeTab, onTabChange, children }) => {
  return (
    <>
      <div className="tabs-header">
        <button
          onClick={() => onTabChange('logs')}
          className={`tab-button ${activeTab === 'logs' ? 'active' : ''}`}
        >
          Logs
        </button>
        <button
          onClick={() => onTabChange('search')}
          className={`tab-button ${activeTab === 'search' ? 'active' : ''}`}
        >
          Search
        </button>
      </div>
      <div className="tabs-content">{children}</div>
    </>
  )
}
