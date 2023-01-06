namespace AthenaBot
{
    public static partial class ByteArrayExtensions
    {
        public static bool IsEmpty(this byte[] bytes) {
            return (bytes?.Length ?? 0) == 0;
        }
    }
}
