using Android.Content;
using Android.Net;
using System.Linq;

namespace Davinci.Helper
{
    public static class HelperFunctions
    {
        public static bool isNetworkAvailable(Context context)
        {
            var connectivityService = context.GetSystemService(Context.ConnectivityService) as ConnectivityManager;
            var networkInfo = connectivityService.ActiveNetworkInfo;
            if (networkInfo == null || networkInfo.IsConnectedOrConnecting == false)
                return true;
            else
                return false;
        }

        public static string Capitalize(this string text)
        {
           return text.First().ToString().ToUpper() + text.Substring(1);
        }
    }
}