using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;
using JricaStudioSharedLibrary.Dtos.Admin.BusinessHours;
using JricaStudioSharedLibrary.Dtos.BusinessHours;




namespace JricaStudioApp.Services.Contracts
{
    public interface ISchedulingService
    {
        Task<IEnumerable<AppointmentUnavailaleDateDto>> GetUnavailableDates(int dateRange, TimeSpan duration);
        Task<IEnumerable<AppointmentAvailableDto>> GetAvailableAppointmentsTimes(DateTime Date, TimeSpan duration);
        Task<AppointmentAvailableDto> GetNextAvailableAppointmentsTime(int dateRange, TimeSpan duration);

        Task<IEnumerable<BusinessHoursDto>> GetBusinessHours();
        Task<IEnumerable<AdminBusinessHoursDto>> GetAdminBusinessHours();
        Task<IEnumerable<AdminBusinessHoursDto>> PutBusinessHours(IEnumerable<AdminBusinessHoursDto> businessHours);
        Task<IEnumerable<BlockOutDatesAdminDto>> GetAdminBlockOutDates();
        Task<IEnumerable<BlockOutDatesAdminDto>> AddBlockOutDate(BlockOutDateToAddDto dto);
        Task<IEnumerable<BlockOutDatesAdminDto>> DeleteBlockOutDate(Guid blockOutDateId);
    }
}
