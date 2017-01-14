using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        /// <summary>
        /// Whether a host string is an <c>IPvFuture</c> (see 3.2.2).
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static bool HostIsFutureIpLiteral(string value) => HostIPvFutureRegex.IsMatch(value);
        private static Regex HostIPvFutureRegex { get; } = new Regex(@"^\[v[0-9a-f]+\.[a-z0-9-._~!$&'()*+,;=:]+\]$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        // TODO: Include an implementation of RFC5952 to canonicalize IPv6 addresses when they are detected.

        /// <summary>
        /// Attempts to parse an <c>IPv6address</c> (see 3.2.2 and RFC6874).
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="octets">The octets of the address.</param>
        /// <param name="zoneId">The resulting zone id.</param> // TODO: Not yet supported
        /// <param name="octetsOffset">The index of <paramref name="octets"/> at which to begin writing.</param>
        public static bool TryParseIpV6Address(string value, byte[] octets, out string zoneId, int octetsOffset = 0)
        {
            if (octets.Length - 16 < octetsOffset)
                throw new ArgumentException("Not enough remaining space in octets array.");
            zoneId = null;
            var elisionSplit = value.Split(new [] { "::" }, StringSplitOptions.None);
            if (elisionSplit.Length == 1)
            {
                var pieces = value.Split(':');
                switch (pieces.Length)
                {
                    case 7:
                        if (!ParseIpV6Pieces(pieces, pieces.Length - 1, octets, ref octetsOffset))
                            return false;
                        return TryParseIpV4Address(pieces[6], octets, octetsOffset);
                    case 8:
                        return ParseIpV6Pieces(pieces, pieces.Length, octets, ref octetsOffset);
                    default:
                        return false;
                }
            }
            if (elisionSplit.Length == 2)
            {
                for (var i = octetsOffset; i != octetsOffset + 16; ++i)
                    octets[i] = 0;
                var pieces1 = elisionSplit[0].Split(':');
                var pieces2 = elisionSplit[1].Split(':');
                if (pieces1.Length + pieces2.Length > 7)
                    return false;
                var pieces2octets = 2 * pieces2.Length;
                if (pieces2.Length > 0 && pieces2[pieces2.Length - 1].Contains("."))
                {
                    if (pieces1.Length + pieces2.Length > 6)
                        return false;
                    if (!TryParseIpV4Address(pieces2[pieces2.Length - 1], octets, octetsOffset + 12))
                        return false;
                    pieces2[pieces2.Length - 1] = null;
                    pieces2octets += 2;
                }
                if (!ParseIpV6Pieces(pieces1, pieces1.Length, octets, ref octetsOffset))
                    return false;
                octetsOffset = 16 - pieces2octets;
                return ParseIpV6Pieces(pieces2, pieces2.Length, octets, ref octetsOffset);
            }
            return false;
        }
        private static Regex H16Regex { get; } = new Regex(@"^[0-9a-f]{1,4}$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static bool ParseIpV6Pieces(string[] pieces, int piecesEnd, byte[] octets, ref int octetsOffset)
        {
            for (var i = 0; i != piecesEnd; ++i)
            {
                if (pieces[i] == null)
                    return true;
                if (!H16Regex.IsMatch(pieces[i]))
                    return false;
                ushort piece;
                if (!ushort.TryParse(pieces[i], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out piece))
                    return false;
                octets[octetsOffset++] = (byte)(piece >> 8);
                octets[octetsOffset++] = (byte)(piece & 0xFF);
            }
            return true;
        }

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
            var octets = new byte[16];
            var zoneId = "";
            return TryParseIpV6Address(value.Substring(1, value.Length - 2), octets, out zoneId);
        }

        /// <summary>
        /// Attempts to parse an <c>IPv4address</c> (see 3.2.2). Note that this algorithm only expects the restricted IPv4 address style allowed in URIs.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="octets">The octets of the address.</param>
        /// <param name="octetsOffset">The index of <paramref name="octets"/> at which to begin writing.</param>
        public static bool TryParseIpV4Address(string value, byte[] octets, int octetsOffset = 0)
        {
            if (octets.Length - 4 < octetsOffset)
                throw new ArgumentException("Not enough remaining space in octets array.");
            var octetStrings = value.Split('.');
            if (octetStrings.Length != 4)
                return false;
            for (var i = 0; i != 4; ++i)
            {
                byte result;
                if (!byte.TryParse(octetStrings[i], NumberStyles.None, CultureInfo.InvariantCulture, out result))
                    return false;
                if (result.ToString(CultureInfo.InvariantCulture) != octetStrings[i]) // Disallow 0-prefixed values.
                    return false;
                octets[octetsOffset + i] = result;
            }
            return true;
        }

        /// <summary>
        /// Whether a host string is an <c>IPv4address</c> (see 3.2.2).
        /// </summary>
        /// <param name="value">The string to test.</param>
        public static bool HostIsIpV4Address(string value)
        {
            var junk = new byte[4];
            return TryParseIpV4Address(value, junk);
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
            bool lastIsDot = false;
            foreach (var segment in pathSegments.SkipWhile(x => x == "." || x == ".."))
            {
                lastIsDot = false;
                switch (segment)
                {
                    case ".":
                        lastIsDot = true;
                        break;
                    case "..":
                        lastIsDot = true;
                        if (result.Count > 1)
                            result.RemoveAt(result.Count - 1);
                        break;
                    default:
                        result.Add(segment);
                        break;
                }
            }
            if (lastIsDot)
                result.Add("");
            return result;
        }

        ///// <summary>
        ///// Removes dot segments from a path sequence, normalizing it.
        ///// </summary>
        ///// <param name="pathSegments">The path segments to normalize.</param>
        //public static List<string> RemoveDotSegments(IEnumerable<string> pathSegments)
        //{
        //    var output = new List<string>();
        //    var input = pathSegments.SkipWhile(x => x == "." || x == "..").ToList();
        //    while (input.Count > 0)
        //    {
        //        if (HasPrefixOf(input, "", "."))
        //            input.RemoveAt(1);
        //        else if (HasPrefixOf(input, "", ".."))
        //        {
        //            input.RemoveAt(1);
        //            if (output.Count > 1)
        //                output.RemoveAt(output.Count - 1);
        //        }
        //        else if (input.Count == 1 && (input[0] == "." || input[0] == ".."))
        //            input.RemoveAt(0);
        //        else
        //        {
        //            output.Add(input[0]);
        //            input.RemoveAt(0);
        //        }
        //    }
        //    return output;
        //}

        private static bool HasPrefixOf(IReadOnlyList<string> path, params string[] prefix)
        {
            if (path.Count < prefix.Length)
                return false;
            for (int i = 0; i != prefix.Length; ++i)
            {
                if (path[i] != prefix[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Whether the given path is empty.
        /// </summary>
        /// <param name="pathSegments">The path to test.</param>
        public static bool PathIsEmpty(IReadOnlyList<string> pathSegments) => pathSegments.Count == 0 || (pathSegments.Count == 1 && pathSegments[0] == "");

        /// <summary>
        /// Whether the given path is absolute (i.e., starts with a forward-slash when converted to a string).
        /// </summary>
        /// <param name="pathSegments">The path to test.</param>
        public static bool PathIsAbsolute(IReadOnlyList<string> pathSegments) => pathSegments.Count > 1 && pathSegments[0] == "";

        /// <summary>
        /// A delegate used to create a type of URI.
        /// </summary>
        /// <typeparam name="T">The type of URI to create.</typeparam>
        /// <param name="userInfo">The user information portion of the authority.</param>
        /// <param name="host">The host portion of the authority.</param>
        /// <param name="port">The port portion of the authority.</param>
        /// <param name="pathSegments">The path segments.</param>
        /// <param name="query">The query string.</param>
        /// <param name="fragment">The fragment string.</param>
        public delegate T DelegateFactory<out T>(string userInfo, string host, string port, IEnumerable<string> pathSegments, string query, string fragment);

        /// <summary>
        /// Resolves a relative URI against a base URI.
        /// </summary>
        /// <typeparam name="T">The type of the base URI; also the type of the result.</typeparam>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="relativeUri">The relative URI.</param>
        /// <param name="factory">The factory method used to create a new URI.</param>
        public static T Resolve<T>(T baseUri, RelativeReference relativeUri, DelegateFactory<T> factory)
            where T : UniformResourceIdentifier
        {
            // See 5.2.2, except that referenceUri will always have a null Scheme.
            string userInfo, host, port, query;
            IEnumerable<string> pathSegments;
            if (relativeUri.AuthorityIsDefined)
            {
                userInfo = relativeUri.UserInfo;
                host = relativeUri.Host;
                port = relativeUri.Port;
                pathSegments = relativeUri.PathSegments;
                query = relativeUri.Query;
            }
            else
            {
                if (relativeUri.PathIsEmpty)
                {
                    pathSegments = baseUri.PathSegments;
                    query = relativeUri.Query ?? baseUri.Query;
                }
                else
                {
                    if (relativeUri.PathIsAbsolute)
                    {
                        pathSegments = relativeUri.PathSegments;
                    }
                    else
                    {
                        // See 5.2.3
                        if (baseUri.AuthorityIsDefined && baseUri.PathIsEmpty)
                            pathSegments = Enumerable.Repeat("", 1).Concat(relativeUri.PathSegments);
                        else
                            pathSegments = baseUri.PathSegments.Take(baseUri.PathSegments.Count - 1).Concat(relativeUri.PathSegments);
                    }
                    query = relativeUri.Query;
                }
                userInfo = baseUri.UserInfo;
                host = baseUri.Host;
                port = baseUri.Port;
            }
            return factory(userInfo, host, port, pathSegments, query, relativeUri.Fragment);
        }

        /// <summary>
        /// Resolves a reference URI against a base URI. The reference URI may be relative or a URI.
        /// </summary>
        /// <typeparam name="T">The type of the base URI. This may be different than the type of the result.</typeparam>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="referenceUri">The reference URI.</param>
        /// <param name="factory">The factory method used to create a new URI.</param>
        public static UniformResourceIdentifier Resolve<T>(T baseUri, UniformResourceIdentifierReference referenceUri, DelegateFactory<T> factory)
            where T : UniformResourceIdentifier
        {
            // See 5.2.2, except we always do strict resolution.
            var relativeReference = referenceUri as RelativeReference;
            if (relativeReference != null)
                return Resolve(baseUri, relativeReference, factory);
            return (UniformResourceIdentifier) referenceUri;
        }
    }
}
