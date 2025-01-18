using Blazorise;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Admin
{
    public class AdminPasswordUpdateBase : ComponentBase
    {
        [Parameter]
        public Guid Id { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorage { get; set; }
        public AdminUserLoginDto Admin { get; set; }
        public string PasswordDuplicate { get; set; }
        public UserCredentialsUpdateDto AdminUpdate { get; set; }
        public string ErrorMessage { get; set; }

        protected override void OnInitialized()
        {
            AdminUpdate = new();
        }

        public void UpdateOldPassword(ChangeEventArgs e)
        {
            AdminUpdate.OldPassword = e.Value.ToString();
        }

        public void UpdateNewPassword(ChangeEventArgs e)
        {
            AdminUpdate.NewPassword = e.Value.ToString();
        }

        public void UpdateNewPasswordDuplicate(ChangeEventArgs e)
        {
            PasswordDuplicate = e.Value.ToString();
        }

        protected async Task UpdatePassword_Click()
        {
            try
            {
                if (AdminUpdate.NewPassword.Equals(AdminUpdate.OldPassword))
                {
                    ErrorMessage = "New password can not be the same as your old password";
                }

                if(AdminUpdate.NewPassword.Equals(PasswordDuplicate))
                {
                    var admin = await AdminService.UpdateAdminPassword(Id, AdminUpdate);

                    if (admin != null)
                    {
                        NavigationManager.NavigateTo("/admin/login");
                        return;
                    }

                }
                else
                {
                    ErrorMessage = "New Passwords do not match";
                }

            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }
    }

}
