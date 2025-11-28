# Database Encryption Plan

## Overview

This document outlines the encryption strategy for the Aquiis property management application, addressing both security requirements and database portability concerns.

## Encryption Approaches Considered

### 1. SQLCipher (File-Level Encryption)

**Pros:**

- Industry standard for SQLite encryption
- Transparent encryption/decryption (encrypts the entire database file)
- Works seamlessly with Entity Framework Core
- Cross-platform (works on Windows, Linux, macOS)
- Protects data at rest

**Cons:**

- Requires native binaries (different per platform)
- Need to manage encryption keys securely
- Slight performance overhead
- Commercial license required for some uses (open source for others)

**Implementation:**

- Use `Microsoft.EntityFrameworkCore.Sqlite.Core` package
- Bundle platform-specific SQLCipher binaries
- Provide encryption key in connection string: `Data Source=app.db;Password=yourkey`

### 2. Column-Level Encryption (Application Layer)

**Pros:**

- Fine-grained control (encrypt only sensitive columns)
- No third-party dependencies
- Works with standard SQLite
- Can use different keys for different columns

**Cons:**

- Manual work to encrypt/decrypt each field
- Can't query encrypted data directly (loses indexing)
- More complex code
- Doesn't protect database structure/metadata

**Implementation:**

- Use .NET's `System.Security.Cryptography` (AES)
- Encrypt sensitive properties in models before saving
- Decrypt when reading from database

### 3. File System Encryption

**Pros:**

- Simplest to implement
- No code changes needed
- Protects all files, not just database

**Cons:**

- Only protects against physical theft, not application-level attacks
- Requires OS-level setup
- For Electron: varies by platform (BitLocker/LUKS/FileVault)
- For Web: typically handled by infrastructure

**Implementation:**

- Windows: BitLocker
- Linux: LUKS, eCryptfs
- macOS: FileVault
- Cloud: Encrypted volumes (AWS EBS, Azure Disk Encryption)

### 4. Hybrid Approach

Combine multiple layers:

- **SQLCipher** for database file encryption
- **Column-level** for extra-sensitive data (SSN, credit cards)
- **File system encryption** for defense in depth
- **TLS/HTTPS** for data in transit

## Portability Considerations

### The Core Problem with OS Keychain

If encrypting the database with a key stored in the OS keychain:

- **Key is tied to that specific machine/user account**
- Moving the `.db` file to another computer = **can't decrypt it**
- The keychain doesn't travel with the database file

### Portability Scenarios

**1. User Gets New Computer:**

```
Old Computer: Has encryption key in keychain → can decrypt database
New Computer: No key in keychain → CANNOT decrypt database
Result: User loses all data unless they manually transfer the key
```

**2. User Wants Backup on USB/Cloud:**

```
They copy the .db file to USB → File is encrypted
Different computer → Can't decrypt without the key
Result: Backup is useless without key transfer mechanism
```

**3. Multiple Devices (Desktop + Laptop):**

```
Desktop: Key in Windows Credential Manager
Laptop: Different key or no key
Result: Can't sync database between devices
```

## Solutions for Portability

### Option 1: Password-Derived Key (Most Portable)

```
User enters password → Derive encryption key from password
Same password on any computer → Same key → Can decrypt
```

**Pros:**

- Database file is fully portable
- Works on any device with the password
- User controls the key (memorable password)

**Cons:**

- Weak password = weak encryption
- No protection if password is forgotten (data lost forever)
- Password must be entered on each startup

### Option 2: Embedded Key File (Moderate Portability)

```
Generate key file (e.g., .key file)
User must keep key file WITH database file
Database + Key file = portable pair
```

**Pros:**

- Stronger than password (can be 256-bit random)
- Portable (just copy both files)
- User doesn't need to remember password

**Cons:**

- User must manage two files
- If key file lost → data lost
- Key file in plaintext is a security risk

### Option 3: Hybrid Approach (RECOMMENDED)

```
Primary: Password-derived key (for portability)
Secondary: Cache key in OS keychain (for convenience)
```

**How it works:**

1. First time: User creates password → Derive key → Encrypt database
2. Store key in OS keychain for convenience
3. On next launch: Try keychain first, fallback to password prompt
4. On new computer: Prompt for password → Derive same key

**Pros:**

- Fully portable (password works anywhere)
- Convenient on main computer (auto-decrypt from keychain)
- User can recover on new device with password

**Cons:**

- Slightly more complex implementation

### Option 4: Cloud-Based Key Management (Enterprise)

```
Store encryption key in cloud (Azure Key Vault, AWS KMS)
User authenticates → Fetch key from cloud → Decrypt database
```

**Pros:**

- Keys accessible from any device
- Centralized key rotation
- Better for multi-user scenarios

**Cons:**

- Requires internet connection
- More complex setup
- Subscription costs

