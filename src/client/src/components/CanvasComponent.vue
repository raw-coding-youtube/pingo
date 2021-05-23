<template>
  <div>
    <canvas
      ref="mycanvas"
      class="canvas"
      width="500"
      height="600"
      @mousemove="mouseMoveInCanvas"
      @click="toggleClick"
    >
    </canvas>
    <ToolBarComponent @clearCanvas="clear" @init="initToolbar" />
  </div>
</template>

<script>
import ToolBarComponent from "@/components/ToolBarComponent";

export default {
  components: { ToolBarComponent },
  name: "CanvasComponent",
  data() {
    return {
      screenX: 0,
      screenY: 0,
      color: "black",
      brushSize: 1,
      tools: null,
    };
  },
  methods: {
    clear() {
      var ctx = this.$refs.mycanvas.getContext("2d");
      ctx.clearRect(
        0,
        0,
        this.$refs.mycanvas.clientWidth,
        this.$refs.mycanvas.clientHeight
      );
      this.$emit("clearCanvas", null);
    },
    initToolbar(tools) {
      this.tools = tools;
    },
    mouseMoveInCanvas(event) {
      let color = this.tools.getColor();
      let brushSize = this.tools.getBrushSize();
      var ctx = this.$refs.mycanvas.getContext("2d");
      ctx.lineWidth = brushSize;
      ctx.strokeStyle = color;
      ctx.fillStyle = color;
      ctx.lineCap = "round";

      ctx.beginPath();
      let xStartPosition = this.screenX;
      let yStartPosition = this.screenY;
      ctx.moveTo(this.screenX, this.screenY);
      this.screenX = event.clientX - this.$refs.mycanvas.offsetLeft;
      this.screenY = event.clientY - this.$refs.mycanvas.offsetTop;

      const notClicked = event.buttons != 1;
      if (notClicked) return;
      let toX = (this.screenX = event.clientX - this.$refs.mycanvas.offsetLeft);
      let toY = (this.screenY = event.clientY - this.$refs.mycanvas.offsetTop);
      ctx.lineTo(toX, toY);

      this.$emit("paint", {
        xStartPosition,
        yStartPosition,
        toX,
        toY,
        color,
        brushSize: parseInt(brushSize),
      });

      ctx.stroke();
    },
  },
  mounted() {
    let canvas = this.$refs.mycanvas;
    this.$emit("InitCanvas", {
      draw({ xStartPosition, yStartPosition, toX, toY, color, brushSize }) {
        var ctx = canvas.getContext("2d");
        ctx.lineWidth = brushSize;
        ctx.strokeStyle = color;
        ctx.lineCap = "round";

        ctx.beginPath();
        ctx.moveTo(xStartPosition, yStartPosition);
        ctx.lineTo(toX, toY);
        ctx.stroke();
      },
      clear() {
        var ctx = canvas.getContext("2d");
        ctx.clearRect(0, 0, canvas.clientWidth, canvas.clientHeight);
      },
    });
  },
};
</script>

<style>
</style>