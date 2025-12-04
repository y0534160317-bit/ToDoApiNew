import { useState } from "react";
import service from "./service.js";

const apiUrl = "http://localhost:5298";

export default function Register() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleRegister = async (e) => {
    e.preventDefault();
    try {
      await service.register
        ? service.register(username, password)
        : await fetch(`${apiUrl}/auth/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password }),
          }).then((r) => {
            if (!r.ok) throw new Error("register failed");
          });

      alert("ההרשמה הצליחה! כעת ניתן להתחבר");
      window.location.href = "/"; // פשוט רידיירקט לאחר הרשמה
    } catch (err) {
      setError("שגיאה בהרשמה — ייתכן שהמשתמש כבר קיים.");
    }
  };

  return (
    <div className="form-wrapper">
      <h2>הרשמה</h2>

      {error && <p className="error">{error}</p>}

      <form onSubmit={handleRegister}>
        <input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
        />

        <input
          type="password"
          placeholder="סיסמה"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        <button type="submit">צור משתמש</button>
      </form>

      <p>
        כבר יש לך משתמש? <a href="/login">למעבר להתחברות</a>
      </p>
    </div>
  );
}
