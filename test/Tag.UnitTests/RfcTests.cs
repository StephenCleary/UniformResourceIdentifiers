using System;
using Xunit;
using Nito.UniformResourceIdentifiers;

namespace Tag.UnitTests
{
    public class RfcTests
    {
        [Theory]
        [InlineData("tag:timothy@hpl.hp.com,2001:web/externalHome", "timothy@hpl.hp.com", 2001, null, null, "web/externalHome")]
        [InlineData("tag:sandro@w3.org,2004-05:Sandro", "sandro@w3.org", 2004, 5, null, "Sandro")]
        [InlineData("tag:my-ids.com,2001-09-15:TimKindberg:presentations:UBath2004-05-19", "my-ids.com", 2001, 9, 15, "TimKindberg:presentations:UBath2004-05-19")]
        [InlineData("tag:blogger.com,1999:blog-555", "blogger.com", 1999, null, null, "blog-555")]
        [InlineData("tag:yaml.org,2002:int", "yaml.org", 2002, null, null, "int")]
        public void Examples(string expectedUrl, string authorityName, int year, int? month, int? day, string specific)
        {
            // See 2.1
            var uri = new TagUniformResourceIdentifierBuilder()
                .WithAuthorityName(authorityName).WithDateYear(year).WithDateMonth(month).WithDateDay(day).WithSpecific(specific)
                .Build();
            var uriString = uri.ToString();
            Assert.Equal(expectedUrl, uriString);

            var parsed = TagUniformResourceIdentifier.Parse(uriString);
            Assert.Equal(authorityName, parsed.AuthorityName);
            Assert.Equal(year, parsed.DateYear);
            Assert.Equal(month, parsed.DateMonth);
            Assert.Equal(day, parsed.DateDay);
            Assert.Equal(specific, parsed.Specific);
        }
    }
}
