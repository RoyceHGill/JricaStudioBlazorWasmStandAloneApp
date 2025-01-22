using JricaStudioSharedLibrary.Dtos;
using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Pages.Services
{
    public class ServicesCardsBase : ComponentBase
    {
        [Parameter]
        public IEnumerable<ServiceDto> Services { get; set; }
    }
}
