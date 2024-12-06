using JricaStudioApp.Pages.Admin.Appointments.Components;
using JricaStudioApp.Services.Contracts;
using JaysLashesWebApp.Models.Dtos;
using JaysLashesWebApp.Models.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.FileProviders;
using Models.Constants;
using Models.Dtos;
using Models.Dtos.Admin;
using System;
using System.Net;
using System.Net.Http.Headers;

namespace JricaStudioApp.Pages.Admin.Services
{
    public class AdminAddNewServicePageBase : ComponentBase
    {
        [Inject]
        public IServiceService ServiceService { get; set; }
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public IBrowserFile ImageToUpload { get; set; }
        public string ImageData { get; set; }

        public AdminUserLoginDto Admin { get; set; }
        public AdminServiceToAddDto<IBrowserFile> ServiceToAdd { get; set; }
        public IEnumerable<AdminServiceCategoryDto> Categories { get; set; }
        public UploadResultDto UploadResult { get; set; }
        public string ErrorMessage { get; set; }


        private int durationInMinutes;
        
        public int DurationInMinutes
        {
            get { return durationInMinutes; }
            set { 
                durationInMinutes = value;
                ServiceToAdd.Duration = TimeSpan.FromMinutes(durationInMinutes);
            }
        }

        public string FieldMargins { get; set; } = "mb-4 mt-3";

        protected async override Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();
            ServiceToAdd = new();
            Categories = await ServiceService.GetServiceCategories();

        }

        public async Task OnImageLoaded(InputFileChangeEventArgs e)
        {
            var file = e.File;
            var dataBuffer = new byte[file.Size];
            await file.OpenReadStream(maxAllowedSize: FileResources.maxuploadSize).ReadAsync(dataBuffer);
            ImageData = $"data:image/png;base64,{Convert.ToBase64String(dataBuffer)}";

            if (file != null)
            {
                try
                {
                    ServiceToAdd.ImageFile = e.File;
                }
                catch (Exception ex)
                {

                    throw;
                }
            } 
        }

        public async Task UploadImage(IBrowserFile file)
        {
            using var content = new MultipartFormDataContent();

            var fileContent = new StreamContent(file.OpenReadStream(FileResources.maxuploadSize));

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            content.Add(
                content: fileContent,
                name: "\"file\"",
                fileName: file.Name
            );


        }

        protected ServiceDto PassServiceDto(AdminServiceToAddDto<IBrowserFile> toAdd)
        {
            return new ServiceDto()
            {
                Name = toAdd.Name,
                Description = toAdd.Description,
                ServiceCategoryId = toAdd.ServiceCategoryId,
                CategoryName = Categories.Single(c => c.Id  == toAdd.ServiceCategoryId).Name,
                Duration = toAdd.Duration,
                Price =  toAdd.Price,
                ImageData = ImageData
            };
        }

        protected async Task UpdatePreview_Click()
        {
            StateHasChanged();
        }

        protected async Task SaveService_Click()
        {
            try
            {
                var result = await ServiceService.PostNewService(ServiceToAdd);
                NavigationManager.NavigateTo("/admin/services");
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                throw;
            }
        }

    }
}
