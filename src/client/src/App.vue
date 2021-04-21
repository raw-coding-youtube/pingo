<template>
  <div>
    <CanvasComponent
      :color="color"
      :pickerValue="pickerValue"
      @SendCoordinate="sendCoordinate"
      @InitCanvas="initCanvas"
    />
    <div>
      <button @click="clearCanvas">clear</button>
      <input ref="colorBtn" type="color" v-model="color" />
      <input type="range" min="1" max="100" v-model="pickerValue" />
    </div>
  </div>
  <p>
    X-Coordinate: {{ screenX }}, Y-Coordinate: {{ screenY }}, Colour:
    {{ color }}
  </p>
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
      screenX: 0,
      screenY: 0,
      clicked: false,
      connection: null,
      color: "black",
      pickerValue: 1,
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
      this.canvasFunctions.clear();
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
