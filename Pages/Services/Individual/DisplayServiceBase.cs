using JricaStudioApp.Services.Contracts;
using JricaStudioApp.Models.Dtos;
using Microsoft.AspNetCore.Components;
using Models.Dtos.Admin;
using Models.Dtos.BusinessHours;

namespace JricaStudioApp.Pages.Services.Individual
{
    public class DisplayServiceBase : ComponentBase, IDisposable
    {
        [Parameter]
        public Guid Id { get; set; }

        [Parameter]
        public ServiceDto PreviewService { get; set; }

        [Inject]
        public IServiceService ServiceService { get; set; }
        [Inject]
        public IAppointmentItemService AppointmentItemService { get; set; }
        [Inject]
        public IAppointmentService AppointmentService { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorageService { get; set; }
        [Inject]
        public ISchedulingService SchedulingService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public ServiceDto Service { get; set; }
        public Guid AppointmentId { get; set; }
        public string ErrorMessage { get; set; }
        public AppointmentAvailableDto AvailableTime { get; set; }
        public IEnumerable<BusinessHoursDto> BusinessHours { get; set; }
        public IEnumerable<AdminServiceCategoryDto> Categories { get; set; }
        public string AvailableTimeButtonclasses { get; set; } = "";
        public int DateRange { get; set; } = 14;

        protected async override Task OnParametersSetAsync()
        {
            try
            {
                if (PreviewService != null)
                {
                    Service = new ServiceDto()
                    {
                        Name = PreviewService.Name,
                        Description = PreviewService.Description,
                        Duration = PreviewService.Duration,
                        CategoryName = Categories.Single(c => c.Id == PreviewService.ServiceCategoryId).Name,
                        ImageData = PreviewService.ImageData,
                        ServiceCategoryId = PreviewService.ServiceCategoryId,
                        Price = PreviewService.Price,
                    };
                }
            }
            catch (Exception e)
            {

                throw;
            }


        }

        private void AppointmentIdChanged(Guid id)
        {
            AppointmentId = id;
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                AppointmentService.OnIdChanged += AppointmentIdChanged;

                AppointmentId = await ManageLocalStorageService.GetLocalAppointmentIdGuid();

                if (Id != default)
                {
                    Service = await ServiceService.GetService(Id);
                }
                else
                {
                    Categories = await ServiceService.GetServiceCategories();

                    Service = new ServiceDto()
                    {
                        Name = PreviewService.Name,
                        Description = PreviewService.Description,
                        Duration = PreviewService.Duration,
                        CategoryName = Categories.Single(c => c.Id == PreviewService.ServiceCategoryId).Name,
                        ImageData = PreviewService.ImageData,
                        ServiceCategoryId = PreviewService.ServiceCategoryId,
                        Price = PreviewService.Price,
                    };

                }
                AvailableTime = await SchedulingService.GetNextAvailableAppointmentsTime(DateRange, Service.Duration);

                BusinessHours = await SchedulingService.GetBusinessHours();


                if (Id == Guid.Empty)
                {
                    AvailableTimeButtonclasses += " disabled";
                }

                if (AvailableTime == null)
                {
                    AvailableTimeButtonclasses += " visually-hidden";
                }

                if (AvailableTime != null && BusinessHours != null)
                {
                    if (TimeOnly.FromDateTime(AvailableTime.StartTime).Add(Service.Duration) > BusinessHours.SingleOrDefault(b => b.Day == AvailableTime.StartTime.DayOfWeek).CloseTime)
                    {
                        AvailableTimeButtonclasses += " btn btn-warning";
                    }
                    else
                    {
                        AvailableTimeButtonclasses += " btn btn-success jay-green-bg";
                    }
                }


                StateHasChanged();

            }
            catch (Exception e)
            {

                throw;
            }
        }

        protected async Task AddToAppointment_Click(AppointmentServiceToAddDto addDto)
        {
            if (addDto.AppointmentId == default)
            {
                addDto.AppointmentId = await ManageLocalStorageService.GetLocalAppointmentIdGuid();
            }

            AppointmentId = await ManageLocalStorageService.GetLocalAppointmentIdGuid();

            var appointment = await AppointmentService.GetAppointment(AppointmentId);

            if (appointment.Status >= JricaStudioApp.Models.enums.AppointmentStatus.AwaitingApproval)
            {
                ApplyErrorMessage("Please start a new appointment if you wish to order this Service.");
                return;
            }

            await AddAppointment(addDto);
        }

        protected async Task AddAppointment(AppointmentServiceToAddDto addDto)
        {
            try
            {
                await AppointmentItemService.PostAppointmentService(addDto);
                var dtos = await AppointmentItemService.GetAppointmentServices(AppointmentId);

                AppointmentItemService.RaiseEventOnServicesChanged(dtos);
            }
            catch (Exception)
            {

                throw;
            }

        }

        protected async Task SetStartTime_OnClick(DateTime time)
        {
            var appointment = await AppointmentService.GetAppointment(AppointmentId);

            if (appointment.Status >= JricaStudioApp.Models.enums.AppointmentStatus.AwaitingApproval)
            {
                ApplyErrorMessage("Please start a new appointment if you wish to order this Service.");
                return;
            }

            await AddAppointment(new AppointmentServiceToAddDto()
            {
                AppointmentId = AppointmentId,
                ServiceId = Service.Id
            });
            await AppointmentService.PatchAppointmentTimes(AppointmentId, new UpdateAppointmentTimesDto()
            {
                StartTime = time,
                EndTime = time.Add(Service.Duration)
            });

            NavigationManager.NavigateTo($"/Appointment/Edit/{AppointmentId}");
        }

        protected void ApplyErrorMessage(string message)
        {
            ErrorMessage = message;

        }

        public void Dispose()
        {
            AppointmentService.OnIdChanged -= AppointmentIdChanged;

        }
    }
}
