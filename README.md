[![](https://img.shields.io/nuget/v/soenneker.semantickernel.pool.openai.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.semantickernel.pool.openai/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.pool.openai/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.pool.openai/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.semantickernel.pool.openai.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker/soenneker.semantickernel.pool.openai/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.pool.openai/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.pool.openai/actions/workflows/codeql.yml)

# Soenneker.SemanticKernel.Pool.OpenAi

OpenAI connector registration helpers for `Soenneker.SemanticKernel.Pool`.

## Installation

```bash
dotnet add package Soenneker.SemanticKernel.Pool.OpenAi
```

## Add an OpenAI entry

Resolve the pool and HTTP client cache from dependency injection, then register an entry:

```csharp
using Soenneker.SemanticKernel.Enums.KernelType;
using Soenneker.SemanticKernel.Pool.OpenAi;

await pool.AddOpenAi(
    poolId: "chat",
    key: "openai-primary",
    type: KernelType.Chat,
    modelId: "chat-model-id",
    apiKey: configuration["OpenAI:ApiKey"]!,
    endpoint: "https://api.openai.com/v1",
    httpClientCache: httpClientCache,
    rps: 2,
    rpm: 60,
    rpd: 1_000,
    tokensPerDay: null,
    cancellationToken);
```

Supported types are:

- `KernelType.Chat` for chat completion
- `KernelType.Embedding` for embedding generation
- `KernelType.Audio` for text-to-audio
- `KernelType.Image` for text-to-image

Other types throw `NotSupportedException` when the pool first constructs the kernel.

Chat and embedding create an `OpenAIClient` using the supplied endpoint. Audio and image use Semantic Kernel connector overloads that receive the API key and cached `HttpClient`, but this adapter does not apply the supplied endpoint to those paths. Use this package's audio/image registrations only when the connector's default endpoint is appropriate.

The HTTP client is cached under `openai:{poolId}:{key}` with a five-minute timeout. Pool quota values are reservations made when `GetAvailable` selects the entry. `tokensPerDay` counts one unit per acquisition; it is not populated from provider token usage.

## Remove the entry

Use the matching helper so both the pool entry and cached HTTP client are removed:

```csharp
await pool.RemoveOpenAi(
    "chat",
    "openai-primary",
    httpClientCache,
    cancellationToken);
```

Keep the API key in a protected configuration provider and avoid logging or serializing the generated `SemanticKernelOptions`.
