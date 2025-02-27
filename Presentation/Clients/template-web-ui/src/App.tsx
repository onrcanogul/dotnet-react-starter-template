import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Auth from "./pages/auth/Auth";
import Home from "./pages/home/Home";
import HeaderComponent from "./pages/home/components/Header";
import { ToastContainer } from "react-toastify";
import { useEffect } from "react";
import {
  isAuthenticated,
  loginWithRefreshtoken,
} from "./pages/auth/services/auth-services";

function App() {
  useEffect(() => {
    console.log(isAuthenticated());
    if (localStorage.getItem("accessToken"))
      if (!isAuthenticated())
        loginWithRefreshtoken(localStorage.getItem("refreshToken"));
  }, []);
  return (
    <Router>
      <ToastContainer />
      <HeaderComponent />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/auth" element={<Auth />} />
      </Routes>
    </Router>
  );
}

export default App;
