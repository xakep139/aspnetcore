// Copyright (c) Microsoft Corporation. All Rights Reserved.

namespace MinimalSample;

/// <summary>
/// Interface for implementing enrichers for incoming request metrics.
/// </summary>
public interface IHttpMetricEnricher
{
    /// <summary>
    /// Enrich properties.
    /// </summary>
    /// <param name="enrichmentBag">Properties to be enriched.</param>
    /// <param name="context">The <see cref="HttpContext"/>.</param>
    /// <param name="exception">The <see cref="Exception"/> that might be thrown during the request processing.</param>
    void Enrich(IHttpMetricEnrichmentPropertyBag enrichmentBag, HttpContext context, Exception? exception);
}

