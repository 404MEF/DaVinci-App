using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Davinci.Activities
{
    public class ToolbarActivity : BaseActivity
    {
        public Toolbar ToolBar { get; private set; }

        protected void SetActionBar(string title = "Davinci")
        {
            ToolBar = FindViewById<Toolbar>(Resource.Id.actionBar);
            ToolBar.FindViewById<TextView>(Resource.Id.actionBarTitle).Text = title;
            SetSupportActionBar(ToolBar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
        }

    }
}