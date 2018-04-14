using System;

namespace Davinci.Api.Models
{
    public class AuthenticationModel : BaseApiModel
    {
        public string token { get; set; }
        public string tokenExpire { get; set; }

        public DateTime getExpiration
        {
            get
            {
                return DateTime.Now.AddSeconds(double.Parse(tokenExpire));
            }
        }
    }
}