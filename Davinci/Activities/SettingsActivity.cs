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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Settings);

            SetActionBar("Setings");

            FindViewById<Button>(Resource.Id.Settings_logoutButton).Click += (s, e) => logout();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.category_toolbar_menu, menu);
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
    }
}