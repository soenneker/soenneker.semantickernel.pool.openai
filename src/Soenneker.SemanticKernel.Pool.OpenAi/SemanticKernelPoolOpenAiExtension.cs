using Microsoft.SemanticKernel;
using OpenAI;
using Soenneker.Dtos.HttpClientOptions;
using Soenneker.Extensions.ValueTask;
using Soenneker.SemanticKernel.Dtos.Options;
using Soenneker.SemanticKernel.Enums.KernelType;
using Soenneker.SemanticKernel.Pool.Abstract;
using Soenneker.Utils.HttpClientCache.Abstract;
using System;
using System.ClientModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.SemanticKernel.Pool.OpenAi;

/// <summary>
/// Provides OpenAI-specific registration extensions for KernelPoolManager, enabling integration with OpenAI models via Semantic Kernel.
/// </summary>
public static class SemanticKernelPoolOpenAiExtension
{
    /// <summary>
    /// Registers an OpenAI model in the kernel pool with the specified kernel type and optional rate/token limits.
    /// </summary>
    /// <param name="pool">Pool that supplies the reusable resource.</param>
    /// <param name="poolId">Identifier of the target pool.</param>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="type">Runtime type to inspect or construct.</param>
    /// <param name="modelId">Identifier of the model to use.</param>
    /// <param name="apiKey">API key used to authenticate the request.</param>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="httpClientCache">http Client Cache used to communicate with the external service.</param>
    /// <param name="rps">Optional requests-per-second limit.</param>
    /// <param name="rpm">Optional requests-per-minute limit.</param>
    /// <param name="rpd">Optional requests-per-day limit.</param>
    /// <param name="tokensPerDay">Optional daily token limit.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the open ai addition is complete.</returns>
    public static ValueTask AddOpenAi(this ISemanticKernelPool pool, string poolId, string key, KernelType type, string modelId, string apiKey, string endpoint,
        IHttpClientCache httpClientCache, int? rps, int? rpm, int? rpd, int? tokensPerDay = null, CancellationToken cancellationToken = default)
    {
        var options = new SemanticKernelOptions
        {
            Type = type,
            ModelId = modelId,
            Endpoint = endpoint,
            ApiKey = apiKey,
            RequestsPerSecond = rps,
            RequestsPerMinute = rpm,
            RequestsPerDay = rpd,
            TokensPerDay = tokensPerDay,
            KernelFactory = async (opts, _) =>
            {
                // No closure: static lambda with no state needed
                HttpClient httpClient = await httpClientCache.Get($"openai:{poolId}:{key}", static () => new HttpClientOptions
                {
                    Timeout = TimeSpan.FromSeconds(300)
                }, cancellationToken)
                .NoSync();

#pragma warning disable SKEXP0010
                var client = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
                {
                    Endpoint = new Uri(endpoint)
                });

                return type switch
                {
                    _ when type == KernelType.Chat => Kernel.CreateBuilder().AddOpenAIChatCompletion(opts.ModelId!, client),
                    _ when type == KernelType.Audio => Kernel.CreateBuilder().AddOpenAITextToAudio(opts.ModelId!, opts.ApiKey!, httpClient: httpClient),
                    _ when type == KernelType.Image => Kernel.CreateBuilder().AddOpenAITextToImage(opts.ApiKey!, null, opts.ModelId, httpClient: httpClient),
                    _ when type == KernelType.Embedding => Kernel.CreateBuilder().AddOpenAIEmbeddingGenerator(opts.ModelId!, client),

                    _ => throw new NotSupportedException($"Unsupported KernelType '{type}' for OpenAI registration.")
                };
#pragma warning restore SKEXP0010
            }
        };

        return pool.Add(poolId, key, options, cancellationToken);
    }

    /// <summary>
    /// Unregisters an OpenAI model from the kernel pool and removes the associated kernel cache entry.
    /// </summary>
    /// <param name="pool">Pool that supplies the reusable resource.</param>
    /// <param name="poolId">Identifier of the target pool.</param>
    /// <param name="key">Key used to locate the target entry.</param>
    /// <param name="httpClientCache">http Client Cache used to communicate with the external service.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the open ai removal is complete.</returns>
    public static async ValueTask RemoveOpenAi(this ISemanticKernelPool pool, string poolId, string key, IHttpClientCache httpClientCache, CancellationToken cancellationToken = default)
    {
        await pool.Remove(poolId, key, cancellationToken).NoSync();
        await httpClientCache.Remove($"openai:{poolId}:{key}").NoSync();
    }
}
