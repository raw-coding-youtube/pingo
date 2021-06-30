<template>
  <div>
    <RoomCreationComponent v-if="!currentRoom" @createRoom="createRoom" @joinRoom="joinRoom"/>    

    <RoomLobbyComponent v-else-if="!currentRoom.started" :currentRoom="currentRoom"/>

    <RoomGameComponent v-else-if="currentRoom.started" :currentRoom="currentRoom"
    :connection="connection"/>
  </div>
</template>

<script>
import { HubConnectionBuilder } from "@microsoft/signalr";
import RoomCreationComponent from "./components/RoomCreationComponent";
import RoomLobbyComponent from "./components/RoomLobbyComponent";
import RoomGameComponent from "./components/RoomGameComponent";
import axios from "axios";

export default {
  name: "App",
  components: { RoomLobbyComponent, RoomGameComponent, RoomCreationComponent },
  data() {
    return {
      currentRoom: null,

      connection: null,
    };
  },
  async created() {
    this.connection = new HubConnectionBuilder()
      .withUrl("http://localhost:7000/ChatHub")
      .build();

    await this.connection.start();

    if (window.location.pathname.indexOf("/join") === 0) {
      let joinRoomId = window.location.pathname.split("/")[2]; 
      await this.joinRoom(joinRoomId);
      window.history.pushState(null, null, "/");
      return;
    }

    await this.getMyRoom().then((res) => {
      if (res.data) {
        this.joinRoom(res.data.roomId);
      }
    });
  },
  watch: {
    currentRoom: function (newRoom) {
      console.log(newRoom);
    },
  },
  methods: {
    subscribeToRoomEvents() {
      this.connection.on("UserJoined", (userId) => {
        console.log(userId);
        this.currentRoom.users.push(userId);
      });
      this.connection.on("GameStarted", () => {
        this.currentRoom.started = true;
      });
      this.connection.on("TurnUpdated", (userId) => {
        this.currentRoom.drawingUser = userId;
      });
    },
    createRoom() {
      return axios
        .post(
          `http://localhost:7000/api/rooms/?connectionId=${this.connection.connectionId}`
        )
        .then((res) => {
          if (res.data) {
            this.currentRoom = res.data;
            this.subscribeToRoomEvents();
          }
        });
    },
    
    joinRoom(roomId) {
      const path = `http://localhost:7000/api/rooms/${roomId}/join?connectionId=${this.connection.connectionId}`;
      return axios.put(path, null).then((res) => {
        if (res.data) {
          this.currentRoom = res.data;
          this.subscribeToRoomEvents();
        }
      });
    },
    getMyRoom() {
      return axios.get(`http://localhost:7000/api/rooms/my`).then((res) => {
        if (res.data) {
          this.currentRoom = res.data;
        }
        return res;
      });
    },
  },
};
</script>

<style>
.canvas {
  border: 1px solid red;
}
.green {
  background-color: green;
}
</style>