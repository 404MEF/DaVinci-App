using Android.Content;
using Android.Views;
using Android.Widget;

namespace Davinci
{
    public class Infobar
    {
        public enum InfoLevel
        {
            Info = 0,
            Warning = 1,
            Error = 2
        }

        public static void Show(Context context, string text, InfoLevel infoLevel, GravityFlags gravity,bool actionBar = true)
        {
            LayoutInflater inflater = context.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;

            View layout = inflater.Inflate(Resource.Layout.Infobar,null);

            //if (!actionBar)
            //{
            //    var layoutParams = layout.FindViewById<RelativeLayout>(Resource.Id.Infobar_root).LayoutParameters as RelativeLayout.LayoutParams;
            //    layoutParams.SetMargins(0, 0, 0, 0);
            //}

            TextView textView = layout.FindViewById<TextView>(Resource.Id.Infobar_text);
            textView.Text = text;

            if (infoLevel == InfoLevel.Error)
            {
                textView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_warning_black_24dp,0,0,0);
            }
            else if (infoLevel == InfoLevel.Info)
            {
                textView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_error_black_24dp, 0, 0, 0);
                textView.SetTextColor(Android.Graphics.Color.Black);
            }
            else // infoLevel == InfoLevel.Warning
            {
                textView.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.ic_error_black_24dp, 0, 0, 0);
                textView.SetTextColor(Android.Graphics.Color.Black);
            }

            Toast toast = new Toast(context);
            toast.SetGravity(gravity, 0, 0);
            toast.View = layout;
            toast.Duration = ToastLength.Short;
            toast.Show();
        }
    }
}