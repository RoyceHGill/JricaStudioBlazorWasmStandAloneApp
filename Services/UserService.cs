using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;


using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace JricaStudioApp.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IManageLocalStorageService _manageLocalStorageService;


        public event Action<UserDto> OnUserUpdated;

        public UserService( HttpClient httpClient, IManageLocalStorageService manageLocalStorageService )
        {
            _httpClient = httpClient;
            _manageLocalStorageService = manageLocalStorageService;
        }

        public async Task<IEnumerable<AdminUserDto>> AdminGetUsers( UserFilterDto filter )
        {
            try
            {
                await AddAdminHeader();
                var response = await _httpClient.PostAsJsonAsync<UserFilterDto>( $"api/User/Search/", filter );

                if ( response.IsSuccessStatusCode )
                {
                    if ( response.StatusCode == System.Net.HttpStatusCode.NoContent )
                    {
                        return default;
                    }
                    RemoveAdminHeader();
                    return await response.Content.ReadFromJsonAsync<IEnumerable<AdminUserDto>>();

                }
                else
                {

                    var message = await response.Content.ReadAsStringAsync();
                    RemoveAdminHeader();
                    throw new Exception( message );
                }
            }
            catch ( Exception e )
            {
                RemoveAdminHeader();
                throw;
            }
        }



        public async Task<UserDto> GetUser( Guid userId )
        {
            try
            {
                var response = await _httpClient.GetAsync( $"api/User/{userId}" );

                if ( response.IsSuccessStatusCode )
                {
                    if ( response.StatusCode == System.Net.HttpStatusCode.NoContent )
                    {
                        throw new ApplicationException( "User Not Found" );
                    }
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception( message );
                }
            }
            catch ( ApplicationException ae )
            {
                throw;
            }
            catch ( Exception e )
            {

                throw;
            }
        }
        public async Task<UserIndemnityDto> GetUserIndemnity( Guid id )
        {
            try
            {
                var response = await _httpClient.GetAsync( $"api/User/Indemnity/{id}" );

                if ( response.IsSuccessStatusCode )
                {
                    return await response.Content.ReadFromJsonAsync<UserIndemnityDto>();
                }
                return null;
            }
            catch ( Exception e )
            {

                throw;
            }
        }

        public async Task<UserWaiverDto> GetUserWaiver( Guid id )
        {
            try
            {
                var response = await _httpClient.GetAsync( $"api/User/Waiver/{id}" );

                if ( response.IsSuccessStatusCode )
                {
                    return await response.Content.ReadFromJsonAsync<UserWaiverDto>();
                }
                return null;
            }
            catch ( Exception e )
            {

                throw;
            }
        }

        public async Task<UserDto> PostNewUser( UserToAddDto user )
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<UserToAddDto>( "api/User", user );

                if ( response.IsSuccessStatusCode )
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }

                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception( $"Http status: {response.StatusCode} Message: {message}" );
                }
            }
            catch ( Exception e )
            {

                throw;
            }
        }

        public async Task<UserIndemnityDto> PutUserIndemityForm( Guid id, UpdateUserDto updateUserDto )
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject( updateUserDto );
                var content = new StringContent( jsonRequest, Encoding.UTF8, "application/json-patch+json" );

                var response = await _httpClient.PutAsync( $"api/User/Indemnity", content );

                if ( response.IsSuccessStatusCode )
                {
                    return await response.Content.ReadFromJsonAsync<UserIndemnityDto>();
                }
                if ( response.StatusCode == System.Net.HttpStatusCode.Conflict )
                {
                    throw new Exception( "That Email Is already Taken" );
                }

                return null;
            }
            catch ( Exception e )
            {

                throw;
            }
        }

        public void RaisedEventOnUserUpdated( UserDto user )
        {
            if ( OnUserUpdated != null )
            {
                OnUserUpdated.Invoke( user );
            }
        }

        public async Task<UserWaiverDto> PatchAcceptUserWaiver( Guid id, bool isAccepted )
        {
            if ( isAccepted )
            {
                try
                {
                    var jsonRequest = JsonConvert.SerializeObject( new UserWaiverPatchDto()
                    {
                        Id = id,
                        IsAccepted = isAccepted
                    } );
                    var content = new StringContent( jsonRequest, Encoding.UTF8, "application/json-patch+json" );

                    var response = await _httpClient.PatchAsync( "api/User/Waiver", content );

                    if ( response.IsSuccessStatusCode )
                    {
                        return await response.Content.ReadFromJsonAsync<UserWaiverDto>();
                    }
                    return null;
                }
                catch ( Exception )
                {

                    throw;
                }
            }
            return null;
        }

        public async Task<AdminUserDto> AdminPostNewUser( UserAdminAddDto user )
        {
            try
            {
                await AddAdminHeader();
                var response = await _httpClient.PostAsJsonAsync<UserAdminAddDto>( "api/User/Admin", user );
                RemoveAdminHeader();
                if ( response.IsSuccessStatusCode )
                {
                    return await response.Content.ReadFromJsonAsync<AdminUserDto>();
                }

                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception( $"Http status: {response.StatusCode} Message: {message}" );
                }
            }
            catch ( Exception e )
            {
                throw;
            }
        }

        private async Task AddAdminHeader()
        {
            RemoveAdminHeader();

            var key = await _manageLocalStorageService.GetLocalAdminKeyGuid();

            if ( !_httpClient.DefaultRequestHeaders.TryGetValues( "AdminKey", out var values ) )
            {
                _httpClient.DefaultRequestHeaders.Add( "AdminKey", key.ToString() );
            }
        }

        private void RemoveAdminHeader()
        {
            _httpClient.DefaultRequestHeaders.Remove( "Adminkey" );
        }

        public async Task<UserIndemnityDto> SoftSignIn( UserSignInDto dto )
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject( dto );
                var content = new StringContent( jsonRequest, Encoding.UTF8, "application/json-patch+json" );

                var response = await _httpClient.PostAsync( $"api/User/SignIn", content );
                if ( response.IsSuccessStatusCode )
                {
                    if ( response.StatusCode == System.Net.HttpStatusCode.NoContent )
                    {
                        throw new Exception( "User Not Found" );
                    }

                    return await response.Content.ReadFromJsonAsync<UserIndemnityDto>();
                }
                if ( response.StatusCode == System.Net.HttpStatusCode.Unauthorized )
                {
                    throw new UnauthorizedAccessException( "Email or Phone number is Taken" );
                }

                return null;
            }
            catch ( Exception )
            {

                throw;
            }
        }

        public async Task<UserDto> DisposeOfTemporaryUser( Guid id )
        {
            try
            {

                var response = await _httpClient.DeleteAsync( $"api/User/{id}" );

                if ( response.IsSuccessStatusCode )
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>();
                }
                if ( response.StatusCode == System.Net.HttpStatusCode.BadRequest )
                {
                    throw new Exception( "Not a temporary user" );
                }

                return null;
            }
            catch ( Exception e )
            {

                throw;
            }
        }

        public async Task<AdminUserDto> PostNewUserAdmin( UserAdminAddDto dto )
        {
            try
            {
                await AddAdminHeader();
                var jsonRequest = JsonConvert.SerializeObject( dto );
                var content = new StringContent( jsonRequest, Encoding.UTF8, "application/json-patch+json" );

                var response = await _httpClient.PostAsync( $"api/User/Admin", content );
                RemoveAdminHeader();

                if ( response.IsSuccessStatusCode )
                {
                    if ( response.StatusCode == System.Net.HttpStatusCode.NoContent )
                    {
                        throw new Exception( "User Not Found" );
                    }
                    return await response.Content.ReadFromJsonAsync<AdminUserDto>();
                }
                return null;
            }
            catch ( Exception )
            {
                throw;
            }
        }

        public async Task<IEnumerable<AdminUserDetailsDto>> SearchUsers( UserFilterDto filter )
        {
            try
            {
                await AddAdminHeader();
                var response = await _httpClient.PostAsJsonAsync( $"api/User/Search/", filter );
                if ( response.IsSuccessStatusCode )
                {
                    if ( response.StatusCode == System.Net.HttpStatusCode.NoContent )
                    {
                        throw new NullReferenceException( "No Users" );
                    }
                    var users = await response.Content.ReadFromJsonAsync<IEnumerable<AdminUserDetailsDto>>();
                    RemoveAdminHeader();

                    return users;
                }
                RemoveAdminHeader();
                return default;
            }
            catch ( NullReferenceException ne )
            {
                RemoveAdminHeader();
                return new List<AdminUserDetailsDto>();
            }
            catch ( Exception e )
            {
                RemoveAdminHeader();
                throw;
            }
        }

        public async Task<UserDto> DeleteUser( Guid id )
        {
            try
            {
                await AddAdminHeader();
                var resoponse = await _httpClient.DeleteAsync( $"api/User/admin/{id}" );

                if ( resoponse.IsSuccessStatusCode )
                {
                    if ( resoponse.StatusCode == System.Net.HttpStatusCode.NotFound )
                    {
                        throw new NullReferenceException( "No user found." );
                    }
                    var user = await resoponse.Content.ReadFromJsonAsync<UserDto>();
                    RemoveAdminHeader();
                    return user;
                }
                RemoveAdminHeader();
                return default;
            }
            catch ( Exception e )
            {
                RemoveAdminHeader();
                throw;
            }
        }

        public async Task<AdminUserDto> AdminGetUser( Guid id )
        {
            try
            {
                await AddAdminHeader();
                var response = await _httpClient.GetAsync( $"api/user/admin/{id}" );
                RemoveAdminHeader();
                if ( response.IsSuccessStatusCode )
                {
                    if ( response.StatusCode == System.Net.HttpStatusCode.NoContent )
                    {
                        throw new ApplicationException( "User Not Found" );
                    }
                    return await response.Content.ReadFromJsonAsync<AdminUserDto>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception( message );
                }
            }
            catch ( ApplicationException ae )
            {
                throw;
            }
            catch ( Exception e )
            {

                throw;
            }
        }

        public async Task<AdminUserDto> UpdateUser( Guid id, UpdateUserDto dto )
        {
            try
            {
                await AddAdminHeader();
                var jsonRequest = JsonConvert.SerializeObject( dto );
                var content = new StringContent( jsonRequest, Encoding.UTF8, "application/json-patch+json" );

                var response = await _httpClient.PutAsync( $"api/User/update/{id}", content );

                if ( response.IsSuccessStatusCode )
                {
                    return await response.Content.ReadFromJsonAsync<AdminUserDto>();
                }
                if ( response.StatusCode == System.Net.HttpStatusCode.Conflict )
                {
                    throw new Exception( "That Email Is already Taken" );
                }

                return null;
            }
            catch ( Exception e )
            {

                throw;
            }
        }

        public async Task<ContactFormResult> PostUserContactForm( ContactFormSubmitDto dto )
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<ContactFormSubmitDto>( "api/user/contact", dto );

                if ( response.IsSuccessStatusCode )
                {
                    return await response.Content.ReadFromJsonAsync<ContactFormResult>();
                }

                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception( $"Http status: {response.StatusCode} Message: {message}" );
                }
            }
            catch ( Exception e )
            {

                throw;
            }
        }



        #region Policy

        public async Task<IEnumerable<PolicyDto>?> GetPoliciies()
        {
            try
            {
                var response = await _httpClient.GetAsync( $"api/policy" );

                if ( response.IsSuccessStatusCode )
                {
                    if ( response.StatusCode == System.Net.HttpStatusCode.NoContent )
                    {
                        return default;
                    }
                    return await response.Content.ReadFromJsonAsync<IEnumerable<PolicyDto>>();

                }
                return null;
            }
            catch ( Exception e )
            {

                throw;
            }
        }

        public async Task<IEnumerable<PolicyAdminDto>> GetAdministratorPoliciies()
        {
            try
            {
                await AddAdminHeader();
                var response = await _httpClient.GetAsync( $"api/policy/admin" );
                RemoveAdminHeader();

                if ( response.IsSuccessStatusCode )
                {
                    if ( response.StatusCode == System.Net.HttpStatusCode.NoContent )
                    {
                        return Enumerable.Empty<PolicyAdminDto>();
                    }
                    if ( response.Content != null )
                    {
                        return await response.Content.ReadFromJsonAsync<IEnumerable<PolicyAdminDto>>()
                            ?? throw new Exception( await response.Content.ReadAsStringAsync() );
                    }
                }
                return Enumerable.Empty<PolicyAdminDto>();
            }
            catch ( Exception e )
            {

                throw;
            }
        }

        public async Task<PolicyAdminDto?> DeletePolicy( Guid id )
        {
            try
            {
                await AddAdminHeader();
                var resoponse = await _httpClient.DeleteAsync( $"api/policy/{id}" );
                RemoveAdminHeader();

                if ( resoponse.IsSuccessStatusCode )
                {
                    if ( resoponse.StatusCode == System.Net.HttpStatusCode.NotFound )
                    {
                        throw new NullReferenceException( "No policy found." );
                    }
                    var policy = await resoponse.Content.ReadFromJsonAsync<PolicyAdminDto>();
                    return policy;
                }
                return default;
            }
            catch ( Exception e )
            {
                throw;
            }
        }

        public async Task<PolicyAdminDto?> PostPolicy( AddPolicyDto addPolicy )
        {
            try
            {
                await AddAdminHeader();
                var jsonRequest = JsonConvert.SerializeObject( addPolicy );
                var content = new StringContent( jsonRequest, Encoding.UTF8, "application/json-patch+json" );

                var response = await _httpClient.PostAsync( $"api/policy", content );
                RemoveAdminHeader();

                if ( response.IsSuccessStatusCode )
                {
                    return await response.Content.ReadFromJsonAsync<PolicyAdminDto>();
                }
                if ( response.StatusCode == System.Net.HttpStatusCode.InternalServerError )
                {
                    throw new Exception( $"There was an error creating resource: {response.Content.ReadAsStream()}" );
                }
                return null;
            }
            catch ( Exception )
            {
                throw;
            }
        }

        #endregion
    }
}
