using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nito.UniformResourceIdentifiers.Helpers
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

        public static bool TryParseDate(string date, out int year, out int? month, out int? day)
        {
            throw new NotImplementedException();
        }
    }
}