## Recommended Implementation

### For Electron (Desktop): Hybrid Approach

```
┌─────────────────────────────────────────────┐
│ User Workflow                               │
├─────────────────────────────────────────────┤
│ First Launch:                               │
│  1. User creates master password            │
│  2. Derive encryption key from password     │
│  3. Cache key in OS keychain                │
│  4. Encrypt database with key               │
│                                             │
│ Subsequent Launches (Same Computer):        │
│  1. Try to get key from keychain            │
│  2. If found → Decrypt and continue         │
│  3. If not found → Prompt for password      │
│                                             │
│ New Computer or Backup Restore:             │
│  1. Copy database file                      │
│  2. App prompts for master password         │
│  3. Derive key from password                │
│  4. Decrypt database                        │
│  5. Cache key in new computer's keychain    │
└─────────────────────────────────────────────┘
```

### For Web: Environment-Based Encryption Password

**Development Server:**

- Set encryption password via environment variable
- Password retrieved automatically on application startup
- No user interaction required

**Production Server:**

- Use secure secret management (Azure Key Vault, AWS Secrets Manager)
- Or environment variables with restricted file permissions
- Single password configured once during deployment

**User Experience:**

- Web users never see the database encryption password
- They just log in with their normal username/password
- Server handles database decryption automatically
- Simpler than Electron (no password prompts)

## Portability Strategy

### Export Feature

Backup includes:

1. Encrypted database file (.db)
2. Metadata file (.json) with:
   - Salt used for key derivation
   - Encryption algorithm details
   - Schema version
3. User instructions (PDF/TXT):
   "To restore: Copy files, enter your master password"

### Benefits of This Approach

✅ Full portability (password-based)  
✅ Convenience (keychain caching)  
✅ Security (strong encryption)  
✅ User control (they know the password)  
✅ Backup-friendly (password travels with user, not machine)

## Use Case Considerations

Since property managers might:

- Upgrade computers
- Work from multiple devices
- Create backups for disaster recovery
- Transfer data to accountants/auditors

**The Hybrid Approach provides the best balance of security, usability, and portability.**

## Key Management Considerations

1. **Never hardcode keys** in source code
2. **Electron**: Use OS keychain/credential manager for caching
3. **Web**: Use Azure Key Vault, AWS Secrets Manager, or environment variables
4. **User-based encryption**: Each user's key derived from their password
5. **Master key rotation**: Plan for key rotation strategy

## Web Server Configuration Options

### Option 1: Environment Variable (Recommended for Development)

**Linux/macOS:**

```bash
# In terminal before running
export DB_ENCRYPTION_PASSWORD="your-secure-password-here"
dotnet run

# Or add to ~/.bashrc for persistence
echo 'export DB_ENCRYPTION_PASSWORD="your-password"' >> ~/.bashrc
```

**launchSettings.json:**

```json
{
  "profiles": {
    "https": {
      "environmentVariables": {
        "DB_ENCRYPTION_PASSWORD": "dev-password-123"
      }
    }
  }
}
```

**systemd Service (Linux Production):**

```ini
[Service]
Environment="DB_ENCRYPTION_PASSWORD=your-secure-password"
# Or use EnvironmentFile for better security
EnvironmentFile=/etc/aquiis/db.env
```

**Setup script for EnvironmentFile:**

```bash
#!/bin/bash
sudo mkdir -p /etc/aquiis
echo "DB_ENCRYPTION_PASSWORD=your-secure-production-password" | sudo tee /etc/aquiis/db.env
sudo chmod 600 /etc/aquiis/db.env
sudo chown aquiis-user:aquiis-user /etc/aquiis/db.env
```

### Option 2: Azure Key Vault (Recommended for Production)

```csharp
// In Program.cs
var secretClient = new SecretClient(
    new Uri(configuration["KeyVault:Url"]),
    new DefaultAzureCredential()
);
var secret = await secretClient.GetSecretAsync("DatabaseEncryptionPassword");
var encryptionPassword = secret.Value.Value;
```

**Advantages:**

- Centralized secret management
- Automatic rotation support
- Audit logging
- Access control via Azure AD
- No secrets in configuration files

### Option 3: AWS Secrets Manager

```csharp
// In Program.cs
var client = new AmazonSecretsManagerClient(RegionEndpoint.USEast1);
var request = new GetSecretValueRequest
{
    SecretId = "aquiis/database-encryption-password"
};
var response = await client.GetSecretValueAsync(request);
var encryptionPassword = response.SecretString;
```

