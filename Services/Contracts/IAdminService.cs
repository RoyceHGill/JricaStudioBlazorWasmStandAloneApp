
using JricaStudioApp.Shared.Models;
using JricaStudioSharedLibrary.Dtos.Admin;

namespace JricaStudioApp.Services.Contracts
{
    public interface IAdminService
    {
        Task<AdminUserLoginDto> Login(AdminLoginRequestDto dto);

        Task<AdminUserLoginDto> Reverify(Guid AdminId);

        Task<AdminUserLoginDto> AuthenticateUser();

        Task<AdminUserLoginDto> UpdateAdminPassword(Guid id, UserCredentialsUpdateDto dto);

        Task<PasswordRequestResult> InitiatePasswordReset(PasswordResetRequestDto dto);

        Task<PasswordRequestResult> SubmitPasswordRequest(Guid key, ResetPasswordDto dto);
    }
}
