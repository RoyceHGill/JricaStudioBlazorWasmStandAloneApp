using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;



namespace JricaStudioApp.Services.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<AdminUserDetailsDto>> SearchUsers( UserFilterDto filter );
        Task<AdminUserDto> PostNewUserAdmin( UserAdminAddDto dto );
        Task<UserIndemnityDto> SoftSignIn( UserSignInDto dto );
        Task<UserDto> DisposeOfTemporaryUser( Guid Id );
        Task<UserDto> GetUser( Guid userId );
        Task<IEnumerable<AdminUserDto>> AdminGetUsers( UserFilterDto filter );

        Task<UserIndemnityDto> GetUserIndemnity( Guid id );
        Task<UserWaiverDto> GetUserWaiver( Guid id );

        Task<UserDto> PostNewUser( UserToAddDto user );
        Task<AdminUserDto> AdminPostNewUser( UserAdminAddDto user );
        Task<UserIndemnityDto> PutUserIndemityForm( Guid id, UpdateUserDto updateUserDto );
        Task<UserWaiverDto> PatchAcceptUserWaiver( Guid id, bool isAccepted );
        Task<UserDto> DeleteUser( Guid id );
        Task<AdminUserDto> AdminGetUser( Guid id );
        Task<AdminUserDto> UpdateUser( Guid id, UpdateUserDto dto );
        Task<ContactFormResult> PostUserContactForm( ContactFormSubmitDto dto );

        #region Policy

        Task<IEnumerable<PolicyDto>?> GetPoliciies();
        Task<IEnumerable<PolicyAdminDto>> GetAdministratorPoliciies();
        Task<PolicyAdminDto?> DeletePolicy( Guid id );
        Task<PolicyAdminDto?> PostPolicy( AddPolicyDto addPolicy );

        #endregion





        event Action<UserDto> OnUserUpdated;

        void RaisedEventOnUserUpdated( UserDto user );
    }
}
