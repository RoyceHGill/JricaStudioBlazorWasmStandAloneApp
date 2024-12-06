using JricaStudioApp.Services.Contracts;
using JaysLashesWebApp.Models.Dtos.Admin;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using JricaStudioApp.Shared.Models;

namespace JricaStudioApp.Services
{
    public class AdminService : IAdminService
    {
        private readonly HttpClient _httpClient;
        private readonly IManageLocalStorageService _manageLocalStorageService;
        private readonly NavigationManager _navigationManager;

        public AdminService(HttpClient httpClient, IManageLocalStorageService manageLocalStorageService, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _manageLocalStorageService = manageLocalStorageService;
            _navigationManager = navigationManager;
        }

        public async Task<AdminUserLoginDto> Login(AdminLoginRequestDto loginDto)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(loginDto);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

                var response = await _httpClient.PostAsync($"api/Admin/Login", content);
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        throw new Exception("User Not Found");
                    }
                    return await response.Content.ReadFromJsonAsync<AdminUserLoginDto>();
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AdminUserLoginDto> Reverify(Guid adminId)
        {
            try
            {
                await AddAdminHeader();
                var response = await _httpClient.GetAsync($"api/Admin/Re-verification/{adminId}");
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        throw new Exception("User Not Found");
                    }
                    return await response.Content.ReadFromJsonAsync<AdminUserLoginDto>();
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task AddAdminHeader()
        {
            RemoveAdminHeader();
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

        public async Task<AdminUserLoginDto> AuthenticateUser()
        {
            if (await _manageLocalStorageService.GetLocalAdminIdGuid() == Guid.Empty || await _manageLocalStorageService.GetLocalAdminKeyGuid() == Guid.Empty)
            {
                if (!_navigationManager.Uri.ToLower().Contains("/admin/login"))
                {
                    _navigationManager.NavigateTo("/");

                }
                return null;
            }

            var admin = await Reverify(await _manageLocalStorageService.GetLocalAdminIdGuid());

            if (admin == null)
            {
                await _manageLocalStorageService.RemoveLocalAdminIdGuid();
                await _manageLocalStorageService.RemoveLocalAdminKeyGuid();
                if (!_navigationManager.Uri.ToLower().Contains("/admin/login"))
                {
                    _navigationManager.NavigateTo("/admin/login");
                }
                return null;
            }
            return admin;
        }


        public async Task<AdminUserLoginDto> UpdateAdminPassword(Guid id, UserCredentialsUpdateDto dto)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");

                var response = await _httpClient.PatchAsync($"api/admin/update/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AdminUserLoginDto>();
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return default;
                }
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errors = await response.Content.ReadAsStringAsync();
                }

                return null;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<PasswordRequestResult> InitiatePasswordReset(PasswordResetRequestDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/admin/passwordresetrequest", dto);
            if (response.IsSuccessStatusCode)
            {
                return new PasswordRequestResult()
                {
                    IsSuccessful = true,
                    StatusCode = response.StatusCode
                };
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new PasswordRequestResult
                {
                    IsSuccessful = false,
                    StatusCode = response.StatusCode,
                    Message = await response.Content.ReadAsStringAsync()
                };
            }
            return new PasswordRequestResult()
            {
                IsSuccessful = false,
                StatusCode = response.StatusCode
            };
        }

        public async Task<PasswordRequestResult> SubmitPasswordRequest(Guid key, ResetPasswordDto dto)
        {
            var jsonRequest = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");
            RemoveAdminHeader();

            if (!_httpClient.DefaultRequestHeaders.TryGetValues("RequestKey", out var values))
            {
                _httpClient.DefaultRequestHeaders.Add("RequestKey", key.ToString());
            }

            var response = await _httpClient.PutAsync($"api/admin/passwordresetrequest", content);

            _httpClient.DefaultRequestHeaders.Remove("RequestKey");

            if (response.IsSuccessStatusCode)
            {
                return new PasswordRequestResult
                {
                    IsSuccessful = true,
                    StatusCode = response.StatusCode
                };
            }
            if (!response.IsSuccessStatusCode)
            {
                return new PasswordRequestResult
                {
                    IsSuccessful = false,
                    StatusCode = response.StatusCode
                };
            }

            return null;
        }
    }
}
