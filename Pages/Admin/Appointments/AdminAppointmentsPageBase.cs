using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;
using JricaStudioSharedLibrary.enums;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using System.Linq.Expressions;

namespace JricaStudioApp.Pages.Admin.Appointments
{
    public class AdminAppointmentsPageBase : ComponentBase
    {
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public AdminAppointmentSearchFilterDto Filter { get; set; }
        [Parameter]
        public string? Option { get; set; }
        public AdminUserLoginDto Admin { get; set; }
        public List<AdminAppointmentDto> Appointments { get; set; }
        public AdminAppointmentDto SelectedAppointment { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorage { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IAppointmentService AppointmentService { get; set; }
        [Inject]
        public IAdminService AdminService { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await AuthenticateUser(); 
            Filter = new AdminAppointmentSearchFilterDto();
            Appointments = new List<AdminAppointmentDto>();
            StateHasChanged();
        }

        protected async override Task OnParametersSetAsync()
        {
            Filter = new AdminAppointmentSearchFilterDto();

            switch (Option)
            {
                case "AwaitingApproval":
                    Filter.Status = AppointmentStatus.AwaitingApproval;
                    Appointments = await GetAppointmentsOrderByStartTime();

                    break;
                case "Confirmed":
                    Filter.Status = AppointmentStatus.Confirmed;
                    Appointments = await GetAppointmentsOrderByStartTime();

                    break;
                case "NotFinalized":
                    Filter.Status = AppointmentStatus.NotFinalized;
                    Appointments = await GetAppointmentsOrderByStartTime();

                    break;
                case "IndemnityFormSubmited":
                    Filter.Status = AppointmentStatus.IndemnityFormSubmited;
                    Appointments = await GetAppointmentsOrderByStartTime();

                    break;
                case "WaiverAccepted":
                    Filter.Status = AppointmentStatus.WaiverAccepted;
                    Appointments = await GetAppointmentsOrderByStartTime();

                    break;
                case "BookingDateSubmited":
                    Filter.Status = AppointmentStatus.BookingDateSubmited;
                    Appointments = await GetAppointmentsOrderByStartTime();

                    break;
                case "Complete":
                    Filter.Status = AppointmentStatus.Complete;
                    Appointments = await GetAppointmentsOrderByStartTime();

                    break;
                case "NoShow":
                    Filter.Status = AppointmentStatus.NoShow;
                    Appointments = await GetAppointmentsOrderByStartTime();

                    break;
                case "CancelledByCustomer":
                    Filter.Status = AppointmentStatus.CancelledByCustomer;
                    Appointments = await GetAppointmentsOrderByStartTime();

                    break;
                case "CancelledByOperator":
                    Filter.Status = AppointmentStatus.CancelledByOperator;
                    Appointments = await GetAppointmentsOrderByStartTime();

                    break;
                default:
                    Appointments = new List<AdminAppointmentDto>();

                    Filter.Status = AppointmentStatus.AwaitingApproval;
                    break;
            }
        }

        protected async Task Enter(KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case "Enter":
                    SearchAppointments_Click();
                    break;
            }
        }

        protected async Task SelectAppointment_Click(AdminAppointmentDto appointment)
        {
            SelectedAppointment = appointment;
            StateHasChanged();
        }

        protected async Task SearchAppointments_Click()
        {
            await AuthenticateUser();
            var collection = await AppointmentService.GetAdminAppointments(Filter);
            Appointments = collection.OrderBy(x => x.StartTime).ToList();
            SelectedAppointment = null;
            StateHasChanged();
        }

        protected async Task AdminUpdateAppointmentStatus_Click(AppointmentStatus status)
        {
            if (SelectedAppointment.Status < AppointmentStatus.AwaitingApproval)
            {
                return;
            }
            var updateDto = new UpdateAppointmentStatusDto()
            {
                Status = status
            };
            await AppointmentService.PatchAdminAppointmentStatus(SelectedAppointment.Id, updateDto);
            NavigationManager.NavigateTo(NavigationManager.BaseUri + $"admin/appointments/{Filter.Status}", true);
        }

        protected void EditAppointment_Click(Guid id)
        {
            NavigationManager.NavigateTo($"admin/appointments/edit/{id}");
        }

        protected void AddNewAppointment_Click()
        {
            NavigationManager.NavigateTo("admin/NewAppointment");
        }

        private async Task<List<AdminAppointmentDto>> GetAppointmentsOrderByStartTime()
        {
            var collection = await AppointmentService.GetAdminAppointments(Filter);
            return collection.OrderBy(x => x.StartTime).ToList();
        }

        protected async Task AuthenticateUser()
        {
            if (await ManageLocalStorage.GetLocalAdminIdGuid() == Guid.Empty || await ManageLocalStorage.GetLocalAdminKeyGuid() == Guid.Empty)
            {
                NavigationManager.NavigateTo("/");
                return;
            }

            Admin = await AdminService.Reverify(await ManageLocalStorage.GetLocalAdminIdGuid());

            if (Admin == null)
            {
                NavigationManager.NavigateTo("/Admin/Login");
                return;
            }
        }
    }
}
