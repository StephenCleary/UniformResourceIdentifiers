using Nito.UniformResourceIdentifiers.Implementation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nito.UniformResourceIdentifiers.Implementation
{
    public static class TagUtil
    {
        private static Regex EmailUserNameRegex { get; } = new Regex(@"^[-._A-Z0-9]+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static Regex DnsComputerNameRegex { get; } = new Regex(@"^[A-Z0-9]([-A-Z0-9]*[A-Z0-9])?$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static bool IsValidAuthorityName(string authorityName)
        {
            var split = authorityName.Split('@');
            if (split.Length != 1 && split.Length != 2)
                return false;
            if (split.Length == 2 && !EmailUserNameRegex.IsMatch(split[0]))
                return false;
            var dnsComputerNames = split[split.Length == 1 ? 0 : 1].Split('.');
            return dnsComputerNames.All(DnsComputerNameRegex.IsMatch);
        }

        private static readonly Regex DateRegex = new Regex(@"^([0-9]{4})(?:-([0-9]{2})(?:-([0-9){2}))?)?$", RegexOptions.CultureInvariant);

        public static bool TryParseDate(string date, int offset, int length, out int year, out int? month, out int? day)
        {
            year = 0;
            month = day = null;
            var dateParts = DateRegex.Match(date, offset, length);
            if (!dateParts.Success)
                return false;
            if (!int.TryParse(dateParts.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out year))
                return false;
            if (dateParts.Groups[2].Value != "")
            {
                if (!int.TryParse(dateParts.Groups[2].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
                    return false;
                month = value;
            }
            if (dateParts.Groups[3].Value != "")
            {
                if (!int.TryParse(dateParts.Groups[3].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
                    return false;
                day = value;
            }
            return true;
        }

        /// <summary>
        /// A delegate for determining whether a specific character is safe (does not require encoding).
        /// </summary>
        public static readonly Func<byte, bool> SpecificCharIsSafe = Util.QueryCharIsSafe;

        /// <summary>
        /// Formats the URI components as a complete string. Components are assumed to be already validated.
        /// </summary>
        public static string ToString(string authorityName, int year, int? month, int? day, string specific, string fragment)
        {
            var sb = new StringBuilder();
            sb.Append(TagUniformResourceIdentifier.TagScheme);
            sb.Append(':');
            sb.Append(authorityName);
            sb.Append(',');
            sb.Append(year.ToString("D4", CultureInfo.InvariantCulture));
            if (month != null)
            {
                sb.Append('-');
                sb.Append(month.Value.ToString("D2", CultureInfo.InvariantCulture));
                if (day != null)
                {
                    sb.Append('-');
                    sb.Append(day.Value.ToString("D2", CultureInfo.InvariantCulture));
                }
            }
            sb.Append(Util.PercentEncode(specific, SpecificCharIsSafe));
            if (fragment != null)
            {
                sb.Append('#');
                sb.Append(Util.PercentEncode(fragment, Util.FragmentCharIsSafe));
            }
            return sb.ToString();
        }
    }
}
