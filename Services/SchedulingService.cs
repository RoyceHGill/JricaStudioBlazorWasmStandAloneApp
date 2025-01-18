using JricaStudioApp.Services.Contracts;
using JricaStudioApp.Models.Dtos;
using JricaStudioApp.Models.Dtos.Admin;
using JricaStudioApp.Models.Dtos.BusinessHours;
using JricaStudioApp.Server.Extentions;
using Models.Dtos.Admin;
using Models.Dtos.Admin.BusinessHours;
using Models.Dtos.BusinessHours;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;

namespace JricaStudioApp.Services
{
    public class SchedulingService : ISchedulingService
    {
        private readonly HttpClient _httpClient;
        private readonly IManageLocalStorageService _manageLocalStorageService;

        public SchedulingService(HttpClient httpClient, IManageLocalStorageService manageLocalStorageService)
        {
            this._httpClient = httpClient;
            _manageLocalStorageService = manageLocalStorageService;
        }

        public async Task<IEnumerable<AppointmentAvailableDto>> GetAvailableAppointmentsTimes(DateTime date, TimeSpan duration)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Scheduling/GetAvailability/Times?date={date}&duration={duration}");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<AppointmentAvailableDto>();
                    }

                    var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentAvailableDto>>();


                    dtos = dtos.ConvertCollectionsDateTimesToLocalTime();


                    return dtos;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<AppointmentAvailableDto> GetNextAvailableAppointmentsTime(int dateRange, TimeSpan duration)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Scheduling/GetAvailability/NextTime?dateRange={dateRange}&duration={duration}");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default;
                    }

                    var dto = await response.Content.ReadFromJsonAsync<AppointmentAvailableDto>();

                    dto = dto.ConvertDateTimesToLocalTime();

                    return dto;

                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }


        public async Task<IEnumerable<AppointmentUnavailaleDateDto>> GetUnavailableDates(int dateRange, TimeSpan duration)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Scheduling/GetUnavailability/Dates?dateRange={dateRange}&duration={duration}");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<AppointmentUnavailaleDateDto>();
                    }

                    var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentUnavailaleDateDto>>();

                    dtos = dtos.ConvertCollectionsDateTimesToLocalTime();

                    return dtos;

                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BusinessHoursDto>> GetBusinessHours()
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Scheduling/GetBusinessHours");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<BusinessHoursDto>();
                    }

                    var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<BusinessHoursDto>>();

                    dtos = ApplyLocalTimeOffset(dtos);

                    return dtos;

                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<IEnumerable<AdminBusinessHoursDto>> GetAdminBusinessHours()
        {
            try
            {
                try
                {
                    await AddAdminHeaders();
                    var response = await _httpClient.GetAsync($"api/Scheduling/AdminBusinessHours");
                    RemoveAdminHeaders();
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        {
                            return Enumerable.Empty<AdminBusinessHoursDto>();
                        }

                        var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<AdminBusinessHoursDto>>();

                        dtos = ApplyLocalTimeOffset(dtos);

                        return dtos;

                    }
                    else
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        throw new Exception(message);
                    }
                }
                catch (Exception e)
                {

                    throw;
                }

            }
            catch (Exception e)
            {

                throw;
            }
        }

        private async Task AddAdminHeaders()
        {
            RemoveAdminHeaders();
            var key = await _manageLocalStorageService.GetLocalAdminKeyGuid();

            if (!_httpClient.DefaultRequestHeaders.TryGetValues("AdminKey", out var values))
            {
                _httpClient.DefaultRequestHeaders.Add("AdminKey", key.ToString());
            }
        }

        private void RemoveAdminHeaders()
        {
            _httpClient.DefaultRequestHeaders.Remove("Adminkey");
        }

        public async Task<IEnumerable<AdminBusinessHoursDto>> PutBusinessHours(IEnumerable<AdminBusinessHoursDto> businessHours)
        {
            try
            {
                await AddAdminHeaders();

                var jsonRequest = JsonConvert.SerializeObject(businessHours);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

                var response = await _httpClient.PutAsync($"api/Scheduling/BusinessHours", content);
                RemoveAdminHeaders();


                if (response.IsSuccessStatusCode)
                {
                    var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<AdminBusinessHoursDto>>();

                    dtos = ApplyLocalTimeOffset(dtos);

                    return dtos;
                }

                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status: {response.StatusCode}Message: {message}");
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BlockOutDatesAdminDto>> GetAdminBlockOutDates()
        {
            try
            {
                await AddAdminHeaders();
                var response = await _httpClient.GetAsync("api/Scheduling/BlockOutDates");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<BlockOutDatesAdminDto>();
                    }

                    return await response.Content.ReadFromJsonAsync<IEnumerable<BlockOutDatesAdminDto>>();

                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BlockOutDatesAdminDto>> AddBlockOutDate(BlockOutDateToAddDto dto)
        {
            try
            {
                await AddAdminHeaders();
                var response = await _httpClient.PostAsJsonAsync<BlockOutDateToAddDto>("api/Scheduling/BlockOutDates", dto);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<BlockOutDatesAdminDto>();
                    }

                    return await response.Content.ReadFromJsonAsync<IEnumerable<BlockOutDatesAdminDto>>();

                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BlockOutDatesAdminDto>> DeleteBlockOutDate(Guid blockOutDateId)
        {
            try
            {
                await AddAdminHeaders();
                var response = await _httpClient.DeleteAsync($"api/Schedule/BlockOutDates/{blockOutDateId}");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<BlockOutDatesAdminDto>();
                    }

                    return await response.Content.ReadFromJsonAsync<IEnumerable<BlockOutDatesAdminDto>>();

                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotAcceptable)
                    {
                        throw new Exception("Date provided has to be a future date.");
                    }

                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception(message);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        private IEnumerable<BusinessHoursDto> ApplyLocalTimeOffset(IEnumerable<BusinessHoursDto> businessHours)
        {
            foreach (var item in businessHours)
            {
                item.OpenTime = item.OpenTime.Value.Add(item.LocalTimeOffset);
                item.CloseTime = item.CloseTime.Value.Add(item.LocalTimeOffset);
            }

            return businessHours;
        }

        private IEnumerable<AdminBusinessHoursDto> ApplyLocalTimeOffset(IEnumerable<AdminBusinessHoursDto> businessHours)
        {
            foreach (var item in businessHours)
            {
                item.OpenTime = item.OpenTime.Value.Add(item.LocalTimeOffset);
                item.CloseTime = item.CloseTime.Value.Add(item.LocalTimeOffset);
            }

            return businessHours;
        }
    }


}
