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
using Android.Support.V7.Widget;
using Android.Views.Animations;
using Android.Animation;
using Java.Lang;

namespace Davinci.Animations
{
    class FeedPostAnimator : ItemAnimator
    {
        private static DecelerateInterpolator DECCELERATE_INTERPOLATOR = new DecelerateInterpolator();
        private static AccelerateInterpolator ACCELERATE_INTERPOLATOR = new AccelerateInterpolator();
        private static OvershootInterpolator OVERSHOOT_INTERPOLATOR = new OvershootInterpolator(4);

        Dictionary<RecyclerView.ViewHolder, AnimatorSet> ratioAnimationDict = new Dictionary<RecyclerView.ViewHolder, AnimatorSet>();

        public override bool CanReuseUpdatedViewHolder(RecyclerView.ViewHolder viewHolder)
        {
            return true;
        }

        public override ItemHolderInfo RecordPreLayoutInformation(RecyclerView.State state, RecyclerView.ViewHolder viewHolder, int changeFlags, IList<Java.Lang.Object> payloads)
        {
            //FLAG_CHANGED
            if (changeFlags == 0)
            {
                foreach (var payload in payloads)
                {
                    if (payload is Java.Lang.String)
                    {
                        //return new FeedItemHolderInfo(payload.ToString());
                        return null;
                    }
                }
            }

            return base.RecordPreLayoutInformation(state, viewHolder, changeFlags, payloads);
        }



    }
}