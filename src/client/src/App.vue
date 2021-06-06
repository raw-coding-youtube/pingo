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
      <!-- <p>{{ rooms.find((x) => x.id === roomId).word }}</p> -->
      <CanvasComponent
        @paint="sendCoordinate"
        @clearCanvas="clearCanvas"
        @InitCanvas="initCanvas"
      />
      <div v-for="(m, i) in messages" :key="`message-${i}`">
        <p :class="{ green: m.result }">{{ m.word }}</p>
      </div>
      <input v-model="wordGuess" />
      <button @click="onWordGuess">Send</button>
    </template>
  </div>
</template>

<script>
import { HubConnectionBuilder } from "@microsoft/signalr";
import CanvasComponent from "./components/CanvasComponent";
import axios from "axios";

export default {
  name: "App",
  components: { CanvasComponent },
  data() {
    return {
      currentRoom: null,
      newRoomId: 0,
      rooms: [],

      connection: null,
      canvasFunctions: null,
      wordGuess: "",
      messages: [],
    };
  },
  async created() {
    this.connection = new HubConnectionBuilder()
      .withUrl("http://localhost:7000/ChatHub")
      .build();

    this.connection.on("JoinResponse", this.joinResponse);

    await this.connection.start();

    await this.getMyRoom().then((res) => {
      if (res.data) {
        this.joinRoom(res.data.id);
      }
    });

    this.loadRooms();
  },
  methods: {
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
    createRoom() {
      return axios
        .post(`http://localhost:7000/api/rooms/${this.newRoomId}?connectionId=${this.connection.connectionId}`)
        .then((res) => {
          if (res.data) {
            this.currentRoom = res.data;
            this.connection.on("ReloadRoom", (room) => {
              console.log(room);
              this.currentRoom = room;
            });
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
          this.connection.on("ReloadRoom", (room) => {
            console.log(room);
            this.currentRoom = room;
          });
        }
      });
      //this.connection.invoke("JoinRoom", parseInt(roomId));
    },
    joinResponse() {
      this.getMyRoom();
      this.connection.on("ReloadRoom", (room) => (this.currentRoom = room));
      this.connection.on("GuessWordResponse", this.guessWordResponse);
    },
    guessWordResponse(word, result) {
      this.messages.push({ word, result });
    },
    onWordGuess() {
      this.connection.invoke("GuessWord", this.wordGuess);
    },
    startGame() {
      return axios.put(
        `http://localhost:7000/api/rooms/${this.currentRoom.id}/start`
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