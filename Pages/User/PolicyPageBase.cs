using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.User
{
    public class PolicyPageBase : ComponentBase
    {
        [Inject]
        public IUserService UserService { get; set; }

        public List<PolicyDto> Policies { get; set; }


        protected async override Task OnInitializedAsync()
        {
            Policies = new List<PolicyDto>();

            var policies = await UserService.GetPoliciies();

            if (policies != null)
            {
                Policies = policies.OrderBy(p => p.PolicyTitle).ToList();
            }
        }
    }
}
