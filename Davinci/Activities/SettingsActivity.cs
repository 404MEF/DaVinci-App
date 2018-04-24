using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;

namespace Davinci
{
    [Activity(Theme = "@style/DavinciTheme.Settings", WindowSoftInputMode = SoftInput.AdjustPan)]
    public class SettingsActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);

            SetContentView(Resource.Layout.SettingsActivity);

            Button logoutBtn = FindViewById<Button>(Resource.Id.Settings_logoutButton);

            logoutBtn.Click += (s, e) => Logout();
        }

        private void Logout()
        {
            var prefsEdit = PreferenceManager.GetDefaultSharedPreferences(Application.Context).Edit();
            prefsEdit.Remove(GetString(Resource.String.username));
            prefsEdit.Remove(GetString(Resource.String.userpassword));
            prefsEdit.Apply();
            prefsEdit.Dispose();

            Token.value = string.Empty;

            StartActivity(new Intent(Application.Context, typeof(AccountActivity)));
            this.Finish();
        }
    }
}