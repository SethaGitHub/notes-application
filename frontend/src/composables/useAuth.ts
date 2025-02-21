import axios from "axios";

import { ref, watchEffect } from "vue";
import { useNotes } from "./useNotes";

const AUTH_API_URL = "http://localhost:5268/api/users";

export function useAuth() {
  const token = ref<string | null>(localStorage.getItem("token"));
  const user = ref<{ email: string; password: string; id?: number }>({
    id: Number(localStorage.getItem("userId")) || undefined,
    email: localStorage.getItem("email") || "",
    password: ""
  });
  const isNewUser = ref(false);
  const message = ref<{ text: string; type: string }>({ text: "", type: "" });

  const { fetchNotes } = useNotes(token, user);

  const setAuthHeader = () => {
    axios.defaults.headers.common["Authorization"] = token.value ? `Bearer ${token.value}` : "";
  };

  const loginUser = async () => {
    try {
      const response = await axios.post(`${AUTH_API_URL}/login`, user.value);
      const get_user = await axios.get(`${AUTH_API_URL}/email/${user.value.email}`);

      token.value = response.data.token as string;
      localStorage.setItem("token", token.value ?? "");
      localStorage.setItem("email", user.value.email);
      localStorage.setItem("userId", get_user.data.id?.toString() || "");
      setAuthHeader();

      await fetchNotes();
      message.value = { text: "Login successful!", type: "text-green-500" };
    } catch (error) {
      message.value = { text: "Login failed. Check your credentials.", type: "text-red-500" };
    }
  };

  const registerUser = async () => {
    try {
      await axios.post(`${AUTH_API_URL}/register`, user.value);
      message.value = { text: "Registration successful! Please log in.", type: "text-green-500" };
      isNewUser.value = false;
    } catch (error) {
      message.value = { text: "Registration failed. Email may already be in use.", type: "text-red-500" };
    }
  };

  const logoutUser = () => {
    token.value = null;
    localStorage.removeItem("token");
    localStorage.removeItem("email");
    localStorage.removeItem("userId");
    delete axios.defaults.headers.common["Authorization"];

    fetchNotes();
    message.value = { text: "Logged out successfully!", type: "text-green-500" };
  };

  watchEffect(setAuthHeader);

  watchEffect(() => {
    if (token.value && user.value.id) {
      fetchNotes();
    }
  });

  return {
    token,
    user,
    isNewUser,
    message,
    loginUser,
    registerUser,
    logoutUser
  };
}
