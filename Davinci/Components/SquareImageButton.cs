using System;

using Android.Content;
using Android.Runtime;
using Android.Widget;
using Android.Util;

namespace Davinci.Components
{
    class SquareImageButton : ImageButton
    {
        public SquareImageButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public SquareImageButton(Context context) : base(context)
        {
        }

        protected SquareImageButton(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(heightMeasureSpec, widthMeasureSpec);
        }
    }
}