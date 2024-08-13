import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/hours')({
  component: () => <div>Hello /hours!</div>
})