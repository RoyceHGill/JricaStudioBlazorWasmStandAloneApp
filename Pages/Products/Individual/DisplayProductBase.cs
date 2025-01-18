using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;
using JricaStudioSharedLibrary.Dtos.BusinessHours;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Products.Individual
{
    public class DisplayProductBase : ComponentBase
    {
        [Parameter]
        public Guid Id { get; set; }
        [Parameter]
        public ProductDto PreviewProduct { get; set; }
        [Inject]
        public IProductService ProductService { get; set; }
        [Inject]
        public IAppointmentItemService AppointmentItemService { get; set; }
        [Inject]
        public IAppointmentService AppointmentService { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorageService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public ProductDto Product { get; set; }
        public Guid AppointmentId { get; set; }
        public string ErrorMessage { get; set; }
        public AppointmentAvailableDto AvailableTime { get; set; }
        public IEnumerable<BusinessHoursDto> BusinessHours { get; set; }
        public IEnumerable<AdminProductCategoryDto> Categories { get; set; }
        public AppointmentIndemnityDto Appointment { get; set; }

        protected async override Task OnParametersSetAsync()
        {

            if (PreviewProduct != null)
            {
                Product = new ProductDto()
                {
                    Name = PreviewProduct.Name,
                    Description = PreviewProduct.Description,
                    Quantity = PreviewProduct.Quantity,
                    CategoryName = Categories.Single(c => c.Id == PreviewProduct.CategoryId).Name,
                    ImageData = PreviewProduct.ImageData,
                    CategoryId = PreviewProduct.CategoryId,
                    Price = PreviewProduct.Price,
                };
            }

        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                AppointmentId = await ManageLocalStorageService.GetLocalAppointmentIdGuid();

                if (Id != default)
                {
                    Product = await ProductService.GetItem(Id);

                    Appointment = await AppointmentService.GetAppointmentIndemnity(AppointmentId);
                }
                else
                {
                    Categories = await ProductService.GetCategories();

                    Product = new ProductDto()
                    {
                        Name = PreviewProduct.Name,
                        Description = PreviewProduct.Description,
                        Quantity = PreviewProduct.Quantity,
                        CategoryName = PreviewProduct.CategoryName,
                        ImageData = PreviewProduct.ImageData,
                        CategoryId = PreviewProduct.CategoryId,
                        Price = PreviewProduct.Price,
                    };

                }

            }
            catch (Exception e)
            {

                throw;
            }
        }

        protected async Task AddToAppointment_Click(AppointmentProductToAddDto toAddDto)
        {
            try
            {
                await AppointmentItemService.PostAppointmentProduct(toAddDto);
                var dtos = await AppointmentItemService.GetAppointmentProducts(AppointmentId);

                AppointmentItemService.RaiseEventOnProductsChanged(dtos);
            }
            catch (Exception)
            {
                NavigationManager.NavigateTo($"/Appointment/Edit/{AppointmentId}");

            }
        }

    }
}
