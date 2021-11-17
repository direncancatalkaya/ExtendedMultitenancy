using System;

namespace EntityFramework.ExtensionUtilities.Entities.Abstract
{
    /// <summary>
    ///     This interface provide a soft deletable database object. İnherit this interface and multi tenancy extension
    ///     will handle soft delete pattern on implemented class.
    /// </summary>
    /// <typeparam name="TUserId">Type Of UserID</typeparam>
    public interface ISoftDelete<TUserId>
    {
        bool IsDeleted { get; set; }
        TUserId DeletedBy { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}