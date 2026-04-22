using Soenneker.Tests.HostedUnit;

namespace Soenneker.SemanticKernel.Pool.OpenAi.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class KernelPoolOpenAiExtensionTests : HostedUnitTest
{
    public KernelPoolOpenAiExtensionTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}
