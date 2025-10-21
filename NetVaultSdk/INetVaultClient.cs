using System;
using System.Threading.Tasks;

namespace NetVaultSdk
{
    /// <summary>
    /// Abstraction for a NetVault client capable of retrieving secret values.
    /// </summary>
    public interface INetVaultClient
    {
        /// <summary>
        /// Retrieves a secret value from NetVault and converts it to the requested type.
        /// </summary>
        /// <typeparam name="T">The expected type of the secret value. Use string to get the raw textual value.</typeparam>
        /// <param name="secretName">The unique name/key of the secret in NetVault.</param>
        /// <param name="ownerId">Optional, owner id of the secret, used only to get shared secret.</param>
        /// <returns>A task that resolves with the secret value as type <typeparamref name="T"/>.</returns>
       Task<T> GetSecretValueAsync<T>(string secretName, Guid? ownerId = null);
    }
}