using Blazorise;
using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioSharedLibrary.Dtos.Admin;
using JricaStudioSharedLibrary.Dtos.BusinessHours;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;

namespace JricaStudioApp.Pages.Admin.Appointments
{
    public class AdminNewAppointmentPageBase : ComponentBase
    {
        [Inject]
        public IManageLocalStorageService ManageLocalStorage { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IAdminService AdminService { get; set; }
        [Inject]
        public IUserService UserService { get; set; }
        [Inject]
        public ISchedulingService SchedulingService { get; set; }
        [Inject]
        public IAppointmentItemService AppointmentItemService { get; set; }
        [Inject]
        public IAppointmentService AppointmentService { get; set; }
        [Inject]
        public IProductService ProductService { get; set; }
        [Inject]
        public IServiceService ServiceService { get; set; }

        public bool IsNewUser { get; set; }
        public Guid AdminId { get; set; }
        public Guid AdminKey { get; set; }
        public List<ServiceDto> ServicesToAdd { get; set; }
        public List<ProductDto> ProductToAdd { get; set; }
        public IEnumerable<ServiceDto> Services { get; set; }
        public IEnumerable<ProductDto> Products { get; set; }
        public AdminUserLoginDto Admin { get; set; }
        public IEnumerable<AdminUserDetailsDto> ExistingUsers { get; set; }
        public AdminUserDto User { get; set; }
        public UserAdminAddDto UserToAdd { get; set; }
        public AppointmentAdminToAddDto AppointmentToAdd { get; set; }
        public AdminAppointmentDto Appointment { get; set; }
        public string SearchFilter { get; set; } = string.Empty;
        public DatePicker<DateTime?> DatePicker { get; set; }
        public DateTime? DatePickerDate { get; set; }
        public string ValidationErrorMessage { get; set; }

        public string FieldMargins { get; set; } = "mb-4 mt-3";
        public int ScheduleRange { get; set; } = 60;

        public IEnumerable<BusinessHoursDto> BusinessHours { get; set; }
        public IEnumerable<AppointmentUnavailaleDateDto> UnavailaleDatesDtos { get; set; }
        public List<DateTime> UnavailaleDates { get; set; }
        public IEnumerable<AppointmentAvailableDto> AvailableTimes { get; set; }
        public TimeSpan Duration { get; set; }
        public Guid? ServiceToAddId { get; set; }
        public Guid? ProductToAddId { get; set; }
        public string AddServiceErrorMessage { get; set; }
        public string AddProductErrorMessage { get; set; }
        public List<string> AppointmentValidationErrorMessages { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Admin = await AdminService.AuthenticateUser();
            UserToAdd = new UserAdminAddDto();
            AppointmentToAdd = new AppointmentAdminToAddDto()
            {
                ServicesToAdd = new List<AppointmentServiceAdminAddDto>(),
                ProductsToAdd = new List<AppointmentProductAdminAddDto>(),
            };

            AppointmentValidationErrorMessages = new List<string>();
            ExistingUsers = await UserService.SearchUsers(new UserFilterDto
            {
                SearchText = SearchFilter
            });


            Products = await ProductService.GetItems();
            Services = await ServiceService.GetServices();
            BusinessHours = await SchedulingService.GetBusinessHours();
            StateHasChanged();
        }

        protected async void SearchUsers_Click()
        {
            var users = await UserService.SearchUsers(new UserFilterDto
            {
                SearchText = SearchFilter
            });

            ExistingUsers = users;

            StateHasChanged();
        }

        protected async Task SubmitAppointment_Click()
        {
            await AuthenticateUser();
            var appointmentValid = await ValidateFeilds();


            if (IsNewUser)
            {
                User = await UserService.PostNewUserAdmin(UserToAdd);
                if (User == null)
                {
                    ValidationErrorMessage = "Email is Taken";
                }
            }

            if (User != null)
            {
                AppointmentToAdd.UserId = User.Id;
                var appointment = await AppointmentService.PostAppointmentAdmin(AppointmentToAdd);
                if (appointment != null)
                {
                    NavigationManager.NavigateTo("/Admin/Appointments/Confirmed");
                }

            }

        }

        protected async Task AddProduct_Click()
        {
            AddProductErrorMessage = string.Empty;

            if (ProductToAddId == null || ProductToAddId == Guid.Empty)
            {
                AddProductErrorMessage = "Please Select a Product";
                return;
            }
            var product = Products.Single(p => p.Id == ProductToAddId);
            await AddProduct(product);
            ProductToAddId = Guid.Empty;
            StateHasChanged();
        }

        protected async Task DeleteAppointmentProduct_Click(Guid id)
        {
            var product = AppointmentToAdd.ProductsToAdd.First(p => p.ProductId == id);
            await RemoveProduct(product);
            StateHasChanged();
        }

        protected async Task RemoveProduct(AppointmentProductAdminAddDto product)
        {
            AppointmentToAdd.ProductsToAdd.Remove(product);
        }

        protected async Task AddProduct(ProductDto product)
        {
            AddProductErrorMessage = string.Empty;
            var productLine = AppointmentToAdd.ProductsToAdd.SingleOrDefault(p => p.ProductId == product.Id);
            if (productLine == null)
            {
                AppointmentToAdd.ProductsToAdd.Add(new AppointmentProductAdminAddDto()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });

            }
            else
            {
                AddProductErrorMessage = "Product Already Exists";
            }
        }

