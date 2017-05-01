using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nito.UniformResourceIdentifiers;
using Nito.UniformResourceIdentifiers.Implementation;
using Xunit;

namespace Core.UnitTests
{
    public class RfcTests
    {
        [Theory]
        [InlineData("ftp://ftp.is.co.za/rfc/rfc1808.txt", "ftp", null, "ftp.is.co.za", null, new[] { "", "rfc", "rfc1808.txt" }, null, null)] // See 1.1.2
        [InlineData("http://www.ietf.org/rfc/rfc2396.txt", "http", null, "www.ietf.org", null, new[] { "", "rfc", "rfc2396.txt" }, null, null)] // See 1.1.2
        [InlineData("ldap://[2001:db8::7]/c=GB?objectClass?one", "ldap", null, "[2001:db8::7]", null, new[] { "", "c=GB" }, "objectClass?one", null)] // See 1.1.2
        [InlineData("mailto:John.Doe@example.com", "mailto", null, null, null, new[] { "John.Doe@example.com" }, null, null)] // See 1.1.2
        [InlineData("news:comp.infosystems.www.servers.unix", "news", null, null, null, new[] { "comp.infosystems.www.servers.unix" }, null, null)] // See 1.1.2
        [InlineData("tel:+1-816-555-1212", "tel", null, null, null, new[] { "+1-816-555-1212" }, null, null)] // See 1.1.2
        [InlineData("telnet://192.0.2.16:80/", "telnet", null, "192.0.2.16", "80", new[] { "", "" }, null, null)] // See 1.1.2
        [InlineData("urn:oasis:names:specification:docbook:dtd:xml:4.1.2", "urn", null, null, null, new[] { "oasis:names:specification:docbook:dtd:xml:4.1.2" }, null, null)] // See 1.1.2
        [InlineData("foo://example.com:8042/over/there?name=ferret#nose", "foo", null, "example.com", "8042", new[] { "", "over", "there" }, "name=ferret", "nose")] // See 3
        [InlineData("urn:example:animal:ferret:nose", "urn", null, null, null, new[] { "example:animal:ferret:nose" }, null, null)] // See 3
        [InlineData("mailto:fred@example.com", "mailto", null, null, null, new[] { "fred@example.com" }, null, null)] // See 3.3
        [InlineData("foo://info.example.com?fred", "foo", null, "info.example.com", null, new string[] { }, "fred", null)] // See 3.3
        [InlineData("foo://info.example.com?fred", "foo", null, "info.example.com", null, new[] { "" }, "fred", null)] // See 3.3
        public void UriFormattingRfcExamples(string expectedUrl, string scheme, string userInfo, string host, string port, IEnumerable<string> path, string query, string fragment)
        {
            var uri = new GenericUniformResourceIdentifier(scheme, userInfo, host, port, path, query, fragment);
            Assert.Equal(expectedUrl, uri.ToString());
        }

        [Theory]
        [InlineData("test3:", "test3", null, null, null, new string[] { }, null, null)] // Scheme only
        [InlineData("scheme://host/b", "scheme", null, "host", null, new[] { "", "a", "..", "..", ".", "b" }, null, null)] // Paths are normalized
        [InlineData("scheme:a/b", "scheme", null, null, null, new[] { "a", "..", "..", ".", "b" }, null, null)] // Paths normalized without host and without leading slash retain initial segment (unusual but legal)
        [InlineData("scheme:/b", "scheme", null, null, null, new[] { "", "a", "..", "..", ".", "b" }, null, null)] // Paths normalized without host (unusual but legal)
        public void UriFormattingExamples(string expectedUrl, string scheme, string userInfo, string host, string port, IEnumerable<string> path, string query, string fragment)
        {
            var uri = new GenericUniformResourceIdentifier(scheme, userInfo, host, port, path, query, fragment);
            Assert.Equal(expectedUrl, uri.ToString());
        }

