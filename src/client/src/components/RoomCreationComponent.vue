<template >
  <div>
    <form id="roomForm" @submit.prevent="$emit('createRoom', null)">
      <button type="submit">Create Room</button>
    </form>
    <div v-for="room in rooms" :key="room.id">
      <a href="#" @click="$emit('joinRoom', room.id)">{{ room.id }}</a>
    </div>
  </div>
</template>

<script>
import axios from "axios";
export default {
  data() {
    return {
      rooms: [],
    };
  },
  created() {
    return this.loadRooms();
  },
  methods: {
    loadRooms() {
      return axios
        .get("http://localhost:7000/api/rooms")
        .then((res) => {
          this.rooms = res.data;
        })
        .catch((err) => {
          console.error(err);
        });
    },
  },
};
</script>

<style>
</style>