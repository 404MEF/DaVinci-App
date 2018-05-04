using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Davinci.Activities
{
    public class ToolbarActivity : BaseActivity
    {
        public Toolbar ToolBar { get; private set; }

        protected void SetActionBar(string title)
        {
            ToolBar = FindViewById<Toolbar>(Resource.Id.actionBar);
            ToolBar.Title = title;
            SetSupportActionBar(ToolBar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
        }

        public void SetTitle(string title)
        {
            if (this.ToolBar != null)
                this.ToolBar.Title = title;
        }
    }
}