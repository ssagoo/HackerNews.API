import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {

  const env = loadEnv(mode, process.cwd(), '');
  return {
    define: {
      "process.env.NODE_ENV": (mode === "staging" && `"production"`) || `"${mode}"`,
    },
    base: "/",
    plugins: [react()],
    preview: {
      port: parseInt(env.VITE_PORT),
      strictPort: true,
    },
    server: {
      host: true,
      port: parseInt(env.VITE_PORT),
      strictPort: true,
      watch: {
        usePolling: true
      }
    }
  }
});
