using Blazorise.Extensions;
using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using JricaStudioSharedLibrary.Constants;

using Newtonsoft.Json.Linq;

namespace JricaStudioApp.Pages.Admin.Services
{
    public class AdminEditServicePageBase : ComponentBase
    {
        [Parameter]
        public Guid Id { get; set; }

        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IServiceService ServiceService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public AdminUserLoginDto Admin { get; set; }
        public AdminEditServiceDto Service { get; set; }
        public IEnumerable<AdminServiceCategoryDto> Categories { get; set; }

        public string ImageInputData { get; set; }
        public IBrowserFile ImageInput { get; set; }
        public string CurrentImage { get; set; }
        public string FieldMargins { get; set; } = "mb-4 mt-3";
        public string ErrorMessage { get; set; }

        private int durationInMinutes;

        public int DurationInMinutes
        {
            get { return (int)Service.Duration.TotalMinutes; ; }
            set
            {
                durationInMinutes = value;
                Service.Duration = TimeSpan.FromMinutes(durationInMinutes);
            }
        }

        protected async override Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();
            Categories = await ServiceService.GetServiceCategories();
            var service = await ServiceService.GetService(Id);
            Service = new AdminEditServiceDto
            {
                Name = service.Name,
                Description = service.Description,
                Duration = service.Duration,
                Price = service.Price,
                ServiceCategoryId = service.ServiceCategoryId,
            };
            CurrentImage = service.ImageData;
        }

        public async Task OnImageLoaded(InputFileChangeEventArgs e)
        {
            var file = e.File;
            var dataBuffer = new byte[file.Size];
            await file.OpenReadStream(maxAllowedSize: FileResources.maxuploadSize).ReadAsync(dataBuffer);
            ImageInputData = $"data:image/png;base64,{Convert.ToBase64String(dataBuffer)}";

            if (file != null)
            {
                try
                {
                    ImageInput = e.File;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        protected ServiceDto PassServiceDto(AdminEditServiceDto dto)
        {
            if (ImageInputData.IsNullOrEmpty())
            {
                return new ServiceDto()
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    ServiceCategoryId = dto.ServiceCategoryId,
                    CategoryName = Categories.Single(c => c.Id == dto.ServiceCategoryId).Name,
                    Duration = dto.Duration,
                    Price = dto.Price,
                    ImageData = CurrentImage,
                };
            }

            return new ServiceDto()
            {
                Name = dto.Name,
                Description = dto.Description,
                ServiceCategoryId = dto.ServiceCategoryId,
                CategoryName = Categories.Single(c => c.Id == dto.ServiceCategoryId).Name,
                Duration = dto.Duration,
                Price = dto.Price,
                ImageData = ImageInputData
            };
        }

        protected async Task UpdatePreview_Click()
        {
            StateHasChanged();
        }

        protected async Task SaveEdit_Click()
        {
            try
            {
                if (ImageInput != null)
                {
                    var result = await ServiceService.PatchServiceImage(Id, ImageInput);
                    if (result == null)
                    {
                        ErrorMessage = "there was an issue uploading the image. ";
                        return;
                    }
                }

                var serviceUpdateResult = await ServiceService.PutService(Id, Service);
                if (serviceUpdateResult == null)
                {
                    ErrorMessage = "Unable to update service";
                }

                NavigationManager.NavigateTo("/admin/services");
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                throw;
            }
        }

        protected async Task BackToServices_Click()
        {
            NavigationManager.NavigateTo("admin/services");
        }
    }
}
