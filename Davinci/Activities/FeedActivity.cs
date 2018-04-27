using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

using Davinci.Fragments.Feed;

namespace Davinci.Activities
{
    [Activity(Theme = "@style/DavinciTheme.Feed", WindowSoftInputMode = SoftInput.AdjustPan, NoHistory = false)]
    public class FeedActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.FeedActivity);

            Toolbar toolBar = FindViewById<Toolbar>(Resource.Id.actionBar);
            toolBar.FindViewById<TextView>(Resource.Id.actionBar_title).Text = "Davinci";
            SetSupportActionBar(toolBar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            ShowFragment(new FeedFragment());

            ImageButton settingsButton = FindViewById<ImageButton>(Resource.Id.Feed_settingsButton);
            ImageButton uploadButton = FindViewById<ImageButton>(Resource.Id.Feed_uploadButton);

            settingsButton.Click += (s, e) => Settings();
            uploadButton.Click += (s, e) => Upload();
        }
        private void Settings()
        {
            StartActivity(new Intent(Application.Context, typeof(SettingsActivity)));
        }
        private void Upload()
        {
            StartActivity(new Intent(Application.Context, typeof(UploadActivity)));
        }
    }
}