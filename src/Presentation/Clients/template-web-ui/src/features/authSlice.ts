import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { jwtDecode } from "jwt-decode";
import api from "../api/axiosInstance";
import { clearTokens, getAccessToken, setTokens } from "../api/tokenStorage";
import i18n from "../utils/i18n";
import ToastrService from "../utils/toastr";
import { DecodedToken, Token } from "../domain/token/token";
import { apiErrorMessage } from "../api/apiError";

interface AuthUser {
  userId: string;
  username: string;
}

interface AuthState {
  user: AuthUser | null;
  isAuthenticated: boolean;
  loading: boolean;
}

const initialState: AuthState = {
  user: null,
  isAuthenticated: false,
  loading: false,
};

/**
 * The identity lives inside the access token, so it is read from there rather
 * than from the login response body - which carries the token pair, not a user.
 */
const readUser = (accessToken: string): AuthUser | null => {
  try {
    const decoded = jwtDecode<DecodedToken>(accessToken);
    if (decoded.exp <= Math.floor(Date.now() / 1000)) return null;
    return { userId: decoded.userId, username: decoded.name };
  } catch {
    return null;
  }
};

export const login = createAsyncThunk(
  "auth/login",
  async (
    credentials: { usernameOrEmail: string; password: string },
    thunkAPI
  ) => {
    try {
      const response = await api.post<{ data: Token }>("/user/login", credentials);
      const { accessToken, refreshToken } = response.data.data;
      setTokens(accessToken, refreshToken);
      ToastrService.success(i18n.t("loginSuccess"));
      return accessToken;
    } catch (error) {
      const message = apiErrorMessage(error, i18n.t("loginError"));
      ToastrService.error(message);
      return thunkAPI.rejectWithValue(message);
    }
  }
);

/**
 * Access tokens are stateless, so signing out is purely local. There is no
 * server call to make - an earlier version posted to /auth/logout, which does
 * not exist, and so left the user signed in whenever it 404'd.
 */
export const logout = createAsyncThunk("auth/logout", async () => {
  clearTokens();
  ToastrService.success(i18n.t("logoutSuccess"));
});

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    /** Rehydrates state from the stored token on page load. */
    checkAuth: (state) => {
      const token = getAccessToken();
      const user = token ? readUser(token) : null;
      state.user = user;
      state.isAuthenticated = user !== null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(login.pending, (state) => {
        state.loading = true;
      })
      .addCase(login.fulfilled, (state, action) => {
        state.loading = false;
        state.user = readUser(action.payload);
        state.isAuthenticated = state.user !== null;
      })
      .addCase(login.rejected, (state) => {
        state.loading = false;
        state.isAuthenticated = false;
        state.user = null;
      })
      .addCase(logout.fulfilled, (state) => {
        state.isAuthenticated = false;
        state.user = null;
      });
  },
});

export const { checkAuth } = authSlice.actions;
export default authSlice.reducer;
