<template>
  <div>
    <div v-for="user in currentRoom.users" :key="user">
      <a href="#">{{ user }}</a>
    </div>

    <div>
      {{ currentRoom }}
    </div>

    <button v-if="currentRoom.isAdmin" @click="startGame">Start Game</button>
    <button @click="getRoomLink">Get Room Link</button>
  </div>
</template>

<script>
import axios from "axios";
export default {
  props: ["currentRoom"],
  methods: {
    startGame() {
      return axios.put(
        `http://localhost:7000/api/rooms/${this.currentRoom.roomId}/start`
      );
    },
    async getRoomLink() {
      var copyText = `http://localhost:7000/join/${this.currentRoom.roomId}`;
      await navigator.clipboard.writeText(copyText);
    },
  },
};
</script>

<style>
</style>