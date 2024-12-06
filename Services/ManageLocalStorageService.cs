using Blazored.LocalStorage;
using JricaStudioApp.Services.Contracts;

namespace JricaStudioApp.Services
{
    public class ManageLocalStorageService : IManageLocalStorageService
    {
        private readonly ILocalStorageService _localStorageService;
        private const string UserKey = "JRicaStudioUserKey";

        private const string AppointmentKey = "JRicaStudioAppointmentKey";

        private const string AdminKey = "JRicaStudioAdminKey";
        private const string AdminId = "JRicaStudioAdminId";


        public ManageLocalStorageService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        #region User

        public async Task<Guid> GetLocalUserIdGuid()
        {
            return await _localStorageService.GetItemAsync<Guid>(UserKey);
        }

        public async Task<Guid> SetLocalUserIdGuid(Guid userkeyValue)
        {
            await _localStorageService.SetItemAsync(UserKey, userkeyValue);

            return await GetLocalUserIdGuid();

        }
        #endregion

        #region Appointment

        public async Task<Guid> GetLocalAppointmentIdGuid()
        {
            return await _localStorageService.GetItemAsync<Guid>(AppointmentKey);
        }

        public async Task<Guid> SetLocalAppointmentIdGuid(Guid appointmentKeyValue)
        {
            await _localStorageService.SetItemAsync(AppointmentKey, appointmentKeyValue);

            return await GetLocalAppointmentIdGuid();
        }

        public async Task RemoveLocalAppointmentIdGuid()
        {
            await _localStorageService.RemoveItemAsync(AppointmentKey);
        }

        #endregion

        #region Admin

        public async Task<Guid> GetLocalAdminKeyGuid()
        {
            return await _localStorageService.GetItemAsync<Guid>(AdminKey);

        }

        public async Task<Guid> SetLocalAdminKeyGuid(Guid adminKey)
        {
            await _localStorageService.SetItemAsync(AdminKey, adminKey);

            return await GetLocalAdminKeyGuid();
        }


        public async Task<Guid> GetLocalAdminIdGuid()
        {
            return await _localStorageService.GetItemAsync<Guid>(AdminId);

        }

        public async Task<Guid> SetLocalAdminIdGuid(Guid adminId)
        {
            await _localStorageService.SetItemAsync(AdminId, adminId);

            return await GetLocalAdminIdGuid();
        }

        public async Task RemoveLocalAdminKeyGuid()
        {
            await _localStorageService.RemoveItemAsync(AdminKey);

        }

        public async Task RemoveLocalAdminIdGuid()
        {
            await _localStorageService.RemoveItemAsync(AdminId);

        }

        #endregion




    }
}
