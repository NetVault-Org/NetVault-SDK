namespace NetVaultSdk.Models
{
    /// <summary>
    /// Payload sent to NetVault to request a secret recovery.
    /// </summary>
    public class NvSecretRecoveryRequest
    {
        /// <summary>
        /// The NetVault application/client identifier.
        /// </summary>
        public string ClientId { get; set; }
        
        /// <summary>
        /// The NetVault application/client secret.
        /// </summary>
        public string ClientSecret { get; set; }
        
        /// <summary>
        /// The unique name/key of the secret to be recovered.
        /// </summary>
        public string SecretName { get; set; }
        
        /// <summary>
        /// Optional owner identifier used when the secret is shared with you.
        /// </summary>
        public string OwnerId { get; set; }
    }
}