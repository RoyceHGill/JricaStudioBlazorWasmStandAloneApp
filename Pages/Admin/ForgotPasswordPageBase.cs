using JricaStudioApp.Services.Contracts;
using Microsoft.AspNetCore.Components;
using JaysLashesWebApp.Models.Dtos.Admin;

namespace JricaStudioApp.Pages.Admin
{
    public class ForgotPasswordPageBase : ComponentBase
    {
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public string Email { get; set; }
        public bool IsSubmitted { get; set; }
        public string ConfirmationMessage { get; set; }
        public string ErrorMessage { get; set; }

        private static bool ValidateEmail(string email)
        {
            if (!email.Contains("@"))
            {
                return false;
            }

            var splitEmail = email.Split("@");

            if (splitEmail.Length != 2)
            {
                return false;
            }

            if (splitEmail.Any(a => a.Contains("@")))
            {
                return false;
            }

            return true;
        }

        protected async Task SubmitPasswordResetRequest_Click()
        {
            if (!ValidateEmail(Email))
            {
                ErrorMessage = "Please enter a valid email.";
                return;
            }

            var result = await AdminService.InitiatePasswordReset(new PasswordResetRequestDto
            {
                Email = Email
            });

            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ConfirmationMessage = "";
                ErrorMessage = "There was an error with the request";
                return;
            }
            ErrorMessage = "";
            ConfirmationMessage = "If there is an account associated with the provided email address then you will receive and email with the your next steps.";
        }


       
    }
}
