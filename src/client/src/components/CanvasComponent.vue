<template>
  <canvas
    ref="mycanvas"
    class="canvas"
    width="500"
    height="600"
    @mousemove="mouseMoveInCanvas"
    @click="toggleClick"
  >
  </canvas>
</template>

<script>
export default {
  props: ["color", "pickerValue"],
  name: "CanvasComponent",
  methods: {
    mouseMoveInCanvas(event) {
      var ctx = this.$refs.mycanvas.getContext("2d");
      ctx.lineWidth = this.pickerValue;
      ctx.strokeStyle = this.color;
      ctx.fillStyle = this.color;
      ctx.lineCap = "round";

      ctx.beginPath();
      let xStartPosition = this.screenX;
      let yStartPosition = this.screenY;
      ctx.moveTo(this.screenX, this.screenY);
      this.screenX = event.clientX;
      this.screenY = event.clientY;

      const notClicked = event.buttons != 1;
      if (notClicked) return;
      let toX = (this.screenX = event.clientX - this.$refs.mycanvas.offsetLeft);
      let toY = (this.screenY = event.clientY - this.$refs.mycanvas.offsetTop);
      ctx.lineTo(toX, toY);
      let color = this.color;
      let pickerValue = parseInt(this.pickerValue);

      this.$emit("SendCoordinate", {
        xStartPosition,
        yStartPosition,
        toX,
        toY,
        color,
        pickerValue,
      });

      ctx.stroke();
    },
  },
  mounted() {
    let canvas = this.$refs.mycanvas;
    this.$emit("InitCanvas", {
      draw(xStartPosition, yStartPosition, toX, toY, color, pickerValue) {
        var ctx = canvas.getContext("2d");
        ctx.lineWidth = pickerValue;
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