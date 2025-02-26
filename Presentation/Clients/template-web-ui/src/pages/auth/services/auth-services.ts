import api from "../../../api/axiosInstance";

//login
export const login = async (usernameOrEmail: string, password: string) => {
  const response = await api.post("/user/login", { usernameOrEmail, password });
  //serviceresponses data from backend
  return response.data.data;
};

//logout
export const logout = async () => {
  await api.post("/auth/logout");
};

export const loginWithRefreshtoken = async (refreshToken: string) => {
  const response = await api.post("/user/refresh-token-login/" + refreshToken);
  //serviceresponses data from backend
  return response.data.data;
};

//register
export const register = async (
  username: string,
  fullName: string,
  email: string,
  password: string,
  confirmPassword: string
) => {
  const response = await api.post("/user/register", {
    username,
    fullName,
    email,
    password,
    confirmPassword,
  });
  //serviceresponses data from backend
  return response.data.data;
};
