using JricaStudioApp.Services.Contracts;
using Microsoft.AspNetCore.Components;
using JricaStudioSharedLibrary.Dtos;
using JricaStudioApp.Services;
using JricaStudioApp.Services.Contracts;

namespace JricaStudioApp.Shared
{
    public class StaticTopNavBarBase : ComponentBase, IDisposable
    {
        [Inject]
        public IManageLocalStorageService LocalStorageService { get; set; }

        [Inject]
        public IUserService UserService { get; set; }
        [Inject]
        public IAppointmentService AppointmentService { get; set; }

        public Guid UserId { get; set; }
        public UserDto User { get; set; }
        public Guid AppointmentId { get; set; }

        private void UserUpdated(UserDto userDto)
        {
            User = userDto;
            StateHasChanged();
        }

        private async void AppointmentUserChanged(AppointmentDto appointmentDto)
        {
            UserId = appointmentDto.UserId;
            AppointmentId = appointmentDto.Id;
            User = await UserService.GetUser(UserId);
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            UserService.OnUserUpdated += UserUpdated;
            AppointmentService.OnUserChange += AppointmentUserChanged;

            try
            {
                UserId = await LocalStorageService.GetLocalUserIdGuid();

                if (UserId == Guid.Empty)
                {
                    User = await UserService.PostNewUser(new UserToAddDto()
                    {
                        FirstName = "Guest",
                        LastName = Guid.NewGuid().ToString().Split('-').First()
                    });

                    UserId = await LocalStorageService.SetLocalUserIdGuid(User.Id);
                    UserService.RaisedEventOnUserUpdated(User);
                }
                else
                {
                    try
                    {
                        User = await UserService.GetUser(UserId);
                    }
                    catch (ApplicationException e)
                    {
                        User = await UserService.PostNewUser(new UserToAddDto()
                        {
                            FirstName = "Guest",
                            LastName = Guid.NewGuid().ToString().Split('-').First()
                        });

                        UserId = await LocalStorageService.SetLocalUserIdGuid(User.Id);
                        UserService.RaisedEventOnUserUpdated(User);


                    }
                    catch (Exception e)
                    {
                        throw;
                    }

                }


                AppointmentId = await LocalStorageService.GetLocalAppointmentIdGuid();

                if (AppointmentId == Guid.Empty)
                {
                    var appointment = await AppointmentService.PostAppointment(new AppointmentToAddDto
                    {
                        Id = AppointmentId,
                        UserId = UserId
                    });

                    AppointmentId = await LocalStorageService.SetLocalAppointmentIdGuid(appointment.Id);
                    AppointmentService.RaiseEventOnIdChanged(AppointmentId);


                }
                else
                {
                    try
                    {
                        var appointment = await AppointmentService.GetAppointment(AppointmentId);
                        if (appointment == null)
                        {
                            appointment = await AppointmentService.PostAppointment(new AppointmentToAddDto
                            {
                                UserId = UserId
                            });
                            AppointmentId = await LocalStorageService.SetLocalAppointmentIdGuid(appointment.Id);
                            AppointmentService.RaiseEventOnIdChanged(AppointmentId);
                            return;
                        }
                        AppointmentId = appointment.Id;

                    }
                    catch (ApplicationException ae)
                    {
                        var appointment = await AppointmentService.PostAppointment(new AppointmentToAddDto
                        {
                            UserId = UserId
                        });
                        AppointmentId = await LocalStorageService.SetLocalAppointmentIdGuid(appointment.Id);
                        AppointmentService.RaiseEventOnIdChanged(AppointmentId);
                    }
                    catch (Exception e)
                    {
                        throw;
                    }

                }
            }
            catch (Exception e)
            {
                throw;
            }


        }

        public void Dispose()
        {
            UserService.OnUserUpdated -= UserUpdated;
        }
    }
}
