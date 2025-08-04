import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/generate": "http://localhost:5042",
      "/generators": "http://localhost:5042",
    },
  },
});
