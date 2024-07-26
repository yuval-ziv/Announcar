﻿using Announcarr.Integrations.Abstractions.Interfaces;

namespace Announcarr.Integrations.Radarr.Integration.Configurations;

public class RadarrIntegrationConfiguration : IIntegrationConfiguration
{
    public bool IsEnabled { get; set; } = false;
    public bool IsGetCalendarEnabled { get; set; } = true;
    public bool IsGetRecentlyAddedEnabled { get; set; } = true;
    public string Url { get; set; } = "http://localhost:7878";
    public string? ApiKey { get; set; }
    public bool IgnoreCertificateValidation { get; set; } = false;
    public string? Name { get; set; } = null;
}