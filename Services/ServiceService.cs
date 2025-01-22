using Blazored.LocalStorage;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using JricaStudioSharedLibrary.Constants;


using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;

namespace JricaStudioApp.Services
{
    public class ServiceService : IServiceService
    {
        private readonly HttpClient _httpClient;
        private readonly IManageLocalStorageService _localStorageService;

        public ServiceService(HttpClient httpClient, IManageLocalStorageService localStorageService)
        {
            this._httpClient = httpClient;
            this._localStorageService = localStorageService;
        }



        private async Task AddAdminHeaders()
        {
            var key = await _localStorageService.GetLocalAdminKeyGuid();

            if (!_httpClient.DefaultRequestHeaders.TryGetValues("AdminKey", out var values))
            {
                _httpClient.DefaultRequestHeaders.Add("AdminKey", key.ToString());
            }
        }

        private void RemoveAdminHeaders()
        {
            _httpClient.DefaultRequestHeaders.Remove("Adminkey");
        }


        public async Task<IEnumerable<ServiceDto>> GetServices()
        {
            try
            {
                var services = await _httpClient.GetFromJsonAsync<IEnumerable<ServiceDto>>("api/Service/All");

                return services;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<ServiceDto>> GetRandomServices(int itemsRequested)
        {
            try
            {
                var services = await _httpClient.GetFromJsonAsync<IEnumerable<ServiceDto>>($"api/Service/GetRandom/{itemsRequested}");
                return services;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public async Task<ServiceDto> GetService(Guid Id)
        {
            try
            {
                var service = await _httpClient.GetFromJsonAsync<ServiceDto>($"api/Service/{Id}");
                return service;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public async Task<IEnumerable<AdminServiceCategoryDto>> GetServiceCategories()
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Service/Categories");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        throw new NullReferenceException("No Appointments");
                    }
                    var aerviceCategories = await response.Content.ReadFromJsonAsync<IEnumerable<AdminServiceCategoryDto>>();

                    return aerviceCategories;
                }
                return default;
            }
            catch (NullReferenceException ne)
            {
                return null;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ServiceAdminPageDto>> GetAdminServices()
        {
            try
            {
                await AddAdminHeaders();
                var response = await _httpClient.GetAsync($"api/Service/Admin");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        throw new NullReferenceException("No Appointments");
                    }
                    var services = await response.Content.ReadFromJsonAsync<IEnumerable<ServiceAdminPageDto>>();
                    RemoveAdminHeaders();
                    return services;
                }
                RemoveAdminHeaders();
                return default;
            }
            catch (NullReferenceException ne)
            {
                RemoveAdminHeaders();
                return null;
            }
            catch (Exception e)
            {
                RemoveAdminHeaders();
                throw;
            }
        }

        public async Task<UploadResultDto> PostServicePicture(IFormFile file)
        {
            try
            {
                await AddAdminHeaders();
                var response = await _httpClient.PostAsJsonAsync<IFormFile>("api/Service/UploadServiceImage", file);

                if (response.IsSuccessStatusCode)
                {
                    RemoveAdminHeaders();
                    return await response.Content.ReadFromJsonAsync<UploadResultDto>();
                }

                else
                {
                    RemoveAdminHeaders();
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status: {response.StatusCode}Message: {message}");
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<UploadResultDto> PostNewService(AdminServiceToAddDto<IBrowserFile> dto)
        {
            try
            {
                await AddAdminHeaders();

                using var content = new MultipartFormDataContent();

                var fileContent = new StreamContent(dto.ImageFile.OpenReadStream(FileResources.maxuploadSize));

                fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType);

                content.Add(new StringContent(dto.Name), nameof(dto.Name));
                content.Add(new StringContent(dto.Description), nameof(dto.Description));
                content.Add(new StringContent(dto.Duration.ToString()), nameof(dto.Duration));
                content.Add(new StringContent(dto.Price.ToString()), nameof(dto.Price));
                content.Add(new StringContent(dto.ServiceCategoryId.ToString()), nameof(dto.ServiceCategoryId));

                content.Add(fileContent, nameof(dto.ImageFile), dto.ImageFile.Name);

                var response = await _httpClient.PostAsync("api/Service", content);

                if (response.IsSuccessStatusCode)
                {
                    RemoveAdminHeaders();
                    return await response.Content.ReadFromJsonAsync<UploadResultDto>();
                }

                else
                {
                    RemoveAdminHeaders();
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status: {response.StatusCode}Message: {message}");
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<ServiceEditResultDto> PutService(Guid id, AdminEditServiceDto dto)
        {
            await AddAdminHeaders();

            var jsonRequest = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

            var response = await _httpClient.PutAsync($"api/Service/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                RemoveAdminHeaders();
                return await response.Content.ReadFromJsonAsync<ServiceEditResultDto>();
            }

            else
            {
                RemoveAdminHeaders();
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"Http status: {response.StatusCode}Message: {message}");
            }
        }

        public async Task<ImageUpdateResultDto> PatchServiceImage(Guid id, IBrowserFile image)
        {

            await AddAdminHeaders();

            using var content = new MultipartFormDataContent();


            var fileContent = new StreamContent(image.OpenReadStream(FileResources.maxuploadSize));

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);

            content.Add(new StringContent(id.ToString()), "Id");

            content.Add(fileContent, "ImageFile", image.Name);

            var response = await _httpClient.PatchAsync("api/Service", content);

            RemoveAdminHeaders();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ImageUpdateResultDto>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"Http status: {response.StatusCode}Message: {message}");
            }

        }

        public async Task<ServiceDto> DeleteService(Guid id)
        {
            await AddAdminHeaders();

            var response = await _httpClient.DeleteAsync($"api/Service/{id}");

            RemoveAdminHeaders();
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ServiceDto>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"Http status: {response.StatusCode}Message: {message}");
            }
        }

        public async Task<IEnumerable<ServiceAdminPageDto>> SearchServices(ServiceFilterDto filter)
        {
            try
            {
                await AddAdminHeaders();
                var response = await _httpClient.PostAsJsonAsync($"api/Service/Search", filter);
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        throw new NullReferenceException("No Services");
                    }
                    var services = await response.Content.ReadFromJsonAsync<IEnumerable<ServiceAdminPageDto>>();
                    RemoveAdminHeaders();
                    return services;
                }
                RemoveAdminHeaders();
                return default;
            }
            catch (NullReferenceException ne)
            {
                RemoveAdminHeaders();
                return new List<ServiceAdminPageDto>();
            }
            catch (Exception e)
            {
                RemoveAdminHeaders();
                throw;
            }
        }

