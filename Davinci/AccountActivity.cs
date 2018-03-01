using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Davinci.Fragments.Account;

namespace Davinci
{
    [Activity(Theme = "@style/DavinciTheme.Login", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = Android.Views.SoftInput.AdjustPan, NoHistory = false)]
    public class AccountActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(WindowManagerFlags.Fullscreen);

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AccountActivity);

            ShowFragment(new LoginFragment());

        }
    }
}