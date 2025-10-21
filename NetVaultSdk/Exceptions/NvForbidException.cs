using System;

namespace NetVaultSdk.Exceptions
{
    /// <summary>
    /// Used when you do not have access to the secret.
    /// </summary>
    public class NvForbidException: Exception
    {
        /// <summary>
        /// A human-readable description of the error.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Creates a new instance of NvForbidException.
        /// </summary>
        /// <param name="message">A human-readable description of the error.</param>
        public NvForbidException(string message)
        {
            ErrorMessage = message;
        }
    }
}