import React from 'react';
import { NavLink } from 'react-router-dom';

export const MainLayout = ({ children }) => (
  <div className="app-shell">
    <aside className="app-sidebar">
      <div className="brand">WiseRavenShare</div>
      <nav className="side-nav">
        <NavLink to="/ai" className={({ isActive }) => `side-nav-item${isActive ? ' active' : ''}`}>
          <span className="side-nav-icon">✨</span>
          <span>AI Command</span>
        </NavLink>
        <NavLink
          to="/learning"
          className={({ isActive }) => `side-nav-item${isActive ? ' active' : ''}`}
        >
          <span className="side-nav-icon">📘</span>
          <span>Learning</span>
        </NavLink>
      </nav>
    </aside>
    <section className="app-content">{children}</section>
  </div>
);
