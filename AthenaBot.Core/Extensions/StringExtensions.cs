using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace AthenaBot
{
    public static class StringExtensions
    {
        public static string Repeat(this string @string, int count) {
            count = count <= 0 ? 1 : count;//if 0 return input
            var sb = new StringBuilder();

            for (int i = 0; i < count; i++)
                sb.Append(@string);

            return sb.ToString();
        }

        public static bool StartsWithAny(this string input, params char[] tests) {
            foreach (var test in tests)
                if (input.StartsWith(test))
                    return true;
            return false;
        }

        public static bool StartsWithAny(this string input, params string[] tests) {
            foreach (var test in tests)
                if (input.StartsWith(test))
                    return true;
            return false;
        }

        public static bool ContainsAny(this string @string, params char[] collection) {
            foreach (char str in collection)
                if (@string.Contains(str)) return true;

            return false;
        }

        public static bool ContainsAny(this string @string, params string[] collection) {
            foreach (string str in collection)
                if (@string.Contains(str)) return true;

            return false;
        }

        public static bool EndsWithAny(this string input, params char[] tests) {
            foreach (var test in tests)
                if (input.EndsWith(test))
                    return true;
            return false;
        }

        public static bool EndsWithAny(this string input, params string[] tests) {
            foreach (var test in tests)
                if (input.EndsWith(test))
                    return true;
            return false;
        }

        public static string Join(this IEnumerable<string> collection, string delim = ", ") {
            var sb = new StringBuilder();
            int count = 0;
            foreach (string str in collection) {
                count++;
                sb.Append(str);
                sb.Append(delim);
            }
            if (count > 0)
                sb.Remove(sb.Length - delim.Length, delim.Length);
            return sb.ToString();
        }

        public static string ToNativeString(this SecureString secure) {
            if (secure == null) return string.Empty;
            IntPtr ptr = IntPtr.Zero;

            try {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secure);
                return Marshal.PtrToStringUni(ptr);
            }
            finally {
                if (ptr != IntPtr.Zero)
                    Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }

        public static SecureString ToSecureString(this string unsecure) {
            SecureString ret = new SecureString();
            foreach (char c in unsecure) ret.AppendChar(c);
            ret.MakeReadOnly();
            return ret;
        }
    }
}
