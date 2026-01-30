using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Cryptography;
using CreatorLedger.Domain.Identity;
using CreatorLedger.Domain.Primitives;
using Shared.Crypto;

namespace CreatorLedger.Infrastructure.Security;

/// <summary>
/// macOS Keychain-based secure key vault.
/// Stores keys in the user's login keychain with automatic encryption.
///
/// Keys are stored with:
/// - Service: CreatorLedger
/// - Account: {creatorId}
/// - Kind: application password
/// </summary>
[SupportedOSPlatform("macos")]
public sealed class KeychainKeyVault : IKeyVault
{
    private const string ServiceName = "CreatorLedger";

    public Task StoreAsync(CreatorId creatorId, Ed25519PrivateKey privateKey, CancellationToken cancellationToken = default)
    {
        var seedBytes = privateKey.AsBytes().ToArray();

        try
        {
            // Encode as base64 for safe storage
            var base64Seed = Convert.ToBase64String(seedBytes);

            // First, delete any existing entry
            DeleteKeyInternal(creatorId);

            // Add to keychain
            var startInfo = new ProcessStartInfo
            {
                FileName = "security",
                Arguments = $"add-generic-password -s \"{ServiceName}\" -a \"{creatorId}\" -w \"{base64Seed}\" -U",
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
                throw new InvalidOperationException("Failed to start security command");

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                var error = process.StandardError.ReadToEnd();
                throw new InvalidOperationException($"Keychain add-generic-password failed: {error}");
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
                FileName = "security",
                Arguments = $"find-generic-password -s \"{ServiceName}\" -a \"{creatorId}\" -w",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
                throw new InvalidOperationException("Failed to start security command");

            var base64Seed = process.StandardOutput.ReadToEnd().Trim();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                // Exit code 44 = item not found
                if (error.Contains("could not be found") || process.ExitCode == 44)
                    return Task.FromResult<Ed25519PrivateKey?>(null);

                throw new InvalidOperationException($"Keychain find-generic-password failed: {error}");
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
        return Task.FromResult(DeleteKeyInternal(creatorId));
    }

    public Task<bool> ExistsAsync(CreatorId creatorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "security",
                Arguments = $"find-generic-password -s \"{ServiceName}\" -a \"{creatorId}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
                return Task.FromResult(false);

            process.WaitForExit();

            // Exit code 0 = found
            return Task.FromResult(process.ExitCode == 0);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private static bool DeleteKeyInternal(CreatorId creatorId)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "security",
                Arguments = $"delete-generic-password -s \"{ServiceName}\" -a \"{creatorId}\"",
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
                return false;

            process.WaitForExit();

            // Exit code 0 = deleted, 44 = not found (both OK for delete)
            return process.ExitCode == 0 || process.ExitCode == 44;
        }
        catch
        {
            return false;
        }
    }
}
