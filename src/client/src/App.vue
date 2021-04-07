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
export default {
  name: "App",
  data() {
    return {
      count: 0,
      screenX: 0,
      screenY: 0,
      clicked: false,
    };
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
      ctx.moveTo(this.screenX, this.screenY);
      this.screenX = event.clientX;
      this.screenY = event.clientY;
      if (event.buttons != 1) return;
      // console.log("in canvas");
      ctx.lineTo(this.screenX, this.screenY);
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
