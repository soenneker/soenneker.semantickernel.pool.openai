using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.SemanticKernel.Pool.OpenAi.Tests;

[Collection("Collection")]
public class KernelPoolOpenAiExtensionTests : FixturedUnitTest
{
    public KernelPoolOpenAiExtensionTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Default()
    {

    }
}
