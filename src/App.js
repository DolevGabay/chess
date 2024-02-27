import React from 'react';
import { BrowserRouter, Route, Routes } from "react-router-dom";
import Board from './Chessboard';
import Home from './Home';
import Login from './Login';

function App() {
  return (
        <div>
          <BrowserRouter>
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/board" element={<Board />} />
              <Route path="/login" element={<Login />} />
            </Routes>
          </BrowserRouter>
        </div>
  );
}

export default App;
