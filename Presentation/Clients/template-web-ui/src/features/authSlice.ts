import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import api from "../api/axiosInstance";
import i18n from "../utils/i18n";
import ToastrService from "../utils/toastr";
import { DecodedToken } from "../domain/token/token";
import { jwtDecode } from "jwt-decode";

interface AuthState {
  user: { userId: string; username: string } | null;
  isAuthenticated: boolean;
  loading: boolean;
}

const initialState: AuthState = {
  user: null,
  isAuthenticated: false,
  loading: false,
};

export const login = createAsyncThunk(
  "auth/login",
  async (
    {
      usernameOrEmail,
      password,
    }: { usernameOrEmail: string; password: string },
    thunkAPI
  ) => {
    try {
      const response = await api.post("/user/login", {
        usernameOrEmail,
        password,
      });
      localStorage.setItem("accessToken", response.data.data.accessToken);
      localStorage.setItem("refreshToken", response.data.data.refreshToken);
      ToastrService.success(i18n.t("loginSuccess"));
      return response.data.data;
    } catch (error) {
      console.error("Login error:", error);
      ToastrService.error(i18n.t("loginError"));
      return thunkAPI.rejectWithValue(error.response?.data || "Login failed");
    }
  }
);

export const logout = createAsyncThunk("auth/logout", async (_, thunkAPI) => {
  try {
    await api.post("/auth/logout");
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    ToastrService.success(i18n.t("logoutSuccess"));
    return true;
  } catch (error) {
    console.error("Logout error:", error);
    ToastrService.error(i18n.t("logoutError"));
    return thunkAPI.rejectWithValue(error.response?.data || "Logout failed");
  }
});

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    checkAuth: (state) => {
      const token = localStorage.getItem("accessToken");
      if (token) {
        try {
          const decoded: DecodedToken = jwtDecode<DecodedToken>(token);
          const currentTime = Math.floor(Date.now() / 1000);
          if (decoded.exp > currentTime) {
            state.isAuthenticated = true;
            state.user = { userId: decoded.userId, username: decoded.name };
          } else {
            state.isAuthenticated = false;
            state.user = null;
          }
        } catch (error) {
          state.isAuthenticated = false;
          state.user = null;
        }
      } else {
        state.isAuthenticated = false;
        state.user = null;
      }
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(login.pending, (state) => {
        state.loading = true;
      })
      .addCase(login.fulfilled, (state, action) => {
        state.loading = false;
        state.isAuthenticated = true;
        state.user = {
          userId: action.payload.userId,
          username: action.payload.username,
        };
      })
      .addCase(login.rejected, (state) => {
        state.loading = false;
        state.isAuthenticated = false;
      })
      .addCase(logout.fulfilled, (state) => {
        state.isAuthenticated = false;
        state.user = null;
      });
  },
});

export const { checkAuth } = authSlice.actions;
export default authSlice.reducer;
