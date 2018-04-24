using Android.Content;
using Android.OS;

using Android.Support.V7.App;

using Calligraphy;

namespace Davinci
{
    public class BaseActivity : AppCompatActivity
    {
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
            if (SupportFragmentManager.BackStackEntryCount == 1)
                this.Finish();

            base.OnBackPressed();
        }

        public void ShowFragment(Android.Support.V4.App.Fragment fragment, bool Animate = true,
            int Animation_In = Resource.Animation.slide_from_right,
            int Animation_Out = Resource.Animation.slide_to_left,
            int PopAnimation_In = Resource.Animation.slide_from_left,
            int PopAnimation_Out = Resource.Animation.slide_to_right,
            bool AddToStack = true)
        {
            var trans = SupportFragmentManager.BeginTransaction();

            if (Animate)
                trans.SetCustomAnimations(Animation_In, Animation_Out, PopAnimation_In, PopAnimation_Out);

            trans.Replace(Resource.Id.fragmentContainer, fragment);

            if (AddToStack)
                trans.AddToBackStack(null);

            trans.Commit();
        }

        public void ClearStack()
        {
            for (int i = 0; i < SupportFragmentManager.BackStackEntryCount; ++i)
                SupportFragmentManager.PopBackStack();
        }
    }
}