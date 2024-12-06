using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using JricaStudioApp.Services.Contracts;
using JricaStudioApp.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace JricaStudioApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddBlazoredLocalStorageAsSingleton();
            builder.Services.AddBlazorise(options =>
            {
                options.Immediate = true;
            })
                .AddBootstrap5Providers()
                .AddFontAwesomeIcons();

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<IAppointmentItemService, AppointmentItemService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<ISchedulingService, SchedulingService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IManageLocalStorageService, ManageLocalStorageService>();
            builder.Services.AddScoped<IAdminService, AdminService>();


#if DEBUG
            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7063/") });

#else
            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration.GetValue<string>("API_URI")) });

#endif

            await builder.Build().RunAsync();
        }
    }
}
