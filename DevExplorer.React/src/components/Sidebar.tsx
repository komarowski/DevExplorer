import React from 'react'
import { useProjects } from '../hooks/useProjectData'

interface SidebarProps {
  selectedProject: string | null
  onSelect: (projectName: string) => void
}

export const Sidebar: React.FC<SidebarProps> = ({ selectedProject, onSelect }) => {
  const { data: projects, isLoading } = useProjects()

  return (
    <aside className="sidebar">
      <div className="sidebar-content">
        {isLoading ? (
          <div className="sidebar-loading">Loading...</div>
        ) : (
          <ul className="project-list">
            {projects?.map((project) => (
              <li key={project.name}>
                <button
                  onClick={() => onSelect(project.name)}
                  className={`project-item ${selectedProject === project.name ? 'active' : ''}`}
                >
                  {project.name}
                </button>
              </li>
            ))}
          </ul>
        )}
      </div>
    </aside>
  )
}
