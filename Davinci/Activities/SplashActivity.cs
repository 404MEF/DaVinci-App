using System.Threading.Tasks;

using Android.App;
using Android.OS;
using Android.Content;
using Android.Support.V7.App;
using Android.Preferences;
using HockeyApp.Android;

namespace Davinci.Activities
{
    [Activity(Theme = "@style/DavinciTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CrashManager.Register(this, "0e2e9be8322c5ab929e41d776bd6d1d5", new AutoCrashManagerListener() { });
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
                    var auth = await Api.DavinciApi.Authenticate(username, userpassword);
                    return auth;
                }).ContinueWith(authTask =>
                {
                    var auth = authTask.Result;

                    if (auth.OK)
                        StartActivity(new Intent(Application.Context, typeof(FeedActivity)));
                    else
                        StartActivity(new Intent(Application.Context, typeof(AccountActivity)));
                },TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
    }
}

