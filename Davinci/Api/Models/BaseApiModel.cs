namespace Davinci.Api.Models
{
    public class BaseApiModel
    {
        public string result { get; set; }
        public string message { get; set; }

        public bool OK
        {
            get
            {
                return result == "ok";
            }
        }
    }
}