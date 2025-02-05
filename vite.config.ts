import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  // Load environment variables based on the current mode
  const env = loadEnv(mode, process.cwd());

  return {
    plugins: [react()],
    base: "",
    server: {
      port: Number(env.VITE_SERVER_PORT) || 3000, // Default to 3000 if not specified
      open: env.VITE_SERVER_OPEN, 
    },
    build: {
      outDir: "dist",
      emptyOutDir: true,
      rollupOptions: {
        input: {
          index: "./index.html",
          editor: "./editor.html",
        },
      },
    },
    publicDir: "static",
  };
});