using System.Threading.Tasks;

using Android.App;
using Android.OS;
using Android.Content;
using Android.Support.V7.App;

namespace Davinci
{
    [Activity(Theme = "@style/DavinciTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        private const bool EMULATOR = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task.Run(() => { SimulateStartup(); });
        }

        async void SimulateStartup()
        {
            await Task.Delay(500);
            StartActivity(new Intent(Application.Context, typeof(AccountActivity)));
        }
    }
}

