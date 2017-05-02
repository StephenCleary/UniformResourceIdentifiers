using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nito.UniformResourceIdentifiers;
using Xunit;

namespace Core.UnitTests
{
    public class UniformResourceIdentifierTests
    {
        private static readonly IEnumerable<string> DefaultPathSegments = Enumerable.Empty<string>();

        [Fact]
        public void Construct_NullScheme_Throws() => Assert.Throws<ArgumentNullException>(() => CreateUri(DefaultPathSegments, scheme: null));

        [Fact]
        public void Construct_EmptyScheme_Throws() => Assert.Throws<ArgumentException>(() => CreateUri(DefaultPathSegments, scheme: ""));

        [Fact]
        public void Construct_InvalidScheme_Throws() => Assert.Throws<ArgumentException>(() => CreateUri(DefaultPathSegments, scheme: "3DS"));

        [Fact]
        public void Construct_InvalidPort_Throws() => Assert.Throws<ArgumentException>(() => CreateUri(DefaultPathSegments, port: "bob"));

        [Fact]
        public void Construct_NullPathSegments_Throws() => Assert.Throws<ArgumentNullException>(() => CreateUri(null));

        [Fact]
        public void Construct_PathWithNullSegments_Throws() => Assert.Throws<ArgumentException>(() => CreateUri(new string[] { null }));

        [Fact]
        public void Construct_HostAndPathWithoutLeadingSlash_Throws() => Assert.Throws<ArgumentException>(() => CreateUri(new[] { "x" }));

        [Fact]
        public void ConstructBuilder_NullPathSegments_Throws() => Assert.Throws<ArgumentNullException>(
            () => new GenericUniformResourceIdentifierBuilder().WithPrefixlessPathSegments(null));

        [Theory]
        [InlineData("ftp://ftp.is.co.za/rfc/rfc1808.txt", "ftp", null, "ftp.is.co.za", null, new[] { "", "rfc", "rfc1808.txt" }, null, null)]
        [InlineData("http://www.ietf.org/rfc/rfc2396.txt", "http", null, "www.ietf.org", null, new[] { "", "rfc", "rfc2396.txt" }, null, null)]
        [InlineData("ldap://[2001:db8::7]/c=GB?objectClass?one", "ldap", null, "[2001:db8::7]", null, new[] { "", "c=GB" }, "objectClass?one", null)]
        [InlineData("mailto:John.Doe@example.com", "mailto", null, null, null, new[] { "John.Doe@example.com" }, null, null)]
        [InlineData("news:comp.infosystems.www.servers.unix", "news", null, null, null, new[] { "comp.infosystems.www.servers.unix" }, null, null)]
        [InlineData("tel:+1-816-555-1212", "tel", null, null, null, new[] { "+1-816-555-1212" }, null, null)]
        [InlineData("telnet://192.0.2.16:80/", "telnet", null, "192.0.2.16", "80", new[] { "", "" }, null, null)]
        [InlineData("urn:oasis:names:specification:docbook:dtd:xml:4.1.2", "urn", null, null, null, new[] { "oasis:names:specification:docbook:dtd:xml:4.1.2" }, null, null)]
        [InlineData("foo://example.com:8042/over/there?name=ferret#nose", "foo", null, "example.com", "8042", new[] { "", "over", "there" }, "name=ferret", "nose")]
        [InlineData("urn:example:animal:ferret:nose", "urn", null, null, null, new[] { "example:animal:ferret:nose" }, null, null)]
        [InlineData("mailto:fred@example.com", "mailto", null, null, null, new[] { "fred@example.com" }, null, null)]
        [InlineData("foo://info.example.com?fred", "foo", null, "info.example.com", null, new[] { "" }, "fred", null)]
        public void RoundTripFormatting(string expectedUrl, string scheme, string userInfo, string host, string port, IEnumerable<string> path, string query, string fragment)
        {
            var uri = CreateUri(path, scheme: scheme, userInfo: userInfo, host: host, port: port, query: query, fragment: fragment);
            Assert.Equal(expectedUrl, uri.ToString());

            var parsed = GenericUniformResourceIdentifier.Parse(uri.ToString());
            Assert.Equal(scheme, parsed.Scheme);
            Assert.Equal(userInfo, parsed.UserInfo);
            Assert.Equal(host, parsed.Host);
            Assert.Equal(port, parsed.Port);
            Assert.Equal(path, parsed.PathSegments);
            Assert.Equal(query, parsed.Query);
            Assert.Equal(fragment, parsed.Fragment);

            var deconstructed = new GenericUniformResourceIdentifierBuilder(uri).Build();
            Assert.Equal(scheme, deconstructed.Scheme);
            Assert.Equal(userInfo, deconstructed.UserInfo);
            Assert.Equal(host, deconstructed.Host);
            Assert.Equal(port, deconstructed.Port);
            Assert.Equal(path, deconstructed.PathSegments);
            Assert.Equal(query, deconstructed.Query);
            Assert.Equal(fragment, deconstructed.Fragment);

            if (expectedUrl != "foo://info.example.com?fred") // Bug in System.Uri will translate this to "foo://info.example.com/?fred"
            {
                var systemUri = uri.ToUri();
                Assert.True(systemUri.IsAbsoluteUri);
                Assert.Equal(uri.ToString(), systemUri.ToString());
            }
        }

        private static GenericUniformResourceIdentifier CreateUri(IEnumerable<string> pathSegments, string scheme = "scheme", string userInfo = "user", string host = "host", string port = "8080", string query = "query", string fragment = "fragment")
            => new GenericUniformResourceIdentifier(scheme, userInfo, host, port, pathSegments, query, fragment);
    }
}
