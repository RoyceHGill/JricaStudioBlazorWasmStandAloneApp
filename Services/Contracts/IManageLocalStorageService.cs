namespace JricaStudioApp.Services.Contracts
{
    public interface IManageLocalStorageService
    {
        Task<Guid> GetLocalUserIdGuid();
        Task<Guid> GetLocalAppointmentIdGuid();
        Task<Guid> GetLocalAdminKeyGuid();
        Task<Guid> SetLocalUserIdGuid(Guid userId);
        Task<Guid> SetLocalAppointmentIdGuid(Guid appointmentid);
        Task RemoveLocalAppointmentIdGuid();
        Task RemoveLocalAdminKeyGuid();
        Task RemoveLocalAdminIdGuid();

        Task<Guid> SetLocalAdminKeyGuid(Guid userId);
        Task<Guid> GetLocalAdminIdGuid();
        Task<Guid> SetLocalAdminIdGuid(Guid adminId);


    }
}
