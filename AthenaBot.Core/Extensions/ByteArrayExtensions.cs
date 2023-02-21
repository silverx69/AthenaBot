namespace AthenaBot
{
    public static class ByteArrayExtensions
    {
        public static bool IsEmpty(this byte[] bytes) {
            return (bytes?.Length ?? 0) == 0;
        }
    }
}
