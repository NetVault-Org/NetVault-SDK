using System;

namespace NetVaultSdk.Exceptions
{
    /// <summary>
    /// Thrown when a required environment variable for configuring NetVaultClient is missing or empty.
    /// </summary>
    public class NvEnvironmentMissingException: Exception
    {
        /// <summary>
        /// A human-readable description of the missing variable.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Creates a new instance of NvEnvironmentMissingException.
        /// </summary>
        /// <param name="message">A human-readable description of the error.</param>
        public NvEnvironmentMissingException(string message)
        {
            ErrorMessage = message;
        }
    }
}