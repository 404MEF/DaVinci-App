using Android.App;
using Android.OS;
using Android.Views;

using Davinci.Fragments.Account;

namespace Davinci
{
    [Activity(Theme = "@style/DavinciTheme.Login", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustPan, NoHistory = false)]
    public class AccountActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);

            SetContentView(Resource.Layout.AccountActivity);

            ShowFragment(new LoginFragment());
        }
    }
}