using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Support.V7.App;

namespace Davinci.Fragments
{
    public abstract class BaseFragment : Fragment
    {
        protected BaseActivity mActivity;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            mActivity = (BaseActivity)Activity;

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        protected void Close()
        {
            mActivity.OnBackPressed();
        }
    }
}