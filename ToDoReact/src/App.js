import React, { useEffect, useState } from 'react';
import Login from './Login.jsx';
import service from './service.js';

function App() {
  const [newTodo, setNewTodo] = useState("");
  const [todos, setTodos] = useState([]);
  const [isAuthenticated, setIsAuthenticated] = useState(service.isLoggedIn());

  async function getTodos() {
    if (!isAuthenticated) return;
    try {
      const todos = await service.getTasks();
      setTodos(todos);
    } catch (err) {
      console.error('getTodos error', err);
      if (err.response?.status === 401) {
        // token invalid or expired - sign out locally
        service.logout();
        setIsAuthenticated(false);
      }
    }
  }

  async function createTodo(e) {
    e.preventDefault();
    if (!newTodo.trim()) return;
    try {
      await service.addTask(newTodo.trim());
      setNewTodo(""); // clear input
      await getTodos(); // refresh tasks list
    } catch (err) {
      console.error('createTodo error', err);
      if (err.response?.status === 401) {
        service.logout();
        setIsAuthenticated(false);
      }
    }
  }

  async function updateCompleted(todo, isComplete) {
    try {
      await service.setCompleted(todo.id, isComplete);
      await getTodos(); // refresh tasks list
    } catch (err) {
      console.error('updateCompleted error', err);
      if (err.response?.status === 401) {
        service.logout();
        setIsAuthenticated(false);
      }
    }
  }

  async function deleteTodo(id) {
    try {
      await service.deleteTask(id);
      await getTodos(); // refresh tasks list
    } catch (err) {
      console.error('deleteTodo error', err);
      if (err.response?.status === 401) {
        service.logout();
        setIsAuthenticated(false);
      }
    }
  }

  async function handleLogin() {
    setIsAuthenticated(true);
    await getTodos();
  }

  function handleLogout() {
    service.logout();
    setIsAuthenticated(false);
    setTodos([]);
  }

  useEffect(() => {
    getTodos();
  }, [isAuthenticated]);

  if (!isAuthenticated) {
    return <Login onLogin={handleLogin} />;
  }

  return (
    <section className="todoapp">
      <header className="header">
        <h1>todos</h1>
        <button onClick={handleLogout} style={{ position: 'absolute', right: 20, top: 20 }}>Logout</button>
        <form onSubmit={createTodo}>
          <input className="new-todo" placeholder="Well, let's take on the day" value={newTodo} onChange={(e) => setNewTodo(e.target.value)} />
        </form>
      </header>
      <section className="main" style={{ display: "block" }}>
        <ul className="todo-list">
          {todos.map(todo => {
            return (
              <li className={todo.isComplete ? "completed" : ""} key={todo.id}>
                <div className="view">
                  <input className="toggle" type="checkbox" defaultChecked={todo.isComplete} onChange={(e) => updateCompleted(todo, e.target.checked)} />
                  <label>{todo.name}</label>
                  <button className="destroy" onClick={() => deleteTodo(todo.id)}></button>
                </div>
              </li>
            );
          })}
        </ul>
      </section>
    </section >
  );
}

export default App;