using JricaStudioApp.Services.Contracts;
using JaysLashesWebApp.Models.Dtos;
using JaysLashesWebApp.Models.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Models.Dtos.Admin;

namespace JricaStudioApp.Pages.Admin.Users
{
    public class EditUserPageBase : ComponentBase
    {
        [Parameter]
        public Guid Id { get; set; }
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IUserService UserService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public AdminUserLoginDto Admin { get; set; }
        public UpdateUserDto User { get; set; }
        public string ErrorMessage { get; set; }


        protected async override Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();
            var user = await UserService.AdminGetUser(Id);

            User = new UpdateUserDto()
            { 
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                DOB = user.DOB,
                HasAllergies = user.HasAllergies,
                HasSensitiveSkin = user.HasSensitiveSkin,
                HasHadEyeProblems4Weeks = user.HasHadEyeProblems4Weeks,
                WearsContanctLenses = user.WearsContanctLenses,
                IsWaiverAcknowledged = user.IsWaiverAcknowledged
                
            };
        }

        protected async Task SaveUserUpdates_Click()
        {
            try
            {
                var user = await UserService.UpdateUser(Id, User);
                if (user != null)
                {
                    NavigationManager.NavigateTo("admin/users");
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            
        }

        protected async Task BackToUsers_Click()
        {
            NavigationManager.NavigateTo("admin/users");
        }
    }
}
