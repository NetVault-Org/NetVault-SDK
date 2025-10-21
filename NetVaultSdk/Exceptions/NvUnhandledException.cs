using System;

namespace NetVaultSdk.Exceptions
{
    /// <summary>
    /// Represents an unexpected or unclassified error returned by NetVault.
    /// </summary>
    public class NvUnhandledException: Exception
    {
        /// <summary>
        /// A human-readable description of the error.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Creates a new instance of NvUnhandledException.
        /// </summary>
        /// <param name="message">A human-readable description of the error.</param>
        public NvUnhandledException(string message)
        {
            ErrorMessage = message;
        }
    }
}