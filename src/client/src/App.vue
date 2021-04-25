<template>
  <div>
    <CanvasComponent
      @paint="sendCoordinate"
      @InitCanvas="initCanvas"
      @clearCanvas="clearCanvas"
    />
  </div>
</template>

<script>
import { HubConnectionBuilder } from "@microsoft/signalr";
import CanvasComponent from "./components/CanvasComponent";

export default {
  name: "App",
  components: { CanvasComponent },
  data() {
    return {
      count: 0,
      clicked: false,
      connection: null,
      canvasFunctions: null,
    };
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
  },
};
</script>

<style>
.canvas {
  border: 1px solid red;
}
</style>
