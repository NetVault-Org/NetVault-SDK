namespace NetVaultSdk.Models
{
    /// <summary>
    /// Response returned by NetVault after a secret recovery request.
    /// </summary>
    public class NvSecretRecoveryResponse
    {
        /// <summary>
        /// Indicates whether the operation succeeded.
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Error message provided by NetVault when <see cref="Success"/> is false.
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// HTTP-like error code provided by NetVault when <see cref="Success"/> is false.
        /// </summary>
        public int? ErrorCode { get; set; }
        /// <summary>
        /// Base64-encoded secret data returned by NetVault when <see cref="Success"/> is true.
        /// </summary>
        public string Data { get; set; }
    }
}