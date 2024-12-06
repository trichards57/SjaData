import { LinkBoxes, LinkBox } from "../components/link-boxes";
import MainLayout from "../components/main-layout";

export default function HomePage() {
    return (
        <MainLayout>
            <h2>Home</h2>
            <LinkBoxes size="small">
                <LinkBox to="/hours">
                    Hours
                </LinkBox>
                <LinkBox to="/trends">
                    Trends
                </LinkBox>
                <LinkBox to="/people">
                    People
                </LinkBox>
                <LinkBox to="/vehicles">
                    Vehicles
                </LinkBox>
                <LinkBox to="/deployments">
                    Deployments
                </LinkBox>
                <LinkBox colour="dark-green" to="/setup">
                    Setup
                </LinkBox>
                <LinkBox colour="dark-green" to="/update">
                    Update
                </LinkBox>
                <LinkBox colour="yellow" to="/access">
                    Access
                </LinkBox>
            </LinkBoxes>
        </MainLayout>);
}