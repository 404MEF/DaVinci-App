using Android.App;
using Android.OS;
using Android.Views;

namespace Davinci
{
    [Activity(Theme = "@style/DavinciTheme.Settings", WindowSoftInputMode = SoftInput.AdjustPan)]
    public class SettingsActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SettingsActivity);
        }
    }
}