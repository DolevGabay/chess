import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { BoardUtils } from './BoardUtils';
import './Home.css';

const Login = () => {
  const navigate = useNavigate();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  
  const handleSignIn = async () => {
    try {
        const Username = await BoardUtils.login(username, password, 'signin');        
        if (Username === 'Invalid username or password') {
            alert('Invalid username or password');
            return;
        }
        navigate(`/?username=${Username}`);
    } catch (error) {
        console.error('Error:', error);
    }
  };

  const handleSignUp = async () => {
    try {
        
        const Username = await BoardUtils.login(username, password, 'signup');
        console.log('Username:', Username);
        navigate(`/?username=${Username}`);
    } catch (error) {
        console.error('Error:', error);
    }
  };

  const handleUsernameChange = (event) => {
    setUsername(event.target.value);
  };

  const handlePasswordChange = (event) => {
    setPassword(event.target.value);
  };

  return (
    <div className="login-wrap">
      <div className="login-html">
        <input id="tab-1" type="radio" name="tab" className="sign-in" defaultChecked />
        <label htmlFor="tab-1" className="tab">Sign In</label>
        <input id="tab-2" type="radio" name="tab" className="sign-up" />
        <label htmlFor="tab-2" className="tab">Sign Up</label>
        <div className="login-form">
          <div className="sign-in-htm">
            <div className="group">
              <label htmlFor="userSignIn" className="label">Username</label>
              <input id="userSignIn" type="text" className="input" value={username} onChange={handleUsernameChange} />
            </div>
            <div className="group">
              <label htmlFor="passSignIn" className="label">Password</label>
              <input id="passSignIn" type="password" className="input" data-type="password" value={password} onChange={handlePasswordChange} />
            </div>
            <div className="group">
              <input type="submit" className="button" value="Sign In" onClick={handleSignIn} />
            </div>
            <div className="hr"></div>
          </div>
          <div className="sign-up-htm">
            <div className="group">
              <label htmlFor="userSignUp" className="label">Username</label>
              <input id="userSignUp" type="text" className="input" value={username} onChange={handleUsernameChange} />
            </div>
            <div className="group">
              <label htmlFor="passSignUp" className="label">Password</label>
              <input id="passSignUp" type="password" className="input" data-type="password" value={password} onChange={handlePasswordChange} />
            </div>
            <div className="group">
              <input type="submit" className="button" value="Sign Up" onClick={handleSignUp} />
            </div>
            <div className="hr"></div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
