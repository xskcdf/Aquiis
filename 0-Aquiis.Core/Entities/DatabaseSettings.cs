using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aquiis.Core.Entities
{
    /// <summary>
    /// Database-level settings that affect all organizations.
    /// These are runtime configuration values stored in the database itself.
    /// </summary>
    public class DatabaseSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Database encryption status - applies to the entire SQLite file
        /// </summary>
        [Required]
        public bool DatabaseEncryptionEnabled { get; set; } = false;

        /// <summary>
        /// When encryption status was last changed
        /// </summary>
        public DateTime? EncryptionChangedOn { get; set; }

        /// <summary>
        /// Salt used for password-derived key (base64 encoded)
        /// Required for portable encryption - same password + salt = same key
        /// </summary>
        [StringLength(256)]
        public string? EncryptionSalt { get; set; }

        /// <summary>
        /// Last time settings were modified
        /// </summary>
        public DateTime LastModifiedOn { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User or system that last modified settings
        /// </summary>
        [StringLength(128)]
        public string LastModifiedBy { get; set; } = "System";
    }
}
