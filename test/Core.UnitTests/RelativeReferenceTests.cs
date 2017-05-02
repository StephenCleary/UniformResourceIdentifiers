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
    public class RelativeReferenceTests
    {
        [Theory]
        [InlineData("g", null, null, null, new[] { "g" }, null, null)]
        [InlineData("./g", null, null, null, new[] { ".", "g" }, null, null)]
        [InlineData("g/", null, null, null, new[] { "g", "" }, null, null)]
        [InlineData("/g", null, null, null, new[] { "", "g" }, null, null)]
        [InlineData("//g", null, "g", null, new[] { "" }, null, null)]
        [InlineData("?y", null, null, null, new[] { "" }, "y", null)]
        [InlineData("g?y", null, null, null, new[] { "g" }, "y", null)]
        [InlineData("#s", null, null, null, new[] { "" }, null, "s")]
        [InlineData("g#s", null, null, null, new[] { "g" }, null, "s")]
        [InlineData("g?y#s", null, null, null, new[] { "g" }, "y", "s")]
        [InlineData(";x", null, null, null, new[] { ";x" }, null, null)]
        [InlineData("g;x", null, null, null, new[] { "g;x" }, null, null)]
        [InlineData("g;x?y#s", null, null, null, new[] { "g;x" }, "y", "s")]
        [InlineData("", null, null, null, new[] { "" }, null, null)]
        [InlineData(".", null, null, null, new[] { "." }, null, null)]
        [InlineData("./", null, null, null, new[] { ".", "" }, null, null)]
        [InlineData("..", null, null, null, new[] { ".." }, null, null)]
        [InlineData("../", null, null, null, new[] { "..", "" }, null, null)]
        [InlineData("../g", null, null, null, new[] { "..", "g" }, null, null)]
        [InlineData("../..", null, null, null, new[] { "..", ".." }, null, null)]
        [InlineData("../../", null, null, null, new[] { "..", "..", "" }, null, null)]
        [InlineData("../../g", null, null, null, new[] { "..", "..", "g" }, null, null)]
        public void ReferenceResolutionNormalExamples(string expectedReferenceUrl, string userInfo, string host, string port, IEnumerable<string> path, string query, string fragment)
        {
            var uri = new RelativeReferenceBuilder().WithHost(host).WithPrefixlessPathSegments(path).WithQuery(query).WithFragment(fragment).Build();
            Assert.Equal(expectedReferenceUrl, uri.ToString());

            var parsed = RelativeReference.Parse(uri.ToString());
            Assert.Equal(userInfo, ((IUniformResourceIdentifierReference) parsed).UserInfo);
            Assert.Equal(host, parsed.Host);
            Assert.Equal(port, parsed.Port);
            Assert.Equal(path, parsed.PathSegments);
            Assert.Equal(query, parsed.Query);
            Assert.Equal(fragment, parsed.Fragment);

            var deconstructed = new RelativeReferenceBuilder(uri).Build();
            Assert.Equal(userInfo, ((IUniformResourceIdentifierReference) deconstructed).UserInfo);
            Assert.Equal(host, deconstructed.Host);
            Assert.Equal(port, deconstructed.Port);
            Assert.Equal(path, deconstructed.PathSegments);
            Assert.Equal(query, deconstructed.Query);
            Assert.Equal(fragment, deconstructed.Fragment);

            var systemUri = uri.ToUri();
            Assert.False(systemUri.IsAbsoluteUri);
            Assert.Equal(uri.ToString(), systemUri.ToString());
        }

        [Fact]
        public void ReferenceResolutionMakesPathRelativeIfNecessary()
        {
            var uri = new RelativeReferenceBuilder().WithPrefixlessPathSegments(new [] { "g:x" }).Build();
            Assert.Equal("./g:x", uri.ToString());

            var parsed = RelativeReference.Parse(uri.ToString());
            Assert.Equal(new [] { ".", "g:x" }, parsed.PathSegments);

            var deconstructed = new RelativeReferenceBuilder(uri).Build();
            Assert.Equal(new[] { ".", "g:x" }, deconstructed.PathSegments);

            var systemUri = uri.ToUri();
            Assert.False(systemUri.IsAbsoluteUri);
            Assert.Equal(uri.ToString(), systemUri.ToString());
        }
    }
}
