using JaysLashesWebApp.Models.Dtos;
using JaysLashesWebApp.Models.Dtos.Admin;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Models.Dtos;
using Models.Dtos.Admin;


namespace JricaStudioApp.Services.Contracts
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceDto>> GetServices();
        Task<ServiceDto> GetService(Guid id);
        Task<IEnumerable<ServiceDto>> GetRandomServices(int itemsRequested);
        Task<IEnumerable<AdminServiceCategoryDto>> GetServiceCategories();
        Task<IEnumerable<ServiceAdminPageDto>> GetAdminServices();
        Task<UploadResultDto> PostServicePicture(IFormFile serviceDto);
        Task<UploadResultDto> PostNewService(AdminServiceToAddDto<IBrowserFile> dto);
        Task<ServiceEditResultDto> PutService(Guid id, AdminEditServiceDto dto);
        Task<ImageUpdateResultDto> PatchServiceImage(Guid id, IBrowserFile image);
        Task<ServiceDto> DeleteService(Guid id);
        Task<IEnumerable<ServiceAdminPageDto>> SearchServices(ServiceFilterDto filter);
        Task<AdminServiceCategoryDto> PostNewServiceCategory(AddServiceCategoryDto dto);
        Task<AdminServiceCategoryDto> DeleteServiceCategory(Guid id);
        Task<ServiceDto> GetServiceShowcase();
        Task<ServiceDto> UpdateServiceShowcase(UpdateServiceShowcaseDto dto);
        Task<PreviousServiceDto> GetPreviouslyOrderedService(Guid userId);
    }
}
