﻿using JricaStudioApp.Pages.Products.Iterative;
using JricaStudioApp.Pages.Services.Iterative;
using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.BusinessHours;
using JricaStudioSharedLibrary.enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;

namespace JricaStudioApp.Pages.Appointments
{
    public class EditAppointmentItineraryPageBase : ComponentBase
    {
        [Parameter]
        public Guid Id { get; set; }
        [Inject]
        public IAppointmentService AppointmentService { get; set; }
        [Inject]
        public IAppointmentItemService AppointmentItemService { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorageService { get; set; }
        [Inject]
        public ISchedulingService SchedulingService { get; set; }

        public IEnumerable<BusinessHoursDto> BusinessHours { get; set; }
        public string ErrorMessage { get; set; }
        public AppointmentDto Appointment { get; set; }

        public decimal TotalPrice { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public int TotalProductsQuantity { get; set; }
        public int TotalServicesQuantity { get; set; }
        public decimal TotalPriceServices { get; set; }
        public decimal TotalPriceProducts { get; set; }


        protected async override Task OnInitializedAsync()
        {
            try
            {
                BusinessHours = await SchedulingService.GetBusinessHours();
                Appointment = await AppointmentService.GetAppointment( Id );

                CalculateTotals();
            }
            catch ( Exception e )
            {
                ErrorMessage = e.Message;
            }
        }

        protected void CalculateTotals()
        {
            CalculateTotalDuration();
            CalculateTotalProductQuantities();
            CalculateTotalPrice();
            CalculateAppointmentEndTime( Appointment.StartTime, TotalDuration );
        }

        private void CalculateAppointmentEndTime( DateTime? startTime, TimeSpan duration )
        {
            if ( startTime.HasValue )
            {
                Appointment.EndTime = startTime + duration;
            }
        }

        public bool ValidateProductsAndServices( IEnumerable<AppointmentServiceDto> services, IEnumerable<AppointmentProductDto> products )
        {
            return services.Any() ? true : products.Any() ? true : false;
        }

        public bool ValiddateAppointmentTimes( AppointmentDto appointment, IEnumerable<BusinessHoursDto> businessHours )
        {
            return ValidateStartTime( appointment, businessHours ) ? true : ValidateEndTime( appointment, businessHours ) ? true : false;

        }

        public bool ValidateStartTime( AppointmentDto appointment, IEnumerable<BusinessHoursDto> businessHours )
        {

            var dayOfTheWeek = appointment.StartTime?.DayOfWeek;

            var businessHoursDay = businessHours.First( x => x.Day == dayOfTheWeek );

            var startTime = TimeOnly.FromDateTime( appointment.StartTime.HasValue ? appointment.StartTime.Value : DateTime.MinValue );

            return startTime >= businessHoursDay.OpenTime && startTime < businessHoursDay.CloseTime;
        }

        public bool ValidateEndTime( AppointmentDto appointment, IEnumerable<BusinessHoursDto> businessHours )
        {

            var dayOfTheWeek = appointment.StartTime?.DayOfWeek;

            var businessHoursDay = businessHours.First( x => x.Day == dayOfTheWeek );

            var endTime = TimeOnly.FromDateTime( appointment.EndTime.HasValue ? appointment.EndTime.Value : DateTime.MinValue );

            return endTime > businessHoursDay.OpenTime && endTime <= businessHoursDay.CloseTime?.AddHours( businessHoursDay.AfterHoursGraceRange );
        }

        protected void CalculateTotalPrice()
        {
            TotalPriceProducts = Appointment.Products.Sum( p => p.Price * p.Quantity );
            TotalPriceServices = Appointment.Services.Sum( p => p.Price );
            TotalPrice = TotalPriceProducts + TotalPriceServices;
        }

        protected void CalculateTotalDuration()
        {
            var totalDuration = TimeSpan.FromMinutes( Appointment.Services.Sum( s => s.Duration.TotalMinutes ) );
            if ( totalDuration == TimeSpan.Zero )
            {
                totalDuration = TimeSpan.FromMinutes( 15 );
            }
            TotalDuration = totalDuration;

            Appointment.EndTime = Appointment.StartTime + TotalDuration;
        }

        protected void CalculateTotalProductQuantities()
        {
            TotalProductsQuantity = Appointment.Products.Sum( p => p.Quantity );
            TotalServicesQuantity = Appointment.Services.Count();
        }

        protected async void MakeVisible( string id )
        {
            await JsRuntime.InvokeVoidAsync( "SetVisible", id );
        }

        protected async void MakeInvisible( string id )
        {
            await JsRuntime.InvokeVoidAsync( "SetHidden", id );
        }

        protected async Task DeleteAppointmentProduct_Click( Guid id )
        {
            var appointmentProduct = await AppointmentItemService.DeleteAppointmentProduct( id );

            Appointment.Products = await AppointmentItemService.GetAppointmentProducts( Appointment.Id );

            CalculateTotals();
            AppointmentItemService.RaiseEventOnProductsChanged( Appointment.Products );
        }

        protected async Task DeleteAppointmentService_Click( Guid id )
        {
            var appointmentService = await AppointmentItemService.DeleteAppointmentService( id );

            Appointment.Services = await AppointmentItemService.GetAppointmentServices( Appointment.Id );

            CalculateTotals();
            AppointmentItemService.RaiseEventOnServicesChanged( Appointment.Services );

        }

        protected async Task StatusBasedNavigation_Click()
        {
            if ( Appointment == null )
            {
                ErrorMessage = "There is was an error confirming your Appointment Itinerary.";
            }
            else
            {
                if ( Appointment.Products.Count() != 0 || Appointment.Services.Count() != 0 )
                {
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
                            NavigationManager.NavigateTo( $"/Finalize/{Id}" );
                            break;
                    }
                }
                else
                {
                    ErrorMessage = "Appointment is null";
                }
            }


        }

        protected async Task UpdateAppointmentProductQuantity_Click( Guid id, int quantity )
        {
            try
            {
                if ( quantity > 0 )
                {
                    var updateDto = new AppointmentProductQuantityUpdateDto()
                    {
                        AppointmentProductId = id,
                        Quantity = quantity
                    };

                    var response = await AppointmentItemService.PatchUpdateAppoitmentProductQuantity( id, updateDto );
                }
                else
                {
                    await DeleteAppointmentProduct_Click( id );
                    return;
                }
                var newProducts = await AppointmentItemService.GetAppointmentProducts( Appointment.Id );
                AppointmentItemService.RaiseEventOnProductsChanged( newProducts );


                CalculateTotals();

                MakeInvisible( id.ToString() );
            }
            catch ( Exception )
            {

                throw;
            }
        }

    }
}
