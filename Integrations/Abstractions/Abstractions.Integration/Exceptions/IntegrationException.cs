﻿namespace Announcarr.Integrations.Abstractions.Exceptions;

public class IntegrationException : Exception
{
    public IntegrationException()
    {
    }

    public IntegrationException(string? message) : base(message)
    {
    }

    public IntegrationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}