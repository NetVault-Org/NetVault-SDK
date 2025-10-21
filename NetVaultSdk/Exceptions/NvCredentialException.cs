using System;

namespace NetVaultSdk.Exceptions
{
    /// <summary>
    /// Represents a error on your credentials, invalid or inactive client id and secret
    /// </summary>
    public class NvCredentialException : Exception
    {
        /// <summary>
        /// A human-readable description of the error.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Creates a new instance of NvCredentialException.
        /// </summary>
        /// <param name="message">A human-readable description of the error.</param>
        public NvCredentialException(string message)
        {
            ErrorMessage = message;
        }
    }
}