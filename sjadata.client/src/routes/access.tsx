import { createFileRoute } from '@tanstack/react-router'
import { Loading } from '../components/loading'

export const Route = createFileRoute('/access')({
  pendingComponent: Loading,
  component: () => <div>Hello /access!</div>,
})
