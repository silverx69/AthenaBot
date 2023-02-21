namespace AthenaBot
{
    public static class DateTimeExtensions
    {
        public static DateTimeOffset ToUtcOffset(this DateTime time) {
            if (time.Kind == DateTimeKind.Utc)
                return new DateTimeOffset(time, TimeSpan.Zero);
            return new DateTimeOffset(time.ToUniversalTime(), TimeSpan.Zero);
        }
    }
}
