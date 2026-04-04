import { ref, onUnmounted } from 'vue'
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'

export function useSignalR(hubUrl) {
  const connection = ref(null)
  const connected = ref(false)
  const error = ref(null)

  async function start() {
    try {
      connection.value = new HubConnectionBuilder()
        .withUrl(hubUrl)
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build()

      connection.value.onclose(() => { connected.value = false })
      connection.value.onreconnected(() => { connected.value = true })

      await connection.value.start()
      connected.value = true
    } catch (err) {
      error.value = err
      connected.value = false
    }
  }

  function on(event, handler) {
    connection.value?.on(event, handler)
  }

  async function invoke(method, ...args) {
    return connection.value?.invoke(method, ...args)
  }

  async function stop() {
    await connection.value?.stop()
    connected.value = false
  }

  onUnmounted(() => { stop() })

  return { connection, connected, error, start, on, invoke, stop }
}
