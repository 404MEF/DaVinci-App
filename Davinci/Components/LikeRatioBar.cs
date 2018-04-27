using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Views.Animations;

namespace Davinci.Components
{
    public class LikeRatioBar : FrameLayout
    {
        Context mContext;
        //Progress Bar
        public ProgressBar progressBar { get; private set; }
        //Primary text
        public TextView primaryTextView { get; private set; }
        //Secondary text
        public TextView secondaryTextView { get; private set; }

        public LikeRatioBar(Context context) : base(context)
        {
           // CreateView(context);
        }

        public LikeRatioBar(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.mContext = context;
            //CreateView(context);

        }

        public LikeRatioBar(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            this.mContext = context;

            //CreateView(context);

        }

        public LikeRatioBar(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            this.mContext = context;

            //CreateView(context);

        }


        public void CreateView()
        {
            LayoutInflater inflater = (LayoutInflater)mContext.GetSystemService(Context.LayoutInflaterService);

            var view = inflater.Inflate(Resource.Layout.RatioBar,this);

            progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);
            primaryTextView = view.FindViewById<TextView>(Resource.Id.primaryText);
            secondaryTextView = view.FindViewById<TextView>(Resource.Id.secondaryText);
        }

        public void SetRatio(int progress)
        {
            ProgressBarAnimation anim = new ProgressBarAnimation(this, this.progressBar.Progress, progress);
            anim.Duration = 1000;
            this.StartAnimation(anim);
        }
    }

    public class ProgressBarAnimation : Animation
    {
        private LikeRatioBar frd;
        private float from;
        private float to;

        public ProgressBarAnimation(LikeRatioBar ratioBar, float from, float to) : base()
        {
            this.frd = ratioBar;
            this.from = from;
            this.to = to;
        }
        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            base.ApplyTransformation(interpolatedTime, t);
            int primaryValue = (int)(from + (to - from) * interpolatedTime);
            int secondaryValue = (int)((100 - from) - ((100 - from) - to) * interpolatedTime);

            frd.progressBar.Progress = primaryValue;
            frd.primaryTextView.Text = string.Format("%{0}", primaryValue);
            frd.secondaryTextView.Text = string.Format("%{0}", 100 - primaryValue);
        }

    }
}