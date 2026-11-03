import { Navigate, Route, Routes } from 'react-router-dom';
import AiCommandCenter from './pages/ai/AiCommandCenter';
import LearningPage from './pages/LearningPage';

function App() {
  return (
    <Routes>
      <Route path="/ai" element={<AiCommandCenter />} />
      <Route path="/learning" element={<LearningPage />} />
      <Route path="*" element={<Navigate to="/ai" replace />} />
    </Routes>
  );
}

export default App;