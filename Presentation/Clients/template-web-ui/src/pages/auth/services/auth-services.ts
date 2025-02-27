import api from "../../../api/axiosInstance";
import ToastrService from "../../../utils/toastr";
import i18n from "../../../utils/i18n";
import { jwtDecode } from "jwt-decode";
import { DecodedToken } from "../../../domain/token/token";

// Login
export const login = async (usernameOrEmail: string, password: string) => {
  try {
    const response = await api.post("/user/login", {
      usernameOrEmail,
      password,
    });
    localStorage.setItem("accessToken", response.data.data.accessToken);
    localStorage.setItem("refreshToken", response.data.data.refreshToken);
    ToastrService.success(i18n.t("successLogin"));
    return response.data.data;
  } catch (error) {
    console.error("Login error:", error);
    ToastrService.error(i18n.t("errorLogin"));
    return null;
  }
};

// Check Authentication
export const isAuthenticated = (): boolean => {
  try {
    debugger;
    const token = localStorage.getItem("accessToken");
    if (!token) return false;

    const decoded: DecodedToken = jwtDecode<DecodedToken>(token);
    const currentTime = Math.floor(Date.now() / 1000);

    return decoded.exp > currentTime;
  } catch (error) {
    console.error("Auth check error:", error);
    return false;
  }
};

// Get Current User
export const getCurrentUser = () => {
  try {
    const token = localStorage.getItem("accessToken");
    if (!token) return null;

    const decoded: DecodedToken = jwtDecode<DecodedToken>(token);
    console.log(decoded);
    return { userId: decoded.userId, username: decoded.name };
  } catch (error) {
    console.error("Error decoding token:", error);
    return null;
  }
};

// Logout
export const logout = async () => {
  try {
    await api.post("/auth/logout");
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    ToastrService.success(i18n.t("logoutSuccess"));
  } catch (error) {
    console.error("Logout error:", error);
    ToastrService.error(i18n.t("logoutError"));
  }
};

// Login with Refresh Token
export const loginWithRefreshtoken = async (refreshToken: string) => {
  try {
    const response = await api.post(
      `/user/refresh-token-login/${refreshToken}`
    );
    ToastrService.success(i18n.t("refreshTokenSuccess"));
    return response.data.data;
  } catch (error) {
    console.error("Refresh token login error:", error);
    ToastrService.error(i18n.t("refreshTokenError"));
    return null;
  }
};

// Register
export const register = async (
  username: string,
  fullName: string,
  email: string,
  password: string,
  confirmPassword: string
) => {
  try {
    const response = await api.post("/user/register", {
      username,
      fullName,
      email,
      password,
      confirmPassword,
    });
    ToastrService.success(i18n.t("successRegister"));
    return response.data.data;
  } catch (error) {
    console.log(error);
    console.error("Register error:", error);
    ToastrService.error(error.response.data.errors[0]);
    return null;
  }
};
