using Blazored.LocalStorage;
using JricaStudioApp.Services.Contracts;
using JricaStudioApp.Models.Dtos;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace JricaStudioApp.Services
{
    public class AppointmentItemService : IAppointmentItemService
    {
        private readonly HttpClient _httpClient;
        private readonly IManageLocalStorageService _manageLocalStorageService;
        public event Action<IEnumerable<AppointmentServiceDto>> OnServiceAdded;
        public event Action<IEnumerable<AppointmentProductDto>> OnProductAdded;

        public AppointmentItemService(HttpClient httpClient, IManageLocalStorageService manageLocalStorageService)
        {
            this._httpClient = httpClient;
            _manageLocalStorageService = manageLocalStorageService;
        }


        private async Task AddAdminHeader()
        {
            var key = await _manageLocalStorageService.GetLocalAdminKeyGuid();

            if (!_httpClient.DefaultRequestHeaders.TryGetValues("AdminKey", out var values))
            {
                _httpClient.DefaultRequestHeaders.Add("AdminKey", key.ToString());
            }
        }

        private void RemoveAdminHeader()
        {
            _httpClient.DefaultRequestHeaders.Remove("Adminkey");
        }


        public void RaiseEventOnServicesChanged(IEnumerable<AppointmentServiceDto> appointmentServices)
        {
            if (OnServiceAdded != null)
            {
                OnServiceAdded.Invoke(appointmentServices);
            }
        }

        public void RaiseEventOnProductsChanged(IEnumerable<AppointmentProductDto> appointmentProducts)
        {
            // check to see if there are any subScribers
            if (OnProductAdded != null)
            {
                OnProductAdded.Invoke(appointmentProducts);
            }
        }

        public async Task<IEnumerable<AppointmentProductDto>> GetAppointmentProducts(Guid appointmentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Appointment/Product/{appointmentId}");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<AppointmentProductDto>();
                    }

                    return await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentProductDto>>();

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

        public async Task<IEnumerable<AppointmentServiceDto>> GetAppointmentServices(Guid appointmentId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Appointment/Service/{appointmentId}");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return Enumerable.Empty<AppointmentServiceDto>();
                    }

                    return await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentServiceDto>>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status:{response.StatusCode} - Message: {message}");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AppointmentProductDto> PostAppointmentProduct(AppointmentProductToAddDto productToAddDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<AppointmentProductToAddDto>("api/Appointment/Product", productToAddDto);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default(AppointmentProductDto);
                    }
                    return await response.Content.ReadFromJsonAsync<AppointmentProductDto>();

                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status:{response.StatusCode} - Message: {message}");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AppointmentServiceDto> PostAppointmentService(AppointmentServiceToAddDto serviceToAddDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<AppointmentServiceToAddDto>("api/Appointment/Service", serviceToAddDto);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return null;
                    }
                    return await response.Content.ReadFromJsonAsync<AppointmentServiceDto>();

                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status:{response.StatusCode} - Message: {message}");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<AppointmentProductDto>> DeleteAppointmentProduct(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Appointment/Product/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentProductDto>>();
                }
                return default;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<AppointmentServiceDto>> DeleteAppointmentService(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Appointment/Service/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<AppointmentServiceDto>>();
                }
                return default;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AppointmentProductDto> PatchUpdateAppoitmentProductQuantity(Guid id, AppointmentProductQuantityUpdateDto quantityUpdateDto)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(quantityUpdateDto);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

                var response = await _httpClient.PatchAsync($"api/Appointment/Product/Quantity/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AppointmentProductDto>();
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AppointmentServiceDto> PostAdminAppointmentService(AppointmentServiceToAddDto serviceToAddDto)
        {
            try
            {
                await AddAdminHeader();
                var response = await _httpClient.PostAsJsonAsync<AppointmentServiceToAddDto>("api/Appointment/Admin/Service", serviceToAddDto);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return null;
                    }
                    RemoveAdminHeader();
                    return await response.Content.ReadFromJsonAsync<AppointmentServiceDto>();

                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    RemoveAdminHeader();
                    throw new Exception($"Http status:{response.StatusCode} - Message: {message}");
                }
            }
            catch (Exception)
            {
                RemoveAdminHeader();
                throw;
            }
        }

        public async Task<AppointmentProductDto> PostAdminAppointmentProduct(AppointmentProductToAddDto productToAddDto)
        {
            try
            {
                await AddAdminHeader();
                var response = await _httpClient.PostAsJsonAsync<AppointmentProductToAddDto>("api/Appointment/Admin/Product", productToAddDto);

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default(AppointmentProductDto);
                    }
                    RemoveAdminHeader();
                    return await response.Content.ReadFromJsonAsync<AppointmentProductDto>();

                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    RemoveAdminHeader();
                    throw new Exception($"Http status:{response.StatusCode} - Message: {message}");
                }
            }
            catch (Exception)
            {
                RemoveAdminHeader();
                throw;
            }
        }
    }
}
