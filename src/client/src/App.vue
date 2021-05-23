<template>
  <div>
    <template v-if="roomId > 0">
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
    <template v-else>
      <form id="roomForm" @submit.prevent="createRoom">
        <label>Id:</label>
        <input type="number" v-model="newRoomId" />

        <button type="submit">Create Room</button>
      </form>
      <div>
        <select v-model="selectedRoomId">
          <option v-for="room in rooms" :key="room.id">{{ room.id }}</option>
        </select>
        {{ selectedRoomId }}
        <button type="submit" @click="joinRoom">Join room</button>
      </div>
      <div v-for="room in rooms" :key="room.id">
        <a href="#">{{ room }}</a>
      </div>
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
      roomId: 0,
      connection: null,
      canvasFunctions: null,
      newRoomId: 0,
      selectedRoomId: null,
      rooms: [],
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

    await axios
      .get(`http://localhost:7000/api/rooms/my`, {
        withCredentials: true,
      })
      .then((res) => {
        if (res.data) {
          this.selectedRoomId = res.data.id;
          this.joinRoom();
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
      console.log(this.newRoomId);
      axios
        .post(`http://localhost:7000/api/rooms/${this.newRoomId}`)
        .then(this.loadRooms);
    },
    loadRooms() {
      axios
        .get("http://localhost:7000/api/rooms")
        .then((res) => {
          this.rooms = res.data;
          if (this.rooms.length > 0) {
            this.selectedRoomId = this.rooms[0].id;
          }
        })
        .catch((err) => {
          console.error(err);
        });
    },
    joinRoom() {
      console.log("joinRoom", this.selectedRoomId);
      this.connection.invoke("JoinRoom", parseInt(this.selectedRoomId));
    },
    joinResponse(roomId) {
      console.log("joined room", roomId);
      this.roomId = roomId;
      this.connection.on("GuessWordResponse", this.guessWordResponse);
    },
    guessWordResponse(word, result) {
      this.messages.push({ word, result });
    },
    onWordGuess() {
      this.connection.invoke("GuessWord", this.wordGuess);
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
