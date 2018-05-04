using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

using Davinci.Fragments;
using Davinci.Fragments.Feed;

namespace Davinci.Activities
{
    [Activity(Theme = "@style/DavinciTheme.Feed", WindowSoftInputMode = SoftInput.AdjustPan, NoHistory = false,ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class FeedActivity : ToolbarActivity
    {
        private BaseFragment feedFragment, searchFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Feed);

            SetActionBar("Davinci");

            initializeFragments();
            ShowFragment(feedFragment);

            Button settingsButton = FindViewById<Button>(Resource.Id.Feed_settingsButton);
            Button uploadButton = FindViewById<Button>(Resource.Id.Feed_uploadButton);
            Button searchBtn = FindViewById<Button>(Resource.Id.Feed_searchBtn);

            settingsButton.Click += (s, e) => Settings();
            uploadButton.Click += (s, e) => Upload();
            searchBtn.Click += (s, e) => Search();
        }

        protected override void initializeFragments()
        {
            base.initializeFragments();

            feedFragment = new FeedFragment();
            searchFragment = new SearchFragment();

            var trans = SupportFragmentManager.BeginTransaction();

            trans.Add(Resource.Id.fragmentContainer, searchFragment, "search");
            trans.Hide(searchFragment);

            trans.Add(Resource.Id.fragmentContainer, feedFragment, "feed");

            currentFragment = feedFragment;

            trans.Commit();
        }

        private void Search()
        {
            if (currentFragment != searchFragment)
            {
                ShowFragment(searchFragment);
                //TODO Set Tint
            }
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