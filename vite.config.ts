import {defineConfig} from "vite"
import react from "@vitejs/plugin-react"

// https://vitejs.dev/config/
export default defineConfig(() => ({
  plugins: [react()],
  base: "",
  server: {
	"port": 3000,
	"open": "https://red-scus-apimdemo-dev.developer.azure-api.net/?MS_APIM_CW_localhost_port=3000"
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
}))
