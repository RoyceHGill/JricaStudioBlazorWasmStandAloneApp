﻿using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos.Admin;
using JricaStudioSharedLibrary.enums;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Admin.Appointments.Components
{
    public class AppointmentRequestsBase : ComponentBase
    {

        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public int AppointmentCount { get; set; }
        public AdminUserLoginDto Admin { get; set; }
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IAppointmentService AppointmentSerivice { get; set; }
        public List<AdminAppointmentWidgetDto> Appointments { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorage { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Id = await ManageLocalStorage.GetLocalAdminIdGuid();
            Key = await ManageLocalStorage.GetLocalAdminKeyGuid();

            if (Id == Guid.Empty || Key == Guid.Empty)
            {
                NavigationManager.NavigateTo("/");
                return;
            }

            Admin = await AdminService.Reverify(Id);
            if (Admin != null)
            {
                var numerable = await AppointmentSerivice.GetAdminAppointmentRequest();
                AppointmentCount = numerable.Count();
                Appointments = numerable.OrderBy(a => a.StartTime).Take(3).ToList();
            }
        }

        protected async Task ViewAll_Click()
        {
            NavigationManager.NavigateTo(NavigationManager.BaseUri + $"Admin/Appointments/{AppointmentStatus.AwaitingApproval}");
        }
    }
}
