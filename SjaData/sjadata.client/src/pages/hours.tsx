import HomeLink from "../components/home-link";
import { LinkBoxes, LinkBox } from "../components/link-boxes";
import MainLayout from "../components/main-layout";

export default function HoursPage() {
    return (<MainLayout>
        <h2>Hours</h2>
        <HomeLink />
        <section>
            <h3>
                @Title
            </h3>
            <LinkBoxes size="large">
                <LinkBox colour="light-gray">
                    <div>Last Month</div>
                    <div>
                        {/*@if (isLoading)*/}
                        {/*{*/}
                        {/*    <span className="loading-control" />*/}
                        {/*}*/}
                        {/*else*/}
                        {/*{*/}
                        {/*    @CalculateSum(lastMonth.Counts, selectedAreas)*/}
                        {/*}*/}
                    </div>
                </LinkBox>
                <LinkBox colour="black">
                    <div>This Month</div>
                    {/*@if (isLoading)*/}
                    {/*{*/}
                    {/*<span class="loading-control light" />*/}
                    {/*<span class="planned loading-control light" />*/}
                    {/*}*/}
                    {/*else*/}
                    {/*{*/}
                    {/*<div>@CalculateSum(thisMonth.Counts, selectedAreas)</div>*/}
                    {/*<div class="planned">@CalculateSum(thisMonthFuture.Counts, selectedAreas) Planned</div>*/}
                    {/*}*/}
                </LinkBox>
                {/*@if (!NhseSelected || RegionSelected)*/}
                {/*{*/}
                    <LinkBox colour="dark-green">
                        <div>Year to Date</div>
                        @if (isLoading)
                        {
                            <span className="loading-control" />
                        }
                        else
                        {
                            <div>@CalculateSum(yearToDate.Counts, selectedAreas)</div>
                        }
                    </LinkBox>
                {/*}*/}
                {/*@if (NhseSelected && !RegionSelected)*/}
                {/*{*/}
                    <LinkBox colour="dark-green">
                        <div>Target</div>
                        @if (isLoading)
                        {
                            <span className="loading-control" />
                        }
                        else
                        {
                            <div>@nhseTarget</div>
                        }
                    </LinkBox>
                {/*}*/}
            </LinkBoxes>
        </section>
        {/*<AreaSelector ActualAreas="@ActualAreas" SelectedAreasChanged="SelectedAreasChanged" />*/}
        <footer>
            <p>
                Hours are all shown as people-hours, not crew-hours (and so are
                double what we bill to NHSE).
            </p>
            <p className="last-update">
                Data last updated : @yearToDate.LastUpdate.ToString("f")
            </p>
        </footer>
    </MainLayout>)
}