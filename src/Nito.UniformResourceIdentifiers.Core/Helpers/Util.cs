using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

// TODO: RFC6570
// TODO: Refactor IPAddress support into a netstandard1.3 addon.

namespace Nito.UniformResourceIdentifiers.Helpers
{
    /// <summary>
    /// Utility constants and methods useful for constructing URI parsers and builders conforming to RFC3986 (https://tools.ietf.org/html/rfc3986).
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Generates all the unreserved characters (see 2.3), in ASCII order.
        /// </summary>
        private static IEnumerable<byte> GenerateUnreservedCharacters()
        {
            yield return (byte)'-';
            yield return (byte)'.';
            for (var ch = (byte)'0'; ch <= '9'; ++ch)
                yield return ch;
            for (var ch = (byte)'A'; ch <= 'Z'; ++ch)
                yield return ch;
            yield return (byte)'_';
            for (var ch = (byte)'a'; ch <= 'z'; ++ch)
                yield return ch;
            yield return (byte)'~';
        }

        /// <summary>
        /// The standard UTF8 encoding.
        /// </summary>
        public static Encoding Utf8EncodingWithoutBom { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        /// <summary>
        /// All the unreserved characters (see 2.3), in ASCII order.
        /// </summary>
        public static byte[] UnreservedCharacters { get; } = GenerateUnreservedCharacters().ToArray();

        /// <summary>
        /// All the general delimiter characters (<c>gen-delims</c>) (see 2.2), in ASCII order.
        /// </summary>
        public static byte[] GeneralDelimiterCharacters { get; } = { (byte)'#', (byte)'/', (byte)':', (byte)'?', (byte)'@', (byte)'[', (byte)']' };

        /// <summary>
        /// All the subcomponent delimiter characters (<c>sub-delims</c>) (see 2.2), in ASCII order.
        /// </summary>
        public static byte[] SubcomponentDelimiterCharacters { get; } = { (byte)'!', (byte)'$', (byte)'&', (byte)'\'', (byte)'(', (byte)')', (byte)'*', (byte)'+', (byte)',', (byte)';', (byte)'=' };

        /// <summary>
        /// Map of the percent-encoded unreserved characters to their unencoded representation.
        /// </summary>
        public static IReadOnlyDictionary<string, string> EncodedUnreservedCharacters { get; } = GenerateUnreservedCharacters().ToDictionary(PercentEncode, x => ((char)x).ToString());

        /// <summary>
        /// Whether a character is unreserved (see 2.3).
        /// </summary>
        /// <param name="ch">The character to test.</param>
        public static bool IsUnreserved(byte ch) => Array.BinarySearch(UnreservedCharacters, ch) >= 0;

        /// <summary>
        /// Whether a character is a general delimiter (<c>gen-delims</c>) (see 2.2).
        /// </summary>
        /// <param name="ch">The character to test.</param>
        public static bool IsGeneralDelimiter(byte ch) => Array.BinarySearch(GeneralDelimiterCharacters, ch) >= 0;

        /// <summary>
        /// Whether a character is a subcomponent delimiter (<c>sub-delims</c>) (see 2.2).
        /// </summary>
        /// <param name="ch">The character to test.</param>
        public static bool IsSubcomponentDelimiter(byte ch) => Array.BinarySearch(SubcomponentDelimiterCharacters, ch) >= 0;

        /// <summary>
        /// Whether a character is reserved (see 2.2).
        /// </summary>
        /// <param name="ch">The character to test.</param>
        public static bool IsReserved(byte ch) => IsGeneralDelimiter(ch) || IsSubcomponentDelimiter(ch);

        /// <summary>
        /// Whether a string is a valid scheme (see 3.1).
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static bool IsValidScheme(string value) => SchemeRegex.IsMatch(value);
        private static Regex SchemeRegex { get; } = new Regex("^[a-z][a-z0-9+-.]*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /// <summary>
        /// Whether a string is a valid port (see 3.2.3).
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static bool IsValidPort(string value) => PortRegex.IsMatch(value);
        private static Regex PortRegex { get; } = new Regex("^[0-9]*$", RegexOptions.CultureInvariant);

        private static Regex HostIPvFutureRegex { get; } = new Regex(@"^\[v[0-9a-f]+\.[a-z0-9-._~!$&'()*+,;=:]+\]$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /// <summary>
        /// Whether a host string is an <c>IP-literal</c> (see 3.2.2).
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static bool HostIsIpLiteral(string value)
        {
            if (!value.StartsWith("[") || !value.EndsWith("]"))
                return false;
            if (HostIPvFutureRegex.IsMatch(value))
                return true;
            IPAddress address;
            if (!IPAddress.TryParse(value.Substring(1, value.Length - 2), out address)) // TODO: see 7.4 and RFC6874
                return false;
            return address.AddressFamily == AddressFamily.InterNetworkV6;
        }

        /// <summary>
        /// Whether a host string is an <c>IPv4address</c> (see 3.2.2).
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static bool HostIsIpV4Address(string value)
        {
            IPAddress address;
            if (!IPAddress.TryParse(value, out address)) // TODO: See 7.4
                return false;
            return address.AddressFamily == AddressFamily.InterNetwork;
        }

        /// <summary>
        /// Whether a host string is an <c>IP-literal</c> or <c>IPv4address</c> (see 3.2.2).
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static bool HostIsIpAddress(string value) => HostIsIpLiteral(value) || HostIsIpV4Address(value);

        /// <summary>
        /// Percent-encodes a single character.
        /// </summary>
        /// <param name="ch">The character to encode.</param>
        public static string PercentEncode(byte ch) => "%" + ch.ToString("X2", CultureInfo.InvariantCulture);

        /// <summary>
        /// Percent-encodes the string <paramref name="value"/> except for those characters for which <paramref name="isSafe"/> returns <c>true</c>.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        /// <param name="isSafe">A function determining whether a character is safe (i.e., does not need encoding).</param>
        public static string PercentEncode(string value, Func<byte, bool> isSafe)
        {
            var bytes = Utf8EncodingWithoutBom.GetBytes(value);
            var sb = new StringBuilder(bytes.Length);
            foreach (var ch in bytes)
            {
                if (isSafe(ch))
                    sb.Append((char)ch);
                else
                    sb.Append(PercentEncode(ch));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Percent-encodes the user information portion of an authority string.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        public static string PercentEncodeUserInfo(string value) => PercentEncode(value, x => IsUnreserved(x) || IsSubcomponentDelimiter(x) || x == ':');

        /// <summary>
        /// Percent-encodes the host portion of an authoirty string.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        public static string PercentEncodeHost(string value) => HostIsIpAddress(value) ? value : PercentEncode(value, x => IsUnreserved(x) || IsSubcomponentDelimiter(x));

        /// <summary>
        /// Percent-encodes a path segment.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        public static string PercentEncodePathSegment(string value) => PercentEncode(value, x => IsUnreserved(x) || IsSubcomponentDelimiter(x) || x == ':' || x == '@');

        /// <summary>
        /// Percent-encodes a sequence of path segments and separates them with forward slashes.
        /// </summary>
        /// <param name="segments">The values to encode.</param>
        public static string PercentEncdePathSegments(IEnumerable<string> segments) => string.Join("/", segments.Select(PercentEncodePathSegment));

        /// <summary>
        /// Percent-encodes a query string.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        public static string PercentEncodeQuery(string value) => PercentEncode(value, x => IsUnreserved(x) || IsSubcomponentDelimiter(x) || x == ':' || x == '@' || x == '/' || x == '?');

        /// <summary>
        /// Percent-encodes a frament string.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        public static string PercentEncodeFragment(string value) => PercentEncode(value, x => IsUnreserved(x) || IsSubcomponentDelimiter(x) || x == ':' || x == '@' || x == '/' || x == '?');

        /// <summary>
        /// Percent-encodes an IPv6 ZoneId for use within an IP-literal host string.
        /// </summary>
        /// <param name="value">The value to encode.</param>
        public static string PercentEncodeZoneId(string value) => PercentEncode(value, IsUnreserved);

        /// <summary>
        /// Unencodes only the unreserved characters in a URI string. This is always a safe operation and does not change semantics (see 2.4).
        /// </summary>
        /// <param name="value">The value to decode.</param>
        public static string DecodeUnreserved(string value)
        {
            var sb = new StringBuilder(value);
            foreach (var kvp in EncodedUnreservedCharacters)
                sb.Replace(kvp.Value, kvp.Key);
            return sb.ToString();
        }

        /// <summary>
        /// Removes dot segments from a path sequence, normalizing it.
        /// </summary>
        /// <param name="pathSegments">The path segments to normalize.</param>
        public static List<string> RemoveDotSegments(IEnumerable<string> pathSegments)
        {
            var result = new List<string>();
            foreach (var segment in pathSegments.SkipWhile(x => x == "." || x == ".."))
            {
                switch (segment)
                {
                    case ".":
                        break;
                    case "..":
                        if (result.Count > 1)
                            result.RemoveAt(result.Count - 1);
                        break;
                    default:
                        result.Add(segment);
                        break;
                }
            }
            return result;
        }
    }
}
