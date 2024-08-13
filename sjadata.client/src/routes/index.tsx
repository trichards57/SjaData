import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/')({
    component: Index,
})

function Index() {
    return (
        <>
            <h1>SJA In Numbers</h1>
            <h2>Home</h2>
            <div className="link-boxes">
                <a className="link-box">Hours</a>
                <a className="link-box">Patients</a>
            </div>
        </>
    )
}
