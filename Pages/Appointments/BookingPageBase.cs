using Blazored.LocalStorage;
using Blazorise;
using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.enums;
using Microsoft.AspNetCore.Components;
using JricaStudioSharedLibrary.Dtos.BusinessHours;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Components.Web;

namespace JricaStudioApp.Pages.Appointments
{
    public class BookingPageBase : ComponentBase, IDisposable
    {
        [Parameter]
        public Guid Id { get; set; }
        [Inject]
        public IAppointmentService AppointmentService { get; set; }
        [Inject]
        public IAppointmentItemService AppointmentItemService { get; set; }
        [Inject]
        public ISchedulingService SchedulingService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorageService { get; set; }
        public DatePicker<DateTime?> DatePicker { get; set; }
        public DateTime? DatePickerDate { get; set; }
        public AppointmentDto Appointment { get; set; }
        public IEnumerable<BusinessHoursDto> BusinessHours { get; set; }
        public IEnumerable<AppointmentUnavailaleDateDto> UnavailaleDatesDtos { get; set; }

        public IEnumerable<AppointmentAvailableDto> AvailableTimes { get; set; }

        public int ScheduleRange { get; set; } = 59;
        public string ErrorMessage { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan Duration { get; set; }

        private DateTime? startTime;

        public DateTime? StartTime
        {
            get { return startTime; }
            set
            {
                if ( value != null )
                {
                    this.EndTime = value + Duration;

                }
                startTime = value;
            }
        }

        protected async override Task OnInitializedAsync()
        {
            Appointment = await AppointmentService.GetAppointment( Id );

            Duration = CalculateDuration();
            StartTime = Appointment.StartTime;
            UnavailaleDatesDtos = await SchedulingService.GetUnavailableDates( ScheduleRange + 1, Duration );

            foreach ( var item in UnavailaleDatesDtos )
            {
                item.UnavailableDate = item.UnavailableDate.ToLocalTime();
            }
            BusinessHours = await SchedulingService.GetBusinessHours();
            AppointmentItemService.OnProductAdded += AppointmentProdcutsChanged;
            StateHasChanged();
        }

        private TimeSpan CalculateDuration()
        {
            var duration = TimeSpan.FromMinutes( Appointment.Services.Sum( s => s.Duration.TotalMinutes ) );
            if ( duration == TimeSpan.Zero )
            {
                duration = TimeSpan.FromMinutes( 15 );
            }
            return duration;
        }

        protected async Task GetAvailability_OnClick( DateTime time )
        {
            Console.WriteLine( time );
            var availableTimes = await SchedulingService.GetAvailableAppointmentsTimes( time, Duration );
            AvailableTimes = availableTimes.OrderBy( a => a.StartTime );
        }

        protected async Task SetStartTime_OnClick( DateTime time )
        {
            try
            {
                StartTime = time;
                Appointment = await AppointmentService.PatchAppointmentTimes( Id, new UpdateAppointmentTimesDto()
                {
                    StartTime = ( DateTime ) StartTime,
                    EndTime = ( DateTime ) EndTime
                } );

            }
            catch ( Exception )
            {
                throw;
            }

            AppointmentService.RaiseEventOnStartTimeChanged( time );

            StateHasChanged();
        }

        public void Dispose()
        {

            AppointmentItemService.OnProductAdded -= AppointmentProdcutsChanged;
        }

        private void AppointmentProdcutsChanged( IEnumerable<AppointmentProductDto> productDtos )
        {
            Appointment.Products = productDtos;

            StateHasChanged();
        }
        protected async Task DeleteAppointmentProduct_Click( Guid id )
        {
            var appointmentProduct = await AppointmentItemService.DeleteAppointmentProduct( id );

            Appointment.Products = await AppointmentItemService.GetAppointmentProducts( Appointment.Id );
            AppointmentItemService.RaiseEventOnProductsChanged( Appointment.Products );
        }

        public async void Enter( KeyboardEventArgs e )
        {
            if ( e.Code == "Enter" || e.Code == "NumpadEnter" )
            {
                if ( StartTime != null )
                {
                    await GetAvailability_OnClick( ( DateTime ) StartTime );
                }
            }
        }

        protected async Task ConfirmAppointmentBooking_Click()
        {
            if ( Appointment != null )
            {
                if ( Appointment.StartTime != null && Appointment.EndTime != null )
                {
                    var appointmentTimesDto = new UpdateAppointmentTimesDto()
                    {
                        StartTime = ( DateTime ) Appointment.StartTime,
                        EndTime = ( DateTime ) Appointment.EndTime
                    };

                    Appointment = await AppointmentService.PatchAppointmentTimes( Appointment.Id, appointmentTimesDto );

                    if ( Appointment.Products.Count() != 0 || Appointment.Services.Count() != 0 )
                    {
                        if ( Appointment.Status == AppointmentStatus.WaiverAccepted )
                        {

                            Appointment.Status = AppointmentStatus.BookingDateSubmited;
                            await AppointmentService.PatchAppointmentStatus( Id, new UpdateAppointmentStatusDto()
                            {
                                Status = Appointment.Status
                            } );
                            AppointmentService.RaiseEventOnStatusChanged( Appointment.Status );
                        }
                        switch ( Appointment.Status )
                        {
                            case AppointmentStatus.NotFinalized:
                                NavigationManager.NavigateTo( $"/IndemnityForm/{Id}" );
                                break;
                            case AppointmentStatus.IndemnityFormSubmited:
                                NavigationManager.NavigateTo( $"/Wavier/{await ManageLocalStorageService.GetLocalUserIdGuid()}" );
                                break;
                            case AppointmentStatus.WaiverAccepted:
                                NavigationManager.NavigateTo( $"/Appointment/Schedule/{Id}" );
                                break;
                            case AppointmentStatus.BookingDateSubmited:
                                NavigationManager.NavigateTo( $"/Finalize/{Id}" );
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    ErrorMessage = "There was a error loading your appointment";
                }
            }
        }
    }
}

