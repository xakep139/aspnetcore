// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http.Features;
using Microsoft.R9.Extensions.Pools;

namespace MinimalSample;

public class EnrichmentMiddleware : IMiddleware
{
    private readonly IHttpMetricEnricher[] _enrichers;
    private readonly ScaledObjectPool<HttpMetricEnrichmentPropertyBag> _propertyBagPool = PoolFactory.CreateResettingPool<HttpMetricEnrichmentPropertyBag>();
    private readonly bool _enrichmentEnabled;

    public EnrichmentMiddleware(IEnumerable<IHttpMetricEnricher> enrichers)
    {
        _enrichers = enrichers.ToArray();
        if (_enrichers.Length > 0)
        {
            _enrichmentEnabled = true;
        }
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (_enrichmentEnabled)
        {
            var callbackState = new CallbackState(context, _enrichers, _propertyBagPool);
            context.Response.OnCompleted(static state =>
            {
                var typedState = (CallbackState)state;
                var enrichmentPropertyBag = typedState.EnrichmentBagPool.Get();
                foreach (var enricher in typedState.Enrichers)
                {
                    enricher.Enrich(enrichmentPropertyBag, typedState.Context, null);
                }

                var feature = typedState.Context.Features.Get<IHttpMetricsTagsFeature>();
                if (feature == null)
                {
                    feature = new HttpMetricsTagsFeature();
                    typedState.Context.Features.Set(feature);
                }

                foreach (var item in enrichmentPropertyBag)
                {
                    feature.Tags.Add(new(item.Key, item.Value));
                }

                typedState.EnrichmentBagPool.Return(enrichmentPropertyBag);

                return Task.CompletedTask;
            }, callbackState);
        }

        await next(context);
    }

    private readonly record struct CallbackState(
        HttpContext Context,
        IHttpMetricEnricher[] Enrichers,
        ScaledObjectPool<HttpMetricEnrichmentPropertyBag> EnrichmentBagPool);

    private sealed class HttpMetricsTagsFeature : IHttpMetricsTagsFeature
    {
        ICollection<KeyValuePair<string, object?>> IHttpMetricsTagsFeature.Tags => TagsList;

        public List<KeyValuePair<string, object?>> TagsList { get; } = new List<KeyValuePair<string, object?>>();
    }
}
