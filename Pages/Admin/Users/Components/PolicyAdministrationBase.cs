using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Admin.Users.Components
{
    public class PolicyAdministrationBase : ComponentBase
    {
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IUserService UserService { get; set; }
        public AdminUserLoginDto Admin { get; set; }
        public IEnumerable<PolicyAdminDto> Policies { get; set; }
        public bool IsAddingNewPolicy { get; set; }
        public string ToAddPolicyTitle { get; set; }
        public string ToAddPolicyArticle { get; set; }
        public string ErrorMessage { get; set; }
        public string ConfirmationMessage { get; set; }


        protected async override Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();

            var policies = await UserService.GetAdministratorPoliciies();
            Policies = policies;
        }

        protected async Task AddPolicy_Click()
        {
            if (!IsAddingNewPolicy)
            {
                IsAddingNewPolicy = true;
            }
        }

        protected async Task DeletePolicy_Click(Guid id)
        {
            try
            {
                var policy = await UserService.DeletePolicy(id);

                if (policy == null)
                {
                    ErrorMessage = "There was an error deleting the Policy";
                    return;
                }

                ConfirmationMessage = "The Policy was successfully deleted.";

            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        protected async Task SaveNewPolicy_Click()
        {
            try
            {
                var policy = await UserService.PostPolicy(new AddPolicyDto
                {
                    PolicyTitle = ToAddPolicyTitle,
                    PolicyArticle = ToAddPolicyArticle,
                });

                if (policy == null)
                {
                    ErrorMessage = "There was an error creating the policy";
                    return;
                }

                ConfirmationMessage = "The Policy was successfully created";
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }
    }
}
