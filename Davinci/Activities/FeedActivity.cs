using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

using Davinci.Fragments;
using Davinci.Fragments.Feed;
using Android.Support.Design.Widget;
using Com.Materialsearchview;

namespace Davinci.Activities
{
    [Activity(Theme = "@style/DavinciTheme.Feed", WindowSoftInputMode = SoftInput.AdjustPan, NoHistory = false,ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class FeedActivity : ToolbarActivity
    {
        private BaseFragment feedFragment, searchFragment,profileFragment;
        BottomNavigationView bottomNavigation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Feed);
            SetActionBar("Davinci");
           
            initializeFragments();
            ShowFragment(feedFragment,false);

            bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            bottomNavigation.SelectedItemId = Resource.Id.menu_home;

            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.feed_toolbar_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_settings:
                    Settings();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            showFragment(e.Item.ItemId);
        }

        private void showFragment(int itemId)
        {
            BaseFragment nextFragment = null;

            switch (itemId)
            {
                case Resource.Id.menu_home:
                    nextFragment = feedFragment;
                    break;
                case Resource.Id.menu_search:
                    nextFragment = searchFragment;
                    break;
                case Resource.Id.menu_settings:
                    nextFragment = profileFragment;
                    break;
            }

            if (nextFragment == null)
                return;

            ClearStack();
            ShowFragment(nextFragment,false);
        }

        protected override void initializeFragments()
        {
            base.initializeFragments();

            feedFragment = new FeedFragment();
            searchFragment = new SearchFragment();
            profileFragment = new ProfileFragment();

            var trans = SupportFragmentManager.BeginTransaction();

            trans.Add(Resource.Id.fragmentContainer, searchFragment, "search");
            trans.Hide(searchFragment);

            trans.Add(Resource.Id.fragmentContainer, profileFragment, "profile");
            trans.Hide(profileFragment);

            trans.Add(Resource.Id.fragmentContainer, feedFragment, "feed");

            currentFragment = feedFragment;

            trans.Commit();
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