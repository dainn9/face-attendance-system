import AppRouter from '../routes/AppRouter'
import AuthProvider from './providers/AuthProvider'
import ToastProvider from './providers/ToastProvider'

function App() {
  return (
    <AuthProvider>
      <AppRouter />
      <ToastProvider/>
    </AuthProvider>
  )
}

export default App
