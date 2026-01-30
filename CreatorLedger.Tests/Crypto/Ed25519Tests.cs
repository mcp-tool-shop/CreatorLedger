using Shared.Crypto;

namespace CreatorLedger.Tests.Crypto;

public class Ed25519Tests
{
    [Fact]
    public void KeyPair_Generate_ProducesValidKeys()
    {
        using var keyPair = Ed25519KeyPair.Generate();

        Assert.NotNull(keyPair.PublicKey);
        Assert.NotNull(keyPair.PrivateKey);
        Assert.Equal(Ed25519PublicKey.ByteLength, keyPair.PublicKey.AsBytes().Length);
        Assert.Equal(Ed25519PrivateKey.ByteLength, keyPair.PrivateKey.AsBytes().Length);
    }

    [Fact]
    public void KeyPair_Generate_ProducesUniqueKeys()
    {
        using var keyPair1 = Ed25519KeyPair.Generate();
        using var keyPair2 = Ed25519KeyPair.Generate();

        Assert.NotEqual(keyPair1.PublicKey, keyPair2.PublicKey);
    }

    [Fact]
    public void Sign_Verify_RoundTrip()
    {
        using var keyPair = Ed25519KeyPair.Generate();
        var data = "message to sign"u8.ToArray();

        var signature = keyPair.Sign(data);
        var isValid = keyPair.Verify(data, signature);

        Assert.True(isValid);
    }

    [Fact]
    public void Verify_WrongData_Fails()
    {
        using var keyPair = Ed25519KeyPair.Generate();
        var originalData = "original message"u8.ToArray();
        var tamperedData = "tampered message"u8.ToArray();

        var signature = keyPair.Sign(originalData);
        var isValid = keyPair.PublicKey.Verify(tamperedData, signature);

        Assert.False(isValid);
    }

    [Fact]
    public void Verify_WrongSignature_Fails()
    {
        using var keyPair = Ed25519KeyPair.Generate();
        var data = "message"u8.ToArray();

        var signature = keyPair.Sign(data);

        // Tamper with signature
        var sigBytes = signature.AsBytes().ToArray();
        sigBytes[0] ^= 0xFF;
        var tamperedSig = Ed25519Signature.FromBytes(sigBytes);

        var isValid = keyPair.PublicKey.Verify(data, tamperedSig);

        Assert.False(isValid);
    }

    [Fact]
    public void Verify_WrongKey_Fails()
    {
        using var keyPair1 = Ed25519KeyPair.Generate();
        using var keyPair2 = Ed25519KeyPair.Generate();
        var data = "message"u8.ToArray();

        var signature = keyPair1.Sign(data);
        var isValid = keyPair2.PublicKey.Verify(data, signature);

        Assert.False(isValid);
    }

    [Fact]
    public void PublicKey_Parse_ToString_RoundTrip()
    {
        using var keyPair = Ed25519KeyPair.Generate();

        var encoded = keyPair.PublicKey.ToString();
        var parsed = Ed25519PublicKey.Parse(encoded);

        Assert.Equal(keyPair.PublicKey, parsed);
    }

    [Fact]
    public void PublicKey_ToString_HasCorrectPrefix()
    {
        using var keyPair = Ed25519KeyPair.Generate();

        var encoded = keyPair.PublicKey.ToString();

        Assert.StartsWith(Ed25519PublicKey.Prefix, encoded);
    }

    [Fact]
    public void PublicKey_Parse_InvalidPrefix_Throws()
    {
        Assert.Throws<FormatException>(() => Ed25519PublicKey.Parse("invalid:AAAA"));
    }

    [Fact]
    public void PublicKey_Parse_InvalidBase64_Throws()
    {
        Assert.Throws<FormatException>(() => Ed25519PublicKey.Parse("ed25519:not-valid-base64!!!"));
    }