        [Theory]
        [InlineData(new[] { "", "a", "g" }, new[] { "", "a", "b", "c", ".", "..", "..", "g" })] // See 5.2.4
        [InlineData(new[] { "mid", "6" }, new[] { "mid", "content=5", "..", "6" })] // See 5.2.4
        public void RemoveDotSegmentsRfcExamples(IEnumerable<string> expected, IEnumerable<string> input)
        {
            var result = Util.RemoveDotSegments(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("example://a/b/c/%7Bfoo%7D", "eXAMPLE://a/./b/../b/%63/%7bfoo%7d")] // See 6.2.2
        [InlineData("HTTP://www.EXAMPLE.com/", "http://www.example.com/")] // See 6.2.2.1
        public void ComparisonExamples(string first, string second)
        {
            var uri1 = GenericUniformResourceIdentifier.Parse(first);
            var uri2 = GenericUniformResourceIdentifier.Parse(second);

            Assert.True(uri1.Equals(uri2));
            Assert.Equal(0, uri1.CompareTo(uri2));
        }

        [Theory]
        [InlineData("http://a/b/c/g", "g", null, new[] { "g" }, null, null)]
        [InlineData("http://a/b/c/g", "./g", null, new[] { ".", "g" }, null, null)]
        [InlineData("http://a/b/c/g/", "g/", null, new[] { "g", "" }, null, null)]
        [InlineData("http://a/g", "/g", null, new[] { "", "g" }, null, null)]
        [InlineData("http://g", "//g", "g", new[] { "" }, null, null)]
        [InlineData("http://a/b/c/d;p?y", "?y", null, new[] { "" }, "y", null)]
        [InlineData("http://a/b/c/g?y", "g?y", null, new[] { "g" }, "y", null)]
        [InlineData("http://a/b/c/d;p?q#s", "#s", null, new[] { "" }, null, "s")]
        [InlineData("http://a/b/c/g#s", "g#s", null, new[] { "g" }, null, "s")]
        [InlineData("http://a/b/c/g?y#s", "g?y#s", null, new[] { "g" }, "y", "s")]
        [InlineData("http://a/b/c/;x", ";x", null, new[] { ";x" }, null, null)]
        [InlineData("http://a/b/c/g;x", "g;x", null, new[] { "g;x" }, null, null)]
        [InlineData("http://a/b/c/g;x?y#s", "g;x?y#s", null, new[] { "g;x" }, "y", "s")]
        [InlineData("http://a/b/c/d;p?q", "", null, new[] { "" }, null, null)]
        [InlineData("http://a/b/c/", ".", null, new[] { "." }, null, null)]
        [InlineData("http://a/b/c/", "./", null, new[] { ".", "" }, null, null)]
        [InlineData("http://a/b/", "..", null, new[] { ".." }, null, null)]
        [InlineData("http://a/b/", "../", null, new[] { "..", "" }, null, null)]
        [InlineData("http://a/b/g", "../g", null, new[] { "..", "g" }, null, null)]
        [InlineData("http://a/", "../..", null, new[] { "..", ".." }, null, null)]
        [InlineData("http://a/", "../../", null, new[] { "..", "..", "" }, null, null)]
        [InlineData("http://a/g", "../../g", null, new[] { "..", "..", "g" }, null, null)]
        public void ReferenceResolutionNormalExamples(string expectedUrl, string expectedReferenceUrl, string host, IEnumerable<string> path, string query, string fragment)
        {
            // See 5.4.1
            var referenceUri = new RelativeReferenceBuilder().WithHost(host).WithPrefixlessPathSegments(path).WithQuery(query).WithFragment(fragment).Build();
            Assert.Equal(expectedReferenceUrl, referenceUri.ToString());

            var baseUri = GenericUniformResourceIdentifier.Parse("http://a/b/c/d;p?q");
            var result = baseUri.Resolve(referenceUri);
            Assert.Equal(expectedUrl, result.ToString());

            var abstractResult = ((IUniformResourceIdentifier) baseUri).Resolve(referenceUri);
            Assert.IsType<GenericUniformResourceIdentifier>(abstractResult);
            Assert.Equal(expectedUrl, abstractResult.ToString());

            var abstractReferenceResult = baseUri.Resolve(UniformResourceIdentifierReference.Parse(expectedReferenceUrl));
            Assert.Equal(expectedUrl, abstractReferenceResult.ToString());
        }
    }
}
