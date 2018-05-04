using Android.App;
using Android.OS;
using Android.Views;
using Davinci.Fragments;
using Davinci.Fragments.Account;

namespace Davinci.Activities
{
    [Activity(Theme = "@style/DavinciTheme.Login", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustPan, NoHistory = true)]
    public class AccountActivity : BaseActivity
    {
        private BaseFragment loginFragment,registerFragment,resetFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //Remove status bar background
            Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);

            SetContentView(Resource.Layout.Account);

            initializeFragments();

            ShowFragment(loginFragment);
        }

        protected override void initializeFragments()
        {
            base.initializeFragments();

            loginFragment = new LoginFragment();
            registerFragment = new RegisterFragment();
            resetFragment = new ResetPasswordFragment();

            var trans = SupportFragmentManager.BeginTransaction();

            trans.Add(Resource.Id.fragmentContainer, registerFragment, "register");
            trans.Hide(registerFragment);

            trans.Add(Resource.Id.fragmentContainer, resetFragment, "reset");
            trans.Hide(resetFragment);

            trans.Add(Resource.Id.fragmentContainer, loginFragment, "login");

            currentFragment = loginFragment;

            trans.Commit();
        }

        public void Login()
        {
            ShowFragment(loginFragment);
        }

        public void Register()
        {
            ShowFragment(registerFragment);
        }

        public void Reset()
        {
            ShowFragment(resetFragment);
        }
    }
}