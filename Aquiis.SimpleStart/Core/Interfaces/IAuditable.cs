using System.ComponentModel.DataAnnotations;

namespace Aquiis.SimpleStart.Core.Interfaces
{
    /// <summary>
    /// Interface for entities that track audit information (creation and modification).
    /// Entities implementing this interface will have their audit fields automatically
    /// managed by the BaseService during create and update operations.
    /// </summary>
    public interface IAuditable
    {
        /// <summary>
        /// Date and time when the entity was created (UTC).
        /// </summary>
        DateTime CreatedOn { get; set; }

        /// <summary>
        /// User ID of the user who created the entity.
        /// </summary>
        string CreatedBy { get; set; }

        /// <summary>
        /// Date and time when the entity was last modified (UTC).
        /// </summary>
        DateTime? LastModifiedOn { get; set; }

        /// <summary>
        /// User ID of the user who last modified the entity.
        /// </summary>
        string? LastModifiedBy { get; set; }
    }
}
