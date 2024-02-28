import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { BoardUtils } from './BoardUtils';
import './Home.css';

const Home = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const [gameId, setGameId] = useState('');
  const [username, setUsername] = useState('');
  const searchParams = new URLSearchParams(location.search);
  const userNameFromLogin = searchParams.get('username');

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

  const handleConnectToLastGame = () => {
    if (userNameFromLogin) {
        navigate(`/board?mode=last-game&username=${userNameFromLogin}`);
    } else {
        alert('Please enter a username');
    }
  };

  useEffect(() => {
    if (userNameFromLogin) {
        setUsername(userNameFromLogin);
    }
  }, [userNameFromLogin]);

  return (
    <div className="home-body">
      <div className="login-wrap">
        <div className="login-html">
          <div>
            {userNameFromLogin ? (
              <h1 className="header">Welcome, {userNameFromLogin}!</h1>
            ) : (
              <h1 className="header">Play as a guest</h1>
            )}
          </div>
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
                <input id="pass" className="input" value={gameId} onChange={handleGameIdChange} />
              </div>
              <div className="group">
                <input type="submit" className="button" value="Join" onClick={handleJoinGame} />
              </div>
            </div>
          </div>
          {userNameFromLogin ? (
            <>
              <div className="hr"></div>
              <div className="foot-lnk">
                <div className="foot-lnk">
                  <label htmlFor="tab-1">Connect to last game?</label>
                </div>
                <button className="loginButton" onClick={handleConnectToLastGame}>Connect</button>
              </div>
            </>
          ) : (
            <>
              <div className="hr"></div>
              <div className="foot-lnk">
                <div className="foot-lnk">
                  <label htmlFor="tab-1">Already Member?</label>
                </div>
                <button className="loginButton" onClick={() => navigate('/login')}>login</button>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
  
};

export default Home;
