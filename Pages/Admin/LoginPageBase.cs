using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace JricaStudioApp.Pages.Admin
{
    public class LoginPageBase : ComponentBase
    {
        [Parameter]
        public EventCallback<ChangeEventArgs> OnInput { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string LoginErrorResult { get; set; }
        public AdminUserLoginDto Admin { get; set; }
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorage { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected async Task Login_Click()
        {
            LoginErrorResult = "";
            StateHasChanged();
            Admin = await AdminService.Login(new AdminLoginRequestDto
            {
                Password = Password,
                Username = Email.ToLower()
            });

            if (Admin != null)
            {
                if (Admin.Updated == null || Admin.Updated < DateTime.Now.AddDays(-30))
                {
                    NavigationManager.NavigateTo($"admin/update/{Admin.Id}");
                    return;
                }

                await ManageLocalStorage.SetLocalAdminKeyGuid(Admin.AdminKey);
                await ManageLocalStorage.SetLocalAdminIdGuid(Admin.Id);
                NavigationManager.NavigateTo("Admin/Home");
            }
            else
            {
                LoginErrorResult = "Email or password is incorrect";
                StateHasChanged();
            }
        }

        protected void UpdatePassword(ChangeEventArgs e)
        {
            Password = e.Value.ToString();
        }

        protected async override Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();

            if (Admin != null)
            {
                NavigationManager.NavigateTo("admin/home");
            }
        }

        public async void Enter(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await Login_Click();
            }
        }

    }
}
