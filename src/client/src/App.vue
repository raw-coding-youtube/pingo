<template>
  <div>
    <template v-if="!currentRoom">
      <form id="roomForm" @submit.prevent="createRoom">
        <label>Id:</label>
        <input type="number" v-model="newRoomId" />

        <button type="submit">Create Room</button>
      </form>
      <div v-for="room in rooms" :key="room.id">
        <a href="#" @click="joinRoom(room.id)">{{ room.id }}</a>
      </div>
    </template>

    <template v-else-if="!currentRoom.started">
      <div v-for="user in currentRoom.users" :key="user">
        <a href="#">{{ user }}</a>
      </div>
      <button v-if="currentRoom.isAdmin" @click="startGame">Start Game</button>
    </template>

    <template v-else-if="currentRoom.started">
      <div v-for="user in currentRoom.users" :key="user">
        <a :class="{ green: currentRoom.drawingUser == user }" href="#">{{
          user
        }}</a>
      </div>

      <CanvasComponent
        :hideToolBar="currentRoom.drawingUser != currentRoom.myUserId"
        @paint="sendCoordinate"
        @clearCanvas="clearCanvas"
        @InitCanvas="initCanvas"
      />
      <ChatComponent :connector="connector" :guess="guess" />
    </template>
  </div>
</template>

<script>
import { HubConnectionBuilder } from "@microsoft/signalr";
import CanvasComponent from "./components/CanvasComponent";
import ChatComponent from "./components/ChatComponent";
import axios from "axios";

export default {
  name: "App",
  components: { CanvasComponent, ChatComponent },
  data() {
    return {
      currentRoom: null,
      newRoomId: 0,
      rooms: [],

      connection: null,
      canvasFunctions: null,
    };
  },
  async created() {
    this.connection = new HubConnectionBuilder()
      .withUrl("http://localhost:7000/ChatHub")
      .build();

    await this.connection.start();

    await this.getMyRoom().then((res) => {
      if (res.data) {
        this.joinRoom(res.data.roomId);
      }
    });

    this.loadRooms();
  },
  watch: {
    currentRoom: function (newRoom) {
      console.log(newRoom);
    },
  },
  methods: {
    connector(fun) {
      this.connection.on("GuessWordResponse", fun);
    },
    guess(func){
      this.connection.invoke("GuessWord",func);
    },
    clearCanvas() {
      this.connection.invoke("SendClearEvent");
    },
    sendCoordinate(data) {
      this.connection.invoke("SendCoordinate", data);
    },
    initCanvas(canvasFunctions) {
      this.canvasFunctions = canvasFunctions;

      this.connection.on("ReceiveCoordinate", this.canvasFunctions.draw);

      this.connection.on("ReceiveClearEvent", this.canvasFunctions.clear);

      this.connection.invoke("ReDraw");
    },
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
          `http://localhost:7000/api/rooms/${this.newRoomId}?connectionId=${this.connection.connectionId}`
        )
        .then((res) => {
          if (res.data) {
            this.currentRoom = res.data;
            this.subscribeToRoomEvents();
          }
        });
    },
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
    joinRoom(roomId) {
      const path = `http://localhost:7000/api/rooms/${roomId}/join?connectionId=${this.connection.connectionId}`;
      return axios.put(path, null).then((res) => {
        if (res.data) {
          this.currentRoom = res.data;
          this.subscribeToRoomEvents();
        }
      });
    },

    startGame() {
      return axios.put(
        `http://localhost:7000/api/rooms/${this.currentRoom.roomId}/start`
      );
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