using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Davinci.Activities
{
    [Activity(Theme = "@style/DavinciTheme.Settings", WindowSoftInputMode = SoftInput.AdjustPan)]
    public class SettingsActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SettingsActivity);

            Toolbar toolBar = FindViewById<Toolbar>(Resource.Id.actionBar);
            toolBar.FindViewById<TextView>(Resource.Id.actionBar_title).Text = "Settings";
            SetSupportActionBar(toolBar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

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