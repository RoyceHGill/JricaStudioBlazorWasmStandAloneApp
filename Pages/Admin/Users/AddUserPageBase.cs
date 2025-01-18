using JricaStudioApp.Services.Contracts;
using JricaStudioApp.Models.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Models.Dtos.Admin;

namespace JricaStudioApp.Pages.Admin.Users
{
    public class AddUserPageBase : ComponentBase
    {
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IUserService UserService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public AdminUserLoginDto Admin { get; set; }
        public UserAdminAddDto User { get; set; }
        public string ErrorMessage { get; set; }


        protected async override Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();
            User = new();

        }

        protected async Task SaveNewUser_Click()
        {
            try
            {
                var user = await UserService.AdminPostNewUser(User);

                if (user != null)
                {
                    NavigationManager.NavigateTo("admin/users");
                    return;
                }
                ErrorMessage = "There was an minor error deleting the User. Try debugging.";
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        protected async Task ClearForm_Click()
        {
            User = new();
            StateHasChanged();
        }

        protected async Task BackToUsers_Click()
        {
            NavigationManager.NavigateTo("admin/users");
        }

    }
}
