using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

using Davinci.Fragments;

using Calligraphy;
using System;

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
            if (currentFragment == fragment)
                return;

            var trans = SupportFragmentManager.BeginTransaction();
            trans.SetTransition((int)FragmentTransit.FragmentFade);
            trans.Hide(currentFragment);
            trans.Show(fragment);
            if (addToStack)
                trans.AddToBackStack(null);
            trans.Commit();

            fragmentStack.Push(currentFragment);
            currentFragment = fragment;

            fragment.OnViewTrigger();
        }

        public void ClearStack()
        {
            for (int i = 0; i < SupportFragmentManager.BackStackEntryCount; ++i)
                SupportFragmentManager.PopBackStack();
        }

    }
}