import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const Home = () => {
  const navigate = useNavigate();
  const [gameId, setGameId] = useState('');

  const handleStartGame = () => {
    navigate(`/board?mode=start&gameId=${gameId}`);
  };

  const handleJoinGame = () => {
    navigate(`/board?mode=join&gameId=${gameId}`);
  };

  const handleGameIdChange = (event) => {
    setGameId(event.target.value);
  };

  return (
    <div>
      <h1>Welcome to My Website</h1>
      <p>This is a simple home page built with React.</p>
      <input
        type="text"
        value={gameId}
        onChange={handleGameIdChange}
        placeholder="Enter Game ID"
      />
      <button onClick={handleStartGame}>Start Game</button>
      <button onClick={handleJoinGame}>Join Game</button>
    </div>
  );
};

export default Home;
