namespace EntityFramework.ExtensionUtilities.Entities.Abstract
{
    /// <summary>
    /// This interface provide a multi tenancy database object. İnherit this interface and multi tenancy extension
    /// will handle multi tenancy on inplemented class.
    /// </summary>
    /// <typeparam name="T">Type of Tenant Id</typeparam>
    public interface ITenant<T>
    {
        T TenantId { get; set; }
    }
}