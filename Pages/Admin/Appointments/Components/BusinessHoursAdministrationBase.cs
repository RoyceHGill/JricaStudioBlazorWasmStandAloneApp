using Blazorise.Extensions;
using JricaStudioApp.Extensions;
using JricaStudioApp.Services.Contracts;
using JaysLashesWebApp.Models.Dtos;
using JaysLashesWebApp.Models.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Models.Dtos.Admin.BusinessHours;

namespace JricaStudioApp.Pages.Admin.Appointments.Components
{
    public class BusinessHoursAdministrationBase : ComponentBase
    {
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public ISchedulingService SchedulingService { get; set; }
        public AdminUserLoginDto Admin { get; set; }
        public IEnumerable<AdminBusinessHoursDto> BusinessHours { get; set; }
        public List<string> ValiadationErrorMessages { get; set; }
        public string ErrorMessage { get; set; } = "";
        public string ConfirmationMessage { get; set; } = "";

        protected async override Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();

            BusinessHours = await SchedulingService.GetAdminBusinessHours();

            if (BusinessHours == null || !BusinessHours.Any())
            {
                var newBusinessHours = new List<AdminBusinessHoursDto>();

                foreach (DayOfWeek item in Enum.GetValues(typeof(DayOfWeek)))
                {
                    newBusinessHours.Add(new AdminBusinessHoursDto
                    {
                        OpenTime = TimeOnly.MinValue,
                        CloseTime = TimeOnly.MaxValue,
                        Day = item,
                        IsDisabled = true,
                        AfterHoursGraceRange = 1
                    });
                }
                BusinessHours = newBusinessHours;
            }


        }

        protected async Task SaveBusinessHours_Click()
        {
            try
            {
                ValiadationErrorMessages = new List<string>();
                foreach (var item in BusinessHours)
                {
                    if (item.OpenTime > item.CloseTime)
                    {
                        ValiadationErrorMessages.Add($"{item.Day}: {nameof(item.OpenTime).FromPascalToString()} " +
                            $"{item.OpenTime.ToString()} can not be later than " +
                            $"{nameof(item.CloseTime).FromPascalToString()} {item.CloseTime.ToString()}.");
                    }
                }
                if (ValiadationErrorMessages.Count() == 0)
                {
                    foreach (var item in BusinessHours)
                    {
                        item.LocalTimeOffset = DateTimeOffset.Now.Offset;
                    }

                    var businesshours = await SchedulingService.PutBusinessHours(BusinessHours);

                    if (businesshours.Any())
                    {
                        BusinessHours = businesshours;

                        StateHasChanged();
                        ApplyConfirmationMessage("business hours updated.");
                    }
                }
            }
            catch (Exception e)
            {
                ApplyErrorMessage(e.Message);
            }
        }

        protected async Task ReloadBusinessHours_Click()
        {
            try
            {
                var businessHours = await SchedulingService.GetAdminBusinessHours();

                if (businessHours != null)
                {
                    BusinessHours = businessHours;
                    StateHasChanged();
                    ApplyConfirmationMessage("business hours reverted to last save.");
                }
            }
            catch (Exception e)
            {
                ApplyErrorMessage(e.Message);
            }
        }

        protected void ApplyConfirmationMessage(string message)
        {
            ErrorMessage = "";
            ConfirmationMessage = message.ToCamelcase();
        }
        protected void ApplyErrorMessage(string message)
        {
            ErrorMessage = message.ToCamelcase();
            ConfirmationMessage = "";
        }
    }
}
