<template>
  <div>
    <div v-for="user in currentRoom.users" :key="user">
      <a :class="{ green: currentRoom.drawingUser == user }" href="#">{{
        user
      }}</a>
    </div>

    <CanvasComponent
      :hideToolBar="currentRoom.drawingUser != currentRoom.myUserId"
      :initCanvas="initCanvas"
      @paint="sendCoordinate"
      @clearCanvas="clearCanvas"
    />

    <ChatComponent :connector="connector" :guess="guess" />
  </div>
</template>

<script>
import CanvasComponent from "./CanvasComponent";
import ChatComponent from "./ChatComponent";

export default {
  props: ["currentRoom", "connection"],
  components: { CanvasComponent, ChatComponent },
  methods: {
    clearCanvas() {
      this.connection.invoke("SendClearEvent");
    },
    sendCoordinate(data) {
      this.connection.invoke("SendCoordinate", data);
    },
    initCanvas({draw, clear}) {
      this.connection.on("ReceiveCoordinate", draw);

      this.connection.on("ReceiveClearEvent", clear);

      this.connection.invoke("ReDraw");
    },
    connector(fun) {
      this.connection.on("GuessWordResponse", fun);
    },
    guess(func){
      this.connection.invoke("GuessWord",func);
    },
  },
};
</script>

<style>
</style>