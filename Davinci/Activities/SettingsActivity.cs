using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;

namespace Davinci.Activities
{
    [Activity(Theme = "@style/DavinciTheme.Settings", WindowSoftInputMode = SoftInput.AdjustPan)]
    public class SettingsActivity : ToolbarActivity
    {
        Button logoutBtn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Settings);
            SetActionBar("Settings");

            setUI();
            setEvents();
        }

        private void setUI()
        {
            logoutBtn = FindViewById<Button>(Resource.Id.Settings_logoutButton);
        }

        private void setEvents()
        {
            logoutBtn.Click += (s, e) => logout();
        }

        private void logout()
        {
            var prefsEdit = PreferenceManager.GetDefaultSharedPreferences(Application.Context).Edit();
            prefsEdit.Remove(GetString(Resource.String.userpassword));
            prefsEdit.Apply();
            prefsEdit.Dispose();

            Token.value = string.Empty;

            StartActivity(new Intent(Application.Context, typeof(AccountActivity)));
            this.Finish();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.activity_toolbar_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_close:
                    this.Finish();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}