import { createFileRoute, Link } from '@tanstack/react-router'

export const Route = createFileRoute('/')({
    component: Index,
})

function Index() {
    return (
        <>
            <h1>SJA In Numbers</h1>
            <h2>Home</h2>
            <div className="link-boxes">
                <Link className="link-box" to="/hours">Hours</Link>
                <Link className="link-box" to="/patients">Patients</Link>
            </div>
        </>
    )
}
