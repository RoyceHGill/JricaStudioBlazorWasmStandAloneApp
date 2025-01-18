using JricaStudioApp.Constants;
using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioApp.Models.Dtos;
using JricaStudioApp.Models.enums;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.User
{
    public class WaiverFormBase : ComponentBase
    {
        [Parameter]
        public Guid Id { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorage { get; set; }
        [Inject]
        public IUserService UserService { get; set; }
        [Inject]
        public IAppointmentService AppointmentService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public Guid AppointmentId { get; set; }
        public UserWaiverDto User { get; set; }
        public string WaiverFormText { get; set; }
        public string ErrorMessage { get; set; }


        protected override async Task OnInitializedAsync()
        {
            User = await UserService.GetUserWaiver(Id);
            AppointmentId = await ManageLocalStorage.GetLocalAppointmentIdGuid();
        }

        protected async Task OnWaiverAccepted_CheckBoxTicked(bool isAccepted)
        {
            await UserService.PatchAcceptUserWaiver(Id, isAccepted);
        }

        protected async Task StatusBasedNavigation_Click()
        {
            if (User.IsWavierAcknowledged == true)
            {
                var appointment = await AppointmentService.GetAppointment(AppointmentId);

                if (appointment == null)
                {
                    ErrorMessage = "There is was an error confirming your Appointment Itinerary.";
                }
                else
                {
                    if (appointment.Products.Count() != 0 || appointment.Services.Count() != 0)
                    {
                        if (appointment.Status == AppointmentStatus.IndemnityFormSubmited)
                        {

                            appointment.Status = AppointmentStatus.WaiverAccepted;
                            await AppointmentService.PatchAppointmentStatus(AppointmentId, new UpdateAppointmentStatusDto()
                            {
                                Status = appointment.Status
                            });
                            AppointmentService.RaiseEventOnStatusChanged(appointment.Status);
                        }
                        switch (appointment.Status)
                        {
                            case AppointmentStatus.NotFinalized:
                                NavigationManager.NavigateTo($"/IndemnityForm/{AppointmentId}");
                                break;
                            case AppointmentStatus.IndemnityFormSubmited:
                                NavigationManager.NavigateTo($"/Wavier/{Id}");
                                break;
                            case AppointmentStatus.WaiverAccepted:
                                NavigationManager.NavigateTo($"/Appointment/Schedule/{AppointmentId}");
                                break;
                            case AppointmentStatus.BookingDateSubmited:
                                NavigationManager.NavigateTo($"/Finalize/{AppointmentId}");
                                break;
                            default:
                                NavigationManager.NavigateTo($"/Finalize/{AppointmentId}");
                                break;
                        }
                    }
                    else
                    {
                        ErrorMessage = "Appointment is null";
                    }
                }
            }




        }
    }
}
