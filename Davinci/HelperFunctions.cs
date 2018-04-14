using Android.Content;
using Android.Net;

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
    }
}