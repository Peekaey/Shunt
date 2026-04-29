using System;
using System.Net;
using System.Net.Sockets;

namespace Shunt.Main.Utilities;

public static class ServerAddressHelper
{
    public static bool TryNormaliseServerHost(string? serverAddress, out string normalisedHost, out string errorMessage)
    {
        normalisedHost = string.Empty;
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(serverAddress))
        {
            errorMessage = "Server IP is required.";
            return false;
        }

        var candidate = serverAddress.Trim();

        if (Uri.TryCreate(candidate, UriKind.Absolute, out var absoluteUri))
        {
            if (!IsSupportedScheme(absoluteUri.Scheme))
            {
                errorMessage = "Server IP must use http or https when a scheme is provided.";
                return false;
            }

            if (!absoluteUri.IsDefaultPort)
            {
                errorMessage = "Server IP must not include a port. Use the Server Port field instead.";
                return false;
            }

            if (!string.IsNullOrEmpty(absoluteUri.Query) || !string.IsNullOrEmpty(absoluteUri.Fragment) ||
                !string.IsNullOrEmpty(absoluteUri.UserInfo) || !IsRootPath(absoluteUri.AbsolutePath))
            {
                errorMessage = "Server IP must only contain a host name or IP address.";
                return false;
            }

            normalisedHost = absoluteUri.IdnHost;
            return true;
        }

        if (candidate.Contains("://", StringComparison.Ordinal) ||
            candidate.Contains('/', StringComparison.Ordinal) ||
            candidate.Contains('?', StringComparison.Ordinal) ||
            candidate.Contains('#', StringComparison.Ordinal) ||
            candidate.Contains('@', StringComparison.Ordinal))
        {
            errorMessage = "Server IP must only contain a host name or IP address.";
            return false;
        }

        var uriCandidate = IPAddress.TryParse(candidate, out var ipAddress) && ipAddress.AddressFamily == AddressFamily.InterNetworkV6
            ? $"http://[{candidate}]"
            : $"http://{candidate}";

        if (!Uri.TryCreate(uriCandidate, UriKind.Absolute, out var hostUri) || string.IsNullOrWhiteSpace(hostUri.Host))
        {
            errorMessage = "Server IP must be a valid host name or IP address.";
            return false;
        }

        if (!hostUri.IsDefaultPort)
        {
            errorMessage = "Server IP must not include a port. Use the Server Port field instead.";
            return false;
        }

        normalisedHost = hostUri.IdnHost;
        return true;
    }

    public static bool TryBuildEndpointUri(string? serverAddress, int port, string relativePath, out Uri? endpointUri, out string errorMessage)
    {
        endpointUri = null;
        errorMessage = string.Empty;

        if (port < 1 || port > 65535)
        {
            errorMessage = "Server port must be between 1 and 65535.";
            return false;
        }

        if (!TryNormaliseServerHost(serverAddress, out var normalisedHost, out errorMessage))
        {
            return false;
        }

        endpointUri = new UriBuilder(Uri.UriSchemeHttp, normalisedHost, port)
        {
            Path = relativePath.TrimStart('/')
        }.Uri;

        return true;
    }

    private static bool IsSupportedScheme(string scheme) =>
        scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
        scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);

    private static bool IsRootPath(string absolutePath) =>
        string.IsNullOrEmpty(absolutePath) || absolutePath == "/";
}
