using JricaStudioApp.Services.Contracts;
using JaysLashesWebApp.Models.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;

namespace JricaStudioApp.Pages.Admin
{
    public class AdminHomeBase : ComponentBase
    {
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public AdminUserLoginDto Admin { get; set; }
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorage { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await AuthenticateUser();
        }

        protected async Task AuthenticateUser()
        {
            Id = await ManageLocalStorage.GetLocalAdminIdGuid();
            Key = await ManageLocalStorage.GetLocalAdminKeyGuid();

            if (Id == Guid.Empty || Key == Guid.Empty)
            {
                NavigationManager.NavigateTo("/");
                return;
            }

            Admin = await AdminService.AuthenticateUser();

            if (Admin == null)
            {
                NavigationManager.NavigateTo("/Admin/Login");
                return;
            }
        }

    }
}
