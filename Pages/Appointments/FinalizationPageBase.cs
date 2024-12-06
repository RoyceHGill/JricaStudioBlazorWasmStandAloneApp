using JricaStudioApp.Services.Contracts;
using JaysLashesWebApp.Models.Dtos;
using JaysLashesWebApp.Models.enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace JricaStudioApp.Pages.Appointments
{
    public class FinalizationPageBase : ComponentBase
    {
        [Parameter]
        public Guid Id { get; set; }
        [Inject]
        public IAppointmentService AppointmentService { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorageService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        public AppointmentFinalizationDto Appointment { get; set; }
        public List<AppointmentValidationAlert> ValidationAlerts { get; set; }
        public Guid UserId { get; set; }
        public string ErrorMessage { get; set; }

        protected async override Task OnInitializedAsync()
        {

            Appointment = await AppointmentService.GetAppointmentFinalization(Id);

            UserId = await ManageLocalStorageService.GetLocalUserIdGuid();

        }

        protected async Task EditAppointmentItinerary_Click(Guid id)
        {


            NavigationManager.NavigateTo($"/Appointment/Edit/{id}");
        }

        protected async Task EditIndemnity_Click(Guid id)
        {
            NavigationManager.NavigateTo($"/IndemnityForm/{id}");
        }

        protected async Task ViewUserWaverForm_Click(Guid id)
        {
            NavigationManager.NavigateTo($"/Wavier/{id}");
        }

        protected async Task EditBookingTime_Click(Guid id)
        {
            NavigationManager.NavigateTo($"/Appointment/Schedule/{id}");
        }

        protected async Task SubmitAppointment_Click()
        {
            ValidationAlerts = new List<AppointmentValidationAlert>();

            if (Appointment != null)
            {
                if (!ValidateFirstName(Appointment.FirstName))
                {
                    ValidationAlerts.Add(new AppointmentValidationAlert(Alerttype.Firstname));
                }

                if (!ValidatePhoneNumber(Appointment.Phone))
                {
                    ValidationAlerts.Add(new AppointmentValidationAlert(Alerttype.Phone));
                }

                if (!ValidateEmail(Appointment.Email))
                {
                    ValidationAlerts.Add(new AppointmentValidationAlert(Alerttype.Email));
                }

                if (!ValidateIndemnityAnswers(Appointment))
                {
                    ValidationAlerts.Add(new AppointmentValidationAlert(Alerttype.Indemnity));
                }

                if (!Appointment.IsWavierAcknowledged)
                {
                    ValidationAlerts.Add(new AppointmentValidationAlert(Alerttype.Waiver));
                }

                if (!ValidateBookingDate(Appointment.StartTime))
                {
                    ValidationAlerts.Add(new AppointmentValidationAlert(Alerttype.Booking));
                }

                if(!ValidateDateOfBirth(Appointment.DOB))
                {
                    ValidationAlerts.Add(new AppointmentValidationAlert(Alerttype.Age));
                }

                if (Appointment.Services.Count() <= 0)
                {
                    ValidationAlerts.Add(new AppointmentValidationAlert(Alerttype.Services));

                }

                if (ValidationAlerts.Count() == 0)
                {
                    var result = await AppointmentService.PatchAppointmentSubmission(Id, new UpdateAppointmentSubmissionDto()
                    {
                        FirstName = Appointment.FirstName,
                        LastName = Appointment.LastName,
                        Phone = Appointment.Phone,
                        Email = Appointment.Email,
                        StartTime = Appointment.StartTime,
                        EndTime = Appointment.EndTime,
                        DOB = Appointment.DOB,
                        HasHadEyelashExtentions = Appointment.HasHadEyelashExtentions,
                        HasAllergies = Appointment.HasAllergies,
                        HasHadEyeProblems4Weeks = Appointment.HasHadEyeProblems4Weeks,
                        HasSensitiveSkin = Appointment.HasSensitiveSkin,
                        IsWavierAcknowledged = Appointment.IsWavierAcknowledged,
                        Products = Appointment.Products,
                        Services = Appointment.Services,
                        Status = Appointment.Status,
                        WearsContanctLenses = Appointment.WearsContanctLenses
                    });
                    AppointmentService.RaiseEventOnStatusChanged(AppointmentStatus.AwaitingApproval);

                    if (result != null)
                    {
                        NavigationManager.NavigateTo($"/Appointment/Submitted/{Id}");
                    }
                    else
                    {
                        ValidationAlerts.Add(new AppointmentValidationAlert(Alerttype.ServerSide));

                        await UpdateAlerts();
                    }
                }
                else
                {
                    await UpdateAlerts();
                }
            }
        }

        private async Task UpdateAlerts()
        {
            await JSRuntime.InvokeVoidAsync("ClearAlerts");

            foreach (var alert in ValidationAlerts)
            {
                await JSRuntime.InvokeVoidAsync("AddAlert", alert.AlertMessage);
            }
        }

        private bool ValidateFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName)) { return false; }

            if (firstName.Length > 20) { return false; }

            return true;
        }

        private bool ValidatePhoneNumber(string phoneNumber)
        {
            var stripedPhoneNumber = phoneNumber.Replace(" ", "");

            if (!stripedPhoneNumber.StartsWith("04") && !stripedPhoneNumber.StartsWith("+614"))
            {
                return false;
            }

            if (stripedPhoneNumber.StartsWith("04"))
            {
                stripedPhoneNumber = stripedPhoneNumber.Remove(0, 2);
            }

            if (stripedPhoneNumber.StartsWith("+614"))
            {
                stripedPhoneNumber.Remove(0, 4);
            }

            var stripedPhoneNumberLength = stripedPhoneNumber.Length;

            if (stripedPhoneNumberLength != 8)
            {
                return false;
            }

            if (!int.TryParse(stripedPhoneNumber, NumberStyles.Integer, new CultureInfo("en-AU"), out int number))
            {
                return false;
            }

            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (!email.Contains("@"))
            {
                return false;
            }

            var splitEmail = email.Split("@");

            if (splitEmail.Length != 2)
            {
                return false;
            }

            if (splitEmail.Any(a => a.Contains("@")))
            {
                return false;
            }

            return true;
        }

        private bool ValidateIndemnityAnswers(AppointmentFinalizationDto appointment)
        {
            if (appointment.HasAllergies || appointment.HasHadEyeProblems4Weeks)
            {
                return false;
            }

            return true;
        }

        private bool ValidateBookingDate(DateTime? startTime)
        {
            if (startTime != null)
            {
                if (startTime < DateTime.Now)
                {
                    return false;
                }

                if (startTime?.Minute != 00)
                {
                    return false;
                }

                return true;
            }
            return false;
        }

        private bool ValidateDateOfBirth(DateOnly DOB)
        {
            if (DOB > DateOnly.FromDateTime(DateTime.Now.AddYears(-18)))
            {
                return false;
            }
            return true;
        }


    }

    public class AppointmentValidationAlert
    {
        public string AlertMessage { get; set; } = string.Empty;
        public Alerttype Type { get; set; }

        public AppointmentValidationAlert(Alerttype alerttype)
        {
            Type = alerttype;
            switch (alerttype)
            {
                case Alerttype.Firstname:
                    AlertMessage = "Your first name must be between 1 and 20 characters.";
                    break;
                case Alerttype.Phone:
                    AlertMessage = "Phone much be a valid Australian mobile phone number.";
                    break;
                case Alerttype.Email:
                    AlertMessage = "Email must be in a valid email.";
                    break;
                case Alerttype.Indemnity:
                    AlertMessage = "Some of your indemnity answers will prevent us from offering services to you please check your answer are correct and truthful.";
                    break;
                case Alerttype.Waiver:
                    AlertMessage = "You must accept the waiver before continuing.";
                    break;
                case Alerttype.Booking:
                    AlertMessage = "There is an issue with your booking date please review it.";
                    break;
                case Alerttype.Age:
                    AlertMessage = "You must be older then 18 to make a booking.";
                    break;
                case Alerttype.ServerSide:
                    AlertMessage = "There was an error Submitting your appointment";
                    break;
                case Alerttype.Services:
                    AlertMessage = "There are no services Selected on your appointment.";
                       break;
                default:
                    break;
            }
        }
    }

    public enum Alerttype
    {
        Firstname,
        Phone,
        Email,
        Indemnity,
        Waiver,
        Booking,
        Age,
        ServerSide,
        Services
    }
}
