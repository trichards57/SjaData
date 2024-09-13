import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/trends')({
    component: () => <div>Hello /patients!</div>
})
