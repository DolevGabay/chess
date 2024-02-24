import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { BoardUtils } from './BoardUtils';
import './Home.css';

const Home = () => {
  const navigate = useNavigate();
  const [gameId, setGameId] = useState('');
  const [username, setUsername] = useState('');

  const handleStartGame = () => {
    navigate(`/board?mode=start&gameId=${gameId}&username=${username}`);
  };

  const handleJoinGame = () => {
    if (gameId === '') {
        alert('Please enter a game ID');
        return;
    }
    
    navigate(`/board?mode=join&gameId=${gameId}&username=${username}`);
  };

  const handleGameIdChange = (event) => {
    setGameId(event.target.value);
  };

  const handleUsernameChange = (event) => {
    setUsername(event.target.value);
  };

  return (
    <div className="home-body">
      <div className="login-wrap">
        <div className="login-html">
          <input id="tab-1" type="radio" name="tab" className="sign-in" defaultChecked />
          <label htmlFor="tab-1" className="tab">Start Game</label>
          <input id="tab-2" type="radio" name="tab" className="sign-up" />
          <label htmlFor="tab-2" className="tab">Join Game</label>
          <div className="login-form">
            <div className="sign-in-htm">
              <div className="group">
                <label htmlFor="user" className="label">Username</label>
                <input id="user" type="text" className="input" value={username} onChange={handleUsernameChange} />
              </div>
              <div className="group">
                <input type="submit" className="button" value="Start" onClick={handleStartGame} />
              </div>
            </div>
            <div className="sign-up-htm">
              <div className="group">
                <label htmlFor="user" className="label">Username</label>
                <input id="user" type="text" className="input" value={username} onChange={handleUsernameChange} />
              </div>
              <div className="group">
                <label htmlFor="pass" className="label">Game ID</label>
                <input id="pass"  className="input"  value={gameId} onChange={handleGameIdChange} />
              </div>
              <div className="group">
                <input type="submit" className="button" value="Join" onClick={handleJoinGame} />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Home;
