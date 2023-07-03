// Copyright (c) Microsoft Corporation. All Rights Reserved.

using Microsoft.R9.Extensions.Enrichment;

namespace MinimalSample;

/// <summary>
/// Defines methods to manipulate collections of enrichment properties.
/// </summary>
/// <remarks>
/// This interface is relevant only if you implement a custom enricher.
/// </remarks>
#pragma warning disable S4023 // Interfaces should not be empty - it is a placeholder for the future API modifications
public interface IHttpMetricEnrichmentPropertyBag : IEnrichmentPropertyBag
{
}
