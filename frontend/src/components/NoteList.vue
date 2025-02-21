<template>
  <div class="flex h-screen w-screen bg-gray-900 text-white">
    <!-- Sidebar: New Note Creation, Search, Sort, and List -->
    <div class="w-1/4 bg-gray-800 p-4 border-r border-gray-700">
      <!-- New Note Button -->
      <button @click="currentNote = { title: `New Note ${notes.length + 1}`, content: '' }" 
              class="w-full py-2 mb-4 bg-gray-700 rounded">
        New Note
      </button>

      <!-- Search Bar -->
      <input v-model="searchQuery" 
             placeholder="Search Notes" 
             class="w-full p-2 mb-4 bg-gray-700 text-white border border-gray-600 rounded" />

      <!-- Sort Dropdown -->
      <select v-model="sortOption" 
              class="w-full p-2 mb-4 bg-gray-700 text-white border border-gray-600 rounded">
        <option value="title">Sort by Title</option>
        <option value="createDate">Sort by Create Date</option>
        <option value="updateDate">Sort by Update Date</option>
      </select>

      <!-- Notes List -->
      <ul>
        <li v-for="note in filteredNotes" :key="note.id" 
            @click="currentNote = { ...note }"
            class="py-2 border-b border-gray-700 cursor-pointer hover:bg-gray-700 p-2 flex justify-between items-center"
            :class="{ 'bg-gray-600': currentNote.id === note.id }">
          <span>{{ note.title }}</span>
          <button v-if="note.id" 
                  @click.stop="deleteNote(note.id)" 
                  class="opacity-0 hover:opacity-100 transition-opacity">
            <span class="text-white hover:text-red-500">🗑️</span>
          </button>
        </li>
      </ul>
    </div>

    <!-- Note Editor -->
    <div class="flex-1 p-4">
      <input v-model="currentNote.title" 
             placeholder="Note Title" 
             class="w-full p-2 mb-4 bg-gray-900 text-white border border-gray-700" />
      <textarea v-model="currentNote.content" 
                class="w-full h-3/4 bg-gray-900 text-white outline-none p-4 border border-gray-700"></textarea>
      <button @click="saveNote" 
              class="py-2 px-4 bg-gray-700 rounded">Save</button>
    </div>

    <!-- User Authentication Section -->
    <div class="w-1/4 bg-gray-800 p-4 border-l border-gray-700">
      <div v-if="!token">
        <button @click="isNewUser = !isNewUser" 
                class="w-full py-2 bg-gray-700 rounded mb-4">
          {{ isNewUser ? "Already have an account?" : "I am a new user" }}
        </button>
        <input v-model="user.email" 
               type="email" 
               placeholder="Email" 
               class="w-full p-2 mb-2 bg-gray-700 border border-gray-600 text-white">
        <input v-model="user.password" 
               type="password" 
               placeholder="Password" 
               class="w-full p-2 mb-2 bg-gray-700 border border-gray-600 text-white">
        <button v-if="isNewUser" 
                @click="enhancedRegisterUser" 
                class="w-full py-2 bg-gray-600 rounded">Sign Up</button>
        <button v-else 
                @click="enhancedLoginUser" 
                class="w-full py-2 bg-gray-600 rounded">Login</button>
        <div v-if="alertMessage" 
             class="text-sm mt-2 bg-red-500 text-white p-2 rounded">{{ alertMessage }}</div>
      </div>
      <div v-else>
        <p>Logged in as {{ user.email }}</p>
        <button @click="enhancedLogoutUser" 
                class="w-full py-2 mt-4 bg-red-600 rounded">Logout</button>
        <div v-if="alertMessage" 
             class="text-sm mt-2 bg-green-500 text-white p-2 rounded">{{ alertMessage }}</div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue';
import { useAuth } from "@/composables/useAuth";
import { useNotes } from "@/composables/useNotes";

const { token, user, isNewUser, loginUser, registerUser, logoutUser } = useAuth();
const { notes, currentNote, saveNote, deleteNote , fetchNotes} = useNotes();

const alertMessage = ref('');

const enhancedLoginUser = async () => {
  try {
    await loginUser();
    alertMessage.value = "Login successful!";
    fetchNotes();
    setTimeout(() => alertMessage.value = "", 3000);
  } catch (error) {
    alertMessage.value = "Login failed. Please try again.";
    setTimeout(() => alertMessage.value = "", 3000);
  }
};

const enhancedRegisterUser = async () => {
  try {
    await registerUser();
    alertMessage.value = "Registration successful!";
    setTimeout(() => alertMessage.value = "", 3000);
  } catch (error) {
    alertMessage.value = "Registration failed. Please try again.";
    setTimeout(() => alertMessage.value = "", 3000);
  }
};

const enhancedLogoutUser = () => {
  logoutUser();
  fetchNotes();
  alertMessage.value = "You have been logged out.";
  setTimeout(() => alertMessage.value = "", 3000);
};

const searchQuery = ref('');
const sortOption = ref('title');

const filteredNotes = computed(() => {
  const validNotes = Array.isArray(notes.value) ? notes.value : [];
  let filtered = validNotes.filter(note => note.title && note.title.toLowerCase().includes(searchQuery.value.toLowerCase()));

  filtered.sort((a, b) => {
    if (sortOption.value === 'title') {
      return a.title.localeCompare(b.title);
    } else if (sortOption.value === 'createDate') {
      const aCreateDate = a.createdAt ? new Date(a.createdAt) : new Date(0);
      const bCreateDate = b.createdAt ? new Date(b.createdAt) : new Date(0);
      return aCreateDate - bCreateDate;
    } else if (sortOption.value === 'updateDate') {
      const aUpdateDate = a.updatedAt ? new Date(a.updatedAt) : new Date(0);
      const bUpdateDate = b.updatedAt ? new Date(b.updatedAt) : new Date(0);
      return bUpdateDate - aUpdateDate;
    }
    return 0;
  });

  return filtered;
});
</script>




