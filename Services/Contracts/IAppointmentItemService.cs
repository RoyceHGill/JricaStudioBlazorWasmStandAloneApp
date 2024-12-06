using JricaStudioApp.Models.Dtos;

namespace JricaStudioApp.Services.Contracts
{
    public interface IAppointmentItemService
    {
        Task<IEnumerable<AppointmentServiceDto>> GetAppointmentServices(Guid appointmentId);

        Task<IEnumerable<AppointmentProductDto>> GetAppointmentProducts(Guid appointmentId);

        Task<AppointmentServiceDto> PostAppointmentService(AppointmentServiceToAddDto serviceToAddDto);
        Task<AppointmentServiceDto> PostAdminAppointmentService(AppointmentServiceToAddDto serviceToAddDto);

        Task<AppointmentProductDto> PostAppointmentProduct(AppointmentProductToAddDto productToAddDto);
        Task<AppointmentProductDto> PostAdminAppointmentProduct(AppointmentProductToAddDto productToAddDto);


        event Action<IEnumerable<AppointmentServiceDto>> OnServiceAdded;

        void RaiseEventOnServicesChanged(IEnumerable<AppointmentServiceDto> appointmentServices);

        event Action<IEnumerable<AppointmentProductDto>> OnProductAdded;

        void RaiseEventOnProductsChanged(IEnumerable<AppointmentProductDto> appointmentProducts);

        Task<IEnumerable<AppointmentProductDto>> DeleteAppointmentProduct(Guid id);
        Task<IEnumerable<AppointmentServiceDto>> DeleteAppointmentService(Guid id);

        Task<AppointmentProductDto> PatchUpdateAppoitmentProductQuantity(Guid id, AppointmentProductQuantityUpdateDto quantityUpdateDto);

    }
}
