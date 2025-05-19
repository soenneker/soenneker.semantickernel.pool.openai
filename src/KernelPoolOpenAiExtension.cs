using Microsoft.SemanticKernel;
using OpenAI;
using Soenneker.Extensions.ValueTask;
using Soenneker.SemanticKernel.Dtos.Options;
using Soenneker.SemanticKernel.Pool.Abstract;
using System;
using System.ClientModel;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.SemanticKernel.Pool.OpenAi;

/// <summary>
/// Provides OpenAI-specific registration extensions for KernelPoolManager, enabling integration with local LLMs via Semantic Kernel.
/// </summary>
public static class KernelPoolOpenAiExtension
{
    /// <summary>
    /// Registers an OpenAi model in the kernel pool with optional rate and token limits.
    /// </summary>
    /// <param name="pool">The kernel pool manager to register the model with.</param>
    /// <param name="key">A unique identifier used to register and later reference the model.</param>
    /// <param name="modelId">The OpenAi model ID to be used for chat completion.</param>
    /// <param name="apiKey"></param>
    /// <param name="endpoint">The base URI endpoint for the OpenAi service.</param>
    /// <param name="rps">Optional maximum number of requests allowed per second.</param>
    /// <param name="rpm">Optional maximum number of requests allowed per minute.</param>
    /// <param name="rpd">Optional maximum number of requests allowed per day.</param>
    /// <param name="tokensPerDay">Optional maximum number of tokens allowed per day.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous registration operation.</returns>
    public static ValueTask RegisterOpenAi(this IKernelPoolManager pool, string key, string modelId, string apiKey, string endpoint,
        int? rps, int? rpm, int? rpd, int? tokensPerDay = null, CancellationToken cancellationToken = default)
    {
        var options = new SemanticKernelOptions
        {
            ModelId = modelId,
            Endpoint = endpoint,
            RequestsPerSecond = rps,
            RequestsPerMinute = rpm,
            RequestsPerDay = rpd,
            TokensPerDay = tokensPerDay,
            KernelFactory = async (opts, _) =>
            {
#pragma warning disable SKEXP0070
                return Kernel.CreateBuilder()
                             .AddOpenAIChatCompletion(modelId: opts.ModelId!,
                                 new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions {Endpoint = new Uri(endpoint)}));
#pragma warning restore SKEXP0070
            }
        };

        return pool.Register(key, options, cancellationToken);
    }

    /// <summary>
    /// Unregisters an OpenAi model from the kernel pool and kernel cache entries.
    /// </summary>
    /// <param name="pool">The kernel pool manager to unregister the model from.</param>
    /// <param name="key">The unique identifier used during registration.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous unregistration operation.</returns>
    public static async ValueTask UnregisterOpenAi(this IKernelPoolManager pool, string key, CancellationToken cancellationToken = default)
    {
        await pool.Unregister(key, cancellationToken).NoSync();
    }
}