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
    </canvas
    ><br />
    <button @click="clearCanvas">clear</button>
    <input ref="colorBtn" type="color" v-model="color" />
    <button @click="ChangeToMedium">Medium</button>
    <div>
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

console.log("signalR", HubConnectionBuilder);

export default {
  name: "App",
  data() {
    return {
      count: 0,
      screenX: 0,
      screenY: 0,
      clicked: false,
      connection: null,
      color: "black",
      pickerValue: 1,
    };
  },
  mounted() {
    console.dir(this.$refs.mycanvas);
    console.log(this.$refs.mycanvas.getContext("2d"));
  },
  created() {
    this.connection = new HubConnectionBuilder()
      .withUrl("http://localhost:7000/ChatHub")
      .build();

    this.connection.on("ReceiveMessage", function (user, message) {
      console.log(user, message);
    });

    this.connection.on(
      "ReceiveCoordinate",
      (
        x,
        y,
        color,
        pickerValue
      ) => {
        var ctx = this.$refs.mycanvas.getContext("2d");
        ctx.fillStyle = color;

        ctx.beginPath();
        ctx.arc(x, y, pickerValue, 0, 2 * Math.PI);
        ctx.fill();
      }
    );

    this.connection.on("ReceiveClearEvent", () => {
      var ctx = this.$refs.mycanvas.getContext("2d");
      ctx.clearRect(
        0,
        0,
        this.$refs.mycanvas.clientWidth,
        this.$refs.mycanvas.clientHeight
      );
    });

    var username = "Angelo";
    var messageInfo = "Hello World!";

    this.connection
      .start()
      .then(() => this.connection.invoke("SendMessage", username, messageInfo));
  },
  methods: {
    test() {},
    toggleClick(event) {
      console.log(event);
      this.clicked = !this.clicked;
      this.screenX = event.clientX;
      this.screenY = event.clientY;
    },
    mouseInsideCanvas() {
      //this.screenX = event.clientX;
      // this.screenY = event.clientY;
      // console.dir(this.$refs.mycanvas);
    },
    mouseOutsideCanvas() {},
    mouseMoveInCanvas(event) {
      var ctx = this.$refs.mycanvas.getContext("2d");
      // ctx.lineWidth = this.pickerValue;
      ctx.strokeStyle = this.color;
      ctx.fillStyle = this.color;

      const notClicked = event.buttons != 1;
      if (notClicked) return;
      // console.log(event.clientX, event.clientY);
      ctx.beginPath();
      let x = event.clientX - this.$refs.mycanvas.offsetLeft;
      let y = event.clientY - this.$refs.mycanvas.offsetTop;
      ctx.arc(x, y, this.pickerValue, 0, 2 * Math.PI);

      // let startX = this.screenX;
      // let startY = this.screenY;
      // ctx.moveTo(this.screenX, this.screenY);
      // let toX = (this.screenX = event.clientX - this.$refs.mycanvas.offsetLeft);
      // let toY = (this.screenY = event.clientY - this.$refs.mycanvas.offsetTop);

      // const notClicked = event.buttons != 1;
      // if (notClicked) return;
      // ctx.lineTo(this.screenX, this.screenY);

      this.connection.invoke(
        "SendCoordinate",
        x,
        y,
        this.color,
        parseInt(this.pickerValue)
      );

      ctx.fill();
    },
    clearCanvas() {
      var ctx = this.$refs.mycanvas.getContext("2d");
      ctx.clearRect(
        0,
        0,
        this.$refs.mycanvas.clientWidth,
        this.$refs.mycanvas.clientHeight
      );

      this.connection.invoke("SendClearEvent");
    },
    ChangeToMedium() {
      var ctx = this.$refs.mycanvas.getContext("2d");
      var value = 20;
      ctx.lineWidth = value;
    },
  },
};
</script>

<style>
.canvas {
  border: 1px solid red;
}
</style>
