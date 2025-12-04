import React, { useState } from 'react';
import service from './service.js';

export default function Login({ onLogin }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e) {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      await service.login(username, password);
      setLoading(false);
      onLogin && onLogin();
    } catch (err) {
      setLoading(false);
      console.error('login error', err.response?.data || err);
      setError(err.response?.data?.message || JSON.stringify(err.response?.data) || 'Login failed');
    }
  }

  return (
    <div className="login-page" style={{ padding: 20 }}>
      <form onSubmit={handleSubmit} className="login-form">
        <h2>Sign in</h2>
        {error && <div style={{ color: 'red', marginBottom: 8 }}>{error}</div>}
        <div>
          <input
            placeholder="Username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            style={{ display: 'block', width: 300, marginBottom: 8 }}
          />
        </div>
        <div>
          <input
            type="password"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            style={{ display: 'block', width: 300, marginBottom: 12 }}
          />
        </div>
        <button type="submit" disabled={loading}>
          {loading ? 'Signing in...' : 'Sign in'}
        </button>
      </form>
    </div>
  );
}
