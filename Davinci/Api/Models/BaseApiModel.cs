namespace Davinci.Api.Models
{
    public class BaseApiModel
    {
        public string result { get; set; }
        public string message { get; set; }

        public bool isOK
        {
            get
            {
                return result == "ok";
            }
        }
    }
}