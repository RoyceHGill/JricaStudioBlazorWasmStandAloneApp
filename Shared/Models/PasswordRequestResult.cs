using System.Net;

namespace JricaStudioApp.Shared.Models
{
    public class PasswordRequestResult
    {
        public bool IsSuccessful { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
