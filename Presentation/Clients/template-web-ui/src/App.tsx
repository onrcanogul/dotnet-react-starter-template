import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Auth from "./pages/auth/Auth";
import Home from "./pages/home/Home";
import HeaderComponent from "./pages/home/components/Header";
import { ToastContainer } from "react-toastify";
import { useEffect } from "react";
import {
  loginWithRefreshtoken,
  logout,
} from "./pages/auth/services/auth-services";
import { useDispatch, useSelector } from "react-redux";
import { checkAuth } from "./features/authSlice";
import { RootState } from "./store";

function App() {
  const dispatch = useDispatch();
  const isAuthenticated = useSelector(
    (state: RootState) => state.auth.isAuthenticated
  );

  useEffect(() => {
    dispatch(checkAuth());
    checkLogin();
  }, [dispatch, isAuthenticated]);

  const checkLogin = async () => {
    const refreshToken = localStorage.getItem("refreshToken");
    if (!isAuthenticated && refreshToken) {
      await loginWithRefreshtoken(refreshToken);
    }
  };

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
