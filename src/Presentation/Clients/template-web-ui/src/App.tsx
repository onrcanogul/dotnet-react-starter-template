import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Auth from "./pages/auth/Auth";
import Home from "./pages/home/Home";
import HeaderComponent from "./pages/home/components/Header";
import { ToastContainer } from "react-toastify";
import { useEffect } from "react";
import { loginWithRefreshToken } from "./pages/auth/services/auth-services";
import { useAppDispatch } from "./hooks";
import { checkAuth } from "./features/authSlice";

function App() {
  const dispatch = useAppDispatch();

  // Restore the session once, on mount. Depending on `isAuthenticated` here
  // re-ran the refresh call every time auth state changed.
  useEffect(() => {
    const restoreSession = async () => {
      dispatch(checkAuth());
      await loginWithRefreshToken();
      dispatch(checkAuth());
    };
    void restoreSession();
  }, [dispatch]);

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
