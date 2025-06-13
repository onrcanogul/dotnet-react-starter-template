import { configureStore } from "@reduxjs/toolkit";
import authReducer from "./features/authSlice";

export const store = configureStore({
  reducer: {
    auth: authReducer,
  },
});

// RootState ve AppDispatch tiplerini oluştur
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