    [Fact]
    public void PublicKey_TryParse_Invalid_ReturnsFalse()
    {
        var success = Ed25519PublicKey.TryParse("garbage", out var result);

        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void PublicKey_TryParse_Null_ReturnsFalse()
    {
        var success = Ed25519PublicKey.TryParse(null, out _);

        Assert.False(success);
    }

    [Fact]
    public void Signature_Parse_ToString_RoundTrip()
    {
        using var keyPair = Ed25519KeyPair.Generate();
        var signature = keyPair.Sign("test"u8.ToArray());

        var base64 = signature.ToString();
        var parsed = Ed25519Signature.Parse(base64);

        Assert.Equal(signature, parsed);
    }

    [Fact]
    public void Signature_TryParse_Invalid_ReturnsFalse()
    {
        var success = Ed25519Signature.TryParse("not-base64!!!", out _);

        Assert.False(success);
    }

    [Fact]
    public void Signature_TryParse_WrongLength_ReturnsFalse()
    {
        var shortBase64 = Convert.ToBase64String(new byte[32]); // Should be 64 bytes

        var success = Ed25519Signature.TryParse(shortBase64, out _);

        Assert.False(success);
    }

    [Fact]
    public void PrivateKey_FromBytes_RoundTrip()
    {
        using var original = Ed25519PrivateKey.Generate();
        var bytes = original.AsBytes().ToArray();

        using var restored = Ed25519PrivateKey.FromBytes(bytes);

        Assert.Equal(original.GetPublicKey(), restored.GetPublicKey());
    }

    [Fact]
    public void PrivateKey_Dispose_ClearsMemory()
    {
        var privateKey = Ed25519PrivateKey.Generate();
        var bytesBeforeDispose = privateKey.AsBytes().ToArray();

        privateKey.Dispose();

        // After dispose, accessing should throw
        Assert.Throws<ObjectDisposedException>(() => privateKey.AsBytes());
    }

    [Fact]
    public void KeyPair_Dispose_ClearsPrivateKey()
    {
        var keyPair = Ed25519KeyPair.Generate();
        keyPair.Dispose();

        Assert.Throws<ObjectDisposedException>(() => keyPair.PrivateKey);
        Assert.Throws<ObjectDisposedException>(() => keyPair.PublicKey);
        Assert.Throws<ObjectDisposedException>(() => keyPair.Sign("test"u8.ToArray()));
    }

    [Fact]
    public void KeyPair_FromPrivateKeyBytes_RestoresKeyPair()
    {
        using var original = Ed25519KeyPair.Generate();
        var seed = original.PrivateKey.AsBytes().ToArray();

        using var restored = Ed25519KeyPair.FromPrivateKeyBytes(seed);

        Assert.Equal(original.PublicKey, restored.PublicKey);
    }

    [Fact]
    public void PublicKey_Equality()
    {
        using var keyPair = Ed25519KeyPair.Generate();
        var pk1 = keyPair.PublicKey;
        var pk2 = Ed25519PublicKey.FromBytes(pk1.AsBytes());

        Assert.True(pk1 == pk2);
        Assert.False(pk1 != pk2);
        Assert.True(pk1.Equals(pk2));
        Assert.Equal(pk1.GetHashCode(), pk2.GetHashCode());
    }

    [Fact]
    public void PublicKey_Null_Equality()
    {
        using var keyPair = Ed25519KeyPair.Generate();
        Ed25519PublicKey? nullKey = null;

        Assert.False(keyPair.PublicKey == nullKey);
        Assert.True(keyPair.PublicKey != nullKey);
        Assert.True(nullKey == null);
    }

    [Fact]
    public void Signature_Equality()
    {
        using var keyPair = Ed25519KeyPair.Generate();
        var sig1 = keyPair.Sign("test"u8.ToArray());
        var sig2 = Ed25519Signature.FromBytes(sig1.AsBytes());

        Assert.True(sig1 == sig2);
        Assert.False(sig1 != sig2);
        Assert.Equal(sig1.GetHashCode(), sig2.GetHashCode());
    }

    [Fact]
    public void Signature_Default_IsEmpty()
    {
        Ed25519Signature defaultSig = default;

        Assert.Equal(string.Empty, defaultSig.ToString());
        Assert.Empty(defaultSig.AsBytes().ToArray());
    }

    /// <summary>
    /// RFC 8032 Test Vector 1: Basic signature verification
    /// Source: https://www.rfc-editor.org/rfc/rfc8032#section-7.1
    /// </summary>
    [Fact]
    public void RFC8032_TestVector1_EmptyMessage()
    {
        // SECRET KEY (32 bytes seed)
        var secretKeyHex = "9d61b19deffd5a60ba844af492ec2cc44449c5697b326919703bac031cae7f60";
        var secretKey = Convert.FromHexString(secretKeyHex);

        // PUBLIC KEY (32 bytes)
        var publicKeyHex = "d75a980182b10ab7d54bfed3c964073a0ee172f3daa62325af021a68f707511a";
        var expectedPublicKey = Convert.FromHexString(publicKeyHex);

        // MESSAGE (empty)
        var message = Array.Empty<byte>();

        // SIGNATURE (64 bytes)
        var expectedSigHex = "e5564300c360ac729086e2cc806e828a84877f1eb8e5d974d873e06522490155" +
                             "5fb8821590a33bacc61e39701cf9b46bd25bf5f0595bbe24655141438e7a100b";
        var expectedSignature = Convert.FromHexString(expectedSigHex);

        // Test: Reconstruct key pair and verify signature
        using var privateKey = Ed25519PrivateKey.FromBytes(secretKey);
        var publicKey = privateKey.GetPublicKey();

        // Verify public key matches
        Assert.Equal(expectedPublicKey, publicKey.AsBytes().ToArray());

        // Verify signature
        var signature = Ed25519Signature.FromBytes(expectedSignature);
        var isValid = publicKey.Verify(message, signature);

        Assert.True(isValid, "RFC 8032 Test Vector 1 failed: signature verification failed");
    }

    /// <summary>
    /// RFC 8032 Test Vector 2: Single-byte message
    /// Source: https://www.rfc-editor.org/rfc/rfc8032#section-7.1
    /// </summary>
    [Fact]
    public void RFC8032_TestVector2_SingleByteMessage()
    {
        // SECRET KEY
        var secretKeyHex = "4ccd089b28ff96da9db6c346ec114e0f5b8a319f35aba624da8cf6ed4fb8a6fb";
        var secretKey = Convert.FromHexString(secretKeyHex);

        // PUBLIC KEY
        var publicKeyHex = "3d4017c3e843895a92b70aa74d1b7ebc9c982ccf2ec4968cc0cd55f12af4660c";
        var expectedPublicKey = Convert.FromHexString(publicKeyHex);

        // MESSAGE (single byte: 0x72)
        var message = new byte[] { 0x72 };

        // SIGNATURE
        var expectedSigHex = "92a009a9f0d4cab8720e820b5f642540a2b27b5416503f8fb3762223ebdb69da" +
                             "085ac1e43e15996e458f3613d0f11d8c387b2eaeb4302aeeb00d291612bb0c00";
        var expectedSignature = Convert.FromHexString(expectedSigHex);

        // Test
        using var privateKey = Ed25519PrivateKey.FromBytes(secretKey);
        var publicKey = privateKey.GetPublicKey();

        Assert.Equal(expectedPublicKey, publicKey.AsBytes().ToArray());

        var signature = Ed25519Signature.FromBytes(expectedSignature);
        var isValid = publicKey.Verify(message, signature);

        Assert.True(isValid, "RFC 8032 Test Vector 2 failed: signature verification failed");
    }

    /// <summary>
    /// RFC 8032 Test Vector 3: Two-byte message
    /// Source: https://www.rfc-editor.org/rfc/rfc8032#section-7.1
    /// </summary>
    [Fact]
    public void RFC8032_TestVector3_TwoByteMessage()
    {
        // SECRET KEY
        var secretKeyHex = "c5aa8df43f9f837bedb7442f31dcb7b166d38535076f094b85ce3a2e0b4458f7";
        var secretKey = Convert.FromHexString(secretKeyHex);

        // PUBLIC KEY
        var publicKeyHex = "fc51cd8e6218a1a38da47ed00230f0580816ed13ba3303ac5deb911548908025";
        var expectedPublicKey = Convert.FromHexString(publicKeyHex);

        // MESSAGE (two bytes: 0xaf, 0x82)
        var message = new byte[] { 0xaf, 0x82 };

        // SIGNATURE
        var expectedSigHex = "6291d657deec24024827e69c3abe01a30ce548a284743a445e3680d7db5ac3ac" +
                             "18ff9b538d16f290ae67f760984dc6594a7c15e9716ed28dc027beceea1ec40a";
        var expectedSignature = Convert.FromHexString(expectedSigHex);

        // Test
        using var privateKey = Ed25519PrivateKey.FromBytes(secretKey);
        var publicKey = privateKey.GetPublicKey();

        Assert.Equal(expectedPublicKey, publicKey.AsBytes().ToArray());

        var signature = Ed25519Signature.FromBytes(expectedSignature);
        var isValid = publicKey.Verify(message, signature);

        Assert.True(isValid, "RFC 8032 Test Vector 3 failed: signature verification failed");
    }
}
