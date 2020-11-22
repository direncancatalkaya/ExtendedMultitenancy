using System;

namespace EntityFramework.ExtensionUtilities.Entities.Abstract
{
    /// <summary>
    /// This interface provide a Audit loggable database object. İnherit this interface and multi tenancy extension
    /// will write audit logs on inherited class.
    /// </summary>
    /// <typeparam name="TUserId">Type Of UserID</typeparam>
    public interface IAuditLog<TUserId> where TUserId : struct
    {
        TUserId? CreatedBy { get; set; }
        DateTime? CreatedAt { get; set; }
        TUserId? ModifiedBy { get; set; }
        DateTime? ModifiedAt { get; set; }
    }
}