        public async Task<AdminServiceCategoryDto> PostNewServiceCategory(AddServiceCategoryDto dto)
        {
            try
            {
                await AddAdminHeaders();
                var response = await _httpClient.PostAsJsonAsync<AddServiceCategoryDto>($"api/Service/Category", dto);
                if (response.IsSuccessStatusCode)
                {

                    var category = await response.Content.ReadFromJsonAsync<AdminServiceCategoryDto>();
                    RemoveAdminHeaders();
                    return category;
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    throw new ArgumentException("Category already exists.");
                }

                RemoveAdminHeaders();
                return default;
            }
            catch (ArgumentException ne)
            {
                RemoveAdminHeaders();
                return new AdminServiceCategoryDto();
            }
            catch (Exception e)
            {
                RemoveAdminHeaders();
                throw;
            }
        }

        public async Task<AdminServiceCategoryDto> DeleteServiceCategory(Guid id)
        {
            try
            {
                await AddAdminHeaders();
                var response = await _httpClient.DeleteAsync($"api/Service/Category/{id}");
                RemoveAdminHeaders();
                if (response.IsSuccessStatusCode)
                {
                    var category = await response.Content.ReadFromJsonAsync<AdminServiceCategoryDto>();
                    return category;
                }
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new ArgumentException("Category not found.");
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    throw new ArgumentException("Category not found.");
                }

                return default;
            }
            catch (ArgumentException ne)
            {
                return new AdminServiceCategoryDto();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ServiceDto> GetServiceShowcase()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/service/serviceShowcase");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default;
                    }
                    var service = await response.Content.ReadFromJsonAsync<ServiceDto>();

                    return service;
                }

                return default;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<ServiceDto> UpdateServiceShowcase(UpdateServiceShowcaseDto dto)
        {
            try
            {
                await AddAdminHeaders();

                var jsonRequest = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

                var response = await _httpClient.PutAsync($"api/service/serviceShowcase", content);
                RemoveAdminHeaders();

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ServiceDto>();
                }

                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status: {response.StatusCode}Message: {message}");
                }
            }
            catch (Exception e)
            {
                RemoveAdminHeaders();
                throw;
            }

        }

        public async Task<PreviousServiceDto> GetPreviouslyOrderedService(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/service/previouslyOrdered/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return default;
                    }
                    var dto = await response.Content.ReadFromJsonAsync<PreviousServiceDto>();
                    if (dto != null)
                    {
                        return dto;
                    }

                    return default;
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
    }
}
