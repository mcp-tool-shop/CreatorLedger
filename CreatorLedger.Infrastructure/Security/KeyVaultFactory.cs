using System.Runtime.InteropServices;
using CreatorLedger.Domain.Identity;

namespace CreatorLedger.Infrastructure.Security;

/// <summary>
/// Factory for creating platform-appropriate key vaults.
/// </summary>
public static class KeyVaultFactory
{
    /// <summary>
    /// Key vault type to use.
    /// </summary>
    public enum VaultType
    {
        /// <summary>
        /// Automatically select based on platform.
        /// Windows: DPAPI, Linux: libsecret, macOS: Keychain.
        /// </summary>
        Auto,

        /// <summary>
        /// Windows DPAPI-based vault. Only works on Windows.
        /// </summary>
        Dpapi,

        /// <summary>
        /// Linux libsecret vault (GNOME Keyring / KDE Wallet). Only works on Linux.
        /// </summary>
        LibSecret,

        /// <summary>
        /// macOS Keychain vault. Only works on macOS.
        /// </summary>
        Keychain,

        /// <summary>
        /// In-memory vault for development/testing.
        /// Keys are NOT persisted and NOT secure.
        /// </summary>
        InMemory
    }

    /// <summary>
    /// Creates a key vault based on the specified type.
    /// </summary>
    /// <param name="vaultType">The type of vault to create.</param>
    /// <param name="baseDirectory">
    /// Base directory for file-based vaults (DPAPI only).
    /// If null, uses the default location (%LOCALAPPDATA%\CreatorLedger).
    /// </param>
    /// <returns>An IKeyVault implementation.</returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Thrown when a platform-specific vault is requested on an unsupported platform.
    /// </exception>
    public static IKeyVault Create(VaultType vaultType, string? baseDirectory = null)
    {
        return vaultType switch
        {
            VaultType.Auto => CreateAuto(baseDirectory),
            VaultType.Dpapi => CreateDpapi(baseDirectory),
            VaultType.LibSecret => CreateLibSecret(),
            VaultType.Keychain => CreateKeychain(),
            VaultType.InMemory => new InMemoryKeyVault(),
            _ => throw new ArgumentOutOfRangeException(nameof(vaultType))
        };
    }

    /// <summary>
    /// Creates a vault using automatic platform detection.
    /// </summary>
    private static IKeyVault CreateAuto(string? baseDirectory)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return CreateDpapi(baseDirectory);
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            try
            {
                return CreateLibSecret();
            }
            catch (PlatformNotSupportedException ex)
            {
                Console.Error.WriteLine(
                    $"WARNING: {ex.Message}. " +
                    "Falling back to in-memory vault - keys will NOT be persisted.");
                return new InMemoryKeyVault();
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return CreateKeychain();
        }

        // Unknown platform: use in-memory vault with a warning
        Console.Error.WriteLine(
            "WARNING: Unknown platform. " +
            "Using in-memory key vault - keys will NOT be persisted.");

        return new InMemoryKeyVault();
    }

    /// <summary>
    /// Creates a DPAPI vault. Throws on non-Windows.
    /// </summary>
    private static IKeyVault CreateDpapi(string? baseDirectory)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException(
                "DPAPI key vault is only supported on Windows. " +
                "Use VaultType.LibSecret (Linux), VaultType.Keychain (macOS), " +
                "or VaultType.InMemory for development/testing.");
        }

        if (baseDirectory is not null)
        {
            return new DpapiKeyVault(baseDirectory);
        }

        return DpapiKeyVault.CreateDefault();
    }

    /// <summary>
    /// Creates a libsecret vault (Linux). Throws on non-Linux or if libsecret-tools is not installed.
    /// </summary>
    private static IKeyVault CreateLibSecret()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            throw new PlatformNotSupportedException(
                "libsecret key vault is only supported on Linux. " +
                "Use VaultType.Dpapi (Windows), VaultType.Keychain (macOS), " +
                "or VaultType.InMemory for development/testing.");
        }

        return new LibSecretKeyVault();
    }

    /// <summary>
    /// Creates a Keychain vault (macOS). Throws on non-macOS.
    /// </summary>
    private static IKeyVault CreateKeychain()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            throw new PlatformNotSupportedException(
                "Keychain key vault is only supported on macOS. " +
                "Use VaultType.Dpapi (Windows), VaultType.LibSecret (Linux), " +
                "or VaultType.InMemory for development/testing.");
        }

        return new KeychainKeyVault();
    }

    /// <summary>
    /// Creates a vault for development/testing (always in-memory).
    /// This method is explicit about not providing secure storage.
    /// </summary>
    public static IKeyVault CreateForDevelopment()
    {
        return new InMemoryKeyVault();
    }
}
