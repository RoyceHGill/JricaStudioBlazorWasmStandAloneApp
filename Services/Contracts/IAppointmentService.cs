using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;
using JricaStudioSharedLibrary.enums;

namespace JricaStudioApp.Services.Contracts
{
    public interface IAppointmentService
    {
        event Action<Guid> OnIdChanged;
        event Action<AppointmentStatus> OnStatusChange;
        event Action<DateTime> OnStartTimeChange;
        event Action<AppointmentDto> OnUserChange;

        void RaiseEventOnIdChanged(Guid id);
        void RaiseEventOnStatusChanged(AppointmentStatus status);
        void RaiseEventOnStartTimeChanged(DateTime status);
        void RaiseEventOnUserChange(AppointmentDto Id);

        Task<AppointmentAdminToAddDto> PostAppointmentAdmin(AppointmentAdminToAddDto dto);
        Task<AppointmentDto> GetAppointment(Guid appointmentId);
        Task<AppointmentIndemnityDto> GetAppointmentIndemnity(Guid appointmentId);
        Task<AppointmentFinalizationDto> GetAppointmentFinalization(Guid appointmentId);
        Task<AppointmentDto> PostAppointment(AppointmentToAddDto appointment);
        Task<AppointmentExistsDto> GetAppointmentExists(Guid id);
        Task<AppointmentIndemnityDto> PatchAppointmentIndemityForm(Guid id, UpdateAppointmentIndemnityDto indemnityDto);
        Task<AppointmentDto> PatchAppointmentTimes(Guid id, UpdateAppointmentTimesDto timesDto);
        Task<AppointmentDto> PatchAppointmentStatus(Guid id, UpdateAppointmentStatusDto status);
        Task<AppointmentFinalizationDto> PatchAppointmentSubmission(Guid id, UpdateAppointmentSubmissionDto dto);
        Task<AppointmentDto> PatchAppointmentUserId(Guid appointmentId, UpdateAppointmentUserDto dto);
        Task<IEnumerable<AdminAppointmentWidgetDto>> GetAdminAppointmentRequest();
        Task<IEnumerable<AdminAppointmentWidgetDto>> GetAdminUpcomingAppointments();
        Task<IEnumerable<AdminAppointmentDto>> GetAdminAppointments(AdminAppointmentSearchFilterDto filter);
        Task<AdminAppointmentDto> GetAdminAppointment(Guid appointmentId);
        Task<AppointmentDto> PatchAdminAppointmentStatus(Guid id, UpdateAppointmentStatusDto status);
        Task<AppointmentDto> UpdateAppointment(Guid id, UpdateAppointmentDto dto);

    }
}
