import axios from 'axios';

const apiUrl = "http://localhost:5298";

axios.interceptors.response.use(
    response => response, // במקרה שהכול תקין
    error => {
        console.error("API Error:", error);
        return Promise.reject(error); // מחזיר את השגיאה הלאה
    }
);

// אם יש טוקן בשמירת מקומי, נרשום אותו ב־axios כדי שישלח עם כל הבקשות
const _savedToken = localStorage.getItem('token');
if (_savedToken) {
  axios.defaults.headers.common['Authorization'] = `Bearer ${_savedToken}`;
}

export default {
  // אוטנטיקציה
  login: async (username, password) => {
    console.log('service login', {username, password})
    // השרת מצפה לקבל את username ו‑password בפרמטרים של ה‑query — נוסיף אותם ל‑URL
    const url = `${apiUrl}/auth/login?username=${encodeURIComponent(username)}&password=${encodeURIComponent(password)}`;
    const res = await axios.post(url, {});
    const token = res.data?.token || res.data?.accessToken;
    console.log('login success, token:', token);
    if (token) {
     
      localStorage.setItem('token', token);
      axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
    }
    return res;
  },

  register: async (username, password) => {
  const url = `${apiUrl}/auth/register`;
  return await axios.post(url, {
    username: username,
    password: password
  });
},

  // register: async (username, password) => {
  //   // במידה וה־API מצפה ל־username ו‑password ב‑query גם ברישום
  //   const url = `${apiUrl}/auth/register?username=${encodeURIComponent(username)}&password=${encodeURIComponent(password)}`;
  //   return await axios.post(url, {});
  // },

  logout: () => {
    localStorage.removeItem('token');
    delete axios.defaults.headers.common['Authorization'];
  },

  isLoggedIn: () => {
    return !!localStorage.getItem('token');
  },

  getToken: () => localStorage.getItem('token'),

  getTasks: async () => {
    const result = await axios.get(`${apiUrl}/items`)    
    return result.data;
  },

  addTask: async(name)=>{
    console.log('addTask', name)
    return await axios.post(`${apiUrl}/items`, { name });
  },

  setCompleted: async(id, isComplete)=>{
    console.log('setCompleted', {id, isComplete})
    return await axios.put(`${apiUrl}/items/${id}`, { isComplete });
  },

  deleteTask:async(id)=>{
    console.log('deleteTask', id)
    return await axios.delete(`${apiUrl}/items/${id}`);
  }
};
