<template>
  <div>
    <div v-for="(m, i) in messages" :key="`message-${i}`">
      <p :class="{ green: m.result }">{{ m.word }}</p>
    </div>
    <input v-model="wordGuess" />
    <button @click="onWordGuess">Send</button>
  </div>
</template>

<script>
export default {
  props: ["connector", "guess"],
  data: () => ({
    wordGuess: "",
    messages: [],
  }),
  created() {
    this.connector(this.guessWordResponse);
  },
  methods: {
    guessWordResponse(word, result) {
      this.messages.push({ word, result });
    },
    onWordGuess() {
      this.guess(this.wordGuess);
    },
  },
};
</script>

<style>
</style>