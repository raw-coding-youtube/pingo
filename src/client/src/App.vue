<template>
  <div>
    <canvas
      ref="mycanvas"
      class="canvas"
      width="500"
      height="600"
      @mouseover="mouseInsideCanvas"
      @mouseout="mouseOutsideCanvas"
      @mousemove="mouseMoveInCanvas"
      @click="toggleClick"
    >
    </canvas>
  </div>
  <p>X-Coordinate: {{ screenX }}, Y-Coordinate: {{ screenY }}</p>
</template>

<script>
import { HubConnectionBuilder } from "@microsoft/signalr";

console.log("signalR", HubConnectionBuilder);


export default {
  name: "App",
  data() {
    return {
      count: 0,
      screenX: 0,
      screenY: 0,
      clicked: false,
      connection:null
    };
  },
  mounted() {
    console.dir(this.$refs.mycanvas);
  },
  created(){
    this.connection = new HubConnectionBuilder()
      .withUrl("http://localhost:7000/ChatHub")
      .build();

    this.connection.on("ReceiveMessage", function (user, message) {
      console.log(user, message);
    });

    this.connection.on(
      "ReceiveCoordinate",
      (startClientX, startClientY, finshClientX, finshClientY) => {
        console.log(startClientX, startClientY, finshClientX, finshClientY);
        var ctx = this.$refs.mycanvas.getContext("2d");
        ctx.beginPath();
        ctx.moveTo(startClientX, startClientY);// console.log("in canvas");
        ctx.lineTo(finshClientX, finshClientY);

        ctx.stroke();
      }
    );

    var username = "Angelo";
    var messageInfo = "Hello World!";
    this.connection
      .start()
      .then(() => this.connection.invoke("SendMessage", username, messageInfo));

  },
  methods: {
    test() {
      let count = 0;
      console.log("hello world: " + count + "  -- " + this.count++);
    },
    toggleClick(event) {
      console.log(event);
      this.clicked = !this.clicked;
      this.screenX = event.clientX;
      this.screenY = event.clientY;
    },
    mouseInsideCanvas(event) {
      //this.screenX = event.clientX;
      // this.screenY = event.clientY;
      // console.dir(this.$refs.mycanvas);
      console.log("Inside", event);
    },
    mouseOutsideCanvas() {
      console.log("Outside");
    },
    mouseMoveInCanvas(event) {
      var ctx = this.$refs.mycanvas.getContext("2d");
      ctx.beginPath();
      let startX = this.screenX;
      let startY = this.screenY;
      ctx.moveTo(this.screenX, this.screenY);
      let toX = (this.screenX = event.clientX - this.$refs.mycanvas.offsetLeft);
      let toY = (this.screenY = event.clientY - this.$refs.mycanvas.offsetTop);

      const notClicked = event.buttons != 1;
      if (notClicked) return;
      // console.log("in canvas");
      ctx.lineTo(this.screenX, this.screenY);

      this.connection.invoke("SendCoordinate", startX, startY, toX, toY);

      ctx.stroke();
    },
  },
};
</script>

<style>
.canvas {
  border: 1px solid red;
}
</style>
