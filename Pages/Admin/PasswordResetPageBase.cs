using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Admin
{
    public class PasswordResetPageBase : ComponentBase
    {
        [Parameter]
        public Guid Key { get; set; }
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        public ResetPasswordDto ResetPassword { get; set; }
        public string PasswordDuplicate { get; set; }
        public string ConfirmationMessage { get; set; }
        public string ErrorMessage { get; set; }

        protected async override Task OnInitializedAsync()
        {
            ResetPassword = new();
        }

        public async void SubmitPasswordReset_Click()
        {
            var result = await AdminService.SubmitPasswordRequest(Key, ResetPassword);

            if (result.IsSuccessful)
            {
                
                NavigationManager.NavigateTo("admin/login");
            }
            else
            {
                if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    if (!string.IsNullOrWhiteSpace(result.Message))
                    {
                        ErrorMessage = result.Message;
                        return;
                    }
                }
                else 
                {
                    NavigationManager.NavigateTo("/");
                }

            }
        }

        public async Task CancelPasswordRest_Click()
        {
            NavigationManager.NavigateTo("admin/login");
        }

    }
}
