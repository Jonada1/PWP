using Xunit;

namespace Limping.Api.Tests.Fixtures
{
    [CollectionDefinition(nameof(WebHostCollection))]
    public class WebHostCollection : ICollectionFixture<WebHostFixture>
    {
    }
}
