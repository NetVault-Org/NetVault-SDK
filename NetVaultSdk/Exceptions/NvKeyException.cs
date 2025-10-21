using System;

namespace NetVaultSdk.Exceptions
{
    /// <summary>
    /// Represents a not-found error when the requested secret does not exist in NetVault.
    /// </summary>
    public class NvKeyException : Exception
    {
        /// <summary>
        /// A human-readable description of the error.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Creates a new instance of NvKeyException.
        /// </summary>
        /// <param name="message">A human-readable description of the error.</param>
        public NvKeyException(string message)
        {
            ErrorMessage = message;
        }
    }
}