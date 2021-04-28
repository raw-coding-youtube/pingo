<template>
  <div>
    <CanvasComponent
      v-if="roomId > 0"
      @paint="sendCoordinate"
      @InitCanvas="initCanvas"
      @clearCanvas="clearCanvas"
    />
    <form id="roomForm" @submit.prevent="createRoom">
      <label>Id:</label>
      <input type="number" v-model="newRoomId" />

      <button type="submit">Create Room</button>
    </form>
    <div v-for="room in rooms" :key="room.id">{{ room }}</div>
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
      count: 0,
      clicked: false,
      connection: null,
      canvasFunctions: null,
      newRoomId: 0,
      rooms: [],
    };
  },
  created() {
    this.loadRooms();
  },
  mounted() {
    this.connection = new HubConnectionBuilder()
      .withUrl("http://localhost:7000/ChatHub")
      .build();

    this.connection.on("ReceiveCoordinate", this.canvasFunctions.draw);

    this.connection.on("ReceiveClearEvent", this.canvasFunctions.clear);

    this.connection.start();
  },
  methods: {
    clearCanvas() {
      this.connection.invoke("SendClearEvent");
    },
    sendCoordinate(data) {
      this.connection.invoke(
        "SendCoordinate",
        data.xStartPosition,
        data.yStartPosition,
        data.toX,
        data.toY,
        data.color,
        data.pickerValue
      );
    },
    initCanvas(canvasFunctions) {
      this.canvasFunctions = canvasFunctions;
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
        })
        .catch((err) => {
          console.error(err);
        });
    },
  },
};
</script>

<style>
.canvas {
  border: 1px solid red;
}
</style>
