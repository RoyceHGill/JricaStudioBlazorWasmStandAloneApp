using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Admin.Appointments.Components
{
    public class BlockOutDatesAdministrationBase : ComponentBase
    {
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public ISchedulingService SchedulingService { get; set; }
        public AdminUserLoginDto Admin { get; set; }
        public IEnumerable<BlockOutDatesAdminDto> BlockOutDates { get; set; }
        public BlockOutDateToAddDto BlockOutDateToAdd { get; set; }
        public string ErrorMessage { get; set; }
        public string ConfirmationMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();

            BlockOutDates = await SchedulingService.GetAdminBlockOutDates();

            BlockOutDateToAdd = new BlockOutDateToAddDto
            {
                Date = DateOnly.FromDateTime(DateTime.Today)
            };
        }

        protected async Task AddBlockOutDate_Click()
        {
            try
            {
                if (BlockOutDateToAdd.Date < DateOnly.FromDateTime(DateTime.Now))
                {
                    ApplyErrorMessage("Block out date can not be in the past.");
                }
                else
                {
                    var blockOutDates = await SchedulingService.AddBlockOutDate(BlockOutDateToAdd);
                    if (blockOutDates != null)
                    {
                        BlockOutDates = blockOutDates;
                        ApplyConfirmationMessage("Block out date was successfully added.");
                        return;
                    }
                    ApplyErrorMessage("Block out date was not added.");
                }


            }
            catch (Exception e)
            {
                ApplyErrorMessage(e.Message);
            }
        }


        protected async Task DeleteBlockOutDate(Guid blockOutDateId)
        {
            try
            {
                var blockOutDates = await SchedulingService.DeleteBlockOutDate(blockOutDateId);

                if (blockOutDates != null)
                {
                    BlockOutDates = blockOutDates;
                    ApplyConfirmationMessage("Block out date was successfully deleted.");
                }
                ApplyErrorMessage("Block out date was not deleted.");
            }
            catch (Exception e)
            {
                ApplyErrorMessage(e.Message);
            }
        }

        protected void ApplyErrorMessage(string message)
        {
            ErrorMessage = message;
            ConfirmationMessage = default;
        }

        protected void ApplyConfirmationMessage(string message)
        {
            ErrorMessage = default;
            ConfirmationMessage = message;
        }

    }
}
