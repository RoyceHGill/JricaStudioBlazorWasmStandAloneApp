using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioApp.Models.Dtos;
using JricaStudioApp.Models.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Models.Dtos.Admin;

namespace JricaStudioApp.Pages.Admin.Users
{
    public class UserAdminPageBase : ComponentBase
    {
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorage { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IUserService UserService { get; set; }
        public AdminUserLoginDto Admin { get; set; }
        public UserFilterDto Filter { get; set; }
        public List<AdminUserDetailsDto> Users { get; set; }
        public AdminUserDetailsDto SelectedUser { get; set; }
        public string ErrorMessage { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Filter = new UserFilterDto();
            Admin = await AdminService.AuthenticateUser();
            Users = default;
        }

        

        protected async Task SelectUser_Click(AdminUserDetailsDto user)
        {
            SelectedUser = user;
            StateHasChanged();
        }


        protected async Task SearchUsers_Click()
        {
            await AdminService.AuthenticateUser();
            var collection = await UserService.SearchUsers(Filter);
            SelectedUser = null;
            if (collection == null)
            {
                Users = default;
                StateHasChanged();
                return;
            }
            
            Users = collection.OrderBy(x => x.FirstName).ToList();
            StateHasChanged();
        }
        protected void AddNewUser_Click()
        {
            NavigationManager.NavigateTo("admin/users/new");
        }

        protected async Task EditUser_Click(Guid id)
        {
            NavigationManager.NavigateTo($"admin/users/edit/{id}");
        }

        protected async Task DeleteUser_Click(Guid id)
        {
            try
            {
                var user = await UserService.DeleteUser(id);

                if (user != null)
                {
                    var clientSideUser = Users.SingleOrDefault(u => u.Id == user.Id);

                    if (clientSideUser != null)
                    {
                        Users.Remove(clientSideUser);
                        SelectedUser = null;
                        StateHasChanged();
                    }

                }

            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        protected async Task Enter(KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case "Enter":
                    SearchUsers_Click();
                    break;
            }
        }

        protected async Task<List<AdminUserDetailsDto>> SortUsersByFirstName(IEnumerable<AdminUserDetailsDto> users)
        {
            return users.OrderBy(x => x.FirstName).ToList();
        }
    }
}
