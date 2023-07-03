// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MinimalSample;

internal class CustomMetricEnricher : IHttpMetricEnricher
{
    public void Enrich(IHttpMetricEnrichmentPropertyBag enrichmentBag, HttpContext context, Exception? exception)
    {
        enrichmentBag.Add("From", "CustomMetricEnricher");
        enrichmentBag.Add("NumberOfHeaders", context.Request.Headers.Count);
    }
}
