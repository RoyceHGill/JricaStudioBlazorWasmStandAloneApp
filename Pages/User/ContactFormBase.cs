
using JricaStudioApp.Services.Contracts;
using JricaStudioSharedLibrary.Dtos;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace JricaStudioApp.Pages.User
{
    public class ContactFormBase : ComponentBase
    {
        [Inject]
        public IUserService UserService { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public ContactFormSubmitDto Contact { get; set; }

        public string MailingError { get; set; }
        public string ConfirmationMessage { get; set; }

        public IEnumerable<string> ValidationErrors { get; set; }

        protected async Task ClearFeilds_Click()
        {
            Name = "";
            Email = "";
            Subject = "";
            Message = "";

            ValidationErrors = new List<string>();
        }

        protected async Task SendContactForm_Click()
        {
            Contact = new ContactFormSubmitDto()
            {
                Name = Name,
                Email = Email,
                Subject = Subject,
                Message = Message,
            };

            ValidationErrors = ValidateContactForm(Contact);

            if (ValidationErrors.Any())
            {
                return;
            }

            try
            {
                var result = await UserService.PostUserContactForm(Contact);
                ConfirmationMessage = "Thank you for your message please check your email inbox for a confirmation email.";

            }
            catch (Exception e)
            {
                MailingError = e.Message;
            }
        }

        private IEnumerable<string> ValidateContactForm(ContactFormSubmitDto contact)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(contact.Name))
            {
                errors.Add("Name can not be empty.");
            }
            else
            {
                if (contact.Name.Length > 50)
                {
                    errors.Add("Name must be less then 50 characters.");
                }
            }

            if (string.IsNullOrWhiteSpace(contact.Email))
            {
                errors.Add("Email can not be empty.");
            }
            else
            {
                if (contact.Email.Length > 70)
                {
                    errors.Add("Email must be less than 70 characters.");
                }

                if (contact.Email.Split("@").Length != 2)
                {
                    errors.Add("Email must be a valid email address");
                }
            }

            if (string.IsNullOrWhiteSpace(contact.Message))
            {
                errors.Add("Message can not be empty.");
            }
            else
            {
                if (contact.Message.Length > 500)
                {
                    errors.Add("Message must be less than 500 characters.");
                }
            }

            if (string.IsNullOrWhiteSpace(contact.Subject))
            {
                errors.Add("Subject can not be empty.");
            }
            else
            {
                if (contact.Subject.Length > 50)
                {
                    errors.Add("Subject must be less then 50 characters.");
                }
            }
            return errors;
        }
    }
}
