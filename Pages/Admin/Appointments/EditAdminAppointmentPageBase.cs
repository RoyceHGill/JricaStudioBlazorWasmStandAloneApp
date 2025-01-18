using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioApp.Models.Dtos;
using JricaStudioApp.Models.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace JricaStudioApp.Pages.Admin.Appointments
{
    public class EditAdminAppointmentPageBase : ComponentBase
    {
        [Parameter]
        public Guid Id { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorage { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IAppointmentService AppointmentService { get; set; }
        [Inject]
        public IProductService ProductService { get; set; }
        [Inject]
        public IServiceService ServiceService { get; set; }
        [Inject]
        public IAppointmentItemService AppointmentItemService { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public IAdminService AdminService { get; set; }
        public AdminAppointmentDto Appointment { get; set; }
        public AdminUserLoginDto Admin { get; set; }
        public IEnumerable<ProductDto>? Products { get; set; }
        public IEnumerable<ServiceDto>? Services { get; set; }
        public bool isAddingNewProduct { get; set; } = false;
        public bool isAddingNewService { get; set; } = false;
        public string AddProductErrorMessage { get; set; }
        public string AddServiceErrorMessage { get; set; }
        public string ErrorMessage { get; set; }
        public Guid? ProductToAddId { get; set; }
        public Guid? ServiceToAddId { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalPrice { get; set; }

        public decimal TotalServicesPrice { get; set; }
        public TimeSpan Duration { get; set; }
        public int ServicesCount { get; set; }


        protected async override Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();
            Appointment = await AppointmentService.GetAdminAppointment(Id);

            Products = await ProductService.GetItems();
            Services = await ServiceService.GetServices();

        }
        protected void CalculateTotals()
        {
            CalculateTotalDuration();
            GetTotalServicesCount();
            CalculateTotalServicePrice();
        }

        protected void CalculateTotalServicePrice()
        {
            TotalServicesPrice = Appointment.Services.Sum(p => p.Price);
        }

        protected void CalculateTotalDuration()
        {
            var totalDuration = TimeSpan.FromMinutes(Appointment.Services.Sum(s => s.Duration.TotalMinutes));
            if (totalDuration == TimeSpan.Zero)
            {
                totalDuration = TimeSpan.FromMinutes(15);
            }
            Duration = totalDuration;
        }

        protected void GetTotalServicesCount()
        {
            ServicesCount = Appointment.Services.Count();
        }

        protected async Task DeleteAppointmentService_Click(Guid id)
        {

            Appointment.Services = await AppointmentItemService.DeleteAppointmentService(id);

            CalculateTotals();
        }

        protected async Task DeleteAppointmentProduct_Click(Guid id)
        {
            Appointment.Products = await AppointmentItemService.DeleteAppointmentProduct(id);
            isAddingNewProduct = false;
        }

        protected async Task UpdateAppointmentProductQuantity_Click(Guid id, int qty)
        {
            try
            {
                if (qty > 0)
                {
                    var updateDto = new AppointmentProductQuantityUpdateDto()
                    {
                        AppointmentProductId = id,
                        Quantity = qty
                    };

                    var response = await AppointmentItemService.PatchUpdateAppoitmentProductQuantity(id, updateDto);
                }
                else
                {
                    await DeleteAppointmentProduct_Click(id);
                    isAddingNewProduct = false;
                    return;
                }
                var newProducts = await AppointmentItemService.GetAppointmentProducts(await ManageLocalStorage.GetLocalAppointmentIdGuid());


                UpdateTotals();

                MakeInvisible(id.ToString());
                isAddingNewProduct = false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void UpdateTotals()
        {
            ProductCount = CalculateQuantities(Appointment.Products);
            TotalPrice = CalculateTotalPrice(Appointment.Products);
        }

        protected decimal CalculateTotalPrice(IEnumerable<AppointmentProductDto> products)
        {
            return (decimal)products.Sum(p => p.Price * p.Quantity);
        }

        protected int CalculateQuantities(IEnumerable<AppointmentProductDto> products)
        {
            return products.Sum(p => p.Quantity);
        }

        protected async void MakeVisible(string id)
        {
            await JS.InvokeVoidAsync("SetVisible", id);
        }

        protected async void MakeInvisible(string id)
        {
            await JS.InvokeVoidAsync("SetHidden", id);
        }

        protected async Task AddProduct_Click()
        {
            try
            {
                if (ProductToAddId == null || ProductToAddId == Guid.Empty)
                {
                    AddProductErrorMessage = "Please Select a Product";
                    return;
                }
                if (AddProductErrorMessage != null)
                {
                    AddProductErrorMessage = string.Empty;
                }
                var result = await AppointmentItemService.PostAdminAppointmentProduct(new AppointmentProductToAddDto()
                {
                    AppointmentId = Id,
                    ProductId = (Guid)ProductToAddId,
                    Quantity = 1
                });

                Appointment = await AppointmentService.GetAdminAppointment(Id);
                await ToggleIsAddingNewProduct_Click();
                StateHasChanged();
            }
            catch (Exception e)
            {

                throw;
            }

        }

        protected async Task AddService_Click()
        {
            try
            {
                if (ServiceToAddId == null || ServiceToAddId == Guid.Empty)
                {
                    AddServiceErrorMessage = "Please Select a Service";
                    return;
                }
                if (AddServiceErrorMessage != null)
                {
                    AddServiceErrorMessage = null;
                }
                var result = await AppointmentItemService.PostAdminAppointmentService(new AppointmentServiceToAddDto()
                {
                    AppointmentId = Id,
                    ServiceId = (Guid)ServiceToAddId,
                });

                Appointment = await AppointmentService.GetAdminAppointment(Id);
                await ToggleIsAddingNewService_Click();
                StateHasChanged();
            }
            catch (Exception e)
            {

                throw;
            }
        }

        protected async Task OnProductSelected()
        {
            AddProductErrorMessage = string.Empty;
        }

        protected async Task ToggleIsAddingNewProduct_Click()
        {
            if (isAddingNewProduct)
            {
                isAddingNewProduct = false;
            }
            else
            {
                isAddingNewProduct = true;
            }
            StateHasChanged();
        }

        protected async Task ToggleIsAddingNewService_Click()
        {
            if (isAddingNewService)
            {
                isAddingNewService = false;
            }
            else
            {
                isAddingNewService = true;
            }
            StateHasChanged();
        }

        protected async Task GoToUser()
        {
            NavigationManager.NavigateTo($"admin/users/edit/{Appointment.User.Id}");
        }

        protected async Task SaveAppointment_Click()
        {
            try
            {
                var appointment = await AppointmentService.UpdateAppointment(Id, new UpdateAppointmentDto
                {
                    StartTime = Appointment.StartTime,
                    EndTime = Appointment.EndTime,
                    IsDepositPaid = Appointment.IsDepositPaid,
                    IsSampleSetComplete = Appointment.IsSampleSetComplete,
                    HasHadEyelashExtentions = Appointment.HasHadEyelashExtentions,
                    SampleSetCompleted = Appointment.SampleSetCompleted,
                    Status = Appointment.Status
                });

                NavigationManager.NavigateTo($"admin/appointments/{Appointment.Status}");

            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        protected async Task BackToAppoitnments()
        {
            NavigationManager.NavigateTo($"admin/appointments/{Appointment.Status}");
        }
    }
}
