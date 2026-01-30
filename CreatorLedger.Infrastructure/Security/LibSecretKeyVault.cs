using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Cryptography;
using CreatorLedger.Domain.Identity;
using CreatorLedger.Domain.Primitives;
using Shared.Crypto;

namespace CreatorLedger.Infrastructure.Security;

/// <summary>
/// Linux secure key vault using libsecret (GNOME Keyring / KDE Wallet).
/// Stores keys in the system keyring with automatic encryption.
///
/// REQUIREMENTS:
/// - libsecret-tools must be installed: `sudo apt install libsecret-tools`
/// - User must have an active session (not headless)
///
/// Keys are stored with:
/// - Service: CreatorLedger
/// - Account: {creatorId}
/// - Label: CreatorLedger Key: {creatorId}
/// </summary>
[SupportedOSPlatform("linux")]
public sealed class LibSecretKeyVault : IKeyVault
{
    private const string ServiceName = "CreatorLedger";
    private const string LabelPrefix = "CreatorLedger Key";

    public LibSecretKeyVault()
    {
        // Verify secret-tool is available
        if (!IsSecretToolAvailable())
        {
            throw new PlatformNotSupportedException(
                "libsecret-tools is not installed. Install with: sudo apt install libsecret-tools");
        }
    }

    public Task StoreAsync(CreatorId creatorId, Ed25519PrivateKey privateKey, CancellationToken cancellationToken = default)
    {
        var seedBytes = privateKey.AsBytes().ToArray();

        try
        {
            // Encode as base64 for safe storage
            var base64Seed = Convert.ToBase64String(seedBytes);

            // Store using secret-tool
            var startInfo = new ProcessStartInfo
            {
                FileName = "secret-tool",
                Arguments = $"store --label=\"{LabelPrefix}: {creatorId}\" service {ServiceName} account {creatorId}",
                RedirectStandardInput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
                throw new InvalidOperationException("Failed to start secret-tool process");

            // Write secret to stdin
            process.StandardInput.Write(base64Seed);
            process.StandardInput.Close();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                var error = process.StandardError.ReadToEnd();
                throw new InvalidOperationException($"secret-tool store failed: {error}");
            }

            return Task.CompletedTask;
        }
        finally
        {
            CryptographicOperations.ZeroMemory(seedBytes);
        }
    }

    public Task<Ed25519PrivateKey?> RetrieveAsync(CreatorId creatorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "secret-tool",
                Arguments = $"lookup service {ServiceName} account {creatorId}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
                throw new InvalidOperationException("Failed to start secret-tool process");

            var base64Seed = process.StandardOutput.ReadToEnd().Trim();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                // Exit code 1 = not found
                if (process.ExitCode == 1)
                    return Task.FromResult<Ed25519PrivateKey?>(null);

                throw new InvalidOperationException($"secret-tool lookup failed: {error}");
            }

            if (string.IsNullOrWhiteSpace(base64Seed))
                return Task.FromResult<Ed25519PrivateKey?>(null);

            byte[]? seedBytes = null;

            try
            {
                seedBytes = Convert.FromBase64String(base64Seed);

                if (seedBytes.Length != Ed25519PrivateKey.ByteLength)
                {
                    throw new CryptographicException(
                        $"Retrieved key has invalid length: expected {Ed25519PrivateKey.ByteLength}, got {seedBytes.Length}");
                }

                var privateKey = Ed25519PrivateKey.FromBytes(seedBytes);
                return Task.FromResult<Ed25519PrivateKey?>(privateKey);
            }
            finally
            {
                if (seedBytes != null)
                    CryptographicOperations.ZeroMemory(seedBytes);
            }
        }
        catch (Exception ex) when (ex is not CryptographicException)
        {
            throw new InvalidOperationException($"Failed to retrieve key for creator {creatorId}", ex);
        }
    }

    public Task<bool> DeleteAsync(CreatorId creatorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "secret-tool",
                Arguments = $"clear service {ServiceName} account {creatorId}",
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
                return Task.FromResult(false);

            process.WaitForExit();

            // Exit code 0 = deleted, 1 = not found (both are success for delete)
            return Task.FromResult(process.ExitCode == 0);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> ExistsAsync(CreatorId creatorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "secret-tool",
                Arguments = $"lookup service {ServiceName} account {creatorId}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
                return Task.FromResult(false);

            process.WaitForExit();

            // Exit code 0 = found, 1 = not found
            return Task.FromResult(process.ExitCode == 0);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private static bool IsSecretToolAvailable()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "which",
                Arguments = "secret-tool",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
                return false;

            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
