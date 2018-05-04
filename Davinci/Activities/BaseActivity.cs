using Android.Content;
using Android.OS;

using Android.Support.V7.App;

using Calligraphy;
using Davinci.Fragments;
using HockeyApp.Android;
using System.Collections.Generic;

namespace Davinci
{
    public abstract class BaseActivity : AppCompatActivity
    {
        protected BaseFragment currentFragment;
        protected Stack<BaseFragment> fragmentStack;

        protected override void AttachBaseContext(Context newBase)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(newBase));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CrashManager.Register(this, "0e2e9be8322c5ab929e41d776bd6d1d5", new AutoCrashManagerListener() { });

        }

        public override void OnBackPressed()
        {
            if (SupportFragmentManager.BackStackEntryCount > 0)
            {
                SupportFragmentManager.PopBackStack();
                currentFragment = fragmentStack.Pop();

            }
            else
            {
                base.OnBackPressed();
            }
        }

        protected virtual void initializeFragments()
        {
            fragmentStack = new Stack<BaseFragment>();
        }

        public virtual void ShowFragment(BaseFragment fragment, bool addToStack = true)
        {
            var trans = SupportFragmentManager.BeginTransaction();

            trans.Hide(currentFragment);
            trans.Show(fragment);
            if (addToStack)
                trans.AddToBackStack(null);
            trans.Commit();

            fragmentStack.Push(currentFragment);
            currentFragment = fragment;
        }

        public void ClearStack()
        {
            for (int i = 0; i < SupportFragmentManager.BackStackEntryCount; ++i)
                SupportFragmentManager.PopBackStack();
        }
    }
}