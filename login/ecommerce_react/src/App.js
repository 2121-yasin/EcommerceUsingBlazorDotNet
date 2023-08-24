import React from 'react';
import './App.css';
import Header from './Header';
import { BrowserRouter as Router,Route,Routes } from 'react-router-dom'; // Import Routes

// Import your additional components/pages here
import Home from './Home'; // Create the Home component

function App() {
  return (
    <div>
      <Router>
        <Header />
        <Routes>
          {/* Define your routes */}
          <Route path="/" element={<Home />} />
          <Route path="/Home" element={<Home />} />
          {/* Add more routes for other pages */}
        </Routes>
      </Router>
    </div>
  );
}

export default App;