### Option 4: appsettings.Production.json (Less Secure, Not Recommended)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db;Password=your-password-here"
  }
}
```

**If using this approach:**

- Add to `.gitignore` (never commit)
- Set file permissions: `chmod 600 appsettings.Production.json`
- Only readable by application user account
- Consider using ASP.NET Data Protection to encrypt the file

### Option 5: Docker Secrets

```yaml
# docker-compose.yml
services:
  aquiis:
    image: aquiis-app
    secrets:
      - db_encryption_password
    environment:
      DB_ENCRYPTION_PASSWORD_FILE: /run/secrets/db_encryption_password

secrets:
  db_encryption_password:
    file: ./secrets/db_password.txt
```

```csharp
// In Program.cs
var passwordFile = Environment.GetEnvironmentVariable("DB_ENCRYPTION_PASSWORD_FILE");
if (!string.IsNullOrEmpty(passwordFile) && File.Exists(passwordFile))
{
    encryptionPassword = File.ReadAllText(passwordFile).Trim();
}
```

## Implementation Pattern for Electron + Web

```csharp
// In Program.cs
string? encryptionPassword = null;

if (HybridSupport.IsElectronActive)
{
    // Electron: Try keychain first, fallback to password prompt
    encryptionPassword = await GetElectronEncryptionPassword();
}
else
{
    // Web: Get from environment variable or secret manager
    encryptionPassword = Environment.GetEnvironmentVariable("DB_ENCRYPTION_PASSWORD");

    // Or from Azure Key Vault
    if (string.IsNullOrEmpty(encryptionPassword) && !string.IsNullOrEmpty(keyVaultUrl))
    {
        var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        var secret = await secretClient.GetSecretAsync("DatabaseEncryptionPassword");
        encryptionPassword = secret.Value.Value;
    }

    if (string.IsNullOrEmpty(encryptionPassword))
    {
        throw new InvalidOperationException(
            "Database encryption password not configured. " +
            "Set DB_ENCRYPTION_PASSWORD environment variable or configure Key Vault."
        );
    }
}

// Use password in connection string
var connectionString = $"Data Source={dbPath};Password={encryptionPassword}";
```

## Password Management Best Practices

### Development

- Use simple password in environment variable
- Document in README how to set it up
- Include in `launchSettings.json` for team consistency

### Staging/Production

- **Never** use environment variables in shared hosting
- Use Azure Key Vault or AWS Secrets Manager
- Enable audit logging for secret access
- Rotate passwords periodically
- Use different passwords per environment

### Security Checklist

- ✅ Password never in source code
- ✅ Password never in git repository
- ✅ Password not in plain text files (production)
- ✅ File permissions restricted (if using files)
- ✅ Access logging enabled (cloud secret managers)
- ✅ Different passwords for dev/staging/production
- ✅ Password rotation strategy documented

## Implementation Technologies

### For SQLCipher Integration:

- `Microsoft.EntityFrameworkCore.Sqlite.Core` package
- Platform-specific SQLCipher binaries
- Connection string with password parameter

### For Password-Derived Keys:

- PBKDF2 (Password-Based Key Derivation Function 2)
- Argon2 (modern alternative, more secure)
- Minimum 100,000 iterations for PBKDF2
- Random salt per database (stored in metadata)

### For OS Keychain Integration:

- Windows: Windows Credential Manager API
- macOS: Keychain Services API
- Linux: Secret Service API (libsecret)

## Security Best Practices

1. **Strong Password Requirements:**

   - Minimum 12 characters
   - Mix of uppercase, lowercase, numbers, symbols
   - No dictionary words
   - Password strength meter in UI

2. **Key Derivation:**

   - Use Argon2id or PBKDF2-SHA256
   - Random salt per database (never reuse)
   - High iteration count (100k+ for PBKDF2)
   - Store salt with database metadata

3. **Backup Security:**

   - Encrypted backups maintain same encryption
   - Include salt and derivation parameters in metadata
   - Clear instructions for password recovery
   - Warning about password loss = data loss

4. **Password Recovery:**
   - **No recovery mechanism** (by design for security)
   - Clear warning to users about password importance
   - Recommend password manager usage
   - Consider optional recovery key export (printed/stored offline)

## Future Enhancements

1. **Multi-Factor Authentication:**

   - Combine password with biometric
   - Hardware security key support (YubiKey)

2. **Key Rotation:**

   - Allow users to change master password
   - Re-encrypt database with new key
   - Maintain backward compatibility

3. **Shared Access:**

   - Multiple users with different passwords
   - Each password can unlock the database
   - Useful for business partnerships

4. **Compliance:**
   - GDPR right to erasure
   - HIPAA compliance for healthcare properties
   - SOC 2 audit trail requirements

## Next Steps

1. Implement password derivation service
2. Create OS keychain abstraction layer
3. Build password setup UI for first launch
4. Add password prompt dialog
5. Integrate SQLCipher with connection string
6. Update backup/restore to handle encryption metadata
7. Add password change feature
8. Document user-facing encryption features
9. Test portability across different machines
10. Create user guide for encryption management