        protected async Task AddService_Click()
        {
            AddServiceErrorMessage = string.Empty;

            if (ServiceToAddId == null || ServiceToAddId == Guid.Empty)
            {
                AddServiceErrorMessage = "Please Select a Service";
                return;
            }
            var service = Services.Single(a => a.Id == ServiceToAddId);
            await AddService(service);
            ServiceToAddId = Guid.Empty;
            StateHasChanged();
        }

        protected async Task DeleteAppointmentService_Click(Guid id)
        {
            var service = AppointmentToAdd.ServicesToAdd.First(s => s.ServiceId == id);
            await RemoveService(service);
            StateHasChanged();
        }

        protected async Task RemoveService(AppointmentServiceAdminAddDto service)
        {

            AppointmentToAdd.ServicesToAdd.Remove(service);
            Duration = CalculateDuration();
        }

        protected async Task AddService(ServiceDto service)
        {
            AppointmentToAdd.ServicesToAdd.Add(new AppointmentServiceAdminAddDto()
            {
                ServiceId = service.Id,
                ServiceName = service.Name,
                Duration = service.Duration,
                Price = service.Price,
            });
            Duration = CalculateDuration();
            if (AppointmentToAdd.StartTime != null)
            {
                AppointmentToAdd.EndTime = CalculateEndTime(Duration, AppointmentToAdd.StartTime);
            }
            UnavailaleDatesDtos = await SchedulingService.GetUnavailableDates(ScheduleRange, Duration);
        }

        private DateTime? CalculateEndTime(TimeSpan duration, DateTime? startTime)
        {
            return startTime + duration;
        }

        protected async Task SetStartTime_OnClick(DateTime time)
        {
            try
            {
                AppointmentToAdd.StartTime = time;
                AppointmentToAdd.EndTime = CalculateEndTime(Duration, time);

            }
            catch (Exception)
            {
                throw;
            }


            StateHasChanged();
        }

        protected async Task SelectUser_Click(AdminUserDto user)
        {
            if (user != null)
            {
                User = user;
            }
            return;
        }

        protected async Task ClearUser_Click()
        {
            if (User != null)
            {
                User = null;
            }
            return;
        }

        protected async Task CreateNewUser_Click()
        {
            try
            {
                await AuthenticateUser();
                ValidationErrorMessage = string.Empty;
                if (UserToAdd.FirstName == string.Empty
                    || UserToAdd.LastName == string.Empty
                    || UserToAdd.FirstName == null
                    || UserToAdd.LastName == null)
                {
                    ValidationErrorMessage = "Please enter the name of the client";
                    return;
                }
                if (UserToAdd.Phone == null)
                {
                    ValidationErrorMessage = "Please Enter a Valid Australian mobile Number";
                    return;
                }
                if (!ValidatePhoneNumber(UserToAdd.Phone) || UserToAdd.Phone == string.Empty)
                {
                    ValidationErrorMessage = "Please Enter a Valid Australian mobile Number";
                    return;
                }
                if (UserToAdd.Email == null)
                {
                    ValidationErrorMessage = "Please Enter a valid Email Address";
                    return;
                }

                if (!ValidateEmail(UserToAdd.Email))
                {
                    ValidationErrorMessage = "Please Enter a valid Email Address";
                    return;
                }

                if (!ValidateDateOfBirth(UserToAdd.DOB))
                {
                    ValidationErrorMessage = "Client must be older the 18 years old.";
                }

                if (!UserToAdd.IsWaiverAcknowledged)
                {
                    ValidationErrorMessage = "Client must be provided a copy of the waiver before continuing.";
                }

                if (UserToAdd.HasAllergies)
                {
                    ValidationErrorMessage = "Client cannot have allergies to chemicals used.";
                }

                if (ValidationErrorMessage == string.Empty)
                {
                    User = new AdminUserDto()
                    {
                        FirstName = UserToAdd.FirstName,
                        LastName = UserToAdd.LastName,
                        Email = UserToAdd.Email,
                        Phone = UserToAdd.Phone,
                        HasAllergies = UserToAdd.HasAllergies,
                        WearsContanctLenses = UserToAdd.WearsContactLenses,
                        HasSensitiveSkin = UserToAdd.HasSensitiveSkin,
                        HasHadEyeProblems4Weeks = UserToAdd.HasHadEyeProblems4Weeks,
                        IsWaiverAcknowledged = UserToAdd.IsWaiverAcknowledged,
                        DOB = UserToAdd.DOB
                    };
                }


            }
            catch (Exception)
            {

                throw;
            }
        }
        protected async Task GetAvailability_OnClick(DateTime time)
        {
            AvailableTimes = await SchedulingService.GetAvailableAppointmentsTimes(time, Duration);
        }

