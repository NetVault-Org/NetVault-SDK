using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NetVaultSdk.Exceptions;
using NetVaultSdk.Models;

namespace NetVaultSdk
{
    /// <summary>
    /// Client for retrieving application secrets from a NetVault instance.
    /// </summary>
    /// <remarks>
    /// NetVault is a secret manager service (similar to AWS Secrets Manager). This client authenticates
    /// using a ClientId and ClientSecret and calls the NetVault recovery endpoint to fetch a secret value.
    /// It supports either direct constructor-provided configuration or environment variables.
    /// </remarks>
    public class NetVaultClient: INetVaultClient
    {
        private readonly string _baseUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly bool _acceptInsecureConnection;
        private readonly JsonSerializerOptions _jsonSerializerOptions = JsonSerializerOptions.Web;
        private HttpClient _httpClient;
        
        /// <summary>
        /// Creates a NetVault client using explicit credentials and endpoint.
        /// </summary>
        /// <param name="clientId">The NetVault application/client identifier.</param>
        /// <param name="clientSecret">The NetVault application/client secret.</param>
        /// <param name="baseUrl">The base URL of the NetVault service (e.g., https://vault.example.com).</param>
        /// <param name="acceptInsecureConnection">If true, disables TLS certificate validation for development/local testing.</param>
        public NetVaultClient(string clientId, string clientSecret, string baseUrl, bool acceptInsecureConnection = false)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _baseUrl = baseUrl;
            _acceptInsecureConnection = acceptInsecureConnection;
            PrepareHttpClient();
        }
        
        /// <summary>
        /// Creates a NetVault client using environment variables for configuration.
        /// </summary>
        /// <param name="acceptInsecureConnection">Optional override; if provided as true, forces insecure connections regardless of NV_ACCEPT_INSECURE.</param>
        /// <remarks>
        /// The following environment variables are required unless provided by the other constructor:
        /// - NV_URL: NetVault base URL (e.g., https://vault.example.com)
        /// - NV_CLIENT_ID: NetVault application/client identifier
        /// - NV_CLIENT_SECRET: NetVault application/client secret
        ///
        /// Optional:
        /// - NV_ACCEPT_INSECURE: true/false to disable certificate validation (development only)
        ///
        /// If both the constructor parameter and environment variable are provided for insecure mode,
        /// the environment variable value is used.
        /// </remarks>
        public NetVaultClient(bool acceptInsecureConnection = false)
        {
            _acceptInsecureConnection = acceptInsecureConnection;
            _baseUrl = GetRequiredEnvVar("NV_URL");
            _clientId = GetRequiredEnvVar("NV_CLIENT_ID");
            _clientSecret = GetRequiredEnvVar("NV_CLIENT_SECRET");

            var insecureEnv = Environment.GetEnvironmentVariable("NV_ACCEPT_INSECURE");
            if (bool.TryParse(insecureEnv, out var insecureFlag))
                _acceptInsecureConnection = insecureFlag;

            PrepareHttpClient();
        }

        private static string GetRequiredEnvVar(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrWhiteSpace(value) ? throw new NvEnvironmentMissingException($"Missing required environment variable: {name}") : value;
        }

        private void PrepareHttpClient()
        {
            if (_acceptInsecureConnection)
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
                _httpClient = new HttpClient(handler);
            }
            else
            {
                _httpClient = new HttpClient();
            }

            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        /// <summary>
        /// Retrieves a secret value from NetVault and deserializes it to the specified type.
        /// </summary>
        /// <typeparam name="T">The expected type of the secret value. Use string to get the raw textual value.</typeparam>
        /// <param name="secretName">The unique name/key of the secret in NetVault.</param>
        /// <param name="ownerId">Optional, owner id of the secret, used only to get shared secret.</param>
        /// <returns>
        /// A task that resolves with the secret value converted to type <typeparamref name="T"/>.
        /// If <typeparamref name="T"/> is string, the base64-decoded content is returned as-is.
        /// Otherwise, the decoded JSON content is deserialized into <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="NvCredentialException">Thrown when your client secret does not work.</exception>
        /// <exception cref="NvForbidException">Thrown when you do not have access to the secret.</exception>
        /// <exception cref="NvKeyException">Thrown when the secret is not found.</exception>
        /// <exception cref="NvUnhandledException">Thrown for other error conditions returned by NetVault.</exception>
        public async Task<T> GetSecretValueAsync<T>(string secretName, Guid? ownerId = null)
        {
            var request = new NvSecretRecoveryRequest
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                SecretName = secretName,
                OwnerId = ownerId.ToString()
            };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync($"{_baseUrl}/api/v1/secrets/recovery", content);
            result.EnsureSuccessStatusCode();
            var response = JsonSerializer.Deserialize<NvSecretRecoveryResponse>(await result.Content.ReadAsStringAsync(), _jsonSerializerOptions);

            if (response.Success)
            {
                var base64Bytes = Convert.FromBase64String(response.Data);
                var dataStr = Encoding.UTF8.GetString(base64Bytes);
                if (typeof(T) == typeof(string))
                    return (T)(object)dataStr;
                return JsonSerializer.Deserialize<T>(dataStr, _jsonSerializerOptions);
            }

            switch (response.ErrorCode)
            {
                case (int) HttpStatusCode.Forbidden:
                    throw new NvCredentialException(response.ErrorMessage);
                case (int) HttpStatusCode.Unauthorized:
                    throw new NvForbidException(response.ErrorMessage);
                case (int) HttpStatusCode.NotFound:
                    throw new NvKeyException(response.ErrorMessage);
                default: throw new NvUnhandledException(response.ErrorMessage);
            }
        }
    }
}