﻿namespace Announcarr.Integrations.Radarr.Integration.Configurations;

public class RadarrIntegrationConfiguration
{
    public bool IsEnabled { get; set; } = false;
    public string? Name { get; set; } = null;
    public string Url { get; set; } = "http://localhost:7878";
    public string? ApiKey { get; set; }
    public bool IgnoreCertificateValidation { get; set; } = false;
}