        private TimeSpan CalculateDuration()
        {
            var duration = TimeSpan.FromMinutes(AppointmentToAdd.ServicesToAdd.Sum(s => s.Duration.TotalMinutes));
            if (duration == TimeSpan.Zero)
            {
                duration = TimeSpan.FromMinutes(15);
            }
            return duration;
        }

        protected void ChangeIsNewUserToFalse()
        {
            IsNewUser = false;
        }

        protected void ChangeIsNewUserToTrue()
        {
            IsNewUser = true;
        }

        protected async Task AuthenticateUser()
        {
            if (await ManageLocalStorage.GetLocalAdminIdGuid() == Guid.Empty || await ManageLocalStorage.GetLocalAdminKeyGuid() == Guid.Empty)
            {
                NavigationManager.NavigateTo("/");
                return;
            }

            if (await AdminService.Reverify(await ManageLocalStorage.GetLocalAdminIdGuid()) == null)
            {
                NavigationManager.NavigateTo("/Admin/Login");
                return;
            }
        }

        private bool ValidateEmail(string email)
        {
            if (!email.Contains("@"))
            {
                return false;
            }

            var splitEmail = email.Split("@");

            if (splitEmail.Length != 2)
            {
                return false;
            }

            if (splitEmail.Any(a => a.Contains("@")))
            {
                return false;
            }

            return true;
        }

        private bool ValidatePhoneNumber(string phoneNumber)
        {
            var stripedPhoneNumber = phoneNumber.Replace(" ", "");

            if (!stripedPhoneNumber.StartsWith("04") && !stripedPhoneNumber.StartsWith("+614"))
            {
                return false;
            }

            if (stripedPhoneNumber.StartsWith("04"))
            {
                stripedPhoneNumber = stripedPhoneNumber.Remove(0, 2);
            }

            if (stripedPhoneNumber.StartsWith("+614"))
            {
                stripedPhoneNumber.Remove(0, 4);
            }

            var stripedPhoneNumberLength = stripedPhoneNumber.Length;

            if (stripedPhoneNumberLength != 8)
            {
                return false;
            }

            if (!int.TryParse(stripedPhoneNumber, NumberStyles.Integer, new CultureInfo("en-AU"), out int number))
            {
                return false;
            }

            return true;
        }

        private bool ValidateDateOfBirth(DateOnly DOB)
        {
            if (DOB > DateOnly.FromDateTime(DateTime.Now.AddYears(-18)))
            {
                return false;
            }
            return true;
        }

        protected async Task<bool> ValidateFeilds()
        {
            AppointmentValidationErrorMessages = new List<string>();

            if (AppointmentToAdd.StartTime < DateTime.Now)
            {
                AppointmentValidationErrorMessages.Add("Please check the Appointment Date");
            }

            if (AppointmentToAdd.ServicesToAdd.Count() <= 0)
            {
                AppointmentValidationErrorMessages.Add("Please add a Service to the appointment to continue.");
            }

            if (UserToAdd.HasHadEyeProblems4Weeks)
            {
                AppointmentValidationErrorMessages.Add("User should wait until they are clear of any medical conditions.");
            }

            if (AppointmentToAdd.IsSampleSetComplete)
            {
                if (AppointmentToAdd.SampleSetCompleted > AppointmentToAdd.StartTime)
                {
                    AppointmentValidationErrorMessages.Add("Please check the date for sample set.");
                }
            }
            else
            {
                AppointmentToAdd.SampleSetCompleted = null;
            }

            if (AppointmentValidationErrorMessages.Count() > 0)
            {
                return false;
            }

            return true;
        }

        protected async Task Enter(KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case "Enter":
                    SearchUsers_Click();
                    break;
            }
        }

    }
}
