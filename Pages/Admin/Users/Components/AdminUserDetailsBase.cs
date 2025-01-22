using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components;


namespace JricaStudioApp.Pages.Admin.Users.Components
{
    public class AdminUserDetailsBase : ComponentBase
    {
        [Parameter]
        public AdminUserDetailsDto User { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public string DetailLabelclasses { get; set; } = "col-5 text-end";
        public string BoolDetailLabelclasses { get; set; } = "col text-end";
        public string DetailRowclasses { get; set; } = "row m-2";
        public string DetailLgLabelclasses { get; set; } = "col-7 text-end";

        protected async Task GoToAppointment_Click( Guid id )
        {
            NavigationManager.NavigateTo( $"admin/appointments/edit/{id}" );
        }
    }
}
