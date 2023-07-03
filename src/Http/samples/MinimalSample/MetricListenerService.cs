// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.Metrics;
using Microsoft.Extensions.Diagnostics.Metrics;

internal sealed class MetricListenerService : BackgroundService
{
    private readonly IMeterFactory _meterFactory;
    private readonly MeterListener _meterListener;

    public MetricListenerService(IMeterFactory meterFactory)
    {
        _meterListener = new MeterListener();
        _meterFactory = meterFactory;
    }

    private void OnMeasurementRecorded(Instrument instrument, double measurement, ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        Console.WriteLine("Instrument: {0}, value: {1}, tags: {2}", instrument.Name, measurement, string.Join("; ", tags.ToArray()));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Listen to the built-in ASP.NET Core counter.
        _meterListener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Meter.Scope == _meterFactory &&
                instrument.Meter.Name == "Microsoft.AspNetCore.Hosting" &&
                instrument.Name == "http-server-request-duration")
            {
                listener.EnableMeasurementEvents(instrument);
            }
        };

        _meterListener.SetMeasurementEventCallback<double>(OnMeasurementRecorded);
        _meterListener.Start();

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override void Dispose()
    {
        base.Dispose();
        _meterListener?.Dispose();
    }
}
