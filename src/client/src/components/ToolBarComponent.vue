<template>
  <div>
    <div>
      <div>
        <button @click="clearCanvas">clear</button>
        <input ref="colorBtn" type="color" v-model="color" />
        <input type="range" min="1" max="100" v-model="brushSize" />
      </div>
    </div>
    <p>Colour: {{ color }}</p>
  </div>
</template>

<script>
// SendColour is names badly, because we are sending tools
export default {
  data: () => ({
    color: "black",
    brushSize: 1,
  }),
  methods: {
    clearCanvas() {
      this.$emit("clearCanvas", null);
    },
  },
  mounted() {
    this.$emit("init", {
      getColor: () => {
        return this.color;
      },
      getBrushSize: () => {
        return this.brushSize;
      },
    });
  },
};
</script>

<style>
</style>