
using Android.App;
using Android.OS;
using Android.Views;
namespace Davinci
{
    [Activity(Theme = "@style/DavinciTheme.Feed", WindowSoftInputMode = SoftInput.AdjustPan, NoHistory = true)]
    public class FeedActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //SetContentView(Resource.Layout.AccountActivity);
        }
    }
}