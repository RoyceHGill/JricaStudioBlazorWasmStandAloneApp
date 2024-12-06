using Microsoft.AspNetCore.Components;

namespace JricaStudioApp.Shared
{
    public class PageFooterBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }


        protected void GoToHome_Click()
        {
            NavigationManager.NavigateTo(NavigationManager.BaseUri);
        }

        protected void GoToServices_Click()
        {
            NavigationManager.NavigateTo("/services");
        }

        protected void GoToProducts_Click()
        {
            NavigationManager.NavigateTo("/products");
        }

        protected void GoToPolices_Click()
        {
            NavigationManager.NavigateTo("/polices");
        }

        protected void GoToAboutUs_Click()
        {
            NavigationManager.NavigateTo("/aboutus");
        }
    }
}
