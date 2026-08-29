[![](https://img.shields.io/nuget/v/soenneker.semantickernel.pool.openai.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.semantickernel.pool.openai/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.pool.openai/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.pool.openai/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.semantickernel.pool.openai.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.semantickernel.pool.openai/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.semantickernel.pool.openai/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.semantickernel.pool.openai/actions/workflows/codeql.yml)

# Soenneker.SemanticKernel.Pool.OpenAi

Provides OpenAI-specific registration extensions for KernelPoolManager, enabling integration with OpenAI models via Semantic Kernel.

## Install

```bash
dotnet add package Soenneker.SemanticKernel.Pool.OpenAi
```

## Quick start

```csharp
using Soenneker.SemanticKernel.Pool.OpenAi;

ISemanticKernelPool pool = /* obtain from your application */;
await pool.AddOpenAi("value", "value", /* supply type */ default!, "value", "value", "value", /* supply httpClientCache */ default!, 1, 1, 1, default);
```

Registers an OpenAI model in the kernel pool with the specified kernel type and optional rate/token limits.

## What you get

- `SemanticKernelPoolOpenAiExtension` — Provides OpenAI-specific registration extensions for KernelPoolManager, enabling integration with OpenAI models via Semantic Kernel.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `SemanticKernelPoolOpenAiExtension.AddOpenAi(pool, poolId, key, type, modelId, apiKey, endpoint, httpClientCache, rps, rpm, rpd, tokensPerDay, cancellationToken)` | Registers an OpenAI model in the kernel pool with the specified kernel type and optional rate/token limits. | A task that completes when the open ai addition is complete. |
| `SemanticKernelPoolOpenAiExtension.RemoveOpenAi(pool, poolId, key, httpClientCache, cancellationToken)` | Unregisters an OpenAI model from the kernel pool and removes the associated kernel cache entry. | A task that completes when the open ai removal is complete. |

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
