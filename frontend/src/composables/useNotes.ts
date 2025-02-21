import { ref, onMounted, computed } from "vue";
import axios from "axios";

const API_BASE_URL = "http://localhost:5268/api/notes";

export function useNotes(token: any, user: any) {
  const notes = ref<{ id: number; title: string; content: string; userId?: number }[]>([]);
  const currentNote = ref<{ id?: number; title: string; content: string; userId?: number }>({ title: "", content: "" });
  const message = ref<{ text: string; type: string }>({ text: "", type: "" });

  const isLoggedIn = computed(() => token?.value && user?.value?.id);

  const fetchNotes = async () => {
    const token = localStorage.getItem("token");  
    const userId = localStorage.getItem("userId");

    const endpoint = userId ? `/user/${userId}` : "/all";  
    const apiURL = `${API_BASE_URL}${endpoint}`;
  
    try {
      console.log("Fetching notes from:", apiURL);
      const response = await axios.get(apiURL, {
        headers: userId ? { Authorization: `Bearer ${token}` } : {}, 
      });
      notes.value = response.data;
      console.log("Fetched notes:", notes.value);
  
      currentNote.value = notes.value.length > 0 ? { ...notes.value[0] } : { title: "", content: "" };
    } catch (error) {
      console.error("Error fetching notes:", error);
      message.value = { text: "Failed to fetch notes.", type: "text-red-500" };
    }
  };

  const saveNote = async () => {
    if (!currentNote.value.title.trim()) {
      message.value = { text: "Title cannot be empty.", type: "text-red-500" };
      return;
    }
  
    const token = localStorage.getItem("token"); 
    const userId = localStorage.getItem("userId"); 
  
    if (token) {
      console.log("Token found:", userId);
  
      if (userId) {
        currentNote.value.userId = parseInt(userId); 
      }
    } else {
      console.log("No token found");
    }
  
    try {
      if (!currentNote.value.id) {
        const response = await axios.post(API_BASE_URL, currentNote.value, {
          headers: {
            Authorization: `Bearer ${token}`, 
          },
        });
        currentNote.value.id = response.data.id;
      } else {
        await axios.put(`${API_BASE_URL}/${currentNote.value.id}`, currentNote.value, {
          headers: {
            Authorization: `Bearer ${token}`, 
          },
        });
      }
  
      await fetchNotes(); 
      message.value = { text: "Note saved successfully!", type: "text-green-500" };
    } catch (error) {
      message.value = { text: "Error saving note.", type: "text-red-500" };
    }
  };

  const deleteNote = async (id: number) => {
    console.log("Deleting note with id:", id);
    try {
      await axios.delete(`${API_BASE_URL}/${id}`);
      await fetchNotes(); 
      if (currentNote.value.id === id) currentNote.value = { title: "", content: "" };
      message.value = { text: "Note deleted.", type: "text-green-500" };
    } catch (error) {
      message.value = { text: "Error deleting note.", type: "text-red-500" };
    }
  };

  onMounted(fetchNotes);

  return {
    notes,
    currentNote,
    message,
    fetchNotes,
    saveNote,
    deleteNote,
  };
}
