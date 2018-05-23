using System.Threading.Tasks;

using Android.App;
using Android.OS;
using Android.Content;
using Android.Support.V7.App;
using Android.Preferences;

namespace Davinci.Activities
{
    [Activity(Theme = "@style/DavinciTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            start();
        }

        void start()
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            var username = prefs.GetString(GetString(Resource.String.username), string.Empty);
            var userpassword = prefs.GetString(GetString(Resource.String.userpassword), string.Empty);

            prefs.Dispose();

            if (string.IsNullOrEmpty(userpassword))
            {
                StartActivity(new Intent(Application.Context, typeof(AccountActivity)));
                this.Finish();
            }
            else
            {
                Task.Run(async () =>
                {
                    return await Api.DavinciApi.Authenticate(username, userpassword);
                }).ContinueWith(t =>
                {
                    if (t.Status == TaskStatus.Canceled)
                        StartActivity(new Intent(Application.Context, typeof(AccountActivity)));
                    else if (t.Result.OK)
                        StartActivity(new Intent(Application.Context, typeof(FeedActivity)));
                    else
                        StartActivity(new Intent(Application.Context, typeof(AccountActivity)));
                },TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
    }
}

