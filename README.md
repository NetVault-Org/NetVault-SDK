# NetVaultSdk

A lightweight .NET client for retrieving application secrets from a NetVault server (a secrets manager similar in purpose to AWS Secrets Manager). The SDK handles authentication with ClientId/ClientSecret and provides a simple API to recover a secret by name.

- Target framework: netstandard2.0 (compatible with .NET Framework 4.6.1+ and .NET Core/.NET 5+)

## Features
- Configure via constructor parameters or environment variables
- Optional insecure connection mode for local development (skips TLS certificate validation)
- Strongly-typed secret retrieval with JSON deserialization
- Clear error mapping to custom exceptions

## Installation

Nuget installation:

```
Install-Package NetVaultSdk
```

## Configuration
You can configure the client either explicitly via the constructor or implicitly via environment variables.

### Constructor configuration
```
var client = new NetVaultSdk.NetVaultClient(
    clientId: "<YOUR_CLIENT_ID>",
    clientSecret: "<YOUR_CLIENT_SECRET>",
    baseUrl: "https://vault.example.com",
    acceptInsecureConnection: false // true only for local/dev
);
```

### Environment variables
When using the parameterless constructor, the following environment variables are read:

- NV_URL: NetVault base URL (e.g., https://vault.example.com)
- NV_CLIENT_ID: NetVault application/client identifier
- NV_CLIENT_SECRET: NetVault application/client secret
- NV_ACCEPT_INSECURE (optional): true/false to disable TLS certificate validation (development only)

Example:
```
Environment.SetEnvironmentVariable("NV_CLIENT_ID", "<YOUR_CLIENT_ID>");
Environment.SetEnvironmentVariable("NV_CLIENT_SECRET", "<YOUR_CLIENT_SECRET>");
Environment.SetEnvironmentVariable("NV_URL", "https://vault.example.com");
Environment.SetEnvironmentVariable("NV_ACCEPT_INSECURE", "false");

var client = new NetVaultSdk.NetVaultClient();
```

Notes:
- If both the constructor parameter `acceptInsecureConnection` and NV_ACCEPT_INSECURE are provided, the environment variable value takes precedence.
- When `acceptInsecureConnection` is true (or NV_ACCEPT_INSECURE is true), the client will skip server certificate validation. Use only for development.

## Usage
The main API is `GetSecretValueAsync<T>(string secretName, Guid? ownerId = null)`.

- If T is `string`, the SDK returns the secret as a plain string.
- For other types, the SDK expects the secret to be JSON and deserializes it into `T`.

### Get a secret as string
```
var secret = await client.GetSecretValueAsync<string>(
    secretName: "my_service_api_key",
    ownerId: null // or a specific tenant/project/application Guid if required
);
```

### Get a complex secret as a typed object
```
public sealed class DbCredentials
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
}

var credentials = await client.GetSecretValueAsync<DbCredentials>("prod_db_credentials");
```

### Shared secrets
Shared secrets can be get passing the owner id of the secret.
```
var ownerId = Guid.Parse("cd7e4090-b07d-4725-b863-bcaa11c0750d");
var secret = await client.GetSecretValueAsync<string>("my_scoped_secret", ownerId);
```

## Error handling
`GetSecretValueAsync` maps NetVault error responses into custom exceptions:

- NvCredentialException (HTTP 403): Invalid credentials (client secret does not work)
- NvForbidException (HTTP 401): Unauthorized (no access to the secret)
- NvKeyException (HTTP 404): Secret not found
- NvUnhandledException (other errors): Unexpected error category from NetVault
- NvEnvironmentMissingException: Thrown on startup when a required environment variable is missing

Example:
```
try
{
    var value = await client.GetSecretValueAsync<string>("my_secret");
}
catch (NetVaultSdk.Exceptions.NvCredentialException ex)
{
    // handle 403 Invalid credentials
}
catch (NetVaultSdk.Exceptions.NvForbidException ex)
{
    // handle 401 Unauthorized
}
catch (NetVaultSdk.Exceptions.NvKeyException ex)
{
    // handle 404 Not Found
}
catch (NetVaultSdk.Exceptions.NvUnhandledException ex)
{
    // handle other/unexpected errors
}
```

## How it works
- The client sends a POST to `{baseUrl}/api/v1/secrets/recovery` with the body:
  - ClientId, ClientSecret, SecretName, OwnerId (optional)
- On success, NetVault returns base64-encoded secret data, which the client decodes:
  - If T is `string`, returns the decoded string.
  - Otherwise, deserializes the decoded JSON into T.

## Development and testing
- For local environments with self-signed certificates, set `NV_ACCEPT_INSECURE=true` or pass `acceptInsecureConnection: true` (development only).