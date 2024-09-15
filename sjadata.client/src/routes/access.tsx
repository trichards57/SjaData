import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/access')({
  component: () => <div>Hello /access!</div>,
})
