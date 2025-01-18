using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

using System.ComponentModel.DataAnnotations;

namespace JricaStudioApp.Pages.Admin.Services
{
    public class ServiceAdminPageBaseBase : ComponentBase
    {
        public AdminUserLoginDto Admin { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorage { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IServiceService ServiceService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        public ServiceFilterDto Filter { get; set; }
        public IEnumerable<AdminServiceCategoryDto> ServiceCategories { get; set; }
        public List<ServiceAdminPageDto> Services { get; set; }
        public  AddServiceCategoryDto CategoryToAdd { get; set; }
        public Guid CategoryIdToDelete { get; set; }
        public Guid CategoryNameInputFieldId { get; set; } = Guid.NewGuid();
        public Guid CategoryNameInputButtonId { get; set; } = Guid.NewGuid();
        public Guid CategoryDeleteDropDownInputId { get; set; } = Guid.NewGuid();
        public Guid CategoryDeleteConfirmationButtonId { get; set; } = Guid.NewGuid();
        public string AddCategoryErrorMessage { get; set; }
        public string AddCategoryConfirmationMessage { get; set; }
        public string DeleteCategoryErrorMessage { get; set; }
        public string DeleteCategoryConfirmationMessage { get; set; }


        protected async override Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();
            if (Admin != null)
            {
                Filter = new ServiceFilterDto();
                ServiceCategories = await ServiceService.GetServiceCategories();
                var services = await ServiceService.GetAdminServices();
                Services = services.OrderBy(a => a.Name).ToList();
                CategoryToAdd = new();
            }
        }


        protected async Task Enter (KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case "Enter":
                    SearchServices_Click();
                    break;
            }
        }

        protected async Task SearchServices_Click()
        {
            var services = await ServiceService.SearchServices(Filter);

            Services = services.OrderBy(s => s.Name).ToList();
            StateHasChanged();
        }

        protected async Task AddNewService_Click()
        {
            NavigationManager.NavigateTo("/Admin/AddService");
        }

        protected async Task EditService_Click(Guid id)
        {
            NavigationManager.NavigateTo($"admin/service/edit/{id}");
        }

        protected async Task DeleteService_Click(Guid id)
        {
            try
            {
                var result = await ServiceService.DeleteService(id);
                if (result != null)
                {
                    NavigationManager.NavigateTo("admin/services");
                }
                var services = await ServiceService.SearchServices(Filter);
                Services = services.OrderBy(s => s.Name).ToList();
                StateHasChanged();
            }
            catch (Exception e)
            {
                AddCategoryConfirmationMessage = string.Empty;
                AddCategoryErrorMessage = string.Empty;
                DeleteCategoryConfirmationMessage = string.Empty;
                DeleteCategoryErrorMessage = "Unable to delete service";
                StateHasChanged();
            }
            
        }

        protected async Task AddNewProductCategory_Click()
        {
            await JSRuntime.InvokeVoidAsync("SetVisible", CategoryNameInputButtonId);
            await JSRuntime.InvokeVoidAsync("SetVisible", CategoryNameInputFieldId);
            await JSRuntime.InvokeVoidAsync("SetHidden", CategoryDeleteDropDownInputId);
            await JSRuntime.InvokeVoidAsync("SetHidden", CategoryDeleteConfirmationButtonId);
            AddCategoryConfirmationMessage = default;
            AddCategoryErrorMessage = default;
            DeleteCategoryConfirmationMessage = default;
            DeleteCategoryErrorMessage = default;
            StateHasChanged();
        }

        protected async Task SaveNewProductCategory_Click()
        {
            try
            {
                var result = await ServiceService.PostNewServiceCategory(CategoryToAdd);

                if (result != null)
                {
                    CategoryToAdd = new();
                    await JSRuntime.InvokeVoidAsync("SetHidden", CategoryNameInputButtonId);
                    await JSRuntime.InvokeVoidAsync("SetHidden", CategoryNameInputFieldId);
                    ServiceCategories = await ServiceService.GetServiceCategories();
                    AddCategoryConfirmationMessage = "Category Successfully Added";
                    StateHasChanged();
                }
            }
            catch (Exception e)
            {
                AddCategoryErrorMessage = e.Message;
            }
        }

        protected async Task DeleteServiceCategory_Click()
        {
            await JSRuntime.InvokeVoidAsync("SetHidden", CategoryNameInputButtonId);
            await JSRuntime.InvokeVoidAsync("SetHidden", CategoryNameInputFieldId);
            await JSRuntime.InvokeVoidAsync("SetVisible", CategoryDeleteDropDownInputId);
            await JSRuntime.InvokeVoidAsync("SetVisible", CategoryDeleteConfirmationButtonId);
            AddCategoryConfirmationMessage = default;
            AddCategoryErrorMessage = default;
            DeleteCategoryConfirmationMessage = default;
            DeleteCategoryErrorMessage = default;
            StateHasChanged();
        }

        protected async Task ConfirmCategoryDeletion_Click()
        {
            try
            {
                if (CategoryIdToDelete == Guid.Empty)
                {
                    DeleteCategoryErrorMessage = "Please Select a Category";
                    return;
                }


                var result = await ServiceService.DeleteServiceCategory(CategoryIdToDelete);
                if (result == null)
                {
                    DeleteCategoryErrorMessage = "The Category was not deleted.";
                }
                DeleteCategoryConfirmationMessage = "Category Successfully Deleted";
                ServiceCategories = await ServiceService.GetServiceCategories();
            }
            catch (Exception e)
            {
                DeleteCategoryErrorMessage = e.Message;
            }
        }
    }
}
