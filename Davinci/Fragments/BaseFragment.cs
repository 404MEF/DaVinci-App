using Android.OS;
using Android.Support.V4.App;
using Android.Views;

namespace Davinci.Fragments
{
    public abstract class BaseFragment : Fragment
    {
        protected BaseActivity parentActivity;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            parentActivity = (BaseActivity)Activity;

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        protected void RunOnUIThread(System.Action action)
        {
            parentActivity.RunOnUiThread(action);
        }

        protected void Close()
        {
            parentActivity.OnBackPressed();
        }
    }
}