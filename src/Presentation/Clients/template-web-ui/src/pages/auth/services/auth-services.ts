import api from "../../../api/axiosInstance";
import { apiErrorMessage } from "../../../api/apiError";
import {
  clearTokens,
  getAccessToken,
  getRefreshToken,
  setTokens,
} from "../../../api/tokenStorage";
import ToastrService from "../../../utils/toastr";
import i18n from "../../../utils/i18n";
import { jwtDecode } from "jwt-decode";
import { DecodedToken, Token } from "../../../domain/token/token";

/**
 * Auth calls that are not part of the Redux flow.
 *
 * Signing in and out live in `features/authSlice` - keeping a second copy here
 * is how the two drifted apart before.
 */

export const isAuthenticated = (): boolean => {
  const token = getAccessToken();
  if (!token) return false;
  try {
    return jwtDecode<DecodedToken>(token).exp > Math.floor(Date.now() / 1000);
  } catch {
    return false;
  }
};

export const getCurrentUser = () => {
  const token = getAccessToken();
  if (!token) return null;
  try {
    const decoded = jwtDecode<DecodedToken>(token);
    return { userId: decoded.userId, username: decoded.name };
  } catch {
    return null;
  }
};

/**
 * Trades the stored refresh token for a fresh pair on start-up.
 * Returns whether the session was restored; the caller decides what to do next.
 */
export const loginWithRefreshToken = async (): Promise<boolean> => {
  const refreshToken = getRefreshToken();
  if (!refreshToken) return false;

  try {
    // The token goes in the body, never the URL - paths end up in proxy and
    // access logs.
    const response = await api.post<{ data: Token }>("/user/refresh-token-login", {
      refreshToken,
    });
    const { accessToken, refreshToken: renewed } = response.data.data;
    setTokens(accessToken, renewed);
    return true;
  } catch {
    clearTokens();
    return false;
  }
};

export const register = async (
  userName: string,
  fullName: string,
  email: string,
  password: string,
  confirmPassword: string
) => {
  try {
    const response = await api.post("/user/register", {
      userName,
      fullName,
      email,
      password,
      confirmPassword,
    });
    ToastrService.success(i18n.t("registerSuccess"));
    return response.data;
  } catch (error) {
    ToastrService.error(apiErrorMessage(error, i18n.t("registerError")));
    return null;
  }
};
