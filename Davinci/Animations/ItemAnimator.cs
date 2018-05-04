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

namespace Davinci.Animations
{
    class ItemAnimator : RecyclerView.ItemAnimator
    {
        public override bool IsRunning
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool AnimateAppearance(RecyclerView.ViewHolder viewHolder, ItemHolderInfo preLayoutInfo, ItemHolderInfo postLayoutInfo)
        {
            throw new NotImplementedException();
        }

        public override bool AnimateChange(RecyclerView.ViewHolder oldHolder, RecyclerView.ViewHolder newHolder, ItemHolderInfo preInfo, ItemHolderInfo postInfo)
        {
            throw new NotImplementedException();
        }

        public override bool AnimateDisappearance(RecyclerView.ViewHolder viewHolder, ItemHolderInfo preLayoutInfo, ItemHolderInfo postLayoutInfo)
        {
            throw new NotImplementedException();
        }

        public override bool AnimatePersistence(RecyclerView.ViewHolder viewHolder, ItemHolderInfo preInfo, ItemHolderInfo postInfo)
        {
            throw new NotImplementedException();
        }

        public override void EndAnimation(RecyclerView.ViewHolder item)
        {
            throw new NotImplementedException();
        }

        public override void EndAnimations()
        {
            throw new NotImplementedException();
        }

        public override void RunPendingAnimations()
        {
            throw new NotImplementedException();
        }
    }
}