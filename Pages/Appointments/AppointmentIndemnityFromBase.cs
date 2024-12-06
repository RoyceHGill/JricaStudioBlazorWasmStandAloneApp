using JricaStudioApp.Services.Contracts;
using JaysLashesWebApp.Models.Dtos;
using JaysLashesWebApp.Models.enums;
using Microsoft.AspNetCore.Components;
using Models.Dtos;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace JricaStudioApp.Pages.Appointments
{
    public class AppointmentIndemnityFromBase : ComponentBase
    {
        [Parameter]
        public Guid Id { get; set; }
        [Inject]
        public IUserService UserService { get; set; }
        [Inject]
        public IAppointmentService AppointmentService { get; set; }
        [Inject]
        public IManageLocalStorageService ManageLocalStorageService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        public Guid UserId { get; set; }
        public UserIndemnityDto User { get; set; }
        public AppointmentIndemnityDto Appointment { get; set; }
        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            UserId = await ManageLocalStorageService.GetLocalUserIdGuid();
            Appointment = await AppointmentService.GetAppointmentIndemnity(Id);
            User = await UserService.
                GetUserIndemnity(UserId);
        }

        protected async Task SubmitIndemityForm_Click()
        {
            try
            {
                // if a user has similar information to an existing user then we pull that users data from the database
                var userSignin = await UserService.SoftSignIn(new UserSignInDto()
                {
                    Id = User.Id,
                    FirstName = User.FirstName,
                    LastName = User.LastName,
                    Email = User.Email,
                    DOB = User.DOB,
                    Phone = User.Phone,
                });

                if (userSignin != null)
                {
                    // if the data pulled from the server has an if that is not equal to the existing ID then the Id is added to the User Id property
                    if (userSignin.Id != UserId)
                    {
                        var uptoDateAppointment = await AppointmentService.PatchAppointmentUserId(Id, new UpdateAppointmentUserDto()
                        {
                            Id = userSignin.Id,
                        });


                        if (uptoDateAppointment != null)
                        {
                            AppointmentService.RaiseEventOnUserChange(uptoDateAppointment);

                            await UserService.DisposeOfTemporaryUser(UserId);

                            UserId = await ManageLocalStorageService.SetLocalUserIdGuid(userSignin.Id);

                            UserService.RaisedEventOnUserUpdated(new UserDto()
                            {
                                FirstName = userSignin.FirstName,
                                LastName = userSignin.LastName,
                                Id = userSignin.Id,
                            });
                        }
                    }
                }

                // We continue the process as per normal, this is where we determine if the entered data has any conflicts with existing users. example Email conflicts are not allowed 
                var user = await UserService.PutUserIndemityForm(UserId, new UpdateUserDto()
                {
                    Id = UserId,
                    FirstName = User.FirstName,
                    LastName = User.LastName,
                    Email = User.Email,
                    Phone = User.Phone,
                    DOB = User.DOB,
                    HasAllergies = User.HasAllergies,
                    HasHadEyeProblems4Weeks = User.HasHadEyeProblems4Weeks,
                    HasSensitiveSkin = User.HasSensitiveSkin,
                    WearsContanctLenses = User.WearsContanctLenses
                });

                var appointment = await AppointmentService.PatchAppointmentIndemityForm(Id, new UpdateAppointmentIndemnityDto()
                {
                    HasHadEyelashExtentions = Appointment.HasHadEyelashExtentions,
                    SampleSetCompleted = Appointment.SampleSetCompleted,
                    IsSampleSetComplete = Appointment.IsSampleSetComplete,
                });

                if (user == null || appointment == null)
                {
                    ErrorMessage = "Sorry an error has occurred";
                }
                else if (User.HasAllergies || User.HasHadEyeProblems4Weeks || User.HasHadEyeProblems4Weeks)
                {
                    NavigationManager.NavigateTo("/indemnityForm/Rejected");
                }
                else
                {
                    User = user;
                    UserId = await ManageLocalStorageService.SetLocalUserIdGuid(User.Id);
                    Appointment = appointment;

                    if (appointment.Status == AppointmentStatus.NotFinalized)
                    {
                        await AppointmentService.PatchAppointmentStatus(Id, new UpdateAppointmentStatusDto()
                        {
                            Status = AppointmentStatus.IndemnityFormSubmited
                        });

                        Appointment.Status = AppointmentStatus.IndemnityFormSubmited;
                        AppointmentService.RaiseEventOnStatusChanged(Appointment.Status);
                    }
                    switch (Appointment.Status)
                    {
                        case AppointmentStatus.NotFinalized:
                            NavigationManager.NavigateTo($"/IndemnityForm/{Id}");
                            break;
                        case AppointmentStatus.IndemnityFormSubmited:
                            NavigationManager.NavigateTo($"/Wavier/{await ManageLocalStorageService.GetLocalUserIdGuid()}");
                            break;
                        case AppointmentStatus.WaiverAccepted:
                            NavigationManager.NavigateTo($"/Appointment/Schedule/{Id}");
                            break;
                        case AppointmentStatus.BookingDateSubmited:
                            NavigationManager.NavigateTo($"/Finalize/{Id}");
                            break;
                        default:
                            if (Appointment.Status > AppointmentStatus.BookingDateSubmited)
                            {
                                NavigationManager.NavigateTo($"/Finalize/{Id}");
                            }
                            break;
                    }
                }

                UserService.RaisedEventOnUserUpdated(new UserDto
                {
                    FirstName = User.FirstName,
                    LastName = User.LastName,
                    Id = UserId
                });

            }

            catch (Exception e)
            {
                ErrorMessage = e.Message;

            }


        }
    }
}
