using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;
using JricaStudioSharedLibrary.enums;
using JricaStudioSharedLibrary.Extentions;
using Microsoft.JSInterop.Infrastructure;


using Newtonsoft.Json;
using System.CodeDom.Compiler;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;

namespace JricaStudioApp.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly HttpClient _httpClient;
        private readonly IManageLocalStorageService _localStorageService;

        public event Action<Guid> OnIdChanged;
        public event Action<AppointmentStatus> OnStatusChange;
        public event Action<DateTime> OnStartTimeChange;
        public event Action<AppointmentDto> OnUserChange;

        public AppointmentService( HttpClient httpClient, IManageLocalStorageService manageLocalStorage )
        {
            _httpClient = httpClient;
            _localStorageService = manageLocalStorage;
        }

        public void RaiseEventOnIdChanged( Guid id )
        {
            if ( OnIdChanged != null )
            {
                OnIdChanged.Invoke( id );
            }
        }

        public void RaiseEventOnStatusChanged( AppointmentStatus status )
        {
            if ( OnStatusChange != null )
            {
                OnStatusChange.Invoke( status );
            }
        }

        public void RaiseEventOnStartTimeChanged( DateTime time )
        {
            if ( OnStartTimeChange != null )
            {
                OnStartTimeChange.Invoke( time );
            }
        }

        public void RaiseEventOnUserChange( AppointmentDto UserId )
        {
            if ( OnUserChange != null )
            {
                OnUserChange.Invoke( UserId );
            }
        }

        public async Task<AppointmentDto> GetAppointment( Guid appointmentId )
        {
            try
            {
                var response = await _httpClient.GetAsync( $"api/Appointment/{appointmentId}" );
                if ( response.IsSuccessStatusCode )
                {
                    if ( response.StatusCode == System.Net.HttpStatusCode.NoContent )
                    {
                        return null;
                    }

                    var dto = await response.Content.ReadFromJsonAsync<AppointmentDto>();

                    dto = dto.ConvertDateTimesToLocalTimes();

                    return dto;
                }
                return null;
            }
            catch ( Exception )
            {

                throw;
            }
        }

        public async Task<AdminAppointmentDto> GetAdminAppointment( Guid appointmentId )
        {
            try
            {
                await AddAdminHeader();
                var response = await _httpClient.GetAsync( $"api/Appointment/Admin/{appointmentId}" );
                RemoveAdminHeader();

                if ( response.IsSuccessStatusCode )
                {
                    if ( response.StatusCode == System.Net.HttpStatusCode.NoContent )
                    {
                        throw new Exception( "User Not Found" );
                    }
                    var dto = await response.Content.ReadFromJsonAsync<AdminAppointmentDto>();

                    dto = dto.ConvertDateTimesToLocalTimes();

                    return dto;
                }
                return null;
            }
            catch ( Exception )
            {
                throw;
            }
        }

        public async Task<AppointmentIndemnityDto> GetAppointmentIndemnity( Guid appointmentId )
        {
            try
            {
                var response = await _httpClient.GetAsync( $"api/Appointment/Indemnity/{appointmentId}" );

                if ( response.IsSuccessStatusCode )
                {
                    var appointmentIndemnityDto = await response.Content.ReadFromJsonAsync<AppointmentIndemnityDto>();

                    appointmentIndemnityDto = appointmentIndemnityDto.ConvertDateTimesToLocalTimes();

                    return appointmentIndemnityDto;
                }
                return null;
            }
            catch ( Exception )
            {

                throw;
            }
        }

        public async Task<AppointmentFinalizationDto> GetAppointmentFinalization( Guid appointmentId )
        {
            try
            {
                var response = await _httpClient.GetAsync( $"api/Appointment/Finalization/{appointmentId}" );

                if ( response.IsSuccessStatusCode )
                {
                    var appointmentFinaliztionDto = await response.Content.ReadFromJsonAsync<AppointmentFinalizationDto>();

                    appointmentFinaliztionDto = appointmentFinaliztionDto.ConvertDateTimesToLocalTimes();

                    return appointmentFinaliztionDto;
                }
                return null;
            }

            catch ( Exception )
            {

                throw;
            }
        }

        public async Task<AppointmentExistsDto> GetAppointmentExists( Guid id )
        {

            try
            {
                var response = await _httpClient.GetAsync( $"api/Appointment/Exists/{id}" );

                if ( response.IsSuccessStatusCode )
                {
                    var dto = await response.Content.ReadFromJsonAsync<AppointmentExistsDto>();

                    dto = dto.ConvertDateTimesToLocalTimes();

                    return dto;
                }

                else
                {
                    var message = response.Content.ReadAsStringAsync();
                    throw new Exception( $"Http status: {response.StatusCode}Message: {message}" );

                }
            }
            catch ( Exception )
            {

                throw;
            }

        }

        public async Task<AppointmentIndemnityDto> PatchAppointmentIndemityForm( Guid id, UpdateAppointmentIndemnityDto indemnityDto )
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject( indemnityDto );
                var content = new StringContent( jsonRequest, Encoding.UTF8, "application/json-patch+json" );

                var response = await _httpClient.PatchAsync( $"api/Appointment/Indemnity/{id}", content );

                if ( response.IsSuccessStatusCode )
                {
                    var dto = await response.Content.ReadFromJsonAsync<AppointmentIndemnityDto>();

                    dto = dto.ConvertDateTimesToLocalTimes();

                    return dto;
                }
                return null;
            }
            catch ( Exception )
            {
                throw;
            }
        }

        public async Task<AppointmentDto> PostAppointment( AppointmentToAddDto appointment )
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<AppointmentToAddDto>( "api/Appointment", appointment );

                if ( response.IsSuccessStatusCode )
                {
                    var dto = await response.Content.ReadFromJsonAsync<AppointmentDto>();

                    dto = dto.ConvertDateTimesToLocalTimes();

                    return dto;
                }

                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception( $"Http status: {response.StatusCode}Message: {message}" );
                }
            }
            catch ( Exception e )
            {

                throw;
            }
        }

        public async Task<AppointmentDto> PatchAppointmentTimes( Guid id, UpdateAppointmentTimesDto timesDto )
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject( timesDto );
                var content = new StringContent( jsonRequest, Encoding.UTF8, "application/json-patch+json" );

                var response = await _httpClient.PatchAsync( $"api/Appointment/UpdateTimes/{id}", content );

                if ( response.IsSuccessStatusCode )
                {
                    var dto = await response.Content.ReadFromJsonAsync<AppointmentDto>();

                    dto = dto.ConvertDateTimesToLocalTimes();

                    return dto;
                }
                return null;
            }
            catch ( Exception )
            {
                throw;
            }
        }

        public async Task<AppointmentDto> PatchAppointmentStatus( Guid id, UpdateAppointmentStatusDto status )
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject( status );
                var content = new StringContent( jsonRequest, Encoding.UTF8, "application/json-patch+json" );

                var response = await _httpClient.PatchAsync( $"api/Appointment/UpdateStatus/{id}", content );

                if ( response.IsSuccessStatusCode )
                {
                    var dto = await response.Content.ReadFromJsonAsync<AppointmentDto>();

                    dto = dto.ConvertDateTimesToLocalTimes();

                    return dto;
                }
                return null;
            }
            catch ( Exception )
            {
                throw;
            }
        }

        public async Task<AppointmentFinalizationDto> PatchAppointmentSubmission( Guid id, UpdateAppointmentSubmissionDto dto )
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject( dto );
                var content = new StringContent( jsonRequest, Encoding.UTF8, "application/json-patch+json" );

                var response = await _httpClient.PatchAsync( $"api/Appointment/Submit/{id}", content );

                if ( response.IsSuccessStatusCode )
                {

                    var dtoResponse = await response.Content.ReadFromJsonAsync<AppointmentFinalizationDto>();

                    dtoResponse = dtoResponse.ConvertDateTimesToLocalTimes();

                    return dtoResponse;
                }
                return null;
            }
            catch ( Exception )
            {
                throw;
            }
        }

        public async Task<IEnumerable<AdminAppointmentWidgetDto>> GetAdminAppointmentRequest()
        {
            try
            {
                await AddAdminHeader();
                var response = await _httpClient.GetAsync( $"api/Appointment/Requests" );
                RemoveAdminHeader();

                if ( response.IsSuccessStatusCode )
                {
                    if ( response.StatusCode == System.Net.HttpStatusCode.NoContent )
                    {
                        throw new NullReferenceException( "No Appointments" );
                    }

                    var dto = await response.Content.ReadFromJsonAsync<IEnumerable<AdminAppointmentWidgetDto>>();

                    dto = dto.ConvertCollectionsDateTimesToLocalTime();

                    return dto;
                }
                return default;
            }
            catch ( NullReferenceException ne )
            {
                return new List<AdminAppointmentWidgetDto>();
            }
            catch ( Exception e )
            {
                throw;
            }
        }

        private async Task AddAdminHeader()
        {
            var key = await _localStorageService.GetLocalAdminKeyGuid();

            if ( !_httpClient.DefaultRequestHeaders.TryGetValues( "AdminKey", out var values ) )
            {
                _httpClient.DefaultRequestHeaders.Add( "AdminKey", key.ToString() );
            }
        }

        private void RemoveAdminHeader()
        {
            _httpClient.DefaultRequestHeaders.Remove( "Adminkey" );
        }

        public async Task<IEnumerable<AdminAppointmentWidgetDto>> GetAdminUpcomingAppointments()
        {
            try
            {
                await AddAdminHeader();
                var response = await _httpClient.GetAsync( $"api/Appointment/Upcoming" );
                RemoveAdminHeader();

                if ( response.IsSuccessStatusCode )
                {
                    if ( response.StatusCode == System.Net.HttpStatusCode.NoContent )
                    {
                        throw new NullReferenceException( "No Appointments" );
                    }
                    var appointments = await response.Content.ReadFromJsonAsync<IEnumerable<AdminAppointmentWidgetDto>>();

                    appointments = appointments.ConvertCollectionsDateTimesToLocalTime();

                    return appointments;
                }
                return default;
            }
            catch ( NullReferenceException ne )
            {
                return new List<AdminAppointmentWidgetDto>();
            }
            catch ( Exception e )
            {
                throw;
            }
        }

        public async Task<IEnumerable<AdminAppointmentDto>> GetAdminAppointments( AdminAppointmentSearchFilterDto filter )
        {
            try
            {
                await AddAdminHeader();
                var response = await _httpClient.PostAsJsonAsync( $"api/Appointment/Search/", filter );
                RemoveAdminHeader();
                if ( response.IsSuccessStatusCode )
                {
                    if ( response.StatusCode == System.Net.HttpStatusCode.NoContent )
                    {
                        throw new NullReferenceException( "No Appointments" );
                    }
                    var appointments = await response.Content.ReadFromJsonAsync<IEnumerable<AdminAppointmentDto>>();

                    appointments = appointments.ConvertCollectionsDateTimesToLocalTime();

                    return appointments;
                }
                return default;
            }
            catch ( NullReferenceException ne )
            {
                return new List<AdminAppointmentDto>();
            }
            catch ( Exception e )
            {
                throw;
            }
        }

        public async Task<AppointmentDto> PatchAdminAppointmentStatus( Guid id, UpdateAppointmentStatusDto status )
        {
            try
            {
                await AddAdminHeader();

                var jsonRequest = JsonConvert.SerializeObject( status );
                var content = new StringContent( jsonRequest, Encoding.UTF8, "application/json-patch+json" );

                var response = await _httpClient.PatchAsync( $"api/Appointment/UpdateStatusAdmin/{id}", content );
                RemoveAdminHeader();


                if ( response.IsSuccessStatusCode )
                {
                    var dto = await response.Content.ReadFromJsonAsync<AppointmentDto>();

                    dto = dto.ConvertDateTimesToLocalTimes();

                    return dto;
                }

                return null;
            }
            catch ( Exception )
            {
                throw;
            }
        }

        public async Task<AppointmentDto> PatchAppointmentUserId( Guid id, UpdateAppointmentUserDto dto )
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject( dto );
                var content = new StringContent( jsonRequest, Encoding.UTF8, "application/json-patch+json" );

                var response = await _httpClient.PatchAsync( $"api/Appointment/UpdateUser/{id}", content );

                if ( response.IsSuccessStatusCode )
                {
                    var dtoResponse = await response.Content.ReadFromJsonAsync<AppointmentDto>();

                    dtoResponse = dtoResponse.ConvertDateTimesToLocalTimes();

                    return dtoResponse;
                }
                return null;
            }
            catch ( Exception )
            {
                throw;
            }
        }

        public async Task<AppointmentDto> UpdateAppointment( Guid id, UpdateAppointmentDto dto )
        {
            try
            {
                await AddAdminHeader();
                var jsonRequest = JsonConvert.SerializeObject( dto );
                var content = new StringContent( jsonRequest, Encoding.UTF8, "application/json-patch+json" );

                var response = await _httpClient.PatchAsync( $"api/Appointment/Update/{id}", content );
                RemoveAdminHeader();
                if ( response.IsSuccessStatusCode )
                {
                    var dtoResponse = await response.Content.ReadFromJsonAsync<AppointmentDto>();

                    dtoResponse = dtoResponse.ConvertDateTimesToLocalTimes();

                    return dtoResponse;
                }

                return null;
            }
            catch ( Exception )
            {
                throw;
            }
        }

        public async Task<AppointmentAdminToAddDto> PostAppointmentAdmin( AppointmentAdminToAddDto dto )
        {
            try
            {
                await AddAdminHeader();
                var jsonRequest = JsonConvert.SerializeObject( dto );
                var content = new StringContent( jsonRequest, Encoding.UTF8, "application/json-patch+json" );

                var response = await _httpClient.PostAsync( "api/Appointment/Admin", content );

                if ( response.IsSuccessStatusCode & response != null )
                {

                    var dtoResponse = await response.Content.ReadFromJsonAsync<AppointmentAdminToAddDto>();

                    dtoResponse = dtoResponse.ConvertDateTimesToLocalTimes();

                    return dtoResponse;
                }

                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception( $"Http status: {response.StatusCode}Message: {message}" );
                }
            }
            catch ( Exception e )
            {

                throw;
            }
        }
    }
}
