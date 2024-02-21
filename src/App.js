import React from 'react';
import { BrowserRouter, Route, Routes } from "react-router-dom";
import Board from './Chessboard';


function App() {
  return (
        <div>
          <BrowserRouter>
            <Routes>
              <Route path="/board" element={<Board />} />
            </Routes>
          </BrowserRouter>
        </div>
  );
}

export default App;
