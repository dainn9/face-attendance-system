namespace BuildingBlocks.Time
{
    public static class TimeZoneConstants
    {
        public static readonly TimeZoneInfo Vietnam = TimeZoneInfo.CreateCustomTimeZone(
            "Vietnam Standard Time",
            TimeSpan.FromHours(7),
            "Vietnam Standard Time",
            "Vietnam Standard Time"
        );
    }
}